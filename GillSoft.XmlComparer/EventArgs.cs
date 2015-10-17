using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GillSoft.XmlComparer
{
    public abstract class CompareEventArgs : EventArgs
    {
        public string XPath { get; private set; }

        public CompareEventArgs(string xPath)
        {
            this.XPath = xPath;
        }
    }

    public class ElementAddedEventArgs : CompareEventArgs
    {
        public XElement Element { get; private set; }

        public int LineNumber { get; private set; }

        public ElementAddedEventArgs(string xPath, XElement element, int lineNumber)
            : base(xPath)
        {
            this.Element = element;
            this.LineNumber = lineNumber;
        }
    }

    public class ElementRemovedEventArgs : CompareEventArgs
    {
        public XElement Element { get; private set; }
        public int LineNumber { get; private set; }

        public ElementRemovedEventArgs(string xPath, XElement element, int lineNumber)
            : base(xPath)
        {
            this.Element = element;
            this.LineNumber = lineNumber;
        }
    }

    public class ElementChangedEventArgs : CompareEventArgs
    {
        public XElement LeftElement { get; private set; }
        public int LeftLineNumber { get; private set; }
        
        public XElement RightElement { get; private set; }
        public int RightLineNumber { get; private set; }

        public ElementChangedEventArgs(string xPath, XElement leftElement, int leftLineNumber, XElement rightElement, int rightLineNumber)
            : base(xPath)
        {
            this.LeftElement = leftElement;
            this.LeftLineNumber = leftLineNumber;

            this.RightElement = rightElement;
            this.RightLineNumber = rightLineNumber;
        }
    }

    public class AttributeAddedEventArgs : CompareEventArgs
    {
        public XAttribute Attribute { get; private set; }

        public int LineNumber { get; private set; }

        public AttributeAddedEventArgs(string xPath, XAttribute Attribute, int lineNumber)
            : base(xPath)
        {
            this.Attribute = Attribute;
            this.LineNumber = lineNumber;
        }
    }

    public class AttributeRemovedEventArgs : CompareEventArgs
    {
        public XAttribute Attribute { get; private set; }
        public int LineNumber { get; private set; }

        public AttributeRemovedEventArgs(string xPath, XAttribute Attribute, int lineNumber)
            : base(xPath)
        {
            this.Attribute = Attribute;
            this.LineNumber = lineNumber;
        }
    }

    public class AttributeChangedEventArgs : CompareEventArgs
    {
        public XAttribute LeftAttribute { get; private set; }
        public int LeftLineNumber { get; private set; }

        public XAttribute RightAttribute { get; private set; }
        public int RightLineNumber { get; private set; }

        public AttributeChangedEventArgs(string xPath, XAttribute leftAttribute, int leftLineNumber, XAttribute rightAttribute, int rightLineNumber)
            : base(xPath)
        {
            this.LeftAttribute = leftAttribute;
            this.LeftLineNumber = leftLineNumber;

            this.RightAttribute = rightAttribute;
            this.RightLineNumber = rightLineNumber;
        }
    }
}
