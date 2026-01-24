namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMaxValueAttribute : Attribute
{
    public const string Template = """
        private float _generatedMaxValue = 100f;

        internal float MaxValue
        {
            get => _generatedMaxValue;
            set
            {
                if (_generatedMaxValue.Equals(value)) return;
                _generatedMaxValue = value;
            }
        }

        public {ClassName} SetMaxValue(float value)
        {
            MaxValue = value;
            return this;
        }

        public {ClassName} BindMaxValue(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MaxValue = getter());
            return this;
        }

        """;
}
