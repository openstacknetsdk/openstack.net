using System;
using System.Globalization;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Acts like Json.NET's [JsonConverter] but allows for constructor arguments
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter)]
    internal sealed class JsonConverterWithConstructorAttribute : Attribute
    {
        private readonly Type _converterType;
        private readonly object[] _constructorArguments;

        public JsonConverterWithConstructorAttribute(Type converterType, params object[] constructorArguments)
        {
            if(converterType == null)
                throw new ArgumentNullException("converterType");
            if(constructorArguments.Length == 0)
                throw new ArgumentException("No constructor arguments were specified. If none are required, use JsonConverterAttribute instead", "constructorArguments");

            _converterType = converterType;
            _constructorArguments = constructorArguments;
        }

        public Type ConverterType
        {
            get { return _converterType; }
        }

        public object[] ConstructorArguments
        {
            get { return _constructorArguments; }
        }

        public JsonConverter CreateJsonConverterInstance()
        {
            try
            {
                return (JsonConverter)Activator.CreateInstance(_converterType, _constructorArguments);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Error creating {0}", _converterType), ex);
            }
        }
    }
}