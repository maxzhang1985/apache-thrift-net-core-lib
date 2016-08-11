/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using Thrift.Server;
using Thrift.Transport;

namespace Thrift
{
    // ReSharper disable once InconsistentNaming
    public class TPrototypeProcessorFactory<TP, TH> : TProcessorFactory where TP : TProcessor
    {
        private readonly object[] _handlerArgs;

        public TPrototypeProcessorFactory()
        {
            _handlerArgs = new object[0];
        }

        public TPrototypeProcessorFactory(params object[] handlerArgs)
        {
            _handlerArgs = handlerArgs;
        }

        public TProcessor GetProcessor(TTransport trans, TServer server = null)
        {
            var handler = (TH) Activator.CreateInstance(typeof(TH), _handlerArgs);

            var handlerServerRef = handler as TControllingHandler;
            if (handlerServerRef != null)
            {
                handlerServerRef.Server = server;
            }
            return Activator.CreateInstance(typeof(TP), handler) as TProcessor;
        }
    }
}