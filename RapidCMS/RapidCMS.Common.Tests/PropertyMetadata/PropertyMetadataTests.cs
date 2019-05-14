using System;
using System.Linq.Expressions;
using NUnit.Framework;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models;

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

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("Test", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(BasicClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Test);
        }

        [Test]
        public void NestedProperty()
        {
            var instance = new ParentClass { Basic = new BasicClass { Test = "Test Value" } };
            Expression<Func<ParentClass, string>> func = (ParentClass x) => x.Basic.Test;

            var data = PropertyMetadataHelper.GetPropertyMetadata(func);

            Assert.IsNotNull(data);
            Assert.AreEqual("BasicTest", data.PropertyName);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            data.Setter(instance, "New Value");

            Assert.AreEqual("New Value", instance.Basic.Test);
        }

        [Test]
        public void BasicStringExpression()
        {
            var instance = new BasicClass { Test = "Test Value" };
            Expression<Func<BasicClass, string>> func = (BasicClass x) => $"{x.Test}";

            var data = PropertyMetadataHelper.GetExpressionMetadata(func) as IExpressionMetadata;

            Assert.IsNotNull(data);
            Assert.AreEqual("Test Value", data.Getter(instance));
            Assert.AreEqual(typeof(string), data.PropertyType);
            Assert.AreEqual(typeof(ParentClass), data.ObjectType);

            Assert.Throws(typeof(ArgumentException), () => PropertyMetadataHelper.GetPropertyMetadata(func));
        }

        class BasicClass
        {
            public string Test { get; set; }
        }

        class ParentClass
        {
            public BasicClass Basic { get; set; }
        }
    }
}
