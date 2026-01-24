namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenTextSizeAttribute : Attribute
{
    public const string Template = """
        private float _generatedTextSize;

        internal float TextSize
        {
            get => _generatedTextSize;
            set
            {
                if (_generatedTextSize.Equals(value)) return;
                _generatedTextSize = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetTextSize(float value)
        {
            TextSize = value;
            return this;
        }

        public {ClassName} BindTextSize(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => TextSize = getter());
            return this;
        }

        """;
}
