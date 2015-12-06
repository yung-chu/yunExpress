/*----------------------------------------
 * 功能: GDI 类库，实现图片的缩略和图片水印，支持透明度
 * 作者：三角猫 DeltaCat
 * 版权：http://www.zu14.cn/
 * 日期：2008/12/19
 * =====================================
 * 注意事项：
 * ------------
 * (1) GIF动画图片水印或缩略后，将只有第一帧
 * (2) 透明背景的GIF图片，水印和缩略后，如果想仍然保持透明背景，须将输出图片的格式设为Png
 * (3) 输出图片的格式，不建议采用GIF，因为gif失真最严重，建议用png 和 高质量的 jpg
 --------------------------------------*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace LighTake.Infrastructure.Common
{
    public static class ImageUtil
    {
        #region 创建缩略图
        private static Bitmap ThumbnailImage(Image sourceImage, int toWidth, int toHeight)
        {
            Bitmap bmp = new Bitmap(toWidth, toHeight, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                ////设置绘图质量的参数，均设为高
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.Clear(Color.Transparent); //清除背景色
                g.DrawImage(sourceImage, new Rectangle(0, 0, toWidth, toHeight),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), GraphicsUnit.Pixel);
            }

            return bmp;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceImage">原图片</param>
        /// <param name="toWidth">缩略到的宽度</param>
        /// <param name="toHeight">缩略到的高度</param>
        /// <param name="toImageFormat">保存的图片格式，不建议采用GIF格式</param>
        /// <param name="imageQulity">生成的图片质量，取值范围0~100，越大质量越高，但文件也越大(只适用于JPG格式)</param>
        /// <returns>返回图片的流格式</returns>
        public static Stream GetThumbnail(Image sourceImage, int toWidth, int toHeight,
            ImageFormat toImageFormat, long imageQulity)
        {
            using (Bitmap bmp = ThumbnailImage(sourceImage, toWidth, toHeight))
            {
                Stream ms = new MemoryStream();

                ////如果是要输出JPEG文件，则调整输出的压缩率
                if (toImageFormat.ToString().Equals("jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (EncoderParameters eps = new EncoderParameters(1))
                    {
                        eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQulity);
                        bmp.Save(ms, GetCodecInfo("image/jpeg"), eps);
                    }
                }
                else
                {
                    bmp.Save(ms, toImageFormat);
                }

                return ms;
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceImage">原图片</param>
        /// <param name="toWidth">缩略到的宽度</param>
        /// <param name="toHeight">缩略到的高度</param>
        /// <param name="toImageFormat">图片保存的格式</param>
        /// <param name="imageQulity">生成的图片质量，取值范围0~100，越大质量越高，但文件也越大，只适用于JPG格式</param>
        /// <param name="thumnailImageSavePath">图片保存的路径</param>
        /// <returns>返回文件保存后的路径/returns>
        public static string GetThumbnail(Image sourceImage, int toWidth, int toHeight,
            ImageFormat toImageFormat, long imageQulity, string thumnailImageSavePath)
        {
            using (Bitmap bmp = ThumbnailImage(sourceImage, toWidth, toHeight))
            {
                ////如果是要输出JPEG文件，则调整输出的压缩率
                if (toImageFormat.ToString().Equals("jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (EncoderParameters eps = new EncoderParameters(1))
                    {
                        eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQulity);
                        bmp.Save(thumnailImageSavePath, GetCodecInfo("image/jpeg"), eps);
                    }
                }
                else
                {
                    bmp.Save(thumnailImageSavePath, toImageFormat);
                }

                return thumnailImageSavePath;
            }
        }

        #endregion

        #region 创建图片水印
        private static Image ImageWaterMask(Image sourceImage, Image maskImage, int xPoint, int yPoint,
            float lucencyValue, Color backgroundColor)
        {
            ////判断原图片是否是GIF图片
            bool sourceImageIsGif = sourceImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif);
            if (sourceImageIsGif)
            {
                Bitmap bmp = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    g.Clear(backgroundColor); //设置背景色

                    g.DrawImage(sourceImage, 0, 0);

                    using (ImageAttributes imgAttrib = ImageAttrib(lucencyValue))
                    {
                        g.DrawImage(maskImage, new Rectangle(xPoint, yPoint, maskImage.Width, maskImage.Height),
                            0, 0, maskImage.Width, maskImage.Height, GraphicsUnit.Pixel, imgAttrib);
                    }
                }

                return bmp as Image;
            }
            else
            {
                using (Graphics g = Graphics.FromImage(sourceImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    using (ImageAttributes imgAttrib = ImageAttrib(lucencyValue))
                    {
                        g.DrawImage(maskImage, new Rectangle(xPoint, yPoint, maskImage.Width, maskImage.Height),
                            0, 0, maskImage.Width, maskImage.Height, GraphicsUnit.Pixel, imgAttrib);
                    }
                }

                return sourceImage;
            }
        }
        /// <summary>
        /// 设置图片水印
        /// </summary>
        /// <param name="sourceImage">原图片</param>
        /// <param name="maskImage">水印图片</param>
        /// <param name="xPoint">水印的左上点的x坐标</param>
        /// <param name="yPoint">水印的左上点的y坐标</param>
        /// <param name="toImageFormat">水印后图片的保存格式</param>
        /// <param name="imageQulity">生成的图片质量，取值范围0~100，越大质量越高，但文件也越大，只适用于JPG格式</param>
        /// <param name="lucencyValue">水印透明度 0~100，越小，透明度越高</param>
        /// <param name="backgroundColor">生成图片的背景色，非特殊需要，请使用 Color.Transparent</param>
        /// <returns>返回流格式</returns>
        public static Stream DoImageWaterMask(Image sourceImage, Image maskImage, int xPoint, int yPoint,
            ImageFormat toImageFormat, long imageQulity, float lucencyValue, Color backgroundColor)
        {
            using (Image img = ImageWaterMask(sourceImage.Clone() as Image, maskImage, xPoint, yPoint, lucencyValue, backgroundColor))
            {
                Stream ms = new MemoryStream();
                if (toImageFormat.ToString().Equals("jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (EncoderParameters eps = new EncoderParameters(1))
                    {
                        eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQulity);
                        img.Save(ms, GetCodecInfo("image/jpeg"), eps);
                    }
                }
                else
                {
                    img.Save(ms, toImageFormat);
                }

                return ms;
            }
        }

        /// <summary>
        /// 设置图片水印
        /// </summary>
        /// <param name="sourceImage">原图片</param>
        /// <param name="maskImage">水印图片</param>
        /// <param name="xPoint">水印的左上点的x坐标</param>
        /// <param name="yPoint">水印的左上点的y坐标</param>
        /// <param name="toImageFormat">水印后图片的保存格式</param>
        /// <param name="imageQulity">生成的图片质量，取值范围0~100，越大质量越高，但文件也越大，只适用于JPG格式</param>
        /// <param name="lucencyValue">水印透明度 0~100，越小，透明度越高</param>
        /// <param name="backgroundColor">生成图片的背景色，非特殊需要，请使用 Color.Transparent</param>
        /// <param name="maskedImageSavePath">水印后图片的保存路径</param>
        /// <returns>返回保存文件的路径</returns>
        public static string DoImageWaterMask(Image sourceImage, Image maskImage, int xPoint, int yPoint,
            ImageFormat toImageFormat, long imageQulity, float lucencyValue, Color backgroundColor, string maskedImageSavePath)
        {
            using (Image img = ImageWaterMask(sourceImage.Clone() as Image, maskImage, xPoint, yPoint, lucencyValue, backgroundColor))
            {
                if (toImageFormat.ToString().Equals("jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (EncoderParameters eps = new EncoderParameters(1))
                    {
                        eps.Param[0] = new EncoderParameter(Encoder.Quality, imageQulity);
                        img.Save(maskedImageSavePath, GetCodecInfo("image/jpeg"), eps);
                    }
                }
                else
                {
                    img.Save(maskedImageSavePath, toImageFormat);
                }

                return maskedImageSavePath;
            }
        }

        #endregion

        /// <summary>
        /// 设置水印图片透明度属性
        /// </summary>
        /// <param name="lucencyValue">透明比例值，0~100,值越小透明度越高</param>
        /// <returns></returns>
        private static ImageAttributes ImageAttrib(float lucencyValue)
        {
            float[][] matrixItems =
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, lucencyValue/100f, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            ImageAttributes imgAttr = new ImageAttributes();
            imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return imgAttr;
        }

        /// <summary> 
        /// 到指定mimeType的ImageCodecInfo 
        /// </summary> 
        /// <param name="mimeType">图片的mime类型</param> 
        /// <returns>返回指定mimeType的ImageCodecInfo</returns> 
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase)) return ici;
            }
            return null;
        }




        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            if (mode == "H" || mode == "W")
            {
                #region 按H和W方式创建缩略图时不足指定高宽处补白
                //新建一个bmp图片
                System.Drawing.Image bitmap1 = new System.Drawing.Bitmap(width, height);

                //新建一个画板
                System.Drawing.Graphics g1 = System.Drawing.Graphics.FromImage(bitmap1);

                //设置高质量插值法
                g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //设置高质量,低速度呈现平滑程度
                g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //清空画布并以白色背景色填充
                g1.Clear(System.Drawing.Color.White);

                //在指定位置并且按指定大小绘制原图片的指定部分
                g1.DrawImage(bitmap, new System.Drawing.Rectangle((width - towidth) / 2, (height - toheight) / 2, towidth, toheight),
                    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.GraphicsUnit.Pixel);

                try
                {
                    //以jpg格式保存缩略图
                    bitmap1.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    bitmap1.Dispose();
                    g.Dispose();
                    g1.Dispose();
                }
                #endregion
            }
            else
            {
                try
                {
                    //以jpg格式保存缩略图
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }


        /**/
        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sy">生成的带文字水印的图片路径</param>
        public static void AddWater(string Path, string Path_sy)
        {
            string addText = "eKindom.com";
            System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            System.Drawing.Font f = new System.Drawing.Font("Verdana", 60);
            System.Drawing.Brush b = new System.Drawing.SolidBrush(System.Drawing.Color.Green);

            g.DrawString(addText, f, b, 35, 35);
            g.Dispose();

            image.Save(Path_sy);
            image.Dispose();
        }

        /**/
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        public static void AddWaterPic(string Path, string Path_syp, string Path_sypf)
        {
            //System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
            //System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf);
            //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            //g.DrawImage(copyImage, new System.Drawing.Rectangle((image.Width - copyImage.Width) / 2, (image.Height - copyImage.Height) / 2, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
            //g.Dispose();

            //image.Save(Path_syp);
            //image.Dispose();

            // 解决图片加水印后质量变差问题 [Modified By Kevin @ 2010-07-23 11:30:18]
            System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(Path);
            System.Drawing.Image maskImage = System.Drawing.Image.FromFile(Path_sypf);

            int x = (sourceImage.Width - maskImage.Width) / 2;
            int y = (sourceImage.Height - maskImage.Height) / 2;
            DoImageWaterMask(sourceImage, maskImage, x, y, System.Drawing.Imaging.ImageFormat.Jpeg, 100, 100, System.Drawing.Color.Transparent, Path_syp);
        }



    }
}
