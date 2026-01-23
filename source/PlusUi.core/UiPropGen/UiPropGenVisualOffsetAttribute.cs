namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenVisualOffsetAttribute : Attribute
{
    public const string Template = """
        private Point _generatedVisualOffset;

        internal Point VisualOffset
        {
            get => _generatedVisualOffset;
            set
            {
                if (_generatedVisualOffset.Equals(value)) return;
                _generatedVisualOffset = value;
            }
        }

        public {ClassName} SetVisualOffset(Point value)
        {
            VisualOffset = value;
            return this;
        }

        public {ClassName} BindVisualOffset(Expression<Func<Point>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => VisualOffset = getter());
            return this;
        }

        """;
}
