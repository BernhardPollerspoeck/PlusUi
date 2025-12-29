using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusUi.core;

namespace Sandbox.Pages.UniformGridDemo;

public partial class UniformGridDemoPageViewModel(INavigationService navigationService) : ObservableObject
{
    // Image sources for the two players
    public const string PlusUiLogo = "plusui.png"; // Embedded resource
    public const string CSharpLogo = "https://upload.wikimedia.org/wikipedia/commons/4/4f/Csharp_Logo.png"; // Web image

    [RelayCommand]
    private void GoBack() => navigationService.GoBack();

    // Game state: 0 = empty, 1 = PlusUi, 2 = C#
    private readonly int[] _board = new int[9];
    private bool _isPlusUiTurn = true;
    private bool _gameOver;

    [ObservableProperty]
    private string _currentPlayerImage = PlusUiLogo;

    [ObservableProperty]
    private string _statusText = "PlusUi's turn";

    // Cell image sources (empty string = no image)
    [ObservableProperty]
    private string? _cell0Image;
    [ObservableProperty]
    private string? _cell1Image;
    [ObservableProperty]
    private string? _cell2Image;
    [ObservableProperty]
    private string? _cell3Image;
    [ObservableProperty]
    private string? _cell4Image;
    [ObservableProperty]
    private string? _cell5Image;
    [ObservableProperty]
    private string? _cell6Image;
    [ObservableProperty]
    private string? _cell7Image;
    [ObservableProperty]
    private string? _cell8Image;

    [RelayCommand]
    private void CellClick(int cellIndex)
    {
        if (_gameOver || _board[cellIndex] != 0)
            return;

        // Make move: 1 = PlusUi, 2 = C#
        _board[cellIndex] = _isPlusUiTurn ? 1 : 2;
        var imageSource = _isPlusUiTurn ? PlusUiLogo : CSharpLogo;
        UpdateCellImage(cellIndex, imageSource);

        // Check for winner
        var winner = CheckWinner();
        if (winner != 0)
        {
            _gameOver = true;
            StatusText = winner == 1 ? "PlusUi wins!" : "C# wins!";
            return;
        }

        // Check for draw
        if (_board.All(c => c != 0))
        {
            _gameOver = true;
            StatusText = "It's a draw!";
            return;
        }

        // Switch turns
        _isPlusUiTurn = !_isPlusUiTurn;
        CurrentPlayerImage = _isPlusUiTurn ? PlusUiLogo : CSharpLogo;
        StatusText = _isPlusUiTurn ? "PlusUi's turn" : "C#'s turn";
    }

    [RelayCommand]
    private void ResetGame()
    {
        for (var i = 0; i < 9; i++)
        {
            _board[i] = 0;
            UpdateCellImage(i, null);
        }
        _isPlusUiTurn = true;
        _gameOver = false;
        CurrentPlayerImage = PlusUiLogo;
        StatusText = "PlusUi's turn";
    }

    private void UpdateCellImage(int index, string? imageSource)
    {
        switch (index)
        {
            case 0: Cell0Image = imageSource; break;
            case 1: Cell1Image = imageSource; break;
            case 2: Cell2Image = imageSource; break;
            case 3: Cell3Image = imageSource; break;
            case 4: Cell4Image = imageSource; break;
            case 5: Cell5Image = imageSource; break;
            case 6: Cell6Image = imageSource; break;
            case 7: Cell7Image = imageSource; break;
            case 8: Cell8Image = imageSource; break;
        }
    }

    private int CheckWinner()
    {
        // Winning combinations
        int[][] winPatterns =
        [
            [0, 1, 2], // Top row
            [3, 4, 5], // Middle row
            [6, 7, 8], // Bottom row
            [0, 3, 6], // Left column
            [1, 4, 7], // Middle column
            [2, 5, 8], // Right column
            [0, 4, 8], // Diagonal
            [2, 4, 6]  // Anti-diagonal
        ];

        foreach (var pattern in winPatterns)
        {
            var a = _board[pattern[0]];
            var b = _board[pattern[1]];
            var c = _board[pattern[2]];

            if (a != 0 && a == b && b == c)
            {
                return a;
            }
        }

        return 0;
    }
}
