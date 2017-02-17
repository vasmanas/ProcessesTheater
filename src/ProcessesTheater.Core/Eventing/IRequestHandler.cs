namespace ProcessesTheater.Core.Eventing
{
    /// <summary>
    /// Interface for handling request.
    /// </summary>
    /// <typeparam name="TRequest"> Request type. </typeparam>
    /// <typeparam name="TResponse"> Response type. </typeparam>
    public interface IRequestHandler<in TRequest, out TResponse> where TRequest : Request<TResponse>
    {
        /// <summary>
        /// Handle request and return response.
        /// </summary>
        /// <param name="request"> Request. </param>
        /// <returns> Response. </returns>
        TResponse Handle(TRequest request);
    }
}