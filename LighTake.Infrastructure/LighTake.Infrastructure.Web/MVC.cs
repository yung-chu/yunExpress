using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Web
{

    public class MvcExtensions
    {
        #region 输出HTML到页面上

        public static void OutputScript(HttpContext CurHttpContext, string strScript)
        {
            //CurHttpContext.Response.Output.Write("<script>alert('OK，呵呵')</script>");
            CurHttpContext.Response.Output.Write("<script>{0}</script>", strScript);
        }

        #endregion

        #region 绑定DropDown

        /// <summary>
        /// 
        /// </summary>
        [Description("Super cool")]
        enum ServiceDocumentType : int
        {
            [Description("Super cool")]
            st1 = 1,
            [Description("Super cool")]
            st2 = 2
        }

        public static void DropDownListBind(Enum eb)
        {
            var typeitems = new List<SelectListItem>();
            foreach (string i in Enum.GetNames(typeof(ServiceDocumentType)))
            {
                typeitems.Add(new SelectListItem
                {
                    Text = ((ServiceDocumentType)
                        Enum.Parse(typeof(ServiceDocumentType), i)).GetDescription(),
                    Value = ((int)((ServiceDocumentType)
                    Enum.Parse(typeof(ServiceDocumentType), i))).ToString()
                });
            }

            /*
             erviceDocumentCreateModel model = new ServiceDocumentCreateModel
            {
                ProductID = entity.ProductID,
                Content = entity.Content,
                DownloadUrl = entity.DownloadUrl,
                ImageUrl = entity.ImageUrl,
                Level = entity.Level,
                LinkUrl = entity.LinkUrl,
                Title = entity.Title,
                Type = new SelectList(typeitems, "Value", "Text", (int)entity.Type),
                Status = new SelectList(statusItems, "Value", "Text", (int)entity.Status),
            };
            其中Type和Status都是SelectList类型，这样方便前台Model在VIEW中的展示。
            前台中展示就一句搞定：
            <%=Html.DropDownList("Type",Model.Type)%>
             */
        }



        #endregion

        #region MVC如何将用户控件内容转换为字符串并输出

        public static string RenderPartialToString(string file, object view)
        {
            ViewDataDictionary vd = new ViewDataDictionary(view);
            ViewPage viewPage = new ViewPage { ViewData = vd };
            Control control = viewPage.LoadControl(file);

            viewPage.Controls.Add(control);

            StringBuilder str = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(str))
            {
                using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter))
                {
                    viewPage.RenderControl(htmlTextWriter);
                }
            }

            return str.ToString();
        }

        // 调用它
        public string GetHtmlFromPartial()
        {
            string s = RenderPartialToString("~/Views/usercart.ascx", null);

            return s;
        }

        #endregion

        #region MVC 把PartialView、View转换成字符串

        /// <summary>
        /// 描述：输出View HTML 字符串
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName">视图文件名</param>
        /// <param name="masterName">母板页文件名</param>
        /// <returns></returns>
        protected static string RenderViewToString(Controller controller, string viewName, string masterName)
        {
            IView view = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, masterName).View;
            using (StringWriter writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, writer);
                viewContext.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 描述：输出PartialView HTML 字符串
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="partialViewName">部分视图文件名</param>
        /// <returns></returns>
        protected static string RenderPartialViewToString(Controller controller, string partialViewName)
        {
            IView view = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName).View;
            using (StringWriter writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, writer);
                viewContext.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }
        #endregion

    }



}
