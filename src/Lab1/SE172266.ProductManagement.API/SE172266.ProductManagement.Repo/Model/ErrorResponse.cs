using System.Collections.Generic;

namespace SE172266.ProductManagement.Repo.Model
{
    public class ErrorResponse
    {
        public List<Error> Errors { get; set; } = new List<Error>();
        public int StatusCode { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
    }
}
