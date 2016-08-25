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
 */

using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    /// <summary>
    /// SSL Server Socket Wrapper Class
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TTLSServerSocket : TServerTransport
    {
        /// <summary>
        /// Underlying tcp server
        /// </summary>
        private TcpListener _server;

        /// <summary>
        /// The port where the socket listen
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Timeout for the created server socket
        /// </summary>
        private readonly int _clientTimeout = 0;

        /// <summary>
        /// Whether or not to wrap new TSocket connections in buffers
        /// </summary>
        private readonly bool _useBufferedSockets;

        /// <summary>
        /// The servercertificate with the private- and public-key
        /// </summary>
        private readonly X509Certificate _serverCertificate;

        /// <summary>
        /// The function to validate the client certificate.
        /// </summary>
        private readonly RemoteCertificateValidationCallback _clientCertValidator;

        /// <summary>
        /// The function to determine which certificate to use.
        /// </summary>
        private readonly LocalCertificateSelectionCallback _localCertificateSelectionCallback;

        /// <summary>
        /// The SslProtocols value that represents the protocol used for authentication.
        /// </summary>
        private readonly SslProtocols _sslProtocols;

        /// <summary>
        /// Initializes a new instance of the <see cref="TTLSServerSocket" /> class.
        /// </summary>
        /// <param name="port">The port where the server runs.</param>
        /// <param name="certificate">The certificate object.</param>
        public TTLSServerSocket(int port, X509Certificate2 certificate)
            : this(port, 0, certificate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TTLSServerSocket" /> class.
        /// </summary>
        /// <param name="port">The port where the server runs.</param>
        /// <param name="clientTimeout">Send/receive timeout.</param>
        /// <param name="certificate">The certificate object.</param>
        public TTLSServerSocket(int port, int clientTimeout, X509Certificate2 certificate)
            : this(port, clientTimeout, false, certificate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TTLSServerSocket" /> class.
        /// </summary>
        /// <param name="port">The port where the server runs.</param>
        /// <param name="clientTimeout">Send/receive timeout.</param>
        /// <param name="useBufferedSockets">If set to <c>true</c> [use buffered sockets].</param>
        /// <param name="certificate">The certificate object.</param>
        /// <param name="clientCertValidator">The certificate validator.</param>
        /// <param name="localCertificateSelectionCallback">The callback to select which certificate to use.</param>
        /// <param name="sslProtocols">The SslProtocols value that represents the protocol used for authentication.</param>
        public TTLSServerSocket(
            int port,
            int clientTimeout,
            bool useBufferedSockets,
            X509Certificate2 certificate,
            RemoteCertificateValidationCallback clientCertValidator = null,
            LocalCertificateSelectionCallback localCertificateSelectionCallback = null,
            // TODO: Enable Tls1 and Tls2 (TLS 1.1 and 1.2) by default once we start using .NET 4.5+.
            SslProtocols sslProtocols = SslProtocols.Tls)
        {
            if (!certificate.HasPrivateKey)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, "Your server-certificate needs to have a private key");
            }

            _port = port;
            _serverCertificate = certificate;
            _useBufferedSockets = useBufferedSockets;
            _clientCertValidator = clientCertValidator;
            _localCertificateSelectionCallback = localCertificateSelectionCallback;
            _sslProtocols = sslProtocols;

            try
            {
                // Create server socket
                _server = new TcpListener(IPAddress.Any, _port);
                _server.Server.NoDelay = true;
            }
            catch (Exception)
            {
                _server = null;
                throw new TTransportException("Could not create ServerSocket on port " + port + ".");
            }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public override void Listen()
        {
            // Make sure accept is not blocking
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

        public override bool IsClientPending()
        {
            throw new NotImplementedException();
        }

        protected override async Task<TTransport> AcceptImplementationAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TTransport>(cancellationToken);
            }

            if (_server == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No underlying server socket.");
            }

            try
            {
                var client = await _server.AcceptTcpClientAsync();
                client.SendTimeout = client.ReceiveTimeout = _clientTimeout;

                //wrap the client in an SSL Socket passing in the SSL cert
                var tTlsSocket = new TTLSSocket(client, _serverCertificate, true, _clientCertValidator, _localCertificateSelectionCallback, _sslProtocols);

                await tTlsSocket.SetupTlsAsync();

                if (_useBufferedSockets)
                {
                    var trans = new TBufferedTransport(tTlsSocket);
                    return trans;
                }

                return tTlsSocket;
            }
            catch (Exception ex)
            {
                throw new TTransportException(ex.ToString());
            }
        }

        /// <summary>
        /// Stops the Server
        /// </summary>
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