using PlusUi.core;
using PlusUi.core.Animations;

namespace UiPlus.core.Tests;

[TestClass]
public class TransitionTests
{
    #region Easing Tests

    [TestMethod]
    public void EasingFunctions_Linear_ReturnsCorrectValues()
    {
        Assert.AreEqual(0f, EasingFunctions.Apply(Easing.Linear, 0f), 0.001f);
        Assert.AreEqual(0.5f, EasingFunctions.Apply(Easing.Linear, 0.5f), 0.001f);
        Assert.AreEqual(1f, EasingFunctions.Apply(Easing.Linear, 1f), 0.001f);
    }

    [TestMethod]
    public void EasingFunctions_EaseIn_ReturnsCorrectValues()
    {
        Assert.AreEqual(0f, EasingFunctions.Apply(Easing.EaseIn, 0f), 0.001f);
        Assert.AreEqual(0.25f, EasingFunctions.Apply(Easing.EaseIn, 0.5f), 0.001f); // 0.5² = 0.25
        Assert.AreEqual(1f, EasingFunctions.Apply(Easing.EaseIn, 1f), 0.001f);
    }

    [TestMethod]
    public void EasingFunctions_EaseOut_ReturnsCorrectValues()
    {
        Assert.AreEqual(0f, EasingFunctions.Apply(Easing.EaseOut, 0f), 0.001f);
        Assert.AreEqual(0.75f, EasingFunctions.Apply(Easing.EaseOut, 0.5f), 0.001f); // 1 - (1-0.5)² = 0.75
        Assert.AreEqual(1f, EasingFunctions.Apply(Easing.EaseOut, 1f), 0.001f);
    }

    [TestMethod]
    public void EasingFunctions_EaseInOut_ReturnsCorrectValues()
    {
        Assert.AreEqual(0f, EasingFunctions.Apply(Easing.EaseInOut, 0f), 0.001f);
        Assert.AreEqual(0.5f, EasingFunctions.Apply(Easing.EaseInOut, 0.5f), 0.001f);
        Assert.AreEqual(1f, EasingFunctions.Apply(Easing.EaseInOut, 1f), 0.001f);
    }

    #endregion

    #region NoneTransition Tests

    [TestMethod]
    public void NoneTransition_DefaultDuration_IsZero()
    {
        var transition = new NoneTransition();
        Assert.AreEqual(TimeSpan.Zero, transition.Duration);
    }

    [TestMethod]
    public void NoneTransition_SetDuration_ReturnsThis()
    {
        var transition = new NoneTransition();
        var result = transition.SetDuration(TimeSpan.FromMilliseconds(100));
        Assert.AreSame(transition, result);
    }

    [TestMethod]
    public void NoneTransition_GetReversed_ReturnsSameType()
    {
        var transition = new NoneTransition()
            .SetDuration(TimeSpan.FromMilliseconds(100))
            .SetEasing(Easing.EaseIn);

        var reversed = transition.GetReversed();

        Assert.IsInstanceOfType(reversed, typeof(NoneTransition));
        Assert.AreEqual(transition.Duration, reversed.Duration);
        Assert.AreEqual(transition.Easing, reversed.Easing);
    }

    #endregion

    #region FadeTransition Tests

    [TestMethod]
    public void FadeTransition_DefaultDuration_Is250ms()
    {
        var transition = new FadeTransition();
        Assert.AreEqual(TimeSpan.FromMilliseconds(250), transition.Duration);
    }

    [TestMethod]
    public void FadeTransition_DefaultEasing_IsEaseInOut()
    {
        var transition = new FadeTransition();
        Assert.AreEqual(Easing.EaseInOut, transition.Easing);
    }

    [TestMethod]
    public void FadeTransition_SetDuration_UpdatesDuration()
    {
        var transition = new FadeTransition()
            .SetDuration(TimeSpan.FromMilliseconds(500));

        Assert.AreEqual(TimeSpan.FromMilliseconds(500), transition.Duration);
    }

    [TestMethod]
    public void FadeTransition_SetEasing_UpdatesEasing()
    {
        var transition = new FadeTransition()
            .SetEasing(Easing.EaseOut);

        Assert.AreEqual(Easing.EaseOut, transition.Easing);
    }

    [TestMethod]
    public void FadeTransition_GetReversed_ReturnsSameType()
    {
        var transition = new FadeTransition()
            .SetDuration(TimeSpan.FromMilliseconds(400))
            .SetEasing(Easing.EaseOut);

        var reversed = transition.GetReversed();

        Assert.IsInstanceOfType(reversed, typeof(FadeTransition));
        Assert.AreEqual(transition.Duration, reversed.Duration);
        Assert.AreEqual(transition.Easing, reversed.Easing);
    }

    #endregion

    #region SlideTransition Tests

    [TestMethod]
    public void SlideTransition_DefaultDuration_Is300ms()
    {
        var transition = new SlideTransition();
        Assert.AreEqual(TimeSpan.FromMilliseconds(300), transition.Duration);
    }

    [TestMethod]
    public void SlideTransition_DefaultDirection_IsLeft()
    {
        var transition = new SlideTransition();
        Assert.AreEqual(SlideDirection.Left, transition.Direction);
    }

