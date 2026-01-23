namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenBackgroundAttribute : Attribute
{
    public const string Template = """
        internal IBackground? Background { get; set; }

        public {ClassName} SetBackground(IBackground? background)
        {
            Background = background;
            return this;
        }

        public {ClassName} SetBackground(Color color)
        {
            Background = new SolidColorBackground(color);
            return this;
        }

        public {ClassName} BindBackground(Expression<Func<IBackground?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Background = getter());
            return this;
        }

        public {ClassName} BindBackgroundColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Background = new SolidColorBackground(getter()));
            return this;
        }

        """;
}
