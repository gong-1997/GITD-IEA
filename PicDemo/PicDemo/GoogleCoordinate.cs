using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PicDemo
{
    public class GoogleCoordinate
    {
        //经度到像素X值
        public static double lngToPixel(double lng, int zoom) 
        {
            return (lng + 180) * (256L << zoom) / 360;
        }

        //像素X到经度
        public static double pixelToLng(double pixelX, int zoom) 
        {
            return pixelX * 360 / (256L << zoom) - 180;
        }

        //纬度到像素Y
        public static double latToPixel(double lat, int zoom) 
        {
            
            double siny = Math.Sin(lat * Math.PI / 180);
            double y = Math.Log((1 + siny) / (1 - siny));
            return (128 << zoom) * (1 - y / (2 * Math.PI));
        }

        //像素Y到纬度
        public static double pixelToLat(double pixelY, int zoom) 
        {
            
            double y = 2 * Math.PI * (1 - pixelY / (128 << zoom));
            double z = Math.Pow(Math.E, y);
            double siny = (z - 1) / (z + 1);
            return Math.Asin(siny) * 180 / Math.PI;
        }
    }
}
