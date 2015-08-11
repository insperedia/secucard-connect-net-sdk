﻿namespace Secucard.Connect.Auth
{
    using Secucard.Connect.Auth.Model;
    using Secucard.Connect.Net.Rest;

    public class RestAuth : RestBase
    {
        public const string DEVICE = "device";
        public const string RREFRESHTOKEN = "refresh_token";
        public const string CLIENTCREDENTIALS = "client_credentials";

        private readonly AuthConfig AuthConfig;

        public RestAuth(AuthConfig authConfig)
            : base(authConfig.OAuthUrl)
        {
            AuthConfig = authConfig;
        }

        public string UserAgentInfo { get; set; }

        public DeviceAuthCode GetDeviceAuthCode(string clientId, string clientSecret)
        {
            var req = new RestRequest
            {
                Host = AuthConfig.Host,
                UserAgent = UserAgentInfo
            };

            req.BodyParameter.Add(AuthConst.Grant_Type, DEVICE);
            req.BodyParameter.Add(AuthConst.Client_Id, clientId);
            req.BodyParameter.Add(AuthConst.Client_Secret, clientSecret);
            req.BodyParameter.Add(AuthConst.Uuid, AuthConfig.Uuid);

            var deviceAuthCode = RestPost<DeviceAuthCode>(req);
            return deviceAuthCode;
        }

        public Token GetToken(string clientId, string clientSecret)
        {
            var req = new RestRequest
            {
                Host = AuthConfig.Host,
                UserAgent = UserAgentInfo
            };

            req.BodyParameter.Add(AuthConst.Grant_Type, CLIENTCREDENTIALS);
            req.BodyParameter.Add(AuthConst.Client_Id, clientId);
            req.BodyParameter.Add(AuthConst.Client_Secret, clientSecret);

            var userAuthtoken = RestPost<Token>(req);
            return userAuthtoken;
        }

        public Token ObtainAuthToken(string deviceCode, string clientId, string clientSecret)
        {
            var req = new RestRequest
            {
                Host = AuthConfig.Host,
                UserAgent = UserAgentInfo
            };

            req.BodyParameter.Add(AuthConst.Grant_Type, DEVICE);
            req.BodyParameter.Add(AuthConst.Client_Id, clientId);
            req.BodyParameter.Add(AuthConst.Client_Secret, clientSecret);
            req.BodyParameter.Add(AuthConst.Code, deviceCode);

            try
            {
                return RestPost<Token>(req);
            }
            catch (RestException ex)
            {
                // ignore 401 Error and return null
                if (ex.StatusCode == 401) return null;
                throw;
            }
        }

        public Token RefreshToken(string refreshToken, string clientId, string clientSecret)
        {
            var req = new RestRequest
            {
                Host = AuthConfig.Host,
                UserAgent = UserAgentInfo
            };
            req.BodyParameter.Add(AuthConst.Grant_Type, RREFRESHTOKEN);
            req.BodyParameter.Add(AuthConst.Client_Id, clientId);
            req.BodyParameter.Add(AuthConst.Client_Secret, clientSecret);
            req.BodyParameter.Add(AuthConst.RefreshToken, refreshToken);

            var token = RestPost<Token>(req);
            return token;
        }
    }
}