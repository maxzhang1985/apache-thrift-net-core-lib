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

using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Protocol
{
    /**
    * TMultiplexedProtocol is a protocol-independent concrete decorator that allows a Thrift
    * client to communicate with a multiplexing Thrift server, by prepending the service name
    * to the function name during function calls.
    *
    * NOTE: THIS IS NOT TO BE USED BY SERVERS.
    * On the server, use TMultiplexedProcessor to handle requests from a multiplexing client.
    *
    * This example uses a single socket transport to invoke two services:
    *
    *     TSocket transport = new TSocket("localhost", 9090);
    *     transport.open();
    *
    *     TBinaryProtocol protocol = new TBinaryProtocol(transport);
    *
    *     TMultiplexedProtocol mp = new TMultiplexedProtocol(protocol, "Calculator");
    *     Calculator.Client service = new Calculator.Client(mp);
    *
    *     TMultiplexedProtocol mp2 = new TMultiplexedProtocol(protocol, "WeatherReport");
    *     WeatherReport.Client service2 = new WeatherReport.Client(mp2);
    *
    *     System.out.println(service.add(2,2));
    *     System.out.println(service2.getTemperature());
    *
    */

    //TODO: implementation of TProtocol

    // ReSharper disable once InconsistentNaming
    public class TMultiplexedProtocol : TProtocolDecorator
    {
        /** Used to delimit the service name from the function name */
        public const string Separator = ":";

        private readonly string _serviceName;

        /**
         * Wrap the specified protocol, allowing it to be used to communicate with a
         * multiplexing server.  The <code>serviceName</code> is required as it is
         * prepended to the message header so that the multiplexing server can broker
         * the function call to the proper service.
         *
         * Args:
         *  protocol        Your communication protocol of choice, e.g. TBinaryProtocol
         *  serviceName     The service name of the service communicating via this protocol.
         */

        public TMultiplexedProtocol(TProtocol protocol, string serviceName)
            : base(protocol)
        {
            _serviceName = serviceName;
        }

        /**
         * Prepends the service name to the function name, separated by TMultiplexedProtocol.SEPARATOR.
         * Args:
         *   tMessage     The original message.
         */

        public override void WriteMessageBegin(TMessage tMessage)
        {
            switch (tMessage.Type)
            {
                case TMessageType.Call:
                case TMessageType.Oneway:
                    base.WriteMessageBegin(new TMessage($"{_serviceName}{Separator}{tMessage.Name}", tMessage.Type, tMessage.SeqID));
                    break;
                default:
                    base.WriteMessageBegin(tMessage);
                    break;
            }
        }

        public override Task WriteMessageBeginAsync(TMessage message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteMessageEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteStructBeginAsync(TStruct struc, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteStructEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteFieldBeginAsync(TField field, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteFieldEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteFieldStopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteMapBeginAsync(TMap map, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteMapEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteListBeginAsync(TList list, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteListEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteSetBeginAsync(TSet set, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteSetEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteBoolAsync(bool b, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteByteAsync(sbyte b, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteI16Async(short i16, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteI32Async(int i32, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteI64Async(long i64, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteDoubleAsync(double d, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteBinaryAsync(byte[] b, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TMessage> ReadMessageBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadMessageEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TStruct> ReadStructBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadStructEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TField> ReadFieldBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadFieldEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TMap> ReadMapBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadMapEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TList> ReadListBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadListEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<TSet> ReadSetBeginAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task ReadSetEndAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> ReadBoolAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<sbyte> ReadByteAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<short> ReadI16Async(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<int> ReadI32Async(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<long> ReadI64Async(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<double> ReadDoubleAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task<byte[]> ReadBinaryAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}