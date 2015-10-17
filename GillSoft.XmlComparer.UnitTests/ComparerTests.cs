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
            //mockHandler.Setup(a => a.AttributeAdded(It.IsAny<string>(),It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.AttributeChanged(It.IsAny<string>(),It.IsAny<XAttribute>(), It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.AttributeRemoved(It.IsAny<string>(),It.IsAny<XAttribute>()));
            //mockHandler.Setup(a => a.ElementAdded(It.IsAny<string>(),It.IsAny<XElement>()));
            //mockHandler.Setup(a => a.ElementChanged(It.IsAny<string>(),It.IsAny<XElement>(), It.IsAny<XElement>()));
            //mockHandler.Setup(a => a.ElementRemoved(It.IsAny<string>(),It.IsAny<XElement>()));

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>())).Callback<AttributeAddedEventArgs>((e) => { newValue = e.Attribute.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()))
                .Callback<AttributeRemovedEventArgs>((e) => { oldValue = e.Attribute.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()))
                .Callback<AttributeChangedEventArgs>((e) => { oldValue = e.LeftAttribute.Value; newValue = e.RightAttribute.Value; });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>())).Callback<ElementAddedEventArgs>((e) => { newValue = e.Element.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>())).Callback<ElementRemovedEventArgs>((e) => { oldValue = e.Element.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Once);

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
            mockHandler.Setup(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>())).Callback<ElementAddedEventArgs>((e) => { newValue = e.Element.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>())).Callback<ElementRemovedEventArgs>((e) => { newValue = e.Element.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Once);

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
            mockHandler.Setup(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()))
                .Callback<ElementChangedEventArgs>((e) => { oldValue = e.LeftElement.ToString(); newValue = e.RightElement.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

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
            mockHandler.Setup(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()))
                .Callback<ElementChangedEventArgs>((e) => { oldValue = e.LeftElement.ToString(); newValue = e.RightElement.ToString(); });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Once);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

            Assert.AreEqual(@"<child>This is element 1 but changed now</child>", newValue);

        }
    }
}
