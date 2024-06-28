using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE172266.ProductManagement.Repo.Model
{
    public class ErrorResponse
    {
        public List<Error> Errors { get; set; }
    }

    public class Error
    {   
        public string Message { get; set; }
    }
}
