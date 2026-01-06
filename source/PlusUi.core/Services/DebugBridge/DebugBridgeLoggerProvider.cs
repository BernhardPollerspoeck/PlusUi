using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core.Services.DebugBridge.Models;

namespace PlusUi.core.Services.DebugBridge;

/// <summary>
/// Logger provider that sends log messages to the debug server via WebSocket.
/// Queues messages and sends them in batches to avoid overwhelming the connection.
/// </summary>
internal class DebugBridgeLoggerProvider : ILoggerProvider
{
    private static readonly ConcurrentQueue<LogMessageDto> _logQueue = new();
    private static Timer? _batchTimer;
    private static DebugBridgeClient? _client;
    private static int _connectAttempts = 0;
    private static readonly object _lock = new();
    private const int MaxConnectAttempts = 10;
    private const int BatchIntervalMs = 500;
    private const int MaxBatchSize = 50;

    public DebugBridgeLoggerProvider()
    {
        // Start batch processing timer if not already started
        if (_batchTimer == null)
        {
            lock (_lock)
            {
                if (_batchTimer == null)
                {
                    _batchTimer = new Timer(ProcessLogQueue, null, BatchIntervalMs, BatchIntervalMs);
                }
            }
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DebugBridgeLogger(categoryName);
    }

    private static void ProcessLogQueue(object? state)
    {
        // Try to get client if we don't have it yet
        if (_client == null && _connectAttempts < MaxConnectAttempts)
        {
            lock (_lock)
            {
                if (_client == null && _connectAttempts < MaxConnectAttempts)
                {
                    _connectAttempts++;
                    _client = ServiceProviderService.ServiceProvider?.GetService<DebugBridgeClient>();
                }
            }
        }

        // If still no client, skip this batch
        if (_client == null)
            return;

        // Process batch
        var batch = new List<LogMessageDto>();
        while (batch.Count < MaxBatchSize && _logQueue.TryDequeue(out var log))
        {
            batch.Add(log);
        }

        if (batch.Count > 0)
        {
            _ = _client.SendLogBatchAsync(batch);
        }
    }

    public void Dispose()
    {
        _batchTimer?.Dispose();
        _batchTimer = null;
    }

    private class DebugBridgeLogger : ILogger
    {
        private readonly string _categoryName;

        public DebugBridgeLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);

            var logDto = new LogMessageDto
            {
                Level = logLevel,
                Timestamp = DateTime.Now,
                Message = message,
                Category = _categoryName,
                Exception = exception?.ToString(),
                EventId = eventId.Id != 0 ? eventId.Id : null
            };

            // Queue log for batch processing
            _logQueue.Enqueue(logDto);
        }
    }
}
