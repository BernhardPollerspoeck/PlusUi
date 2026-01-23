namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenPlaceholderAttribute : Attribute
{
    public const string Template = """
        private string? _generatedPlaceholder;

        internal string? Placeholder
        {
            get => _generatedPlaceholder;
            set
            {
                if (Equals(_generatedPlaceholder, value)) return;
                _generatedPlaceholder = value;
            }
        }

        public {ClassName} SetPlaceholder(string? value)
        {
            Placeholder = value;
            return this;
        }

        public {ClassName} BindPlaceholder(Expression<Func<string?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Placeholder = getter());
            return this;
        }

        """;
}
