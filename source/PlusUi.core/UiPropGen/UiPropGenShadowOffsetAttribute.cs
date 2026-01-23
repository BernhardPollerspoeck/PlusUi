namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenShadowOffsetAttribute : Attribute
{
    public const string Template = """
        internal Point ShadowOffset
        {
            get => field;
            set
            {
                field = value;
                InvalidateShadowCache();
            }
        }

        public {ClassName} SetShadowOffset(Point offset)
        {
            ShadowOffset = offset;
            return this;
        }

        public {ClassName} BindShadowOffset(Expression<Func<Point>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => ShadowOffset = getter());
            return this;
        }

        """;
}
