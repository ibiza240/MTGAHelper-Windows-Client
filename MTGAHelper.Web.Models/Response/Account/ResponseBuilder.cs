namespace MTGAHelper.Web.Models.Response.Account
{
    public enum ResponseStatusEnum
    {
        Success,
        Error,
    }

    public class ResponseBase
    {
        public string ResponseStatus { get; set; } = ResponseStatusEnum.Success.ToString();
        public string Message { get; set; }

        public ResponseBase()
        {
        }
    }

    public class ResponseBuilder<TResponse> where TResponse : ResponseBase, new()
    {
        protected TResponse response = new TResponse();

        public ResponseBuilder()
        {
        }

        public ResponseBuilder(TResponse response)
        {
            this.response = response;
        }

        public ResponseBuilder<TResponse> WithError(string message)
        {
            response.ResponseStatus = ResponseStatusEnum.Error.ToString();
            response.Message = message;
            return this;
        }

        public ResponseBuilder<TResponse> WithMessage(string message)
        {
            response.Message = message;
            return this;
        }

        public TResponse Build() => response;
    }
}
