using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GillSoft.XmlComparer
{
    public static class XmlExtensionMethods
    {
        public static string FQN(this XAttribute element)
        {
            if (element == null)
                return string.Empty;

            return "@" + element.Name.LocalName;
        }

        public static string FQN(this XElement element)
        {
            if (element == null)
                return string.Empty;

            var ns = element.Name.NamespaceName;
            var prefix = element.Document.Root.GetPrefixOfNamespace(ns);

            if (string.IsNullOrWhiteSpace(prefix))
            {
                var nsRoot = element.Document.Root.GetDefaultNamespace().NamespaceName;
                if (!string.IsNullOrWhiteSpace(nsRoot))
                {
                    if (string.IsNullOrWhiteSpace(prefix) && nsRoot.Equals(ns))
                    {
                        prefix = Common.DefaultNamespace;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = prefix + ":";

            return prefix + element.Name.LocalName;
        }

        public static List<XAttribute> GetAttributes(this XElement element)
        {
            var res = new List<XAttribute>();

            if (element != null)
            {
                res.AddRange(element.Attributes().Where(a => !a.ToString().StartsWith("xmlns")));
            }

            return res;
        }


        public static string GetXPath(this XElement element, KeyValueElementInfo keyValueInfo)
        {
            if (element == null)
                return string.Empty;

            if (element == element.Document.Root)
            {
                var s = "/" + element.FQN();
                return s;
            }

            var kvParent = element.Parent.GetBestKeyValueInfo();

            var res = element.Parent.GetXPath(kvParent) + "/" + element.FQN();

            var attribs = element.GetAttributes();

            if (attribs.Any())
            {
                if (keyValueInfo == null)
                {
                    var filter = "[" + string.Join(" and ", element.GetAttributes().Select(a => a.FQN() + "=" + "\"" + a.Value.XmlEncode() + "\"")) + "]";
                    res += filter;
                }
                else
                {
                    var filterAttrs = element.GetAttributes().Where(a => keyValueInfo.KeyNames.Any(k => k == a.Name.LocalName)).ToList();
                    if (!filterAttrs.Any())
                        Debugger.Break();
                    var filter = "[" + string.Join(" and ", filterAttrs.Select(a => a.FQN() + "=" + "\"" + a.Value.XmlEncode() + "\"")) + "]";
                    res += filter;
                }
            }

            return res;
        }
        
        public static string GetXPath(this XAttribute attribute)
        {
            var kvParent = attribute.Parent.GetBestKeyValueInfo();
            var res = attribute.Parent.GetXPath(kvParent) + "/" + attribute.FQN();
            return res;
        }

        internal static KeyValueElementInfo GetBestKeyValueInfo(this XElement element)
        {
            var res = default(KeyValueElementInfo);
            if (element != null)
            {
                var attribNames = element.GetAttributes().Select(a => a.Name.LocalName);
                res = Common.commonKeyValues
                    .Where(a => attribNames.Count() > 1 || string.IsNullOrWhiteSpace(a.ValueName))
                    .Select(a => new { MatchCount = KeyValueElementInfo.KeyMatchCount(a, element.Name.LocalName, attribNames), KeyValueInfo = a, })
                    .OrderByDescending(a => a.MatchCount)
                    .Where(a => a.MatchCount >= (attribNames.Count() == 1 ? 1 : 2))
                    .Select(a => a.KeyValueInfo)
                    .FirstOrDefault()
                    ;
            }
            return res;
        }

        public static string XmlEncode(this string value)
        {
            var res = string.Empty;
            if (value != null)
            {
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Auto }))
                {
                    writer.WriteString(value);
                }
                res = sb.ToString();
            }
            return res;
        }

        public static int LineNumber(this XElement element)
        {
            if (element == null)
                return 0;
            var res = ((IXmlLineInfo)element).LineNumber;
            return res;
        }

    }
}
