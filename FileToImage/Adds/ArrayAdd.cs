using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToImage.Adds
{
    /// <summary>
    /// 列表方法追加
    /// </summary>
    public static class ArrayAdd
    {
        /// <summary>
        /// 遍历执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this Array array,Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                object t = array.GetValue(i);
                action((T)t);
            }
        } 
    }
}
