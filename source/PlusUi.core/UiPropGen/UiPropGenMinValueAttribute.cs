namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMinValueAttribute : Attribute
{
    public const string Template = """
        private float _generatedMinValue;

        internal float MinValue
        {
            get => _generatedMinValue;
            set
            {
                if (_generatedMinValue.Equals(value)) return;
                _generatedMinValue = value;
            }
        }

        public {ClassName} SetMinValue(float value)
        {
            MinValue = value;
            return this;
        }

        public {ClassName} BindMinValue(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MinValue = getter());
            return this;
        }

        """;
}
