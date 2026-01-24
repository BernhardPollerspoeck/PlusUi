namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenIsCheckedAttribute : Attribute
{
    public const string Template = """
        private bool _generatedIsChecked;

        internal bool IsChecked
        {
            get => _generatedIsChecked;
            set
            {
                if (_generatedIsChecked == value) return;
                _generatedIsChecked = value;
            }
        }

        public {ClassName} SetIsChecked(bool value)
        {
            IsChecked = value;
            return this;
        }

        public {ClassName} BindIsChecked(Expression<Func<bool>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => IsChecked = getter());
            return this;
        }

        """;
}
