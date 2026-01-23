namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenVerticalAlignmentAttribute : Attribute
{
    public const string Template = """
        internal virtual VerticalAlignment VerticalAlignment
        {
            get => field;
            set
            {
                if (field == value) return;
                field = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetVerticalAlignment(VerticalAlignment alignment)
        {
            VerticalAlignment = alignment;
            return this;
        }

        public {ClassName} BindVerticalAlignment(Expression<Func<VerticalAlignment>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => VerticalAlignment = getter());
            return this;
        }

        """;
}
