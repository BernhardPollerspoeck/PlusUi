namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFocusedBackgroundAttribute : Attribute
{
    public const string Template = """
        internal IBackground? FocusedBackground { get; set; }

        public {ClassName} SetFocusedBackground(IBackground? background)
        {
            FocusedBackground = background;
            return this;
        }

        public {ClassName} SetFocusedBackground(Color color)
        {
            FocusedBackground = new SolidColorBackground(color);
            return this;
        }

        public {ClassName} BindFocusedBackground(Expression<Func<IBackground?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusedBackground = getter());
            return this;
        }

        public {ClassName} BindFocusedBackgroundColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusedBackground = new SolidColorBackground(getter()));
            return this;
        }

        """;
}
