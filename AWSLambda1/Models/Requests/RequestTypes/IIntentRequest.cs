namespace AWSLambda1.Models.Requests.RequestTypes
{
    public interface IIntentRequest : IRequest
    {
        /// <summary>
        /// An object that represents what the user wants.
        /// </summary>
        Intent Intent { get; set; }
    }
}