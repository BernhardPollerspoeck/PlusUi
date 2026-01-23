namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenPlaceholderColorAttribute : Attribute
{
    public const string Template = """
        private Color _generatedPlaceholderColor;

        internal Color PlaceholderColor
        {
            get => _generatedPlaceholderColor;
            set
            {
                if (_generatedPlaceholderColor.Equals(value)) return;
                _generatedPlaceholderColor = value;
            }
        }

        public {ClassName} SetPlaceholderColor(Color value)
        {
            PlaceholderColor = value;
            return this;
        }

        public {ClassName} BindPlaceholderColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => PlaceholderColor = getter());
            return this;
        }

        """;
}
