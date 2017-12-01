﻿using System;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Models
{
    // fixme - need to reorg our tests!
    // fixme - test IsDirty on properties

    [TestFixture]
    public class VariationTests
    {
        [SetUp]
        public void SetUp()
        {
            // annoying, but content type wants short string helper ;(
            SettingsForTests.Reset();
        }

        [Test]
        public void PropertyTests()
        {
            var propertyType = new PropertyType("editor", DataTypeDatabaseType.Nvarchar) { Alias = "prop" };
            var prop = new Property(propertyType);

            // can set value
            // and get edited and published value
            // because non-publishing
            prop.SetValue("a");
            Assert.AreEqual("a", prop.GetValue());
            Assert.AreEqual("a", prop.GetValue(published: true));

            // illegal, 'cos non-publishing
            Assert.Throws<NotSupportedException>(() => prop.PublishValue());

            // change
            propertyType.IsPublishing = true;

            // can get value
            // and now published value is null
            Assert.AreEqual("a", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));

            // cannot set non-supported variation value
            Assert.Throws<NotSupportedException>(() => prop.SetValue("x", 1));
            Assert.IsNull(prop.GetValue(1));

            // can publish value
            // and get edited and published values
            prop.PublishValue();
            Assert.AreEqual("a", prop.GetValue());
            Assert.AreEqual("a", prop.GetValue(published: true));

            // can set value
            // and get edited and published values
            prop.SetValue("b");
            Assert.AreEqual("b", prop.GetValue());
            Assert.AreEqual("a", prop.GetValue(published: true));

            // can clear value
            prop.ClearPublishedValue();
            Assert.AreEqual("b", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));

            // change
            propertyType.Variations |= ContentVariation.CultureNeutral;

            // can set value
            // and get values
            prop.SetValue("c", 1);
            Assert.AreEqual("b", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.IsNull(prop.GetValue(1, published: true));

            // can publish value
            // and get edited and published values
            prop.PublishValue(1);
            Assert.AreEqual("b", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.AreEqual("c", prop.GetValue(1, published: true));

            // can clear all
            prop.ClearPublishedAllValues();
            Assert.AreEqual("b", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.IsNull(prop.GetValue(1, published: true));

            // can publish all
            prop.PublishAllValues();
            Assert.AreEqual("b", prop.GetValue());
            Assert.AreEqual("b", prop.GetValue(published: true));
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.AreEqual("c", prop.GetValue(1, published: true));

            // same for culture
            prop.ClearPublishedCultureValues(1);
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.IsNull(prop.GetValue(1, published: true));
            prop.PublishCultureValues(1);
            Assert.AreEqual("c", prop.GetValue(1));
            Assert.AreEqual("c", prop.GetValue(1, published: true));

            prop.ClearPublishedCultureValues();
            Assert.AreEqual("b", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));
            prop.PublishCultureValues();
            Assert.AreEqual("b", prop.GetValue());
            Assert.AreEqual("b", prop.GetValue(published: true));
        }

        [Test]
        public void ContentTests()
        {
            var propertyType = new PropertyType("editor", DataTypeDatabaseType.Nvarchar) { Alias = "prop" };
            var contentType = new ContentType(-1) { Alias = "contentType" };
            contentType.AddPropertyType(propertyType);

            var content = new Content("content", -1, contentType) { Id = 1, VersionId = 1 };

            // can set value
            // and get edited value, published is null
            // because publishing
            content.SetValue("prop", "a");
            Assert.AreEqual("a", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));

            // cannot set non-supported variation value
            Assert.Throws<NotSupportedException>(() => content.SetValue("prop", "x", 1));
            Assert.IsNull(content.GetValue("prop", 1));

            // can publish value
            // and get edited and published values
            content.PublishValues();
            Assert.AreEqual("a", content.GetValue("prop"));
            Assert.AreEqual("a", content.GetValue("prop", published: true));

            // can set value
            // and get edited and published values
            content.SetValue("prop", "b");
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.AreEqual("a", content.GetValue("prop", published: true));

            // can clear value
            content.ClearPublishedValues();
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));

            // change
            propertyType.Variations |= ContentVariation.CultureNeutral;

            // can set value
            // and get values
            content.SetValue("prop", "c", 1);
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.IsNull(content.GetValue("prop", 1, published: true));

            // can publish value
            // and get edited and published values
            content.PublishValues(1);
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.AreEqual("c", content.GetValue("prop", 1, published: true));

            // can clear all
            content.ClearAllPublishedValues();
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.IsNull(content.GetValue("prop", 1, published: true));

            // can publish all
            content.PublishAllValues();
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.AreEqual("b", content.GetValue("prop", published: true));
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.AreEqual("c", content.GetValue("prop", 1, published: true));

            // same for culture
            content.ClearCulturePublishedValues(1);
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.IsNull(content.GetValue("prop", 1, published: true));
            content.PublishCultureValues(1);
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.AreEqual("c", content.GetValue("prop", 1, published: true));

            content.ClearCulturePublishedValues();
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));
            content.PublishCultureValues();
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.AreEqual("b", content.GetValue("prop", published: true));

            var other = new Content("other", -1, contentType) { Id = 2, VersionId = 1 };
            other.SetValue("prop", "o");
            other.SetValue("prop", "o1", 1);

            // can copy other's edited value
            content.CopyAllValues(other);
            Assert.AreEqual("o", content.GetValue("prop"));
            Assert.AreEqual("b", content.GetValue("prop", published: true));
            Assert.AreEqual("o1", content.GetValue("prop", 1));
            Assert.AreEqual("c", content.GetValue("prop", 1, published: true));

            // can copy self's published value
            content.CopyAllValues(content);
            Assert.AreEqual("b", content.GetValue("prop"));
            Assert.AreEqual("b", content.GetValue("prop", published: true));
            Assert.AreEqual("c", content.GetValue("prop", 1));
            Assert.AreEqual("c", content.GetValue("prop", 1, published: true));
        }

        [Test]
        public void IsDirtyTests()
        {
            var propertyType = new PropertyType("editor", DataTypeDatabaseType.Nvarchar) { Alias = "prop" };
            var prop = new Property(propertyType);
            var contentType = new ContentType(-1) { Alias = "contentType" };
            contentType.AddPropertyType(propertyType);

            var content = new Content("content", -1, contentType) { Id = 1, VersionId = 1 };

            prop.SetValue("a");
            Assert.AreEqual("a", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));

            Assert.IsTrue(prop.IsDirty());

            content.SetValue("prop", "a");
            Assert.AreEqual("a", content.GetValue("prop"));
            Assert.IsNull(content.GetValue("prop", published: true));

            Assert.IsTrue(content.IsDirty());
            Assert.IsTrue(content.IsAnyUserPropertyDirty());
            // how can we tell which variation was dirty?
        }

        [Test]
        public void ValidationTests()
        {
            var propertyType = new PropertyType("editor", DataTypeDatabaseType.Nvarchar) { Alias = "prop", IsPublishing = true };
            var prop = new Property(propertyType);

            prop.SetValue("a");
            Assert.AreEqual("a", prop.GetValue());
            Assert.IsNull(prop.GetValue(published: true));

            Assert.IsTrue(prop.IsValid());

            propertyType.Mandatory = true;
            Assert.IsTrue(prop.IsValid());

            prop.SetValue(null);
            Assert.IsFalse(prop.IsValid());

            // cannot publish, because invalid
            Assert.Throws<InvalidOperationException>(() => prop.PublishValue());
        }
    }
}
