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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Transport;

namespace Thrift.Protocol
{
    // ReSharper disable once InconsistentNaming
    public class TBinaryProtocol : TProtocol
    {
        //TODO: Unit tests
        //TODO: Localization
        //TODO: pragma

        protected const uint VersionMask = 0xffff0000;
        protected const uint Version1 = 0x80010000;

        protected bool StrictRead;
        protected bool StrictWrite;

        public class Factory : TProtocolFactory
        {
            protected bool StrictRead;
            protected bool StrictWrite;

            public Factory()
                : this(false, true)
            {
            }

            public Factory(bool strictRead, bool strictWrite)
            {
                StrictRead = strictRead;
                StrictWrite = strictWrite;
            }

            public TProtocol GetProtocol(TTransport trans)
            {
                return new TBinaryProtocol(trans, StrictRead, StrictWrite);
            }
        }

        public TBinaryProtocol(TTransport trans)
            : this(trans, false, true)
        {
        }

        public TBinaryProtocol(TTransport trans, bool strictRead, bool strictWrite)
            : base(trans)
        {
            StrictRead = strictRead;
            StrictWrite = strictWrite;
        }

        public override void WriteMessageBegin(TMessage message)
        {
            if (StrictWrite)
            {
                var version = Version1 | (uint)message.Type;
                WriteI32((int) version);
                WriteString(message.Name);
                WriteI32(message.SeqID);
            }
            else
            {
                WriteString(message.Name);
                WriteByte((sbyte) message.Type);
                WriteI32(message.SeqID);
            }
        }

        public override async Task WriteMessageBeginAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (StrictWrite)
            {
                var version = Version1 | (uint)message.Type;
                await WriteI32Async((int)version, cancellationToken);
                await WriteStringAsync(message.Name, cancellationToken);
                await WriteI32Async(message.SeqID, cancellationToken);
            }
            else
            {
                await WriteStringAsync(message.Name, cancellationToken);
                await WriteByteAsync((sbyte)message.Type, cancellationToken);
                await WriteI32Async(message.SeqID, cancellationToken);
            }
        }

        public override void WriteMessageEnd()
        {
        }

        public override async Task WriteMessageEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteStructBegin(TStruct struc)
        {
            //TODO: where is impl?
        }

        public override async Task WriteStructBeginAsync(TStruct struc, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteStructEnd()
        {
        }

        public override async Task WriteStructEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteFieldBegin(TField field)
        {
            WriteByte((sbyte) field.Type);
            WriteI16(field.ID);
        }

        public override async Task WriteFieldBeginAsync(TField field, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync((sbyte)field.Type, cancellationToken);
            await WriteI16Async(field.ID, cancellationToken);
        }

        public override void WriteFieldEnd()
        {
        }

        public override async Task WriteFieldEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteFieldStop()
        {
            WriteByte((sbyte) TType.Stop);
        }

        public override async Task WriteFieldStopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync((sbyte)TType.Stop, cancellationToken);
        }

        public override void WriteMapBegin(TMap map)
        {
            WriteByte((sbyte) map.KeyType);
            WriteByte((sbyte) map.ValueType);
            WriteI32(map.Count);
        }

        public override async Task WriteMapBeginAsync(TMap map, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync((sbyte)map.KeyType, cancellationToken);
            await WriteByteAsync((sbyte)map.ValueType, cancellationToken);
            await WriteI32Async(map.Count, cancellationToken);
        }

        public override void WriteMapEnd()
        {
        }

        public override async Task WriteMapEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteListBegin(TList list)
        {
            WriteByte((sbyte) list.ElementType);
            WriteI32(list.Count);
        }

        public override async Task WriteListBeginAsync(TList list, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync((sbyte)list.ElementType, cancellationToken);
            await WriteI32Async(list.Count, cancellationToken);
        }

        public override void WriteListEnd()
        {
        }

        public override async Task WriteListEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteSetBegin(TSet set)
        {
            WriteByte((sbyte) set.ElementType);
            WriteI32(set.Count);
        }

        public override async Task WriteSetBeginAsync(TSet set, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync((sbyte)set.ElementType, cancellationToken);
            await WriteI32Async(set.Count, cancellationToken);
        }

