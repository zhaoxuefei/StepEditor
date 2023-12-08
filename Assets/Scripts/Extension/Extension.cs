using UnityEngine;
using System.Collections;
using System;

namespace ZXFFrame
{
    public static class Extension
    {
        /// <summary>
        /// get a compent form the gameobject , if the compent is null , add a compent to the gameobject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmpGo"></param>
        /// <returns></returns>
        public static T GetOrCreatComponent<T>(this GameObject tmpGo) where T : Component
        {
            T t = tmpGo.GetComponent<T>();
            return t ? t : tmpGo.AddComponent<T>();
        }

        /// <summary>
        /// delete all children transforms of the gameobject
        /// </summary>
        /// <param name="tmpGo"></param>
        public static void DeleteAllChildren(this GameObject tmpGo)
        {
            for(int i = tmpGo.transform.childCount-1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(tmpGo.transform.GetChild(i).gameObject);
            }
        }

        public static Vector3 TransPosToVector3(this Vector3 tpmVector , Trans trans)
        {
            return new Vector3(trans.pos.x, trans.pos.y, trans.pos.z);
        }


        public static Vector3 TransRotToVector3(this Vector3 tpmVector, Trans trans)
        {
            return new Vector3(trans.rot.x, trans.rot.y, trans.rot.z);
        }



        /// <summary>
        /// 转整型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetInt(this System.Object value)
        {
            try
            {
                if (value == null)
                {
                    return 0;
                }
                switch (value.GetType().ToString())
                {
                    case "System.String":
                        return Int32.Parse((string)value);
                    case "System.Int64":
                        return Int32.Parse(((long)value).ToString());
                    case "System.Int32":
                        return (int)value;
                    case "System.Double":
                        return (int)System.Math.Round((double)value, 0);
                    case "System.Boolean":
                        return 0;
                    case "System.DateTime":
                        return 0;
                    //  case "System.IO.MemoryStream":
                    //      return getInt(MyConvert.getString(value));
                    //  case "System.Byte[]":
                    //      return getInt(MyConvert.getString(value));
                    default:
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 转双精度浮点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDouble(this System.Object value)
        {
            try
            {
                if (value == null)
                {
                    return 0.0;
                }
                switch (value.GetType().ToString())
                {
                    case "System.String":
                        return Double.Parse((string)value);
                    case "System.Int64":
                        return Double.Parse(((long)value).ToString());
                    case "System.Int32":
                        return Double.Parse(((int)value).ToString());
                    case "System.Boolean":
                        return 0.0;
                    case "System.Double":
                        return (int)System.Math.Round((double)value, 0);
                    case "System.DateTime":
                        return 0.0;
                    // case "System.IO.MemoryStream":
                    //     return MyConvert.getInt(MyConvert.getString(value));
                    // case "System.Byte[]":
                    //     return MyConvert.getInt(MyConvert.getString(value));
                    default:
                        return 0;
                }
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            DateTime dd = new DateTime(1970,1,1,0,0,0,0);
            TimeSpan ts = DateTime.Now - dd;
            return (Int64)ts.TotalMilliseconds;
        }
    }

}



