using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GillSoft.XmlComparer
{
    public interface IXmlCompareHandler
    {
        void ElementAdded(XElement element);

        void ElementRemoved(XElement element);

        void ElementChanged(XElement leftElement, XElement rightElement);

        void AttributeAdded(XAttribute attribute);

        void AttributeRemoved(XAttribute attribute);

        void AttributeChanged(XAttribute leftAttribute, XAttribute rightAttribute);
    }
}