        public override void WriteSetEnd()
        {
        }

        public override async Task WriteSetEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override void WriteBool(bool b)
        {
            WriteByte(b ? (sbyte) 1 : (sbyte) 0);
        }

        public override async Task WriteBoolAsync(bool b, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteByteAsync(b ? (sbyte)1 : (sbyte)0, cancellationToken);
        }

        protected internal static byte[] CreateWriteByte(sbyte b)
        {
            var bout = new byte[1];

            bout[0] = (byte)b;

            return bout;
        }

        public override void WriteByte(sbyte b)
        {
            var bout = CreateWriteByte(b);
            Trans.Write(bout, 0, 1);
        }

        public override async Task WriteByteAsync(sbyte b, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var bout = CreateWriteByte(b);
            await Trans.WriteAsync(bout, 0, 1, cancellationToken);
        }

        protected internal static byte[] CreateWriteI16(short s)
        {
            var i16Out = new byte[2];

            i16Out[0] = (byte)(0xff & (s >> 8));
            i16Out[1] = (byte)(0xff & s);

            return i16Out;
        }

        public override void WriteI16(short int16)
        {
            var i16Out = CreateWriteI16(int16);
            Trans.Write(i16Out, 0, 2);
        }
        
        public override async Task WriteI16Async(short i16, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var i16Out = CreateWriteI16(i16);
            await Trans.WriteAsync(i16Out, 0, 2, cancellationToken);
        }

        protected internal static byte[] CreateWriteI32(int i32)
        {
            var i32Out = new byte[4];

            i32Out[0] = (byte)(0xff & (i32 >> 24));
            i32Out[1] = (byte)(0xff & (i32 >> 16));
            i32Out[2] = (byte)(0xff & (i32 >> 8));
            i32Out[3] = (byte)(0xff & i32);

            return i32Out;
        }

        public override void WriteI32(int i32)
        {
            var i32Out = CreateWriteI32(i32);
            Trans.Write(i32Out, 0, 4);
        }

        public override async Task WriteI32Async(int i32, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var i32Out = CreateWriteI32(i32);
            await Trans.WriteAsync(i32Out, 0, 4, cancellationToken);
        }

        protected internal static byte[] CreateWriteI64(long i64)
        {
            var i64Out = new byte[8];

            i64Out[0] = (byte)(0xff & (i64 >> 56));
            i64Out[1] = (byte)(0xff & (i64 >> 48));
            i64Out[2] = (byte)(0xff & (i64 >> 40));
            i64Out[3] = (byte)(0xff & (i64 >> 32));
            i64Out[4] = (byte)(0xff & (i64 >> 24));
            i64Out[5] = (byte)(0xff & (i64 >> 16));
            i64Out[6] = (byte)(0xff & (i64 >> 8));
            i64Out[7] = (byte)(0xff & i64);

            return i64Out;
        }

        public override void WriteI64(long i64)
        {
            var i64Out = CreateWriteI64(i64);
            Trans.Write(i64Out, 0, 8);
        }

        public override async Task WriteI64Async(long i64, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var i64Out = CreateWriteI64(i64);
            await Trans.WriteAsync(i64Out, 0, 8, cancellationToken);
        }

        public override void WriteDouble(double d)
        {
            WriteI64(BitConverter.DoubleToInt64Bits(d));
        }

        public override async Task WriteDoubleAsync(double d, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteI64Async(BitConverter.DoubleToInt64Bits(d), cancellationToken);
        }

        public override void WriteBinary(byte[] b)
        {
            WriteI32(b.Length);
            Trans.Write(b, 0, b.Length);
        }

        public override async Task WriteBinaryAsync(byte[] b, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await WriteI32Async(b.Length, CancellationToken.None);
            await Trans.WriteAsync(b, 0, b.Length, cancellationToken);
        }

