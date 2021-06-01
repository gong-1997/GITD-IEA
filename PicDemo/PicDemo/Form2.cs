using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace PicDemo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
           
        }
        private string pathname = string.Empty;
        private string imgname = string.Empty;

        private void 选择文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            MessageBox.Show(file.ToString());
            if (file.FileName != string.Empty)
            {
                try
                {
                    pathname = file.FileName;   //获得文件的绝对路径
                    var files = Directory.GetFiles("G:\\图纸", "*.jpg"); 

                    
                    foreach (var f in files) {
                        
                        DataGridViewRow row = new DataGridViewRow();
                        int index = dataGridView1.Rows.Add(row);

                        dataGridView1.Rows[index].Cells[0].Value = f;

                        
                    }
                    //UploadImage(pathname);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }  
        }

        private void 选择图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            if (file.FileName != string.Empty)
            {
                try
                {
                    pathname = file.FileName;   //获得文件的绝对路径
                    this.pictureBox1.Load(pathname);
                    //UploadImage(pathname);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }  
        }

        public class IMage
        {
            public string index;

        }
        public class Index
        {
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string text { get; set; }
            /// <summary>
            /// 
            /// </summary>

            /// </summary>
            public string data { get; set; }
        }


        public void Post()
        {
            //地址
            string _url = "http://39.98.47.98:8004/application/img/ruku";
            //json参数

            IMage im = new IMage();
            im.index = pathname;
            string imfo = JsonConvert.SerializeObject(im).ToString();//反序列化json数据
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            byte[] byteData = Encoding.UTF8.GetBytes(imfo);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();
            writer.Write(byteData, 0, length);
            writer.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                MessageBox.Show(responseString.ToString());
            }
            catch (Exception ex1)
            {

                MessageBox.Show("处理完成!");
            }
            

           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("操作完成!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Post();
            MessageBox.Show("操作完成!");
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(dataGridView1.Rows[e.RowIndex].Cells["address"].Value.ToString());
            string pathname = dataGridView1.Rows[e.RowIndex].Cells["address"].Value.ToString();
            imgname = dataGridView1.Rows[e.RowIndex].Cells["address"].Value.ToString();
            this.pictureBox1.Load(pathname);

        }
    }
}
