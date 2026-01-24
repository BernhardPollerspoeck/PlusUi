namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMaxDateAttribute : Attribute
{
    public const string Template = """
        private DateOnly? _generatedMaxDate;

        internal DateOnly? MaxDate
        {
            get => _generatedMaxDate;
            set
            {
                if (_generatedMaxDate == value) return;
                _generatedMaxDate = value;
            }
        }

        public {ClassName} SetMaxDate(DateOnly? value)
        {
            MaxDate = value;
            return this;
        }

        public {ClassName} BindMaxDate(Expression<Func<DateOnly?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MaxDate = getter());
            return this;
        }

        """;
}
