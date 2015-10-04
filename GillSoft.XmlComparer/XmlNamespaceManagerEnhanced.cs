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
            var nslist = new Dictionary<string, string>();
            if (ns != null && !string.IsNullOrWhiteSpace(ns.NamespaceName))
            {
                this.defaultNamespace = ns.NamespaceName;
                AddNamespace(Common.DefaultNamespace, this.defaultNamespace);
                var nav = doc.CreateNavigator();
                while (nav.MoveToFollowing(XPathNodeType.Element))
                {
                    foreach (var item in nav.GetNamespacesInScope(XmlNamespaceScope.All))
                    {
                        if (nslist.ContainsKey(item.Key))
                            continue;
                        nslist.Add(item.Key, item.Value);
                    }
                }
            }
            foreach (var item in nslist)
            {
                if(string.IsNullOrWhiteSpace(item.Key))
                    continue;
                if (!this.HasNamespace(item.Key))
                {
                    AddNamespace(item.Key, item.Value);
                }
            }
        }
    }
}
