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
            public const string General = "General";
            public const string NameValueElements = "Name Value Elements";
            public const string GeneralElements = "Non Name Value Elements";
            public const string GeneralWithNamespace = "General With Namespace";
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

        #region General tests

        [Test]
        [Category(TestCategories.General)]
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
        [Category(TestCategories.General)]
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


        #endregion

        #region Attribute tests

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
        public void NameValueElement_AttributeAdded_WithNamespace()
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
<root xmlns:ns1=""http://www.example.com/ns1"">
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" newAttribute=""new value"" ns1:newAttribute=""new value in ns1""/>
</root>
";
            #endregion

            var newValue1 = string.Empty;
            var newValue2 = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>())).Callback<AttributeAddedEventArgs>((e) =>
            {
                if (e.Attribute.Name.Namespace == "")
                {
                    newValue1 = e.Attribute.Value;
                }
                if (e.Attribute.Name.Namespace == "http://www.example.com/ns1")
                {
                    newValue2 = e.Attribute.Value;
                }
            });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Exactly(2));
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

            Assert.AreEqual("new value", newValue1);
            Assert.AreEqual("new value in ns1", newValue2);
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
        public void NameValueElement_AttributeRemoved_WithNamespace()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root xmlns:ns1=""http://www.example.com/ns1"">
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" oldAttribute=""old value"" ns1:oldAttribute=""old value in ns1""/>
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

            var oldValue1 = string.Empty;
            var oldValue2 = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>())).Callback<AttributeRemovedEventArgs>((e) =>
            {
                if (e.Attribute.Name.Namespace == "")
                {
                    oldValue1 = e.Attribute.Value;
                }
                if (e.Attribute.Name.Namespace == "http://www.example.com/ns1")
                {
                    oldValue2 = e.Attribute.Value;
                }
            });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Exactly(2));
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

            Assert.AreEqual("old value", oldValue1);
            Assert.AreEqual("old value in ns1", oldValue2);
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
        public void NameValueElement_WithNamespace_AttributeChanged()
        {
            //Arrange

            #region sample values

            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root xmlns:ns1=""http://www.example.com/ns1"">
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" newAttribute=""old value"" ns1:newAttribute=""old value in ns1""/>
</root>
";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root xmlns:ns1=""http://www.example.com/ns1"">
  <elem1>This is element 1</elem1>
  <elem2>This is element 2</elem2>
<add name=""name1"" value=""value1"" newAttribute=""new value"" ns1:newAttribute=""new value in ns1""/>
</root>
";
            #endregion

            var oldValue1 = string.Empty;
            var newValue1 = string.Empty;

            var oldValue2 = string.Empty;
            var newValue2 = string.Empty;

            var mockHandler = new Mock<IXmlCompareHandler>(MockBehavior.Strict);
            mockHandler.Setup(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()))
                .Callback<AttributeChangedEventArgs>((e) =>
                {
                    if (e.LeftAttribute.Name.Namespace == "")
                    {
                        oldValue1 = e.LeftAttribute.Value;
                    }
                    if (e.LeftAttribute.Name.Namespace == "http://www.example.com/ns1")
                    {
                        oldValue2 = e.LeftAttribute.Value;
                    }

                    if (e.RightAttribute.Name.Namespace == "")
                    {
                        newValue1 = e.RightAttribute.Value;
                    }
                    if (e.RightAttribute.Name.Namespace == "http://www.example.com/ns1")
                    {
                        newValue2 = e.RightAttribute.Value;
                    }
                });

            var comparer = new Comparer(mockHandler.Object);

            //act
            comparer.Compare(GetStream(xml1), GetStream(xml2), mockHandler.Object);

            //assert

            mockHandler.Verify(a => a.AttributeAdded(It.IsAny<AttributeAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.AttributeChanged(It.IsAny<AttributeChangedEventArgs>()), Times.Exactly(2));
            mockHandler.Verify(a => a.AttributeRemoved(It.IsAny<AttributeRemovedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementAdded(It.IsAny<ElementAddedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementChanged(It.IsAny<ElementChangedEventArgs>()), Times.Never);
            mockHandler.Verify(a => a.ElementRemoved(It.IsAny<ElementRemovedEventArgs>()), Times.Never);

            Assert.AreEqual("old value", oldValue1);
            Assert.AreEqual("new value", newValue1);

            Assert.AreEqual("old value in ns1", oldValue2);
            Assert.AreEqual("new value in ns1", newValue2);

        }

        #endregion

        #region Element tests

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

        #endregion

    }
}