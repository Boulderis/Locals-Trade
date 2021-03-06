﻿using System;
using System.Net.Http;
using Xamarin.Forms;

namespace MSupportYourLocals.Services
{
    public abstract class Service
    {

        protected HttpClient httpClient;

        protected JsonWebTokenHolder tokenService = DependencyService.Get<JsonWebTokenHolder>();

        protected Service()
        {
            httpClient = MakeHttpClient();
        }

        private HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }

        public HttpClient MakeHttpClient()
        {
#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            HttpClient httpClient = new HttpClient(insecureHandler);
#else
            HttpClient httpClient = new HttpClient();
#endif
            httpClient.BaseAddress = new Uri("https://sylrestapi.azurewebsites.net/");
            // httpClient.DefaultRequestHeaders.Accept.Clear();
            // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

    }
}
