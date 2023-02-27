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
        private static byte[] message = new byte[944] { 197, 85, 91, 79, 19, 81, 16, 126, 39, 225, 63, 28, 222, 182, 9, 203, 22, 34, 62, 52, 49, 198, 106, 16, 82, 139, 77, 74, 48, 210, 244, 161, 202, 26, 8, 45, 187, 105, 65, 209, 167, 34, 148, 10, 20, 138, 136, 64, 184, 196, 162, 92, 19, 90, 26, 49, 180, 244, 198, 127, 209, 61, 187, 203, 147, 127, 193, 57, 59, 11, 109, 67, 49, 17, 27, 77, 54, 187, 115, 246, 124, 51, 103, 206, 204, 55, 51, 63, 243, 103, 223, 195, 75, 245, 122, 26, 27, 224, 245, 35, 28, 190, 250, 168, 203, 81, 37, 127, 66, 23, 99, 116, 189, 164, 77, 71, 149, 220, 162, 94, 60, 164, 153, 29, 26, 201, 212, 196, 255, 214, 216, 159, 62, 55, 50, 166, 20, 55, 244, 112, 228, 103, 97, 77, 203, 175, 170, 159, 119, 181, 197, 137, 106, 99, 117, 141, 89, 253, 140, 121, 196, 49, 177, 219, 23, 16, 189, 45, 32, 16, 222, 99, 239, 233, 16, 58, 122, 236, 2, 124, 159, 48, 225, 137, 151, 240, 93, 46, 226, 121, 49, 232, 71, 28, 105, 108, 224, 221, 196, 35, 251, 130, 35, 238, 193, 55, 162, 173, 213, 218, 118, 75, 104, 29, 18, 90, 173, 206, 103, 130, 181, 197, 218, 254, 16, 52, 30, 131, 134, 52, 58, 226, 242, 141, 12, 24, 10, 247, 157, 196, 211, 45, 9, 247, 31, 245, 117, 8, 125, 93, 46, 225, 129, 248, 194, 239, 27, 1, 99, 124, 183, 131, 240, 14, 216, 117, 62, 104, 23, 220, 157, 247, 218, 218, 111, 123, 1, 239, 232, 37, 158, 167, 210, 40, 113, 249, 66, 161, 87, 82, 176, 31, 254, 253, 127, 87, 93, 87, 124, 170, 107, 38, 224, 236, 187, 132, 239, 36, 252, 128, 232, 151, 137, 62, 159, 161, 241, 101, 154, 77, 209, 153, 3, 216, 129, 235, 17, 158, 93, 172, 71, 178, 7, 100, 130, 229, 1, 37, 161, 100, 115, 88, 33, 128, 129, 56, 16, 254, 89, 64, 238, 145, 58, 0, 72, 240, 63, 98, 16, 15, 24, 136, 15, 63, 56, 44, 227, 125, 77, 81, 102, 162, 94, 250, 64, 35, 59, 166, 221, 204, 17, 45, 77, 2, 26, 98, 195, 75, 23, 88, 169, 2, 25, 205, 85, 34, 185, 243, 252, 170, 158, 218, 166, 243, 159, 112, 109, 105, 108, 168, 89, 34, 156, 82, 60, 211, 150, 246, 213, 228, 54, 141, 191, 85, 63, 166, 213, 149, 147, 102, 37, 59, 167, 20, 214, 244, 244, 4, 108, 136, 99, 178, 95, 10, 138, 65, 208, 55, 120, 49, 44, 138, 253, 14, 241, 53, 10, 67, 32, 156, 111, 132, 245, 221, 113, 122, 52, 165, 37, 198, 57, 208, 68, 167, 109, 234, 74, 130, 169, 0, 141, 120, 64, 57, 165, 126, 209, 16, 2, 76, 160, 51, 9, 192, 171, 251, 91, 180, 16, 55, 253, 188, 196, 247, 26, 176, 94, 159, 127, 20, 21, 94, 26, 18, 154, 167, 83, 17, 154, 58, 5, 20, 132, 64, 54, 51, 110, 110, 113, 90, 114, 154, 46, 196, 148, 220, 252, 5, 123, 145, 184, 32, 246, 90, 144, 65, 252, 115, 41, 32, 7, 197, 80, 8, 157, 185, 88, 161, 71, 243, 179, 90, 225, 160, 166, 71, 110, 194, 135, 128, 168, 132, 46, 28, 65, 56, 232, 187, 41, 186, 185, 210, 12, 18, 28, 69, 163, 113, 154, 126, 207, 154, 96, 108, 153, 110, 239, 97, 248, 33, 128, 204, 207, 228, 42, 157, 75, 0, 140, 113, 168, 206, 148, 196, 140, 233, 177, 9, 165, 52, 235, 81, 151, 79, 213, 217, 67, 26, 207, 40, 217, 36, 144, 74, 41, 38, 104, 110, 9, 115, 160, 173, 77, 98, 196, 188, 28, 66, 96, 5, 16, 125, 239, 203, 121, 116, 198, 98, 187, 82, 188, 6, 87, 171, 139, 149, 213, 151, 92, 174, 45, 154, 158, 42, 111, 42, 249, 157, 242, 30, 166, 148, 57, 144, 13, 211, 66, 2, 121, 78, 79, 79, 148, 179, 77, 8, 69, 141, 78, 97, 20, 207, 13, 15, 131, 27, 64, 202, 89, 9, 229, 22, 204, 161, 84, 121, 18, 203, 126, 37, 197, 108, 248, 171, 91, 226, 212, 175, 91, 234, 198, 52, 238, 89, 204, 214, 198, 133, 6, 124, 240, 49, 127, 26, 250, 64, 150, 42, 66, 224, 130, 85, 5, 92, 46, 181, 198, 106, 35, 117, 96, 102, 123, 253, 27, 77, 199, 89, 48, 153, 86, 249, 8, 67, 195, 210, 108, 116, 44, 179, 91, 177, 206, 197, 85, 218, 181, 212, 157, 27, 77, 77, 117, 153, 181, 77, 77, 85, 166, 212, 227, 125, 117, 50, 222, 140, 180, 227, 221, 244, 44, 114, 190, 149, 55, 87, 143, 93, 106, 44, 10, 49, 193, 6, 164, 20, 231, 180, 98, 234, 58, 59, 127, 235, 210, 191, 101, 126, 197, 144, 242, 98, 219, 45, 143, 164, 107, 7, 210, 85, 214, 170, 71, 113, 104, 24, 101, 83, 208, 39, 128, 48, 215, 149, 11, 92, 17, 136, 94, 203, 50, 82, 6, 233, 115, 201, 118, 232, 65, 151, 78, 25, 204, 117, 147, 170, 62, 101, 187, 160, 99, 186, 164, 21, 194, 116, 238, 152, 141, 166, 228, 138, 62, 243, 22, 221, 192, 254, 70, 216, 12, 230, 238, 180, 58, 236, 22, 1, 94, 66, 155, 211, 142, 147, 216, 206, 169, 27, 97, 134, 140, 140, 235, 169, 44, 179, 215, 110, 117, 218, 235, 77, 219, 95 };

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
#endif

                var ret = 0;

                //Console.WriteLine(args.ToString());
                //MessageBox.Show(args.ToString(true));

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
                        else if (temp == "CC"||temp == "contentCompression")
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
                        else if (temp == "BTFW" || temp == "bmpToFileWati")
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
                                Item.WriteColorLine("压缩模式输入错误!\n已经选择默认压缩模式",ConsoleColor.Red);
                            }
                        }
                        //分块大小
                        else if (temp == "S"||temp =="size")
                        {
                            size = Item.LimitBytes(Item.GetBytes(args[i + 1]));
#if DEBUG
                            Console.WriteLine(string.Format("SIZE:{0}",size));
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

                #region 检查缺少的参数
                //如果缺少工作模式
                while (project == Project.Project.NoInput)
                {
                    Item.WriteColorLine("工作模式错误!\n需要重新输入!\n",ConsoleColor.Yellow);
                    temp = Interaction.InputBox("请输入工作模式!", "提示", "BTF/FTB");
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
                            ret = 401;
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

