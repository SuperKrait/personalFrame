using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace PictureModel
{
    public class PictureHelper
    {
        public static Image GetImage(byte[] fileData)
        {
            Image image = null;
            using (MemoryStream mem = new MemoryStream(fileData))
            {
                image = Image.FromStream(mem);
            }
            return image;
        }
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="oriImage">原始图片</param>
        /// <param name="width">要求的宽</param>
        /// <param name="height">要求的高</param>
        /// <returns></returns>
        public static Image GetThumbnail(Image oriImage, int desWidth, int desHeight)
        {
            int oriWidth = oriImage.Width;
            int oriHeight = oriImage.Height;

            int tmpWidth = 0;
            int tmpHeight = 0;

            //保证某条边一定超过要求的边长
            if (oriWidth > desWidth || oriHeight > desHeight)
            {
                //高比宽要大的话
                if (oriHeight > oriWidth)
                {
                    tmpWidth = (int)(Math.Round(((float)desHeight / oriHeight) * oriWidth));
                    if (tmpWidth <= 0)
                        tmpWidth = 1;
                    tmpHeight = desHeight;
                }
                else
                {
                    tmpHeight = (int)(Math.Round(((float)desWidth / oriWidth) * oriHeight));
                    if (tmpHeight <= 0)
                        tmpHeight = 1;
                    tmpWidth = desWidth;
                }
            }
            else//都小于要求的边长，则保持原始不变
            {
                tmpWidth = oriWidth;
                tmpHeight = oriHeight;
            }

            Image bitmap = new Bitmap(desWidth, desHeight);
            Graphics g = Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.White);
            if(tmpWidth < tmpHeight)
                g.DrawImage(oriImage, new System.Drawing.Rectangle((desWidth - tmpWidth) / 2, 0, tmpWidth, tmpHeight), new System.Drawing.Rectangle(0, 0, oriWidth, oriHeight), System.Drawing.GraphicsUnit.Pixel);
            else
                g.DrawImage(oriImage, new System.Drawing.Rectangle(0, (desHeight - tmpHeight) / 2, tmpWidth, tmpHeight), new System.Drawing.Rectangle(0, 0, oriWidth, oriHeight), System.Drawing.GraphicsUnit.Pixel);

            return bitmap;
        }

        public static byte[] GetBytes(Image image, string exName)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            image.Save(ms, exName == ".png" ? System.Drawing.Imaging.ImageFormat.Png : System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] data = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(data, 0, data.Length);
            return data;
        }
    }
}
