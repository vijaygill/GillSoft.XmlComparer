using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GillSoft.XmlComparer
{
    public interface IXmlCompareHandler
    {
        void ElementAdded(ElementAddedEventArgs e);

        void ElementRemoved(ElementRemovedEventArgs e);

        void ElementChanged(ElementChangedEventArgs e);

        void AttributeAdded(AttributeAddedEventArgs e);

        void AttributeRemoved(AttributeRemovedEventArgs e);

        void AttributeChanged(AttributeChangedEventArgs e);
    }
}
