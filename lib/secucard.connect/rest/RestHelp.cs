﻿namespace secucard.connect.rest
{
    using System.Text;

    public static class RestHelp
    {
        public static byte[] ToUTF8Bytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}