﻿namespace Secucard.Connect.Test.Client
{
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Secucard.Connect.Auth;
    using Secucard.Connect.Client;
    using Secucard.Connect.Net.Rest;
    using Secucard.Connect.Net.Util;
    using Secucard.Connect.Product.Smart.Model;
    using Secucard.Connect.Storage;
    using Secucard.Connect.Trace;

    public class Test_Client_Base
    {
        protected const string logPath = "data\\secucard.cliend.log";
        protected const string storagePath = "data\\secucard.sec";
        private readonly ClientConfiguration ClientConfigurationDevice;
        private readonly ClientConfiguration ClientConfigurationUser;
        private readonly MemoryDataStorage Storage;
        protected readonly ISecucardTrace Tracer;

        protected SecucardConnect Client;

        protected Test_Client_Base()
        {
            Tracer = new SecucardTraceMemory();
            Storage = new MemoryDataStorage();

            ClientConfigurationDevice = ClientConfiguration.Get(); // Load Defaults
            ClientConfigurationDevice.ClientAuthDetails = new ClientAuthDetailsDeviceTest();
            ClientConfigurationDevice.SecucardTrace = Tracer;
            ClientConfigurationDevice.DataStorage = Storage;

            //ClientConfigurationDevice = new ClientConfiguration
            //{
            //    CacheDir = null,
            //    DefaultChannel = "REST",
            //    AuthConfig = new AuthConfig
            //    {
            //        Host = "core-dev10.secupay-ag.de",
            //        AuthType = AuthTypeEnum.Device,
            //        OAuthUrl = "https://core-dev10.secupay-ag.de/app.core.connector/oauth/token",
            //        AuthWaitTimeoutSec = 240,
            //        Uuid = "/vendor/unknown/cashier/dotnettest1"
            //    },
            //    StompConfig = new StompConfig
            //    {
            //        Host = "dev10.secupay-ag.de",
            //        Port = 61614,
            //        Login = "v7ad2eejbgt135q6v47vehopg7",
            //        Password = "v7ad2eejbgt135q6v47vehopg7",
            //        AcceptVersion = "1.2",
            //        HeartbeatClientMs = 5000,
            //        HeartbeatServerMs = 5000,
            //        MaxMessageAgeSec = 10,
            //        MessageTimeoutSec=10,
            //        ReplyTo = "/temp-queue/main",
            //        Destination = "/exchange/connect.api/",
            //        ReceiptTimeoutSec = 10,
            //        RequestSENDReceipt = true,
            //        DisconnectOnSENDReceiptTimeout =10,
            //        DisconnectOnError = true,
            //        Ssl = true
            //    },
            //    RestConfig = new RestConfig
            //    {
            //        Host = "core-dev10.secupay-ag.de",
            //        BaseUrl = "https://core-dev10.secupay-ag.de/app.core.connector/api/v2/"
            //    },
            //    ClientAuthDetails = new ClientAuthDetailsDeviceTest(),
            //    SecucardTrace = Tracer,
            //    DataStorage = Storage
            //};



            ClientConfigurationUser = ClientConfiguration.Get();
                        ClientConfigurationUser.ClientAuthDetails = new ClientAuthDetailsUserTest();
            ClientConfigurationUser.SecucardTrace = Tracer;
            ClientConfigurationUser.DataStorage = Storage;
            //{
            //    CacheDir = null,
            //    DefaultChannel = "REST",
            //    AuthConfig = new AuthConfig
            //    {
            //        Host = "core-dev10.secupay-ag.de",
            //        AuthType = AuthTypeEnum.User,
            //        OAuthUrl = "https://core-dev10.secupay-ag.de/app.core.connector/oauth/token",
            //        AuthWaitTimeoutSec = 240,
            //        Uuid = "/vendor/unknown/cashier/dotnettest1"
            //    },
            //    StompConfig = new StompConfig
            //    {
            //        Host = "dev10.secupay-ag.de",
            //        Port = 61614,
            //        Login = "v7ad2eejbgt135q6v47vehopg7",
            //        Password = "v7ad2eejbgt135q6v47vehopg7",
            //        AcceptVersion = "1.2",
            //        HeartbeatClientMs = 5000,
            //        HeartbeatServerMs = 5000,
            //        MaxMessageAgeSec = 10,
            //        MessageTimeoutSec = 10,
            //        ReplyTo = "/temp-queue/main",
            //        Destination = "/exchange/connect.api/",
            //        ReceiptTimeoutSec = 10,
            //        RequestSENDReceipt = true,
            //        DisconnectOnSENDReceiptTimeout = 10,
            //        DisconnectOnError = true,
            //        Ssl = true
            //    },
            //    RestConfig = new RestConfig
            //    {
            //        Host = "core-dev10.secupay-ag.de",
            //        BaseUrl = "https://core-dev10.secupay-ag.de/app.core.connector/api/v2/"
            //    },
            //    ClientAuthDetails = new ClientAuthDetailsUserTest(),
            //    SecucardTrace = Tracer,
            //    DataStorage = Storage
            //};
        }
        
        protected void StartupClientDevice()
        {
            Client = SecucardConnect.Create(ClientConfigurationDevice);
            Client.AuthEvent += ClientOnAuthEvent;
            Client.ConnectionStateChangedEvent += ClientOnConnectionStateChangedEvent;
            Client.Open();
        }

        protected void StartupClientUser()
        {
            Client = SecucardConnect.Create(ClientConfigurationUser);
            Client.AuthEvent += ClientOnAuthEvent;
            Client.ConnectionStateChangedEvent += ClientOnConnectionStateChangedEvent;
            Client.Open();
        }


        /// <summary>
        ///     Handles device authentication. Enter pin thru web interface service
        /// </summary>
        private void ClientOnAuthEvent(object sender, AuthEventArgs args)
        {
            Tracer.Info("ClientOnSecucardConnectEvent Status={0}", args.Status);

            if (args.Status == AuthStatusEnum.Pending)
            {
                // Set pin via SMART REST (only development)

                var reqSmartPin = new RestRequest
                {
                    Host = ClientConfigurationDevice.Properties.Get("Auth.Host"),
                    BodyJsonString =
                        JsonSerializer.SerializeJson(new SmartPin {UserPin = args.DeviceAuthCodes.UserCode})
                };

                reqSmartPin.Header.Add("Authorization", "Bearer p11htpu8n1c6f85d221imj8l20");
                var restSmart =
                    new RestService("https://core-dev10.secupay-ag.de/app.core.connector/api/v2/Smart/Devices/SDV_2YJDXYESB2YBHECVB5GQGSYPNM8UA6/pin");
                var response = restSmart.RestPut(reqSmartPin);
                Assert.IsTrue(response.Length > 0);
            }
        }

        /// <summary>
        ///     Handles connect and disconnect events
        /// </summary>
        private void ClientOnConnectionStateChangedEvent(object sender, ConnectionStateChangedEventArgs args)
        {
            Tracer.Info("Client Connected={0}", args.Connected);
        }


        [TestCleanup]
        public void Cleanup()
        {
            if(Client!=null) Client.Close();
            Debug.WriteLine((Tracer as SecucardTraceMemory).GetAllTrace());
        }
    }
}