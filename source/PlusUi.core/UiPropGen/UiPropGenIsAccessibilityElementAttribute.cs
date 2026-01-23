namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenIsAccessibilityElementAttribute : Attribute
{
    public const string Template = """
        private bool _generatedIsAccessibilityElement = true;

        public bool IsAccessibilityElement
        {
            get => _generatedIsAccessibilityElement;
            protected internal set
            {
                if (_generatedIsAccessibilityElement == value) return;
                _generatedIsAccessibilityElement = value;
            }
        }

        public {ClassName} SetIsAccessibilityElement(bool value)
        {
            IsAccessibilityElement = value;
            return this;
        }

        public {ClassName} BindIsAccessibilityElement(Expression<Func<bool>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => IsAccessibilityElement = getter());
            return this;
        }

        """;
}
