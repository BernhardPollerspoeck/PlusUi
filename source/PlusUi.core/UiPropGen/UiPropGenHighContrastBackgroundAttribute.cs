namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenHighContrastBackgroundAttribute : Attribute
{
    public const string Template = """
        internal IBackground? HighContrastBackground { get; set; }

        public {ClassName} SetHighContrastBackground(IBackground? background)
        {
            HighContrastBackground = background;
            return this;
        }

        public {ClassName} SetHighContrastBackground(Color color)
        {
            HighContrastBackground = new SolidColorBackground(color);
            return this;
        }

        public {ClassName} BindHighContrastBackground(Expression<Func<IBackground?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => HighContrastBackground = getter());
            return this;
        }

        public {ClassName} BindHighContrastBackgroundColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => HighContrastBackground = new SolidColorBackground(getter()));
            return this;
        }

        """;
}
