namespace OpenStackNetTests.Unit
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [TestClass]
    public class TestExtensibleJsonObject
    {
        [TestMethod]
        public void TestDeserializeEmptyString()
        {
            string @object = "";
            ExtensibleJsonObject result = JsonConvert.DeserializeObject<BasicExtensibleJsonObject>(@object);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeserializeEmptyObject()
        {
            string @object = "{}";
            ExtensibleJsonObject result = JsonConvert.DeserializeObject<BasicExtensibleJsonObject>(@object);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ExtensionData);
            Assert.AreEqual(0, result.ExtensionData.Count);
            Assert.AreEqual(@object, JsonConvert.SerializeObject(result, Formatting.None));
        }

        [TestMethod]
        public void TestUpdateExtensibleObject()
        {
            ExtensibleJsonObject originalObject = new BasicExtensibleJsonObject();
            Assert.IsNotNull(originalObject.ExtensionData);
            Assert.AreEqual("{}", JsonConvert.SerializeObject(originalObject, Formatting.None));

            ExtensibleJsonObject updatedObject = new BasicExtensibleJsonObject(originalObject.ExtensionData.Add("key", "value"));
            Assert.AreEqual("{\"key\":\"value\"}", JsonConvert.SerializeObject(updatedObject, Formatting.None));
            Assert.AreEqual("value", updatedObject.ExtensionData["key"]);
            Assert.AreEqual(1, updatedObject.ExtensionData.Count);

            // the original object did not change
            Assert.AreEqual(0, originalObject.ExtensionData.Count);
        }

        [TestMethod]
        public void TestDeserializeNullValue()
        {
            string @object = "{\"key\":null}";
            ExtensibleJsonObject result = JsonConvert.DeserializeObject<BasicExtensibleJsonObject>(@object);
            Assert.AreEqual(1, result.ExtensionData.Count);
            Assert.AreEqual(@object, JsonConvert.SerializeObject(result, Formatting.None));
            Assert.IsTrue(result.ExtensionData.ContainsKey("key"));
            Assert.IsNull(result.ExtensionData["key"]);

            Assert.IsFalse(result.ExtensionData.ContainsKey("key2"));
        }

        [TestMethod]
        public void TestDeserializeMultipleValues()
        {
            string @object = "{\"key\":\"value\",\"key2\":\"value2\"}";
            ExtensibleJsonObject result = JsonConvert.DeserializeObject<BasicExtensibleJsonObject>(@object);
            Assert.AreEqual(2, result.ExtensionData.Count);
            Assert.AreEqual(@object, JsonConvert.SerializeObject(result, Formatting.None));
            Assert.IsTrue(result.ExtensionData.ContainsKey("key"));
            Assert.AreEqual("value", result.ExtensionData["key"]);
            Assert.IsTrue(result.ExtensionData.ContainsKey("key2"));
            Assert.AreEqual("value2", result.ExtensionData["key2"]);

            Assert.IsFalse(result.ExtensionData.ContainsKey("key3"));
        }

        [TestMethod]
        [ExpectedException(typeof(JsonSerializationException))]
        public void TestDeserializeMultipleValuesSameKey()
        {
            string @object = "{\"key\":\"value\",\"key\":\"value2\"}";
            JsonConvert.DeserializeObject<BasicExtensibleJsonObject>(@object);
        }

        private class BasicExtensibleJsonObject : ExtensibleJsonObject
        {
            public BasicExtensibleJsonObject()
            {
            }

            public BasicExtensibleJsonObject(params JProperty[] extensionData)
                : base(extensionData)
            {
            }

            public BasicExtensibleJsonObject(IEnumerable<JProperty> extensionData)
                : base(extensionData)
            {
            }

            public BasicExtensibleJsonObject(ImmutableDictionary<string, JToken> extensionData)
                : base(extensionData)
            {
            }
        }
    }
}
