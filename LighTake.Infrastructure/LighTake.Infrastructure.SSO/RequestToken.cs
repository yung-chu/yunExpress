//@Author: Nick Miao
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SsoFramework
{
    [Serializable]
    public class RequestToken
    {
        private string _returnUrl;
        private DateTime _timeStamp;
        private string _seed;

        public RequestToken()
        {
        }

        public RequestToken(string returnUrl, DateTime timeStamp, string seed)
        {
            _returnUrl = returnUrl;
            _timeStamp = timeStamp;
            _seed = seed;
        }

        public string ReturnUrl
        {
            get { return _returnUrl; }
            set { _returnUrl = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public string Encode()
        {
            string encodedText = SsoCipher.EncryptVector(
                _returnUrl,
                _timeStamp == default(DateTime)?null:_timeStamp.ToString(),
                _seed);
            return HttpUtility.UrlEncode(encodedText);
        }

        public override string ToString()
        {
            return Encode();
        }

        public static bool TryParse(string tokenText, out RequestToken token)
        {
            token = null;
            if(string.IsNullOrEmpty(tokenText))
                return false;
            string textToDecypt = HttpUtility.UrlDecode(tokenText);
            string[] vector = null;
            if (!SsoCipher.TryParseVector(textToDecypt, out vector))
                return false;
            if (vector.Length != 3)
                return false;
            string returnUrl = vector[0];
            DateTime timeStamp = Convert.ToDateTime(vector[1]);
            string seed = vector[2];
            token = new RequestToken(returnUrl, timeStamp, seed);
            return true;
        }
    }
}
