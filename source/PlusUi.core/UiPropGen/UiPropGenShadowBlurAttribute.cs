namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenShadowBlurAttribute : Attribute
{
    public const string Template = """
        internal float ShadowBlur
        {
            get => field;
            set
            {
                field = value;
                InvalidateShadowCache();
            }
        }

        public {ClassName} SetShadowBlur(float blur)
        {
            ShadowBlur = blur;
            return this;
        }

        public {ClassName} BindShadowBlur(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => ShadowBlur = getter());
            return this;
        }

        """;
}
