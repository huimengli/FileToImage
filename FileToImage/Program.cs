using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FileToImage
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //防止多个工具同时运行
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("程序已经在运行！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#if DEBUG
            //MessageBox.Show("DEBUG");
            //程序启动
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());

#else
            //MessageBox.Show("else");
            //捕获全局异常
            try
            {
                //处理未捕获的异常模式
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                //程序启动
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception err)
            {
                string str = "";
                string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";

                if (err != null)
                {
                    str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                    err.GetType().Name, err.Message, err.StackTrace);
                }
                else
                {
                    str = string.Format("应用程序线程错误:{0}", err);
                }

                MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("发生致命错误，请及时联系作者！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

#endif
        }

        /// <summary>
        ///这就是我们要在发生未处理异常时处理的方法，我这是写出错详细信息到文本，如出错后弹出一个漂亮的出错提示窗体，给大家做个参考
        ///做法很多，可以是把出错详细信息记录到文本、数据库，发送出错邮件到作者信箱或出错后重新初始化等等
        ///这就是仁者见仁智者见智，大家自己做了。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

            string str = "";
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }

            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //MessageBox.Show("发生致命错误，请及时联系作者！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }

            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //MessageBox.Show("发生致命错误，请停止当前操作并及时联系作者！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

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
        /// 读取文件，返回相应字符串  
        /// </summary>  
        /// <param name="fileName">文件路径</param>  
        /// <returns>返回文件内容</returns>  
        public static string ReadFile(string fileName)
        {
            StringBuilder str = new StringBuilder();
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
                    str = str.Append(Encoding.UTF8.GetString(buffer));
                }
            }
            return str.ToString();
        }

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
            if (dialog.ShowDialog()==DialogResult.OK)
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
        public static void Base64ToBitmapData(ref BitmapData data,string base64Value)
        {
            var count = (int)Math.Ceiling(base64Value.Length / 3d);
            var temp = new byte[data.Stride*data.Height];
            var ret = new byte[base64Value.Length];
            var j = 0;
            for (int i = 0; i < base64Value.Length; i++)
            {
                ret[i] = (byte)base64Value[i];
            }
            for (int i = 0; i < count*4; i++)
            {
                if (i%4==3)
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
        public static void BmpToJpgSave(Bitmap image,string path)
        {
            if (File.Exists(path))
            {
                MessageBox.Show("文件: " + path + " 已经存在!", "错误!", MessageBoxButtons.OK);
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
                        //image.Dispose();
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
        public static string ByteToString(byte[] bytes,int start,int count)
        {
            var ret = new StringBuilder();
            for (int i = start; i < count; i++)
            {
                ret.Append((char)bytes[i]);
            }
            Console.WriteLine(bytes.Length);
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
        public static byte[] StringToByte(string str,int start,int count)
        {
            var ret = new byte[str.Length];
            var temp = 0;
            for (int i = start; i < count; i++)
            {
                temp = (int)str[i];
                if (temp > 255)
                {
                    throw new Exception("错误内容!\n转码的内容值超过255!");
                }
                else
                {
                    ret[i] = (byte)temp;
                }
            }
            Console.WriteLine(ret.Length);
            return ret;
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
    /// 编码加密方式
    /// </summary>
    public enum CodingMode
    {
        /// <summary>
        /// 没有加密
        /// </summary>
        NoCoding,

        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256,

        /// <summary>
        /// 双层MD5加密
        /// </summary>
        MD5,
    }

    /// <summary>
    /// base64类
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// base64原始密匙
        /// </summary>
        public static string _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        /// <summary>
        /// 将输入内容以utf-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string _utf8_encode(string input)
        {
            input = input.Replace("\r\n", "\n");
            var ret = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var c = (int)input[i];
                if (c<128)
                {
                    ret.Append((char)c);
                }
                else if (c>127 && c<2048)
                {
                    ret.Append((char)((c >> 6) | 192));
                    ret.Append((char)((c & 63) | 128));
                }
                else
                {
                    ret.Append((char)((c >> 12) | 224));
                    ret.Append((char)(((c >> 6) & 63) | 128));
                    ret.Append((char)((c & 63) | 128));
                }
            }
            return ret.ToString();
        }

        /// <summary>
        /// 将utf8编码的内容解码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string _utf8_decode(string input) {
            var ret = new StringBuilder();
            var i = 0;
            int c = 0, c2 = 0, c3 = 0;
            while (i<input.Length)
            {
                c = (int)input[i++];
                if (c < 128)
                {
                    ret.Append((char)c);
                }
                else if (c>191 && c<224)
                {
                    c2 = (int)input[i++];
                    ret.Append((char)(((c & 31) << 6) | (c2 & 63)));
                }
                else
                {
                    c2 = (int)input[i++];
                    c3 = (int)input[i++];
                    ret.Append((char)(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63)));
                }
            }
            return ret.ToString();
        }

        /// <summary>
        /// base64加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encode(string input)
        {
            var ret = new StringBuilder();
            int? chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            input = _utf8_encode(input);
            while (i<input.Length)
            {
                chr1 = (int)input[i++];
                chr2 = input.GetChar(i++);
                chr3 = input.GetChar(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2.OR(0) >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3.OR(0) >> 6);
                enc4 = chr3 & 63;
                if (chr2==null)
                {
                    enc3 = enc4 = 64;
                }
                else if (chr3==null)
                {
                    enc4 = 64;
                }
                ret.Append(_keyStr[enc1.Value]);
                ret.Append(_keyStr[enc2.Value]);
                ret.Append(_keyStr[enc3.Value]);
                ret.Append(_keyStr[enc4.Value]);
            }
            return ret.ToString();
        }

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(string input)
        {
            var ret = new StringBuilder();
            int? chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            while (i<input.Length)
            {
                enc1 = _keyStr.IndexOf(input[i++]);
                enc2 = _keyStr.IndexOf(input[i++]);
                enc3 = _keyStr.IndexOf(input[i++]);
                enc4 = _keyStr.IndexOf(input[i++]);
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                ret.Append((char)chr1.Value);
                if (enc3!=64)
                {
                    ret.Append((char)chr2.Value);
                }
                if (enc4!=64)
                {
                    ret.Append((char)chr3.Value);
                }
            }
            return _utf8_decode(ret.ToString());
        }

        /// <summary>
        /// 获取随机密匙
        /// </summary>
        /// <returns></returns>
        public static string RandomKey()
        {
            var key = new List<char>(_keyStr.ToArray<char>());
            var ret = new StringBuilder();
            var count = key.Count;
            for (int i = 0; i < count; i++)
            {
                ret.Append(key.GetRandomOne(true));
            }
            return ret.ToString();
        }

        /// <summary>
        /// base64编码
        /// 使用特殊密匙
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encode(string input,string key)
        {
            var ret = new StringBuilder();
            int? chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            input = _utf8_encode(input);
            while (i < input.Length)
            {
                chr1 = (int)input[i++];
                chr2 = input.GetChar(i++);
                chr3 = input.GetChar(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2.OR(0) >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3.OR(0) >> 6);
                enc4 = chr3 & 63;
                if (chr2 == null)
                {
                    enc3 = enc4 = 64;
                }
                else if (chr3 == null)
                {
                    enc4 = 64;
                }
                ret.Append(key[enc1.Value]);
                ret.Append(key[enc2.Value]);
                ret.Append(key[enc3.Value]);
                ret.Append(key[enc4.Value]);
            }
            return ret.ToString();
        }

        /// <summary>
        /// base64解码
        /// 使用特殊密匙
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decode(string input,string key)
        {
            var ret = new StringBuilder();
            int? chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            while (i < input.Length)
            {
                enc1 = key.IndexOf(input[i++]);
                enc2 = key.IndexOf(input[i++]);
                enc3 = key.IndexOf(input[i++]);
                enc4 = key.IndexOf(input[i++]);
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                ret.Append((char)chr1.Value);
                if (enc3 != 64)
                {
                    ret.Append((char)chr2.Value);
                }
                if (enc4 != 64)
                {
                    ret.Append((char)chr3.Value);
                }
            }
            return _utf8_decode(ret.ToString());
        }

        /// <summary>
        /// 获取密匙
        /// </summary>
        /// <returns></returns>
        public static string GetKey()
        {
            return RandomKey();
        }

        /// <summary>
        /// 获取密匙
        /// (默认SHA256)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetKey(string input)
        {
            return GetKey(input, CodingMode.SHA256);
        }

        /// <summary>
        /// 获取密匙
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetKey(string input,CodingMode mode)
        {
            switch (mode)
            {
                case CodingMode.NoCoding:
                    return _keyStr;
                case CodingMode.SHA256:
                    input = Item.SHA256(input);
                    var temp = input;
                    var keys = "ABCDEF0123456789";
                    var count = 0;
                    var keyss = new List<int>();
                    var indexs = new Dictionary<string,int>();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        var x = keys[i].ToString();
                        count = temp.Length;
                        temp = temp.Replace(x, "");
                        keyss.Add(count - temp.Length);
                        indexs.Add(x, 0);
                    }
                    var key = new Dictionary<string, string>();
                    var key2 = _keyStr.Substring(0, 64);
                    for (int i = 0; i < keyss.Count; i++)
                    {
                        var x = key2.Substring(0, keyss[i]);
                        key2 = key2.Replace(x, "");
                        key.Add(keys[i].ToString(), x);
                    }
                    var ret = new StringBuilder();
                    for (int i = 0; i < input.Length; i++)
                    {
                        var x = input[i].ToString();
                        ret.Append(key[x][indexs[x]]);
                        indexs[x]++;
                    }
                    ret.Append('=');
                    return ret.ToString();
                case CodingMode.MD5:
                    throw new Exception("这里我准备使用双层MD5编码\n不过内容尚未完成,等以后再写");
                default:
                    throw new Exception("没有这种编码模式!");
            }
        }


    }

    /// <summary>
    /// base64类追加函数
    /// </summary>
    public static class Base64Add
    {
        /// <summary>
        /// 获取一个字符对象编码值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index">指针</param>
        /// <returns></returns>
        public static int? GetChar(this string str,int index)
        {
            index = index < 0 ? str.Length - index:index;
            if (index >= str.Length || index<0)
            {
                return null;
            }
            else
            {
                return (int)str[index];
            }
        }

        /// <summary>
        /// 或者
        /// </summary>
        /// <param name="num"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int OR(this int? num,int defaultValue)
        {
            if (num==null)
            {
                return defaultValue;
            }
            else
            {
                return num.Value;
            }
        }

        /// <summary>
        /// 随机取出一个对象
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        public static T GetRandomOne<T>(this List<T> list,bool isDelete)
        {
            var r = new Random();
            T ret = list[r.Next(list.Count)];
            if (isDelete)
            {
                list.Remove(ret);
            }
            return ret;
        }

        /// <summary>
        /// 随机取出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomOne<T>(this List<T> list)
        {
            return list.GetRandomOne<T>(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CodingMode Pause(this CodingMode mode,string value)
        {
            switch (value)
            {
                case "无":
                    return CodingMode.NoCoding;
                default:
                    return (CodingMode)Enum.Parse(typeof(CodingMode), value);
            }
        }

        public static string GetValue(this CodingMode mode)
        {
            switch (mode)
            {
                case CodingMode.NoCoding:
                    return "无";
                case CodingMode.SHA256:
                    return "SHA256";
                case CodingMode.MD5:
                    return "MD5";
                default:
                    throw new Exception("没有这个参数!");
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
        public static T GetItem<T>(this T[] ts,int index)
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
        public static Dictionary<string,string> ToDict(this string str)
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
        public static string ToString(this Dictionary<string,string> dict,bool tf)
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

