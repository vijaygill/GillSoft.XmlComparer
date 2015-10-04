using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GillSoft.XmlComparer
{
    public class XmlNamespaceManagerEnhanced : XmlNamespaceManager
    {
        private string defaultNamespace;

        public override string DefaultNamespace
        {
            get
            {
                return this.defaultNamespace;
            }
        }

        public XmlNamespaceManagerEnhanced(XDocument doc)
            : base(doc.CreateReader().NameTable)
        {
            var ns = doc.Root.GetDefaultNamespace();
            if (ns != null && !string.IsNullOrWhiteSpace(ns.NamespaceName))
            {
                this.defaultNamespace = ns.NamespaceName;
                AddNamespace(Common.DefaultNamespace, this.defaultNamespace);
            }

            var nslist = new Dictionary<string, string>();
            var nav = doc.CreateNavigator();
            while (nav.MoveToFollowing(XPathNodeType.Element))
            {
                var newFoundNapespaces = nav.GetNamespacesInScope(XmlNamespaceScope.All).Where(a => !nslist.ContainsKey(a.Key) && !this.HasNamespace(a.Key));
                foreach (var item in newFoundNapespaces)
                {
                    nslist.Add(item.Key, item.Value);
                }
            }

            foreach (var item in nslist)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    continue;
                AddNamespace(item.Key, item.Value);
            }
        }
    }
}
