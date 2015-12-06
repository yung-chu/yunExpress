using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SsoFramework
{
    [Serializable]
    public class ResponseToken
    {
        private string _userId;
        private DateTime _timeStamp;
        private int _expireDuration;
        private string _seed;
        private int _resultCode;

        public ResponseToken()
        {
        }

        public ResponseToken(string userId, DateTime timeStamp, int expireDuration, string seed, int resultCode)
        {
            _userId = userId;
            _timeStamp = timeStamp;
            _expireDuration = expireDuration;
            _seed = seed;
            _resultCode = resultCode;
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public int ExpireDuration
        {
            get { return _expireDuration; }
            set { _expireDuration = value; }
        }

        public string Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public int ResultCode
        {
            get { return _resultCode; }
            set { _resultCode = value; }
        }

        public string Encode()
        {
            string encodedText = SsoCipher.EncryptVector
                (_userId,
                _timeStamp == default(DateTime) ? null : _timeStamp.ToString(),
                _expireDuration.ToString(),
                _seed,
                _resultCode.ToString());
            return HttpUtility.UrlEncode(encodedText);
        }

        public override string ToString()
        {
            return Encode();
        }

        public static bool TryParse(string tokenText, out ResponseToken token)
        {
            token = null;
            if (string.IsNullOrEmpty(tokenText))
                return false;
            string textToDecypt = HttpUtility.UrlDecode(tokenText);
            string[] vector = null;
            if (!SsoCipher.TryParseVector(textToDecypt, out vector))
                return false;
            if (vector.Length != 5)
                return false;
            string userId = vector[0];
            DateTime timeStamp = Convert.ToDateTime(vector[1]);
            int expire = Convert.ToInt32(vector[2]);
            string seed = vector[3];
            int resultCode = Convert.ToInt32(vector[4]);
            token = new ResponseToken(userId, timeStamp, expire, seed, resultCode);
            return true;
        }
    }
}
