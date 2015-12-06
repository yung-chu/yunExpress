using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace LighTake.Infrastructure.Common
{
    public static class HttpRequestsFunctions
    {
        public static string HttpPost(string uri, string parameters)
        {
            // parameters: name1=value1&name2=value2	
            WebRequest webRequest = WebRequest.Create(uri);

            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex)
            {
                throw new BusinessLogicException("HttpPost: Request error", ex.InnerException);
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            try
            { // get the response
                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                { return null; }
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                throw new BusinessLogicException("HttpPost: Response error", ex.InnerException);
            }
        } // end HttpPost 

        private static Stream getResponseStream(WebRequest request)
        {
            //grab the respons stream
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream ResponseStream = response.GetResponseStream();


            //create the response buffer
            byte[] ResponseBuffer = new byte[response.ContentLength];
            int BytesLeft = ResponseBuffer.Length;
            int TotalBytesRead = 0;
            bool EOF = false;

            //loop while not EOF
            while (!EOF)
            {
                //get the next chunk and calc the remaining bytes
                int BytesRead = ResponseStream.Read(ResponseBuffer, TotalBytesRead, BytesLeft);
                TotalBytesRead += BytesRead;
                BytesLeft -= BytesRead;

                //has EOF been reached
                EOF = (BytesLeft == 0);
            }



            ResponseStream.Close();

            //create a memory stream and pass in the respones buffer
            ResponseStream = new MemoryStream(ResponseBuffer);
            return ResponseStream;

        }

        private static XmlDocument GetResponseXml(WebRequest request)
        {
            //load the stream into an xml document
            XmlDocument ResponseDocument = new XmlDocument();
            ResponseDocument.Load(getResponseStream(request));
            return ResponseDocument;

        }

        public static string PostRetString(string url)
        {
            Stream stream = MakeRequestGet(url);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static string PostRetString(string url, XmlDocument document)
        {
            WebRequest request = MakeRequestPost(url, document);
            Stream stream = getResponseStream(request);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static Stream MakeRequestGet(string url)
        {
            WebRequest Request = WebRequest.Create(url);
            Request.Method = "GET";
            Request.ContentType = "application/x-www-form-urlencoded";

            // If required by the server, set the credentials.
            Request.Credentials = CredentialCache.DefaultCredentials;

            // Request.ContentLength = 
            HttpWebResponse response = (HttpWebResponse)Request.GetResponse();
            Stream RequestStream = response.GetResponseStream();
            return RequestStream;

            /*
            Request.Headers.Add("Authorization", string.Format("Basic {0}", authValue));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(objReader);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("original");
            //XmlNodeList nodes = doc.SelectNodes("/rss/channel/item/yweather:forecast", ns);

            // You can also get elements based on their tag name and namespace,  
            // though this isn't recommended  
            //XmlNodeList nodes = doc.GetElementsByTagName("forecast",   
            //                          "http://xml.weather.yahoo.com/ns/rss/1.0");  

            foreach (XmlNode node in nodes)
            {

                string day = node.Attributes["day"].InnerText;
                string text = node.Attributes["text"].InnerText;
                string low = node.Attributes["low"].InnerText;
                string high = node.Attributes["high"].InnerText; 
            }  
             */
        }

        private static WebRequest MakeRequestPost(string url, XmlDocument document)
        {
            ////convert the document to stream
            //MemoryStream Stream = new MemoryStream();
            //XmlWriter Writer = XmlWriter.Create(Stream);
            //document.WriteContentTo(Writer);

            ////reset the stream position
            //Stream.Position = 0;

            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(document.InnerXml);

            //create the request and set the content type and length
            WebRequest Request = WebRequest.Create(url);
            Request.Method = "POST";
            Request.ContentType = "text/xml; encoding='utf-8'";
            Request.ContentLength = bytes.Length;

            //get the request stream and post the xml
            Stream RequestStream = Request.GetRequestStream();
            RequestStream.Write(bytes, 0, (int)bytes.Length);
            return Request;
        }

        public static XmlDocument PostRetXml(string url, XmlDocument document)
        {
            WebRequest Request = MakeRequestPost(url, document);
            return GetResponseXml(Request);
        }

    }
}
