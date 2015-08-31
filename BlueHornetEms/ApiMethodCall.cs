using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmsLibrary.BlueHornetEms
{
    public class ApiMethodCall
    {
        private const string _methodCallWrapper = "<methodCall>{0}</methodCall>";
        private string _methodName = "";
        private Dictionary<string, object> _methodArguments = new Dictionary<string, object>();

        public string MethodName
        {
            get { return _methodName; }
            private set { _methodName = value; }
        }
        public Dictionary<string, object> MethodArguments
        {
            get { return _methodArguments; }
            private set { _methodArguments = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            //  build data to xml string
            {
                sb.AppendFormat("<methodName><![CDATA[{0}]]></methodName>", _methodName);

                string methodArgMeat = RecurseArgValues(_methodArguments);
                sb.Append(methodArgMeat);

                return String.Format(_methodCallWrapper, sb.ToString());
            }
            //return base.ToString();
        }
        public void AddArg(string key, object val)
        {
            _methodArguments.Add(key, val);
        }

        public static ApiMethodCall CreateMethodCall(string name)
        {
            return CreateMethodCall(name, new Dictionary<string, object>());
        }
        public static ApiMethodCall CreateMethodCall(string name, Dictionary<string, object> arguments)
        {
            ApiMethodCall output = new ApiMethodCall();
            output.MethodName = name;
            output.MethodArguments = arguments;
            return output;
        }

        private string RecurseArgValues(Dictionary<string, object> argCollection)
        {
            StringBuilder sb = new StringBuilder("");

            foreach (KeyValuePair<string, object> arg in argCollection)
            {
                string key = arg.Key;

                if (arg.Value is string)
                {
                    sb.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", arg.Key, Uri.EscapeUriString(arg.Value.ToString()));
                }
                else if (arg.Value is Dictionary<string, object>)
                {
                    Dictionary<string, object> argValues = (Dictionary<string, object>)arg.Value;
                    sb.AppendFormat("<{0}>{1}</{0}>", arg.Key, RecurseArgValues(argValues));
                }
                else if (arg.Value is List<Dictionary<string, object>>)
                {
                    List<Dictionary<string, object>> dList = (List<Dictionary<string, object>>)arg.Value;
                    sb.AppendFormat("<{0}>", arg.Key);
                    foreach (Dictionary<string, object> obj in dList)
                    {
                        sb.Append(RecurseArgValues(obj));
                    }
                    sb.AppendFormat("</{0}>", arg.Key);
                }
            }
            return sb.ToString();
        }
    }
}
