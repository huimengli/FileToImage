using System;
using System.Diagnostics;
using System.Windows.Forms;
using FileToImage.Project;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace FileToImage
{
    static class Program
    {
        #region 固定参数
        /// <summary>
        /// 读取功能项
        /// </summary>
        private static Regex Read_ = new Regex(@"([-\/])?([^\/-]*)");

        /// <summary>
        /// 需要展现给用户的消息
        /// </summary>
        private static byte[] message = new byte[] { 197, 86, 91, 111, 26, 71, 20, 126, 183, 228, 255, 48, 126, 91, 36, 175, 23, 187, 113, 31, 144, 170, 170, 36, 114, 19, 81, 28, 36, 44, 71, 13, 226, 129, 212, 27, 217, 10, 100, 17, 216, 105, 210, 39, 136, 13, 216, 220, 235, 16, 112, 109, 210, 224, 212, 56, 88, 50, 23, 213, 150, 193, 92, 204, 127, 105, 119, 102, 215, 79, 249, 11, 61, 179, 179, 54, 32, 227, 74, 77, 81, 43, 173, 224, 204, 206, 119, 206, 156, 57, 243, 157, 111, 246, 83, 171, 251, 71, 32, 51, 170, 103, 124, 12, 126, 254, 12, 4, 110, 62, 36, 27, 145, 91, 103, 120, 59, 142, 247, 46, 148, 173, 136, 220, 220, 86, 59, 199, 184, 94, 196, 161, 250, 80, 252, 223, 6, 251, 167, 207, 103, 5, 147, 59, 121, 53, 16, 250, 212, 222, 85, 90, 59, 228, 195, 161, 178, 189, 62, 24, 108, 164, 53, 27, 93, 48, 135, 248, 82, 156, 119, 121, 68, 231, 20, 24, 136, 119, 152, 23, 230, 132, 185, 5, 179, 0, 255, 143, 168, 241, 200, 137, 248, 7, 54, 228, 120, 186, 226, 102, 56, 52, 62, 198, 219, 145, 195, 235, 242, 173, 218, 87, 126, 18, 77, 211, 198, 153, 59, 194, 244, 51, 97, 218, 104, 125, 34, 24, 167, 140, 179, 223, 130, 199, 67, 240, 144, 214, 86, 109, 174, 213, 101, 205, 225, 174, 21, 57, 230, 37, 225, 238, 119, 143, 231, 132, 199, 15, 108, 194, 61, 241, 169, 219, 181, 10, 193, 248, 121, 11, 226, 45, 48, 107, 189, 55, 43, 216, 239, 127, 51, 51, 251, 165, 19, 240, 150, 69, 228, 248, 94, 90, 67, 54, 151, 223, 255, 163, 228, 91, 130, 119, 255, 127, 170, 182, 27, 57, 141, 244, 36, 96, 237, 175, 17, 127, 31, 241, 203, 162, 219, 139, 212, 100, 29, 167, 178, 184, 81, 193, 209, 35, 152, 129, 237, 33, 158, 110, 108, 65, 50, 123, 188, 136, 181, 7, 180, 132, 220, 104, 178, 14, 1, 12, 212, 1, 241, 79, 60, 222, 5, 105, 14, 128, 136, 189, 103, 24, 134, 7, 12, 212, 135, 95, 121, 238, 101, 251, 213, 77, 47, 53, 213, 139, 55, 56, 84, 212, 227, 214, 171, 248, 98, 3, 208, 80, 27, 94, 186, 194, 74, 125, 200, 72, 179, 31, 201, 93, 182, 118, 212, 202, 1, 78, 190, 103, 99, 195, 248, 216, 208, 22, 225, 228, 78, 87, 201, 148, 72, 249, 0, 167, 94, 147, 183, 53, 146, 59, 155, 148, 27, 9, 185, 189, 171, 214, 214, 97, 66, 124, 233, 117, 75, 62, 209, 7, 254, 26, 47, 158, 139, 226, 146, 69, 124, 197, 140, 103, 96, 92, 230, 3, 234, 97, 16, 87, 195, 74, 33, 200, 129, 39, 75, 218, 68, 114, 5, 234, 2, 52, 226, 1, 101, 149, 150, 68, 205, 240, 80, 3, 71, 11, 128, 39, 165, 125, 220, 78, 233, 121, 94, 227, 23, 53, 216, 162, 203, 189, 198, 28, 94, 104, 22, 11, 143, 195, 33, 92, 57, 7, 20, 148, 192, 171, 159, 184, 62, 197, 41, 229, 45, 156, 142, 203, 205, 228, 21, 123, 25, 113, 193, 92, 52, 48, 6, 241, 63, 72, 30, 175, 79, 244, 251, 89, 50, 87, 35, 150, 81, 50, 166, 180, 143, 134, 102, 100, 71, 188, 31, 136, 138, 112, 186, 10, 229, 192, 155, 97, 252, 46, 55, 9, 22, 44, 133, 35, 41, 92, 251, 153, 138, 96, 60, 139, 15, 62, 178, 242, 67, 1, 105, 158, 229, 29, 156, 40, 0, 140, 114, 104, 164, 148, 124, 241, 197, 212, 29, 146, 173, 225, 15, 191, 178, 114, 128, 186, 209, 53, 148, 100, 13, 231, 75, 36, 127, 172, 148, 98, 184, 153, 34, 153, 42, 137, 7, 149, 189, 83, 146, 44, 226, 90, 88, 79, 45, 150, 37, 177, 14, 222, 172, 65, 175, 202, 141, 168, 218, 221, 83, 247, 227, 228, 77, 2, 100, 82, 110, 134, 199, 199, 212, 106, 157, 156, 148, 200, 70, 138, 1, 149, 204, 123, 178, 153, 86, 118, 55, 116, 225, 79, 39, 72, 62, 128, 139, 29, 122, 200, 135, 65, 185, 91, 33, 25, 88, 61, 142, 211, 135, 120, 243, 23, 32, 12, 174, 182, 213, 200, 41, 164, 194, 50, 200, 21, 200, 201, 91, 245, 227, 111, 112, 58, 35, 239, 74, 70, 90, 53, 190, 46, 95, 196, 28, 36, 123, 78, 98, 199, 56, 85, 151, 27, 101, 232, 43, 185, 83, 192, 205, 12, 163, 33, 36, 207, 170, 228, 228, 24, 4, 70, 0, 129, 172, 46, 35, 81, 131, 233, 134, 126, 105, 237, 58, 168, 87, 84, 98, 188, 61, 121, 129, 98, 246, 38, 229, 86, 177, 55, 199, 88, 77, 19, 104, 4, 112, 187, 192, 90, 29, 159, 159, 201, 221, 119, 192, 134, 33, 98, 169, 233, 199, 103, 46, 198, 234, 74, 85, 164, 153, 214, 143, 167, 127, 37, 218, 0, 253, 93, 102, 98, 175, 230, 37, 142, 252, 190, 79, 242, 91, 108, 206, 160, 171, 59, 231, 95, 118, 193, 159, 254, 82, 243, 135, 126, 25, 232, 9, 54, 160, 194, 0, 155, 171, 236, 82, 121, 168, 28, 233, 172, 218, 59, 197, 181, 20, 45, 38, 245, 234, 45, 161, 121, 24, 38, 53, 209, 214, 5, 155, 138, 55, 215, 31, 215, 48, 242, 246, 152, 152, 24, 201, 231, 198, 196, 196, 64, 40, 214, 22, 147, 140, 118, 188, 29, 119, 67, 151, 251, 45, 125, 244, 208, 70, 226, 17, 168, 9, 211, 96, 185, 147, 80, 58, 149, 219, 226, 252, 219, 148, 254, 91, 230, 247, 221, 211, 78, 118, 243, 244, 110, 229, 91, 239, 228, 155, 172, 37, 213, 20, 104, 102, 47, 20, 72, 37, 16, 230, 182, 118, 129, 45, 2, 209, 135, 69, 102, 148, 97, 244, 185, 102, 59, 200, 240, 117, 82, 26, 115, 237, 104, 64, 170, 77, 87, 116, 172, 93, 40, 237, 0, 78, 156, 208, 219, 185, 156, 83, 163, 175, 89, 26, 76, 226, 17, 253, 12, 225, 190, 154, 182, 152, 13, 2, 252, 8, 51, 86, 51, 251, 24, 49, 115, 84, 243, 0, 25, 10, 170, 149, 6, 141, 55, 107, 180, 154, 71, 77, 219, 191, 0 };

        /// <summary>
        /// 需要展现给用户的信息
        /// </summary>
        private static string Message
        {
            get
            {
                var ret = Item.Decompress(message, CompressMode.Deflate);
                return Encoding.UTF8.GetString(ret);
            }
        }

        /// <summary>
        /// 是否是控制台模式
        /// </summary>
        private static bool IsConsoleMode = true;
        #endregion

        #region 调用CMD输出(已经用不上了)
        //[DllImport("kernel32.dll")]
        //static extern bool FreeConsole();
        //[DllImport("kernel32.dll")]
        //public static extern bool AllocConsole();
        #endregion

        #region 禁止显示CMD窗口

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetStdHandle([MarshalAs(UnmanagedType.I4)]int nStdHandle);

        // see the comment below
        private enum StdHandle
        {
            StdIn = -10,
            StdOut = -11,
            StdErr = -12
        };

        /// <summary>
        /// 禁止非控制台启动时显示CMD界面
        /// </summary>
        static void HideConsole()
        {
            var ptr = GetStdHandle((int)StdHandle.StdOut);
            if (!CloseHandle(ptr))
                throw new Win32Exception();

            ptr = IntPtr.Zero;

            if (!FreeConsole())
                throw new Win32Exception();
        }
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                IsConsoleMode = false;
#if !DEBUG
                try
                {
                    HideConsole();//调试的时候运行此代码会报错
                }
                catch (Exception)
                {

                } 
#endif
                //防止多个工具同时运行
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    MessageBox.Show("程序已经在运行！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
#if DEBUG
                //MessageBox.Show("DEBUG");
                //程序启动
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
                return 0;
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
                    return 0;
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
                    return 0;
                }
#endif
            }
            else
            {
                //调用系统api
                //AllocConsole();
                //处理未捕获的异常模式
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                var project = Project.Project.NoInput;
                var inputpath = "";
                var outpath = "";
                var needkey = false;
                var keyMode = CodingMode.NoCoding;
                var keyValue = "";
                var compressMode = CompressMode.NoCompress;
                var size = -1;
#if DEBUG
                var isTest = false;
                MessageBox.Show(args.ToString(true));
#endif

                var ret = 0;

                //Console.WriteLine(args.ToString());
                //MessageBox.Show(args.ToString(true));

                // 特殊参数检查
                if (args.Length == 1 && !(args[0].StartsWith("-") || args[0].StartsWith("/")))
                {
                    inputpath = args[0];
                    FileInfo imgFile = new FileInfo(inputpath);
                    if (imgFile.Exists)
                    {
                        //判断工作模式
                        while (project == Project.Project.NoInput)
                        {
                            Item.WriteColorLine("需要设置工作模式!\n", ConsoleColor.Yellow);
                            var temp = Interaction.InputBox("请输入工作模式!", "提示", "BTF/FTB");
                            project = Project.Project.NoInput.Pause(temp);
                        }

                        // 根据工作模式继续输入内容
                        switch (project)
                        {
                            default:
                            case Project.Project.NoInput:
                                ret = 406; break;
                            case Project.Project.BmpToFile:
                                // 尝试从文件名中读取参数
                                try
                                {
                                    // 读取文件名中的参数
                                    string extension = imgFile.Extension;
                                    string fullName = imgFile.FullName.Remove(imgFile.FullName.Length - extension.Length,extension.Length);
                                    string[] @params = fullName.Split('_');

                                    // 读取输出文件名
                                    outpath = @params[0];

                                    // 读取其他参数
                                    Match match;
                                    string t;
                                    for (int i = 1; i < @params.Length; i++)
                                    {
                                        string[] targs = @params[i].Split('#');
                                        match = Read_.Match(targs[0]);
                                        t = match.Groups[1].ToString();
                                        if (t == "-")
                                        {
                                            t = match.Groups[2].ToString();

                                            // 是否需要密码
                                            if (t == "P" || t == "password")
                                            {
                                                needkey = true;
                                                keyMode = CodingMode.SHA256;
                                                keyValue = Item.Base64Decode(targs[1]);
                                            }
                                            // 压缩模式
                                            else if(t == "CM" || t == "compressMode" || t == "compressmode")
                                            {
                                                try
                                                {
                                                    compressMode = CompressMode.NoCompress.Pause(targs[1]);
                                                }
                                                catch (Exception)
                                                {
                                                    compressMode = CompressMode.NoCompress;
                                                    Item.WriteColorLine("压缩模式输入错误!\n已经选择默认压缩模式", ConsoleColor.Red);
                                                }
                                            }
                                            //分块大小
                                            else if (t == "S" || t == "size")
                                            {
                                                size = Item.LimitBytes(Item.GetBytes(targs[1]));
#if DEBUG
                                                Console.WriteLine(string.Format("SIZE:{0}", size));
#endif
                                            }
                                        }
                                    }

                                    // 解码图片
                                    ret = Item.BmpToFile(inputpath, needkey, keyMode, keyValue, compressMode, outpath);
                                }
                                catch (Exception ex)
                                {
                                    Item.WriteColorLine("尝试直接解码图片失败!", ConsoleColor.Red);
                                    Item.WriteColorLine("错误信息:\n", ConsoleColor.Red);
#if DEBUG
                                    throw ex;
#else
                                    Item.WriteColorLine(ex.Message, ConsoleColor.Red);
#endif
                                }

                                break;
                            case Project.Project.FileToBmp:
                                // 设置工作参数

                                // 设置输出文件参数
                                StringBuilder outputBuilder = new StringBuilder();

                                // 密码加密
                                var temp = Interaction.InputBox("是否需要加密?(需要的话直接输入密码)", "提示", "");
                                if (string.IsNullOrEmpty(temp))
                                {
                                    needkey = false;
                                    keyMode = CodingMode.NoCoding;
                                    keyValue = "";
                                }
                                else
                                {
                                    needkey = true;
                                    keyMode = CodingMode.SHA256;
                                    keyValue = temp;

                                    outputBuilder.Append("-P#");
                                    outputBuilder.Append(Item.Base64Encode(temp));
                                    outputBuilder.Append("_");
                                }

                                // 压缩模式
                                temp = Interaction.InputBox(
                                    "是否需要压缩?(为空则不压缩)",
                                    "提示",
                                    Enum.GetValues(typeof(CompressMode))
                                        .Cast<CompressMode>()
                                        .Select(value => value.ToString())
                                        .ToArray()
                                        .Join(",")
                                );
                                if (string.IsNullOrEmpty(temp))
                                {
                                    compressMode = CompressMode.NoCompress;
                                }
                                else
                                {
                                    try
                                    {
                                        compressMode = CompressMode.NoCompress.Pause(temp);
                                        if(compressMode != CompressMode.NoCompress)
                                        {
                                            outputBuilder.Append("-CM#");
                                            outputBuilder.Append(compressMode.ToString());
                                            outputBuilder.Append("_");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        compressMode = CompressMode.NoCompress;
                                        Item.WriteColorLine("压缩模式输入错误!\n已经选择默认压缩模式", ConsoleColor.Red);
                                    }
                                }

                                // 默认分块(50MB)
                                size = Item.LimitBytes(Item.GetBytes("50mb"));
                                outputBuilder.Append("-S#1");

                                // 设置输出文件
                                outpath = $"{imgFile.FullName}_{outputBuilder.ToString()}.png";

                                // 转码文件
                                ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath, size);
                                break;
                        }
                    }
                    else
                    {
                        ret = 201;
                    }
                }
                else  // 原有处理逻辑
                {
                    Match match;
                    string temp;
                    //检查给予参数
                    for (int i = 0; i < args.Length; i++)
                    {
                        match = Read_.Match(args[i]);
                        temp = match.Groups[1].ToString();
                        if (temp == "/" || temp == "-")
                        {
                            temp = match.Groups[2].ToString();
                            //帮助
                            if (temp == "?" || temp == "help")
                            {
                                //MessageBox.Show(message, "帮助", MessageBoxButtons.OK);
                                //return message;
                                Console.WriteLine(Message);
                                return 0;
                            }
#if DEBUG
                            //测试
                            else if (temp == "test" || temp == "T")
                            {
                                //var tempStr = new StringBuilder();
                                //for (int x = 0; x < 10; x++)
                                //{
                                //    tempStr.Append(i);
                                //    Console.WriteLine(i);
                                //    Console.WriteLine(Base64.Encode(tempStr.ToString()));
                                //}

                                //测试填充速度
                                //var tempByte = new byte[100000];
                                //var t1 = DateTime.Now;
                                //tempByte.Fill((byte)12);
                                //var t2 = DateTime.Now;
                                //Console.WriteLine(t2.ToFileTime() - t1.ToFileTime());
                                //return 0;

                                //测试加密时间
                                isTest = true;
                            }

                            //内容压缩
                            else if (temp == "CC" || temp == "contentCompression")
                            {
                                var file = new FileInfo(args[i + 1]);
                                if (file.Exists)
                                {
                                    using (var fs = file.OpenRead())
                                    {
                                        var allvalue = new byte[file.Length];
                                        fs.Read(allvalue, 0, allvalue.Length);
                                        allvalue = Item.Compress(allvalue, CompressMode.Deflate);
                                        Console.WriteLine(allvalue.ToString(true));
                                    }
                                }
                                return 0;
                            }
#endif
                            //工作模式
                            else if (temp == "FTB" || temp == "fileToBmp")
                            {
                                if (project == Project.Project.NoInput)
                                {
                                    project = Project.Project.FileToBmp;
                                }
                                else
                                {
                                    MessageBox.Show("不能多次设定工作模式!", "错误", MessageBoxButtons.OK);
                                    return 404;
                                }
                            }
                            else if (temp == "BTF" || temp == "bmpToFile")
                            {
                                if (project == Project.Project.NoInput)
                                {
                                    project = Project.Project.BmpToFile;
                                }
                                else
                                {
                                    MessageBox.Show("不能多次设定工作模式!", "错误", MessageBoxButtons.OK);
                                    return 404;
                                }
                            }
                            else if (temp == "FTBW" || temp == "fileToBmpWait")
                            {
                                if (project == Project.Project.NoInput)
                                {
                                    project = Project.Project.FileToBmpWait;
                                }
                                else
                                {
                                    MessageBox.Show("不能多次设定工作模式!", "错误", MessageBoxButtons.OK);
                                    return 404;
                                }
                            }
                            else if (temp == "BTFW" || temp == "bmpToFileWait")
                            {
                                if (project == Project.Project.NoInput)
                                {
                                    project = Project.Project.BmpToFileWait;
                                }
                                else
                                {
                                    MessageBox.Show("不能多次设定工作模式!", "错误", MessageBoxButtons.OK);
                                    return 404;
                                }
                            }
                            //输入文件路径
                            else if (temp == "IP" || temp == "inputPath" || temp == "inputpath")
                            {
                                inputpath = args[i + 1];
                            }
                            //输出文件路径
                            else if (temp == "OP" || temp == "outPath" || temp == "outpath")
                            {
                                outpath = args[i + 1];
                            }
                            //是否需要密码
                            else if (temp == "NK" || temp == "needKey" || temp == "needkey")
                            {
                                needkey = true;
                            }
                            //加密模式
                            else if (temp == "KM" || temp == "keyMode" || temp == "keymode")
                            {
                                keyMode = CodingMode.NoCoding.Pause(args[i + 1]);
                            }
                            //输入密码
                            else if (temp == "KV" || temp == "keyValue" || temp == "keyvalue")
                            {
                                keyValue = args[i + 1];
                            }
                            else if (temp == "P" || temp == "password")
                            {
                                needkey = true;
                                keyMode = CodingMode.SHA256;
                                keyValue = args[i + 1];
                            }
                            //压缩模式
                            else if (temp == "CM" || temp == "compressMode" || temp == "compressmode")
                            {
                                try
                                {
                                    compressMode = CompressMode.NoCompress.Pause(args[i + 1]);
                                }
                                catch (Exception)
                                {
                                    compressMode = CompressMode.NoCompress;
                                    Item.WriteColorLine("压缩模式输入错误!\n已经选择默认压缩模式", ConsoleColor.Red);
                                }
                            }
                            //分块大小
                            else if (temp == "S" || temp == "size")
                            {
                                size = Item.LimitBytes(Item.GetBytes(args[i + 1]));
#if DEBUG
                                Console.WriteLine(string.Format("SIZE:{0}", size));
#endif
                                switch (project)
                                {
                                    case Project.Project.NoInput:
                                        break;
                                    case Project.Project.FileToBmp:
                                        project = Project.Project.FileToBmpWait;
                                        break;
                                    case Project.Project.BmpToFile:
                                        project = Project.Project.BmpToFileWait;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }                

                #region 检查缺少的参数
                //如果缺少工作模式
                while (project == Project.Project.NoInput)
                {
                    Item.WriteColorLine("工作模式错误!\n需要重新输入!\n",ConsoleColor.Yellow);
                    var temp = Interaction.InputBox("请输入工作模式!", "提示", "BTF/FTB");
                    project = Project.Project.NoInput.Pause(temp);
                }
                //如果缺少输入文件
                while (inputpath == null || string.IsNullOrEmpty(inputpath))
                {
                    inputpath = Item.GetFile();
                    if (!File.Exists(inputpath))
                    {
                        inputpath = "";
                    }
                }
                #endregion

#if DEBUG
                DateTime T1, T2;
                T1 = DateTime.Now; 
#endif
                //运行功能
                switch (project)
                {
                    case Project.Project.NoInput:
                        Item.WriteColorLine("没有选择功能!",ConsoleColor.Red);
                        return 404;
                    case Project.Project.FileToBmp:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode);
                        }
                        else
                        {
                            ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath);
                        }
                        break;
                    case Project.Project.BmpToFile:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = Item.BmpToFile(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode);
                        }
                        else
                        {
                            ret = Item.BmpToFile(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath);
                        }
                        break;
                    case Project.Project.FileToBmpWait:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = 401;
                        }
                        else if (size<0)
                        {
                            ret = 402;
                        }
                        else
                        {
                            //ret = Item.FileToBmpBase2(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath,size);
                            ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath,size);
                        }
                        break;
                    case Project.Project.BmpToFileWait:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = Item.BmpToFile(inputpath, needkey, keyMode, keyValue, compressMode, null);
                        }
                        else
                        {
                            //ret = Item.FileToBmpBase2(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath,size);
                            ret = Item.BmpToFile(inputpath, needkey, keyMode, keyValue, compressMode, outpath);
                        }
                        break;
                    default:
                        Item.WriteColorLine("没有这个功能!",ConsoleColor.Red);
                        break;
                }
