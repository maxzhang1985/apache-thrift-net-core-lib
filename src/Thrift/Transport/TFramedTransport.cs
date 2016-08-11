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

using System;
using System.IO;

namespace Thrift.Transport
{
    // ReSharper disable once InconsistentNaming
    public class TFramedTransport : TTransport
    {
        private readonly TTransport _transport;
        private readonly MemoryStream _writeBuffer = new MemoryStream(1024);
        private readonly MemoryStream _readBuffer = new MemoryStream(1024);

        private const int HeaderSize = 4;
        private readonly byte[] _headerBuf = new byte[HeaderSize];

        public class Factory : TTransportFactory
        {
            public override TTransport GetTransport(TTransport trans)
            {
                return new TFramedTransport(trans);
            }
        }

        public TFramedTransport(TTransport transport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));
            _transport = transport;
            InitWriteBuffer();
        }

        public override void Open()
        {
            CheckNotDisposed();
            _transport.Open();
        }

        public override bool IsOpen => !_isDisposed && _transport.IsOpen;

      public override void Close()
        {
            CheckNotDisposed();
            _transport.Close();
        }

        public override int Read(byte[] buf, int off, int len)
        {
            CheckNotDisposed();
            ValidateBufferArgs(buf, off, len);
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            var got = _readBuffer.Read(buf, off, len);
            if (got > 0)
            {
                return got;
            }

            // Read another frame of data
            ReadFrame();

            return _readBuffer.Read(buf, off, len);
        }

        private void ReadFrame()
        {
            _transport.ReadAll(_headerBuf, 0, HeaderSize);
            var size = DecodeFrameSize(_headerBuf);

            _readBuffer.SetLength(size);
            _readBuffer.Seek(0, SeekOrigin.Begin);
            ArraySegment<byte> bufSegment;
            _readBuffer.TryGetBuffer(out bufSegment);
            byte[] buff = bufSegment.Array;

            //byte[] buff = readBuffer.GetBuffer();
            _transport.ReadAll(buff, 0, size);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            CheckNotDisposed();
            ValidateBufferArgs(buf, off, len);
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            if (_writeBuffer.Length + len > int.MaxValue)
                Flush();
            _writeBuffer.Write(buf, off, len);
        }

        public override void Flush()
        {
            CheckNotDisposed();
            if (!IsOpen)
                throw new TTransportException(TTransportException.ExceptionType.NotOpen);
            ArraySegment<byte> bufSegment;
            _writeBuffer.TryGetBuffer(out bufSegment);
            byte[] buf = bufSegment.Array;

            //byte[] buf = writeBuffer.GetBuffer();
            var len = (int) _writeBuffer.Length;
            var dataLen = len - HeaderSize;
            if (dataLen < 0)
                throw new InvalidOperationException(); // logic error actually

            // Inject message header into the reserved buffer space
            EncodeFrameSize(dataLen, buf);

            // Send the entire message at once
            _transport.Write(buf, 0, len);

            InitWriteBuffer();

            _transport.Flush();
        }

        private void InitWriteBuffer()
        {
            // Reserve space for message header to be put right before sending it out
            _writeBuffer.SetLength(HeaderSize);
            _writeBuffer.Seek(0, SeekOrigin.End);
        }

        private static void EncodeFrameSize(int frameSize, byte[] buf)
        {
            buf[0] = (byte) (0xff & (frameSize >> 24));
            buf[1] = (byte) (0xff & (frameSize >> 16));
            buf[2] = (byte) (0xff & (frameSize >> 8));
            buf[3] = (byte) (0xff & (frameSize));
        }

        private static int DecodeFrameSize(byte[] buf)
        {
            return
                ((buf[0] & 0xff) << 24) |
                ((buf[1] & 0xff) << 16) |
                ((buf[2] & 0xff) << 8) |
                ((buf[3] & 0xff));
        }


        private void CheckNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("TFramedTransport");
        }

        private bool _isDisposed;

        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _readBuffer.Dispose();
                    _writeBuffer.Dispose();
                }
            }
            _isDisposed = true;
        }
    }
}