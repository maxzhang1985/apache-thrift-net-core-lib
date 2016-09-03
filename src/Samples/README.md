# Building of samples for different platforms 

Details: 
    
- [https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index  "https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ")
- [https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog "https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog")

# Running of samples 

# Server

Usage: 

    Server.exe -h
        will diplay help information 

    Server.exe -t:<transport> -p:<protocol>
        will run server with specified arguments (tcp transport and binary protocol by default)

Options:

    -t (transport): 
        tcp - (default) tcp transport will be used (host - ""localhost"", port - 9090)
        tcpbuffered - tcp buffered transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (http address - ""localhost:9090"")
        tcptls - tcp transport with tls will be used (host - ""localhost"", port - 9090)

    -p (protocol): 
        binary - (default) binary protocol will be used
        compact - compact protocol will be used
        json - json protocol will be used

Sample:

    Server.exe -t:tcp

**Remarks**:

    For TcpTls mode certificate's file ThriftTest.pfx should be in directory with binaries in case of command line usage (or at project level in case of debugging from IDE).
    Password for certificate - "ThriftTest".



# Client

Usage: 

    Client.exe -h
        will diplay help information 

    Client.exe -t:<transport> -p:<protocol>
        will run client with specified arguments (tcp transport and binary protocol by default)

Options:

    -t (transport): 
        tcp - (default) tcp transport will be used (host - ""localhost"", port - 9090)
        tcpbuffered - buffered transport over tcp will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (address - ""http://localhost:9090"")        
        tcptls - tcp tls transport will be used (host - ""localhost"", port - 9090)

    -p (protocol): 
        binary - (default) binary protocol will be used
        compact - compact protocol will be used
        json - json protocol will be used

Sample:

    Client.exe -t:tcp -p:binary

**Remarks**:

    For TcpTls mode certificate's file ThriftTest.pfx should be in directory with binaries in case of command line usage (or at project level in case of debugging from IDE).
    Password for certificate - "ThriftTest".
