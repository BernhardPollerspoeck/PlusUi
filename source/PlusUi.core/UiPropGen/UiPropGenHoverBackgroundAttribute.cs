namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenHoverBackgroundAttribute : Attribute
{
    public const string Template = """
        private IBackground? _generatedHoverBackground;

        internal IBackground? HoverBackground
        {
            get => _generatedHoverBackground;
            set
            {
                if (Equals(_generatedHoverBackground, value)) return;
                _generatedHoverBackground = value;
            }
        }

        public {ClassName} SetHoverBackground(IBackground? value)
        {
            HoverBackground = value;
            return this;
        }

        public {ClassName} BindHoverBackground(Expression<Func<IBackground?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => HoverBackground = getter());
            return this;
        }

        """;
}
