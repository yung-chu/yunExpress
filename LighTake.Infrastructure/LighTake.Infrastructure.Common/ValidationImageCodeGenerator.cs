using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace LighTake.Infrastructure.Common
{
    public class ValidationImageCodeGenerator
    {
        public class ValidationImageCode
        {
            private int _length = 4;

            public string Code { get; set; }

            public byte[] FileContent { get; set; }

            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }
        }

        public static ValidationImageCode Generate()
        {
            ValidationImageCode imageCode = new ValidationImageCode();
            imageCode.Code = CreateValidateCode(imageCode.Length);
            imageCode.FileContent = CreateValidateGraphic(imageCode.Code);
            return imageCode;
        }

        private static string CreateValidateCode(int length)
        {
            var randMembers = new int[length];
            var validateNums = new int[length];
            var validateNumberStr = "";
            //生成起始序列值
            var seekSeek = unchecked((int)DateTime.Now.Ticks);
            var seekRand = new Random(seekSeek);
            var beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            var seeks = new int[length];
            for (var i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (var i = 0; i < length; i++)
            {
                var rand = new Random(seeks[i]);
                var pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (var i = 0; i < length; i++)
            {
                var numStr = randMembers[i].ToString();
                var numLength = numStr.Length;
                var rand = new Random();
                var numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (var i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        private static byte[] CreateValidateGraphic(string validateCode)
        {
            var image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 25);
            var g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                var random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    var x1 = random.Next(image.Width);
                    var x2 = random.Next(image.Width);
                    var y1 = random.Next(image.Height);
                    var y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                //Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                //string[] fontName = { "华文新魏", "宋体", "圆体", "黑体", "隶书" };
                //var font = new Font(fontName[new Random().Next(0, validateCode.Length)], 12, (FontStyle.Bold | FontStyle.Italic));
                var font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                var brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(image.Width);
                    var y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}

