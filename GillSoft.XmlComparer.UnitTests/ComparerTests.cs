using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GillSoft.XmlComparer.UnitTests
{
    [TestFixture]
    public class ComparerTests
    {
        private class TestCategories 
        {
            public const string NameValueElements = "Name Value Elements";
            public const string GeneralElements = "General Elements";
        }

        private static Stream GetStream(string value)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(value);
            sw.Flush();
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static string GetString(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(stream);
            var res = sr.ReadToEnd();
            return res;
        }

        public void NoChange_SameOrderOfElements()
        {
            //Arrange
            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" />
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1""/>
</root>
";

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            //mockHandler.Setup(a => a.AttributeAdded(It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.AttributeRemoved(It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.ElementAdded(It.IsAny<XElement>()));
            //mockHandler.Setup(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()));
            //mockHandler.Setup(a => a.ElementRemoved(It.IsAny<XElement>()));

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

        }

        [Test]
        public void NoChange_DifferentOrderOfElements()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" />
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
<add name=""name1"" value=""value1""/>
  <elem2>This is element 2</elem2>
  <elem1>This is element 1</elem1>
</root>
";

            #endregion

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

        }

        [Test]
        [Category(TestCategories.NameValueElements)]
        public void NameValueElement_AttributeAdded()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" />
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" newAttribute=""new value""/>
</root>
";
            #endregion

            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeAdded(It.IsAny<XAttribute>())).Callback<XAttribute>((a) => { newValue = a.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Once);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual("new value", newValue);

        }

        [Test]
        [Category(TestCategories.NameValueElements)]
        public void NameValueElement_AttributeRemoved()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" />
</root>
";

            #endregion

            var oldValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeRemoved(It.IsAny<XAttribute>())).Callback<XAttribute>((a) => { oldValue = a.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Once);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual("to be deleted", oldValue);

        }

        [Test]
        [Category(TestCategories.NameValueElements)]
        public void NameValueElement_AttributeChanged()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" />
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value2"" />
</root>
";
            #endregion

            var oldValue = string.Empty;
            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()))
                .Callback<XAttribute, XAttribute>((left, right) => { oldValue = left.Value; newValue = right.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Once);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual("value1", oldValue);
            Assert.AreEqual("value2", newValue);

        }

        [Test]
        [Category(TestCategories.NameValueElements)]
        public void NameValueElement_ElementAdded()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" newAttribute=""new value""/>
</root>
";

            #endregion

            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementAdded(It.IsAny<XElement>())).Callback<XElement>((a) => { newValue = a.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Once);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual(@"<add name=""name1"" value=""value1"" newAttribute=""new value"" />", newValue);

        }

        [Test]
        [Category(TestCategories.NameValueElements)]
        public void NameValueElement_ElementRemoved()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
</root>
";
            #endregion

            var oldValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementRemoved(It.IsAny<XElement>())).Callback<XElement>((a) => { oldValue = a.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Once);

            Assert.AreEqual(@"<add name=""name1"" value=""value1"" oldAttribute=""to be deleted"" />", oldValue);

        }

        [Test]
        [Category(TestCategories.GeneralElements)]
        public void GeneralElement_ElementAdded()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";
            #endregion

            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementAdded(It.IsAny<XElement>())).Callback<XElement>((a) => { newValue = a.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Once);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual(@"<elem2>This is element 2</elem2>", newValue);

        }

        [Test]
        [Category(TestCategories.GeneralElements)]
        public void GeneralElement_ElementRemoved()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1>This is element 1</elem1>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";
            #endregion

            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementRemoved(It.IsAny<XElement>())).Callback<XElement>((a) => { newValue = a.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Once);

            Assert.AreEqual(@"<elem2>This is element 2</elem2>", newValue);

        }

        [Test]
        [Category(TestCategories.GeneralElements)]
        public void GeneralElement_ElementChanged()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1 name=""MyLogger"">This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1 name=""MyLogger"">This is element 1 but changed now</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";
            #endregion

            var oldValue = string.Empty;
            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>())).Callback<XElement, XElement>((left, right) => { oldValue = left.ToString(); newValue = right.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Once);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual(@"<elem1 name=""MyLogger"">This is element 1 but changed now</elem1>", newValue);

        }

        [Test]
        [Category(TestCategories.GeneralElements)]
        public void GeneralElement_ChildElementChanged()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1 name=""MyLogger""><child>This is element 1</child></elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
  <elem1 name=""MyLogger""><child>This is element 1 but changed now</child></elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""to be deleted""/>
</root>
";
            #endregion

            var oldValue = string.Empty;
            var newValue = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>())).Callback<XElement, XElement>((left, right) => { oldValue = left.ToString(); newValue = right.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<XAttribute>(), It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<XAttribute>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<XElement>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<XElement>(), It.IsAny<XElement>()), Times.Once);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<XElement>()), Times.Never);

            Assert.AreEqual(@"<child>This is element 1 but changed now</child>", newValue);

        }
    }
}
