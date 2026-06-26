namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenCursorAttribute : Attribute
{
    public const string Template = """
        internal CursorType Cursor { get; set; } = CursorType.Default;

        public {ClassName} SetCursor(CursorType cursor)
        {
            Cursor = cursor;
            return this;
        }

        public {ClassName} BindCursor(Expression<Func<CursorType>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Cursor = getter());
            return this;
        }

        """;
}
