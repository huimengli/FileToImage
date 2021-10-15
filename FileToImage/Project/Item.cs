using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FileToImage.Project
{
    /// <summary>
    /// 工具类
    /// </summary>
    static class Item
    {
        /// <summary>
        /// 打开网站|其他东西
        /// </summary>
        /// <param name="web">网址|地址</param>
        public static void OpenOnWindows(string web)
        {
            System.Diagnostics.Process.Start(web);
        }

        /// <summary>
        /// base64加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64Encode(string input)
        {
            var en = new Base64Encoder();
            return en.GetEncoded(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64Decode(string input)
        {
            var de = new Base64Decoder();
            return Encoding.UTF8.GetString(de.GetDecoded(input));
        }

        /// <summary>
        /// SHA256签名
        /// (不适用于签名中文内容,中文加密和js上的加密不同)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(bytes);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(byte[] data)
        {
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(data);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        ///// <summary>
        ///// 读取文件
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static string ReadFile(string path)
        //{
        //    var ret = "";

        //    var file = new FileInfo(path);
        //    var sr = new StreamReader(file.OpenRead());
        //    ret = sr.ReadToEnd();

        //    return ret;
        //}

        /// <summary>
        /// 读取文件后直接进行base64编码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Base64EncodeFile(string fileName)
        {
            StringBuilder str = new StringBuilder();
            var en = new Base64Encoder();
            using (FileStream fs = File.OpenRead(fileName))
            {
                long left = fs.Length;
                int maxLength = 1000;//每次读取的最大长度  
                int start = 0;//起始位置  
                int num = 0;//已读取长度  
                while (left > 0)
                {
                    byte[] buffer = new byte[maxLength];//缓存读取结果  
                    char[] cbuffer = new char[maxLength];
                    fs.Position = start;//读取开始的位置  
                    num = 0;
                    if (left < maxLength)
                    {
                        num = fs.Read(buffer, 0, Convert.ToInt32(left));
                    }
                    else
                    {
                        num = fs.Read(buffer, 0, maxLength);
                    }
                    if (num == 0)
                    {
                        break;
                    }
                    start += num;
                    left -= num;
                    str = str.Append(en.GetEncoded(buffer));
                }
            }
            return str.ToString();
        }

        ///// <summary>
        ///// 读取文件
        ///// </summary>
        ///// <returns></returns>
        //public static string ReadFile()
        //{
        //    var dialog = new OpenFileDialog();
        //    if (dialog.ShowDialog()==DialogResult.OK)
        //    {
        //        if (!string.IsNullOrEmpty(dialog.FileName))
        //        {
        //            return Base64EncodeFile(dialog.FileName);
        //        }
        //    }
        //    return "";
        //}

        /// <summary>
        /// 让用户选择文件
        /// </summary>
        /// <returns></returns>
        public static string GetFile()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    return dialog.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// 让用户选择文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFile(string type)
        {
            var dialog = new OpenFileDialog();
            switch (type)
            {
                case "photo":
                case "image":
                case "Image":
                case "Photo":
                case "picture":
                case "Picture":
                    dialog.Filter = "@Image|*.jpg;*.png;*.bmp|@.Jpg|*.jpg|@.Png|*.png|@.Bmp|*.bmp|@.All files|*.*";
                    break;
                default:
                    break;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    return dialog.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// 将base64编码的内容转变为图片数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="base64Value"></param>
        public static void Base64ToBitmapData(ref BitmapData data, string base64Value)
        {
            var count = (int)Math.Ceiling(base64Value.Length / 3d);
            var temp = new byte[data.Stride * data.Height];
            var ret = new byte[base64Value.Length];
            var j = 0;
            for (int i = 0; i < base64Value.Length; i++)
            {
                ret[i] = (byte)base64Value[i];
            }
            for (int i = 0; i < count * 4; i++)
            {
                if (i % 4 == 3)
                {
                    temp[i] = 255;
                }
                else
                {
                    temp[i] = ret.GetItem(j++);
                }
            }
            ret = temp;
            //count = (int)Math.Ceiling(Math.Sqrt(ret.Length / 4));
            //指针
            var ptr = data.Scan0;
            count = data.Stride * data.Height;
            //用于指向数据存在的对象
            //var value = new byte[count];
            //原本这里是用于将图片数据取出修改
            //但是我只需要将变量和内存对应起来
            //Marshal.Copy(ptr, value, 0, count);
            //将已经处理好的数据直接通过内存操作放置到需要的位置
            Marshal.Copy(ret, 0, ptr, count);
            //尝试释放内存
            temp = null;
            ret = null;
            GC.Collect();
        }

        public static void Base64ToBitmapData(ref BitmapData data,byte[] base64Value)
        {
            var count = (int)Math.Ceiling(base64Value.Length / 3d);
            var temp = new byte[data.Stride * data.Height];
            var j = 0;
            for (int i = 0; i < count * 4; i++)
            {
                if (i % 4 == 3)
                {
                    temp[i] = 255;
                }
                else
                {
                    temp[i] = base64Value.GetItem(j++);
                }
            }
            //count = (int)Math.Ceiling(Math.Sqrt(ret.Length / 4));
            //指针
            var ptr = data.Scan0;
            count = data.Stride * data.Height;
            //用于指向数据存在的对象
            //var value = new byte[count];
            //原本这里是用于将图片数据取出修改
            //但是我只需要将变量和内存对应起来
            //Marshal.Copy(ptr, value, 0, count);
            //将已经处理好的数据直接通过内存操作放置到需要的位置
            Marshal.Copy(temp, 0, ptr, count);
            //尝试释放内存
            temp = null;
            GC.Collect();
        }

        public static ImageCodecInfo GetImageCoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// 将图片转为jpg并保存
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        public static void BmpToJpgSave(Bitmap image, string path)
        {
            if (File.Exists(path))
            {
                MessageBox.Show("文件: " + path + " 已经存在!", "错误", MessageBoxButtons.OK);
            }
            else
            {
                using (var esp = new EncoderParameters(1))
                {
                    using (var ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L))
                    {
                        esp.Param[0] = ep;
                        var jpsEncoder = GetImageCoder(ImageFormat.Jpeg);
                        //保存图片为jpg
                        //image.Save(path, jpsEncoder, esp);
                        image.Save(path);
                        //释放资源
                        image.Dispose();
                        //image.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ReadFile(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                var i = 0;
                var temp = "";
                var num = 0;//已经读取长度
                var left = stream.Length;
                var maxLength = 1002;//每次读取的最大长度
                var allValue = new StringBuilder();
                //var check = new StringBuilder();

                while (left > 0)
                {
                    var buffer = new byte[maxLength];//缓存读取结果
                    var cbuffer = new char[maxLength];
                    stream.Position = i;
                    num = 0;
                    if (left < maxLength)
                    {
                        num = stream.Read(buffer, 0, Convert.ToInt32(left));
                    }
                    else
                    {
                        num = stream.Read(buffer, 0, maxLength);
                    }
                    if (num == 0)
                    {
                        break;
                    }
                    i += num;
                    if (left < maxLength)
                    {
                        temp = ByteToString(buffer.Take((int)left).ToArray());
                    }
                    else
                    {
                        temp = ByteToString(buffer);
                    }
                    var test = StringToByte(temp);
                    left -= num;
                    allValue.Append(temp);
                }
                return allValue.ToString();
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] ReadFile(string file)
        {
            var theFile = new FileInfo(file);
            using (var stream = theFile.OpenRead())
            {
                var ret = new byte[theFile.Length];
                stream.Read(ret, 0, (int)theFile.Length);
                return ret;
            }
        }

        /// <summary>
        /// 将字节转为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bytes)
        {
            return ByteToString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 将字节转为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bytes, int start, int count)
        {
            var ret = new StringBuilder();
            for (int i = start; i < count; i++)
            {
                ret.Append((char)bytes[i]);
            }
            //Console.WriteLine(bytes.Length);
            return ret.ToString();
        }

        /// <summary>
        /// 将字符串转为字节流
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string str)
        {
            return StringToByte(str, 0, str.Length);
        }

        /// <summary>
        /// 将字符串转为字节流
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string str, int start, int count)
        {
            var ret = new byte[str.Length];
            var temp = 0;
            for (int i = start; i < count; i++)
            {
                temp = (int)str[i];
                if (temp > 255)
                {
                    throw new InvalidCastException("错误内容!\n转码的内容值超过255!");
                }
                else
                {
                    ret[i] = (byte)temp;
                }
            }
            //Console.WriteLine(ret.Length);
            return ret;
        }

        /// <summary>
        /// 创建一个加载界面
        /// </summary>
        /// <param name="title"></param>
        /// <param name="value"></param>
        public static void Loading(string title,string value)
        {
            
        }

        /// <summary>
        /// 创建一个加载界面
        /// </summary>
        /// <param name="value"></param>
        public static void Loading(string value)
        {
            
        }

        /// <summary>
        /// 关闭加载界面
        /// </summary>
        public static void CloseLoading()
        {

        }

        /// <summary>
        /// 使用CLZF模块压缩字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] bytes)
        {
            return CLZF.Compress(bytes);
        }

        /// <summary>
        /// 使用CLZF模块解压字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] bytes)
        {
            return CLZF.Decompress(bytes);
        }
    }

    /// <summary>
    /// Base64编码类。
    /// 将byte[]类型转换成Base64编码的string类型。
    /// </summary>
    public class Base64Encoder
    {
        byte[] source;
        int length, length2;
        int blockCount;
        int paddingCount;
        public static Base64Encoder Encoder = new Base64Encoder();

        public Base64Encoder()
        {
        }

        private void init(byte[] input)
        {
            source = input;
            length = input.Length;
            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);
                blockCount = (length + paddingCount) / 3;
            }
            length2 = length + paddingCount;
        }

        public string GetEncoded(byte[] input)
        {
            //初始化
            init(input);

            byte[] source2;
            source2 = new byte[length2];

            for (int x = 0; x < length2; x++)
            {
                if (x < length)
                {
                    source2[x] = source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }

            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];
            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];

                temp1 = (byte)((b1 & 252) >> 2);

                temp = (byte)((b1 & 3) << 4);
                temp2 = (byte)((b2 & 240) >> 4);
                temp2 += temp;

                temp = (byte)((b2 & 15) << 2);
                temp3 = (byte)((b3 & 192) >> 6);
                temp3 += temp;

                temp4 = (byte)(b3 & 63);

                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;

            }

            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = sixbit2char(buffer[x]);
            }


            switch (paddingCount)
            {
                case 0: break;
                case 1: result[blockCount * 4 - 1] = '='; break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default: break;
            }
            return new string(result);
        }
        private char sixbit2char(byte b)
        {
            char[] lookupTable = new char[64]{
                  'A','B','C','D','E','F','G','H','I','J','K','L','M',
                 'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                 'a','b','c','d','e','f','g','h','i','j','k','l','m',
                 'n','o','p','q','r','s','t','u','v','w','x','y','z',
                 '0','1','2','3','4','5','6','7','8','9','+','/'};

            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[(int)b];
            }
            else
            {

                return ' ';
            }
        }

    }

    /// <summary>
    /// Base64解码类
    /// 将Base64编码的string类型转换成byte[]类型
    /// </summary>
    public class Base64Decoder
    {
        char[] source;
        int length, length2, length3;
        int blockCount;
        int paddingCount;
        public static Base64Decoder Decoder = new Base64Decoder();

        public Base64Decoder()
        {
        }

        private void init(char[] input)
        {
            int temp = 0;
            source = input;
            length = input.Length;

            for (int x = 0; x < 2; x++)
            {
                if (input[length - x - 1] == '=')
                    temp++;
            }
            paddingCount = temp;

            blockCount = length / 4;
            length2 = blockCount * 3;
        }

        public byte[] GetDecoded(string strInput)
        {
            //初始化
            init(strInput.ToCharArray());

            byte[] buffer = new byte[length];
            byte[] buffer2 = new byte[length2];

            for (int x = 0; x < length; x++)
            {
                buffer[x] = char2sixbit(source[x]);
            }

            byte b, b1, b2, b3;
            byte temp1, temp2, temp3, temp4;

            for (int x = 0; x < blockCount; x++)
            {
                temp1 = buffer[x * 4];
                temp2 = buffer[x * 4 + 1];
                temp3 = buffer[x * 4 + 2];
                temp4 = buffer[x * 4 + 3];

                b = (byte)(temp1 << 2);
                b1 = (byte)((temp2 & 48) >> 4);
                b1 += b;

                b = (byte)((temp2 & 15) << 4);
                b2 = (byte)((temp3 & 60) >> 2);
                b2 += b;

                b = (byte)((temp3 & 3) << 6);
                b3 = temp4;
                b3 += b;

                buffer2[x * 3] = b1;
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }

            length3 = length2 - paddingCount;
            byte[] result = new byte[length3];

            for (int x = 0; x < length3; x++)
            {
                result[x] = buffer2[x];
            }

            return result;
        }

        private byte char2sixbit(char c)
        {
            char[] lookupTable = new char[64]{
                 'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
                 'O','P','Q','R','S','T','U','V','W','X','Y', 'Z',
                 'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                 'o','p','q','r','s','t','u','v','w','x','y','z',
                 '0','1','2','3','4','5','6','7','8','9','+','/'};
            if (c == '=')
                return 0;
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    if (lookupTable[x] == c)
                        return (byte)x;
                }

                return 0;
            }

        }
    }

    /// <summary>
    /// 工具类追加函数
    /// </summary>
    public static class ItemAdd
    {
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetItem<T>(this T[] ts, int index)
        {
            index = index < 0 ? ts.Length - index : index;
            if (index >= ts.Length || index < 0)
            {
                return default(T);
            }
            else
            {
                return ts[index];
            }
        }

        /// <summary>
        /// 将字符串转为字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDict(this string str)
        {
            var temp = str.Split(';');
            var ret = new Dictionary<string, string>();
            foreach (var item in temp)
            {
                var x = item.Split(':');
                ret.Add(x[0], x[1]);
            }
            return ret;
        }

        /// <summary>
        /// 将字典转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToString(this Dictionary<string, string> dict, bool tf)
        {
            if (tf)
            {
                var ret = new StringBuilder();
                foreach (var item in dict)
                {
                    ret.Append(item.Key);
                    ret.Append(":");
                    ret.Append(item.Value);
                    ret.Append(";");
                }
                return ret.ToString(0, ret.Length - 1);
            }
            else
            {
                return dict.ToString();
            }
        }
    }

}
