using System.Linq.Expressions;

namespace PlusUi.core.Binding;

internal class ExpressionPathService : IExpressionPathService
{
    public string[] GetPropertyPath<T>(Expression<Func<T>> expression)
    {
        // First try to get a direct property chain (e.g., obj.Prop1.Prop2)
        var chainPath = GetPropertyChain(expression.Body);
        if (chainPath.Length > 0)
        {
            return chainPath;
        }

        // For complex expressions (conditionals, method calls, etc.),
        // collect all unique member names from the entire expression tree
        var allMembers = new HashSet<string>();
        CollectAllMemberNames(expression.Body, allMembers);
        return [.. allMembers];
    }

    private static string[] GetPropertyChain(Expression? current)
    {
        var members = new List<string>();
        bool endsAtClosure = false;

        while (current != null)
        {
            switch (current)
            {
                case MemberExpression memberExpr:
                    members.Insert(0, memberExpr.Member.Name);
                    current = memberExpr.Expression;
                    // Check if next step is a ConstantExpression (closure/DisplayClass)
                    if (current is ConstantExpression)
                    {
                        endsAtClosure = true;
                        current = null;
                    }
                    break;
                case UnaryExpression { NodeType: ExpressionType.Convert } unary:
                    current = unary.Operand;
                    break;
                case ConstantExpression:
                    current = null;
                    break;
                default:
                    current = null;
                    break;
            }
        }

        // If the chain ends at a closure and we have more than one member,
        // the first member is the closure field (e.g., "vm") that captures the ViewModel - skip it.
        // For expressions like () => vm.Checked: ["vm", "Checked"] -> ["Checked"]
        // For expressions like () => isVisible: ["isVisible"] -> ["isVisible"] (keep single member)
        if (endsAtClosure && members.Count > 1)
        {
            members.RemoveAt(0);
        }

        return [.. members];
    }

    private static void CollectAllMemberNames(Expression? expression, HashSet<string> members)
    {
        if (expression == null) return;

        switch (expression)
        {
            case MemberExpression memberExpr:
                // Only add if not directly accessing a closure
                if (memberExpr.Expression is not ConstantExpression)
                {
                    members.Add(memberExpr.Member.Name);
                    CollectAllMemberNames(memberExpr.Expression, members);
                }
                else
                {
                    // Single member accessing closure - still add it
                    members.Add(memberExpr.Member.Name);
                }
                break;

            case BinaryExpression binaryExpr:
                CollectAllMemberNames(binaryExpr.Left, members);
                CollectAllMemberNames(binaryExpr.Right, members);
                break;

            case ConditionalExpression condExpr:
                CollectAllMemberNames(condExpr.Test, members);
                CollectAllMemberNames(condExpr.IfTrue, members);
                CollectAllMemberNames(condExpr.IfFalse, members);
                break;

            case UnaryExpression unaryExpr:
                CollectAllMemberNames(unaryExpr.Operand, members);
                break;

            case MethodCallExpression methodExpr:
                CollectAllMemberNames(methodExpr.Object, members);
                foreach (var arg in methodExpr.Arguments)
                {
                    CollectAllMemberNames(arg, members);
                }
                break;

            case NewExpression newExpr:
                foreach (var arg in newExpr.Arguments)
                {
                    CollectAllMemberNames(arg, members);
                }
                break;

            case LambdaExpression lambdaExpr:
                CollectAllMemberNames(lambdaExpr.Body, members);
                break;

            case InvocationExpression invocationExpr:
                CollectAllMemberNames(invocationExpr.Expression, members);
                foreach (var arg in invocationExpr.Arguments)
                {
                    CollectAllMemberNames(arg, members);
                }
                break;
        }
    }
}
