namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenThumbColorAttribute : Attribute
{
    public const string Template = """
        private Color _generatedThumbColor;

        internal Color ThumbColor
        {
            get => _generatedThumbColor;
            set
            {
                if (_generatedThumbColor.Equals(value)) return;
                _generatedThumbColor = value;
            }
        }

        public {ClassName} SetThumbColor(Color value)
        {
            ThumbColor = value;
            return this;
        }

        public {ClassName} BindThumbColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => ThumbColor = getter());
            return this;
        }

        """;
}
