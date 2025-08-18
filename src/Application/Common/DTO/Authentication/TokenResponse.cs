using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTO.Authentication
{
    public class TokenResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
