using DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IRegistrationService
    {
        Task<LoginResponse> Login(LoginRequest model);
        Task<RegistrationResponse> Register(RegistrationRequest model);
        string GenerateStringOTP();
       
        string GenerateRefreshToken();
    }
}