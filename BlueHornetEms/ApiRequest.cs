using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace EmsLibrary.BlueHornetEms
{
    public class ApiRequest
    {
        private const string _apiRequestWrapper = "<api>{0}<data>{1}</data></api>";
        private string _bhApiUrl = "";
        private ApiAuthentication _authentication = new ApiAuthentication();
        private List<ApiMethodCall> _methodCalls = new List<ApiMethodCall>();
        private string _lastEventMessage = "";

        public string ApiUrl
        {
            get { return _bhApiUrl; }
            private set { _bhApiUrl = value; }
        }
        public ApiAuthentication Authentication
        {
            get { return _authentication; }
            private set { _authentication = value; }
        }
        public List<ApiMethodCall> MethodCalls
        {
            get { return _methodCalls; }
            private set { _methodCalls = value; }
        }
        public string LastEventMessage
        {
            get { return _lastEventMessage; }
        }

        public void AddMethodCall(ApiMethodCall methodCall)
        {
            _methodCalls.Add(methodCall);
        }
        public bool SendRequest()
        {
            string output = "";
            return SendRequest(out output);
        }
        public bool SendRequest(out string responseOutput)
        {
            bool output = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(_bhApiUrl));
                request.Method = "POST";
                request.ContentType = "application/xml";
                request.Accept = "application/xml";

                byte[] bytes = Encoding.UTF8.GetBytes(BuildApiData());

                request.ContentLength = bytes.Length;

                using (Stream putStream = request.GetRequestStream())
                {
                    putStream.Write(bytes, 0, bytes.Length);
                }

                // Log the response API
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string readerResponse = reader.ReadToEnd();
                    //ONPSErpIntegrationEventLog.LogInformationEvent("Response BlueHornet: " + reader.ReadToEnd(), EventLogSource);
                    //_lastEventMessage = "Response BlueHornet: " + readerResponse;
                    output = ParseResponse(readerResponse);
                    responseOutput = readerResponse;
                }
            }
            catch (Exception err)
            {
                //ONPSErpIntegrationEventLog.LogErrorEventAndNotifyTech("EmsIntegration :: SendRequest(): Something wrong with posting the HttpWebRequest. More Details: " + err.ToString(), EventLogSource);
                _lastEventMessage = "EmsIntegration :: SendRequest(): Something wrong with posting the HttpWebRequest. More Details: " + err.ToString();
                output = false;
                responseOutput = "";
            }
            return output;
        }

        private string BuildApiData()
        {
            string output = "";

            StringBuilder sbMethods = new StringBuilder("");
            foreach (ApiMethodCall method in MethodCalls)
            {
                sbMethods.Append(method.ToString());
            }
            output = String.Format(_apiRequestWrapper, _authentication.ToString(), sbMethods.ToString());

            return output;
        }

        private bool ParseResponse(string responseInput)
        {
            bool output = true;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseInput);

            string xPathErr = "methodResponse/item/error";
            XmlNode errNode = xmlDoc.SelectSingleNode(xPathErr);
            XmlNode msgNode = xmlDoc.SelectSingleNode("methodResponse/item/responseText");
            XmlNode bulkInsertMsgNode = xmlDoc.SelectSingleNode("methodResponse/item/responseData/message");

            //  error check
            if (errNode != null)
            {
                //  we have err
                string errMsge = (msgNode != null) ? msgNode.InnerText : "No error message found.";
                _lastEventMessage = "BlueHornet API response Error: " + msgNode.InnerText + "\n\r Whole Response: \n\r" + responseInput + "\n\rRequestData: \n\r" + BuildApiData();
                output = false;
            }
            else if (bulkInsertMsgNode != null)
            {
                // if 2 then error
                if (bulkInsertMsgNode.InnerText.Trim() == "2")
                {
                    string errReason = xmlDoc.SelectSingleNode("methodResponse/item/responseData/reason") != null ? xmlDoc.SelectSingleNode("methodResponse/item/responseData/reason").InnerText : "No error message found.";
                    _lastEventMessage = "BlueHornet API response Error: " + errReason + "\n\r Whole Response: \n\r" + responseInput + "\n\rRequestData: \n\r" + BuildApiData();
                    output = false;
                }
                else // 1 is success
                {
                    _lastEventMessage = "Successful BlueHornet API response information: " + responseInput;
                    output = true;
                }
            }
            else
            {
                //  no err
                _lastEventMessage = "Successful BlueHornet API response information: " + responseInput;
                output = true;
            }


            return output;
        }

        public static ApiRequest CreateRequest(string apiUrl, ApiAuthentication authentication)
        {
            return CreateRequest(apiUrl, authentication, new List<ApiMethodCall>());
        }
        public static ApiRequest CreateRequest(string apiUrl, ApiAuthentication authentication, List<ApiMethodCall> methodCalls)
        {
            ApiRequest output = new ApiRequest();
            output.ApiUrl = apiUrl;
            output.Authentication = authentication;
            output.MethodCalls = methodCalls;
            return output;
        }
    }
}
