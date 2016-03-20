/*
 * Copyright (c) 2015. hp.weber GmbH & Co secucard KG (www.secucard.com)
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Secucard.Connect.Client
{
    using System;

    /// <summary>
    /// Indicates that an authentication problem happened during an operation.
    /// Inspect the actual exception type to get details about the cause. Some type are recoverable just by correcting user input.
    /// </summary>
    public class AuthError : Exception
    {
        public AuthError()
        {
        }

        public AuthError(string message)
            : base(message)
        {
        }

        public AuthError(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}