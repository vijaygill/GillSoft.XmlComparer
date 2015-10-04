using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer
{
    public class Common
    {
        public static readonly string DefaultNamespace = "ns000";

        internal static readonly List<KeyValueInfo> commonKeyValues = new List<KeyValueInfo> { 
            new KeyValueInfo("name", "value"),
            new KeyValueInfo("name", "type"),
            new KeyValueInfo("key", "value"),
            new KeyValueInfo("name"),
        };
    }

}
