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

//TODO: replacement for TThreadedServer

//using System;
//using System.Collections.Generic;
//using System.Threading;
//using Thrift.Collections;
//using Thrift.Protocol;
//using Thrift.Transport;

//namespace Thrift.Server
//{
//    /// <summary>
//    /// Server that uses C# threads (as opposed to the ThreadPool) when handling requests
//    /// </summary>
//    // ReSharper disable once InconsistentNaming
//    public class TThreadedServer : TServer
//    {
//        private const int DefaultMaxThreads = 100;
//        private volatile bool _stop;
//        private readonly int _maxThreads;

//        private readonly Queue<TTransport> _clientQueue;
//        private readonly THashSet<Thread> _clientThreads;
//        private readonly object _clientLock;
//        private Thread _workerThread;

//        public int ClientThreadsCount => _clientThreads.Count;

//        public TThreadedServer(TProcessor processor, TServerTransport serverTransport)
//            : this(new TSingletonProcessorFactory(processor), serverTransport,
//                new TTransportFactory(), new TTransportFactory(),
//                new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(),
//                DefaultMaxThreads, DefaultLogDelegate)
//        {
//        }

//        public TThreadedServer(TProcessor processor, TServerTransport serverTransport, LogDelegate logDelegate)
//            : this(new TSingletonProcessorFactory(processor), serverTransport,
//                new TTransportFactory(), new TTransportFactory(),
//                new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(),
//                DefaultMaxThreads, logDelegate)
//        {
//        }


//        public TThreadedServer(TProcessor processor,
//            TServerTransport serverTransport,
//            TTransportFactory transportFactory,
//            TProtocolFactory protocolFactory)
//            : this(new TSingletonProcessorFactory(processor), serverTransport,
//                transportFactory, transportFactory,
//                protocolFactory, protocolFactory,
//                DefaultMaxThreads, DefaultLogDelegate)
//        {
//        }

//        public TThreadedServer(TProcessorFactory processorFactory,
//            TServerTransport serverTransport,
//            TTransportFactory transportFactory,
//            TProtocolFactory protocolFactory)
//            : this(processorFactory, serverTransport,
//                transportFactory, transportFactory,
//                protocolFactory, protocolFactory,
//                DefaultMaxThreads, DefaultLogDelegate)
//        {
//        }

//        public TThreadedServer(TProcessorFactory processorFactory,
//            TServerTransport serverTransport,
//            TTransportFactory inputTransportFactory,
//            TTransportFactory outputTransportFactory,
//            TProtocolFactory inputProtocolFactory,
//            TProtocolFactory outputProtocolFactory,
//            int maxThreads, LogDelegate logDel)
//            : base(processorFactory, serverTransport, inputTransportFactory, outputTransportFactory,
//                inputProtocolFactory, outputProtocolFactory, logDel)
//        {
//            _maxThreads = maxThreads;
//            _clientQueue = new Queue<TTransport>();
//            _clientLock = new object();
//            _clientThreads = new THashSet<Thread>();
//        }

//        /// <summary>
//        /// Use new Thread for each new client connection. block until numConnections less maxThreads
//        /// </summary>
//        public override void Serve()
//        {
//            try
//            {
//                //start worker thread
//                _workerThread = new Thread(Execute);
//                _workerThread.Start();
//                ServerTransport.Listen();
//            }
//            catch (TTransportException ttx)
//            {
//                LoggingDelegate("Error, could not listen on ServerTransport: " + ttx);
//                return;
//            }

//            //Fire the preServe server event when server is up but before any client connections
//            if (ServerEventHandler != null)
//                ServerEventHandler.PreServe();

//            while (!_stop)
//            {
//                var failureCount = 0;
//                try
//                {
//                    var client = ServerTransport.Accept();
//                    lock (_clientLock)
//                    {
//                        _clientQueue.Enqueue(client);
//                        Monitor.Pulse(_clientLock);
//                    }
//                }
//                catch (TTransportException ttx)
//                {
//                    if (!_stop || ttx.Type != TTransportException.ExceptionType.Interrupted)
//                    {
//                        ++failureCount;
//                        LoggingDelegate(ttx.ToString());
//                    }
//                }
//            }

//            if (_stop)
//            {
//                try
//                {
//                    ServerTransport.Close();
//                }
//                catch (TTransportException ttx)
//                {
//                    LoggingDelegate("TServeTransport failed on close: " + ttx.Message);
//                }
//                _stop = false;
//            }
//        }

//        /// <summary>
//        /// Loops on processing a client forever
//        /// threadContext will be a TTransport instance
//        /// </summary>
//        private void Execute()
//        {
//            while (!_stop)
//            {
//                TTransport client;
//                Thread t;
//                lock (_clientLock)
//                {
//                    //don't dequeue if too many connections
//                    while (_clientThreads.Count >= _maxThreads)
//                    {
//                        Monitor.Wait(_clientLock);
//                    }

//                    while (_clientQueue.Count == 0)
//                    {
//                        Monitor.Wait(_clientLock);
//                    }

//                    client = _clientQueue.Dequeue();
//                    t = new Thread(ClientWorker);
//                    _clientThreads.Add(t);
//                }
//                //start processing requests from client on new thread
//                t.Start(client);
//            }
//        }

//        private void ClientWorker(object context)
//        {
//            var client = (TTransport) context;
//            var processor = ProcessorFactory.GetProcessor(client);
//            TProtocol inputProtocol = null;
//            TProtocol outputProtocol = null;
//            object connectionContext = null;
//            try
//            {
//                TTransport inputTransport = null;
//                using (inputTransport = InputTransportFactory.GetTransport(client))
//                {
//                    TTransport outputTransport = null;
//                    using (outputTransport = OutputTransportFactory.GetTransport(client))
//                    {
//                        inputProtocol = InputProtocolFactory.GetProtocol(inputTransport);
//                        outputProtocol = OutputProtocolFactory.GetProtocol(outputTransport);

//                        //Recover event handler (if any) and fire createContext server event when a client connects
//                        if (ServerEventHandler != null)
//                            connectionContext = ServerEventHandler.CreateContext(inputProtocol, outputProtocol);

//                        //Process client requests until client disconnects
//                        while (!_stop)
//                        {
//                            if (!inputTransport.Peek())
//                                break;

//                            //Fire processContext server event
//                            //N.B. This is the pattern implemented in C++ and the event fires provisionally.
//                            //That is to say it may be many minutes between the event firing and the client request
//                            //actually arriving or the client may hang up without ever makeing a request.
//                            ServerEventHandler?.ProcessContext(connectionContext, inputTransport);
//                            //Process client request (blocks until transport is readable)
//                            if (!processor.Process(inputProtocol, outputProtocol))
//                                break;
//                        }
//                    }
//                }
//            }
//            catch (TTransportException)
//            {
//                //Usually a client disconnect, expected
//            }
//            catch (Exception x)
//            {
//                //Unexpected
//                LoggingDelegate("Error: " + x);
//            }

//            //Fire deleteContext server event after client disconnects
//            ServerEventHandler?.DeleteContext(connectionContext, inputProtocol, outputProtocol);

//            lock (_clientLock)
//            {
//                _clientThreads.Remove(Thread.CurrentThread);
//                Monitor.Pulse(_clientLock);
//            }
//        }

//        //TODO: Thread Abort
//        public override void Stop()
//        {
//            _stop = true;
//            ServerTransport.Close();
//            //clean up all the threads myself
//            _workerThread.Join(); //Abort();
//            foreach (Thread t in _clientThreads)
//            {
//                t.Join(); //Abort();
//            }
//        }
//    }
//}