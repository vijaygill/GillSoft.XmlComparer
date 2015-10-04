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
                using(var comparer = new XmlComparer.Comparer(p))
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
            Console.WriteLine("++E: " + element);
        }

        public void ElementRemoved(System.Xml.Linq.XElement element)
        {
            Console.WriteLine("--E: " + element);
        }

        public void ElementChanged(System.Xml.Linq.XElement leftElement, System.Xml.Linq.XElement rightElement)
        {
            Console.WriteLine("<>E: " + leftElement);
        }

        public void AttributeAdded(System.Xml.Linq.XAttribute attribute)
        {
            Console.WriteLine("++A: " + attribute);
        }

        public void AttributeRemoved(System.Xml.Linq.XAttribute attribute)
        {
            Console.WriteLine("--A: " + attribute);
        }

        public void AttributeChanged(System.Xml.Linq.XAttribute leftAttribute, System.Xml.Linq.XAttribute rightAttribute)
        {
            Console.WriteLine("<>A: " + leftAttribute);
        }
    }
}
