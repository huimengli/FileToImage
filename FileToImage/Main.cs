using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

                var test = new StringBuilder();

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
                    test.Append(temp);
                    temp = Base64.Encode(temp, key).Replace("=", "");
                    allValue.Append(temp);
                }
                temp = allValue.ToString();
                while (temp.Length % 4 != 0)
                {
                    temp += "=";
                }
                Console.WriteLine(temp.Length);
                Console.WriteLine(test.ToString().Length);
                Console.WriteLine(test.ToString());
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
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = Item.ReadFile(new FileInfo(file));
            var check = temp;
            temp = Base64.Encode(temp, key);
            while (temp.Length % 4 != 0)
            {
                temp += "=";
            }
            var temp2 = new Dictionary<string, string>()
                {
                    {"data",temp },
                    {"fileName",Base64.Encode(file) },
                    {"size",temp.Length.ToString() },
                    {"code",coding.ToString() },
                    {"MD5",Item.MD5(Item.StringToByte(check)) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };
            temp = temp2.ToString(true);
            count = temp.Length / 3;
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

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                MessageBox.Show("没有选择文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            var count = file.Length;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;

            var temp = Item.ReadFile(new FileInfo(file));
            var check = temp;
            temp = Base64.Encode(temp, key);
            while (temp.Length % 4 != 0)
            {
                temp += "=";
            }
            var temp2 = new Dictionary<string, string>()
                {
                    {"data",temp },
                    {"fileName",Base64.Encode(file) },
                    {"size",temp.Length.ToString() },
                    {"code",coding.ToString() },
                    {"MD5",Item.MD5(Item.StringToByte(check)) },
                    {"end","0" }//由于保存图片会有没有任何数据的结果,所以在字典上添加一个结尾标记
                };
            temp = temp2.ToString(true);
            count = temp.Length / 3;
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
            //pictureBox1.Image = bmp;
            //label1.Visible = false;
            Item.BmpToJpgSave(bmp, file + ".EN.jpg");
            MessageBox.Show("文件加密完成!", "通知", MessageBoxButtons.OK);
            Item.OpenOnWindows(new FileInfo(file).DirectoryName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var image = Image.FromFile(img);
            pictureBox1.Image = image;
            label1.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
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
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr :
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;
            var coding = checkBox1.Checked ? CodingMode.NoCoding.Pause(comboBox1.Text) : CodingMode.NoCoding;
            var values = temp2.ToString().ToDict();
            if (values["code"] == coding.ToString())
            {
                var value = Base64.Decode(values["data"], key);
                if (Item.MD5(Item.StringToByte(value)) != values["MD5"])
                {
                    Console.Write(values.ToString(true));
                    MessageBox.Show("密码错误!", "错误!", MessageBoxButtons.OK);
                }
                else
                {
                    if (File.Exists(Base64.Decode(values["fileName"])))
                    {
                        MessageBox.Show("文件: " + Base64.Decode(values["fileName"]) + " 已经存在!", "错误!", MessageBoxButtons.OK);
                    }
                    else
                    {
                        using (var downFile = File.Create(Base64.Decode(values["fileName"])))
                        {
                            var downValue = Item.StringToByte(value);
                            downFile.Write(downValue, 0, downValue.Length);
                            MessageBox.Show("解码完成!", "通知", MessageBoxButtons.OK);
                            Item.OpenOnWindows(new FileInfo(img).DirectoryName);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("选择编码方式错误!\n" + "文件是用 " + CodingMode.NoCoding.Pause(values["code"]).GetValue() + " 方式编码的!", "错误!", MessageBoxButtons.OK);
            }
        }
    }
}
