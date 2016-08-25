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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Thrift.Protocol;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class THttpHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        protected TAsyncProcessor Processor;

        protected TProtocolFactory InputProtocolFactory;
        protected TProtocolFactory OutputProtocolFactory;

        protected const string ContentType = "application/x-thrift";
        protected Encoding Encoding = Encoding.UTF8;

        public THttpHandler(TAsyncProcessor processor, RequestDelegate next, ILoggerFactory loggerFactory)
            : this(processor, new TBinaryProtocol.Factory(), next, loggerFactory)
        {
        }

        public THttpHandler(TAsyncProcessor processor, TProtocolFactory protocolFactory, RequestDelegate next, ILoggerFactory loggerFactory)
            : this(processor, protocolFactory, protocolFactory, next, loggerFactory)
        {
        }

        public THttpHandler(TAsyncProcessor processor, TProtocolFactory inputProtocolFactory,
            TProtocolFactory outputProtocolFactory, RequestDelegate next, ILoggerFactory loggerFactory)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            if (inputProtocolFactory == null)
            {
                throw new ArgumentNullException(nameof(inputProtocolFactory));
            }

            if (outputProtocolFactory == null)
            {
                throw new ArgumentNullException(nameof(outputProtocolFactory));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Processor = processor;
            InputProtocolFactory = inputProtocolFactory;
            OutputProtocolFactory = outputProtocolFactory;

            _next = next;
            _logger = loggerFactory.CreateLogger<THttpHandler>();
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = ContentType;
            await ProcessRequestAsync(context, context.RequestAborted); //TODO: check for correct logic
        }

        public async Task ProcessRequestAsync(HttpContext context, CancellationToken cancellationToken)
        {
            var transport = new TStreamTransport(context.Request.Body, context.Response.Body);

            try
            {
                var input = InputProtocolFactory.GetProtocol(transport);
                var output = OutputProtocolFactory.GetProtocol(transport);

                while (await Processor.ProcessAsync(input, output, cancellationToken))
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