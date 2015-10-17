using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer
{
    public class KeyValueElementInfo
    {
        public List<string> KeyNames { get; private set; }

        public string ValueName { get; private set; }

        public KeyValueElementInfo(string keyName, string valueName = null)
            : this(new[] { keyName }, valueName)
        {
        }

        public KeyValueElementInfo(string[] keyNames, string valueName = null)
        {
            this.KeyNames = new List<string>();
            this.KeyNames.AddRange(keyNames);

            this.ValueName = valueName;
        }


        public static int KeyMatchCount(KeyValueElementInfo kv, IEnumerable<string> names)
        {
            var res = names == null ? -1 : names.Where(a => kv.KeyNames.Any(b => b == a)).Count();
            if (!string.IsNullOrWhiteSpace(kv.ValueName))
            {
                if (names.Any(a => a == kv.ValueName))
                    res++;
            }
            return res;
        }

        public override string ToString()
        {
            var res = string.Join(", ", KeyNames) + ": " + ValueName;
            return res;
        }

    }
}
