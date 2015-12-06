using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Wuyi.Common
{
    /*
     * 作者：周小强
     */
    public class HttpServer
    {
       private TcpListener myListener;

        private int Port; //端口

        private Thread th; //监听进程

        public bool IsRuning = false;

        public delegate void DelegateException(Exception er);

        public delegate void DelegateDoWork(RequestStruct ds);

        public DelegateException ExBack;

        public DelegateDoWork DoWork;

        /// <summary>
        /// HttpServerHelper类构造函数
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="doWork">HTTP请求处理方法</param>
        public HttpServer(int port, DelegateDoWork doWork)
        {
            this.Port = port;

            this.DoWork = doWork;
        }

        /// <summary>
        /// HttpServerHelper类构造函数
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="doWork">HTTP请求处理方法</param>
        /// <param name="exBack">错误回调方法</param>
        public HttpServer(int port, DelegateDoWork doWork, DelegateException exBack)
        {
            this.Port = port;

            this.DoWork = doWork;

            this.ExBack = exBack;
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
        {
            try
            {
                myListener = new TcpListener(IPAddress.Parse("0.0.0.0"), this.Port); //指定端口及IP

                this.myListener.Start(); //打开监听端口

                IsRuning = true;

                this.th = new Thread(new ThreadStart(StartListen));

                this.th.IsBackground = true;

                this.th.Start(); //启动监听线程

            }
            catch (Exception e)
            {
                ExceptionCaller(e);
            }
        }

        /// <summary>
        /// 错误回调
        /// </summary>
        /// <param name="e"></param>
        private void ExceptionCaller(Exception e)
        {
            if (ExBack != null)
                ExBack(e);
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        private void StartListen()
        {
            while (IsRuning)
            {
                try
                {
                    TcpClient client = myListener.AcceptTcpClient(); //阻塞当前线程直到接收到连接请求

                    //ThreadPool.QueueUserWorkItem(ProcessClient, client.Client);

                    Thread t = new Thread(new ParameterizedThreadStart(ProcessClient));
                    t.IsBackground = true;
                    t.Start(client.Client); //在新线程中处理该连接
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 连接具体处理方法
        /// </summary>
        /// <param name="clientObj"></param>
        private void ProcessClient(object clientObj)
        {
            Socket mySocket = clientObj as Socket;

            try
            {
                Byte[] bReceive = new Byte[1024];

                int i = mySocket.Receive(bReceive, bReceive.Length, 0);

                //转换成字符串类型
                string sBuffer = Encoding.ASCII.GetString(bReceive);

                // 查找 "HTTP" 的位置
                int iStartPos = sBuffer.IndexOf("HTTP", 1);

                if (iStartPos != -1)
                {
                    RequestStruct Struct = new RequestStruct();

                    Struct.HttpVersion = sBuffer.Substring(iStartPos, 8);

                    Struct.RequestUrl = GetRequestUrl(sBuffer);

                    Struct.RequestSocket = mySocket;

                    DoWork(Struct);
                }
                else
                {
                    mySocket.Shutdown(SocketShutdown.Both);
                    mySocket.Close();
                }
            }
            catch (Exception e)
            {
                ExceptionCaller(e);
            }
            finally
            {
                if (mySocket.Connected)
                {
                    try
                    {
                        mySocket.Shutdown(SocketShutdown.Both);
                        mySocket.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 返回HTTP请求中的URL地址
        /// </summary>
        /// <param name="RequestStr">HTTP请求字符串</param>
        /// <returns>URL地址</returns>
        private string GetRequestUrl(string RequestStr)
        {
            try
            {
                return Regex.Split(Regex.Split(RequestStr, Environment.NewLine)[0], " ")[1];
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 发送HTTP头到客户端Socket
        /// </summary>
        /// <param name="DataStruct"></param>
        protected Byte[] GetHeaderByte(ResponseStruct DataStruct)
        {
            string sBuffer = "";

            string sMIMEHeader = DataStruct.MIMEHeader;

            string sStatusCode = DataStruct.StatusCode;

            int iTotBytes = DataStruct.TotBytes;

            if (String.IsNullOrEmpty(sMIMEHeader))
            {
                sMIMEHeader = "application/x-javascript"; //默认 text/html
            }

            if (String.IsNullOrEmpty(sStatusCode))
            {
                sStatusCode = "200 OK"; //默认 200 OK
            }

            sBuffer = String.Format("{0} {1}\r\n", DataStruct.HttpVersion, sStatusCode);

            //sBuffer = String.Format("{0}Content-Type: {1};Charset=UTF-8\r\n", sBuffer, sMIMEHeader);

            sBuffer = String.Format("{0}Content-Type: {1}\r\n", sBuffer, sMIMEHeader);

            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";

            sBuffer = String.Format("{0}Content-Length: {1}\r\n\r\n", sBuffer, iTotBytes);

            Byte[] bSendData = Encoding.UTF8.GetBytes(sBuffer);

            return bSendData;

        }

        /// <summary>
        /// 发送数据到Socket
        /// </summary>
        /// <param name="DataStruct"></param>
        protected void SendToBrowser(ResponseStruct DataStruct)
        {
            Byte[] bSendData = DataStruct.ByteContent;

            Byte[] ResponseByte = CopyByte(GetHeaderByte(DataStruct), bSendData);

            DataStruct.ResponseSocket.Send(ResponseByte, ResponseByte.Length, 0);
        }

        ///<summary>
        /// 发送数据到Socket
        /// </summary>
        /// <param name="DataStruct"></param>
        /// <param name="bSendData"></param>
        public void SendToBrowser(ResponseStruct DataStruct, Byte[] bSendData)
        {
            DataStruct.ByteContent = bSendData;

            SendToBrowser(DataStruct);
        }

        /// <summary>
        /// 发送字符串到Socket
        /// </summary>
        /// <param name="DataStruct"></param>
        /// <param name="sData"></param>
        public void SendToBrowser(ResponseStruct DataStruct, String sData)
        {
            DataStruct.ByteContent = Encoding.UTF8.GetBytes(sData);

            SendToBrowser(DataStruct);
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            try
            {
                IsRuning = false;

                myListener.Stop();

                if (th != null)
                    th.Abort();

                th.Join();

            }
            catch (Exception ex)
            {
                ExceptionCaller(ex);
            }
        }

        private byte[] CopyByte(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }

    }

    public class RequestStruct
    {
        public string HttpVersion { get; set; }

        public string RequestUrl { get; set; }

        public string RequestPath
        { 
            get
            {
                if (RequestUrl.Contains("?"))
                {
                    return RequestUrl.Substring(0, RequestUrl.IndexOf("?"));
                }
                else
                {
                    return RequestUrl;
                }
            } 
        }

        public Socket RequestSocket { get; set; }

        public string RequestQuerystring(string Target)
        {
            try
            {
                string[] ArrayData = RequestUrl.Substring(RequestUrl.IndexOf("?") + 1, RequestUrl.Length - RequestUrl.IndexOf("?") - 1).Split('&');
                for (int i = 0; i < ArrayData.Length; i++)
                {
                    string[] ArrayTarget = ArrayData[i].Split('=');
                    if (ArrayTarget[0] == Target)
                        return ArrayTarget[1];
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public void Close()
        {
            RequestSocket.Shutdown(SocketShutdown.Both);
            RequestSocket.Close();
        }

    }

    public class ResponseStruct
    {
        public string HttpVersion { get; set; }

        public string MIMEHeader { get; set; }

        public string StatusCode { get; set; }

        public byte[] ByteContent{ get; set; }

        public int TotBytes{ get { return ByteContent.Length; } }

        public Socket ResponseSocket { get; set; }

        public void Close()
        {
            ResponseSocket.Shutdown(SocketShutdown.Both);
            ResponseSocket.Close();
        }
    }
}