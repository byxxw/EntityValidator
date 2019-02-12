using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityValidator
{

    public class ResolveStringExpressionExpception : Exception
    {
        public ResolveStringExpressionExpception() { }

        public ResolveStringExpressionExpception(string message, Exception inner) : base(message, inner) { }
    }

    public class EntityPropertyNotFoundException : Exception
    {
        public EntityPropertyNotFoundException() { }

        public EntityPropertyNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
