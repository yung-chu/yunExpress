using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Collections.Generic;


namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// XmlDocument 操作类
    /// Creater : Kevin.Mo
    /// Create Date : 2010-05-13
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 构构XmlDocument对象
        /// </summary>
        public XmlDocument ObjXmlDoc = new XmlDocument();

        /// <summary>
        /// XML路径名
        /// </summary>
        protected string StrXmlFile;

        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="xmlFile"></param>
        public XmlHelper(string xmlFile)
        {
            try
            {
                ObjXmlDoc.Load(xmlFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            StrXmlFile = xmlFile;
        }
        public XmlHelper() { }

        /// <summary>
        /// 删除一个节点
        /// </summary>

        /// <param name="xpath"> </param>
        public void Delete(string xpath)
        {
            //string xpath = Node.Substring(0, Node.LastIndexOf("/"));
            XmlNode root = ObjXmlDoc.DocumentElement;

            if (root != null)
            {
                XmlNodeList nodeList = root.SelectNodes(xpath);

                if (nodeList != null)
                    foreach (XmlNode item in nodeList)
                        root.RemoveChild(item);
            }

            Save();
        }

        /// <summary>
        /// XML转为DataView
        /// </summary>
        /// <param name="xmlPathNode"></param>
        /// <returns></returns>
        public DataView GetData(string xmlPathNode)
        {
            var set = new DataSet();
            var selectSingleNode = ObjXmlDoc.SelectSingleNode(xmlPathNode);
            if (selectSingleNode != null)
            {
                var reader = new StringReader(selectSingleNode.OuterXml);
                set.ReadXml(reader);
            }
            if (set.Tables.Count > 0)
                return set.Tables[0].DefaultView;
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainNode"></param>
        /// <returns></returns>
        public XmlNode SelectSingleNode(string mainNode)
        {
            XmlNode node = ObjXmlDoc.SelectSingleNode(mainNode);
            return node;
        }
        /// <summary>
        /// 创建一个文档节点
        /// </summary>
        /// <param name="mainNode"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public XmlNode CreateNode(string mainNode, string content)
        {
            XmlNode node = ObjXmlDoc.CreateNode(XmlNodeType.Element, mainNode, "");
            node.InnerText = content;
            return node;
        }
        /// <summary>
        /// 创建一个文档节点
        /// </summary>
        /// <param name="element"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public XmlElement CreateElement(string element, string content)
        {
            XmlElement newChild = ObjXmlDoc.CreateElement(element);
            newChild.InnerText = content;
            return newChild;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="element"></param>
        public void AppendChild(XmlNode node, XmlElement element)
        {
            element.AppendChild(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeMain"></param>
        /// <param name="nodeChild"></param>
        public void AppendChild(XmlNode nodeMain, XmlNode nodeChild)
        {
            nodeMain.AppendChild(nodeChild);
        }

        /// <summary>
        /// 插入一个节点
        /// </summary>
        /// <param name="mainNode"></param>
        /// <param name="element"></param>
        /// <param name="content"></param>
        public void InsertElement(string mainNode, string element, string content)
        {
            XmlNode node = ObjXmlDoc.SelectSingleNode(mainNode);
            XmlElement newChild = ObjXmlDoc.CreateElement(element);
            newChild.InnerText = content;
            if (node != null) node.AppendChild(newChild);
        }

        /// <summary>
        /// 插入一个节点
        /// </summary>
        /// <param name="mainNode"></param>
        /// <param name="element"></param>
        /// <param name="attrib"></param>
        /// <param name="attribContent"></param>
        /// <param name="content"></param>
        public void InsertElement(string mainNode, string element, string attrib, string attribContent, string content)
        {
            XmlNode node = ObjXmlDoc.SelectSingleNode(mainNode);
            XmlElement newChild = ObjXmlDoc.CreateElement(element);
            newChild.SetAttribute(attrib, attribContent);
            newChild.InnerText = content;
            if (node != null) node.AppendChild(newChild);
        }

        /// <summary>
        /// 插入文档节点
        /// </summary>
        /// <param name="mainNode"></param>
        /// <param name="childNode"></param>
        /// <param name="element"></param>
        /// <param name="content"></param>
        public void InsertNode(string mainNode, string childNode, string element, string content)
        {
            XmlNode node = ObjXmlDoc.SelectSingleNode(mainNode);
            XmlElement newChild = ObjXmlDoc.CreateElement(childNode);
            if (node != null) node.AppendChild(newChild);
            XmlElement element2 = ObjXmlDoc.CreateElement(element);
            element2.InnerText = content;
            newChild.AppendChild(element2);
        }

        /// <summary>
        /// 更新节点内容
        /// </summary>
        /// <param name="xmlPathNode"></param>
        /// <param name="content"></param>
        public void Replace(string xmlPathNode, string content)
        {
            var selectSingleNode = ObjXmlDoc.SelectSingleNode(xmlPathNode);
            if (selectSingleNode != null)
                selectSingleNode.InnerText = content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath">/bookstore/book[price>35.00]</param>
        /// <returns></returns>
        public string GetFooterNodeValue(string xpath)
        {
            string strValue = string.Empty;
            XmlNode root = ObjXmlDoc.DocumentElement;

            if (root != null)
            {
                XmlNodeList nodeList = root.SelectNodes(xpath);
                //nodeList[nodeList.Count-1]
                //Change the price on the books.
                if (nodeList != null && nodeList.Count > 0)
                {
                    strValue = nodeList[nodeList.Count - 1].InnerText;
                }
            }

            return strValue;
        }



        /// <summary>
        /// 保存XML文件
        /// </summary>
        public void Save()
        {
            try
            {
                ObjXmlDoc.Save(StrXmlFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ObjXmlDoc = null;
        }

        /*
        public IList<DataBindXmlDoc> Items = new List<DataBindXmlDoc>();
        public IList<DataBindXmlDoc> GetIList(string cateID) //CateID为栏目ID
        {
            Items.Clear();
            string strDataBindPath = Tools.GetAppSettings("DataBindPath").IndexOf("~", StringComparison.Ordinal) >= 0 ? System.Web.HttpContext.Current.Server.MapPath(Tools.GetAppSettings("DataBindPath")) : Tools.GetAppSettings("DataBindPath");

            if (File.Exists(strDataBindPath))
            {
                var xmlDoc = new XmlHelper(strDataBindPath);
                Loop("0", cateID, 1, xmlDoc);//递归子类ParentID=0从顶部开始递归
            }
            return Items;
        }
        protected void Loop(string parentID, string cateID, int level, XmlHelper xmlDoc) //level层数
        {
            XmlNode root = xmlDoc.ObjXmlDoc.DocumentElement;
            if (root != null)
            {
                XmlNodeList nodeList = root.SelectNodes(string.Format("/DataBind/ListItem[ParentID='{0}' and DataBindCateID='{1}']", parentID, cateID));
                if (nodeList != null)
                    foreach (XmlNode xn in nodeList)
                    {
                        var xee = xn as XmlElement;
                        if (null == xee)
                            continue;
                        var item = new DataBindXmlDoc
                                       {
                                           DataTextField =
                                               Getleve(level) + xee.GetElementsByTagName("DataTextField")[0].InnerText,
                                           DataValueField = xee.GetElementsByTagName("DataValueField")[0].InnerText,
                                           DataColorField = xee.GetElementsByTagName("DataColorField")[0].InnerText,
                                           ListItemID = xee.GetElementsByTagName("ListItemID")[0].InnerText
                                       };
                        Items.Add(item);
                        Loop(xee.GetElementsByTagName("ListItemID")[0].InnerText, cateID, (level + 1), xmlDoc);
                    }
            }
        }

        public string Getleve(int level)
        {
            string rev = string.Empty;
            for (int i = 1; i < level; i++)
            {
                rev += System.Web.HttpContext.Current.Server.HtmlDecode("&nbsp;&nbsp;");
            }
            rev += "├";
            return rev;
        }
        */

    }


    [Serializable]
    public class DataBindXmlDoc
    {
        private string _mDataValueField;
        /// <summary>
        /// 用于值的字段
        /// </summary>
        public string DataValueField
        {
            get { return _mDataValueField; }
            set { _mDataValueField = value; }
        }

        private string _mDataTextField;
        /// <summary>
        /// 用于显示的字段
        /// </summary>
        public string DataTextField
        {
            get { return _mDataTextField; }
            set { _mDataTextField = value; }
        }

        private string _mDataColorField;
        /// <summary>
        /// 用于显示的字段的颜色
        /// </summary>
        public string DataColorField
        {
            get { return _mDataColorField; }
            set { _mDataColorField = value; }
        }

        public string ListItemID { get; set; }
    }
}
