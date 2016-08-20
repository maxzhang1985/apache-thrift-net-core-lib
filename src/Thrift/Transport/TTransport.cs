/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    //TODO: think about client info 
    // ReSharper disable once InconsistentNaming
    public abstract class TTransport : IDisposable
    {
        public abstract bool IsOpen { get; }

        private readonly byte[] _peekBuffer = new byte[1];
        private bool _hasPeekByte;

        public bool Peek()
        {
            //If we already have a byte read but not consumed, do nothing.
            if (_hasPeekByte)
            {
                return true;
            }

            //If transport closed we can't peek.
            if (!IsOpen)
            {
                return false;
            }

            //Try to read one byte. If succeeds we will need to store it for the next read.
            try
            {
                var bytes = Read(_peekBuffer, 0, 1);
                if (bytes == 0)
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return false;
            }

            _hasPeekByte = true;
            return true;
        }

        public abstract void Open();

        public virtual async Task OpenAsync()
        {
            await OpenAsync(CancellationToken.None);
        }

        public virtual async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            //TODO: check for correct work
            await Task.Factory.StartNew(state => ((TTransport)state).Open(), this, cancellationToken,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public abstract void Close();

        protected static void ValidateBufferArgs(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset),"Buffer offset is smaller than zero.");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Buffer length is smaller than zero.");
            }

            if (offset + length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), "Not enough data.");
            }
        }

        public abstract int Read(byte[] buffer, int offset, int length);

        public virtual async Task<int> ReadAsync(byte[] buffer, int offset, int length)
        {
            return await ReadAsync(buffer, offset, length, CancellationToken.None);
        }

        public virtual async Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<int>(cancellationToken);
            }

            //TODO: check for correct work
            return await Task.Factory.StartNew(state => ((TTransport)state).Read(buffer, offset, length), this, cancellationToken,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public int ReadAll(byte[] buffer, int offset, int length)
        {
            ValidateBufferArgs(buffer, offset, length);
            var retrieved = 0;

            //If we previously peeked a byte, we need to use that first.
            if (_hasPeekByte)
            {
                buffer[offset + retrieved++] = _peekBuffer[0];
                _hasPeekByte = false;
            }

            while (retrieved < length)
            {
                var returnedCount = Read(buffer, offset + retrieved, length - retrieved);
                if (returnedCount <= 0)
                {
                    throw new TTransportException(TTransportException.ExceptionType.EndOfFile, "Cannot read, Remote side has closed");
                }
                retrieved += returnedCount;
            }
            return retrieved;
        }

        public virtual async Task<int> ReadAllAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            ValidateBufferArgs(buffer, offset, length);

            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<int>(cancellationToken);
            }

            var retrieved = 0;

            //If we previously peeked a byte, we need to use that first.
            if (_hasPeekByte)
            {
                buffer[offset + retrieved++] = _peekBuffer[0];
                _hasPeekByte = false;
            }

            while (retrieved < length)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return await Task.FromCanceled<int>(cancellationToken);
                }

                var returnedCount = await ReadAsync(buffer, offset + retrieved, length - retrieved, cancellationToken);
                if (returnedCount <= 0)
                {
                    throw new TTransportException(TTransportException.ExceptionType.EndOfFile, "Cannot read, Remote side has closed");
                }
                retrieved += returnedCount;
            }
            return retrieved;
        }

        public virtual void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public virtual async Task WriteAsync(byte[] buffer)
        {
            await WriteAsync(buffer, CancellationToken.None);
        }

        public virtual async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            //TODO: check for correct work
            await Task.Factory.StartNew(state => ((TTransport) state).Write(buffer), this, cancellationToken,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public abstract void Write(byte[] buffer, int offset, int length);

        public virtual async Task WriteAsync(byte[] buffer, int offset, int length)
        {
            await WriteAsync(buffer, offset, length, CancellationToken.None);
        }

        public virtual async Task WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            //TODO: check for correct work
            await Task.Factory.StartNew(state => ((TTransport)state).Write(buffer, offset, length), this, cancellationToken,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public virtual void Flush()
        {
        }

        public virtual async Task FlushAsync()
        {
            await FlushAsync(CancellationToken.None);
        }

        public virtual async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            //TODO: check for correct work
            await Task.Factory.StartNew(state => ((TTransport) state).Flush(), this, cancellationToken,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public virtual IAsyncResult BeginFlush(AsyncCallback callback, object state)
        {
            throw new TTransportException(TTransportException.ExceptionType.Unknown, "Asynchronous operations are not supported by this transport.");
        }

        public virtual void EndFlush(IAsyncResult asyncResult)
        {
            throw new TTransportException(TTransportException.ExceptionType.Unknown, "Asynchronous operations are not supported by this transport.");
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}