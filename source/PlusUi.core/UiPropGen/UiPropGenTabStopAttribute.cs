namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenTabStopAttribute : Attribute
{
    public const string Template = """
        private bool _generatedTabStop = true;

        internal bool TabStop
        {
            get => _generatedTabStop;
            set
            {
                if (_generatedTabStop == value) return;
                _generatedTabStop = value;
            }
        }

        public {ClassName} SetTabStop(bool value)
        {
            TabStop = value;
            return this;
        }

        public {ClassName} BindTabStop(Expression<Func<bool>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => TabStop = getter());
            return this;
        }

        """;
}
