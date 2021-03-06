using System;
using System.Diagnostics;
using System.Windows.Forms;
using FileToImage.Project;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.IO;

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
—　　　　　　　　文件和图片互转工具　　　　　　　　—
—　　　　　　　　　　　　　　　　　　　　　　　　　—
—　　　　　　　　　　　　　　　　　作者：绘梦璃　　—  
—————————————————————————

-? -H -help 获取帮助
-FTB -fileToBmp 文件转为图片
-BTF -bmpToFile 图片转为文件
-IP -inputPath -inputpath 输入文件路径
-OP -outPath -outpath 输出文件路径(默认原路径)
-NK -needKey -needkey 需要密码(不输入:无)
-KM -keyMode -keymode 加密模式(默认:无)
-KV -keyValue -keyvalue 密码内容
-P -password 密码(等同于-NK -KM SHA256 -KV)
-CM -compressMode -compressmode 压缩模式(默认:无)

—————————————————————————

使用范例[方括号中为你应输入的内容](括号内为解释):
[exeName].exe -BTF -IP [fileName] -P [password]
将[fileName]以[password]加密为一张图片并保存

[exeName].exe -FTB -IP [fileName] -P [password]
将[fileName]以[password]解码为源文件并保存

-KM 加密模式:
-KM No(没有加密)/SHA256(sha256加密)

-CM 压缩模式(压缩不一定会让文件更小):
-CM No(没有压缩)/CLZF(CLZF压缩模式)

—————————————————————————
";
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
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
            else
            {
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
                            MessageBox.Show(message, "帮助", MessageBoxButtons.OK);
                            return;
                        }
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
                                return;
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
                                return;
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
                            compressMode = CompressMode.NoCompress.Pause(args[i + 1]);
                        }
                    }
                }

                #region 检查缺少的参数
                //如果缺少工作模式
                while (project == Project.Project.NoInput)
                {
                    MessageBox.Show("工作模式错误!\n需要重新输入!", "错误", MessageBoxButtons.OK);
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

                var ret = 0;

                //运行功能
                switch (project)
                {
                    case Project.Project.NoInput:
                        MessageBox.Show("没有选择工具模式!", "错误", MessageBoxButtons.OK);
                        return;
                    case Project.Project.FileToBmp:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode);
                        }
                        else
                        {
                            ret = Item.FileToBmp(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath);
                        }
                        return;
                    case Project.Project.BmpToFile:
                        if (string.IsNullOrEmpty(outpath))
                        {
                            ret = Item.BmpToFile(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode);
                        }
                        else
                        {
                            ret = Item.BmpToFile(inputpath, needkey, keyMode.GetValue(), keyValue, compressMode, outpath);
                        }
                        return;
                    default:
                        MessageBox.Show("没有这个模式!", "错误", MessageBoxButtons.OK);
                        return;
                }

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

        static void My_CurrentDomain_UnhandledException(object sender,UnhandledExceptionEventArgs e)
        {

        }
    }
}

