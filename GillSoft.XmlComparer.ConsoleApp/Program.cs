using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GillSoft.XmlComparer.ConsoleApp
{
    class Program : IXmlCompareHandler
    {
        static void Main(string[] args)
        {
            try
            {
                var file1 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\App.config";
                var file2 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\GSSalesManager.exe.config";

                file1 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\TheBrowBarLounge.csproj";
                file2 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\TheBrowBarLounge.Deploy.csproj";

                //file1 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\a1.config";
                //file2 = @"C:\Temp\Dev\GillSoft.XmlComparer\Test\a2.config";

                var p = new Program();

                using (var comparer = new XmlComparer.Comparer(p))
                {
                    comparer.Compare(file1, file2, p);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
            Console.Write("Press RETURN to close...");
            Console.ReadLine();
        }



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
