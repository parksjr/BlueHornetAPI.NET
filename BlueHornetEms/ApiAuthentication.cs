using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmsLibrary.BlueHornetEms
{
    public enum ApiResponseType { XML, PHP }

    public class ApiAuthentication
    {
        private string _apiKey = "";
        private string _apiSecret = "";
        private ApiResponseType _responseType = ApiResponseType.XML;
        private bool _noHalt = false;

        public string ApiKey
        {
            get { return _apiKey; }
            private set { _apiKey = value; }
        }
        public string ApiSecret
        {
            get { return _apiSecret; }
            private set { _apiSecret = value; }
        }
        public ApiResponseType ResponseType
        {
            get { return _responseType; }
            private set { _responseType = value; }
        }
        public bool NoHalt
        {
            get { return _noHalt; }
            private set { _noHalt = value; }
        }
        public override string ToString()
        {
            return String.Format("<authentication><api_key><![CDATA[{0}]]></api_key><shared_secret><![CDATA[{1}]]></shared_secret><response_type><![CDATA[{2}]]></response_type><no_halt><![CDATA[{3}]]></no_halt></authentication>", _apiKey, _apiSecret, _responseType.ToString().ToLower(), (_noHalt ? "1" : "0"));
            //return base.ToString();
        }

        public static ApiAuthentication CreateAuthenticationObject(string key, string secret, ApiResponseType responseType, bool noHalt)
        {
            ApiAuthentication output = new ApiAuthentication();
            output.ApiKey = key;
            output.ApiSecret = secret;
            output.ResponseType = responseType;
            output.NoHalt = noHalt;
            return output;
        }
    }
}
