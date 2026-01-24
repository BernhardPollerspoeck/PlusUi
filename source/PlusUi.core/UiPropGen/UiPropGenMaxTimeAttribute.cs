namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenMaxTimeAttribute : Attribute
{
    public const string Template = """
        private TimeOnly? _generatedMaxTime;

        internal TimeOnly? MaxTime
        {
            get => _generatedMaxTime;
            set
            {
                if (_generatedMaxTime == value) return;
                _generatedMaxTime = value;
            }
        }

        public {ClassName} SetMaxTime(TimeOnly? value)
        {
            MaxTime = value;
            return this;
        }

        public {ClassName} BindMaxTime(Expression<Func<TimeOnly?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => MaxTime = getter());
            return this;
        }

        """;
}
