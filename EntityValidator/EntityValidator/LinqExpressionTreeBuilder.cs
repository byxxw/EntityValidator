using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityValidator
{
    public class LinqExpressionTreeBuilder
    {
        enum ExpressionPositionEnum
        {
            None = 0,
            Left = 1,
            Right = 2
        }

        private ParameterExpression _parameterExpression;
        private Type _parameterType;

        private static readonly BindingFlags BINDING_FLAGS =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        private static readonly string CLEAR_WHIHT_SPACE_PATTERN = @"\s*";
        private Dictionary<string, ExpressionType> OPERTIONS_MAP = new Dictionary<string, ExpressionType>() {
              {"=", ExpressionType.Equal},
              {">", ExpressionType.GreaterThan},
              {"\\", ExpressionType.GreaterThanOrEqual},
              {"<", ExpressionType.LessThan},
              {"/", ExpressionType.LessThanOrEqual},
              {"&", ExpressionType.And},
              {"|", ExpressionType.Or}
            };


        public Expression<Func<object, bool>> Build(string expression, object instance)
        {
            Init(instance.GetType());
            var clearedExpression = Regex.Replace(expression, CLEAR_WHIHT_SPACE_PATTERN, "");
            var converter = new BinaryTreeCreator();
            ExpressionNode root = null;
            try
            {
                root = converter.CreateBinaryTree(clearedExpression);
            }
            catch (Exception e)
            {
                throw new ResolveStringExpressionExpception("解析表达式异常, 请检查.", null);
            }

            Expression<Func<object, bool>> lambda = null;
            try
            {

                var body = BuildLinqExpression(root, ExpressionPositionEnum.None);
                lambda = Expression.Lambda<Func<object, bool>>(body, _parameterExpression);
            }
            catch (Exception e)
            {
                throw new EntityPropertyNotFoundException("lambda表达式转换异常.", e);
            }

            return lambda;
        }

        private void Init(Type parameterType)
        {
            _parameterType = parameterType;
            _parameterExpression = Expression.Parameter(typeof(object), "instance");
        }

        private Expression BuildLinqExpression(ExpressionNode node, ExpressionPositionEnum expressionPosition)
        {
            if (node != null)
            {
                if (OPERTIONS_MAP.ContainsKey(node.Value))
                {
                    var left = BuildLinqExpression(node.Left, ExpressionPositionEnum.Left);
                    if (left is MemberExpression)
                    {
                        var parameterLeft = (MemberExpression)left;
                        var t = (PropertyInfo)parameterLeft.Member;
                        var changeData = Convert.ChangeType(node.Right.Value, t.PropertyType, null);
                        node.Right.ChangedValue = changeData;

                    }
                    var right = BuildLinqExpression(node.Right, ExpressionPositionEnum.Right);
                    var expressionType = OPERTIONS_MAP[node.Value];
                    var expression = Expression.MakeBinary(expressionType, left, right);
                    return expression;

                }
                else
                {
                    if (expressionPosition == ExpressionPositionEnum.Left)
                    {
                        var prop = _parameterType.GetProperty(node.Value, BINDING_FLAGS);
                        if (prop == null)
                        {
                            throw new ArgumentException(string.Format("在实体中找不对应的属性: {0}", node.Value));
                        }
                        var convertedParameterExpression = Expression.Convert(_parameterExpression, _parameterType);
                        var propExpression = Expression.MakeMemberAccess(convertedParameterExpression, prop);
                        return propExpression;
                    }
                    else if (expressionPosition == ExpressionPositionEnum.Right)
                    {
                        var valueExpression = Expression.Constant(node.ChangedValue);
                        return valueExpression;
                    }
                }

            }
            return null;
        }

    }
}
