/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 *
 */

using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Thrift.Protocol;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class THttpHandler
    {
        private readonly RequestDelegate _next;

        protected TAsyncProcessor processor;

        protected TProtocolFactory inputProtocolFactory;
        protected TProtocolFactory outputProtocolFactory;

        protected const string contentType = "application/x-thrift";
        protected System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        public THttpHandler(TAsyncProcessor processor)
            : this(processor, new TBinaryProtocol.Factory())
        {
        }

        public THttpHandler(TAsyncProcessor processor, TProtocolFactory protocolFactory)
            : this(processor, protocolFactory, protocolFactory)
        {
        }

        public THttpHandler(TAsyncProcessor processor, TProtocolFactory inputProtocolFactory,
            TProtocolFactory outputProtocolFactory)
        {
            this.processor = processor;
            this.inputProtocolFactory = inputProtocolFactory;
            this.outputProtocolFactory = outputProtocolFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = contentType;
            //context.Response.ContentEncoding = encoding;
            //ProcessRequest(context.Request.Body, context.Response.Body);
            await ProcessRequestAsync(context);
        }

        //TODO correct TPL statement
        public async Task ProcessRequestAsync(HttpContext context)
        {
            var transport = new TStreamTransport(context.Request.Body, context.Response.Body);

            try
            {
                var input = inputProtocolFactory.GetProtocol(transport);
                var output = outputProtocolFactory.GetProtocol(transport);

                while (await processor.ProcessAsync(input, output))
                {
                }
            }
            catch (TTransportException)
            {
                // Client died, just move on
            }
            finally
            {
                transport.Close();
            }
        }
    }
}