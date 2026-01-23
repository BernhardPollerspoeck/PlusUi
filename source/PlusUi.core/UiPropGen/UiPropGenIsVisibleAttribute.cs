namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenIsVisibleAttribute : Attribute
{
    public const string Template = """
        private bool _generatedIsVisible = true;

        public bool IsVisible
        {
            get => _generatedIsVisible;
            internal set
            {
                if (_generatedIsVisible == value) return;
                _generatedIsVisible = value;
            }
        }

        public {ClassName} SetIsVisible(bool value)
        {
            IsVisible = value;
            return this;
        }

        public {ClassName} BindIsVisible(Expression<Func<bool>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => IsVisible = getter());
            return this;
        }

        """;
}
