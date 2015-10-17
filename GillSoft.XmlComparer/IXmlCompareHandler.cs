using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GillSoft.XmlComparer
{
    public interface IXmlCompareHandler
    {
        void ElementAdded(string xPath, XElement element);

        void ElementRemoved(string xPath, XElement element);

        void ElementChanged(string xPath, XElement leftElement, XElement rightElement);

        void AttributeAdded(string xPath, XAttribute attribute);

        void AttributeRemoved(string xPath, XAttribute attribute);

        void AttributeChanged(string xPath, XAttribute leftAttribute, XAttribute rightAttribute);
    }
}
