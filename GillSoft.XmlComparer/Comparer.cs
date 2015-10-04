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

            var elems = elemsAll.Select(a => new { Element = a, KeyValueInfo = a.GetBestKeyValueInfo() })
                .Select(a => new { Element = a.Element, KeyValueInfo = a.KeyValueInfo, XPath = a.Element.GetXPath(a.KeyValueInfo == null) }) // if KV is null, use all attributes for filter
                .GroupBy(a => a.XPath)
                .Select(g => new { XPath = g.Key, Element = g.First().Element, KeyValueInfo = g.First().KeyValueInfo })
                .ToList();


            foreach (var item in elems)
            {
                //Console.WriteLine(item.XPath);
                // compare elements in both documents
                var node1 = doc1.XPathSelectElement(item.XPath, nsm1);
                var node2 = doc2.XPathSelectElement(item.XPath, nsm2);

                if (node1 == null && node2 != null)
                {
                    //added
                    callback.ElementAdded(node2);
                    continue;
                }


                if (node1 != null && node2 == null)
                {
                    //removed
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
            var attribsAll = node1.Attributes().ToList();
            attribsAll.AddRange(node2.Attributes().ToList());
        }

        public void Dispose()
        {
        }
    }
}
