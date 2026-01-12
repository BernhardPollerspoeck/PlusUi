using System.Linq.Expressions;

namespace PlusUi.core.Binding;

/// <summary>
/// Internal service for extracting property paths from expressions for data binding.
/// </summary>
internal interface IExpressionPathService
{
    string[] GetPropertyPath<T>(Expression<Func<T>> expression);
}
