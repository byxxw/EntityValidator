using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityValidator
{
    /// <summary>
    /// 通过字符串创建二叉树
    /// </summary>
    public class BinaryTreeCreator
    {
        public ExpressionNode CreateBinaryTree(string expression)
        {
            var expressionArray = ConvertStringExpressionToArray(expression);
            return CreateBinaryTree(expressionArray);
        }

        public ExpressionNode CreateBinaryTree(string[] expression)
        {
            var opStack = new Stack<string>();
            var reversePolish = new Queue<string>();
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];

                if (IsDigitOrLetter(c))
                {
                    reversePolish.Enqueue(c);
                }
                else if (CheckIsOp(c))
                {
                    if (c == "(")
                    {
                        opStack.Push(c);
                    }
                    else if (c == ")")
                    {
                        while (opStack.Count > 0)
                        {
                            var op = opStack.Peek();
                            if (op == "(")
                            {
                                opStack.Pop();
                                break;
                            }
                            else
                            {
                                reversePolish.Enqueue(opStack.Pop());
                            }
                        }
                    }
                    else
                    {
                        while (opStack.Count > 0)
                        {
                            if ("(" == opStack.Peek())
                            {
                                opStack.Push(c);
                                break;
                            }
                            else if (IsGreat(opStack.Peek(), c))
                            {
                                reversePolish.Enqueue(opStack.Pop());
                            }
                            else if (IsEqual(c, opStack.Peek()))
                            {
                                opStack.Push(c);
                                break;
                            }
                        }

                        if (opStack.Count == 0)
                        {
                            opStack.Push(c);
                        }
                    }
                }



            }

            // 将剩余的操作符入队
            while (opStack.Count > 0)
            {
                reversePolish.Enqueue(opStack.Pop());
            }

            var nodeStack = new Stack<ExpressionNode>();

            // 将逆波兰式转化成二叉树
            while (reversePolish.Count > 0)
            {

                var s = reversePolish.Dequeue();
                // 以当前的元素的值新建一个节点
                var node = new ExpressionNode();
                node.Value = s;
                // 如果是数字
                if (IsDigitOrLetter(s))
                {

                    nodeStack.Push(node);
                    // 如果是操作符
                }
                else if (CheckIsOp(s))
                {

                    //从栈里弹出两个节点作为当前节点的左右子节点
                    var rightNode = nodeStack.Pop();
                    var leftNode = nodeStack.Pop();
                    node.Left = leftNode;
                    node.Right = rightNode;
                    // 入栈
                    nodeStack.Push(node);
                }

            }

            return nodeStack.Pop();
        }

        private String[] ConvertStringExpressionToArray(string expression)
        {
            var list = new List<string>();

            var replacedExpression = expression;

            foreach (var kvp in SPECIAL_OP)
            {
                replacedExpression = replacedExpression.Replace(kvp.Value, kvp.Key);
            }

            var chars = replacedExpression.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var item = chars[i];
                //if (item == '(' || item == ')' || item == '&' || item == '|' || item == '=')
                if (CheckIsOp(item.ToString()))
                {
                    list.Add(item.ToString());
                }
                else if (CheckIsValue(item))
                {
                    var j = i + 1;
                    var tmp = item.ToString();
                    while (j < chars.Length && CheckIsValue(chars[j]))
                    {
                        tmp += chars[j].ToString();
                        j++;
                    }
                    list.Add(tmp);
                    i = j - 1;
                }
            }

            return list.ToArray();
        }

        private bool CheckIsValue(char c)
        {
            var isValue = char.IsLetter(c) || char.IsDigit(c) || c == '.' || '-' == c;
            return isValue;
        }

        private bool IsDigitOrLetter(string s)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (Char.IsDigit(s[i]) || char.IsLetter(s[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static readonly HashSet<string> OP_SETS = new HashSet<string>(){
            "(",")", "|","=","&",">", "\\","<","/"
        };

        private static readonly Dictionary<string, string> SPECIAL_OP = new Dictionary<string, string>(){
            {"\\",">="},
            {"/","<="},
            {"=","=="},
            {"&", "&&"},
            {"|", "||"}
        };


        private bool CheckIsOp(string c)
        {
            return OP_SETS.Contains(c);
        }


        private bool IsGreat(string op1, string op2)
        {

            if (GetPriority(op1) > GetPriority(op2))
                return true;
            else
                return false;
        }

        private bool IsEqual(string op1, string op2)
        {
            if (GetPriority(op1) == GetPriority(op2))
                return true;
            return false;
        }

        private int GetPriority(string op)
        {
            // \ 表示：>=
            // / 表示：<=
            if (">" == (op) || "<" == op || op == "=" || "\\" == op || "/" == op)
                return 1;
            else if (op == "&" || op == "|")
                return 2;
            else
                throw new Exception("Unsupported operator!");
        }

        public void Print(ExpressionNode root)
        {
            if (root != null)
            {
                if (CheckIsOp(root.Value))
                {
                    Console.Write("(");
                }
                Print(root.Left);

                var val = root.Value;
                if (SPECIAL_OP.ContainsKey(val))
                {
                    Console.WriteLine(SPECIAL_OP[val]);
                }
                else
                {
                    Console.Write(root.Value);
                }

                Print(root.Right);
                if (CheckIsOp(root.Value))
                {
                    Console.Write(")");
                }

            }

        }
    }
}
