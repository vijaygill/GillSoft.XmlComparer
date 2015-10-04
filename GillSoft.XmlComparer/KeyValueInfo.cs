using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer
{
    internal class KeyValueInfo
    {
        public List<string> KeyNames { get; private set; }

        public string ValueName { get; private set; }

        public KeyValueInfo(string keyName, string valueName = null)
            : this(new[] { keyName }, valueName)
        {
        }

        public KeyValueInfo(string[] keyNames, string valueName = null)
        {
            this.KeyNames = new List<string>();
            this.KeyNames.AddRange(keyNames);

            this.ValueName = valueName;
        }

        public int KeyMatchCount(List<string> names)
        {
            var res = names == null ? -1: names.Where(a=> KeyNames.Any(b=> b == a)).Count();
            if (!string.IsNullOrWhiteSpace(ValueName))
            {
                if (names.Any(a => a == ValueName))
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
