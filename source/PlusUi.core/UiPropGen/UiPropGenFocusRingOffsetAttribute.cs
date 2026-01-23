namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFocusRingOffsetAttribute : Attribute
{
    public const string Template = """
        private float _generatedFocusRingOffset;

        internal float FocusRingOffset
        {
            get => _generatedFocusRingOffset;
            set
            {
                if (_generatedFocusRingOffset.Equals(value)) return;
                _generatedFocusRingOffset = value;
            }
        }

        public {ClassName} SetFocusRingOffset(float value)
        {
            FocusRingOffset = value;
            return this;
        }

        public {ClassName} BindFocusRingOffset(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusRingOffset = getter());
            return this;
        }

        """;
}
