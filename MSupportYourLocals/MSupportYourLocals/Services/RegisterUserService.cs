﻿using System.Net.Http;
using System.Threading.Tasks;
using MSupportYourLocals.Models;

namespace MSupportYourLocals.Services
{
    public class RegisterUserService : Service, IRegisterUserService
    {

        public async Task Register(UserBindingTarget target)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/User/SignUp", target);
        }

    }
}
