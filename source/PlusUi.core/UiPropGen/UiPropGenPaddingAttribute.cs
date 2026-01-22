namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenPaddingAttribute : Attribute
{
    public const string Template = """
        internal Margin Padding
        {
            get => field;
            set
            {
                field = value;
                InvalidateMeasure();
            }
        } = new Margin(PlusUiDefaults.PaddingHorizontal, PlusUiDefaults.PaddingVertical);

        public {ClassName} SetPadding(Margin padding)
        {
            Padding = padding;
            return this;
        }

        public {ClassName} BindPadding(Expression<Func<Margin>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Padding = getter());
            return this;
        }

        """;
}
