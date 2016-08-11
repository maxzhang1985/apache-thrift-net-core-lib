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
using System.Net.Sockets;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TSocket : TStreamTransport
    {
        private int _timeout;

        public TSocket(TcpClient client)
        {
            TcpClient = client;

            if (IsOpen)
            {
                inputStream = client.GetStream();
                outputStream = client.GetStream();
            }
        }

        public TSocket(string host, int port)
            : this(host, port, 0)
        {
        }

        public TSocket(string host, int port, int timeout)
        {
            Host = host;
            Port = port;
            _timeout = timeout;

            InitSocket();
        }

        private void InitSocket()
        {
            TcpClient = new TcpClient();
            TcpClient.ReceiveTimeout = TcpClient.SendTimeout = _timeout;
            TcpClient.Client.NoDelay = true;
        }

        public int Timeout
        {
            set { TcpClient.ReceiveTimeout = TcpClient.SendTimeout = _timeout = value; }
        }

        public TcpClient TcpClient { get; private set; }

        public string Host { get; }

        public int Port { get; }

        public override bool IsOpen
        {
            get
            {
                if (TcpClient == null)
                {
                    return false;
                }

                return TcpClient.Connected;
            }
        }

        public override void Open()
        {
            if (IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            }

            if (string.IsNullOrEmpty(Host))
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open null host");
            }

            if (Port <= 0)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open without port");
            }

            if (TcpClient == null)
            {
                InitSocket();
            }

            if (_timeout == 0) // no timeout -> infinite
            {
                //TODO: Async - and errors
                TcpClient?.ConnectAsync(Host, Port).Wait();
            }
            //else                        // we have a timeout -> use it
            //{
            //    var hlp = new ConnectHelper(client);
            //    IAsyncResult asyncres = client.BeginConnect(host, port, new AsyncCallback(ConnectCallback), hlp);
            //    var bConnected = asyncres.AsyncWaitHandle.WaitOne(timeout) && client.Connected;
            //    if (!bConnected)
            //    {
            //        lock (hlp.Mutex)
            //        {
            //            if( hlp.CallbackDone)
            //            {
            //                asyncres.AsyncWaitHandle.Close();
            //                client.Close();
            //            }
            //            else
            //            {
            //                hlp.DoCleanup = true;
            //                client = null;
            //            }
            //        }
            //        throw new TTransportException(TTransportException.ExceptionType.TimedOut, "Connect timed out");
            //    }
            //}

            inputStream = TcpClient?.GetStream();
            outputStream = TcpClient?.GetStream();
        }


        //static void ConnectCallback(IAsyncResult asyncres)
        //{
        //    var hlp = asyncres.AsyncState as ConnectHelper;
        //    lock (hlp.Mutex)
        //    {
        //        hlp.CallbackDone = true;

        //        try
        //        {
        //            if( hlp.Client.Client != null)
        //                hlp.Client.EndConnect(asyncres);
        //        }
        //        catch (Exception)
        //        {
        //            // catch that away
        //        }

        //        if (hlp.DoCleanup)
        //        {
        //            try {
        //                asyncres.AsyncWaitHandle.Close();
        //            } catch (Exception) {}

        //            try {
        //                if (hlp.Client is IDisposable)
        //                    ((IDisposable)hlp.Client).Dispose();
        //            } catch (Exception) {}
        //            hlp.Client = null;
        //        }
        //    }
        //}

        //private class ConnectHelper
        //{
        //    public object Mutex = new object();
        //    public bool DoCleanup = false;
        //    public bool CallbackDone = false;
        //    public TcpClient Client;
        //    public ConnectHelper(TcpClient client)
        //    {
        //        Client = client;
        //    }
        //}

        public override void Close()
        {
            base.Close();
            if (TcpClient != null)
            {
                TcpClient.Dispose();
                TcpClient = null;
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
                    ((IDisposable) TcpClient)?.Dispose();

                    base.Dispose(disposing);
                }
            }
            _isDisposed = true;
        }
    }
}