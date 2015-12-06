using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Web.UI;

namespace LighTake.Infrastructure.Web.Controllers
{
    public class PersonalCache
    {
        private PersonalCache()
        {
        }

        private static readonly PersonalCache S_Instance = new PersonalCache();

        public object this[string key]
        {
            get { return HttpContext.Current.Session[key]; }
            set { HttpContext.Current.Session[key] = value; }
        }

        public void Remove(string key)
        {
            if (this[key] != null)
                HttpContext.Current.Session.Remove(key);
        }

        public static PersonalCache Instance
        {
            get { return S_Instance; }
        }
    }

    public class BaseController : Controller
    {
        /// <summary>
        /// 判断页面提交方式是否为POST请求
        /// </summary>
        public bool IsPostRequest
        {
            get { return Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase); }
        }

        protected PersonalCache Personal
        {
            get { return PersonalCache.Instance; }
        }

        public void SetViewMessage(ShowMessageType messageType, string message, bool isNotCurrentRequest = false, bool fadeOut = true)
        {
            if (isNotCurrentRequest)
            {
                TempData["ShowMessageType"] = messageType;
                TempData["Message"] = message;
                TempData["IsFadeOut"] = fadeOut;
                return;
            }
            ViewBag.ShowMessageType = messageType;
            ViewBag.Message = message;
            ViewBag.IsFadeOut = fadeOut;
        }


        private void LogException(Exception exception)
        {

        }

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// 
        protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            if (logException)
                LogException(exception);
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }

        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("sys.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }

        protected FileResult FileExcel(byte[] fileContents, string fileNameWithoutExtionsion)
        {
            return new ExcelXlsFileResult(fileContents, fileNameWithoutExtionsion);
        }

        #region 释放对象
        public IList<IDisposable> DisposableObjects { get; private set; }
        public BaseController()
        {
            this.DisposableObjects = new List<IDisposable>();
        }

        protected void AddDisposableObject(object obj)
        {
            var disposable = obj as IDisposable;
            if (null != disposable)
            {
                this.DisposableObjects.Add(disposable);
            }
        }

        protected void AddDisposableObject(params object[] objects)
        {
            foreach (var obj in objects)
            {
                var disposable = obj as IDisposable;
                if (null != disposable)
                {
                    this.DisposableObjects.Add(disposable);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var obj in DisposableObjects)
                {
                    if (null != obj)
                    {
                        obj.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