        public override TMessage ReadMessageBegin()
        {
            var message = new TMessage();
            var size = ReadI32();
            if (size < 0)
            {
                var version = (uint) size & VersionMask;
                if (version != Version1)
                {
                    throw new TProtocolException(TProtocolException.BAD_VERSION, $"Bad version in ReadMessageBegin: {version}");
                }
                message.Type = (TMessageType) (size & 0x000000ff);
                message.Name = ReadString();
                message.SeqID = ReadI32();
            }
            else
            {
                if (StrictRead)
                {
                    throw new TProtocolException(TProtocolException.BAD_VERSION, "Missing version in ReadMessageBegin, old client?");
                }
                message.Name = ReadStringBody(size);
                message.Type = (TMessageType) ReadByte();
                message.SeqID = ReadI32();
            }
            return message;
        }

        public override async Task<TMessage> ReadMessageBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TMessage>(cancellationToken);
            }

            var message = new TMessage();
            var size = await ReadI32Async(cancellationToken);
            if (size < 0)
            {
                var version = (uint)size & VersionMask;
                if (version != Version1)
                {
                    throw new TProtocolException(TProtocolException.BAD_VERSION, $"Bad version in ReadMessageBegin: {version}");
                }
                message.Type = (TMessageType)(size & 0x000000ff);
                message.Name = await ReadStringAsync(cancellationToken);
                message.SeqID = await ReadI32Async(cancellationToken);
            }
            else
            {
                if (StrictRead)
                {
                    throw new TProtocolException(TProtocolException.BAD_VERSION, "Missing version in ReadMessageBegin, old client?");
                }
                message.Name = await ReadStringBodyAsync(size, cancellationToken);
                message.Type = (TMessageType)await ReadByteAsync(cancellationToken);
                message.SeqID = await ReadI32Async(cancellationToken);
            }
            return message;
        }

        public override void ReadMessageEnd()
        {
        }

        public override async Task ReadMessageEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override TStruct ReadStructBegin()
        {
            //TODO: no read from internal transport?
            return new TStruct();
        }

        public override async Task<TStruct> ReadStructBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            //TODO: no read from internal transport?
            return new TStruct();
        }

        public override void ReadStructEnd()
        {
        }

        public override async Task ReadStructEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override TField ReadFieldBegin()
        {
            var field = new TField
            {
                Type = (TType) ReadByte()
            };

            if (field.Type != TType.Stop)
            {
                field.ID = ReadI16();
            }

            return field;
        }

        public override async Task<TField> ReadFieldBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TField>(cancellationToken);
            }

            var field = new TField
            {
                Type = (TType) await ReadByteAsync(cancellationToken)
            };

            if (field.Type != TType.Stop)
            {
                field.ID = await ReadI16Async(cancellationToken);
            }

            return field;
        }

        public override void ReadFieldEnd()
        {
        }

        public override async Task ReadFieldEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override TMap ReadMapBegin()
        {
            var map = new TMap
            {
                KeyType = (TType) ReadByte(),
                ValueType = (TType) ReadByte(),
                Count = ReadI32()
            };

            return map;
        }

        public override async Task<TMap> ReadMapBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TMap>(cancellationToken);
            }

            var map = new TMap
            {
                KeyType = (TType)await ReadByteAsync(cancellationToken),
                ValueType = (TType)await ReadByteAsync(cancellationToken),
                Count = await ReadI32Async(cancellationToken)
            };

            return map;
        }

        public override void ReadMapEnd()
        {
        }

        public override async Task ReadMapEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override TList ReadListBegin()
        {
            var list = new TList
            {
                ElementType = (TType) ReadByte(),
                Count = ReadI32()
            };

            return list;
        }

        public override async Task<TList> ReadListBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TList>(cancellationToken);
            }

            var list = new TList
            {
                ElementType = (TType)await ReadByteAsync(cancellationToken),
                Count = await ReadI32Async(cancellationToken)
            };

            return list;
        }

        public override void ReadListEnd()
        {
        }

        public override async Task ReadListEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override TSet ReadSetBegin()
        {
            var set = new TSet
            {
                ElementType = (TType) ReadByte(),
                Count = ReadI32()
            };

            return set;
        }

        public override async Task<TSet> ReadSetBeginAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<TSet>(cancellationToken);
            }

            var set = new TSet
            {
                ElementType = (TType)await ReadByteAsync(cancellationToken),
                Count = await ReadI32Async(cancellationToken)
            };

            return set;
        }

        public override void ReadSetEnd()
        {
        }

        public override async Task ReadSetEndAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }
        }

        public override bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public override async Task<bool> ReadBoolAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<bool>(cancellationToken);
            }

            return await ReadByteAsync(cancellationToken) == 1;
        }

        public override sbyte ReadByte()
        {
            var bin = new byte[1];
            Trans.ReadAll(bin, 0, 1); //TODO: why readall ?
            return (sbyte) bin[0];
        }

        public override async Task<sbyte> ReadByteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<sbyte>(cancellationToken);
            }

            var bin = new byte[1];
            await Trans.ReadAllAsync(bin, 0, 1, cancellationToken); //TODO: why readall ?
            return (sbyte)bin[0];
        }

        public override short ReadI16()
        {
            var i16In = new byte[2];
            Trans.ReadAll(i16In, 0, 2);
            var result = (short) (((i16In[0] & 0xff) << 8) | i16In[1] & 0xff);
            return result;
        }

        public override async Task<short> ReadI16Async(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<short>(cancellationToken);
            }

            var i16In = new byte[2];
            await Trans.ReadAllAsync(i16In, 0, 2, cancellationToken);
            var result = (short)(((i16In[0] & 0xff) << 8) | i16In[1] & 0xff);
            return result;
        }
        
        public override int ReadI32()
        {
            var i32In = new byte[4];
            Trans.ReadAll(i32In, 0, 4);
            var result = ((i32In[0] & 0xff) << 24) | ((i32In[1] & 0xff) << 16) | ((i32In[2] & 0xff) << 8) | i32In[3] & 0xff;
            return result;
        }

        public override async Task<int> ReadI32Async(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<int>(cancellationToken);
            }

            var i32In = new byte[4];
            await Trans.ReadAllAsync(i32In, 0, 4, cancellationToken);
            var result = ((i32In[0] & 0xff) << 24) | ((i32In[1] & 0xff) << 16) | ((i32In[2] & 0xff) << 8) | i32In[3] & 0xff;
            return result;
        }

