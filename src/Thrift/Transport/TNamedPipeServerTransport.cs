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
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TNamedPipeServerTransport : TServerTransport
    {
        /// <summary>
        /// This is the address of the Pipe on the localhost.
        /// </summary>
        private readonly string _pipeAddress;

        private NamedPipeServerStream _stream = null;
        private bool _asyncMode = true;

        public TNamedPipeServerTransport(string pipeAddress)
        {
            _pipeAddress = pipeAddress;
        }

        public override void Listen()
        {
            // nothing to do here
        }

        public override void Close()
        {
            if (_stream != null)
            {
                try
                {
                    //TODO: check for disconection 
                    _stream.Disconnect();
                    _stream.Dispose();
                }
                finally
                {
                    _stream = null;
                }
            }
        }

        private void EnsurePipeInstance()
        {
            if (_stream == null)
            {
                var direction = PipeDirection.InOut;
                var maxconn = 254;
                var mode = PipeTransmissionMode.Byte;
                var options = _asyncMode ? PipeOptions.Asynchronous : PipeOptions.None;
                var inbuf = 4096;
                var outbuf = 4096;
                // TODO: security

                try
                {
                    _stream = new NamedPipeServerStream(_pipeAddress, direction, maxconn, mode, options, inbuf, outbuf);
                }
                catch (NotImplementedException) // Mono still does not support async, fallback to sync
                {
                    if (_asyncMode)
                    {
                        options &= (~PipeOptions.Asynchronous);
                        _stream = new NamedPipeServerStream(_pipeAddress, direction, maxconn, mode, options, inbuf,
                            outbuf);
                        _asyncMode = false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        protected override TTransport AcceptImpl()
        {
            try
            {
                EnsurePipeInstance();

                if (_asyncMode)
                {

                    //TODO: test async
                    _stream.WaitForConnectionAsync(CancellationToken.None).GetAwaiter().GetResult();
                }
                else
                {
                    _stream.WaitForConnection();
                }

                var trans = new ServerTransport(_stream, _asyncMode);
                _stream = null; // pass ownership to ServerTransport
                return trans;
            }
            catch (TTransportException)
            {
                Close();
                throw;
            }
            catch (Exception e)
            {
                Close();
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, e.Message);
            }
        }

        private class ServerTransport : TTransport
        {
            private readonly NamedPipeServerStream _stream;
            private readonly bool _asyncMode;

            public ServerTransport(NamedPipeServerStream stream, bool asyncMode)
            {
                _stream = stream;
                _asyncMode = asyncMode;
            }

            public override bool IsOpen => _stream != null && _stream.IsConnected;

            public override void Open()
            {
            }

            public override void Close()
            {
                _stream?.Dispose();
            }

            public override int Read(byte[] buffer, int offset, int length)
            {
                if (_stream == null)
                {
                    throw new TTransportException(TTransportException.ExceptionType.NotOpen);
                }

                if (_asyncMode)
                {
                    var retval = 0;

                    retval = _stream.ReadAsync(buffer, offset, length).ConfigureAwait(false).GetAwaiter().GetResult();

                    return retval;
                }

                return _stream.Read(buffer, offset, length);
            }

            public override void Write(byte[] buffer, int offset, int length)
            {
                if (_stream == null)
                {
                    throw new TTransportException(TTransportException.ExceptionType.NotOpen);
                }

                if (_asyncMode)
                {
                    _stream.WriteAsync(buffer, offset, length).GetAwaiter().GetResult();
                }
                else
                {
                    _stream.Write(buffer, offset, length);
                }
            }

            protected override void Dispose(bool disposing)
            {
                _stream?.Dispose();
            }
        }
    }
}