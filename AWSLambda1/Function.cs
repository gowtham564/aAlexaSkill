using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Responses;

using Amazon.Lambda.Core;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda1
{
    [JsonObject]
    public class AlexaRequest
    {

        public class Context
        {
            public System System { get; set; }

        }

        public class System
        {

            public device device { get; set; }
            public string apiEndpoint { get; set; }
        }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("context")]
        public Context context { get; set; }

        [JsonProperty("session")]
        public SessionAttributes Session { get; set; }

        [JsonProperty("request")]
        public RequestAttributes Request { get; set; }

        [JsonObject("attributes")]
        public class SessionCustomAttributes
        {
            [JsonProperty("memberId")]
            public int MemberId { get; set; }
        }

        [JsonObject("session")]
        public class SessionAttributes
        {
            [JsonProperty("sessionId")]
            public string SessionId { get; set; }

            [JsonProperty("application")]
            public ApplicationAttributes Application { get; set; }

            [JsonProperty("attributes")]
            public SessionCustomAttributes Attributes { get; set; }

            [JsonProperty("user")]
            public UserAttributes User { get; set; }

            [JsonProperty("device")]

            public device device { get; set; }

            [JsonProperty("new")]
            public bool New { get; set; }

            [JsonObject("application")]
            public class ApplicationAttributes
            {
                [JsonProperty("applicationId")]
                public string ApplicationId { get; set; }
            }



            [JsonObject("user")]
            public class UserAttributes
            {
                [JsonProperty("userId")]
                public string UserId { get; set; }

                [JsonProperty("accessToken")]
                public string AccessToken { get; set; }

                [JsonProperty("permissions")]
                public Permissions Permission { get; set; }




            }
        }

        [JsonObject("device")]
        public class device
        {
            [JsonProperty("deviceId")]

            public string deviceId { get; set; }

            public SupportedInterfaces supportedInterfaces { get; set; }

        }
        public class AudioPlayer
        {
        }

        public class SupportedInterfaces
        {
            public AudioPlayer AudioPlayer { get; set; }
        }

        [JsonObject("permissions")]
        public class Permissions
        {
            [JsonProperty("consentToken")]
            public string consentToken { get; set; }
        }

        [JsonObject("request")]
        public class RequestAttributes
        {
            private string _timestampEpoch;
            private double _timestamp;

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("requestId")]
            public string RequestId { get; set; }

            [JsonProperty("timestamp")]
            public string TimestampEpoch
            {
                get
                {
                    return _timestampEpoch;
                }
                set
                {
                    _timestampEpoch = value;

                    if (Double.TryParse(value, out _timestamp) && _timestamp > 0)
                        Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(_timestamp);
                    else
                    {
                        var timeStamp = DateTime.MinValue;
                        if (DateTime.TryParse(_timestampEpoch, out timeStamp))
                            Timestamp = timeStamp.ToUniversalTime();
                    }
                }
            }

            [JsonIgnore]
            public DateTime Timestamp { get; set; }

            [JsonProperty("intent")]
            public IntentAttributes Intent { get; set; }

            [JsonProperty("reason")]
            public string Reason { get; set; }

            public RequestAttributes()
            {
                Intent = new IntentAttributes();
            }

            [JsonObject("intent")]
            public class IntentAttributes
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("slots")]
                public dynamic Slots { get; set; }

                public List<KeyValuePair<string, string>> GetSlots()
                {
                    var output = new List<KeyValuePair<string, string>>();
                    if (Slots == null) return output;

                    foreach (var slot in Slots.Children())
                    {
                        if (slot.First.value != null)
                            output.Add(new KeyValuePair<string, string>(slot.First.name.ToString(), slot.First.value.ToString()));
                    }

                    return output;
                }
            }
        }
    }
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = null;

            switch (input.Request.Type)
            {
                case "LaunchRequest":
                    response = LaunchRequestHandler(input.Request);
                    break;
                case "IntentRequest":
                    response = IntentRequestHandler(input.Request);
                    break;
                case "SessionEndedRequest":
                    response = SessionEndedRequestHandler(input.Request);
                    break;
            }

            return response;
        }

        private SkillResponse SessionEndedRequestHandler(object request)
        {
            throw new NotImplementedException();
        }

        private SkillResponse LaunchRequestHandler(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "Hi and Welcome to Avista Voice App. Please issue your voice commands or say Help to get a list.";

            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech {Text = speech};
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;

        }

        #region IntentRequestHandler

        private SkillResponse IntentRequestHandler(RequestBundle request)
        {
            SkillResponse response = null;

            switch (request.Intent.Name)
            {
                case "HelloIntent":
                    response = HelloIntentHandler(request);
                    break;
                case "GetCurrentBill":
                    response = GetCurrentBillHandler(request);
                    break;

                case "AMAZON.CancelIntent":
                    response = CancelOrStopIntentHandler(request);
                    break;
                case "AMAZON.StopIntent":
                    response = StopIntentHandler(request);
                    break;
                case "AMAZON.HelpIntent":
                case "HelpIntent":
                    response = HelpIntent(request);
                    break;
            }

            return response;
        }

        private SkillResponse GetCurrentBillHandler(RequestBundle request)
        {
            SkillResponse response = new SkillResponse();

            return response;
        }

        private SkillResponse HelpIntent(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "You can say, 'Whats my bill' or 'Pay my bill' or 'Talk to the Product owner'";

            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech { Text = speech };
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
        }

        private SkillResponse StopIntentHandler(RequestBundle request)
        {
            throw new NotImplementedException();
        }

        private SkillResponse CancelOrStopIntentHandler(RequestBundle request)
        {
            throw new NotImplementedException();
        }

        private SkillResponse HelloIntentHandler(RequestBundle request)
        {

            SkillResponse skillResponse = new SkillResponse();

            var speech = "Hi and Welcome to Avista Voice App. Please issue your voice commands or say Help to get a list.";

            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech { Text = speech };
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
        }

        #endregion

        private IRestResponse CallService(string url, IRestRequest request, string jsonContent, bool addParameter)
        {
            IRestResponse restResponse;

            if (addParameter)
            {
                request.AddParameter("application/json", jsonContent, ParameterType.RequestBody);
            }

            try
            {
                var client = new RestClient(url);
                
                restResponse = client.Execute(request);
            }
            catch (Exception ex)
            {
                throw;
            }
            return restResponse;
        }
    }
}
