namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenOnHoverCommandsAttribute : Attribute
{
    public const string Template = """
        internal System.Windows.Input.ICommand? OnHoverEnterCommand { get; set; }

        public {ClassName} SetOnHoverEnterCommand(System.Windows.Input.ICommand? command)
        {
            OnHoverEnterCommand = command;
            return this;
        }

        public {ClassName} BindOnHoverEnterCommand(Expression<Func<System.Windows.Input.ICommand?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => OnHoverEnterCommand = getter());
            return this;
        }

        internal System.Windows.Input.ICommand? OnHoverExitCommand { get; set; }

        public {ClassName} SetOnHoverExitCommand(System.Windows.Input.ICommand? command)
        {
            OnHoverExitCommand = command;
            return this;
        }

        public {ClassName} BindOnHoverExitCommand(Expression<Func<System.Windows.Input.ICommand?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => OnHoverExitCommand = getter());
            return this;
        }

        """;
}
