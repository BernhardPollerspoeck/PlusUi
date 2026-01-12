using System.Runtime.Versioning;
using Android.Content;
using Android.OS;
using PlusUi.core;

namespace PlusUi.droid;

public class AndroidHapticService(Context context) : IHapticService
{
    private readonly Vibrator? _vibrator = GetVibrator(context);

    private static Vibrator? GetVibrator(Context context)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            var vibratorManager = context.GetSystemService(Context.VibratorManagerService) as VibratorManager;
            return vibratorManager?.DefaultVibrator;
        }

        return context.GetSystemService(Context.VibratorService) as Vibrator;
    }

    public bool IsSupported => _vibrator?.HasVibrator == true;

    public void Emit(HapticFeedback feedback)
    {
        if (!IsSupported || _vibrator is null)
        {
            return;
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            EmitWithVibrationEffect(feedback);
        }
        else
        {
            EmitLegacy(feedback);
        }
    }

    [SupportedOSPlatform("android29.0")]
    private void EmitWithVibrationEffect(HapticFeedback feedback)
    {
        if (_vibrator is null) return;

        var effect = feedback switch
        {
            HapticFeedback.Light => VibrationEffect.CreatePredefined(VibrationEffect.EffectTick),
            HapticFeedback.Medium => VibrationEffect.CreatePredefined(VibrationEffect.EffectClick),
            HapticFeedback.Heavy => VibrationEffect.CreatePredefined(VibrationEffect.EffectHeavyClick),
            HapticFeedback.Success => VibrationEffect.CreatePredefined(VibrationEffect.EffectDoubleClick),
            HapticFeedback.Warning => VibrationEffect.CreatePredefined(VibrationEffect.EffectClick),
            HapticFeedback.Error => VibrationEffect.CreatePredefined(VibrationEffect.EffectHeavyClick),
            HapticFeedback.Selection => VibrationEffect.CreatePredefined(VibrationEffect.EffectTick),
            _ => VibrationEffect.CreatePredefined(VibrationEffect.EffectClick)
        };

        if (effect is not null)
        {
            _vibrator.Vibrate(effect);
        }
    }

    private void EmitLegacy(HapticFeedback feedback)
    {
        if (_vibrator is null) return;

        var durationMs = feedback switch
        {
            HapticFeedback.Light => 10,
            HapticFeedback.Medium => 20,
            HapticFeedback.Heavy => 50,
            HapticFeedback.Success => 30,
            HapticFeedback.Warning => 30,
            HapticFeedback.Error => 50,
            HapticFeedback.Selection => 10,
            _ => 20
        };

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var effect = VibrationEffect.CreateOneShot(durationMs, VibrationEffect.DefaultAmplitude);
            if (effect is not null)
            {
                _vibrator.Vibrate(effect);
            }
        }
        else
        {
            _vibrator.Vibrate(durationMs);
        }
    }
}
