namespace PlusUi.core.UiPropGen;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UiPropGenDesiredSizeAttribute : Attribute
{
    public const string Template = """
        internal virtual Size? DesiredSize
        {
            get => field;
            set
            {
                if (field.HasValue && field.Equals(value)) return;
                field = value;
                InvalidateMeasure();
            }
        }

        public {ClassName} SetDesiredSize(Size size)
        {
            DesiredSize = size;
            return this;
        }

        public {ClassName} BindDesiredSize(Expression<Func<Size>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => DesiredSize = getter());
            return this;
        }

        public {ClassName} SetDesiredWidth(float width)
        {
            DesiredSize = new Size(width, DesiredSize?.Height ?? -1);
            return this;
        }

        public {ClassName} SetDesiredHeight(float height)
        {
            DesiredSize = new Size(DesiredSize?.Width ?? -1, height);
            return this;
        }

        public {ClassName} BindDesiredWidth(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => DesiredSize = new Size(getter(), DesiredSize?.Height ?? -1));
            return this;
        }

        public {ClassName} BindDesiredHeight(Expression<Func<float>> propertyExpression)
        {
            var path = ExpressionPathService.GetPropertyPath(propertyExpression);
            var getter = propertyExpression.Compile();
            RegisterPathBinding(path, () => DesiredSize = new Size(DesiredSize?.Width ?? -1, getter()));
            return this;
        }

        """;
}
