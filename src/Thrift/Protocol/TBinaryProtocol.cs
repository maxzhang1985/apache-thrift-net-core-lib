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
using Thrift.Transport;

namespace Thrift.Protocol
{
    // ReSharper disable once InconsistentNaming
    public class TBinaryProtocol : TProtocol
    {
        protected const uint VersionMask = 0xffff0000;
        protected const uint Version1 = 0x80010000;

        protected bool StrictRead;
        protected bool StrictWrite = true;

        /**
          * Factory
          */

        public class Factory : TProtocolFactory
        {
            protected bool StrictRead;
            protected bool StrictWrite = true;

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
                var version = Version1 | (uint) (message.Type);
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

        public override void WriteMessageEnd()
        {
        }

        public override void WriteStructBegin(TStruct struc)
        {
        }

        public override void WriteStructEnd()
        {
        }

        public override void WriteFieldBegin(TField field)
        {
            WriteByte((sbyte) field.Type);
            WriteI16(field.ID);
        }

        public override void WriteFieldEnd()
        {
        }

        public override void WriteFieldStop()
        {
            WriteByte((sbyte) TType.Stop);
        }

        public override void WriteMapBegin(TMap map)
        {
            WriteByte((sbyte) map.KeyType);
            WriteByte((sbyte) map.ValueType);
            WriteI32(map.Count);
        }

        public override void WriteMapEnd()
        {
        }

        public override void WriteListBegin(TList list)
        {
            WriteByte((sbyte) list.ElementType);
            WriteI32(list.Count);
        }

        public override void WriteListEnd()
        {
        }

        public override void WriteSetBegin(TSet set)
        {
            WriteByte((sbyte) set.ElementType);
            WriteI32(set.Count);
        }

        public override void WriteSetEnd()
        {
        }

        public override void WriteBool(bool b)
        {
            WriteByte(b ? (sbyte) 1 : (sbyte) 0);
        }

        private readonly byte[] _bout = new byte[1];

        public override void WriteByte(sbyte b)
        {
            _bout[0] = (byte) b;
            Trans.Write(_bout, 0, 1);
        }

        private readonly byte[] _i16Out = new byte[2];

        public override void WriteI16(short s)
        {
            _i16Out[0] = (byte) (0xff & (s >> 8));
            _i16Out[1] = (byte) (0xff & s);
            Trans.Write(_i16Out, 0, 2);
        }

        private readonly byte[] _i32Out = new byte[4];

        public override void WriteI32(int i32)
        {
            _i32Out[0] = (byte) (0xff & (i32 >> 24));
            _i32Out[1] = (byte) (0xff & (i32 >> 16));
            _i32Out[2] = (byte) (0xff & (i32 >> 8));
            _i32Out[3] = (byte) (0xff & i32);
            Trans.Write(_i32Out, 0, 4);
        }

        private readonly byte[] _i64Out = new byte[8];

        public override void WriteI64(long i64)
        {
            _i64Out[0] = (byte) (0xff & (i64 >> 56));
            _i64Out[1] = (byte) (0xff & (i64 >> 48));
            _i64Out[2] = (byte) (0xff & (i64 >> 40));
            _i64Out[3] = (byte) (0xff & (i64 >> 32));
            _i64Out[4] = (byte) (0xff & (i64 >> 24));
            _i64Out[5] = (byte) (0xff & (i64 >> 16));
            _i64Out[6] = (byte) (0xff & (i64 >> 8));
            _i64Out[7] = (byte) (0xff & i64);
            Trans.Write(_i64Out, 0, 8);
        }

        public override void WriteDouble(double d)
        {
            WriteI64(BitConverter.DoubleToInt64Bits(d));
        }

        public override void WriteBinary(byte[] b)
        {
            WriteI32(b.Length);
            Trans.Write(b, 0, b.Length);
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
                    throw new TProtocolException(TProtocolException.BAD_VERSION,
                        "Bad version in ReadMessageBegin: " + version);
                }
                message.Type = (TMessageType) (size & 0x000000ff);
                message.Name = ReadString();
                message.SeqID = ReadI32();
            }
            else
            {
                if (StrictRead)
                {
                    throw new TProtocolException(TProtocolException.BAD_VERSION,
                        "Missing version in readMessageBegin, old client?");
                }
                message.Name = ReadStringBody(size);
                message.Type = (TMessageType) ReadByte();
                message.SeqID = ReadI32();
            }
            return message;
        }

        public override void ReadMessageEnd()
        {
        }

        public override TStruct ReadStructBegin()
        {
            return new TStruct();
        }

        public override void ReadStructEnd()
        {
        }

        public override TField ReadFieldBegin()
        {
            var field = new TField();
            field.Type = (TType) ReadByte();

            if (field.Type != TType.Stop)
            {
                field.ID = ReadI16();
            }

            return field;
        }

        public override void ReadFieldEnd()
        {
        }

        public override TMap ReadMapBegin()
        {
            var map = new TMap();
            map.KeyType = (TType) ReadByte();
            map.ValueType = (TType) ReadByte();
            map.Count = ReadI32();

            return map;
        }

        public override void ReadMapEnd()
        {
        }

        public override TList ReadListBegin()
        {
            var list = new TList();
            list.ElementType = (TType) ReadByte();
            list.Count = ReadI32();

            return list;
        }

        public override void ReadListEnd()
        {
        }

        public override TSet ReadSetBegin()
        {
            var set = new TSet();
            set.ElementType = (TType) ReadByte();
            set.Count = ReadI32();

            return set;
        }

        public override void ReadSetEnd()
        {
        }

        public override bool ReadBool()
        {
            return ReadByte() == 1;
        }

        private readonly byte[] _bin = new byte[1];

        public override sbyte ReadByte()
        {
            ReadAll(_bin, 0, 1);
            return (sbyte) _bin[0];
        }

        private readonly byte[] _i16In = new byte[2];

        public override short ReadI16()
        {
            ReadAll(_i16In, 0, 2);
            return (short) (((_i16In[0] & 0xff) << 8) | ((_i16In[1] & 0xff)));
        }

        private readonly byte[] _i32In = new byte[4];

        public override int ReadI32()
        {
            ReadAll(_i32In, 0, 4);
            return
                ((_i32In[0] & 0xff) << 24) | ((_i32In[1] & 0xff) << 16) | ((_i32In[2] & 0xff) << 8) | ((_i32In[3] & 0xff));
        }

#pragma warning disable 675

        private readonly byte[] _i64In = new byte[8];

        public override long ReadI64()
        {
            ReadAll(_i64In, 0, 8);
            return ((long) (_i64In[0] & 0xff) << 56) |
                   ((long) (_i64In[1] & 0xff) << 48) |
                   ((long) (_i64In[2] & 0xff) << 40) |
                   ((long) (_i64In[3] & 0xff) << 32) |
                   ((long) (_i64In[4] & 0xff) << 24) |
                   ((long) (_i64In[5] & 0xff) << 16) |
                   ((long) (_i64In[6] & 0xff) << 8) |
                   _i64In[7] & 0xff;
        }

#pragma warning restore 675

        public override double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadI64());
        }

        public override byte[] ReadBinary()
        {
            var size = ReadI32();
            var buf = new byte[size];
            Trans.ReadAll(buf, 0, size);
            return buf;
        }

        private string ReadStringBody(int size)
        {
            var buf = new byte[size];
            Trans.ReadAll(buf, 0, size);
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        private int ReadAll(byte[] buf, int off, int len)
        {
            return Trans.ReadAll(buf, off, len);
        }
    }
}