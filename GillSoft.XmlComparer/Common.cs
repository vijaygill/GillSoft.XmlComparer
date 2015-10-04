using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer
{
    public class Common
    {
        public static readonly string DefaultNamespace = "ns000";

        internal static readonly List<KeyValueElementInfo> commonKeyValues = new List<KeyValueElementInfo> { 
            new KeyValueElementInfo("name", "value"),
            new KeyValueElementInfo("name", "type"),
            new KeyValueElementInfo("key", "value"),
            new KeyValueElementInfo("name"),
            new KeyValueElementInfo("key"),
        };
    }

}
