using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace VeraSoft.Wpf.Utils
{
    public class XmlTools
    {
        private XmlTools()
        { }

        public static object LoadFromXml(string sXml, Type objectType)
        {
            if (string.IsNullOrEmpty(sXml))
                return null;

            //// Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(objectType);

            object res = null;

            using (TextReader reader = new StringReader(sXml))
            {
                try
                {
                    res = serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("XmlTools::LoadFromXml [ " + objectType.Name + "]: " + ex.Message);
                }
            }

            return res;
        }

        public static T LoadFromXml<T>(string sXml) where T : class
        {
            return LoadFromXml(sXml, typeof(T)) as T;
        }

        public static T LoadFromXmlUsingFileStream<T>(string sXml) where T : class
        {
            return LoadFromXmlUsingFileStream(sXml, typeof(T)) as T;
        }

        public static object LoadFromXmlUsingFileStream(string sXml, Type objectType)
        {
            if (string.IsNullOrEmpty(sXml))
                return null;

            XmlSerializer serializer = new XmlSerializer(objectType);
            object res = null;

            using (FileStream fileStream = new FileStream(sXml, FileMode.Open))
            {
                try
                {
                    res = serializer.Deserialize(fileStream);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("XmlTools::LoadFromXml [ " + objectType.Name + "]: " + ex.Message);
                }
            }

            return res;
        }

        public static string GetAsXml(object o)
        {
            if (o == null)
                return "";

            var serializer = new XmlSerializer(o.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                try
                {
                    serializer.Serialize(writer, o);
                }
                catch (Exception)
                { }
            }

            return sb.ToString();
        }

        public static object Deserialize(string sFileName, Type objectType)
        {
            return Deserialize(sFileName, objectType, null, "");
        }

        public static T Deserialize<T>(string sFileName) where T : class
        {
            return Deserialize(sFileName, typeof(T)) as T;
        }

        public static object Deserialize(string sFileName, Type objectType, string sEncoding)
        {
            return Deserialize(sFileName, objectType, null, sEncoding);
        }

        public static T Deserialize<T>(string sFileName, string sEncoding) where T : class
        {
            return Deserialize(sFileName, typeof(T), null, sEncoding) as T;
        }

        public static object Deserialize(string sFileName, Type objectType, XmlRootAttribute root, string sEncoding)
        {
            //// Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(objectType);

            object res = null;

            // A FileStream is needed to write the XML document.
            // Como tenemos que cambiar la codificación para leer caracteres con acentos y tal,
            // hacemos un StreamReader del FileStream, con el ASCII extendido más normal (Latin-1)
            StreamReader fs;
            try
            {
                FileStream temp = new FileStream(sFileName, FileMode.Open, FileAccess.Read);
                if (sEncoding == null || sEncoding.Length == 0)
                    fs = new StreamReader(temp);
                else
                    fs = new StreamReader(temp, Encoding.GetEncoding(sEncoding));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }

            // Serialize the object and close the file
            try
            {
                res = serializer.Deserialize(fs);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            fs.Close();
            fs.Dispose();

            if (res == null)
            {
                Trace.TraceError("Error al desserializar");
                return null;
            }

            return res;
        }

        public static T Deserialize<T>(string sFileName, XmlRootAttribute root, string sEncoding) where T : class
        {
            return Deserialize(sFileName, typeof(T), root, sEncoding) as T;
        }


        public static bool Serialize(string sFileName, object obj)
        {
            return Serialize(sFileName, obj, null);
        }

        public static bool Serialize(string sFileName, object obj, XmlSerializer serializer)
        {
            return Serialize(sFileName, obj, serializer, null);
        }

        public static bool Serialize(string sFileName, object obj, XmlSerializer serializer, XmlRootAttribute root)
        {
            // A FileStream is needed to read the XML document.
            FileStream fs;
            try
            {
                fs = new FileStream(sFileName, FileMode.Create);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return false;
            }

            if (serializer == null)
            {
                try
                {
                    if (root != null)
                        serializer = new XmlSerializer(obj.GetType(), root);
                    else
                        serializer = new XmlSerializer(obj.GetType());
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }

                if (serializer == null)
                    return false;
            }

            bool res = false;
            try
            {
                serializer.Serialize(fs, obj);
                res = true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            try
            {
                fs.Close();
                fs.Dispose();
            }
            catch (Exception)
            { }

            return res;
        }
    }

}
