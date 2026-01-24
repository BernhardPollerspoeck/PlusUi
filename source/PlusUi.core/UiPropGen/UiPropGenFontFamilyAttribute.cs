namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFontFamilyAttribute : Attribute
{
    public const string Template = """
        private string? _generatedFontFamily;

        internal string? FontFamily
        {
            get => _generatedFontFamily;
            set
            {
                if (Equals(_generatedFontFamily, value)) return;
                _generatedFontFamily = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetFontFamily(string? value)
        {
            FontFamily = value;
            return this;
        }

        public {ClassName} BindFontFamily(Expression<Func<string?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FontFamily = getter());
            return this;
        }

        """;
}
