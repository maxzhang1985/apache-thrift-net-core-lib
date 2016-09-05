# apache-thrift-net-core-lib

[![Build Status](https://travis-ci.org/vgotra/apache-thrift-net-core-lib.svg?branch=master)](https://travis-ci.org/vgotra/apache-thrift-net-core-lib)

Non official fork of Apache Thrift .Net lib (https://www.nuget.org/packages/ApacheThrift/1.0.0-dev) partially ported to .Net Core. 

Notes: Removed support of Silverlight, commented client and server related to usage of NamedPipeServerStream  

# How to build
* Download the latest version of dotnet from https://www.microsoft.com/net/core (it can be https://go.microsoft.com/fwlink/?LinkID=809122 in case of VS Code)
* Install downloaded version of dotnet
* Clone repo
* Run **build.sh** or **build.cmd** from the root of cloned repository
* Check samples in **src/Samples** folder (https://github.com/vgotra/apache-thrift-net-core-lib/tree/master/src/Samples)
