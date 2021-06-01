using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using OpenCvSharp;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace PicDemo 
{
    public partial class MainForm : Form
    {
       
        private string _curOperateType = "";
        private bool _isMouseDown = false;
        private Point _mousePosA = new Point(0, 0);
        private Point _mousePosB = new Point(0, 0);
        private Point _mousePosC = new Point(0, 0);
        private Point _mousePosD = new Point(0, 0);
        private Point _mousePosE = new Point(0, 0);
        private Point _mousePosF = new Point(0, 0);
        private Point _mousePosG = new Point(0, 0);

        private Cursor _cursorPan, _cursorZoomIn, _cursorZoomOut, _cursorCenter, _cursor;
        private float _imgAspectRatio;
        private List<Geometry> _listGeometry = new List<Geometry>();
        private Image _imgSymbol;

        private List<Point> _listAddPoint = new List<Point>();
        private bool _isPolyBegin = false;
        private List<GeometryPoly> _listGeometryPoly = new List<GeometryPoly>();

        float multiple = 1;

        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _cursorPan = new Cursor(this.GetType(), "MapInfoWebPan.cur");
            _cursorZoomIn = new Cursor(this.GetType(), "MapInfoWebZoomIn.cur");
            _cursorZoomOut = new Cursor(this.GetType(), "MapInfoWebZoomOut.cur");
            _cursorCenter = new Cursor(this.GetType(), "MapInfoWebCenter.cur");
            //_cursor = new Cursor(this.GetType(),"\\3.cur");
            _imgSymbol = Image.FromFile(Application.StartupPath + "\\thm_grad.PNG");
            //int x1 = 100;
            //int x2 = 30;

            //float y = (float)(x1 * 1.0 / x2);
            //MessageBox.Show((256<<2).ToString());
        }


        bool color = false;
        private void DrawGeometry(Graphics graphics)
        {
            int x1, y1, x2, y2;
            
                foreach (Geometry g in _listGeometry)
                {
                    if (imgPictureBox.Image == null)
                    { x1 = 0; x2 = 0; y1 = 0; y2 = 0; g.geometryType = 0; }
                    else
                    {
                        x1 = g.x1 * imgPictureBox.Width / imgPictureBox.Image.Width;
                        y1 = g.y1 * imgPictureBox.Height / imgPictureBox.Image.Height;
                        x2 = g.x2 * imgPictureBox.Width / imgPictureBox.Image.Width;
                        y2 = g.y2 * imgPictureBox.Height / imgPictureBox.Image.Height;
                    }
                    
                    switch (g.geometryType)
                    {
                        case GeometryType.Symbol:
                            graphics.DrawImage(_imgSymbol, x1, y1);
                            break;
                        case GeometryType.Line:
                            graphics.DrawLine(Pens.Yellow, x1, y1, x2, y2);
                            break;
                        case GeometryType.Rectangle:

                            //graphics.DrawRectangle(Pens.Red, Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));


                            break;
                        case GeometryType.Rectangle1:
                            //graphics.DrawRectangle(Pens.Red, Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                            break;
                        case GeometryType.Ellipse:
                            graphics.DrawEllipse(Pens.Yellow, x1, y1, x2 - x1, y2 - y1);
                            break;
                        case GeometryType.Text:
                            graphics.DrawString(g.geometryInfo, new Font("宋体", 10), Brushes.White, x1, y1);
                            break;
                    }
                }
                foreach (GeometryPoly g in _listGeometryPoly)
                {
                    Point[] p = (Point[])g.points.Clone();
                    for (int k = 0; k < p.Length; k++)
                    {
                        p[k].X = p[k].X * imgPictureBox.Width / imgPictureBox.Image.Width;
                        p[k].Y = p[k].Y * imgPictureBox.Height / imgPictureBox.Image.Height;
                    }
                    if (g.geometryType == GeometryType.Polyline)
                        graphics.DrawLines(Pens.Yellow, p);
                }
           

            
            
            
         }

       
        private void ZoomWork(double zoom, MouseEventArgs e)
        {
            int x0, y0, x1, y1, x2, y2, x3, y3;

            

            //(x1,y1)鼠标点在原PictureBxo中的坐标
            x1 = e.Location.X;
            y1 = e.Location.Y;
            //zoom后，鼠标点也产生偏移，(x2,y2)为鼠标点在zoom后PictureBxo中的坐标
            x2 = (int)(e.Location.X * zoom);
            y2 = (int)(e.Location.Y * zoom);
            
            //(x0,y0)偏移后的鼠标点在imgPanel中的坐标
            x0 = x2 + imgPictureBox.Left - (int)((zoom - 1) * imgPictureBox.Width / 2);
            y0 = y2 + imgPictureBox.Top - (int)((zoom - 1) * imgPictureBox.Height / 2);
            _mousePosF.X = x0;
            _mousePosF.Y = y0;


            //(x3,y3)为zoom后未平移前，PictureBox的左上角在imgPanel中的坐标
            x3 = imgPictureBox.Left - (int)((zoom - 1) * imgPictureBox.Width / 2);
            y3 = imgPictureBox.Top - (int)((zoom - 1) * imgPictureBox.Height / 2);
            _mousePosG.X = x3;
            _mousePosG.Y = y3;

            imgPictureBox.Width = (int)(imgPictureBox.Width * zoom);
            imgPictureBox.Height = (int)(imgPictureBox.Height * zoom);
            
            //将偏移后的鼠标点，平移到imgPanel中心
            imgPictureBox.Left = x3 + (int)(imgPanel.Width / 2 - x0);
            imgPictureBox.Top = y3 + (int)(imgPanel.Height / 2 - y0);


            imgPictureBox.Refresh();
            eyePictureBox.Invalidate();
        }

        private void ImageInitCenter()
        {
            imgPictureBox.Size = imgPictureBox.Image.Size;//设定图片初始尺
            int x = (int)Math.Ceiling((imgPanel.Width - imgPictureBox.Width) * 0.5);
            int y = (int)Math.Ceiling((imgPanel.Height - imgPictureBox.Height) * 0.5);
            imgPictureBox.Location = new Point(x, y);//设定图片位置

            eyePictureBox.Invalidate();
        }
       

       
        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null || ((string)e.ClickedItem.Tag).Trim().Length == 0) return;
            _curOperateType = (string)e.ClickedItem.Tag;

            ToolStrip ts = sender as ToolStrip;
            for (int k = 0; k < ts.Items.Count; k++)
            {
                if (!(ts.Items[k] is ToolStripButton)) continue;
                (ts.Items[k] as ToolStripButton).Checked = e.ClickedItem == ts.Items[k] ? true : false;
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openImgDlg.Filter = "图片文件|*.jpg;*.bmp;*.png";
            if (openImgDlg.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openImgDlg.FileName);
                imgPictureBox.Image = img;
                ImageInitCenter();
                _imgAspectRatio = imgPictureBox.Image.Height * 1000 / imgPictureBox.Image.Width;

                
                //eyePictureBox.Image = img;
                //eyePictureBox.Image = Thumbnail.GenThumbnail(img, eyePictureBox.Width);
                eyePictureBox.Image = Thumbnail.GenThumbnail(img, eyePanel.Width-2);
                eyePictureBox.Size = eyePictureBox.Image.Size;
                eyePictureBox.Left = (eyePanel.Width - eyePictureBox.Width - 2) / 2;
                eyePictureBox.Top = (eyePanel.Height - eyePictureBox.Height - 2) / 2;
            }
        }
       

       
        private void imgPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                eraser = true;
            }
            
            Geometry g = null;

           
            _isMouseDown = true;
            if (e.Button == MouseButtons.Left)
            {
                switch (_curOperateType)
                {
                    case "Pan":
                        _mousePosA = e.Location;
                        break;
                    case "ZoomIn":
                        ZoomWork(1.1, e);
                        multiple=multiple + 0.1f;
                        break;
                    case "ZoomOut":
                        ZoomWork(0.9, e);
                        multiple = multiple - 0.1f;
                        break;
                    case "Center":
                        ZoomWork(1, e);
                        break;
                    case "Symbol":
                        imgPictureBox.CreateGraphics().DrawImage(_imgSymbol, e.Location);
                        g = new Geometry();
                        g.x1 = e.Location.X * imgPictureBox.Image.Width / imgPictureBox.Width;
                        g.y1 = e.Location.Y * imgPictureBox.Image.Height / imgPictureBox.Height;
                        g.geometryType = GeometryType.Symbol;
                        _listGeometry.Add(g);
                        break;
                    case "Line":
                    case "Rectangle":
                        _mousePosA = e.Location;
                        _mousePosB = e.Location;
                        
                        break;
                   
                    case "Ellipse":
                        _mousePosA = e.Location;
                        _mousePosB = e.Location;
                        break;
                    case "Rectangle1":
                        _mousePosD = e.Location;
                        _mousePosE = e.Location;
                        break;
                    case "Polyline":
                        if (!_isPolyBegin)
                        {
                            _isPolyBegin = true;
                            _listAddPoint.Clear();
                            _mousePosB.X = 0;
                            _mousePosB.Y = 0;
                            _mousePosA = _mousePosB;
                        }
                        (sender as PictureBox).CreateGraphics().DrawLine(Pens.Yellow, _mousePosA, _mousePosB);

                        _listAddPoint.Add(e.Location);
                        _mousePosA = e.Location;
                        _mousePosB = e.Location;

                        if (e.Clicks > 1)
                        {
                            _isPolyBegin = false;
                            GeometryPoly gp = new GeometryPoly();
                            gp.geometryType = GeometryType.Polyline;
                            gp.points = _listAddPoint.ToArray();
                            for (int k = 0; k < gp.points.Length; k++)
                            {
                                gp.points[k].X = gp.points[k].X * imgPictureBox.Image.Width / imgPictureBox.Width;
                                gp.points[k].Y = gp.points[k].Y * imgPictureBox.Image.Height / imgPictureBox.Height;
                            }
                            _listGeometryPoly.Add(gp);
                        }
                        break;
                    case "Polygon":
                        if (!_isPolyBegin)
                        {
                            _listAddPoint.Clear();
                            _mousePosB.X = 0;
                            _mousePosB.Y = 0;
                            _mousePosA = _mousePosB;
                            _mousePosC = _mousePosB;
                        }
                        
                        //(sender as PictureBox).CreateGraphics().DrawLine(Pens.Yellow, _mousePosA, _mousePosB);
                        (sender as PictureBox).CreateGraphics().DrawLine(Pens.Yellow, _mousePosC, _mousePosB);
                        if (!_isPolyBegin)
                        {
                            _mousePosA = e.Location;
                            _isPolyBegin = true;
                        }

                        _listAddPoint.Add(e.Location);
                        _mousePosC = e.Location;
                        _mousePosB = e.Location;

                        if (e.Clicks > 1)
                        {
                            _isPolyBegin = false;
                            (sender as PictureBox).CreateGraphics().DrawLine(Pens.Yellow, _mousePosA, _mousePosB);
                        }
                        break;
                    case "Text":
                        //TextEditForm frm = new TextEditForm();
                        //if (frm.ShowDialog() != DialogResult.OK) return;
                        //g = new Geometry();
                       // g.x1 = e.Location.X * imgPictureBox.Image.Width / imgPictureBox.Width;
                       // g.y1 = e.Location.Y * imgPictureBox.Image.Height / imgPictureBox.Height;
                        //g.geometryInfo = frm.GeometryInfo;
                        //g.geometryType = GeometryType.Text;
                        //_listGeometry.Add(g);
                        //imgPictureBox.CreateGraphics().DrawString(frm.GeometryInfo, new Font("宋体",20) , Brushes.Red, e.Location);
                        break;
                }
            }

            if (mark == true)
            {
                List<Point> list0 = new List<Point>();
                if (e.Button == MouseButtons.Left)
                {
                    Graphics g1 = ((PictureBox)sender).CreateGraphics();
                    g1.FillEllipse(Brushes.Red, e.X, e.Y, 5, 5);
                    //list0.Add(new Point(e.X,e.Y));
                   
                    
                }
               
            }
           
        
        }

        bool eraser = false;
        int check = 0;
        private void imgPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            
            double x = e.X + 256 * 26768;
            double y = e.Y + 256 * 14276;
            x = GoogleCoordinate.pixelToLng(x, 15);
            y = GoogleCoordinate.pixelToLat(y, 15);
            lblXY.Text = e.X.ToString() + "," + e.Y.ToString();
            //textBox1.Text = multiple.ToString();

            Bitmap im;              // 暂时存储后台图片
            im = (Bitmap)imgPictureBox.Image;
            Graphics G = imgPictureBox.CreateGraphics();

           
            if (eraser == true && check == 4)
            {
                G = Graphics.FromImage(im);
                G.FillEllipse(new SolidBrush(Color.White), e.X, e.Y, 5, 5);
                imgPictureBox.Image = im;
            }
            if (eraser == true && check == 6)
            {
                G = Graphics.FromImage(im);
                G.FillEllipse(new SolidBrush(Color.White), e.X, e.Y, 10, 10);
                imgPictureBox.Image = im;
            }
            if (eraser == true && check == 8)
            {
                G = Graphics.FromImage(im);
                G.FillEllipse(new SolidBrush(Color.White), e.X, e.Y, 15, 15);
                imgPictureBox.Image = im;
            }
            
           
            switch(_curOperateType)
            {
                case "Pan":
                    if (!_isMouseDown || !(e.Button == MouseButtons.Left)) return;
                    //if (Math.Max(Math.Abs(e.X - _mousePosA.X), Math.Abs(e.Y - _mousePosA.Y)) < 5) return;
                    (sender as PictureBox).Left += e.Location.X - _mousePosA.X;
                    (sender as PictureBox).Top += e.Location.Y - _mousePosA.Y;
                    break;
                case "Line":
                    if (!_isMouseDown || !(e.Button == MouseButtons.Left)) return;
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA),(sender as PictureBox).PointToScreen( _mousePosB), Color.Yellow);
                    _mousePosB = e.Location;
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Yellow);
                    break;
                case "Rectangle":
                    if (!_isMouseDown || !(e.Button == MouseButtons.Left)) return;
                   
                    //ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                    //    new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y)), Color.Red, FrameStyle.Thick);
                    
                    _mousePosB = e.Location;
                    //ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                    //    new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y)), Color.Red, FrameStyle.Thick);
                    
                        break;
                case "Rectangle1":
                        if (!_isMouseDown || !(e.Button == MouseButtons.Left)) return;

                        //ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                        //    new Size(_mousePosE.X - _mousePosD.X, _mousePosE.Y - _mousePosD.Y)), Color.Red, FrameStyle.Thick);
                        
                        _mousePosB = e.Location;
                        
                        //ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                        //    new Size(_mousePosE.X - _mousePosD.X, _mousePosE.Y - _mousePosD.Y)), Color.Red, FrameStyle.Thick);

                        break;
                case "Ellipse":
                    if (!_isMouseDown || !(e.Button == MouseButtons.Left)) return;
                    ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                        new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y)), Color.Yellow, FrameStyle.Dashed);
                    _mousePosB = e.Location;
                    ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA),
                        new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y)), Color.Yellow, FrameStyle.Dashed);
                    break;
                case "Polyline":
                    if (!_isPolyBegin) return;
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Yellow);
                    _mousePosB = e.Location;
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Yellow);
                    break;
                case "Polygon":
                    if (!_isPolyBegin) return;
                    if (_mousePosA != _mousePosC)
                        ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Red);
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosC), (sender as PictureBox).PointToScreen(_mousePosB), Color.Red);
                    _mousePosB = e.Location;
                    if (_mousePosA != _mousePosC)
                        ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Red);
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosC), (sender as PictureBox).PointToScreen(_mousePosB), Color.Red);
                    if (_listAddPoint.Count == 2)
                            (sender as PictureBox).CreateGraphics().DrawLine(Pens.Red, _mousePosA, _mousePosC);
                    break;
            }
           
        }

        private void imgPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            eraser = false;
            Size size;
            Geometry g = null;

            if (e.Button == MouseButtons.Left)
            {
                if (_curOperateType == "Ellipse" || _curOperateType == "Rectangle" || _curOperateType == "Line" || _curOperateType == "Rectangle1")
                {
                    try
                    {
                        g = new Geometry();
                        g.x1 = _mousePosA.X * imgPictureBox.Image.Width / imgPictureBox.Width;
                        g.y1 = _mousePosA.Y * imgPictureBox.Image.Height / imgPictureBox.Height;
                        g.x2 = e.Location.X * imgPictureBox.Image.Width / imgPictureBox.Width;
                        g.y2 = e.Location.Y * imgPictureBox.Image.Height / imgPictureBox.Height;
                    }
                    catch (Exception ex2) { MessageBox.Show("请先加载图片!"); }
                }
                if (_curOperateType == "Pan")
                {
                    eyePictureBox.Invalidate();
                }

                if (_curOperateType == "Ellipse")
                {
                    if (!_isMouseDown) return;
                    size = new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y);
                    ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA), size), Color.Yellow, FrameStyle.Dashed);


                    size = new Size(e.Location.X - _mousePosA.X, e.Location.Y - _mousePosA.Y);
                    (sender as PictureBox).CreateGraphics().DrawEllipse(Pens.Yellow, new Rectangle(_mousePosA, size));

                    g.geometryType = GeometryType.Ellipse;
                    _listGeometry.Add(g);
                }
                if (_curOperateType == "Rectangle")
                {
                    
                    if (!_isMouseDown) return;
                    size = new Size(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y);
                    //ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosA), size), Color.Red, FrameStyle.Dashed);
                    (sender as PictureBox).CreateGraphics().DrawRectangle(Pens.Red, new Rectangle(_mousePosA, size));

                    (sender as PictureBox).CreateGraphics().DrawRectangle(Pens.Red,
                        new Rectangle(Math.Min(_mousePosA.X, e.Location.X),
                                      Math.Min(_mousePosA.Y, e.Location.Y),
                                      Math.Abs(_mousePosA.X - e.Location.X),
                                      Math.Abs(_mousePosA.Y - e.Location.Y)));

                    g.geometryType = GeometryType.Rectangle;
                    _listGeometry.Add(g);
                    save_WT();
                    UploadImage("c.bmp");
                   
                   
                    //Form2 fm2 = new Form2();

                    //fm2.Show();
                    //this.imgPictureBox.Refresh();
                }

                if (_curOperateType == "Rectangle1")
                {
                    if (!_isMouseDown) return;
                    size = new Size(_mousePosE.X - _mousePosD.X, _mousePosE.Y - _mousePosD.Y);
                    ControlPaint.DrawReversibleFrame(new Rectangle((sender as PictureBox).PointToScreen(_mousePosD), size), Color.Red, FrameStyle.Dashed);
                    (sender as PictureBox).CreateGraphics().DrawRectangle(Pens.Red, new Rectangle(_mousePosD, size));

                    (sender as PictureBox).CreateGraphics().DrawRectangle(Pens.Red,
                        new Rectangle(Math.Min(_mousePosD.X, e.Location.X),
                                      Math.Min(_mousePosD.Y, e.Location.Y),
                                      Math.Abs(_mousePosD.X - e.Location.X),
                                      Math.Abs(_mousePosD.Y - e.Location.Y)));

                    g.geometryType = GeometryType.Rectangle1;
                    _listGeometry.Add(g);
                    
                }



                if (_curOperateType == "Line")
                {
                    if (!_isMouseDown) return;
                    ControlPaint.DrawReversibleLine((sender as PictureBox).PointToScreen(_mousePosA), (sender as PictureBox).PointToScreen(_mousePosB), Color.Yellow);
                    (sender as PictureBox).CreateGraphics().DrawLine(Pens.Yellow, _mousePosA, e.Location);

                    g.geometryType = GeometryType.Line;
                    _listGeometry.Add(g);
                }
            }
            _isMouseDown = false;
        }


        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>

            /// </summary>
            public List<DataItem> data { get; set; }
        }
        public class DataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string location { get; set; }

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

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>

        public class IMage
        {
            public string index;
           
        }
        public  void Post()
        {
            //地址
            string _url = "http://39.98.47.98:8004/application/img/index";
            //json参数
            
            IMage im = new IMage();
            im.index = textBox2.Text;
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
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
            MessageBox.Show(responseString.ToString());

            try
            {
                Index index = JsonConvert.DeserializeObject<Index>(responseString.ToString());


                Image img = Image.FromFile(index.data);

                this.textBox3.Text = index.data;
                this.textBox1.Text = index.text;

                this.imgPictureBox.Image = img;
            }
            catch (Exception ex1) {

                MessageBox.Show("没有检索到关键字!");
            }
        }

        public void UploadImage(string imgPath)
        {
            var uploadUrl = "http://39.98.47.98:8004/application/img/process";
            HttpWebRequest request = WebRequest.Create(uploadUrl) as HttpWebRequest;
            request.AllowAutoRedirect = true;
            request.Method = "POST";

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            int pos = imgPath.LastIndexOf("\\");
            string fileName = imgPath.Substring(pos + 1);

            //请求头部信息 
            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            byte[] bArr = new byte[fs.Length];
            fs.Read(bArr, 0, bArr.Length);
            fs.Close();

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bArr, 0, bArr.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            string content = sr.ReadToEnd();
            Root root = JsonConvert.DeserializeObject<Root>(content);

            MessageBox.Show(content);



            
            this.dataGridView1.Rows.Clear();
            for(int i=0;i<root.data.Count;i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                int index = dataGridView1.Rows.Add(row);

                dataGridView1.Rows[index].Cells[0].Value = root.data[i].Text;

                dataGridView1.Rows[index].Cells[1].Value = root.data[i].location;
            }
;

            //this.pictureBox2.LoadAsync(root.data[0].img);
            //this.pictureBox3.LoadAsync(root.data[1].img);

            //this.textBox1.Text = root.data[0].scoremap;
            //this.textBox2.Text = root.data[1].scoremap;
        }

        private void imgPictureBox_MouseEnter(object sender, EventArgs e)
        {
            switch (_curOperateType)
            {
                case "Pan":
                    this.Cursor = _cursorPan;
                    break;
                case "ZoomIn":
                    this.Cursor = _cursorZoomIn;
                    break;
                case "ZoomOut":
                    this.Cursor = _cursorZoomOut;
                    break;
                case "Center":
                    this.Cursor = _cursorCenter;
                    break;
                case "Symbol":
                case "Line":
                case "Rectangle":
                case "Rectangle1":
                case "Ellipse":
                case "Polyline":
                case "Polygon":
                    this.Cursor = Cursors.Cross;
                    break;
                case "Text":
                    this.Cursor = Cursors.Hand;
                    break;
                
                default:
                    this.Cursor = Cursors.Default;
                    break;
            }
        }

        private void imgPictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void imgPictureBox_Paint(object sender, PaintEventArgs e)
        {
           
                DrawGeometry(e.Graphics);
           
        }

        //private void imgPictureBox_LocationChanged(object sender, EventArgs e)
        //{
        //    //eyePictureBox.Invalidate();
        //}

      

       
        private void eyePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (eyePictureBox.Image == null)
                return;
            int x1 = -imgPictureBox.Left;
            int y1 = -imgPictureBox.Top;
            //if (x1 < 0) x1 = 0;
            //if (y1 < 0) y1 = 0;

            int x2 = x1 + imgPanel.Width;
            int y2 = y1 + imgPanel.Height;
            if (x2 > imgPictureBox.Width)
                x2 = imgPictureBox.Width;
            if (y2 > imgPictureBox.Height)
                y2 = imgPictureBox.Height;

            float fx1 = eyePictureBox.Width * x1 / imgPictureBox.Width;
            float fx2 = eyePictureBox.Width * x2 / imgPictureBox.Width;
            float fy1 = eyePictureBox.Height * y1 / imgPictureBox.Height;
            float fy2 = eyePictureBox.Height * y2 / imgPictureBox.Height;

            e.Graphics.DrawRectangle(Pens.Red, fx1, fy1, fx2 - fx1, fy2 - fy1);
        }

        private void eyePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            //记录框选坐标的起始位置
            int x1 = (int)(e.Location.X * imgPictureBox.Width / eyePictureBox.Width);
            int y1 = (int)(e.Location.Y * imgPictureBox.Height / eyePictureBox.Height);

            int x0 = x1 + imgPictureBox.Left;
            int y0 = y1 + imgPictureBox.Top;

            imgPictureBox.Left += (int)(imgPanel.Width * 0.5 - x0);
            imgPictureBox.Top += (int)(imgPanel.Height * 0.5 - y0);

            eyePictureBox.Invalidate();
        }
        

        private void btnZoomIn_Click(object sender, EventArgs e)
        {

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {

        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {

        }
        
        ArrayList[] Row = new ArrayList[200];   //存放每一条波形
        ArrayList Line = new ArrayList();        //用于存储起始点坐标
        private unsafe void Draw()
        {

            BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(new Rectangle(0, 0, imgPictureBox.Image.Width, imgPictureBox.Image.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            

            int PixelSize = 3;

            for (int m = 0; m < bmd.Height; m++)
            {
                byte* row = (byte*)bmd.Scan0 + (m * bmd.Stride);
                for (int n = 0; n < bmd.Width; n++)
                {
                    if (row[n * PixelSize] < 100)
                    {
                        row[n * PixelSize] = 0;
                        row[n * PixelSize + 1] = 0;
                        row[n * PixelSize + 2] = 0;
                    }
                    else
                    {
                        row[n * PixelSize] = 255;
                        row[n * PixelSize + 1] = 255;
                        row[n * PixelSize + 2] = 255;


                    }
                }
            }
            for (int m = 0; m < Line.Count; m++)
            {
                for (int e = 0; e < Row[m].Count; e++)
                {
                    if (e == 0)
                    {
                        if (Row[m][e] != null)
                        {
                            for (int jsq = ((Point)Row[m][e]).Y - 2; jsq < ((Point)Row[m][e]).Y + 2; jsq++)
                            {
                                byte* row = (byte*)bmd.Scan0 + (jsq * bmd.Stride);
                                row[((Point)Row[m][e]).X * PixelSize] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 1] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 2] = 255;
                                row[(((Point)Row[m][e]).X + 1) * PixelSize + 2] = 255;
                                row[(((Point)Row[m][e]).X + 2) * PixelSize + 2] = 255;
                                row[(((Point)Row[m][e]).X + 3) * PixelSize + 2] = 255;
                                row[(((Point)Row[m][e]).X + 4) * PixelSize + 2] = 255;
                                row[(((Point)Row[m][e]).X + 5) * PixelSize + 2] = 255;
                            }
                        }
                    }
                    else if (Row[m][e] != null)
                    {
                        if (((Point)Row[m][e]).Y >= 2)
                            for (int jsq = ((Point)Row[m][e]).Y - 2; jsq < ((Point)Row[m][e]).Y + 2; jsq++)
                            {
                                byte* row = (byte*)bmd.Scan0 + (jsq * bmd.Stride);
                                row[((Point)Row[m][e]).X * PixelSize] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 1] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 2] = 255;
                            }
                        else
                        {
                            for (int jsq = ((Point)Row[m][e]).Y; jsq < ((Point)Row[m][e]).Y + 2; jsq++)
                            {
                                byte* row = (byte*)bmd.Scan0 + (jsq * bmd.Stride);
                                row[((Point)Row[m][e]).X * PixelSize] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 1] = 0;
                                row[((Point)Row[m][e]).X * PixelSize + 2] = 255;
                            }
                        }
                    }
                }
            }
            ((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            imgPictureBox.Image = imgPictureBox.Image;
           
        }    
        private void 二值化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Draw();
            MessageBox.Show("二值化完成！");
        }

        int weight, height;
        bool picture =false;
        private void 加载图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.imgPictureBox.Image = null;
            _listGeometry.Clear();
            openImgDlg.Filter = "图片文件|*.jpg;*.bmp;*.png";
            if (openImgDlg.ShowDialog() == DialogResult.OK)
            {
               
                Image img = Image.FromFile(openImgDlg.FileName);
                
                imgPictureBox.Image = img;
                
                ImageInitCenter();
                _imgAspectRatio = imgPictureBox.Image.Height * 1000 / imgPictureBox.Image.Width;


                //eyePictureBox.Image = img;
                //eyePictureBox.Image = Thumbnail.GenThumbnail(img, eyePictureBox.Width);
                eyePictureBox.Image = Thumbnail.GenThumbnail(img, eyePanel.Width - 2);
                eyePictureBox.Size = eyePictureBox.Image.Size;
                eyePictureBox.Left = (eyePanel.Width - eyePictureBox.Width - 2) / 2;
                eyePictureBox.Top = (eyePanel.Height - eyePictureBox.Height - 2) / 2;
            }
            weight = imgPictureBox.Width;
            height = imgPictureBox.Height;
        }


        private System.Drawing.Bitmap m_Bitmap;
        private System.Drawing.Bitmap m_Undo;
        private void 边缘化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Rectangle rect=new Rectangle(_mousePosA.X, _mousePosA.Y, _mousePosB.X-_mousePosA.X, _mousePosB.Y-_mousePosA.Y);
            //m_Bitmap = new Bitmap(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y);
            //imgPictureBox.DrawToBitmap(m_Bitmap,rect);
            m_Bitmap = imgPictureBox.Image as Bitmap;
            Parameter dlg = new Parameter();

            dlg.nValue = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                m_Undo = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.EdgeDetectDifference(m_Bitmap, (byte)dlg.nValue)) ;
                   
            }
            imgPictureBox.Refresh();
            
        }

        private void 小橡皮ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
           
        }


        List<Point> listtop = new List<Point>();
        List<Point> listdown = new List<Point>();
        List<Point> t = new List<Point>();
        List<Point> startpoint=new List<Point>();
        List<Point> listS = new List<Point>();
        List<Point> listF = new List<Point>();
        ArrayList width = new ArrayList();
        private unsafe void initial1()
        {
            FileStream fs = new FileStream("HAN.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            FileStream fs1 = new FileStream("x.txt", FileMode.Create);
            StreamWriter sw1 = new StreamWriter(fs1);
            FileStream fs2 = new FileStream("y.txt", FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2);
            
            Graphics g = imgPictureBox.CreateGraphics();
            //g.FillEllipse(Brushes.Red, e.X, e.Y, 3, 3);
            Point p;
            int y;
            int x = _mousePosA.X + 1;
            int hang = 0;
            int hei = 0;
            int flag = 0;
            int min = 0;
            int max = 0;

            
            for (y = _mousePosA.Y; y < _mousePosB.Y; y++)
            {
                hang = hei;
                hei = 0;
                //if (y < pictureBox1.Image.Height && y >0)
                if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B)
                {
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y).B.ToString());
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y + 1).B.ToString());
                    if (((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B < 255 && flag == 0)
                    {
                        min = y;
                        flag++;

                    }

                    else if (flag == 1)
                    {

                        max = y;
                        //MessageBox.Show(y.ToString());
                        flag = 0;
                        hei = (max + min) / 2;
                        //MessageBox.Show(hei.ToString());
                        //list.Add(new Point(x, hei));
                        g.FillEllipse(Brushes.Red, x, hei, 5, 5);
                        
                        listS.Add(new Point(x,hei));
                        MessageBox.Show("找到平衡位置!");
                        break;
                        //s = new Point(1, hei);
                        //firstavgwidth += (max - min);


                    }

                }

            }
            //MessageBox.Show(list[0].Y.ToString());
            p = new Point(x, hei);
            
            int border = 0;
            while(x<_mousePosB.X)
            {

                for (int y0 = p.Y; y < _mousePosB.Y-10; y++)//向下搜寻
                {

                    if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B)
                    {

                        max = y;

                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B > 0)
                        {
                            break;
                        }



                    }



                }


                for (int y0 = p.Y; y > _mousePosA.Y+10; y--)//向上搜寻
                {

                    if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y - 1).B)
                    {

                        
                        min = y;
                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y - 1).B > 0)
                        {
                            break;
                        }


                    }


                }



                if ((max - p.Y) > (p.Y - min))
                {
                    border = max;

                }
                else { border = min; }

               
                   g.FillEllipse(Brushes.Red, x, p.Y, 5, 5);
                   g.FillEllipse(Brushes.Blue, x, border, 5, 5);
                   if (Math.Abs(border-p.Y) < 80)
                   {
                       listtop.Add(new Point(x, max));
                       listdown.Add(new Point(x, min));
                   }
                   //g.FillEllipse(Brushes.Blue, x, max, 3, 3);
                   //g.FillEllipse(Brushes.Red, x, min, 3, 3);
                   listF.Add(new Point(x,border));  
                   sw.Write(x + "," + border + "\r\n");
                   sw1.Write(x + "\r\n");
                   sw2.Write( border + "\r\n");
                x++;
            
            
               }





            width.Add(listF.Count);

            sw.Flush();
            sw.Close();
            fs.Close();

            sw1.Flush();
            sw1.Close();
            fs1.Close();

            sw2.Flush();
            sw2.Close();
            fs2.Close();
            
            MessageBox.Show("追踪成功!");
        
        
        
        }


        private unsafe void Complex2()
        {
            //Graphics g = imgPictureBox.CreateGraphics();
            ////g.FillEllipse(Brushes.Red, e.X, e.Y, 3, 3);
            //BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //int PixelSize = 3;
            //int x0=_mousePosA.X;
            //for (int i = 0; i < 15; i++)
            //{
            //    while(x0<_mousePosB.X)
            //    for (int y0 = _mousePosA.Y; y0 < _mousePosB.Y; y0++)
            //    {
            //        byte* row = (byte*)bmd.Scan0 + (y0 * bmd.Stride);

            //        byte* row1 = (byte*)bmd.Scan0 + ((y0+1) * bmd.Stride);
                    
            //        if (row[x0 * PixelSize] == 0)
            //        {

            //            row[x0 * PixelSize] = (byte)i;
            //            row[x0 * PixelSize + 1] = (byte)i;
            //            row[x0 * PixelSize + 2] = (byte)i;

            //        }
            //        else { 
                    
                    
                    
                    
            //        }
                
                
            //    }
            
            
            
            
            //}
            
            //int x0=listtop[0].X;
            //int y0;

            //new Rectangle(0, 0, imgPictureBox.Image.Width, imgPictureBox.Image.Height
            BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int PixelSize = 3;



            for (int i = 0; i < listtop.Count; i++)
            {
                int x0 = listtop[i].X;
                int y0 = listtop[i].Y;



                for (int y = y0; y < _mousePosB.Y; y++)//向下搜寻
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                    if (row[x0 * PixelSize] < 255)
                    {

                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;

                    }

                }


            }

            for (int i = 0; i < listdown.Count; i++)
            {
                int x0 = listdown[i].X;
                int y0 = listdown[i].Y;


                for (int y = y0; y > _mousePosA.Y; y--)//向上搜寻
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                    if (row[x0 * PixelSize] < 255)
                    {

                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;

                    }

                }

            }



            ((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            imgPictureBox.Image = imgPictureBox.Image;


        }
        
        int firstavgwidth = 0;
        List<Point> list = new List<Point>();
        private void initial()
        {
            Point s;
            
            int y;
            int x = _mousePosA.X+10;
            int hang = 0;
            int hei = 0;
            int flag = 0;
            int min = 0;
            int max = 0;


            for (y = _mousePosA.Y; y < _mousePosB.Y; y++)
            {
                hang = hei;
                hei = 0;
                //if (y < pictureBox1.Image.Height && y >0)
                if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B)
                {
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y).B.ToString());
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y + 1).B.ToString());
                    if (((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B < 255 && flag == 0)
                    {
                        min = y;
                        flag++;
                       
                    }

                    else if (flag == 1)
                    {

                        max = y;
                        //MessageBox.Show(y.ToString());
                        flag = 0;
                        hei = (max + min) / 2;
                        //MessageBox.Show(hei.ToString());
                        list.Add(new Point(x, hei));

                        //s = new Point(1, hei);
                        firstavgwidth += (max - min);

                        
                    }

                }

            }
            //MessageBox.Show(list[0].Y.ToString());

            MessageBox.Show("搜索成功");
        }

        private void initial2()
        {
            Point s;

            int y;
            int x = 10;
            int hang = 0;
            int hei = 0;
            int flag = 0;
            int min = 0;
            int max = 0;


            for (y = 0; y < imgPictureBox.Image.Height-1; y++)
            {
                hang = hei;
                hei = 0;
                //if (y < pictureBox1.Image.Height && y >0)
                if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B)
                {
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y).B.ToString());
                    //MessageBox.Show(((Bitmap)pictureBox1.Image).GetPixel(x, y + 1).B.ToString());
                    if (((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B < 255 && flag == 0)
                    {
                        min = y;
                        flag++;

                    }

                    else if (flag == 1)
                    {

                        max = y;
                        //MessageBox.Show(y.ToString());
                        flag = 0;
                        hei = (max + min) / 2;
                        //MessageBox.Show(hei.ToString());
                        list.Add(new Point(x, hei));

                        //s = new Point(1, hei);
                        firstavgwidth += (max - min);


                    }

                }

            }
            //MessageBox.Show(list[0].Y.ToString());

            MessageBox.Show("搜索成功");
        }
        
       
        private void 搜索起点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initial2();
        }
        int complexid = 0;
        private void 波形扫描ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //搜索起点,指定复杂波
            Graphics g = imgPictureBox.CreateGraphics();
            //g.FillEllipse(Brushes.Red, e.X, e.Y, 3, 3);
            ArrayList aa = new ArrayList();
            int x = _mousePosA.X+1;
            int min = 0;
            int max = 0;
            int p = 0;//记录波形的平衡位置
            int sum = 0;
            for (int i = 0; i < list.Count; i++)
            {

                p = list[i].Y;
                //MessageBox.Show(pictureBox1.Image.Width.ToString());
                while (x < _mousePosB.X)
                {

                    for (int y = p; y < imgPictureBox.Image.Height - 1; y++)//向下搜寻
                    {

                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B || y > p + 3)
                        {
                            if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B > 0)
                            {
                                //g.FillEllipse(Brushes.Red, x,y , 2, 2);
                                sum++;


                            }

                            max = y;

                            //lineTop.Add(max+1);

                            break;


                        }


                    }

                    for (int y = p; y > 0; y--)//向上搜寻
                    {

                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y - 1).B || y < p - 3)
                        {
                            if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B > 0)
                            {
                               
                                sum++;

                            }

                            min = y;
                            break;

                        }

                    }

                    //MessageBox.Show((max+min).ToString());
                    //p = (max + min) / 2;
                    //MessageBox.Show(p.ToString());
                    g.FillEllipse(Brushes.Blue, x, p, 2, 2);
                    //g.FillEllipse(Brushes.Red, x, min, 4, 4);
                    x++;



                }

                //MessageBox.Show(sum.ToString());
                aa.Add(sum);
                sum = 0;
                x = _mousePosA.X+1;
            }
            int max0 = (int)aa[0];
            for (int jj = 0; jj < aa.Count; jj++)
            {
                if (max0 < (int)aa[jj])
                {
                    max0 = (int)aa[jj];
                }
            }
            //MessageBox.Show("最大值" + ":" + max0.ToString());
            for (int ii = 0; ii < aa.Count; ii++)
            {
                if (max0 == (int)aa[ii])
                {
                    MessageBox.Show("共有"+aa.Count+"条波"+","+"第" + ii + "条是复杂波");
                    complexid = ii;
                }
            }

            MessageBox.Show("扫描成功!");
        }

        private void 复位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string c = textBox1.Text;
           
            //if (c== "1")
            //{

                //imgPictureBox.Width = weight;
                //imgPictureBox.Height = height;
            //}
            //else
            //{
                //MessageBox.Show("放大倍数应为1.0!");
            
            //}
            
            //imgPictureBox.Refresh();
        }

        List<Point> newID = new List<Point>();
        private void Complex()
        {
            int x =_mousePosA.X+1;
            int y;

            int max = 0;
            int min = 0;
            int p = 0;
            int linewidth = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (i == complexid)
                {
                    continue;
                }
                p = list[i].Y;

                while (x < _mousePosB.X)
                {

                    for (y = p; y < _mousePosB.Y; y++)//向下搜索
                    {
                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y + 1).B)
                        {
                            max = y;
                            break;
                        }

                    }

                    for (y = p; y > _mousePosA.Y; y--)//向上搜索
                    {
                        if (((Bitmap)imgPictureBox.Image).GetPixel(x, y).B != ((Bitmap)imgPictureBox.Image).GetPixel(x, y - 1).B)
                        {
                            min = y;
                            break;
                        }


                    }

                    linewidth = max - min;
                    if (linewidth < 14)
                    {
                        p = (max + min) / 2;
                        //Complex1(x, p);
                        newID.Add(new Point(x, y));
                    }
                    else
                    {


                    }
                    //line.Add(new Point(x, linewidth));
                    x++;


                }
                Complex1();
                newID.Clear();
                //MessageBox.Show(x.ToString());
                x = _mousePosA.X+1;
            }
            MessageBox.Show("去除成功!");

        }

        private unsafe void Complex1()
        {
            int x0;
            int y0;

            //new Rectangle(0, 0, imgPictureBox.Image.Width, imgPictureBox.Image.Height
            BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int PixelSize = 3;

            for (int i=0; i < newID.Count;i++ )
            {
                x0 = newID[i].X;
                y0 = newID[i].Y;
                //border = line[(x-1) + i * (pictureBox1.Image.Width - 1)].Y;
                for (int y = y0; y < bmd.Height - 1; y++)//向下搜寻
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    byte* row1 = (byte*)bmd.Scan0 + ((y + 1) * bmd.Stride);

                    if (row[x0 * PixelSize] < 255)
                    {

                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;

                    }
                    if (row[x0 * PixelSize] == row1[x0 * PixelSize])
                    {

                        break;
                    }

                }

                for (int y = y0; y > 0; y--)//向上搜寻
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    byte* row1 = (byte*)bmd.Scan0 + ((y - 1) * bmd.Stride);

                    if (row[x0 * PixelSize] < 255)
                    {

                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;

                    }
                    if (row[x0 * PixelSize] == row1[x0 * PixelSize])
                    {

                        break;
                    }

                }


            }

            ((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            imgPictureBox.Image = imgPictureBox.Image;
        
        
        }
        private void 平滑波去除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Complex();
        }

        private unsafe void outline()
        {
            //int x0;
            //int PixelSize = 3;

            //BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //for (int y0 = _mousePosA.Y; y0 < _mousePosB.Y; y0++)
            //{
            //    byte* row = (byte*)bmd.Scan0 + (y0 * bmd.Stride);
            //    byte* row1 = (byte*)bmd.Scan0 + ((y0+1) * bmd.Stride);
            //    byte* row2 = (byte*)bmd.Scan0 + ((y0-1) * bmd.Stride);
                

            //    for(x0=_mousePosA.X;x0<_mousePosB.X;x0++)
            //    {
            //        //if (row2[x0 * PixelSize] != row1[x0 * PixelSize])// && row[(x0 - 1) * PixelSize] < 255 && row[(x0 + 1) * PixelSize] < 255&& row2[(x0 - 1) * PixelSize] < 255 && row1[(x0 - 1) * PixelSize] < 255 && row2[(x0 + 1) * PixelSize] < 255 && row1[(x0 + 1) * PixelSize] < 255)
            //        //{

            //        //    //row[x0 * PixelSize] = 0;
            //        //    //row[x0 * PixelSize + 1] = 0;
            //        //    //row[x0 * PixelSize + 2] = 0;
            //        //}
            //        if (row[x0 * PixelSize] != row1[x0 * PixelSize] || row[(x0) * PixelSize] != row[(x0+1) * PixelSize])
            //        {
            //            row[x0 * PixelSize] = 0;
            //            row[x0 * PixelSize + 1] = 255;
            //            row[x0 * PixelSize + 2] = 0;
                        
            //        }
            //        else
            //        {
            //            row[x0 * PixelSize] = 0;
            //            row[x0 * PixelSize + 1] = 0;
            //            row[x0 * PixelSize + 2] = 0;

                        
            //        }
                    
                   
                
                
            //    }



            //}

            //((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            //imgPictureBox.Image = imgPictureBox.Image;
            //imgPictureBox.Refresh();
        

        
        }
        
        private void 提取轮廓ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outline();
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start("help.doc");//Word
        }

        private void 鹰眼图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eyePanel.Visible = true;
           
        }

        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eyePanel.Visible = false;
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eyePanel.Width += 50;
            eyePanel.Height += 50;
        }

        private void 数字化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void 保存位图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }

        private void 起点入库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        int aa = 1;
        private void 取消框选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show((_listGeometry.Count - aa).ToString());
            if (_listGeometry.Count - 1 >= 0)
            {
                _listGeometry[_listGeometry.Count - 1].geometryType = 0;
                aa++;
            }
            //else { _listGeometry.Clear(); aa = 1; }
            
            imgPictureBox.Refresh();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

        }

        private void 数字化图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void 提取波形ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private unsafe void  eraser1()
        {
            int x0;
            int PixelSize = 3;

            BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int y0 = _mousePosA.Y; y0 < _mousePosB.Y; y0++)
            {
                byte* row = (byte*)bmd.Scan0 + (y0 * bmd.Stride);
                byte* row1 = (byte*)bmd.Scan0 + ((y0 + 1) * bmd.Stride);
                byte* row2 = (byte*)bmd.Scan0 + ((y0 - 1) * bmd.Stride);


                for (x0 = _mousePosA.X; x0 < _mousePosB.X; x0++)
                {
                    
                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;


                }



            }

            ((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            imgPictureBox.Image = imgPictureBox.Image;
            imgPictureBox.Refresh();


        }
        private void 橡皮ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //eraser1();
        }

        string connString = "mongodb://localhost:27017";
        private void 起点录入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取数据库实例连接
            MongoServer server = MongoServer.Create(connString);
            MongoDatabase seismograms = server.GetDatabase("seismicwaves");
            MongoCollection mytable = seismograms.GetCollection("mytable");
            //实例化一个客户端连接属性实例
            MongoClientSettings clientSetting = new MongoClientSettings();
            //设置mongo地址，作为集群的对外口的接口
            MongoServerAddress ServerAdder = new MongoServerAddress("G", 4009);
            clientSetting.Server = ServerAdder;
            MongoClient client = new MongoClient(clientSetting);//根据设置的属性，实例化一个客户端
            MongoServer server1 = client.GetServer();

            for (int i = 0; i < list.Count; i++)
            {
                BsonDocument doc = new BsonDocument   
                {
                    {"id",i },
                    {"startpoint",list[i].X+","+list[i].Y },
                    {"time",20-(i/10) },
                    
                };
                //for (int i = 0; i < list.Count;i++ )
                //{
                //    mytable.Insert(list[i].Y);
                //}
                mytable.Insert(doc);
            }
            MessageBox.Show("录入成功");
        }





        private unsafe void outline1()
        {
            int x0;
            int PixelSize = 3;

            BitmapData bmd = ((Bitmap)imgPictureBox.Image).LockBits(imgPictureBox.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int y0 = _mousePosA.Y; y0 < _mousePosB.Y; y0++)
            {
                byte* row = (byte*)bmd.Scan0 + (y0 * bmd.Stride);
                byte* row1 = (byte*)bmd.Scan0 + ((y0 + 1) * bmd.Stride);
                byte* row2 = (byte*)bmd.Scan0 + ((y0 - 1) * bmd.Stride);


                for (x0 = _mousePosA.X; x0 < _mousePosB.X; x0++)
                {
                    if (row1[x0 * PixelSize]>0&&row2[x0 * PixelSize]>0&& row[(x0 - 1) * PixelSize] >0 && row[(x0 + 1) * PixelSize] >0&& row2[(x0 - 1) * PixelSize] >0 && row1[(x0 - 1) * PixelSize] >0 && row2[(x0 + 1) * PixelSize] >0 && row1[(x0 + 1) * PixelSize] >0)
                    {
                        row[x0 * PixelSize] = 255;
                        row[x0 * PixelSize + 1] = 255;
                        row[x0 * PixelSize + 2] = 255;
                    }
                   
                }



            }

            ((Bitmap)imgPictureBox.Image).UnlockBits(bmd);
            imgPictureBox.Image = imgPictureBox.Image;
            imgPictureBox.Refresh();


        }

        private unsafe void outline2()
        { 
        
        
        
        
        
        
        }

        private void 去除孤立点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outline1();
        }

        private void 去噪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Complex2();
        }

        private void 区域起点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            list.Clear();
            initial();
        }

        private void 提取轮廓ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            outline();
        }

        private void 波形跟踪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initial1();
        }

        private void 保存位图ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            Bitmap bit = new Bitmap(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y);
            using (Graphics g = Graphics.FromImage(bit))
            {
                g.DrawImage(imgPictureBox.Image, new Rectangle(0, 0, _mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y), new Rectangle(_mousePosA.X, _mousePosA.Y, _mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y), GraphicsUnit.Pixel);
                bit.Save("c.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                bit.Dispose();
            }
            MessageBox.Show("保存成功!");
        }
        public void save_WT()
        {
            
            try
            {
            int x = _mousePosB.X- _mousePosA.X;
            int y = _mousePosB.Y - _mousePosA.Y;
            Bitmap bit = new Bitmap(x, y);
            using (Graphics g = Graphics.FromImage(bit))
            {
                

                    g.DrawImage(imgPictureBox.Image, new Rectangle(0, 0, x, y), new Rectangle(_mousePosA.X, _mousePosA.Y, x,y), GraphicsUnit.Pixel);
                    bit.Save("c.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    bit.Dispose();


            }
           }
            catch (Exception ex)
            {

                MessageBox.Show("请先关闭当前窗体!");
            }
        
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            check = 8;
            imgPictureBox.Cursor = new Cursor("1.cur");
        }

        private void 图纸录入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        public unsafe void splice()
        {
            FileStream fs = new FileStream("HAN.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //int xid0=0,xid1=0,xid2=0;
            //for (int i = 0; i < list.Count; i++)
            //{

            //    if (list[i].Y - listS[0].Y> 0)
            //    {
            //        MessageBox.Show("xid0" + i);
            //        xid0 = i;
            //        break;
            //    }

            //    if (list[i].Y - listS[1].Y > 0)
            //    {
            //        MessageBox.Show("xid1" + i);
            //        xid1 = i;
            //        break;
            //    }

            //    if (list[i].Y - listS[2].Y > 0)
            //    {
            //        MessageBox.Show("xid2" + i);
            //        xid2 = i;
            //        break;
            //    }

            
            
            //}
            List<Point> Final = new List<Point>();
            //int breakpoint0, breakpoint1, breakpoint2;
            //breakpoint0 = start.X + (_sid - xid0)*imgPictureBox.Width+listS[0].X;
            //breakpoint1 = start.X + (_sid - xid0) * imgPictureBox.Width + listS[1].X;
            //breakpoint2 = start.X + (_sid - xid0)*imgPictureBox.Width+listS[2].X;
            Point start1,start2;

           
            int aa = 0,dd=0;

            //for (int j = 0; ; j++)
            //{
            //    Final.Add(new Point(j + 1, listF[0].Y));
            //    if (j > 1000)
            //    {
            //        break;
            //    }
            
            //}

            for (int j = 0; j < listF.Count; j++)
            {

                Final.Add(new Point(j + 1, listF[j].Y));
                //sw.Write(j+1+"," + listF[j].Y + "\r\n");
                aa = j;
                if (j > (int)width[0])
                {
                    break;
                }
            }
            Final.RemoveAt(aa);
            Final.RemoveAt(aa-1);
            int bb = listF[(int)width[0] - 1].Y - listF[(int)width[0]].Y;


            start1 = new Point((int)width[0]+1,listF[(int)width[0]].Y+bb);
            for (int j = (int)width[0]; j < listF.Count; j++)
            {
                Final.Add(new Point(j + 1, listF[j].Y+bb));
                //sw.Write(j + 1 + "," + listF[j].Y + "\r\n");
                dd = j;
                if (j > (int)width[1])
                {
                    break;
                }
            }
            Final.RemoveAt(dd);
            Final.RemoveAt(dd - 1);

            
            int cc = listF[(int)width[1]-1].Y - listF[(int)width[1]+1].Y;
            start2 = new Point((int)width[1] + 1, listF[(int)width[1]].Y + bb + cc);
            MessageBox.Show(width[1].ToString());
            MessageBox.Show(cc.ToString());
            for (int j = (int)width[1]; j < listF.Count; j++)
            {
                Final.Add(new Point(j + 1, listF[j].Y + cc+bb));
                //sw.Write(j + 1 + "," + listF[j].Y + "\r\n");
                
            }

            for (int j = 0; j < Final.Count; j++)
            {
                sw.Write(Final[j].X + "," + Final[j].Y + "\r\n");

            }


            
            //时间拼接完成后，加入时间信息

            //List<Point> Last = new List<Point>();
            //for (int j = 1; j < 10000; j++)
            //{
            //    Last.Add(new Point( j, Final[0].Y));

            //}

            //for (int j = 10000; j < 10000 + start1.X; j++)
            //{
            //    Last.Add(new Point(j, Final[j-10000].Y));
                
            //}

            //for (int j = 10000 + start1.X; j < 10000 + start1.X + 20000; j++)
            //{
            //    Last.Add(new Point(j, start1.Y));
            
            
            //}

            //for (int j = 10000 + start1.X+20000; j < 10000 + 20000+start2.X; j++)
            //{
            //    Last.Add(new Point(j, Final[j - 30000].Y));


            //}

            //for (int j = 10000 + 20000 + start2.X; j < 10000 + 20000 + start2.X+10000; j++)
            //{
            //    Last.Add(new Point(j, start2.Y));


            //}

            //for (int j = 10000 + 20000 + start2.X+10000; j < 40000 + Final.Count; j++)
            //{
            //    Last.Add(new Point(j, Final[j - 40000].Y));


            //}







                //for (int j = 0; j < Last.Count; j++)
                //{
                //    sw.Write(Last[j].X + "," + Last[j].Y + "\r\n");

                //}




            sw.Flush();
            sw.Close();
            fs.Close();
            MessageBox.Show("波形拼接完成!!");

           
        
        
        }

        Hashtable ht = new Hashtable();
        Point start;
        Point end;
        int _sid = 0, _eid = 0;
        
        public unsafe void timesplice()
        {
            
           
            //MessageBox.Show(start.X.ToString() + "," + start.Y.ToString());
            //MessageBox.Show(end.X.ToString() + "," + end.Y.ToString());

          
        
        }
        
        
        private void 波形拼接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splice();
        }

        private void 时间拼接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timesplice();
        }

        private void imgPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check = 0;
            //imgPictureBox.Cursor = Cursors.Arrow;
            //mark = false;
            this.imgPictureBox.Refresh();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        bool mark=false;
        private void 指定时间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            mark = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(_mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y);
            using (Graphics g = Graphics.FromImage(bit))
            {
                g.DrawImage(imgPictureBox.Image, new Rectangle(0, 0, _mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y), new Rectangle(_mousePosA.X, _mousePosA.Y, _mousePosB.X - _mousePosA.X, _mousePosB.Y - _mousePosA.Y), GraphicsUnit.Pixel);
                bit.Save("E:\\测试\\c.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                bit.Dispose();
            }
            //MessageBox.Show("保存成功!");
            Form1 f = new Form1();
            f.Show();
        }

        private void 小号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            check = 4;
            imgPictureBox.Cursor = new Cursor("1.cur");
        }

        private void 中号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            check = 6;
            imgPictureBox.Cursor = new Cursor("2.cur");
        }

        private void 大号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            check = 8;
            imgPictureBox.Cursor = new Cursor("3.cur");
        }

        private void 取消ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Post();
        }

        private void 批量选择ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2();
            fm2.Show();
        }
    }

   
    public enum GeometryType 
    {
        Symbol=1,
        Line,
        Polyline,
        Polygon,
        Rectangle,
        Rectangle1,
        Ellipse,
        Text,
        cursor

    }

    public class Geometry
    {
        public int x1, y1, x2, y2;
        public GeometryType geometryType;
        public Point[] points;
        public Color color;
        public string geometryInfo;
    }

    public class GeometryPoly
    {
        public Point[] points;
        public Color color;
        public GeometryType geometryType;
    }
   
}
