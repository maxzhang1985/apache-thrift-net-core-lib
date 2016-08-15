/**
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
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */

using System;
using Microsoft.Extensions.Logging;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift.Server
{
    // ReSharper disable once InconsistentNaming
    public abstract class TServer
    {
        protected TProcessorFactory ProcessorFactory;
        protected TServerTransport ServerTransport;
        protected TTransportFactory InputTransportFactory;
        protected TTransportFactory OutputTransportFactory;
        protected TProtocolFactory InputProtocolFactory;
        protected TProtocolFactory OutputProtocolFactory;
        protected TServerEventHandler ServerEventHandler;
        protected readonly ILogger Logger;

        public void setEventHandler(TServerEventHandler seh)
        {
            ServerEventHandler = seh;
        }

        public TServerEventHandler getEventHandler()
        {
            return ServerEventHandler;
        }

        protected TServer(TProcessor processor, TServerTransport serverTransport, ILoggerFactory loggerFactory)
            : this(processor, serverTransport,
                new TTransportFactory(),
                new TTransportFactory(),
                new TBinaryProtocol.Factory(),
                new TBinaryProtocol.Factory(),
                loggerFactory.CreateLogger<TServer>())
        {
        }

        protected TServer(TProcessor processor, TServerTransport serverTransport, ILogger logger)
            : this(processor,
                serverTransport,
                new TTransportFactory(),
                new TTransportFactory(),
                new TBinaryProtocol.Factory(),
                new TBinaryProtocol.Factory(),
                logger)
        {
        }

        protected TServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory transportFactory,
            ILogger logger)
            : this(processor,
                serverTransport,
                transportFactory,
                transportFactory,
                new TBinaryProtocol.Factory(),
                new TBinaryProtocol.Factory(),
                logger)
        {
        }

        protected TServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory transportFactory,
            TProtocolFactory protocolFactory, ILogger logger)
            : this(processor,
                serverTransport,
                transportFactory,
                transportFactory,
                protocolFactory,
                protocolFactory,
                logger)
        {
        }

        protected TServer(TProcessor processor,
            TServerTransport serverTransport,
            TTransportFactory inputTransportFactory,
            TTransportFactory outputTransportFactory,
            TProtocolFactory inputProtocolFactory,
            TProtocolFactory outputProtocolFactory,
            ILogger logger)
        {
            ProcessorFactory = new TSingletonProcessorFactory(processor);
            ServerTransport = serverTransport;
            InputTransportFactory = inputTransportFactory;
            OutputTransportFactory = outputTransportFactory;
            InputProtocolFactory = inputProtocolFactory;
            OutputProtocolFactory = outputProtocolFactory;
            Logger = logger;
        }

        protected TServer(TProcessorFactory processorFactory,
            TServerTransport serverTransport,
            TTransportFactory inputTransportFactory,
            TTransportFactory outputTransportFactory,
            TProtocolFactory inputProtocolFactory,
            TProtocolFactory outputProtocolFactory,
            ILogger logger)
        {
            ProcessorFactory = processorFactory;
            ServerTransport = serverTransport;
            InputTransportFactory = inputTransportFactory;
            OutputTransportFactory = outputTransportFactory;
            InputProtocolFactory = inputProtocolFactory;
            OutputProtocolFactory = outputProtocolFactory;
            Logger = logger;
        }

        public abstract void Serve();
        public abstract void Stop();
    }
}