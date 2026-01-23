namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenAccessibilityLabelAttribute : Attribute
{
    public const string Template = """
        private string? _generatedAccessibilityLabel;

        public string? AccessibilityLabel
        {
            get => _generatedAccessibilityLabel;
            protected internal set
            {
                if (_generatedAccessibilityLabel == value) return;
                _generatedAccessibilityLabel = value;
            }
        }

        public {ClassName} SetAccessibilityLabel(string? value)
        {
            AccessibilityLabel = value;
            return this;
        }

        public {ClassName} BindAccessibilityLabel(Expression<Func<string?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => AccessibilityLabel = getter());
            return this;
        }

        """;
}