#if DEBUG
                if (isTest)
                {
                    T2 = DateTime.Now;
                    Item.WriteColorLine(string.Format("编码使用的时间:{0}",(T2-T1).ToString()),ConsoleColor.Blue);
                }
#endif

                switch (ret)
                {
                    case 100:
                        Item.WriteColorLine( "编码成功",ConsoleColor.Green); break;
                    case 101:
                        Item.WriteColorLine( "解码成功", ConsoleColor.Green); break;
                    case 200:
                        Item.WriteColorLine( "文件已经存在!",ConsoleColor.Yellow); break;
                    case 201:
                        Item.WriteColorLine("文件不存在!", ConsoleColor.Yellow); break;
                    case 300:
                        Item.WriteColorLine( "编码方式错误!",ConsoleColor.Red); break;
                    case 3000:
                        Item.WriteColorLine( "编码方式错误!\n文件是用"+CodingMode.NoCoding.GetValue()+"方式编码的",ConsoleColor.Yellow); break;
                    case 3001:
                        Item.WriteColorLine( "编码方式错误!\n文件是用"+CodingMode.SHA256.GetValue()+"方式编码的",ConsoleColor.Yellow); break;
                    case 3002:
                        Item.WriteColorLine( "编码方式错误!\n文件是用"+CodingMode.MD5.GetValue()+"方式编码的",ConsoleColor.Yellow); break;
                    case 301:
                        Item.WriteColorLine( "压缩方式错误!",ConsoleColor.Red); break;
                    case 3010:
                        Item.WriteColorLine( "压缩方式错误!\n文件是用"+CompressMode.NoCompress.GetValue()+"方式压缩的",ConsoleColor.Yellow); break;
                    case 3011:
                        Item.WriteColorLine( "压缩方式错误!\n文件是用"+CompressMode.CLZF.GetValue()+"方式压缩的",ConsoleColor.Yellow); break;
                    case 3012:
                        Item.WriteColorLine( "压缩方式错误!\n文件是用"+CompressMode.ZIP.GetValue()+"方式压缩的",ConsoleColor.Yellow); break;
                    case 3013:
                        Item.WriteColorLine( "压缩方式错误!\n文件是用"+CompressMode.Deflate.GetValue()+"方式压缩的",ConsoleColor.Yellow); break;
                    case 303:
                        Item.WriteColorLine( "密码错误!",ConsoleColor.Red); break;
                    case 401:
                        Item.WriteColorLine("使用分步式必须输入-OP(输出位置)参数!", ConsoleColor.Red); break;
                    case 402:
                        Item.WriteColorLine("使用分步式必须输入-S(分块大小)参数!", ConsoleColor.Red); break;
                    case 404:
                        Item.WriteColorLine("不能多次设定运行模式!",ConsoleColor.Red);break;
                    case 405:
                        Item.WriteColorLine("此功能尚未完成!",ConsoleColor.Red);break;
                    case 406:
                        Item.WriteColorLine("错误的工作模式!", ConsoleColor.Red);break;
                    default:
                        Item.WriteColorLine( "未知错误!",ConsoleColor.Red); break;
                }
                //释放控制台
                //FreeConsole();
                return ret;
                //}
                //catch (Exception err)
                //{
                //    MessageBox.Show(string.Format("图片和文件互转错误:{0} \n\r堆栈信息:{1}", err.Message,err.StackTrace),"系统错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                //}
            }
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

            if (IsConsoleMode)
            {
                Item.WriteColorLine("系统错误!", ConsoleColor.DarkRed);
            }
            else
            {
                MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            if (IsConsoleMode)
            {
                Item.WriteColorLine("系统错误!", ConsoleColor.DarkRed);
            }
            else
            {
                MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //MessageBox.Show("发生致命错误，请停止当前操作并及时联系作者！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void My_CurrentDomain_UnhandledException(object sender,UnhandledExceptionEventArgs e)
        {

        }
    }
}

