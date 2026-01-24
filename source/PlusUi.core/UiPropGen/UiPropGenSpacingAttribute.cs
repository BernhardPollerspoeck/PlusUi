namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenSpacingAttribute : Attribute
{
    public const string Template = """
        private float _generatedSpacing;

        internal float Spacing
        {
            get => _generatedSpacing;
            set
            {
                if (_generatedSpacing.Equals(value)) return;
                _generatedSpacing = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetSpacing(float value)
        {
            Spacing = value;
            return this;
        }

        public {ClassName} BindSpacing(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Spacing = getter());
            return this;
        }

        """;
}
