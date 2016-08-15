# Building of samples for different platforms 

Details: 
	
- [https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index  "https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index ")
- [https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog "https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog")

# Running of samples 

# Server

Usage: 

    Server.exe 
        will diplay help information 

    Server.exe -t:<transport> -s:<server>
        will run server with specified arguments

Options:

    -t (transport): 
        tcp - tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        
    -s (server):
        simple - simple server will be used 

Sample:
    Server.exe -t:tcp -s:simple

# Client

Usage: 

    Client.exe 
        will diplay help information 

    Client.exe -t:<transport> -s:<server>
        will run client with specified arguments

Options:

    -t (transport): 
        tcp - tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        
    -s (server):
        simple - simple server will be used 

Sample:
    Client.exe -transport:tcp -server:simple