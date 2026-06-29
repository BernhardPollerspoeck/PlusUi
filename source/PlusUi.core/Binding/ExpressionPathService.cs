using System.Linq.Expressions;

namespace PlusUi.core.Binding;

internal class ExpressionPathService : IExpressionPathService
{
    public BindingPath GetPropertyPath<T>(Expression<Func<T>> expression)
    {
        // First try to get a direct property chain (e.g., obj.Prop1.Prop2)
        var chain = GetPropertyChain(expression.Body);
        if (chain.Count > 0)
        {
            var segments = new string[chain.Count];
            var accessors = new Func<object, object?>?[chain.Count];
            for (int i = 0; i < chain.Count; i++)
            {
                segments[i] = chain[i].Member.Name;

                // Only the non-leaf segments are traversed to reach the next object in the chain.
                if (i < chain.Count - 1)
                {
                    accessors[i] = BuildAccessor(chain[i]);
                }
            }

            return new BindingPath(segments, accessors);
        }

        // For complex expressions (conditionals, method calls, etc.),
        // collect all unique member names from the entire expression tree.
        // These cannot be traversed as a simple chain, so no accessors are produced.
        var allMembers = new HashSet<string>();
        CollectAllMemberNames(expression.Body, allMembers);
        var names = new string[allMembers.Count];
        allMembers.CopyTo(names);
        return new BindingPath(names, new Func<object, object?>?[names.Length]);
    }

    /// <summary>
    /// Builds a reflection-free accessor that reads the member represented by <paramref name="memberNode"/>
    /// from the parent object. The member metadata comes from the user's own expression, so no runtime
    /// member lookup (GetProperty) is performed and trimming keeps the member rooted.
    /// </summary>
    private static Func<object, object?> BuildAccessor(MemberExpression memberNode)
    {
        var parentType = memberNode.Expression!.Type;
        var param = Expression.Parameter(typeof(object), "obj");

        var typedAccess = Expression.MakeMemberAccess(Expression.Convert(param, parentType), memberNode.Member);
        var body = Expression.Condition(
            Expression.TypeIs(param, parentType),
            Expression.Convert(typedAccess, typeof(object)),
            Expression.Constant(null, typeof(object)));

        return Expression.Lambda<Func<object, object?>>(body, param).Compile();
    }

    private static List<MemberExpression> GetPropertyChain(Expression? current)
    {
        var nodes = new List<MemberExpression>();
        bool endsAtClosure = false;

        while (current != null)
        {
            switch (current)
            {
                case MemberExpression memberExpr:
                    nodes.Insert(0, memberExpr);
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
        if (endsAtClosure && nodes.Count > 1)
        {
            nodes.RemoveAt(0);
        }

        return nodes;
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
