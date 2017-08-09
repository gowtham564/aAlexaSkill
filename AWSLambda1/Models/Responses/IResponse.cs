using Newtonsoft.Json;

namespace AWSLambda1.Models.Responses
{
    public interface IResponse
    {
        [JsonRequired]
        string Type { get; }
    }
}
