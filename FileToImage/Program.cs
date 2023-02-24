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
        private static string message =
@"
—————————————————————————
—　　　　　　　文件和图片互转工具　　　　　　　—
—　　　　　　　　　　　　　　　　　　　　　　　—
—　　　　　　　　　　　　　　　作者：绘梦璃　　—  
—————————————————————————

-? -H -help 获取帮助
-FTB -fileToBmp 文件转为图片
-BTF -bmpToFile 图片转为文件
-IP -inputPath -inputpath 输入文件路径
-OP -outPath -outpath 输出文件路径(默认原路径)
　　　　　　　(使用此参数时,不会调用explorer)
-NK -needKey -needkey 需要密码(不输入:无)
-KM -keyMode -keymode 加密模式(默认:无)
-KV -keyValue -keyvalue 密码内容
-P -password 密码(等同于-NK -KM SHA256 -KV)
-CM -compressMode -compressmode 压缩模式(默认:无)
-S -size 启用分块,用于减少转化大文件时内存占用

—————————————————————————

使用范例[方括号中为你应输入的内容](括号内为解释):
[exeName].exe -BTF -IP [fileName] -P [password]
将[fileName]以[password]加密为一张图片并保存

[exeName].exe -FTB -IP [fileName] -P [password]
将[fileName]以[password]解码为源文件并保存

-KM 加密模式:
-KM No(没有加密)/SHA256(sha256加密)

-CM 压缩模式(压缩不一定会让文件更小):
-CM No(没有压缩),CLZF/Deflate/ZIP(压缩模式)

—————————————————————————
";

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
                            Console.WriteLine(message);
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
                            var tempByte = new byte[100000];
                            var t1 = DateTime.Now;
                            tempByte.Fill((byte)12);
                            var t2 = DateTime.Now;
                            Console.WriteLine(t2.ToFileTime() - t1.ToFileTime());
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
                            size = int.Parse(args[i + 1]);
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

