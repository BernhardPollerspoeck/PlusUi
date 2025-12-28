using Android.Content;
using Android.OS;
using PlusUi.core;

namespace PlusUi.droid;

public class AndroidHapticService : IHapticService
{
    private readonly Vibrator? _vibrator;

    public bool IsSupported => _vibrator?.HasVibrator == true;

    public AndroidHapticService(Context context)
    {
        _vibrator = context.GetSystemService(Context.VibratorService) as Vibrator;
    }

    public void Emit(HapticFeedback feedback)
    {
        if (!IsSupported || _vibrator is null)
        {
            return;
        }

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            EmitWithVibrationEffect(feedback);
        }
        else
        {
            EmitLegacy(feedback);
        }
    }

    private void EmitWithVibrationEffect(HapticFeedback feedback)
    {
        if (_vibrator is null) return;

#pragma warning disable CA1416 // Validate platform compatibility
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
#pragma warning restore CA1416 // Validate platform compatibility
    }

    private void EmitLegacy(HapticFeedback feedback)
    {
        if (_vibrator is null) return;

#pragma warning disable CS0618 // Type or member is obsolete
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

        _vibrator.Vibrate(durationMs);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
