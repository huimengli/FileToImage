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
            var ret = Item.FileToBmpShow(file, checkBox1, comboBox1, textBox1, compressMode,pictureBox1,label1);
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
            var ret = Item.FileToBmp(file, checkBox1, comboBox1, textBox1, compressMode);
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
            if (string.IsNullOrEmpty(img))
            {
                MessageBox.Show("没有选择图片!", "错误", MessageBoxButtons.OK);
                return;
            }
            Loading("图片解码中...");
            var ret = Item.BmpToFile(img, checkBox1, comboBox1, textBox1, compressMode);
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
