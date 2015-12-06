//@Copy right Nick Miao 2007. Email: yufengmiao@yahoo.com  MSN: yufeng_miao@hotmail.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Web.Security;


namespace SsoFramework
{
    /// <summary>
    /// The Single sign-on engine
    /// </summary>
    public class SsoEngine
    {
        internal static class Constants
        {
            internal const string TokenCookieName = "Sso_Token";
            internal const string AuthUrl = "Auth_Url";
            internal const string RequestToken = "RequestToken";
            internal const string ResponseToken = "ResponseToken";
            internal const string SeedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        }

        private static string Seed
        {
            get
            { return Session["Soo_Seed"] == null ? string.Empty : Session["Soo_Seed"] as string; }
            set { Session["Soo_Seed"] = value; }
        }

        static SsoEngine()
        {
            Seed = NextSeed();
        }

        protected static HttpResponse Response
        {
            get { return HttpContext.Current.Response; }
        }

        protected static HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        protected static HttpSessionStateBase Session
        {
            get { return new HttpSessionStateWrapper(HttpContext.Current.Session); }
        }

        public static bool IsAuthenticated
        {
            get
            {
                return GetAuthenticatedTicket() != null;
            }
        }

        public static bool IsAuthenticating
        {
            get
            {
                return !string.IsNullOrEmpty(Request[Constants.ResponseToken]);
            }
        }

        public static bool Authenticate(out ResponseToken token)
        {
            string tokenText = Request[Constants.ResponseToken];
            if (!ResponseToken.TryParse(tokenText, out token))
                return false;
            if (token.Seed != Seed)
                return false;
            return true;
        }
        public static SsoTicket GetAuthenticatedTicket()
        {
            HttpCookie cookie = Request.Cookies[Constants.TokenCookieName];
            if (cookie == null)
                return null;
            string cookieText = cookie.Value;
            if (string.IsNullOrEmpty(cookieText))
                return null;
            SsoTicket ticket;
            if (!SsoTicket.TryParse(cookieText, out ticket))
                return null;
            return ticket;
        }

        public static RequestToken AuthenticateRequest
        {
            get
            {
                string tokenText = Request[Constants.RequestToken];
                if (string.IsNullOrEmpty(tokenText))
                    return null;
                RequestToken token;
                if (!RequestToken.TryParse(tokenText, out token))
                    return null;
                return token;
            }
        }

        //public static void RedirectToLogon()
        //{
        //    string returnUrl = HttpContext.Current.Request.Url.ToString();
        //    RequestToken token = new RequestToken(
        //        returnUrl,
        //        DateTime.Now,
        //        NextSeed());
        //    string authUrl = ConfigurationManager.AppSettings[Constants.AuthUrl];
        //    string redirectUrl = string.Format("{0}?{1}={2}", authUrl, Constants.RequestToken, token.Encode());
        //    Response.Redirect(redirectUrl);
        //}

        //public static void RedirectToSetTicket()
        //{
        //    string returnUrl = HttpContext.Current.Request.Url.ToString();
        //    RequestToken token = new RequestToken(
        //        returnUrl,
        //        DateTime.Now,
        //        NextSeed());
        //    string authUrl = ConfigurationManager.AppSettings[Constants.AuthUrl];
        //    string redirectUrl = string.Format("{0}?{1}={2}&&{3}={4}", authUrl, Constants.RequestToken, token.Encode(), "Action", "SetTicket");
        //    Response.Redirect(redirectUrl);
        //}

        public static string GetRedirectToSetTicketUrl()
        {
            string returnUrl = HttpContext.Current.Request.Url.ToString();

            returnUrl = RemoveQueryString(returnUrl, Constants.ResponseToken);

            RequestToken token = new RequestToken(
                returnUrl,
                DateTime.Now,
                NextSeed());
            string authUrl = ConfigurationManager.AppSettings[Constants.AuthUrl];
            string redirectUrl = string.Format("{0}?{1}={2}&{3}={4}", authUrl, Constants.RequestToken, token.Encode(), "Action", "SetTicket");
            return redirectUrl;
        }

        public static string GetRedirectToLogonUrl()
        {
            string returnUrl = HttpContext.Current.Request.Url.ToString();
            returnUrl = RemoveQueryString(returnUrl, Constants.ResponseToken);

            RequestToken token = new RequestToken(
                returnUrl,
                DateTime.Now,
                NextSeed());
            string authUrl = ConfigurationManager.AppSettings[Constants.AuthUrl];
            string redirectUrl = string.Format("{0}?{1}={2}", authUrl, Constants.RequestToken, token.Encode());
            return redirectUrl;
        }

        public static string GetRedirectToAppUrl(string returnUrl, ResponseToken token)
        {
            char appendChar = '?';
            if (returnUrl.IndexOf('?') != -1)
                appendChar = '&';
            string redirectUrl = string.Format("{0}{1}{2}={3}", returnUrl, appendChar, Constants.ResponseToken, token.Encode());
            return redirectUrl;
        }

        public static string GetRedirectToSignOutUrl(string returnUrl)
        {
            RequestToken token = new RequestToken(
             returnUrl,
             DateTime.Now,
             NextSeed());
            string authUrl = ConfigurationManager.AppSettings[Constants.AuthUrl];
            string redirectUrl = string.Format("{0}?{1}={2}&{3}={4}", authUrl, Constants.RequestToken, token.Encode(), "Action", "Logout");
            return redirectUrl;
        }

        //public static void RedirectToApp(string returnUrl, ResponseToken token)
        //{
        //    char appendChar = '?';
        //    if (returnUrl.IndexOf('?') != -1)
        //        appendChar = '&';
        //    string redirectUrl = string.Format("{0}{1}{2}={3}", returnUrl, appendChar, Constants.ResponseToken, token.Encode());
        //    Response.Redirect(redirectUrl);
        //} 

        public static void GenerateTicket(SsoTicket ticket)
        {
            ResetCookie(Constants.TokenCookieName, ticket.Encode());
        }

        private static void ResetCookie(string cookieName, string cookieValue)
        {
            HttpContext.Current.Response.Cookies.Remove(cookieName);
            HttpContext.Current.Response.Cookies[cookieName].Value = cookieValue;
        }

        public static void SignOut()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            ResetCookie(Constants.TokenCookieName, string.Empty);
            //HttpCookie cookie = HttpContext.Current.Request.Cookies[Constants.TokenCookieName];
            //if (cookie != null)
            //{
            //    cookie.Values.Clear();
            //    cookie.Expires = DateTime.Now.AddDays(-1);
            //    HttpContext.Current.Response.Cookies.Add(cookie);
            //}
        }

        private static string NextSeed()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                int idx = random.Next(0, Constants.SeedChars.Length);
                sb.Append(Constants.SeedChars.Substring(idx, 1));
            }
            sb.Append(Guid.NewGuid().ToString());
            return Seed = SsoCipher.Hash(sb.ToString());
        }

        private static string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;

            if (queryString == null)
                queryString = string.Empty;

            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    if (dictionary.ContainsKey(queryString))
                    {
                        dictionary.Remove(queryString);
                    }

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }
    }
}
