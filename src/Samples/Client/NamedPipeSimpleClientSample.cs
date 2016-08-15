using System;
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

                transport.Open();
                try
                {
                    client.Ping();
                    Console.WriteLine("ping()");

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
