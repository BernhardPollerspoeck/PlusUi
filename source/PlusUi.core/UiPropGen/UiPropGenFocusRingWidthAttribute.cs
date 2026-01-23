namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFocusRingWidthAttribute : Attribute
{
    public const string Template = """
        private float _generatedFocusRingWidth;

        internal float FocusRingWidth
        {
            get => _generatedFocusRingWidth;
            set
            {
                if (_generatedFocusRingWidth.Equals(value)) return;
                _generatedFocusRingWidth = value;
            }
        }

        public {ClassName} SetFocusRingWidth(float value)
        {
            FocusRingWidth = value;
            return this;
        }

        public {ClassName} BindFocusRingWidth(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusRingWidth = getter());
            return this;
        }

        """;
}
