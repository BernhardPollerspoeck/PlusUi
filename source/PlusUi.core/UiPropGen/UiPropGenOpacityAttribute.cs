namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenOpacityAttribute : Attribute
{
    public const string Template = """
        internal float Opacity { get; set; }

        public {ClassName} SetOpacity(float opacity)
        {
            Opacity = Math.Clamp(opacity, 0f, 1f);
            return this;
        }

        public {ClassName} BindOpacity(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Opacity = Math.Clamp(getter(), 0f, 1f));
            return this;
        }

        """;
}
