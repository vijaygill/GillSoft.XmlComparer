using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GillSoft.XmlComparer
{
    public class Comparer : IDisposable
    {
        private readonly IXmlCompareHandler xmlCompareHandler;

        public Comparer(IXmlCompareHandler xmlCompareHandler)
        {
            this.xmlCompareHandler = xmlCompareHandler;
        }

        public void Compare(string file1, string file2, IXmlCompareHandler callback)
        {
            var loadOptions = LoadOptions.SetBaseUri | LoadOptions.SetLineInfo;

            var doc1 = XDocument.Load(file1, loadOptions);
            var doc2 = XDocument.Load(file2, loadOptions);

            var nsm1 = new XmlNamespaceManagerEnhanced(doc1);
            var nsm2 = new XmlNamespaceManagerEnhanced(doc2);

            var elemsAll = doc1.Descendants().Where(a => a != doc1.Root).ToList();
            elemsAll.AddRange(doc2.Descendants().Where(a => a != doc2.Root).ToList());

            var elems = elemsAll.Select(a => new { Element = a, KeyValueInfo = a.GetBestKeyValueInfo(), LineNumber = ((IXmlLineInfo)a).LineNumber })
                .Select(a => new { Element = a.Element, KeyValueInfo = a.KeyValueInfo, XPath = a.Element.GetXPath(a.KeyValueInfo == null), LineNumber = a.LineNumber }) // if KV is null, use all attributes for filter
                .GroupBy(a => a.XPath)
                .Select(g => new { XPath = g.Key, Element = g.First().Element, KeyValueInfo = g.First().KeyValueInfo, LineNumber = g.First().LineNumber })
                .OrderBy(a => a.LineNumber)
                .ToList();

            var xPathsToIgnore = new List<string>();


            foreach (var item in elems)
            {
                //Console.WriteLine(item.XPath);
                // compare elements in both documents

                if (xPathsToIgnore.Any(a => a == item.XPath))
                    continue;

                var node1 = default(XElement);
                try
                {
                    node1 = doc1.XPathSelectElements(item.XPath, nsm1).FirstOrDefault();
                }
                catch
                {
                }
                var node2 = default(XElement);
                try
                {
                    node2 = doc2.XPathSelectElements(item.XPath, nsm2).FirstOrDefault();
                }
                catch
                {
                }

                if (node1 == null && node2 != null)
                {
                    //added
                    xPathsToIgnore.AddRange(elems.Where(a => a.XPath.StartsWith(item.XPath)).Select(a => a.XPath));

                    callback.ElementAdded(node2);
                    continue;
                }


                if (node1 != null && node2 == null)
                {
                    //removed
                    xPathsToIgnore.AddRange(elems.Where(a => a.XPath.StartsWith(item.XPath)).Select(a => a.XPath));

                    callback.ElementRemoved(node1);
                    continue;
                }


                if (node1 != null && node2 != null)
                {
                    //might have changed
                    //compare values

                    CompareAttributes(node1, node2, callback);

                    if (node1.HasElements || node2.HasElements)
                    {
                        //if there are child elements, ignore these nodes
                        //as those elements will be handled individually
                        continue;
                    }

                    xPathsToIgnore.AddRange(elems.Where(a => a.XPath.StartsWith(item.XPath)).Select(a => a.XPath));

                    var val1 = node1.Value;
                    var val2 = node2.Value;

                    if (string.Equals(val1, val2))
                        continue;

                    callback.ElementChanged(node1, node2);
                    continue;
                }

                throw new Exception("Invalid scenario while comparing elements: " + item.XPath);
            }
        }

        private void CompareAttributes(XElement node1, XElement node2, IXmlCompareHandler callback)
        {
            var attribsAll = node1.GetAttributes().ToList();
            attribsAll.AddRange(node2.GetAttributes().ToList());

            var attribs = attribsAll.GroupBy(a => a.Name.ToString())
                .Select(a => a.First())
                .ToList();

            foreach (var attrib in attribs)
            {
                // compare Attributes in both documents
                var attribute1 = node1.Attribute(attrib.Name);
                var attribute2 = node2.Attribute(attrib.Name);

                if (attribute1 == null && attribute2 != null)
                {
                    //added
                    callback.AttributeAdded(attribute2);
                    continue;
                }


                if (attribute1 != null && attribute2 == null)
                {
                    //removed
                    callback.AttributeRemoved(attribute1);
                    continue;
                }


                if (attribute1 != null && attribute2 != null)
                {
                    //might have changed
                    //compare values

                    var val1 = attribute1.Value;
                    var val2 = attribute2.Value;

                    if (string.Equals(val1, val2))
                        continue;

                    callback.AttributeChanged(attribute1, attribute2);
                    continue;
                }

                throw new Exception("Invalid scenario while comparing Attributes: " + attrib);
            }

        }

        public void Dispose()
        {
        }
    }
}