    [TestMethod]
    public void SlideTransition_DefaultEasing_IsEaseOut()
    {
        var transition = new SlideTransition();
        Assert.AreEqual(Easing.EaseOut, transition.Easing);
    }

    [TestMethod]
    public void SlideTransition_SetDirection_UpdatesDirection()
    {
        var transition = new SlideTransition()
            .SetDirection(SlideDirection.Up);

        Assert.AreEqual(SlideDirection.Up, transition.Direction);
    }

    [TestMethod]
    public void SlideTransition_GetReversed_ReversesLeftToRight()
    {
        var transition = new SlideTransition()
            .SetDirection(SlideDirection.Left);

        var reversed = (SlideTransition)transition.GetReversed();

        Assert.AreEqual(SlideDirection.Right, reversed.Direction);
    }

    [TestMethod]
    public void SlideTransition_GetReversed_ReversesRightToLeft()
    {
        var transition = new SlideTransition()
            .SetDirection(SlideDirection.Right);

        var reversed = (SlideTransition)transition.GetReversed();

        Assert.AreEqual(SlideDirection.Left, reversed.Direction);
    }

    [TestMethod]
    public void SlideTransition_GetReversed_ReversesUpToDown()
    {
        var transition = new SlideTransition()
            .SetDirection(SlideDirection.Up);

        var reversed = (SlideTransition)transition.GetReversed();

        Assert.AreEqual(SlideDirection.Down, reversed.Direction);
    }

    [TestMethod]
    public void SlideTransition_GetReversed_ReversesDownToUp()
    {
        var transition = new SlideTransition()
            .SetDirection(SlideDirection.Down);

        var reversed = (SlideTransition)transition.GetReversed();

        Assert.AreEqual(SlideDirection.Up, reversed.Direction);
    }

    [TestMethod]
    public void SlideTransition_GetReversed_PreservesDurationAndEasing()
    {
        var transition = new SlideTransition()
            .SetDuration(TimeSpan.FromMilliseconds(500))
            .SetEasing(Easing.EaseIn)
            .SetDirection(SlideDirection.Left);

        var reversed = transition.GetReversed();

        Assert.AreEqual(transition.Duration, reversed.Duration);
        Assert.AreEqual(transition.Easing, reversed.Easing);
    }

    #endregion

    #region TransitionService Tests

    [TestMethod]
    public void TransitionService_IsTransitioning_FalseByDefault()
    {
        var service = new TransitionService();
        Assert.IsFalse(service.IsTransitioning);
    }

    [TestMethod]
    public void TransitionService_OutgoingPage_NullByDefault()
    {
        var service = new TransitionService();
        Assert.IsNull(service.OutgoingPage);
    }

    [TestMethod]
    public void TransitionService_Update_DoesNothingWhenNotTransitioning()
    {
        var service = new TransitionService();

        // Should not throw
        service.Update();

        Assert.IsFalse(service.IsTransitioning);
    }

    #endregion

    #region PlusUiConfiguration Tests

    [TestMethod]
    public void PlusUiConfiguration_DefaultTransition_IsSlideTransition()
    {
        var config = new PlusUiConfiguration();
        Assert.IsInstanceOfType(config.DefaultTransition, typeof(SlideTransition));
    }

    [TestMethod]
    public void PlusUiConfiguration_DefaultTransition_CanBeChanged()
    {
        var config = new PlusUiConfiguration
        {
            DefaultTransition = new FadeTransition()
        };

        Assert.IsInstanceOfType(config.DefaultTransition, typeof(FadeTransition));
    }

    [TestMethod]
    public void PlusUiConfiguration_DefaultTransition_CanBeSetToNone()
    {
        var config = new PlusUiConfiguration
        {
            DefaultTransition = new NoneTransition()
        };

        Assert.IsInstanceOfType(config.DefaultTransition, typeof(NoneTransition));
    }

    #endregion

    #region Method Chaining Tests

    [TestMethod]
    public void FadeTransition_MethodChaining_Works()
    {
        var transition = new FadeTransition()
            .SetDuration(TimeSpan.FromMilliseconds(400))
            .SetEasing(Easing.Linear);

        Assert.AreEqual(TimeSpan.FromMilliseconds(400), transition.Duration);
        Assert.AreEqual(Easing.Linear, transition.Easing);
    }

    [TestMethod]
    public void SlideTransition_MethodChaining_Works()
    {
        var transition = new SlideTransition()
            .SetDuration(TimeSpan.FromMilliseconds(500))
            .SetEasing(Easing.EaseInOut)
            .SetDirection(SlideDirection.Up);

        Assert.AreEqual(TimeSpan.FromMilliseconds(500), transition.Duration);
        Assert.AreEqual(Easing.EaseInOut, transition.Easing);
        Assert.AreEqual(SlideDirection.Up, transition.Direction);
    }

    [TestMethod]
    public void NoneTransition_MethodChaining_Works()
    {
        var transition = new NoneTransition()
            .SetDuration(TimeSpan.FromMilliseconds(100))
            .SetEasing(Easing.Linear);

        Assert.AreEqual(TimeSpan.FromMilliseconds(100), transition.Duration);
        Assert.AreEqual(Easing.Linear, transition.Easing);
    }

    #endregion
}
