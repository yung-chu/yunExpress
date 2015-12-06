using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Core;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Controllers.NewsController
{
    public class KindEditorController : Controller
    {
        private JsonResult ShowError(string message)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = message;

            return Json(hash, "text/html; charset=UTF-8");
        }

        private JsonResult Validate(HttpPostedFileBase imgFile)
        {
            //最大文件大小
            int maxSize = 1024 * 1024;

            if (imgFile == null)
            {
                return ShowError("请选择文件");
            }

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable
                {
                    {"image", "gif,jpg,jpeg,png,bmp"}, 
                };

            String fileName = imgFile.FileName;
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                return ShowError("上传文件大小超过限制,最大1M");
            }

            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable["image"]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return ShowError("上传文件扩展名是不允许的扩展名.\n只允许" + ((String)extTable["image"]) + "格式。");
            }

            return null;
        }

        public JsonResult Upload(HttpPostedFileBase imgFile)
        {


	

            var validateResult = Validate(imgFile);
            if (validateResult != null)
                return validateResult;

            string strDateSection = DateTime.Now.ToString("yyyyMM");

			//文件保存目录路径
			string strSavePath = Server.MapPath(@"\Images\News\" + strDateSection);
			//string strSavePath = sysConfig.UploadPath + "Help\\" + strDateSection + "\\";

			//文件保存目录URL
			string strSaveUrl =Url.Content(@"~/Images/News/" + strDateSection);

			//string strSaveUrl = sysConfig.UploadWebPath + "Help/" + strDateSection + "/";

            if (!Directory.Exists(strSavePath))
            {
                Directory.CreateDirectory(strSavePath);
            }

            String strFileName = imgFile.FileName;
            String strFileExt = Path.GetExtension(strFileName).ToLower();

            String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + strFileExt;

            try
            {
                imgFile.SaveAs(strSavePath+"\\" + newFileName);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return Json(new Hashtable() { { "error", 1 }, { "message", "服务器错误" } }, "text/html; charset=UTF-8");
            }

            String strFileUrl = strSaveUrl + "/" + newFileName;

            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = strFileUrl;
            return Json(hash, "text/html; charset=UTF-8");
        }
    }
}