#pragma warning disable 675

        protected internal long CreateReadI64(byte[] buf)
        {
            var result =
                ((long)(buf[0] & 0xff) << 56) |
                ((long)(buf[1] & 0xff) << 48) |
                ((long)(buf[2] & 0xff) << 40) |
                ((long)(buf[3] & 0xff) << 32) |
                ((long)(buf[4] & 0xff) << 24) |
                ((long)(buf[5] & 0xff) << 16) |
                ((long)(buf[6] & 0xff) << 8) |
                buf[7] & 0xff;

            return result;
        }

#pragma warning restore 675

        public override long ReadI64()
        {
            var i64In = new byte[8];
            Trans.ReadAll(i64In, 0, 8);
            return CreateReadI64(i64In);
        }

        public override async Task<long> ReadI64Async(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<long>(cancellationToken);
            }

            var i64In = new byte[8];
            await Trans.ReadAllAsync(i64In, 0, 8, cancellationToken);
            return CreateReadI64(i64In);
        }

        public override double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadI64());
        }

        public override async Task<double> ReadDoubleAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<double>(cancellationToken);
            }

            var d = await ReadI64Async(cancellationToken);
            return BitConverter.Int64BitsToDouble(d); 
        }

        public override byte[] ReadBinary()
        {
            var size = ReadI32();
            var buf = new byte[size];
            Trans.ReadAll(buf, 0, size);
            return buf;
        }

        public override async Task<byte[]> ReadBinaryAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<byte[]>(cancellationToken);
            }

            var size = await ReadI32Async(cancellationToken);
            var buf = new byte[size];
            await Trans.ReadAllAsync(buf, 0, size, cancellationToken);
            return buf;
        }

        private string ReadStringBody(int size)
        {
            var buf = new byte[size];
            Trans.ReadAll(buf, 0, size);
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        private async Task<string> ReadStringBodyAsync(int size, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled<string>(cancellationToken);
            }

            var buf = new byte[size];
            await Trans.ReadAllAsync(buf, 0, size, cancellationToken);
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }
    }
}