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


/**
 * Abstract implementation which just delegates the token persistence to a file based cache.
 */

using System;
using Secucard.Connect.auth.Model;

namespace Secucard.Connect.auth
{
    using Secucard.Connect.Auth;
    using Secucard.Connect.Storage;

    public abstract class AbstractClientAuthDetails  {
        private DataStorage storage;

        public AbstractClientAuthDetails() {
            storage = new MemoryDataStorage();
        }

        public Token GetCurrent() {
            return (Token) storage.Get("token");
        }

        public void OnTokenChanged(Token token) {
            storage.Save("token", token);
        }
    }
}