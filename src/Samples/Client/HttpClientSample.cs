﻿using System;
using Thrift.Transport;
using Thrift;
using Thrift.Protocol;
using Thrift.Samples;

// TODO: Asp.net core sample

namespace Client
{
    public class HttpClientSample
    {
        public void Run()
        {
            try
            {
                TTransport transport = new THttpClient(new Uri("http://localhost:9090"));
                TProtocol protocol = new TBinaryProtocol(transport);
                var client = new Calculator.Client(protocol);

                transport.Open();
                try
                {
                    // Async version

                    Console.WriteLine("PingAsync()");
                    client.PingAsync().GetAwaiter().GetResult();

                    Console.WriteLine("AddAsync()");
                    var sum = client.AddAsync(1, 1).GetAwaiter().GetResult();
                    Console.WriteLine("1+1={0}", sum);

                    var work = new Work
                    {
                        Op = Operation.Divide,
                        Num1 = 1,
                        Num2 = 0
                    };

                    try
                    {
                        Console.WriteLine("CalculateAsync()");
                        var quotient = client.CalculateAsync(1, work).GetAwaiter().GetResult();
                        Console.WriteLine("Whoa we can divide by 0");
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    work.Op = Operation.Substract;
                    work.Num1 = 15;
                    work.Num2 = 10;

                    try
                    {
                        Console.WriteLine("CalculateAsync()");
                        var diff = client.CalculateAsync(1, work).GetAwaiter().GetResult();
                        Console.WriteLine("15-10={0}", diff);
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    Console.WriteLine("GetStructAsync()");
                    var log = client.GetStructAsync(1).GetAwaiter().GetResult();
                    Console.WriteLine("Check log: {0}", log.Value);

                }
                finally
                {
                    transport.Close();
                }
            }
            catch (TApplicationException x)
            {
                Console.WriteLine(x.StackTrace);
            }
        }
    }
}
