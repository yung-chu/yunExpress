using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common.Utities
{
    public class TreeNodeHelper
    {
        /// <summary>
        /// 生成一个根节点的树
        /// </summary>
        /// <param name="nodeList">节点列表，包含未连接的树节点，节点中给出id,pid,text字段</param>
        /// <returns></returns>
        public TreeNode GenerateTreeRoot(List<TreeNode> nodeList)
        {
            TreeNode root = new TreeNode();
            TreeNode cNode;
            TreeNode chNode;
            TreeNode pNode;
            Stack<TreeNode> stack = new Stack<TreeNode>();
            while (nodeList.Count > 0)
            {
                cNode = nodeList[0];
                nodeList.Remove(cNode);
                stack.Push(cNode);
                while (cNode != null)
                {
                    cNode = stack.Pop();
                    if ((chNode = getChildren(cNode, nodeList)) != null)
                    {
                        stack.Push(cNode);
                        nodeList.Remove(chNode);
                        stack.Push(chNode);
                    }
                    else
                    {
                        if (stack.Count > 0)
                        {
                            pNode = stack.Pop();
                            pNode.Children.Add(cNode);
                            stack.Push(pNode);
                        }
                        else
                        {
                            if ((pNode = getParent(cNode, nodeList)) != null)
                            {
                                nodeList.Remove(pNode);
                                stack.Push(pNode);
                                pNode.Children.Add(cNode);
                            }
                            else
                            {
                                root.Children.Add(cNode);
                                cNode = null;
                            }
                        }
                    }
                }
            }
            return root;
        }

        public TreeNode getChildren(TreeNode node, List<TreeNode> list)
        {
            return list.Find(delegate(TreeNode n) { return n.Pid == node.Id; });
        }
        public TreeNode getParent(TreeNode node, List<TreeNode> list)
        {
            return list.Find(delegate(TreeNode n) { return n.Id == node.Pid; });
        }
    }
    public class TreeNode
    {
        public TreeNode()
        {
            m_Id = String.Empty;
            m_Pid = String.Empty;
            m_Text = String.Empty;
            m_Children = new List<TreeNode>();
        }

        public TreeNode(string id, string pid, string text)
        {
            m_Id = id;
            m_Pid = pid;
            m_Text = text;
            m_Children = new List<TreeNode>();
        }

        private string m_Id;
        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private string m_Pid;
        public string Pid
        {
            get { return m_Pid; }
            set { m_Pid = value; }
        }

        private string m_Text;
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
        private List<TreeNode> m_Children;
        public List<TreeNode> Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }
        public bool HasChildren
        {
            get
            {
                if (this.Children != null)
                    return m_Children.Count > 0 ? true : false;
                else
                    return false;
            }
        }

        /// <summary>
        /// 生成根节点的json格式字符串
        /// </summary>
        /// <returns></returns>
        public string ToJsonTreeString()
        {
            if (!this.HasChildren)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (TreeNode node in this.Children)
            {
                sb.Append("{");
                sb.Append("\"id\":\"");
                sb.Append(node.Id);
                sb.Append("\",\"text\":\"");
                sb.Append(node.Text);
                sb.Append("\"");
                //有孩子节点时添加children字段,否则令leaf字段为true
                if (node.HasChildren)
                {
                    sb.Append(",");
                    sb.Append("\"children\":");
                    sb.Append(node.ToJsonTreeString());
                }
                sb.Append("},");
            }
            //去掉最后一个逗号
            if (this.Children.Count > 0)
                sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
