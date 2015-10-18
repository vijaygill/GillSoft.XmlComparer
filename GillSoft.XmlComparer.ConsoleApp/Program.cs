using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GillSoft.XmlComparer.ConsoleApp
{
    class Program 
    {
        static void Main(string[] args)
        {
            try
            {
                var file1 = @".\TestFiles\a1.config";
                var file2 = @".\TestFiles\a2.config";

                var handler = new TestXmlCompareHandler();

                using (var comparer = new Comparer(handler))
                {
                    comparer.Compare(file1, file2, handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
            Console.Write("Press RETURN to close...");
            Console.ReadLine();
        }

    }
}
