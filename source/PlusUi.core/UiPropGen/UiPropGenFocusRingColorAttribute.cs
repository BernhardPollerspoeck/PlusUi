namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFocusRingColorAttribute : Attribute
{
    public const string Template = """
        private Color _generatedFocusRingColor;

        internal Color FocusRingColor
        {
            get => _generatedFocusRingColor;
            set
            {
                if (_generatedFocusRingColor.Equals(value)) return;
                _generatedFocusRingColor = value;
            }
        }

        public {ClassName} SetFocusRingColor(Color value)
        {
            FocusRingColor = value;
            return this;
        }

        public {ClassName} BindFocusRingColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusRingColor = getter());
            return this;
        }

        """;
}
