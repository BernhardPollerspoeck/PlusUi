namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenDisplayFormatAttribute : Attribute
{
    public const string Template = """
        private string _generatedDisplayFormat = "";

        internal string DisplayFormat
        {
            get => _generatedDisplayFormat;
            set
            {
                if (_generatedDisplayFormat == value) return;
                _generatedDisplayFormat = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetDisplayFormat(string value)
        {
            DisplayFormat = value;
            return this;
        }

        public {ClassName} BindDisplayFormat(Expression<Func<string>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => DisplayFormat = getter());
            return this;
        }

        """;
}
