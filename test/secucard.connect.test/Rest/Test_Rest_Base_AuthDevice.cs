﻿namespace secucard.connect.test.Rest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using secucard.connect;
    using Secucard.Connect.auth;
    using Secucard.Connect.Auth;
    using Secucard.Connect.Channel.Rest;
    using Secucard.Connect.Rest;
    using Secucard.Connect.Storage;
    using Secucard.Connect.Test;
    using Secucard.Connect.Trace;
    using Secucard.Model;
    using Secucard.Model.Auth;
    using Secucard.Model.Smart;

    [TestClass]
    [DeploymentItem("Data", "Data")]
    public class Test_Rest_Base_AuthDevice : Test_Base
    {
        protected readonly AuthToken Token;
        protected readonly RestService RestService;

        public  Test_Rest_Base_AuthDevice()
        {
            ConfigAuth = new AuthConfig
            {
                Host = host,
                AuthType = AuthTypeEnum.Device,
                OAuthUrl = "https://core-dev10.secupay-ag.de/app.core.connector/oauth/token",
                WaitTimeoutSec = 240,
                Uuid = "/vendor/unknown/cashier/dotnettest1",
                ClientCredentials =
                    new ClientCredentials("611c00ec6b2be6c77c2338774f50040b",
                        "dc1f422dde755f0b1c4ac04e7efbd6c4c78870691fe783266d7d6c89439925eb")
            };

            Tracer = new SecucardTraceFile(logPath);
            Storage = MemoryDataStorage.LoadFromFile(storagePath);


            var authProvider = new AuthProvider("testprovider", ConfigAuth, Tracer, Storage);
            authProvider.AuthProviderStatusUpdate += AuthProviderOnAuthProviderStatusUpdate;
            Token = authProvider.GetToken(true);
            Storage.SaveToFile(storagePath); // Save new token 

            RestService = new RestService(new RestConfig { BaseUrl = "https://core-dev10.secupay-ag.de/app.core.connector/api/v2/" });

        }

        private void AuthProviderOnAuthProviderStatusUpdate(object sender, AuthProviderStatusUpdateEventArgs args)
        {
            if (args.Status == AuthProviderStatusEnum.Pending)
            {
                // Set pin via SMART REST (only development)

                var reqSmartPin = new RestRequest
                {
                    Host = ConfigAuth.Host,
                    BodyJsonString =
                        JsonSerializer.SerializeJson(new SmartPin { UserPin = args.DeviceAuthCodes.UserCode })
                };

                reqSmartPin.Header.Add("Authorization", "Bearer p11htpu8n1c6f85d221imj8l20");
                var restSmart = new RestService(new RestConfig { BaseUrl = "https://core-dev10.secupay-ag.de/app.core.connector/api/v2/Smart/Devices/SDV_2YJDXYESB2YBHECVB5GQGSYPNM8UA6/pin" });
                var response = restSmart.RestPut(reqSmartPin);
                Assert.IsTrue(response.Length > 0);
            }
        }
    }
}