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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    //TODO: check for correct work

    /// <summary>
    /// SSL Socket Wrapper class
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TTLSSocket : TStreamTransport
    {
        /// <summary>
        /// Internal TCP Client
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The host
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// The port
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The timeout for the connection
        /// </summary>
        private int _timeout;

        /// <summary>
        /// Internal SSL Stream for IO
        /// </summary>
        private SslStream _secureStream;

        /// <summary>
        /// Defines wheter or not this socket is a server socket<br/>
        /// This is used for the TLS-authentication
        /// </summary>
        private readonly bool _isServer;

        /// <summary>
        /// The certificate
        /// </summary>
        private readonly X509Certificate _certificate;

        /// <summary>
        /// User defined certificate validator.
        /// </summary>
        private readonly RemoteCertificateValidationCallback _certValidator;

        /// <summary>
        /// The function to determine which certificate to use.
        /// </summary>
        private readonly LocalCertificateSelectionCallback _localCertificateSelectionCallback;

        /// <summary>
        /// The SslProtocols value that represents the protocol used for authentication.SSL protocols to be used.
        /// </summary>
        private readonly SslProtocols _sslProtocols;

        public TTLSSocket(TcpClient client, X509Certificate certificate, bool isServer = false,
            RemoteCertificateValidationCallback certValidator = null,
            LocalCertificateSelectionCallback localCertificateSelectionCallback = null,
            // TODO: Enable Tls11 and Tls12 (TLS 1.1 and 1.2) by default once we start using .NET 4.5+.
            SslProtocols sslProtocols = SslProtocols.Tls)
        {
            _client = client;
            _certificate = certificate;
            _certValidator = certValidator;
            _localCertificateSelectionCallback = localCertificateSelectionCallback;
            _sslProtocols = sslProtocols;
            _isServer = isServer;
            if (isServer && certificate == null)
            {
                throw new ArgumentException("TTLSSocket needs certificate to be used for server", "certificate");
            }

            if (IsOpen)
            {
                InputStream = client.GetStream();
                OutputStream = client.GetStream();
            }
        }

        public TTLSSocket(string host, int port, string certificatePath,
            RemoteCertificateValidationCallback certValidator = null,
            LocalCertificateSelectionCallback localCertificateSelectionCallback = null,
            SslProtocols sslProtocols = SslProtocols.Tls)
            : this(
                host, port, 0, new X509Certificate(certificatePath), certValidator, localCertificateSelectionCallback,
                sslProtocols)
        {
        }

        public TTLSSocket(string host, int port, X509Certificate certificate = null,
            RemoteCertificateValidationCallback certValidator = null,
            LocalCertificateSelectionCallback localCertificateSelectionCallback = null,
            SslProtocols sslProtocols = SslProtocols.Tls)
            : this(host, port, 0, certificate, certValidator, localCertificateSelectionCallback, sslProtocols)
        {
        }

        public TTLSSocket(string host, int port, int timeout, X509Certificate certificate,
            RemoteCertificateValidationCallback certValidator = null,
            LocalCertificateSelectionCallback localCertificateSelectionCallback = null,
            SslProtocols sslProtocols = SslProtocols.Tls)
        {
            _host = host;
            _port = port;
            _timeout = timeout;
            _certificate = certificate;
            _certValidator = certValidator;
            _localCertificateSelectionCallback = localCertificateSelectionCallback;
            _sslProtocols = sslProtocols;

            InitSocket();
        }

        private void InitSocket()
        {
            _client = new TcpClient();
            _client.ReceiveTimeout = _client.SendTimeout = _timeout;
            _client.Client.NoDelay = true;
        }

        /// <summary>
        /// Sets Send / Recv Timeout for IO
        /// </summary>
        public int Timeout
        {
            set { _client.ReceiveTimeout = _client.SendTimeout = _timeout = value; }
        }

        /// <summary>
        /// Gets the TCP client.
        /// </summary>
        public TcpClient TcpClient => _client;

        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host => _host;

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port => _port;

        /// <summary>
        /// Gets a value indicating whether TCP Client is Cpen
        /// </summary>
        public override bool IsOpen
        {
            get
            {
                if (_client == null)
                {
                    return false;
                }

                return _client.Connected;
            }
        }

        private bool DefaultCertificateValidator(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslValidationErrors)
        {
            return sslValidationErrors == SslPolicyErrors.None;
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            }

            if (string.IsNullOrEmpty(_host))
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open null host");
            }

            if (_port <= 0)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open without port");
            }

            if (_client == null)
            {
                InitSocket();
            }

            //TODO: check logic
            if (_client != null)
            {
                await _client.ConnectAsync(_host, _port);

                await SetupTlsAsync();
            }
        }

        public async Task SetupTlsAsync()
        {
            var validator = _certValidator ?? DefaultCertificateValidator;

            if (_localCertificateSelectionCallback != null)
            {
                _secureStream = new SslStream(_client.GetStream(), false, validator, _localCertificateSelectionCallback);
            }
            else
            {
                _secureStream = new SslStream(_client.GetStream(), false, validator);
            }

            try
            {
                if (_isServer)
                {
                    // Server authentication
                    await
                        _secureStream.AuthenticateAsServerAsync(_certificate, _certValidator != null, _sslProtocols,
                            true);
                }
                else
                {
                    // Client authentication
                    var certs = _certificate != null
                        ? new X509CertificateCollection {_certificate}
                        : new X509CertificateCollection();

                    await _secureStream.AuthenticateAsClientAsync(_host, certs, _sslProtocols, true);
                }
            }
            catch (Exception)
            {
                Close();
                throw;
            }

            InputStream = _secureStream;
            OutputStream = _secureStream;
        }

        /// <summary>
        /// Closes the SSL Socket
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

            if (_secureStream != null)
            {
                _secureStream.Dispose();
                _secureStream = null;
            }
        }
    }
}