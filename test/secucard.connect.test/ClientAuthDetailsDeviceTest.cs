﻿namespace secucard.connect.test
{
    using Secucard.Connect.auth;
    using Secucard.Connect.auth.Model;
    using Secucard.Connect.Auth;

    public class ClientAuthDetailsDeviceTest: AbstractClientAuthDetails,IClientAuthDetails
    {
        public OAuthCredentials GetCredentials()
        {
            return new DeviceCredentials("611c00ec6b2be6c77c2338774f50040b", "dc1f422dde755f0b1c4ac04e7efbd6c4c78870691fe783266d7d6c89439925eb", "device");
        }

        public ClientCredentials GetClientCredentials()
        {
            return (ClientCredentials)this.GetCredentials();
        }
    }
}