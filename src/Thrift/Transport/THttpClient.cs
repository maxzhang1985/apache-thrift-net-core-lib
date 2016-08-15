/**
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

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

        private int _readTimeout = 30000;

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

        public int ReadTimeout
        {
            set { _readTimeout = value; }
        }

        public IDictionary<string, string> CustomHeaders { get; } = new ConcurrentDictionary<string, string>();

        public override bool IsOpen => true;

        public override void Open()
        {
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

        public override int Read(byte[] buf, int off, int len)
        {
            if (_inputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No request has been sent");
            }

            try
            {
                var ret = _inputStream.Read(buf, off, len);

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

        public override void Write(byte[] buf, int off, int len)
        {
            _outputStream.Write(buf, off, len);
        }

        public override void Flush()
        {
            try
            {
                SendRequest();
            }
            finally
            {
                _outputStream = new MemoryStream();
            }
        }

        private void SendRequest()
        {
            try
            {
                var httpClient = CreateClient();

                //var data = outputStream.ToArray();
                //connection.ContentLength = data.Length;

                using (var outStream = new StreamContent(_outputStream))
                {
                    var msg = httpClient.PostAsync(_uri, outStream).Result;
                    msg.EnsureSuccessStatusCode();

                    //TODO: clean stream
                    _inputStream.Dispose();
                    _inputStream = msg.Content.ReadAsStreamAsync().Result;
                    if (_inputStream.CanSeek) _inputStream.Seek(0, SeekOrigin.Begin);
                }

                //using (Stream requestStream = connection.GetRequestStream())
                //{
                //    requestStream.Write(data, 0, data.Length);

                //    // Resolve HTTP hang that can happens after successive calls by making sure
                //    // that we release the response and response stream. To support this, we copy
                //    // the response to a memory stream.
                //    using (var response = connection.GetResponse())
                //    {
                //        using (var responseStream = response.GetResponseStream())
                //        {
                //            // Copy the response to a memory stream so that we can
                //            // cleanly close the response and response stream.
                //            inputStream = new MemoryStream();
                //            var buffer = new byte[8096];
                //            int bytesRead;
                //            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                //            {
                //                inputStream.Write(buffer, 0, bytesRead);
                //            }
                //            inputStream.Seek(0, 0);
                //        }
                //    }
                //}
            }
            catch (IOException iox)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown, iox.ToString());
            }
            catch (Exception wx)
            {
                throw new TTransportException(TTransportException.ExceptionType.Unknown,
                    "Couldn't connect to server: " + wx);
            }
            //catch (WebException wx)
            //{
            //    throw new TTransportException(TTransportException.ExceptionType.Unknown, "Couldn't connect to server: " + wx);
            //}
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

            //TODO: HttpClient ReadWriteTimeout
            //if (readTimeout > 0)
            //{
            //    connection.ReadWriteTimeout = readTimeout;
            //}

            //TODO: check for existing
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-thrift"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("C#/THttpClient"));


            // Make the request
            //connection.ContentType = "application/x-thrift";
            //connection.Accept = "application/x-thrift";
            //connection.UserAgent = "C#/THttpClient";
            //connection.Method = "POST";

            //connection.ProtocolVersion = HttpVersion.Version10;

            //add custom headers here
            foreach (var item in CustomHeaders)
            {
                //TODO: check for existing
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                //connection.Headers.Add(item.Key, item.Value);
            }

            //connection.Proxy = proxy;

            return httpClient;
        }

        //public override IAsyncResult BeginFlush(AsyncCallback callback, object state)
        //{
        //    // Extract request and reset buffer
        //    var data = outputStream.ToArray();

        //    //requestBuffer_ = new MemoryStream();

        //    try
        //    {
        //        // Create connection object
        //        var flushAsyncResult = new FlushAsyncResult(callback, state);
        //        flushAsyncResult.Connection = CreateClient();

        //        flushAsyncResult.Data = data;


        //        flushAsyncResult.Connection.BeginGetRequestStream(GetRequestStreamCallback, flushAsyncResult);
        //        return flushAsyncResult;

        //    }
        //    catch (IOException iox)
        //    {
        //        throw new TTransportException(iox.ToString());
        //    }
        //}

        public override void EndFlush(IAsyncResult asyncResult)
        {
            try
            {
                var flushAsyncResult = (FlushAsyncResult) asyncResult;

                if (!flushAsyncResult.IsCompleted)
                {
                    var waitHandle = flushAsyncResult.AsyncWaitHandle;
                    waitHandle.WaitOne(); // blocking INFINITEly
                    waitHandle.Dispose();
                }

                if (flushAsyncResult.AsyncException != null)
                {
                    throw flushAsyncResult.AsyncException;
                }
            }
            finally
            {
                _outputStream = new MemoryStream();
            }
        }

        //private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        //{
        //    var flushAsyncResult = (FlushAsyncResult)asynchronousResult.AsyncState;
        //    try
        //    {
        //        var reqStream = flushAsyncResult.Connection.EndGetRequestStream(asynchronousResult);
        //        reqStream.Write(flushAsyncResult.Data, 0, flushAsyncResult.Data.Length);
        //        reqStream.Flush();
        //        reqStream.Close();

        //        // Start the asynchronous operation to get the response
        //        flushAsyncResult.Connection.BeginGetResponse(GetResponseCallback, flushAsyncResult);
        //    }
        //    catch (Exception exception)
        //    {
        //        flushAsyncResult.AsyncException = new TTransportException(exception.ToString());
        //        flushAsyncResult.UpdateStatusToComplete();
        //        flushAsyncResult.NotifyCallbackWhenAvailable();
        //    }
        //}

        //private void GetResponseCallback(IAsyncResult asynchronousResult)
        //{
        //    var flushAsyncResult = (FlushAsyncResult)asynchronousResult.AsyncState;
        //    try
        //    {
        //        inputStream = flushAsyncResult.Connection.EndGetResponse(asynchronousResult).GetResponseStream();
        //    }
        //    catch (Exception exception)
        //    {
        //        flushAsyncResult.AsyncException = new TTransportException(exception.ToString());
        //    }
        //    flushAsyncResult.UpdateStatusToComplete();
        //    flushAsyncResult.NotifyCallbackWhenAvailable();
        //}

        // Based on http://msmvps.com/blogs/luisabreu/archive/2009/06/15/multithreading-implementing-the-iasyncresult-interface.aspx
        class FlushAsyncResult : IAsyncResult
        {
            private volatile bool _isCompleted;
            private ManualResetEvent _evt;
            private readonly AsyncCallback _cbMethod;

            public FlushAsyncResult(AsyncCallback cbMethod, object state)
            {
                _cbMethod = cbMethod;
                AsyncState = state;
            }

            internal byte[] Data { get; set; }
            internal HttpClient Connection { get; set; }
            internal TTransportException AsyncException { get; set; }

            public object AsyncState { get; }

            public WaitHandle AsyncWaitHandle => GetEvtHandle();

            public bool CompletedSynchronously => false;

            public bool IsCompleted => _isCompleted;

            private readonly object _locker = new object();

            private ManualResetEvent GetEvtHandle()
            {
                lock (_locker)
                {
                    if (_evt == null)
                    {
                        _evt = new ManualResetEvent(false);
                    }
                    if (_isCompleted)
                    {
                        _evt.Set();
                    }
                }
                return _evt;
            }

            internal void UpdateStatusToComplete()
            {
                _isCompleted = true; //1. set _iscompleted to true
                lock (_locker)
                {
                    if (_evt != null)
                    {
                        _evt.Set(); //2. set the event, when it exists
                    }
                }
            }

            internal void NotifyCallbackWhenAvailable()
            {
                if (_cbMethod != null)
                {
                    _cbMethod(this);
                }
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