﻿namespace secucard.stomp
{
    using System;

    public class StompClientFrameArrivedEventArgs : EventArgs
    {
        public StompFrame Frame { get; set; }
        public DateTime Time { get; set; }
    }
}