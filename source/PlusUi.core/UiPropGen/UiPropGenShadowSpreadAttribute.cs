namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenShadowSpreadAttribute : Attribute
{
    public const string Template = """
        private float _generatedShadowSpread;

        internal float ShadowSpread
        {
            get => _generatedShadowSpread;
            set
            {
                if (_generatedShadowSpread.Equals(value)) return;
                _generatedShadowSpread = value;
            }
        }

        public {ClassName} SetShadowSpread(float value)
        {
            ShadowSpread = value;
            return this;
        }

        public {ClassName} BindShadowSpread(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => ShadowSpread = getter());
            return this;
        }

        """;
}
