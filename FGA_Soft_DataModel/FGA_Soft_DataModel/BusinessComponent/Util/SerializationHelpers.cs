using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace FGABusinessComponent.BusinessComponent.Util
{
    public class Stringeable : IStringeable
    {
        public const char SEPARATOR = '~';

        public virtual void FromString(string s)
        {
            throw new NotImplementedException();
        }


        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }
    }

    public interface IStringeable : IFormattable
    {
        void FromString(string s);
    }

    public class FlatFileAttribute : Attribute
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public Padding Padding { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFileAttribute"/> class.
        /// </summary>
        /// <param name="position">Each item needs to be ordered so that 
        /// serialization/deserilization works even if the properties 
        /// are reordered in the class.</param>
        /// <param name="length">Total width in the text file</param>
        /// <param name="padding">How to do the padding</param>
        public FlatFileAttribute(int position, int length, Padding padding)
        {
            Position = position;
            Length = length;
            Padding = padding;
        }
    }

    public enum Padding
    {
        Left,
        Right
    }

    public static class SerializationHelpers
    {

        #region Binary Serializer
        /// <summary>
        /// Serialization Object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///Convert a byte array to an Object 
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)binForm.Deserialize(memStream);
            return obj;
        }
        #endregion

        #region XML Serializer Methods

        public static string ObjectToXML<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);

                return writer.ToString();
            }
        }


        public static T XMLToObject<T>(string s)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(s))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
        #endregion

        #region Flat String Serializer Methods
        public static string ObjectToString<T>(T obj) where T : IStringeable
        {
            return obj.ToString();            
        }


        public static T StringToObject<T>(string s) where T : IStringeable, new()
        {
            T obj = new T();
            obj.FromString(s);
            return obj;
        }
        #endregion

        #region Flat String Serializer Methods
        public static string ObjectToString2<T>(T obj)
        {
            using (StringWriter writer = new StringWriter())
            {
                StringSerializer.Serialize(obj, writer);
                return writer.ToString();
            }
        }


        public static T StringToObject2<T>(string s) where T : class, new()
        {

            using (StringReader reader = new StringReader(s))
            {
                return (T)StringSerializer.Deserialize<T>(reader);
            }
        }
        #endregion

    }
    public static class StringSerializer
    {
        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            var attributeType = typeof(FlatFileAttribute);

            return type
                .GetProperties()
                .Where(prop => prop.GetCustomAttributes(attributeType, false).Any())
                .OrderBy(
                    prop =>
                    ((FlatFileAttribute)prop.GetCustomAttributes(attributeType, false).First()).
                        Position);
        }
        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var attributeType = typeof(FlatFileAttribute);

            return type
                .GetFields()
                .Where(prop => prop.GetCustomAttributes(attributeType, false).Any())
                .OrderBy(
                    prop =>
                    ((FlatFileAttribute)prop.GetCustomAttributes(attributeType, false).First()).
                        Position);
        }
        public static void Serialize(object obj, StringWriter writer)
        {
            var attributeType = typeof(FlatFileAttribute);
            var properties = GetProperties(obj.GetType());

            if (properties != null && properties.Count<PropertyInfo>() > 0)
            {
                foreach (var propertyInfo in properties)
                {
                    var value = propertyInfo.GetValue(obj, null).ToString();
                    var attr = (FlatFileAttribute)propertyInfo.GetCustomAttributes(attributeType, false).First();
                    value = attr.Padding == Padding.Left ? value.PadLeft(attr.Length) : value.PadRight(attr.Length);
                    writer.Write(value);
                }
            }
            else
            {
                var fields = GetFields(obj.GetType());
                if (fields != null && fields.Count<FieldInfo>() > 0)
                {
                    foreach (var fieldInfo in fields)
                    {
                        var value = fieldInfo.GetValue(obj).ToString();
                        var attr = (FlatFileAttribute)fieldInfo.GetCustomAttributes(attributeType, false).First();
                        value = attr.Padding == Padding.Left ? value.PadLeft(attr.Length) : value.PadRight(attr.Length);
                        writer.Write(value);
                    }
                }
            }
                writer.WriteLine();
        }

        public static T Deserialize<T>(StringReader reader) where T : class, new()
        {
            var properties = GetProperties(typeof(T));
            var obj = new T();
            var attributeType = typeof(FlatFileAttribute);
            if (properties != null && properties.Count<PropertyInfo>() > 0)
            {
                foreach (var propertyInfo in properties)
                {
                    var attr = (FlatFileAttribute)propertyInfo.GetCustomAttributes(attributeType, false).First();
                    var buffer = new char[attr.Length];
                    reader.Read(buffer, 0, buffer.Length);
                    var value = new string(buffer).Trim();

                    if (propertyInfo.PropertyType != typeof(string))
                        propertyInfo.SetValue(obj, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    else
                        propertyInfo.SetValue(obj, value.Trim(), null);
                }
            }
            else
            {
                var fields = GetFields(obj.GetType());
                if (fields != null && fields.Count<FieldInfo>() > 0)
                {
                    foreach (var fieldInfo in fields)
                    {
                        var attr = (FlatFileAttribute)fieldInfo.GetCustomAttributes(attributeType, false).First();
                        var buffer = new char[attr.Length];
                        reader.Read(buffer, 0, buffer.Length);
                        var value = new string(buffer).Trim();

                        if (fieldInfo.GetType()   != typeof(string))
                            fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.GetType()));
                        else
                            fieldInfo.SetValue(obj, value.Trim());
                    }
                }

            }

            return obj;
        }
    }
}
