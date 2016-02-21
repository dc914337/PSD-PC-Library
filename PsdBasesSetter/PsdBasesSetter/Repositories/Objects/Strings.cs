using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsdBasesSetter.Repositories.Objects
{
    public class Strings : Dictionary<string, string>
    {
        public bool TryAdd(string key, string value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
                return false;
            }
            base.Add(key, value);
            return true;
        }


        public Strings GetCopy()
        {
            var res = new Strings();
            foreach (var pair in this)
            {
                res.Add(pair.Key, pair.Value);
            }

            return res;
        }
    }
}
