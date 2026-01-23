namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenHighContrastForegroundAttribute : Attribute
{
    public const string Template = """
        private Color? _generatedHighContrastForeground;

        internal Color? HighContrastForeground
        {
            get => _generatedHighContrastForeground;
            set
            {
                if (Nullable.Equals(_generatedHighContrastForeground, value)) return;
                _generatedHighContrastForeground = value;
            }
        }

        public {ClassName} SetHighContrastForeground(Color value)
        {
            HighContrastForeground = value;
            return this;
        }

        public {ClassName} BindHighContrastForeground(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => HighContrastForeground = getter());
            return this;
        }

        """;
}
