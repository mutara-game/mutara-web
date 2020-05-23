namespace Mutara.Web.Api
{
    public class ApiOkResponse<T> : ApiResponse
    {
        public T Content { get; } = default!;

        public ApiOkResponse() : base(200)
        {
        }

        public ApiOkResponse(T content) : base(200)
        {
            this.Content = content;
        }
    }
}