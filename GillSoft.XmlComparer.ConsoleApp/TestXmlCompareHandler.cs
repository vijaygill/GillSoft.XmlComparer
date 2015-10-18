using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.XmlComparer.ConsoleApp
{
    public class TestXmlCompareHandler : IXmlCompareHandler
    {
        public void ElementAdded(ElementAddedEventArgs e)
        {
            Console.WriteLine("++E: {0,-8}: {1}", e.LineNumber, e.XPath);
            Console.WriteLine("   : {0}", e.Element);
            Console.WriteLine();
        }

        public void ElementRemoved(ElementRemovedEventArgs e)
        {
            Console.WriteLine("--E: {0,-8}: {1}", e.LineNumber, e.XPath);
            Console.WriteLine("   : {0}", e.Element);
            Console.WriteLine();
        }

        public void ElementChanged(ElementChangedEventArgs e)
        {
            Console.WriteLine("<>E: {0}", e.XPath);
            Console.WriteLine("  o: {0,-8}: {1}", e.LeftLineNumber, e.LeftElement.Value);
            Console.WriteLine("  n: {0,-8}: {1}", e.RightLineNumber, e.RightElement.Value);
            Console.WriteLine();
        }

        public void AttributeAdded(AttributeAddedEventArgs e)
        {
            Console.WriteLine("++A: {0,-8}: {1}", e.LineNumber, e.XPath);
            Console.WriteLine("   : {0}", e.Attribute.Value);
            Console.WriteLine();
        }

        public void AttributeRemoved(AttributeRemovedEventArgs e)
        {
            Console.WriteLine("--A: {0,-8}: {1}", e.LineNumber, e.XPath);
            Console.WriteLine("   : {0}", e.Attribute.Value);
            Console.WriteLine();
        }

        public void AttributeChanged(AttributeChangedEventArgs e)
        {
            Console.WriteLine("<>A: {0}", e.XPath);
            Console.WriteLine("  o: {0,-8}: {1}", e.LeftLineNumber, e.LeftAttribute.Value);
            Console.WriteLine("  n: {0,-8}: {1}", e.RightLineNumber, e.RightAttribute.Value);

            Console.WriteLine();
        }

    }
}
