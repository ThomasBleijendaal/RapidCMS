using System;
using System.Linq.Expressions;
using NUnit.Framework;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Tests.PropertyMetadata
{
    public class PropertyMetadataTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BasicProperty()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("Test", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Test);
        }

        [Test]
        public void BasicNonStringProperty()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 1 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("Id", data.PropertyName);
            Assert.AreEqual(1, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            data.Setter(instance, 2);

            Assert.AreEqual(2, instance.Id);
        }

        [Test]
        public void NestedProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicTest", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Basic.Test);
        }

        [Test]
        public void NestedNonStringProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
            Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func) as IFullPropertyMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicId", data.PropertyName);
            Assert.AreEqual(1, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            data.Setter(instance, 2);

            Assert.AreEqual(2, instance.Basic.Id);
        }

        [Test]
        public void BasicStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => $"{x.Test} - Blub";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Test Value - Blub", data.StringGetter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Format(\"{0} - Blub\", x.Test)", propertyData.PropertyName);
            Assert.AreEqual("Test Value - Blub", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
        }

        [Test]
        public void BasicStringExpression2()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => $"Blaat";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Blaat", data.StringGetter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => (\"Blaat\" ?? \"\")", propertyData.PropertyName);
            Assert.AreEqual("Blaat", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
        }

        [Test]
        public void BasicStringExpression3()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => string.Join(' ', x.Test.ToCharArray());

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("T e s t   V a l u e", data.StringGetter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Join( , x.Test.ToCharArray())", propertyData.PropertyName);
            Assert.AreEqual("T e s t   V a l u e", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
        }

        [Test]
        public void BasicNonStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 3 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id;

            Assert.Throws<ArgumentException>(() => PropertyMetadataHelper.GetExpressionMetadata(func));
        }

        [Test]
        public void NestedStringExpression()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => $"{x.Basic} {x.Basic.Test}";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("RapidCMS.Common.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", data.StringGetter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);

            var propertyData = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(propertyData);
            Assert.IsNull(propertyData as IFullPropertyMetadata);
            Assert.AreEqual("x => Format(\"{0} {1}\", x.Basic, x.Basic.Test)", propertyData.PropertyName);
            Assert.AreEqual("RapidCMS.Common.Tests.PropertyMetadata.PropertyMetadataTests+BasicClass Test Value", propertyData.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
        }

        [Test]
        public void BasicReadonlyProperty()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => x.Test.ToLower();

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => x.Test.ToLower()", data.PropertyName);
            Assert.AreEqual("test value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void BasicReadonlyNonStringProperty()
        {
            var instance = new BasicClass { Test = "Test Value", Id = 1 };
            Expression<Func<BasicClass, int>> func = (BasicClass x) => x.Id * 2;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => (x.Id * 2)", data.PropertyName);
            Assert.AreEqual(2, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedReadonlyProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test.ToLower();

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => x.Basic.Test.ToLower()", data.PropertyName);
            Assert.AreEqual("test value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        [Test]
        public void NestedReadonlyNonStringProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value", Id = 1 } };
            Expression<Func<ParentClass, int>> func = (ParentClass x) => x.Basic.Id * 2;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("x => (x.Basic.Id * 2)", data.PropertyName);
            Assert.AreEqual(2, data.Getter(instance));
            Assert.AreEqual(typeof(int), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.IsNull(data as IFullPropertyMetadata);
        }

        class BasicClass
        {
            public string Test { get; set; }
            public int Id { get; set; }
        }

        class ParentClass
        {
            public BasicClass Basic { get; set; }
        }
    }
}
