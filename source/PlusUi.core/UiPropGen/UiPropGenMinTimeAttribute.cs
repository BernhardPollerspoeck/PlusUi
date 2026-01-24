namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMinTimeAttribute : Attribute
{
    public const string Template = """
        private TimeOnly? _generatedMinTime;

        internal TimeOnly? MinTime
        {
            get => _generatedMinTime;
            set
            {
                if (_generatedMinTime == value) return;
                _generatedMinTime = value;
            }
        }

        public {ClassName} SetMinTime(TimeOnly? value)
        {
            MinTime = value;
            return this;
        }

        public {ClassName} BindMinTime(Expression<Func<TimeOnly?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MinTime = getter());
            return this;
        }

        """;
}
