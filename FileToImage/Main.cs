using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public FileInfo file;

        /// <summary>
        /// 图片对象
        /// </summary>
        public FileInfo img;

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
            if (comboBox1.Text=="无")
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (file==null)
            {
                MessageBox.Show( "没有选择文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            var count = file.Length;
            var side = (int)Math.Ceiling(Math.Sqrt(count));
            var bmp = new Bitmap(side, side);
            var data = bmp.LockBits(new Rectangle(0, 0, side, side),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            var key = checkBox1.Checked == false ? Base64._keyStr :
                comboBox1.Text == "无" ? Base64._keyStr : 
                comboBox1.Text == "SHA256" ? Base64.GetKey(textBox1.Text, CodingMode.SHA256) :
                Base64._keyStr;
            
            using (var stream = file.OpenRead())
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
                    left -= num;
                    temp = Encoding.UTF8.GetString(buffer);
                    temp = Base64.Encode(temp, key).Replace("=","");
                    allValue.Append(temp);
                }
                temp = allValue.ToString();
                while (temp.Length%4!=0)
                {
                    temp += "=";
                }
                Item.Base64ToBitmapData(ref data, temp);
                bmp.UnlockBits(data);
                pictureBox1.Image = bmp;
                label1.Visible = false;
            }
        }
    }
}
