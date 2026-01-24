namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenTrackColorAttribute : Attribute
{
    public const string Template = """
        private Color _generatedTrackColor;

        internal Color TrackColor
        {
            get => _generatedTrackColor;
            set
            {
                if (_generatedTrackColor.Equals(value)) return;
                _generatedTrackColor = value;
            }
        }

        public {ClassName} SetTrackColor(Color value)
        {
            TrackColor = value;
            return this;
        }

        public {ClassName} BindTrackColor(Expression<Func<Color>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => TrackColor = getter());
            return this;
        }

        """;
}
