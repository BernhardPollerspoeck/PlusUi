namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenCornerRadiusAttribute : Attribute
{
    public const string Template = """
        private float _generatedCornerRadius;

        internal float CornerRadius
        {
            get => _generatedCornerRadius;
            set
            {
                if (_generatedCornerRadius.Equals(value)) return;
                _generatedCornerRadius = value;
            }
        }

        public {ClassName} SetCornerRadius(float value)
        {
            CornerRadius = value;
            return this;
        }

        public {ClassName} BindCornerRadius(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => CornerRadius = getter());
            return this;
        }

        """;
}
