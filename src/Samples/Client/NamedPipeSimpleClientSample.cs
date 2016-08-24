using System;
using System.Threading;
using Thrift;
using Thrift.Protocol;
using Thrift.Samples;
using Thrift.Transport;

namespace Client
{
    public class NamedPipeSimpleClientSample
    {
        public void Run()
        {
            try
            {
                var transport = new TNamedPipeClientTransport(".test");
                var protocol = new TBinaryProtocol(transport);
                var client = new Calculator.Client(protocol);

                transport.OpenAsync(CancellationToken.None).GetAwaiter().GetResult();

                try
                {
                    client.PingAsync().GetAwaiter().GetResult();
                    Console.WriteLine("Ping()");

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
                        client.CalculateAsync(1, work).GetAwaiter().GetResult();
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
                        var diff = client.CalculateAsync(1, work).GetAwaiter().GetResult(); ;
                        Console.WriteLine("15-10={0}", diff);
                    }
                    catch (InvalidOperation io)
                    {
                        Console.WriteLine("Invalid operation: " + io.Why);
                    }

                    var log = client.GetStructAsync(1).GetAwaiter().GetResult(); ;
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
