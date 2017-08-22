using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using AWSLambda1.Models.Requests;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using AWSLambda1.Models.Requests.RequestTypes;
using AWSLambda1.Models.Responses;
using Newtonsoft.Json.Linq;
using RestSharp;

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
        public string Version { get; set; } = "1.0";

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
                    response = IntentRequestHandler(input.Request, input.Session.User.AccessToken);
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

        private SkillResponse IntentRequestHandler(RequestBundle request, string accessToken)
        {
            SkillResponse response = null;

            switch (request.Intent.Name)
            {
                case "HelloIntent":
                    response = HelloIntentHandler(request);
                    break;
                case "TalkToProductOwner":
                    response = HelloIntentHandler(request);
                    break;
                case "GetCurrentBill":
                    response = GetCurrentBillHandler(request, accessToken);
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

        private SkillResponse HelpIntent(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "<speak>You can say, <prosody rate=\"slow\">'Whats my bill' or 'Pay my bill' or 'Talk to the Product owner'</prosody></speak>";

            SsmlOutputSpeech innerResponse = new SsmlOutputSpeech()
            {
                Ssml = speech
            };

            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
        }

        private SkillResponse StopIntentHandler(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "Thank You for visiting Avista Skill. Come back soon. Bye now";


            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech { Text = speech };
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse,
                ShouldEndSession = true
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
        }

        private SkillResponse CancelOrStopIntentHandler(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "Operation Cancelled. You can say Help or continue to issue commands to Road Star.";


            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech { Text = speech };
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
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

        private SkillResponse TalkToProductOwnerHandler(RequestBundle request)
        {
            SkillResponse skillResponse = new SkillResponse();

            var speech = "If you need any new features for this skill, you got the right person. Too bad Amy is in a meeting right now. Bye Now.";

            PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech { Text = speech };
            Response innerResponseresponse = new Response
            {
                OutputSpeech = innerResponse,
                ShouldEndSession = true
            };
            skillResponse.Response = innerResponseresponse;

            return skillResponse;
        }

        public SkillResponse GetCurrentBillHandler(RequestBundle request, string accessToken)
        {
            SkillResponse response = new SkillResponse();
            string pathSeg = "http://gatewayapimock20170817012815.azurewebsites.net/api/BillingDetails/3";
            //string strJsonContent = JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var restRequest = new RestRequest( Method.GET);

            var restResponse = CallService(pathSeg, restRequest, null, true); var speech = "Hi and Welcome to Avista Voice App. Please issue your voice commands or say Help to get a list.";

            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech {Text = $"Sorry, something went wrong {restResponse.StatusCode}"};
                Response innerResponseresponse = new Response
                {
                    OutputSpeech = innerResponse
                };
                response.Response = innerResponseresponse;
            }
            else
            {
                var response1 = JObject.Parse(restResponse.Content).ToObject<GetAccountDetailsResponse>();

                PlainTextOutputSpeech innerResponse = new PlainTextOutputSpeech
                {
                    Text = $" Hi {response1.Name}, you account balance is {response1.CurrentBalance}. Access Token is {accessToken}"
                };
                Response innerResponseresponse = new Response
                {
                    OutputSpeech = innerResponse
                };
                response.Response = innerResponseresponse;
            }
            return response;
        }
        #endregion

        private RestResponse CallService(string url, RestRequest request, string jsonContent, bool addParameter)
        {
            RestResponse restResponse = new RestResponse();
            request.Credentials = CredentialCache.DefaultCredentials; 

            if (addParameter)
            {
                request.AddParameter("application/json", jsonContent, ParameterType.RequestBody);
            }
            var handler = new HttpClientHandler { UseDefaultCredentials = true };

            var client = new RestClient(url);
            
            Task.Run(async () =>
            {
                restResponse = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();
            return restResponse;
        }
        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }

        private string AddSSML(string text)
        {

            string _text = string.Empty;

            _text = "<speak>" + text + "</speak>";

            return _text;

        }
    }
}
