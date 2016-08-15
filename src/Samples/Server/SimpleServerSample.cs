using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thrift.Samples;
using Thrift.Server;
using Thrift.Transport;

namespace Server
{
    public class SimpleServerSample
    {
        public class CalculatorHandler : Calculator.Iface
        {
            Dictionary<int, SharedStruct> _log;

            public CalculatorHandler()
            {
                _log = new Dictionary<int, SharedStruct>();
            }

            public void Ping()
            {
                Console.WriteLine("Ping()");
            }

            public int Add(int num1, int num2)
            {
                Console.WriteLine("Add({0},{1})", num1, num2);
                return num1 + num2;
            }

            public int Calculate(int logid, Work w)
            {
                Console.WriteLine("Calculate({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);
                var val = 0;

                switch (w.Op)
                {
                    case Operation.Add:
                        val = w.Num1 + w.Num2;
                        break;

                    case Operation.Substract:
                        val = w.Num1 - w.Num2;
                        break;

                    case Operation.Multiply:
                        val = w.Num1 * w.Num2;
                        break;

                    case Operation.Divide:
                        if (w.Num2 == 0)
                        {
                            var io = new InvalidOperation
                            {
                                WhatOp = (int)w.Op,
                                Why = "Cannot divide by 0"
                            };

                            throw io;
                        }
                        val = w.Num1 / w.Num2;
                        break;

                    default:
                        {
                            var io = new InvalidOperation
                            {
                                WhatOp = (int)w.Op,
                                Why = "Unknown operation"
                            };

                            throw io;
                        }
                }

                var entry = new SharedStruct
                {
                    Key = logid,
                    Value = val.ToString()
                };

                _log[logid] = entry;

                return val;
            }

            public void Zip()
            {
                Console.WriteLine("Zip()");
            }

            public SharedStruct GetStruct(int key)
            {
                Console.WriteLine("GetStruct({0})", key);
                return _log[key];
            }

            public async Task<SharedStruct> GetStructAsync(int key)
            {
                Console.WriteLine("GetStructAsync({0})", key);
                return await Task.FromResult(_log[key]);
            }

            public async Task PingAsync()
            {
                Console.WriteLine("PingAsync()");
                await Task.Delay(10, CancellationToken.None);
            }

            public async Task<int> AddAsync(int num1, int num2)
            {
                Console.WriteLine("AddAsync({0},{1})", num1, num2);

                return await Task.FromResult(Add(num1, num2));
            }

            public async Task<int> CalculateAsync(int logid, Work w)
            {
                Console.WriteLine("CalculateAsync({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);

                return await Task.FromResult(Calculate(logid, w));
            }

            public async Task ZipAsync()
            {
                Console.WriteLine("ZipAsync()");
                await Task.Delay(10, CancellationToken.None);
            }

            public IAsyncResult Begin_Ping(AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            public void End_Ping(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }

            public IAsyncResult Begin_Add(AsyncCallback callback, object state, int num1, int num2)
            {
                throw new NotImplementedException();
            }

            public int End_Add(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }

            public IAsyncResult Begin_Calculate(AsyncCallback callback, object state, int logid, Work w)
            {
                throw new NotImplementedException();
            }

            public int End_Calculate(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }

            public IAsyncResult Begin_Zip(AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            public void End_Zip(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }
        }

        public void Run()
        {
            Console.WriteLine("Selected Simple server with TCP transport");

            try
            {
                var fabric = new LoggerFactory();
                fabric.AddConsole(LogLevel.Trace);

                var handler = new CalculatorHandler();
                var processor = new Calculator.Processor(handler);
                var serverTransport = new TServerSocket(9090);
                var server = new TSimpleServer(processor, serverTransport, fabric);

                Console.WriteLine("Starting the server...");
                server.Serve();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.StackTrace);
            }
            Console.WriteLine("Server stopped.");
        }
    }
}
