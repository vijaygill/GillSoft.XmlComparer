using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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

        public XmlNamespaceManagerEnhanced(XDocument doc1)
            : base(doc1.CreateReader().NameTable)
        {
            var ns = doc1.Root.GetDefaultNamespace();
            if (ns != null && !string.IsNullOrWhiteSpace(ns.NamespaceName))
            {
                this.defaultNamespace = ns.NamespaceName;
                AddNamespace(Common.DefaultNamespace, this.defaultNamespace);
            }
        }
    }
}
