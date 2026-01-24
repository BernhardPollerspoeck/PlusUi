namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMinDateAttribute : Attribute
{
    public const string Template = """
        private DateOnly? _generatedMinDate;

        internal DateOnly? MinDate
        {
            get => _generatedMinDate;
            set
            {
                if (_generatedMinDate == value) return;
                _generatedMinDate = value;
            }
        }

        public {ClassName} SetMinDate(DateOnly? value)
        {
            MinDate = value;
            return this;
        }

        public {ClassName} BindMinDate(Expression<Func<DateOnly?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MinDate = getter());
            return this;
        }

        """;
}
