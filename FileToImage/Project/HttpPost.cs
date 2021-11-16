using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FileToImage.Project
{

    public class HttpPost
    {
        #region c#原生方式
        /// <summary>       
        /// 指定Post地址使用Get 方式获取全部字符串       
        /// </summary>       
        /// <param name="url">请求后台地址</param>       
        /// <returns></returns>       
        public static string Post(string url)
        {
            string result = "";
            CookieContainer cookie = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "*/*";
            req.UserAgent = "PC";
            req.CookieContainer = cookie.Load();
            req.Method = "POST";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            cookie = req.CookieContainer;
            cookie.Save();
            Console.WriteLine(cookie.GetAllCookies().ToString(true));
            Stream stream = resp.GetResponseStream();            //获取内容   
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>        
        /// 指定Post地址使用Get 方式获取全部字符串      
        /// </summary>      
        /// <param name="url">请求后台地址</param>      
        /// <returns></returns>    
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数 
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0) builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length; using (Stream reqStream = req.GetRequestStream()) { reqStream.Write(data, 0, data.Length); reqStream.Close(); }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();            //获取响应内容        
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>
        public static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数   
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length); reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();    //获取响应内容   
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        #endregion
    }

    public static class HttpPostAdd
    {
        #region 正则表达式
        /// <summary>
        /// 读取单个内容
        /// </summary>
        private static Regex readValue = new Regex("\"?([^\",{}:]+)\"?:\"?([^\",{}:]+)\"?");

        /// <summary>
        /// 读取单项内容
        /// </summary>
        private static Regex readItem = new Regex("([^\",{}:]+)");


        #endregion

        /// <summary>
        /// 获取所有cookies
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static List<Cookie> GetAllCookies(this CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }

        /// <summary>
        /// 将多个cookie转为post请求所需对象
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static CookieContainer GetCookieContainer(this List<Cookie> cookies)
        {
            var ret = new CookieContainer();
            foreach (var cookie in cookies)
            {
                ret.Add(cookie);
            }
            return ret;
        }


        /// <summary>
        /// 将cookie转为字典
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDict(this Cookie cookie)
        {
            var ret = new Dictionary<string, string>
        {
            { "name",cookie.Name },
            { "value",cookie.Value },
            { "path",cookie.Path },
            { "domain",cookie.Domain }
        };
            return ret;
        }

        /// <summary>
        /// 将cookie保存起来
        /// </summary>
        /// <param name="cookie"></param>
        public static void Save(this CookieContainer cookieContainer)
        {
            var list = cookieContainer.GetAllCookies();
            var save = new List<(string, string)>();
            foreach (var cookie in list)
            {
                save.Add((cookie.Name, Item.Base64Encode(cookie.ToDict().ToList().ToString(true))));
            }
            PlayerPrefs.SetString("cookie", Item.Base64Encode(save.ToString(true)));
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString(this List<(string, object)> lists, bool tf)
        {
            if (tf)
            {
                var ret = "{";
                for (int i = 0; i < lists.Count; i++)
                {
                    var item = lists[i];
                    ret += item.Item1 + ":" + item.Item2.ToString();
                    if (i < lists.Count - 1)
                    {
                        ret += ",";
                    }
                }
                ret += "}";
                return ret;
            }
            else
            {
                return lists.ToString();
            }
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString(this List<(string, string)> lists, bool tf)
        {
            if (tf)
            {
                var ret = "{";
                for (int i = 0; i < lists.Count; i++)
                {
                    var item = lists[i];
                    ret += item.Item1 + ":" + item.Item2;
                    if (i < lists.Count - 1)
                    {
                        ret += ",";
                    }
                }
                ret += "}";
                return ret;
            }
            else
            {
                return lists.ToString();
            }
        }

        /// <summary>
        /// 转化为列表
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<(string, string)> ToList(this Dictionary<string, string> dict)
        {
            var ret = new List<(string, string)>();
            foreach (var item in dict)
            {
                ret.Add((item.Key, item.Value));
            }
            return ret;
        }

        /// <summary>
        /// 读取所有保存的cookie
        /// </summary>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        public static CookieContainer Load(this CookieContainer cookieContainer)
        {
            var load = PlayerPrefs.GetString("cookie", "");
            if (string.IsNullOrEmpty(load))
            {
                return cookieContainer;
            }
            var all = Item.Base64Decode(load).GetLists();
            var cookies = new List<Cookie>();
            foreach (var item in all)
            {
                cookies.Add(Item.Base64Decode(item.Item2).GetLists().ToDict().ToCookie());
            }
            cookieContainer = cookies.GetCookieContainer();
            return cookieContainer;
        }

        /// <summary>
        /// 从内容中获取列表
        /// </summary>
        /// <param name="liststr"></param>
        /// <returns></returns>
        public static List<(string, string)> GetLists(this string liststr)
        {
            //var lists = liststr.Split(',');
            //var list = new List<Match>();
            //foreach (var item in lists)
            //{
            //    list.Add(readValue.Match(item));
            //}
            //throw new Exception(lists.ToString(true));
            var lists = readValue.Matches(liststr);
            var ret = new List<(string, string)>();
            for (int i = 0; i < lists.Count; i++)
            {
                var item = readItem.Matches(lists[i].Groups[0].Value);
                ret.Add((item[0].Value, item[1].Value));
            }
            return ret;
        }

        /// <summary>
        /// 从内容中获取字典
        /// </summary>
        /// <param name="dictstr"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDicts(this string dictstr)
        {
            var ret = new Dictionary<string, string>();

            dictstr = dictstr.Replace("{", "").Replace("}", "").Replace('"', ' ').Replace(" ", "");
            var values = dictstr.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i].Split(':');
                ret.Add(value[0], value[1]);
            }

            return ret;
        }

        /// <summary>
        /// 转为字典
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDict(this List<(string, string)> lists)
        {
            var ret = new Dictionary<string, string>();
            foreach (var item in lists)
            {
                ret.Add(item.Item1, item.Item2);
            }
            return ret;
        }

        /// <summary>
        /// 将cookie字典转回cookie
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static Cookie ToCookie(this Dictionary<string, string> dict)
        {
            return new Cookie(dict["name"], dict["value"], dict["path"], dict["domain"]);
        }

        /// <summary>
        /// 重载为转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<T>(this List<T> lists, bool tf) where T : class
        {
            if (tf)
            {
                var ret = "[";
                for (int i = 0; i < lists.Count; i++)
                {
                    ret += lists[i].ToString();
                    if (i < lists.Count - 1)
                    {
                        ret += ",";
                    }
                }
                ret += "]";
                return ret;
            }
            else
            {
                return lists.ToString();
            }
        }

        /// <summary>
        /// 转为网址(get传参)
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static string ToUrl(this List<(string, string)> lists, string URL)
        {
            var ret = URL;
            ret += "?";
            for (int i = 0; i < lists.Count; i++)
            {
                var item = lists[i];
                ret += item.Item1 + "=" + item.Item2;
                if (i < lists.Count - 1)
                {
                    ret += "&";
                }
            }
            return ret;
        }

        /// <summary>
        /// 转为json字符串
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static string ToJson(this List<(string, string)> lists)
        {
            var ret = "{";
            for (int i = 0; i < lists.Count; i++)
            {
                var item = lists[i];
                ret += "\"" + item.Item1 + "\":\"" + item.Item2 + "\"";
                if (i < lists.Count - 1)
                {
                    ret += ",";
                }
            }
            ret += "}";
            return ret;
        }


    }
}