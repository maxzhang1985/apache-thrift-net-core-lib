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

using System.IO.Pipes;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TNamedPipeClientTransport : TTransport
    {
        private NamedPipeClientStream _client;
        private readonly string _serverName;
        private readonly string _pipeName;

        public TNamedPipeClientTransport(string pipe)
        {
            _serverName = ".";
            _pipeName = pipe;
        }

        public TNamedPipeClientTransport(string server, string pipe)
        {
            _serverName = string.IsNullOrWhiteSpace(server) ? server : ".";
            _pipeName = pipe;
        }

        public override bool IsOpen => _client != null && _client.IsConnected;

        public override void Open()
        {
            if (IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen);
            }
            _client = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut, PipeOptions.None);
            _client.Connect();
        }

        public override void Close()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }

        public override int Read(byte[] buf, int off, int len)
        {
            if (_client == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            }

            return _client.Read(buf, off, len);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            if (_client == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            }

            _client.Write(buf, off, len);
        }

        protected override void Dispose(bool disposing)
        {
            _client.Dispose();
        }
    }
}

