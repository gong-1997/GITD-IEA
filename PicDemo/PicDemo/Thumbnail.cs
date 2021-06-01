using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PicDemo
{
    public class Thumbnail
    {
        /*
        /// <summary>
        /// 创建缩略图(只能指定长或者宽里面的一种，两者都指定的话会失真)
        /// </summary>
        public static void GenThumbnail(string pathImageFrom, string pathImageTo, int width, int height)
        {
            Image imageFrom = null;
            try
            {
                imageFrom = Image.FromFile(pathImageFrom);
            }
            catch
            {
                //throw;
            }
            if (imageFrom == null)
            {
                return;
            }
            // 源图宽度及高度
            int imageFromWidth = imageFrom.Width;
            int imageFromHeight = imageFrom.Height;



            if (height == 0)
            {
                height = width * imageFromHeight / imageFromWidth;
            }
            else
            {
                width = height * imageFromWidth / imageFromHeight;
            }


            // 生成的缩略图实际宽度及高度
            int bitmapWidth = width;
            int bitmapHeight = height;
            // 生成的缩略图在上述"画布"上的位置
            int X = 0;
            int Y = 0;
            // 根据源图及欲生成的缩略图尺寸,计算缩略图的实际尺寸及其在"画布"上的位置
            if (bitmapHeight * imageFromWidth > bitmapWidth * imageFromHeight)
            {
                bitmapHeight = imageFromHeight * width / imageFromWidth;
                Y = (height - bitmapHeight) / 2;
            }
            else
            {
                bitmapWidth = imageFromWidth * height / imageFromHeight;
                X = (width - bitmapWidth) / 2;
            }
            // 创建画布
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            // 用白色清空
            g.Clear(Color.White);
            // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 指定高质量、低速度呈现。
            g.SmoothingMode = SmoothingMode.HighQuality;
            // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。
            g.DrawImage(imageFrom, new Rectangle(X, Y, bitmapWidth, bitmapHeight), new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);
            try
            {
                //经测试 .jpg 格式缩略图大小与质量等最优
                bmp.Save(pathImageTo, ImageFormat.Jpeg);
            }
            catch
            {
            }
            finally
            {
                //显示释放资源
                imageFrom.Dispose();
                bmp.Dispose();
                g.Dispose();
            }
        }
        */
        public static Bitmap GenThumbnail(Image srcImage,int len)
        {
            int dstImageWidth, dstImageHeight;
            Bitmap bmp = null;
            Graphics g = null;

            if (srcImage.Width > srcImage.Height)
            {
                dstImageWidth = len;
                dstImageHeight = srcImage.Height * len / srcImage.Width;
            }
            else
            {
                dstImageHeight = len;
                dstImageWidth = srcImage.Width * len / srcImage.Height;
            }

            try
            {
                bmp = new Bitmap(dstImageWidth, dstImageHeight);
                g = Graphics.FromImage(bmp);

                g.Clear(Color.White);
                //g.DrawImage(bmp, 0, 0);
                g.DrawImage(srcImage, new Rectangle(0, 0, dstImageWidth, dstImageHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
            
            }
            catch
            {
            }
            finally
            {
                if (g != null)
                    g.Dispose();
            }
            return bmp;
            /*
            // 生成的缩略图实际宽度及高度
            int bitmapWidth = width;
            int bitmapHeight = height;
            // 生成的缩略图在上述"画布"上的位置
            int X = 0;
            int Y = 0;
            // 根据源图及欲生成的缩略图尺寸,计算缩略图的实际尺寸及其在"画布"上的位置
            if (bitmapHeight * imageFromWidth > bitmapWidth * imageFromHeight)
            {
                bitmapHeight = imageFromHeight * width / imageFromWidth;
                Y = (height - bitmapHeight) / 2;
            }
            else
            {
                bitmapWidth = imageFromWidth * height / imageFromHeight;
                X = (width - bitmapWidth) / 2;
            }
            // 创建画布
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            // 用白色清空
            g.Clear(Color.White);
            // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 指定高质量、低速度呈现。
            g.SmoothingMode = SmoothingMode.HighQuality;
            // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。
            g.DrawImage(imageFrom, new Rectangle(X, Y, bitmapWidth, bitmapHeight), new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);
            try
            {
                //经测试 .jpg 格式缩略图大小与质量等最优
                bmp.Save(pathImageTo, ImageFormat.Jpeg);
            }
            catch
            {
            }
            finally
            {
                //显示释放资源
                imageFrom.Dispose();
                bmp.Dispose();
                g.Dispose();
            }
             */

        }

    }
}
