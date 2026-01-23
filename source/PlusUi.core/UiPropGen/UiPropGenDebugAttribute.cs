namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenDebugAttribute : Attribute
{
    public const string Template = """
        private bool _generatedDebug;

        protected bool Debug
        {
            get => _generatedDebug;
            private set
            {
                if (_generatedDebug == value) return;
                _generatedDebug = value;
            }
        }

        public {ClassName} SetDebug(bool value = true)
        {
            Debug = value;
            return this;
        }

        public {ClassName} BindDebug(Expression<Func<bool>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Debug = getter());
            return this;
        }

        """;
}
