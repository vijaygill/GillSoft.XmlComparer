using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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



        public void ElementAdded(System.Xml.Linq.XElement element)
        {
            Console.WriteLine("++E: {0,-8}: {1}", element.LineNumber(), element);
            Console.WriteLine();
        }

        public void ElementRemoved(System.Xml.Linq.XElement element)
        {
            Console.WriteLine("++E: {0,-8}: {1}", element.LineNumber(), element);
            Console.WriteLine();
        }

        public void ElementChanged(System.Xml.Linq.XElement leftElement, System.Xml.Linq.XElement rightElement)
        {
            Console.WriteLine("<>E: {0,-8}: {1}", leftElement.LineNumber(), leftElement.Value);
            Console.WriteLine("  >: {0,-8}: {1}", rightElement.LineNumber(), rightElement.Value);
            Console.WriteLine();
        }

        public void AttributeAdded(System.Xml.Linq.XAttribute attribute)
        {
            Console.WriteLine("++A: {0,-8}: {1}", attribute.Parent.LineNumber(), attribute.Value);
            Console.WriteLine();
        }

        public void AttributeRemoved(System.Xml.Linq.XAttribute attribute)
        {
            Console.WriteLine("--A: {0,-8}: {1}", attribute.Parent.LineNumber(), attribute.Value);
            Console.WriteLine();
        }

        public void AttributeChanged(System.Xml.Linq.XAttribute leftAttribute, System.Xml.Linq.XAttribute rightAttribute)
        {
            Console.WriteLine("<>A: {0,-8}: {1}", leftAttribute.Parent.LineNumber(), leftAttribute.Value);
            Console.WriteLine("  >: {0,-8}: {1}", rightAttribute.Parent.LineNumber(), rightAttribute.Value);
            Console.WriteLine();
        }
    }
}
