using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileToImage.Project
{
    /// <summary>
    /// 工具类
    /// </summary>
    static class Item
    {
        /// <summary>
        /// 加法器
        /// </summary>
        /// <returns></returns>
        delegate int SUM();

        /// <summary>
        /// ICON大小
        /// </summary>
        private static int ICONLENGTH = 16958;

        /// <summary>
        /// 内容开始标记(未确定,是否要使用这个)
        /// </summary>
        private static string STARTMARK = "###StartMark###";

        /// <summary>
        /// 临时存储文件
        /// </summary>
        private static string TEMPFILE = "tempSave.file";

        /// <summary>
        /// 打开网站|其他东西
        /// </summary>
        /// <param name="web">网址|地址</param>
        public static void OpenOnWindows(string web)
        {
            System.Diagnostics.Process.Start(web);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFile(string path)
        {
            var command = string.Format("explorer /select,{0}", path);
            UseCmd(command);
        }

        /// <summary>
        /// 使用cmd命令
        /// </summary>
        /// <param name="cmdCode"></param>
        public static void UseCmd(string cmdCode)
        {
            System.Diagnostics.Process proIP = new System.Diagnostics.Process();
            proIP.StartInfo.FileName = "cmd.exe";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;
            proIP.Start();
            proIP.StandardInput.WriteLine(cmdCode);
            proIP.StandardInput.WriteLine("exit");
            string strResult = proIP.StandardOutput.ReadToEnd();
            Console.WriteLine(strResult);
            proIP.Close();
        }

        /// <summary>
        /// 使用cmd命令
        /// </summary>
        /// <param name="cmdCode"></param>
        public static void UseCmd(params string[] cmdCodes)
        {
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;
            proIP.Start();
            foreach (var eachCmd in cmdCodes)
            {
                proIP.StandardInput.WriteLine(eachCmd);
            }
            proIP.StandardInput.WriteLine("exit");
            string strResult = proIP.StandardOutput.ReadToEnd();
            Console.WriteLine(strResult);
            proIP.Close();
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

        public static void Base64ToBitmapData(ref BitmapData data, byte[] base64Value)
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
        /// 读取文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static byte[] ReadFile(string file, int start, int end)
        {
            var theFile = new FileInfo(file);
            using (var stream = theFile.OpenRead())
            {
                var ret = new byte[end - start];
                stream.Read(ret, start, end);
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
        public static void Loading(string title, string value)
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

        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] bytes, CompressMode mode)
        {
            switch (mode)
            {
                case CompressMode.NoCompress:
                    return bytes;
                case CompressMode.CLZF:
                    return CLZF.Compress(bytes);
                case CompressMode.ZIP:
                    return ZIP.Compress(bytes);
                case CompressMode.Deflate:
                    return Deflate.Compress(bytes);
                default:
                    throw new Exception("未知压缩字节模式");
            }
        }

        /// <summary>
        /// 解压字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] bytes, CompressMode mode)
        {
            switch (mode)
            {
                case CompressMode.NoCompress:
                    return bytes;
                case CompressMode.CLZF:
                    return CLZF.Decompress(bytes);
                case CompressMode.ZIP:
                    return ZIP.Decompress(bytes);
                case CompressMode.Deflate:
                    return Deflate.Decompress(bytes);
                default:
                    throw new Exception("未知压缩字节模式");
            }
        }

        /// <summary>
        /// 图片解码
        /// 常见错误:
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <returns></returns>
        public static int BmpToFile(string img, CheckBox checkBox1, ComboBox comboBox1, TextBox textBox1, CompressMode compressMode)
        {
            var bmp = new Bitmap(img);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            var temp = new byte[bmp.Width * bmp.Height * 4];
            var ptr = data.Scan0;
            Marshal.Copy(ptr, temp, 0, data.Stride * data.Height);
            var temp2 = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                if (i % 4 == 3)
                {
                    continue;
                }
                else
                {
                    temp2.Append((char)temp[i]);
                }
            }
#if DEBUG
            MessageBox.Show(temp2.ToString());
            Item.WriteColorLine(temp2.ToString(), ConsoleColor.Blue);
#endif
            var values = temp2.ToString().ToDict();
            temp2 = null;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;

            if (values["code"] != coding.ToString())
            {
                MessageBox.Show("选择编码方式错误!\n" + "文件是用 " + CodingMode.NoCoding.Pause(values["code"]).GetValue() + " 方式编码的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                return 300;
            }

            if (values["compress"] != compressMode.ToString())
            {
                MessageBox.Show("选择压缩方式错误!\n" + "文件是用 " + CompressMode.NoCompress.Pause(values["compress"]).GetValue() + " 方法压缩的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                return 301;
            }

            var value = Base64.Decode(values["data"], key);
            try
            {
                var nowFile = new FileInfo(Base64.Decode(values["fileName"]));
                var nowImg = new FileInfo(img);
                if (File.Exists(nowImg.DirectoryName + "\\" + nowFile.Name))
                {
                    MessageBox.Show("文件: " + Base64.Decode(values["fileName"]) + " 已经存在!", "错误", MessageBoxButtons.OK);
                    return 200;
                }
                else
                {
                    var downValue = Item.StringToByte(value);
                    if (Item.MD5(downValue) != values["MD5"])
                    {
                        //Console.Write(values.ToString(true));
                        MessageBox.Show("密码错误!", "错误", MessageBoxButtons.OK);
                        CloseLoading();
                        GC.Collect();
                        return 303;
                    }

                    downValue = Item.Decompress(downValue, compressMode);
                    var downFile = File.Create(nowImg.DirectoryName + "\\" + nowFile.Name);
                    downFile.Write(downValue, 0, downValue.Length);
                    downValue = null;//尝试释放内存
                    MessageBox.Show("解码完成!", "通知", MessageBoxButtons.OK);
                    //Item.OpenOnWindows(new FileInfo(img).DirectoryName);
                    Item.OpenFile(downFile.Name);
                    downFile.Dispose();
                    downFile.Close();
                }

            }
            catch (InvalidCastException err)
            {
                MessageBox.Show("密码错误!" + err.Message, "错误", MessageBoxButtons.OK);
                return 303;
            }

            bmp.Dispose();
            //bmp.Clone();
            data = null;
            temp = null;
            //temp2 = null;
            return 101;
        }

        /// <summary>
        /// 图片解码
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <param name="outPath">输出存储位置</param>
        /// <returns></returns>
        public static int BmpToFile(string img, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode, string outPath)
        {
            if (File.Exists(outPath))
            {
                //MessageBox.Show("文件: " + outPath + " 已经存在!", "错误", MessageBoxButtons.OK);
                return 200;
            }
            var bmp = new Bitmap(img);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            var temp = new byte[bmp.Width * bmp.Height * 4];
            var ptr = data.Scan0;
            Marshal.Copy(ptr, temp, 0, data.Stride * data.Height);
            var temp2 = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                if (i % 4 == 3)
                {
                    continue;
                }
                else
                {
                    temp2.Append((char)temp[i]);
                }
            }
            var values = temp2.ToString().ToDict();
            temp2 = null;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;

            if (values["code"] != coding.ToString())
            {
                //MessageBox.Show("选择编码方式错误!\n" + "文件是用 " + CodingMode.NoCoding.Pause(values["code"]).GetValue() + " 方式编码的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                var ret = string.Format("300{0}", (int)CodingMode.NoCoding.Pause(values["code"]));
                return int.Parse(ret);
            }

            if (values["compress"] != compressMode.ToString())
            {
                //MessageBox.Show("选择压缩方式错误!\n" + "文件是用 " + CompressMode.NoCompress.Pause(values["compress"]).GetValue() + " 方法压缩的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                var ret = string.Format("301{0}", (int)CompressMode.NoCompress.Pause(values["compress"]));
                return int.Parse(ret);
            }

            var value = Base64.Decode(values["data"], key);
            try
            {
                var nowFile = new FileInfo(Base64.Decode(values["fileName"]));
                var nowImg = new FileInfo(img);

                var downValue = Item.StringToByte(value);
                if (Item.MD5(downValue) != values["MD5"])
                {
                    //Console.Write(values.ToString(true));
                    //MessageBox.Show("密码错误!", "错误", MessageBoxButtons.OK);
                    CloseLoading();
                    GC.Collect();
                    return 303;
                }

                downValue = Item.Decompress(downValue, compressMode);
                Console.WriteLine(outPath);
                var downFile = File.Create(outPath);//nowImg.DirectoryName + "\\" + nowFile.Name);
                downFile.Write(downValue, 0, downValue.Length);
                downValue = null;//尝试释放内存
                //MessageBox.Show("解码完成!", "通知", MessageBoxButtons.OK);
                //Item.OpenOnWindows(new FileInfo(img).DirectoryName);

                //使用-OP参数时不会调用explorer
                //Item.OpenFile(downFile.Name);
                downFile.Dispose();
                downFile.Close();
            }
            catch (InvalidCastException err)
            {
                //MessageBox.Show("密码错误!" + err.Message, "错误", MessageBoxButtons.OK);
                return 303;
            }

            bmp.Dispose();
            //bmp.Clone();
            data = null;
            temp = null;
            //temp2 = null;
            return 101;
        }

        /// <summary>
        /// 图片解码
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <param name="outPath">输出存储位置</param>
        /// <returns></returns>
        public static int BmpToFile(string img, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode)
        {
            var bmp = new Bitmap(img);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            var temp = new byte[bmp.Width * bmp.Height * 4];
            var ptr = data.Scan0;
            Marshal.Copy(ptr, temp, 0, data.Stride * data.Height);
            var temp2 = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                if (i % 4 == 3)
                {
                    continue;
                }
                else
                {
                    temp2.Append((char)temp[i]);
                }
            }
            var values = temp2.ToString().ToDict();
            temp2 = null;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;

            if (values["code"] != coding.ToString())
            {
                //MessageBox.Show("选择编码方式错误!\n" + "文件是用 " + CodingMode.NoCoding.Pause(values["code"]).GetValue() + " 方式编码的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                var ret = string.Format("300{0}", (int)CodingMode.NoCoding.Pause(values["code"]));
                return int.Parse(ret);
            }

            if (values["compress"] != compressMode.ToString())
            {
                //MessageBox.Show("选择压缩方式错误!\n" + "文件是用 " + CompressMode.NoCompress.Pause(values["compress"]).GetValue() + " 方法压缩的!", "错误", MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                var ret = string.Format("301{0}", (int)CompressMode.NoCompress.Pause(values["compress"]));
                return int.Parse(ret);
            }

            var value = Base64.Decode(values["data"], key);
            try
            {
                var nowFile = new FileInfo(Base64.Decode(values["fileName"]));
                var nowImg = new FileInfo(img);
                if (File.Exists(nowImg.DirectoryName + "\\" + nowFile.Name))
                {
                    //MessageBox.Show("文件: " + Base64.Decode(values["fileName"]) + " 已经存在!", "错误", MessageBoxButtons.OK);
                    return 200;
                }
                else
                {
                    var downValue = Item.StringToByte(value);
                    if (Item.MD5(downValue) != values["MD5"])
                    {
                        //Console.Write(values.ToString(true));
                        //MessageBox.Show("密码错误!", "错误", MessageBoxButtons.OK);
                        CloseLoading();
                        GC.Collect();
                        return 303;
                    }

                    downValue = Item.Decompress(downValue, compressMode);
                    var downFile = File.Create(nowImg.DirectoryName + "\\" + nowFile.Name);
                    downFile.Write(downValue, 0, downValue.Length);
                    downValue = null;//尝试释放内存
                    //MessageBox.Show("解码完成!", "通知", MessageBoxButtons.OK);
                    //Item.OpenOnWindows(new FileInfo(img).DirectoryName);
                    Item.OpenFile(downFile.Name);
                    downFile.Dispose();
                    downFile.Close();
                }

            }
            catch (InvalidCastException err)
            {
                //MessageBox.Show("密码错误!" + err.Message, "错误", MessageBoxButtons.OK);
                return 303;
            }

            bmp.Dispose();
            //bmp.Clone();
            data = null;
            temp = null;
            //temp2 = null;
            return 101;
        }

        /// <summary>
        /// 图片解码(尝试不经过base64编码)
        /// </summary>
        /// <param name="img"></param>
        /// <param name="checkBox1"></param>
        /// <param name="comboBox1"></param>
        /// <param name="textBox1"></param>
        /// <param name="compressMode"></param>
        /// <param name="outPath"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int BmpToFile(string img, bool checkBox1, CodingMode comboBox1, string textBox1, CompressMode compressMode, string outPath)
        {
            if (File.Exists(outPath))
            {
                return 200;
            }

            using (var imgFile = new FileInfo(img).OpenRead())
            {
                imgFile.Position = ICONLENGTH;

                var reader = new StreamReader(imgFile);
                var tempByte = new byte[4 + 2 + int.MaxValue.ToString().Length];
                imgFile.Read(tempByte, 0, tempByte.Length);
                var temp = Encoding.UTF8.GetString(tempByte);
#if DEBUG
                Console.WriteLine(temp); 
#endif
                var temp2 = temp.ToDict();
                int headSize = int.Parse(temp2["head"]);
                temp = temp2.ToString(true);

                imgFile.Position = ICONLENGTH + temp.Length + 1;
                tempByte = new byte[headSize];
                imgFile.Read(tempByte, 0, tempByte.Length);
                temp = Encoding.UTF8.GetString(tempByte);
                temp2 = temp.ToDict();

                var coding = checkBox1 ? comboBox1 : CodingMode.NoCoding;
                var key = "";
                switch (coding)
                {
                    case CodingMode.SHA256:
                        key = MD5(SHA256(textBox1));
                        break;
                    case CodingMode.MD5:
                        key = MD5(textBox1);
                        break;
                    case CodingMode.NoCoding:
                    default:
                        key = MD5("");
                        break;
                }
                var IV = Encoding.UTF8.GetBytes(MD5(key)).Separate(2);
                var ret = 100;

                if (temp2["code"] != coding.ToString())
                {
                    GC.Collect();
                    ret = string.Format("300{0}", (int)CodingMode.NoCoding.Pause(temp2["code"])).ToInt();
                    return ret;
                }

                if (temp2["compress"] != compressMode.ToString())
                {
                    GC.Collect();
                    ret = string.Format("301{0}", (int)CompressMode.NoCompress.Pause(temp2["compress"])).ToInt();
                    return ret;
                }

                var md5s = temp2["MD5"].Split('/');
                var eachPartSize = temp2["eachPartSize"].Split('/').ToInt();

                //开始解密
                using (var outFile = File.Create(outPath))
                {
                    imgFile.Position+=6;                         //;data:
                    string md5;
                    for (int i = 0; i < eachPartSize.Length; i++)
                    {
                        tempByte = new byte[eachPartSize[i]];
                        imgFile.Read(tempByte, 0, tempByte.Length);
                        //先进行解密
                        try
                        {
                            tempByte = AES.Decrypt(tempByte, key, IV);
                        }
                        catch (Exception)
                        {
                            ret = 303;
                            break;
                        }
                        md5 = MD5(tempByte);
                        if (md5!=md5s[i])
                        {
                            ret = 303;
                        }
                        //再解压
                        tempByte = Item.Decompress(tempByte, compressMode);
                        //再将内容写入解码文件
                        outFile.Write(tempByte, 0, tempByte.Length);
                    }
                }

                GC.Collect();
                if (ret>=300)
                {
                    File.Delete(outPath);
                }
                return ret;
            }
        }

        /// <summary>
        /// 文件编码
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <returns></returns>
        public static int FileToBmp(string file, CheckBox checkBox1, ComboBox comboBox1, TextBox textBox1, CompressMode compressMode)
        {
            if (File.Exists(file + ".EN.jpg"))
            {
                MessageBox.Show("文件: " + file + ".EN.jpg" + " 已经存在!", "错误", MessageBoxButtons.OK);
                return 200;
            }
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var temp2 = new Dictionary<string, string>();

            var tempByte = Item.ReadFile(file);
            tempByte = Item.Compress(tempByte, compressMode);
            temp = Item.ByteToString(tempByte);
            temp = Base64.Encode(temp, key);
            while (temp.Length % 4 != 0)
            {
                temp += "=";
            }
            temp2 = new Dictionary<string, string>()
            {
                {"data",temp },
                {"fileName",Base64.Encode(file) },
                {"size",temp.Length.ToString() },
                {"code",coding.ToString() },
                {"compress",compressMode.ToString() },
                {"MD5",Item.MD5(tempByte) },
                {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
            };

            var temp3 = Item.StringToByte(temp2.ToString(true));
            count = temp3.Length / 3;
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            //指针
            var ptr = data.Scan0;

            Item.Base64ToBitmapData(ref data, temp3);
            bmp.UnlockBits(data);
            Item.BmpToJpgSave(bmp, file + ".EN.jpg");
            MessageBox.Show("文件加密完成!", "通知", MessageBoxButtons.OK);
            //Item.OpenOnWindows(new FileInfo(file).DirectoryName);
            var filePath = file + ".EN.jpg";
            Item.OpenFile(filePath);
            bmp.Dispose();
            return 100;
        }

        /// <summary>
        /// 文件编码
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <returns></returns>
        public static int FileToBmp(string file, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode)
        {
            if (File.Exists(file + ".EN.jpg"))
            {
                //MessageBox.Show("文件: " + file + ".EN.jpg" + " 已经存在!", "错误", MessageBoxButtons.OK);
                return 200;
            }
            var count = file.Length;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var temp2 = new Dictionary<string, string>();

            var tempByte = Item.ReadFile(file);
            tempByte = Item.Compress(tempByte, compressMode);
            temp = Item.ByteToString(tempByte);
            temp = Base64.Encode(temp, key);
            while (temp.Length % 4 != 0)
            {
                temp += "=";
            }
            temp2 = new Dictionary<string, string>()
            {
                {"data",temp },
                {"fileName",Base64.Encode(file) },
                {"size",temp.Length.ToString() },
                {"code",coding.ToString() },
                {"compress",compressMode.ToString() },
                {"MD5",Item.MD5(tempByte) },
                {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
            };

            var temp3 = Item.StringToByte(temp2.ToString(true));
            count = temp3.Length / 3;
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            //指针
            var ptr = data.Scan0;

            Item.Base64ToBitmapData(ref data, temp3);
            bmp.UnlockBits(data);
            Item.BmpToJpgSave(bmp, file + ".EN.jpg");
            //MessageBox.Show("文件加密完成!", "通知", MessageBoxButtons.OK);
            //Item.OpenOnWindows(new FileInfo(file).DirectoryName);
            var filePath = file + ".EN.jpg";
            Item.OpenFile(filePath);
            bmp.Dispose();
            return 100;
        }

        /// <summary>
        /// 文件编码
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <returns></returns>
        public static int FileToBmp(string file, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode, string outPath)
        {
            if (File.Exists(outPath))
            {
                //MessageBox.Show("文件: " + outPath + " 已经存在!", "错误", MessageBoxButtons.OK);
                return 200;
            }
            var count = file.Length;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var temp2 = new Dictionary<string, string>();

            var tempByte = Item.ReadFile(file);
            tempByte = Item.Compress(tempByte, compressMode);
            temp = Item.ByteToString(tempByte);
            temp = Base64.Encode(temp, key);
            while (temp.Length % 4 != 0)
            {
                temp += "=";
            }
            temp2 = new Dictionary<string, string>()
                {
                    {"data",temp },
                    {"fileName",Base64.Encode(file) },
                    {"size",temp.Length.ToString() },
                    {"code",coding.ToString() },
                    {"compress",compressMode.ToString() },
                    {"MD5",Item.MD5(tempByte) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };

            var temp3 = Item.StringToByte(temp2.ToString(true));
            count = temp3.Length / 3;
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            //指针
            var ptr = data.Scan0;

            Item.Base64ToBitmapData(ref data, temp3);
            bmp.UnlockBits(data);
            //Item.BmpToJpgSave(bmp, file + ".EN.jpg");
            Item.BmpToJpgSave(bmp, outPath);        //使用-OP参数时不会使用explorer
            //MessageBox.Show("文件加密完成!", "通知", MessageBoxButtons.OK);
            //Item.OpenOnWindows(new FileInfo(file).DirectoryName);
            //var filePath = file + ".EN.jpg";
            //Item.OpenFile(filePath);
            bmp.Dispose();
            return 100;
        }

        /// <summary>
        /// 文件编码(尝试不经过base64编码)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="checkBox1"></param>
        /// <param name="comboBox1"></param>
        /// <param name="textBox1"></param>
        /// <param name="compressMode"></param>
        /// <param name="outPath"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int FileToBmp(string filePath, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode, string outPath, long size)
        {
            if (size == 0)
            {
                return FileToBmp(filePath, checkBox1, comboBox1, textBox1, compressMode, outPath);
            }
            if (File.Exists(outPath))
            {
                return 200;
            }

            var img = FileToImage.Properties.Resources.favicon;

            using (var outFile = File.Create(outPath))
            {
                img.Save(outFile);
                img.Dispose();

                var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;
                //var key = checkBox1 == false ? Base64._keyStr :
                //    comboBox1 == "无" ? Base64._keyStr :
                //    comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                //    Base64._keyStr;
                var key = "";
                switch (coding)
                {
                    case CodingMode.SHA256:
                        key = MD5(SHA256(textBox1));
                        break;
                    case CodingMode.MD5:
                        key = MD5(textBox1);
                        break;
                    case CodingMode.NoCoding:
                    default:
                        key = MD5("");
                        break;
                }
                var VI = Encoding.UTF8.GetBytes(MD5(key)).Separate(2);

                string temp;
                Dictionary<string, string> temp2;
                var file = new FileInfo(filePath);
                var fileCount = file.Length;
#if DEBUG
                Item.WriteColorLine(string.Format("文件大小:{0}", fileCount), ConsoleColor.Blue);
#endif
                size = size > fileCount ? fileCount : size;     //处理size大小,防止造成aes浪费
                var readIndex = (int)Math.Ceiling((double)(fileCount / size));//此参数用于显示读取位置,在最开始时候显示分块
                Console.WriteLine(string.Format("文件分块数量:{0}", readIndex));

                var eachPartSize = new List<int>();             //每块大小
                var md5s = new List<string>();                  //每块签名
                var readPart = new byte[size];                  //读取使用的字节块,防止重复分配
                double tempSizeCut = size / 4;                  //
                var saveSize = Math.Ceiling(tempSizeCut) * 4;   //记录数据每块大小,保证读取时正常
                byte[] tempByte;

                //开始加密
                using (var readStream = file.OpenRead())
                {
                    readIndex = 0;
                    ////分步操作,需要在内容开始写入分块大小
                    //temp = string.Format("{0}:{1};data:", "part", saveSize);
                    //tempByte = Item.StringToByte(temp);

                    ////不需要处理,直接写入文件
                    //outFile.Write(tempByte, 0, tempByte.Length);

                    //先将文件读取加密写入临时文件中
                    using (var tempFile = File.Create(TEMPFILE))
                    {
                        while (readIndex++ * size < fileCount)
                        {
                            readStream.Read(readPart, 0, (int)size);
                            tempByte = Item.Compress(readPart, compressMode);
                            md5s.Add(Item.MD5(tempByte));
                            //进行AES加密
                            tempByte = AES.Encrypt(tempByte, key, VI);
                            eachPartSize.Add(tempByte.Length);
                            //写入临时文件
                            tempFile.Write(tempByte, 0, tempByte.Length);
                        }
                    }
                    //由于分步操作,所以这里字典不需要记录data数据块
                    temp2 = new Dictionary<string, string>()
                    {
                        {"partNumber",(readIndex-1).ToString() },
                        {"eachPartSize",eachPartSize.Join("/") },
                        {"fileName",Base64.Encode(file.Name) },
                        {"size",fileCount.ToString() },
                        {"code",coding.ToString() },
                        {"compress",compressMode.ToString() },
                        {"MD5",md5s.Join("/") },
                        {"end","0" }//由于习惯原因,给结尾填上一个标记
                    };
                    temp = temp2.ToString(true);
                    tempByte = StringToByte(string.Format("head:{0};{1};data:",temp.Length,temp));
                    //将数据写入文件
                    outFile.Write(tempByte, 0, tempByte.Length);
                    //将临时文件中的数据读取并写入文件
                    using (var tempFile = new FileInfo(TEMPFILE).OpenRead())
                    {
                        tempFile.CopyTo(outFile);
                    }
                    //删除临时文件
                    File.Delete(TEMPFILE);
                }
                return 100;
            }
        }

        /// <summary>
        /// 文件编码(分步原版)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="checkBox1"></param>
        /// <param name="comboBox1"></param>
        /// <param name="textBox1"></param>
        /// <param name="compressMode"></param>
        /// <param name="outPath"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int FileToBmpBase(string filePath, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode, string outPath,long size)
        {
            if (size==0)
            {
                return FileToBmp(filePath, checkBox1, comboBox1, textBox1, compressMode, outPath);
            }
            if (File.Exists(outPath))
            {
                return 200;
            }
            var count = filePath.Length;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            Dictionary<string,string> temp2;
            var file = new FileInfo(filePath);
            var fileCount = file.Length;
            Item.WriteColorLine(string.Format("文件大小:{0}", fileCount), ConsoleColor.Blue);
            var readIndex = (int)Math.Ceiling((double)(fileCount / size));
            Console.WriteLine(readIndex);
            var md5s = new List<string>();
            var tempByte = new byte[size];
            double tempSizeCut = size / 4;
            var saveSize = Math.Ceiling(tempSizeCut) * 4;   //记录数据每块大小,保证读取时正常
            var saveLength = int.MaxValue;                  //记录数据编码完成后的最大值
            
            //记录添加的字节,用于计算Side
            count = new SUM(() =>
            {
                double t = 4 + 2 +                                  //part
                saveSize.ToString().Length +                        //每块的大小
                4 + 2 +                                             //data
                readIndex * saveSize +                              //数据保存大小
                8 + 2 +                                             //fileName
                Math.Ceiling((double)(file.Name.Length / 3)) * 4 +  //文件名大小
                4 + 2 +                                             //size
                saveLength.ToString().Length +                      //文件编码后大小(预计直接int最大值)
                8 + 2 +                                             //compress
                compressMode.ToString().Length +                    //压缩模式标记长度
                3 + 2 +                                             //MD5
                readIndex*33 +                                      //所有的签名(单个签名32位,加上分隔符33)
                4 + 2 +                                             //end标记
                20;                                                 //保留数据大小(防止意外)
                return (int)Math.Ceiling(t);
            })();

            count = count / 3;                                      //一个像素可以保存三个字节
            var side = (int)Math.Ceiling(Math.Sqrt(count));         //计算side
            Item.WriteColorLine(string.Format("SIDE大小:{0}", side), ConsoleColor.Blue);

            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            var ptr = data.Scan0;
            
            using (var readStream = file.OpenRead())
            {
                readIndex = 0;
                saveLength = 0;
                //分步操作,需要在图片最开头写入分块大小.
                temp = string.Format("{0}:{1};data:", "part",saveSize);
                tempByte = Item.StringToByte(temp);
                var tempCount = (int)Math.Ceiling(tempByte.Length / 3d) * 4;          //循环临时参数(count)
                var temp3 = new byte[tempCount];
                var j = 0;
                for (int i = 0; i < tempCount; i++)
                {
                    if (i%4==3)
                    {
                        temp3[i] = 255;
                    }
                    else
                    {
                        temp3[i] = tempByte.GetItem(j++);
                    }
                }
                //将已经处理好的数据直接通过内存操作放置到需要的位置
                Marshal.Copy(temp3, saveLength, ptr, tempCount);
                saveLength += tempCount;

                while (readIndex * size < fileCount)
                {
                    readStream.Read(tempByte, (readIndex++) * (int)size, readIndex * (int)size);
                    tempByte = Item.Compress(tempByte, compressMode);
                    md5s.Add(Item.MD5(tempByte));
                    temp = Item.ByteToString(tempByte);
                    temp = Base64.Encode(temp, key);
                    tempByte = Item.StringToByte(temp);

                    //因为是分步操作,所以不能直接使用Base64ToBitmapData,需要在这里重写这个模块
                    tempCount = (int)Math.Ceiling(tempByte.Length / 3d) * 4;
                    temp3 = readIndex == 1 ? new byte[tempCount] : temp3.Fill((byte)0);//就第一次的时候开辟空间,其他时候都用fill填充0
                    j = 0;
                    for (int i = 0; i < tempCount; i++)
                    {
                        temp3[i] = (i % 4 == 3) ? byte.MaxValue : tempByte.GetItem(j++);
                    }
                    //将已经处理好的数据直接通过内存操作放置到需要的位置
                    Marshal.Copy(temp3, saveLength, ptr, tempCount);
                    saveLength += tempCount;
                }
                //由于分步操作,所以这里字典不需要记录data数据块
                temp2 = new Dictionary<string, string>()
                {
                    {"fileName",Base64.Encode(file.Name) },
                    {"size",fileCount.ToString() },
                    {"code",coding.ToString() },
                    {"compress",compressMode.ToString() },
                    {"MD5",md5s.Join("/") },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };

                tempByte = StringToByte(string.Format(";{0}", temp2.ToString(true)));
                tempCount = (int)Math.Ceiling(tempByte.Length / 3d) * 4;
                temp3 = new byte[tempCount];
                j = 0;
                for (int i = 0; i < tempCount; i++)
                {
                    temp3[i] = (i % 4 == 3) ? byte.MaxValue : tempByte.GetItem(j++);
                }
                //将已经处理好的数据直接通过内存操作放置到需要的位置
                Marshal.Copy(temp3, saveLength, ptr, tempCount);
                saveLength += tempCount;
                //解锁内存
                bmp.UnlockBits(data);
                Item.BmpToJpgSave(bmp, outPath);        //使用-OP参数时不会使用explorer
                bmp.Dispose();
                return 100;
            }
        }

        /// <summary>
        /// 文件编码
        /// (分步原版2)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="checkBox1"></param>
        /// <param name="comboBox1"></param>
        /// <param name="textBox1"></param>
        /// <param name="compressMode"></param>
        /// <param name="outPath"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int FileToBmpBase2(string filePath, bool checkBox1, string comboBox1, string textBox1, CompressMode compressMode, string outPath,long size)
        {
            if (size==0)
            {
                return FileToBmp(filePath, checkBox1, comboBox1, textBox1, compressMode, outPath);
            }
            if (File.Exists(outPath))
            {
                return 200;
            }
            var count = filePath.Length;
            var coding = checkBox1 ? CodingMode.NoCoding.Pause(comboBox1) : CodingMode.NoCoding;
            var key = checkBox1 == false ? Base64._keyStr :
                comboBox1 == "无" ? Base64._keyStr :
                comboBox1 == "SHA256" ? Base64.GetKey(textBox1, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            Dictionary<string,string> temp2;
            var file = new FileInfo(filePath);
            var fileCount = file.Length;
            Item.WriteColorLine(string.Format("文件大小:{0}", fileCount), ConsoleColor.Blue);
            double tempDoubleValue = (double)fileCount / (double)size;
            var readIndex = Math.Ceiling(tempDoubleValue);
            Console.WriteLine("需要分:"+readIndex+"块");
            var md5s = new List<string>();
            var tempByte = new byte[size];
            tempDoubleValue = size / 3;
            var saveSize = Math.Ceiling(tempDoubleValue) * 4;   //记录数据每块大小,保证读取时正常
            var saveLength = int.MaxValue;                  //记录数据编码完成后的最大值
            
            //记录添加的字节,用于计算Side
            count = new SUM(() =>
            {
                double t = 4 + 2 +                                  //part
                saveSize.ToString().Length +                        //每块的大小
                4 + 2 +                                             //data
                readIndex * saveSize * 1 +                          //数据保存大小
                8 + 2 +                                             //fileName
                Math.Ceiling((double)(file.Name.Length / 3)) * 4 +  //文件名大小
                4 + 2 +                                             //size
                saveLength.ToString().Length +                      //文件编码后大小(预计直接int最大值)
                4 + 2 +                                             //code
                coding.ToString().Length +                          //加密标记长度
                8 + 2 +                                             //compress
                compressMode.ToString().Length +                    //压缩模式标记长度
                3 + 2 +                                             //MD5
                readIndex*33 +                                      //所有的签名(单个签名32位,加上分隔符33)
                4 + 2 +                                             //end标记
                size;                                               //保留一个块的数据大小(防止意外)
                return (int)Math.Ceiling(t);
            })();
            MessageBox.Show(count.ToString());
            count = (int)Math.Floor(count / 3d * 4);
            var temp3 = new byte[count];
            var temp3Index = 0;

            count = count / 3;                                      //一个像素可以保存三个字节
            

            using (var readStream = file.OpenRead())
            {
                readIndex = 0;
                saveLength = 0;
                //分步操作,需要在图片最开头写入分块大小.
                temp = string.Format("{0}:{1};data:", "part",saveSize);
                tempByte = Item.StringToByte(temp);
                //var tempCount = (int)Math.Ceiling(tempByte.Length / 3d) * 4;          //循环临时参数(count)
                var tempCount = (int)Math.Floor(tempByte.Length / 3d * 4);          //循环临时参数(count)
                var j = 0;
                for (int i = 0; i < tempCount; i++)
                {
                    temp3[temp3Index] = (temp3Index++ % 4 == 3) ? byte.MaxValue : tempByte.GetItem(j++);
                }
                //将已经处理好的数据直接通过内存操作放置到需要的位置
                //Marshal.Copy(temp3, saveLength, ptr, tempCount);
                saveLength += tempCount;

                while (readIndex * size < fileCount)
                {
                    tempByte = new byte[size];
                    readStream.Read(tempByte, 0, (int)size);
                    Console.WriteLine(tempByte.Length);
                    readIndex++;
                    tempByte = Item.Compress(tempByte, compressMode);
                    md5s.Add(Item.MD5(tempByte));
                    temp = Item.ByteToString(tempByte);
                    temp = Base64.Encode(temp, key);
                    tempByte = Item.StringToByte(temp);
                    Console.WriteLine(tempByte.Length);
                    Console.WriteLine();

                    //因为是分步操作,所以不能直接使用Base64ToBitmapData,需要在这里重写这个模块
                    tempCount = (int)Math.Floor(tempByte.Length / 3d * 4);
                    //temp3 = readIndex == 1 ? new byte[tempCount] : temp3.Fill((byte)0);//就第一次的时候开辟空间,其他时候都用fill填充0
                    j = 0;
                    for (int i = 0; i < tempCount; i++)
                    {
                        try
                        {
                            temp3[temp3Index] = (temp3Index++ % 4 == 3) ? byte.MaxValue : tempByte.GetItem(j++);
                        }
                        catch (Exception)
                        {
                            Item.WriteColorLine(Item.ByteToString(temp3), ConsoleColor.Blue);
                            throw;
                        }
                    }
                    //将已经处理好的数据直接通过内存操作放置到需要的位置
                    //Marshal.Copy(temp3, saveLength, ptr, tempCount);
                    saveLength += tempCount;
                }
                //由于分步操作,所以这里字典不需要记录data数据块
                temp2 = new Dictionary<string, string>()
                {
                    {"fileName",Base64.Encode(file.Name) },
                    {"size",fileCount.ToString() },
                    {"code",coding.ToString() },
                    {"compress",compressMode.ToString() },
                    {"MD5",md5s.Join("/") },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };

                tempByte = StringToByte(string.Format(";{0}", temp2.ToString(true)));
                tempCount = (int)Math.Floor(tempByte.Length / 3d * 4);
                //temp3 = new byte[tempCount];
                j = 0;
                for (int i = 0; i < tempCount; i++)
                {
                    try
                    {
                        temp3[temp3Index] = (temp3Index++ % 4 == 3) ? byte.MaxValue : tempByte.GetItem(j++);
                    }
                    catch (Exception)
                    {
                        Item.WriteColorLine(Item.ByteToString(temp3), ConsoleColor.Blue);
                        throw;
                    }                  
                }
                saveLength += tempCount;
                count = saveLength / 3;
                //因为这里实际上已经将全部内容读取并存储在内存里了,所以可以将side计算拖到后面来
                var side = (int)Math.Ceiling(Math.Sqrt(count));         //计算side
                Item.WriteColorLine(string.Format("SIDE大小:{0}", side), ConsoleColor.Blue);

                var bmp = new Bitmap(side, side);
                var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);
                var ptr = data.Scan0;
                //将已经处理好的数据直接通过内存操作放置到需要的位置
                //Marshal.Copy(temp3, saveLength, ptr, tempCount);
                
                Marshal.Copy(temp3, 0, ptr, saveLength);
                //Item.WriteColorLine(Item.ByteToString(temp3), ConsoleColor.Blue);
                //throw new Exception();
                saveLength += tempCount;
                //解锁内存
                bmp.UnlockBits(data);
                Item.BmpToJpgSave(bmp, outPath);        //使用-OP参数时不会使用explorer
                bmp.Dispose();
                return 100;
            }
        }

        /// <summary>
        /// 文件编码后显示
        /// </summary>
        /// <param name="img">图片所在路径</param>
        /// <param name="checkBox1">是否启用密码</param>
        /// <param name="comboBox1">下拉菜单</param>
        /// <param name="textBox1">密码框</param>
        /// <param name="compressMode">压缩模式</param>
        /// <param name="pictureBox1">图片显示框</param>
        /// <param name="label1">图片未显示时内容</param>
        /// <returns></returns>
        public static int FileToBmpShow(string file, CheckBox checkBox1, ComboBox comboBox1, TextBox textBox1, CompressMode compressMode, PictureBox pictureBox1, Label label1)
        {
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var temp2 = new Dictionary<string, string>();
            
                var tempByte = Item.ReadFile(file);
                tempByte = Item.Compress(tempByte,compressMode);
                temp = Item.ByteToString(tempByte);
                temp = Base64.Encode(temp, key);
                while (temp.Length % 4 != 0)
                {
                    temp += "=";
                }
                temp2 = new Dictionary<string, string>()
                {
                    {"data",temp },
                    {"fileName",Base64.Encode(file) },
                    {"size",temp.Length.ToString() },
                    {"code",coding.ToString() },
                    {"compress",compressMode.ToString() },
                    {"MD5",Item.MD5(tempByte) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };
            
            var temp3 = Item.StringToByte(temp2.ToString(true));
            count = temp3.Length / 3;
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            //指针
            var ptr = data.Scan0;
            byte[] rgbValues = new byte[side * side * 4];

            Item.Base64ToBitmapData(ref data, temp);
            bmp.UnlockBits(data);
            pictureBox1.Image = bmp;
            label1.Visible = false;
            return 100;
        }

        /// <summary>
        /// 打印有颜色的内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        public static void WriteColorLine(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = currentForeColor;
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
                if (x.Length==2)
                {
                    ret.Add(x[0], x[1]);
                }
                else
                {
                    break;
                }
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

        /// <summary>
        /// 将列表转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<T>(this T[] objects, bool tf)
        {
            if (tf)
            {
                var ret = typeof(T).ToString();
                ret += "[" + objects.Length + "] { ";
                for (int i = 0; i < objects.Length; i++)
                {
                    ret += objects[i].ToString();
                    if (i < objects.Length - 1)
                    {
                        ret += ", ";
                    }
                }
                ret += " }";
                return ret;
            }
            else
            {
                return objects.ToString();
            }
        }

        /// <summary>
        /// 将列表转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<T>(this List<T> ts,bool tf)
        {
            if (tf==false)
            {
                return ts.ToString();
            }
            else
            {
                var ret = "List<" + typeof(T).ToString()+"> ";
                ret += "[" + ts.Count + "] { ";
                for (int i = 0; i < ts.Count; i++)
                {
                    ret += ts[i].ToString();
                    if (i < ts.Count - 1)
                    {
                        ret += ", ";
                    }
                }
                ret += " }";
                return ret;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this List<T> ts,string add)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < ts.Count -1; i++)
            {
                ret.Append(ts[i]);
                ret.Append(add);
            }
            ret.Append(ts[ts.Count - 1]);
            return ret.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this T[] ts,string add)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < ts.Length - 1; i++)
            {
                ret.Append(ts[i]);
                ret.Append(add);
            }
            ret.Append(ts[ts.Length - 1]);
            return ret.ToString();
        }

        /// <summary>
        /// 填充
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=""></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Fill<T>(this T[] ts,T value)
        {
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i] = value;
            }
            return ts;
        }

        /// <summary>
        /// 数组分离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=""></param>
        /// <param name="groupNumber"></param>
        /// <returns></returns>
        public static T[] Separate<T>(this T[] ts, int groupNumber)
        {
            return ts.Separate(groupNumber, 0);
        }

        /// <summary>
        /// 数组分离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="groubNumber"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static T[] Separate<T>(this T[] ts,int groubNumber,int group)
        {
            if (groubNumber==0||groubNumber==1)
            {
                return ts;
            }
            var rets = new List<List<T>> { };
            for (int i = 0; i < groubNumber; i++)
            {
                rets.Add(new List<T>());
            }
            for (int i = 0; i < ts.Length; i++)
            {
                rets[i % groubNumber].Add(ts[i]);
            }
            return rets[group].ToArray();
        }

        /// <summary>
        /// 转为INT32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        /// <summary>
        /// 转为INT32数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int[] ToInt(this string[] str)
        {
            var ret = new int[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                ret[i] = str[i].ToInt();
            }
            return ret;
        }
    }

    /// <summary>
    /// 项目空间追加函数块
    /// </summary>
    public static class ProjectAdd
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CodingMode Pause(this CodingMode mode, string value)
        {
            switch (value)
            {
                case "无":
                case "No":
                case "False":
                case "false":
                case "null":
                case "none":
                case "None":
                case "no":
                case "NaN":
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Project Pause(this Project project, string value)
        {
            switch (value)
            {
                case "无":
                case "No":
                case "False":
                case "false":
                case "null":
                case "none":
                case "None":
                case "no":
                case "NaN":
                    return Project.NoInput;
                case "BTF":
                case "B2F":
                case "BmpToFile":
                case "Bmp2File":
                case "bmpToFile":
                case "bmp2File":
                    return Project.BmpToFile;
                case "FTB":
                case "F2B":
                case "FileToBmp":
                case "File2Bmp":
                case "fileToBmp":
                case "file2Bmp":
                    return Project.FileToBmp;
                default:
                    return Project.NoInput;
            }
        }
    }

}
