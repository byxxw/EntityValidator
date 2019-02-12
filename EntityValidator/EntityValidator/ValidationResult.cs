using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityValidator
{
    public class ValidationResult
    {
        public bool Success { get; private set; }
        public LambdaExpression Expression { get; set; }

        public ValidationResult(bool success, LambdaExpression expression)
        {
            Success = success;
            Expression = expression;
        }

        public override string ToString()
        {
            return string.Format("Expression: {0}\r\nSuccess: {1}", Expression, Success);
        }
    }
}
