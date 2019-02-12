using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityValidator
{
    public class Validator
    {
        private static readonly LinqExpressionTreeBuilder linqExpressionTreeBuilder = new LinqExpressionTreeBuilder();

        public ValidationResult Validate(string expression, object instance)
        {
            var lambda = linqExpressionTreeBuilder.Build(expression, instance);
            var validateFunc = lambda.Compile();
            var success = validateFunc(instance);
            return new ValidationResult(success, lambda);
        }
    }
}
