using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace ZXFFrame
{
    public static class SerializeHelper
    {
        /// <summary>
        /// 使用UTF8编码将byte数组转成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ConvertToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// 使用指定字符编码将byte数组转成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ConvertToString(byte[] data, Encoding encoding)
        {
            return encoding.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// 使用UTF8编码将字符串转成byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ConvertToByte(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 使用指定字符编码将字符串转成byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ConvertToByte(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// 将对象序列化为二进制数据 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, obj);

            byte[] data = stream.ToArray();
            stream.Close();

            return data;
        }

        /// <summary>
        /// byte文件流写文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tmpPath"></param>
        /// <param name="tmpFileName"></param>
        public static void BinaryToFile(byte[] data , string tmpPath, string tmpFileName)
        {
            FileStream fs = new FileStream(tmpPath+"/"+ tmpFileName, FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }


        /// <summary>
        /// 将对象序列化为XML数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToXml(object obj)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            xs.Serialize(stream, obj);

            byte[] data = stream.ToArray();
            stream.Close();

            return data;
        }

        /// <summary>
        /// 将二进制数据反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeWithBinary(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(stream);

            stream.Close();

            return obj;
        }

        /// <summary>
        /// 将二进制数据反序列化为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeWithBinary<T>(byte[] data)
        {
            return (T)DeserializeWithBinary(data);
        }

        /// <summary>
        /// 将XML数据反序列化为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeWithXml<T>(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            try
            {
                object obj = xs.Deserialize(stream);
                stream.Close();
                return (T)obj;
            }
           catch(Exception e)
            {
                e.ToString();
                stream.Close();
                return default(T);
            }
        }




        /// <summary>
        /// 将一个类序列化为二进制数组，并写成文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tmpPath"></param>
        public static void SerializeToFile(object obj, string tmpPath, string tmpFileName)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, obj);

            byte[] data = stream.ToArray();
            stream.Close();

            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }

            File.WriteAllBytes(tmpPath + "/" + tmpFileName, data);
        }

        /// <summary>
        /// 将一个文件反序列化为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmpFilePath"></param>
        /// <returns></returns>
        public static T DeserializeWithFile<T>(string tmpFilePath)
        {
            FileStream fs = new FileStream(tmpFilePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(fs);
            //byte[] data = new byte[fs.Length];
            //fs.Read(data, 0, data.Length);
            fs.Close();
            return (T)obj;
        }

        /// <summary>
        /// 将对象序列化为XML文件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void SerializeToXmlFile(object obj, string tmpPath)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            xs.Serialize(stream, obj);

            byte[] data = stream.ToArray();
            stream.Close();
            File.WriteAllBytes(tmpPath, data);
        }

        /// <summary>
        /// xml文件反序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmpFilePath"></param>
        /// <returns></returns>
        public static T DeserializeWithXmlFile<T>(string tmpFilePath)
        {
            FileStream fs = new FileStream(tmpFilePath, FileMode.Open);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            object obj = xs.Deserialize(fs);
            fs.Close();
            return (T)obj;
        }
        /*
        /// <summary>
        /// json字符串反序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmpJsonText"></param>
        /// <returns></returns>
        public static T DeserializeWithJson<T>(string tmpJsonText)
        {
            return LitJson.JsonMapper.ToObject<T>(tmpJsonText);
        }

        /// <summary>
        /// 把一个文件按序列化成json文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tmpPath"></param>
        /// <param name="tmpFileName"></param>
        public static void SerializeToJsonFile(object obj, string tmpPath, string tmpFileName)
        {
            string text = LitJson.JsonMapper.ToJson(obj);
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
            File.AppendAllText(tmpPath+"/"+tmpFileName, text);//添加至文件
        }


        /// <summary>
        /// json文件反序列化成类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFilePath"></param>
        /// <returns></returns>
        public static T DeserializeWithJsonFile<T>(string jsonFilePath)
        {
            LitJson.JsonReader jr = new LitJson.JsonReader(File.ReadAllText(jsonFilePath));
            return LitJson.JsonMapper.ToObject<T>(jr);
        }

        /// <summary>
        /// 将对象序列化为json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToJsonText(object obj)
        {
            return LitJson.JsonMapper.ToJson(obj);
        }
        */

    }
}
