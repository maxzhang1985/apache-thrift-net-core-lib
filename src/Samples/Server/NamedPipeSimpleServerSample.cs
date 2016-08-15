using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thrift.Samples;
using Thrift.Server;
using Thrift.Transport;

namespace Server
{
    public class NamedPipeSimpleServerSample
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
                Console.WriteLine("ping()");
            }

            public int Add(int num1, int num2)
            {
                Console.WriteLine("add({0},{1})", num1, num2);
                return num1 + num2;
            }

            public int Calculate(int logid, Work w)
            {
                Console.WriteLine("calculate({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);
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
                Console.WriteLine("zip()");
            }

            public SharedStruct GetStruct(int key)
            {
                Console.WriteLine("getStruct({0})", key);
                return _log[key];
            }

            public Task<SharedStruct> GetStructAsync(int key)
            {
                throw new NotImplementedException();
            }

            public Task PingAsync()
            {
                throw new NotImplementedException();
            }

            public Task<int> AddAsync(int num1, int num2)
            {
                throw new NotImplementedException();
            }

            public Task<int> CalculateAsync(int logid, Work w)
            {
                throw new NotImplementedException();
            }

            public Task ZipAsync()
            {
                throw new NotImplementedException();
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
            Console.WriteLine("Selected Simple server with NamedPipe transport");

            try
            {
                var fabric = new LoggerFactory();
                fabric.AddConsole(LogLevel.Trace);

                var handler = new CalculatorHandler();
                var processor = new Calculator.Processor(handler);
                var serverTransport = new TNamedPipeServerTransport(".test");
                var server = new TSimpleServer(processor, serverTransport, fabric);

                Console.WriteLine("Starting the server...");
                server.Serve();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.StackTrace);
            }
            Console.WriteLine("done.");
        }
    }
}
