﻿/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 *
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class THttpClient : TTransport
    {
        private readonly Uri _uri;
        private readonly X509Certificate[] _certificates;
        private Stream _inputStream;
        private MemoryStream _outputStream = new MemoryStream();

        // Timeouts in milliseconds
        private int _connectTimeout = 30000;

        public THttpClient(Uri u)
            : this(u, Enumerable.Empty<X509Certificate>())
        {
        }

        public THttpClient(Uri u, IEnumerable<X509Certificate> certificates)
        {
            _uri = u;
            _certificates = (certificates ?? Enumerable.Empty<X509Certificate>()).ToArray();
        }

        public int ConnectTimeout
        {
            set { _connectTimeout = value; }
        }

        public IDictionary<string, string> CustomHeaders { get; } = new ConcurrentDictionary<string, string>();

        public override bool IsOpen => true;

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void Close()
        {
            if (_inputStream != null)
            {
                _inputStream.Dispose();
                _inputStream = null;
            }
            if (_outputStream != null)
            {
                _outputStream.Dispose();
                _outputStream = null;
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<int>(cancellationToken);
            }

            if (_inputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No request has been sent");
            }

            try
            {
                var ret = await _inputStream.ReadAsync(buffer, offset, length, cancellationToken);

                if (ret == -1)
                {
                    throw new TTransportException(TTransportException.ExceptionType.EndOfFile, "No more data available");
                }

                return ret;
            }
            catch (IOException iox)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, iox.ToString());
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            await _outputStream.WriteAsync(buffer, offset, length,cancellationToken);
        }

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.AddRange(_certificates);

            var httpClient = new HttpClient(handler);

            if (_connectTimeout > 0)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(_connectTimeout);
            }

            //TODO: check for existing
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-thrift"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("THttpClient", "1.0.0"));

            foreach (var item in CustomHeaders)
            {
                //TODO: check for existing
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            return httpClient;
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    var httpClient = CreateClient();

                    using (var outStream = new StreamContent(_outputStream))
                    {
                        var msg = await httpClient.PostAsync(_uri, outStream, cancellationToken);
                        msg.EnsureSuccessStatusCode();

                        _inputStream.Dispose();
                        _inputStream = null;

                        _inputStream = await msg.Content.ReadAsStreamAsync();

                        if (_inputStream.CanSeek)
                        {
                            _inputStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
                catch (IOException iox)
                {
                    throw new TTransportException(TTransportException.ExceptionType.Unknown, iox.ToString());
                }
                catch (WebException wx)
                {
                    throw new TTransportException(TTransportException.ExceptionType.Unknown, "Couldn't connect to server: " + wx);
                }
            }
            finally
            {
                _outputStream = new MemoryStream();
            }
        }
        
        private bool _isDisposed;

        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _inputStream?.Dispose();
                    _outputStream?.Dispose();
                }
            }
            _isDisposed = true;
        }
    }
}