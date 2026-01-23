namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenAccessibilityValueAttribute : Attribute
{
    public const string Template = """
        private string? _generatedAccessibilityValue;

        public string? AccessibilityValue
        {
            get => _generatedAccessibilityValue;
            protected internal set
            {
                if (_generatedAccessibilityValue == value) return;
                _generatedAccessibilityValue = value;
            }
        }

        public {ClassName} SetAccessibilityValue(string? value)
        {
            AccessibilityValue = value;
            return this;
        }

        public {ClassName} BindAccessibilityValue(Expression<Func<string?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => AccessibilityValue = getter());
            return this;
        }

        """;
}
