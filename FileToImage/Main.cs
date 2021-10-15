using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using FileToImage.Project;

namespace FileToImage
{
    public partial class Main : Form
    {
        /// <summary>
        /// 文件对象
        /// </summary>
        public string file;

        /// <summary>
        /// 图片对象
        /// </summary>
        public string img;

        /// <summary>
        /// 压缩模式
        /// </summary>
        public CompressMode compressMode = CompressMode.NoCompress;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "无")
            {
                textBox1.ReadOnly = true;
            }
            else
            {
                textBox1.ReadOnly = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            file = Item.GetFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            img = Item.GetFile("image");
        }

        private void button4_Click_base(object sender, EventArgs e)
        {
            if (file == null)
            {
                MessageBox.Show("没有选择文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            var count = file.Length;
            //Console.WriteLine(count);
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            //Console.WriteLine(side);
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            using (var stream = new FileInfo(file).OpenRead())
            {
                //指针
                var ptr = data.Scan0;
                var i = 0;
                var temp = "";
                var num = 0;//已经读取长度
                var left = stream.Length;
                var maxLength = 1002;//每次读取的最大长度
                var allValue = new StringBuilder();

                byte[] rgbValues = new byte[side * side * 4];

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
                        temp = Encoding.UTF8.GetString(buffer.Take((int)left).ToArray());
                    }
                    else
                    {
                        temp = Encoding.UTF8.GetString(buffer);
                    }
                    left -= num;
                    temp = Base64.Encode(temp, key).Replace("=", "");
                    allValue.Append(temp);
                }
                temp = allValue.ToString();
                while (temp.Length % 4 != 0)
                {
                    temp += "=";
                }
                //Console.WriteLine(temp.Length);
                Item.Base64ToBitmapData(ref data, temp);
                bmp.UnlockBits(data);
                pictureBox1.Image = bmp;
                label1.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                MessageBox.Show("没有选择文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            Loading("文件加密中...");
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var check = "";
            var temp2 = new Dictionary<string, string>();
            if (compressMode == CompressMode.NoCompress)
            {
                temp = Item.ReadFile(new FileInfo(file));
                check = temp;
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
                    {"MD5",Item.MD5(Item.StringToByte(check)) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };
            }
            else if (compressMode == CompressMode.CLZF)
            {
                var tempByte = Item.ReadFile(file);
                tempByte = Item.Compress(tempByte);
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
            }
            else
            {
                throw new Exception("没有这种压缩模式!");
            }
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
            CloseLoading();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                MessageBox.Show("没有选择文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            Loading("文件加密中...");
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = "";
            var check = "";
            var temp2 = new Dictionary<string, string>();
            if (compressMode == CompressMode.NoCompress)
            {
                temp = Item.ReadFile(new FileInfo(file));
                check = temp;
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
                    {"MD5",Item.MD5(Item.StringToByte(check)) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };
            }
            else if (compressMode == CompressMode.CLZF)
            {
                var tempByte = Item.ReadFile(file);
                tempByte = Item.Compress(tempByte);
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
            }
            else
            {
                throw new Exception("没有这种压缩模式!");
            }
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
            Item.OpenOnWindows(new FileInfo(file).DirectoryName);
            bmp.Dispose();
            CloseLoading();
            GC.Collect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(img))
            {
                MessageBox.Show("没有选择图片!", "错误", MessageBoxButtons.OK);
            }
            else
            {
                var image = Image.FromFile(img);
                pictureBox1.Image = image;
                label1.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Loading("图片解码中...");
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
                return;
            }

            if (values["compress"]!=compressMode.ToString())
            {
                MessageBox.Show("选择压缩方式错误!\n"+"文件是用 "+CompressMode.NoCompress.Pause(values["compress"]).GetValue()+" 方法压缩的!","错误",MessageBoxButtons.OK);
                CloseLoading();
                GC.Collect();
                return;
            }

            var value = Base64.Decode(values["data"], key);
            try
            {
                var nowFile = new FileInfo(Base64.Decode(values["fileName"]));
                var nowImg = new FileInfo(img);
                if (File.Exists(nowImg.DirectoryName + "\\" + nowFile.Name))
                {
                    MessageBox.Show("文件: " + Base64.Decode(values["fileName"]) + " 已经存在!", "错误", MessageBoxButtons.OK);
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
                        return;
                    }

                    if (compressMode == CompressMode.NoCompress)
                    {
                        
                    }
                    else if (compressMode == CompressMode.CLZF)
                    {
                        downValue = Item.Decompress(downValue);
                    }
                    else
                    {
                        throw new Exception("没有这种编码方式!");
                    }
                    var downFile = File.Create(nowImg.DirectoryName + "\\" + nowFile.Name);
                    downFile.Write(downValue, 0, downValue.Length);
                    downValue = null;//尝试释放内存
                    MessageBox.Show("解码完成!", "通知", MessageBoxButtons.OK);
                    Item.OpenOnWindows(new FileInfo(img).DirectoryName);
                    downFile.Dispose();
                    downFile.Close();
                }

            }
            catch (InvalidCastException err)
            {
                MessageBox.Show("密码错误!" + err.Message, "错误", MessageBoxButtons.OK);
            }

            bmp.Dispose();
            //bmp.Clone();
            data = null;
            temp = null;
            //temp2 = null;
            CloseLoading();
            GC.Collect();
        }

        public void Loading(string title,string value)
        {
            Text = title + "-" + value;
            Enabled = false;
        }

        public void Loading(string value)
        {
            Text = "文件与图片互转-" + value;
            Enabled = false;
        }

        public void CloseLoading()
        {
            Text = "文件与图片互转";
            Enabled = true;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            var RB = (RadioButton)sender;
            if (RB.Checked)
            {
                compressMode = CompressMode.NoCompress.Pause(RB.Text);
            }
        }
    }
}
