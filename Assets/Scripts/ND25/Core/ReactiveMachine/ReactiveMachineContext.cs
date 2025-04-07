using System.Collections.Generic;
using System.Linq;
namespace ND25.Core.ReactiveMachine
{
    public abstract class ReactiveMachineContext<ContextKey>
    {
        readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public object this[ContextKey key]
        {
            get
            {
                return _dict[key.ToString()];
            }
            set
            {
                _dict[key.ToString()] = value;
            }
        }

        public object this[string key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
            }
        }

        public override string ToString()
        {
            return _dict.Aggregate("", (current, kv) => current + $"{kv.Key}: {kv.Value}\n");
        }
    }
}
