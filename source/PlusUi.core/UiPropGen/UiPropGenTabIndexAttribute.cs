namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenTabIndexAttribute : Attribute
{
    public const string Template = """
        private int? _generatedTabIndex;

        internal int? TabIndex
        {
            get => _generatedTabIndex;
            set
            {
                if (_generatedTabIndex == value) return;
                _generatedTabIndex = value;
            }
        }

        public {ClassName} SetTabIndex(int? value)
        {
            TabIndex = value;
            return this;
        }

        public {ClassName} BindTabIndex(Expression<Func<int?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => TabIndex = getter());
            return this;
        }

        """;
}
