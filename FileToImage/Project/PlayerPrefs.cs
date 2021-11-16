using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileToImage.Project
{
    /// <summary>
    /// 本地化存储类
    /// </summary>
    class PlayerPrefs
    {
        /// <summary>
        /// 所有boolean数据
        /// </summary>
        static Dictionary<string, bool> AllBoolValue = new Dictionary<string, bool>();

        /// <summary>
        /// 所有int数据
        /// </summary>
        static Dictionary<string, int> AllIntValue = new Dictionary<string, int>();

        /// <summary>
        /// 所有字符串数据
        /// </summary>
        static Dictionary<string, string> AllStringValue = new Dictionary<string, string>();

        /// <summary>
        /// 内容保存位置
        /// </summary>
        static string SavePath = "PlayerPrefs.txt";

        /// <summary>
        /// 内容保存密码
        /// </summary>
        static string Password = Item.NewUUID(Item.HostName);

        /// <summary>
        /// 初始化
        /// </summary>
        public static bool Init()
        {
            if (File.Exists(SavePath)==false)
            {
                return false;
            }
            else
            {
                var file = new FileInfo(SavePath);
                var sr = new StreamReader(file.OpenRead());
                var key = Base64.GetKey(Password);
                var allValue = sr.ReadToEnd();
                if (allValue.Length==0)
                {
                    return false;
                }
                var save = Base64.Decode(allValue, key).FromStringDictEX();
                AllBoolValue = save["bool"].FromBoolDict();
                AllIntValue = save["int"].FromIntDict();
                AllStringValue = save["string"].FromStringDict();
                return true;
            }
        }

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public static void Save()
        {
            var save = new DictionaryEX<string, string>
            {
                { "bool",AllBoolValue.ToSave() },
                { "int",AllIntValue.ToSave() },
                { "string",AllStringValue.ToSave() }
            };
            //Console.WriteLine(AllBoolValue.ToString(true));
            //Console.WriteLine(AllIntValue.ToString(true));
            Console.WriteLine(AllStringValue.ToString(true));
            var file = File.Create(SavePath);
            var sw = new StreamWriter(file);
            var key = Base64.GetKey(Password);
            sw.Write(Base64.Encode(save.ToSave(), key));
            sw.Close();
            file.Close();
        }

        /// <summary>
        /// 设置boolean
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetBool(string key,bool value)
        {
            AllBoolValue[key] = value;
        }

        /// <summary>
        /// 设置int
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetInt(string key,int value)
        {
            AllIntValue[key] = value;
        }

        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetString(string key,string value)
        {
            AllStringValue[key] = value;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key,bool value)
        {
            SetBool(key, value);
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key,int value)
        {
            SetInt(key, value);
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key,string value)
        {
            SetString(key, value);
        }

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="key"></param>
        public static bool GetBool(string key,bool defaultValue)
        {
            try
            {
                return AllBoolValue[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(string key,int defaultValue)
        {
            try
            {
                return AllIntValue[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(string key,string defaultValue)
        {
            try
            {
                return AllStringValue[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return GetString(key, "");
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public static void Clear()
        {
            AllBoolValue = new Dictionary<string, bool>();
            AllIntValue = new Dictionary<string, int>();
            AllStringValue = new Dictionary<string, string>();
        }

        /// <summary>
        /// 删除一整类数据
        /// </summary>
        /// <param name="type"></param>
        public static void Clear(Type type)
        {
            if (type==typeof(bool))
            {
                AllBoolValue = new Dictionary<string, bool>();
            }
            else if (type==typeof(int))
            {
                AllIntValue = new Dictionary<string, int>();
            }
            else if (type==typeof(string))
            {
                AllStringValue = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 根据某一项删除
        /// </summary>
        /// <param name="key"></param>
        public static void Clear(string key)
        {
            AllBoolValue[key] = default(bool);
            AllIntValue[key] = default(int);
            AllStringValue[key] = default(string);
        }

        /// <summary>
        /// 指定类型删除一项
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public static void Clear(Type type,string key)
        {
            if (type == typeof(bool))
            {
                AllBoolValue[key] = default(bool);
            }
            else if (type == typeof(int))
            {
                AllIntValue[key] = default(int);
            }
            else if (type == typeof(string))
            {
                AllStringValue[key] = default(string);
            }

        }
    }

    /// <summary>
    /// 增强字典类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryEX<TKey,TValue> : Dictionary<TKey,TValue>
    {
        public DictionaryEX() { }

        public DictionaryEX(Dictionary<TKey,TValue> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public DictionaryEX(TKey[] keys,TValue[] values)
        {
            for (int i = 0; i < Math.Min(keys.Length,values.Length); i++)
            {
                this.Add(keys[i], values[i]);
            }
        }

        public DictionaryEX(List<TKey> keys,List<TValue> values) : this(keys.ToArray(), values.ToArray())
        {

        }

        //
        // 摘要:
        //     获取或设置与指定的键关联的值。
        //
        // 参数:
        //   key:
        //     要获取或设置的值的键。
        //
        // 返回结果:
        //     与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
        //     Set 操作创建一个带指定键的新元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     key 为 null。
        public new TValue this[TKey key]
        {
            set
            {

            }
            get
            {
                try
                {
                    return base[key];
                }
                catch (KeyNotFoundException)
                {
                    return default(TValue);
                }
            }
        }
    }
    
    /// <summary>
    /// 本地化存储追加类
    /// </summary>
    public static class PlayerPrefsAdd
    {
        #region 保存

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string,bool> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value ? 1 : 0);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string,int> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string,string> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(Base64.Encode(item.Value));
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string,bool> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value ? 1 : 0);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string,int> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string,string> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(Base64.Encode(item.Value));
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        #endregion

        #region 读取

        public static Dictionary<string,bool> FromBoolDict(this string str)
        {
            var ret = new Dictionary<string, bool>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
;           string key;
            bool val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = value[1] == "1" ? true : false;
                ret.Add(key, val);
            }

            return ret;
        }

        public static Dictionary<string, int> FromIntDict(this string str)
        {
            var ret = new Dictionary<string, int>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            int val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = int.Parse(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static Dictionary<string,string> FromStringDict(this string str)
        {
            var ret = new Dictionary<string, string>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            string val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = Base64.Decode(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string,bool> FromBoolDictEX(this string str)
        {
            var ret = new DictionaryEX<string, bool>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
;           string key;
            bool val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = value[1] == "1" ? true : false;
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string, int> FromIntDictEX(this string str)
        {
            var ret = new DictionaryEX<string, int>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            int val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = int.Parse(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string,string> FromStringDictEX(this string str)
        {
            var ret = new DictionaryEX<string, string>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            string val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length==1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = Base64.Decode(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        #endregion

        #region 重载ToString

        /// <summary>
        /// 重载ToString
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<TKey,TValue>(this Dictionary<TKey,TValue> dict,bool tf)
        {
            var ret = new StringBuilder();
            ret.Append("Dictionary<");
            ret.Append(typeof(TKey));
            ret.Append(", ");
            ret.Append(typeof(TValue));
            ret.Append(">(");
            ret.Append(dict.Count);
            ret.Append(") { ");
            foreach (var item in dict)
            {
                ret.Append("{ ");
                ret.Append("\"");
                ret.Append(item.Key.ToString());
                ret.Append("\", \"");
                ret.Append(item.Value.ToString());
                ret.Append("\" }, ");
            }
            ret.Append("}");
            return ret.ToString();
        }

        #endregion
    }
}
