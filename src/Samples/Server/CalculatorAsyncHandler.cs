using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Samples;

namespace Server
{
    public class CalculatorAsyncHandler : Calculator.IAsync
    {
        Dictionary<int, SharedStruct> _log;

        public CalculatorAsyncHandler()
        {
            _log = new Dictionary<int, SharedStruct>();
        }

        public async Task<SharedStruct> GetStructAsync(int key, CancellationToken cancellationToken)
        {
            Console.WriteLine("GetStructAsync({0})", key);
            return await Task.FromResult(_log[key]);
        }

        public async Task PingAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("PingAsync()");
            await Task.CompletedTask;
        }

        public async Task<int> AddAsync(int num1, int num2, CancellationToken cancellationToken)
        {
            Console.WriteLine("AddAsync({0},{1})", num1, num2);
            return await Task.FromResult(num1 + num2);
        }

        public async Task<int> CalculateAsync(int logid, Work w, CancellationToken cancellationToken)
        {
            Console.WriteLine("CalculateAsync({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);

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

            return await Task.FromResult(val);
        }

        public async Task ZipAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ZipAsync()");
            await Task.Delay(10, CancellationToken.None);
        }
    }
}