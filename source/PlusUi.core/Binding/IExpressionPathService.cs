using System.Linq.Expressions;

namespace PlusUi.core.Binding;

public interface IExpressionPathService
{
    string[] GetPropertyPath<T>(Expression<Func<T>> expression);
}
