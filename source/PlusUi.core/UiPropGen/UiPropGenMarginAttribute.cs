namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMarginAttribute : Attribute
{
    public const string Template = """
        internal Margin Margin
        {
            get => field;
            set
            {
                if (field.Equals(value)) return;
                field = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetMargin(Margin margin)
        {
            Margin = margin;
            return this;
        }

        public {ClassName} BindMargin(Expression<Func<Margin>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => Margin = getter());
            return this;
        }

        """;
}
