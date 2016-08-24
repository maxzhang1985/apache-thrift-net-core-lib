# Building of samples for different platforms 

Details: 
    
- [https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index  "https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ")
- [https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog "https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog")

# Running of samples 

# Server

Usage: 

    Server.exe 
        will diplay help information 

    Server.exe -t:<transport>
        will run server with specified arguments

Options:

    -t (transport): 
        tcp - tcp transport will be used (host - localhost, port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - .test)
        http - http transport will be used (address - http://localhost:9090)

Sample:

    Server.exe -t:tcp

# Client

Usage: 

    Client.exe 
        will diplay help information 

    Client.exe -t:<transport>
        will run client with specified arguments

Options:

    -t (transport): 
        tcp - tcp transport will be used (host - localhost, port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - .test)
        http - http transport will be used (address - http://localhost:9090)

Sample:

    Client.exe -t:tcp