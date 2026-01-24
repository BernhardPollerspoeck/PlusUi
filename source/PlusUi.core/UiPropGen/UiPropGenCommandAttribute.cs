namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenCommandAttribute : Attribute
{
    public const string Template = """
        internal System.Windows.Input.ICommand? Command { get; set; }

        public {ClassName} SetCommand(System.Windows.Input.ICommand command)
        {
            Command = command;
            return this;
        }

        public {ClassName} BindCommand(Expression<Func<System.Windows.Input.ICommand?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Command = getter());
            return this;
        }

        internal object? CommandParameter { get; set; }

        public {ClassName} SetCommandParameter(object? parameter)
        {
            CommandParameter = parameter;
            return this;
        }

        public {ClassName} BindCommandParameter(Expression<Func<object?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => CommandParameter = getter());
            return this;
        }

        """;
}
