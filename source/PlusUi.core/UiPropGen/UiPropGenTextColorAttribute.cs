namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenTextColorAttribute : Attribute
{
    public const string Template = """
        private Color _generatedTextColor;

        internal Color TextColor
        {
            get => _generatedTextColor;
            set
            {
                if (_generatedTextColor.Equals(value)) return;
                _generatedTextColor = value;
            }
        }

        public {ClassName} SetTextColor(Color value)
        {
            TextColor = value;
            return this;
        }

        public {ClassName} BindTextColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => TextColor = getter());
            return this;
        }

        """;
}
