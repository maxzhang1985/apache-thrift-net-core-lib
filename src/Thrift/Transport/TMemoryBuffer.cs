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
 */

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TMemoryBuffer : TTransport
    {
        private readonly MemoryStream _byteStream;

        public TMemoryBuffer()
        {
            _byteStream = new MemoryStream();
        }

        public TMemoryBuffer(byte[] buf)
        {
            _byteStream = new MemoryStream(buf);
        }

        public override void Open()
        {
            /** do nothing **/
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void Close()
        {
            /** do nothing **/
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            return _byteStream.Read(buffer, offset, length);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            return await _byteStream.ReadAsync(buffer, offset, length, cancellationToken);
        }

        public override async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            await _byteStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int length)
        {
            _byteStream.Write(buffer, offset, length);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            await _byteStream.WriteAsync(buffer, offset, length, cancellationToken);
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public byte[] GetBuffer()
        {
            return _byteStream.ToArray();
        }

        public override bool IsOpen => true;

        //public static byte[] Serialize(TAbstractBase s)
        //{
        //    var t = new TMemoryBuffer();
        //    var p = new TBinaryProtocol(t);

        //    s.Write(p);

        //    return t.GetBuffer();
        //}

        //public static T DeSerialize<T>(byte[] buf) where T : TAbstractBase
        //{
        //    var trans = new TMemoryBuffer(buf);
        //    var p = new TBinaryProtocol(trans);
        //    if (typeof(TBase).IsAssignableFrom(typeof(T)))
        //    {
        //        var method = typeof(T).GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);
        //        var t = Activator.CreateInstance<T>();
        //        method.Invoke(t, new object[] {p});
        //        return t;
        //    }
        //    else
        //    {
        //        var method = typeof(T).GetMethod("Read", BindingFlags.Static | BindingFlags.Public);
        //        return (T) method.Invoke(null, new object[] {p});
        //    }
        //}

        private bool _isDisposed;

        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _byteStream?.Dispose();
                }
            }
            _isDisposed = true;
        }
    }
}