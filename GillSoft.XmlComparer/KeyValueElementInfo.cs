using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer
{
    public class KeyValueElementInfo
    {
        public string ElementName { get; private set; }

        public List<string> KeyNames { get; private set; }

        public string ValueName { get; private set; }

        public KeyValueElementInfo(string elementName, string keyName, string valueName = null)
            : this(elementName, new[] { keyName }, valueName)
        {
        }

        public KeyValueElementInfo(string elementName, string[] keyNames, string valueName = null)
        {
            this.ElementName = elementName;
            this.KeyNames = new List<string>();
            this.KeyNames.AddRange(keyNames);

            this.ValueName = valueName;
        }


        public static int KeyMatchCount(KeyValueElementInfo kv, string elementName, IEnumerable<string> names)
        {
            var res = 0;
            if (kv.ElementName == "*" || kv.ElementName == elementName)
            {
                res = names == null || names.Count() == 0 ? -1 : names.Where(a => kv.KeyNames.Any(b => b == a)).Count();
                if (!string.IsNullOrWhiteSpace(kv.ValueName))
                {
                    if (names != null && names.Any(a => a == kv.ValueName))
                        res++;
                }
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
