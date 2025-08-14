using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class Result
    {
        public Result()
        {
            Succeeded = true;
            Errors = Array.Empty<string>();
        }

        public Result(bool succeeded, string[] errors)
        {
            Succeeded = succeeded;
            Errors = errors ?? Array.Empty<string>();
        }

        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }

    }
}
