namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenShadowColorAttribute : Attribute
{
    public const string Template = """
        internal Color ShadowColor
        {
            get => field;
            set
            {
                field = value;
                InvalidateShadowCache();
            }
        }

        public {ClassName} SetShadowColor(Color color)
        {
            ShadowColor = color;
            return this;
        }

        public {ClassName} BindShadowColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => ShadowColor = getter());
            return this;
        }

        """;
}
