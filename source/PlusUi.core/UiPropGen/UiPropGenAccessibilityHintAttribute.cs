namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenAccessibilityHintAttribute : Attribute
{
    public const string Template = """
        private string? _generatedAccessibilityHint;

        public string? AccessibilityHint
        {
            get => _generatedAccessibilityHint;
            protected internal set
            {
                if (_generatedAccessibilityHint == value) return;
                _generatedAccessibilityHint = value;
            }
        }

        public {ClassName} SetAccessibilityHint(string? value)
        {
            AccessibilityHint = value;
            return this;
        }

        public {ClassName} BindAccessibilityHint(Expression<Func<string?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => AccessibilityHint = getter());
            return this;
        }

        """;
}
