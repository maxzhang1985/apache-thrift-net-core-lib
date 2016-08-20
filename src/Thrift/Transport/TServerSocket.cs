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
using System.Net;
using System.Net.Sockets;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TServerSocket : TServerTransport
    {
        private TcpListener _server;
        private readonly int _port;
        private readonly int _clientTimeout;
        private readonly bool _useBufferedSockets;

        public TServerSocket(TcpListener listener)
            : this(listener, 0)
        {
        }

        public TServerSocket(TcpListener listener, int clientTimeout)
        {
            _server = listener;
            _clientTimeout = clientTimeout;
        }

        public TServerSocket(int port)
            : this(port, 0)
        {
        }

        public TServerSocket(int port, int clientTimeout)
            : this(port, clientTimeout, false)
        {
        }

        public TServerSocket(int port, int clientTimeout, bool useBufferedSockets)
        {
            _port = port;
            _clientTimeout = clientTimeout;
            _useBufferedSockets = useBufferedSockets;
            try
            {
                // Make server socket
                _server = new TcpListener(IPAddress.Any, _port);
                _server.Server.NoDelay = true;
            }
            catch (Exception)
            {
                _server = null;
                throw new TTransportException("Could not create ServerSocket on port " + port + ".");
            }
        }

        public override void Listen()
        {
            // Make sure not to block on accept
            if (_server != null)
            {
                try
                {
                    _server.Start();
                }
                catch (SocketException sx)
                {
                    throw new TTransportException("Could not accept on listening socket: " + sx.Message);
                }
            }
        }

        protected override TTransport AcceptImpl()
        {
            if (_server == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No underlying server socket.");
            }

            try
            {
                TSocket result2 = null;
                //TODO: Async
                var result = _server.AcceptTcpClientAsync().Result;

                try
                {
                    result2 = new TSocket(result)
                    {
                        Timeout = _clientTimeout
                    };

                    if (_useBufferedSockets)
                    {
                        var result3 = new TBufferedTransport(result2);
                        return result3;
                    }

                    return result2;
                }
                catch (Exception)
                {
                    // If a TSocket was successfully created, then let
                    // it do proper cleanup of the TcpClient object.
                    if (result2 != null)
                    {
                        result2.Dispose();
                    }
                    else //  Otherwise, clean it up ourselves.
                    {
                        ((IDisposable) result).Dispose();
                    }

                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new TTransportException(ex.ToString());
            }
        }

        public override void Close()
        {
            if (_server != null)
            {
                try
                {
                    _server.Stop();
                }
                catch (Exception ex)
                {
                    throw new TTransportException("WARNING: Could not close server socket: " + ex);
                }
                _server = null;
            }
        }
    }
}