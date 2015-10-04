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


        public static string GetXPath(this XElement element, bool useAllAttributesAsFilter)
        {
            if (element == null)
                return string.Empty;

            var res = element.Parent.GetXPath(useAllAttributesAsFilter) + "/" + element.FQN();

            if (useAllAttributesAsFilter && element.GetAttributes().Any())
            {
                var filter = "[" + string.Join(" and ", element.GetAttributes().Select(a => "@" + a.Name.LocalName + "=" + "\"" + a.Value.XmlEncode() + "\"")) + "]";
                res += filter;
            }

            return res;
        }

        internal static KeyValueInfo GetBestKeyValueInfo(this XElement element)
        {
            var res = default(KeyValueInfo);
            if (element != null)
            {
                var attribNames = element.GetAttributes().Select(a => a.Name.LocalName).ToList();
                res = Common.commonKeyValues
                    .Select(a => new { MatchCount = a.KeyMatchCount(attribNames), KeyValueInfo = a, })
                    .OrderBy(a => a.MatchCount)
                    .Where(a => a.MatchCount >= 1)
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

    }
}
