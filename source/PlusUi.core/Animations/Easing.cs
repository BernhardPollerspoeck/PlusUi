namespace PlusUi.core.Animations;

public enum Easing
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut
}

public static class EasingFunctions
{
    public static float Apply(Easing easing, float t) => easing switch
    {
        Easing.Linear => t,
        Easing.EaseIn => t * t,
        Easing.EaseOut => 1f - (1f - t) * (1f - t),
        Easing.EaseInOut => t < 0.5f
            ? 2f * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f,
        _ => t
    };
}
