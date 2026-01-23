namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenHorizontalAlignmentAttribute : Attribute
{
    public const string Template = """
        internal virtual HorizontalAlignment HorizontalAlignment
        {
            get => field;
            set
            {
                if (field == value) return;
                field = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetHorizontalAlignment(HorizontalAlignment alignment)
        {
            HorizontalAlignment = alignment;
            return this;
        }

        public {ClassName} BindHorizontalAlignment(Expression<Func<HorizontalAlignment>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => HorizontalAlignment = getter());
            return this;
        }

        """;
}
