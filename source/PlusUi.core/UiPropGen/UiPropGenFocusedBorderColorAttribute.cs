namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenFocusedBorderColorAttribute : Attribute
{
    public const string Template = """
        private Color? _generatedFocusedBorderColor;

        internal Color? FocusedBorderColor
        {
            get => _generatedFocusedBorderColor;
            set
            {
                if (Nullable.Equals(_generatedFocusedBorderColor, value)) return;
                _generatedFocusedBorderColor = value;
            }
        }

        public {ClassName} SetFocusedBorderColor(Color? value)
        {
            FocusedBorderColor = value;
            return this;
        }

        public {ClassName} BindFocusedBorderColor(Expression<Func<Color?>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => FocusedBorderColor = getter());
            return this;
        }

        """;
}
