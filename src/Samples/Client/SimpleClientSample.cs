using System;
using Thrift;
using Thrift.Protocol;
using Thrift.Samples;
using Thrift.Transport;

namespace Client
{
    public class SimpleClientSample
    {
        public void Run()
        {
            try
            {
                TTransport transport = new TSocket("localhost", 9090);
                TProtocol protocol = new TBinaryProtocol(transport);
                var client = new Calculator.Client(protocol);

                transport.Open();
                try
                {
                    client.Ping();
                    Console.WriteLine("Ping()");

                    var sum = client.Add(1, 1);
                    Console.WriteLine("1+1={0}", sum);

                    var work = new Work
                    {
                        Op = Operation.Divide,
                        Num1 = 1,
                        Num2 = 0
                    };

                    try
                    {
                        var quotient = client.Calculate(1, work);
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
                        var diff = client.Calculate(1, work);
                        Console.WriteLine("15-10={0}", diff);
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    var log = client.GetStruct(1);
                    Console.WriteLine("Check log: {0}", log.Value);



                     // Async version

                    Console.WriteLine("PingAsync()");
                    client.PingAsync().GetAwaiter().GetResult();

                    Console.WriteLine("AddAsync()");
                    sum = client.AddAsync(1, 1).GetAwaiter().GetResult();
                    Console.WriteLine("1+1={0}", sum);

                    work = new Work
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
                    log = client.GetStructAsync(1).GetAwaiter().GetResult();
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
