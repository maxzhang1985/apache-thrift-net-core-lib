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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Transport;

namespace Thrift.Protocol
{
    //TODO: implementation of TProtocol

    // ReSharper disable once InconsistentNaming
    public class TCompactProtocol : TProtocol
    {
        private const byte ProtocolId = 0x82;
        private const byte Version = 1;
        private const byte VersionMask = 0x1f; // 0001 1111
        private const byte TypeMask = 0xE0; // 1110 0000
        private const byte TypeBits = 0x07; // 0000 0111
        private const int TypeShiftAmount = 5;
        private static readonly TStruct AnonymousStruct = new TStruct("");
        private static readonly TField Tstop = new TField("", TType.Stop, 0);

        // ReSharper disable once InconsistentNaming
        private static readonly byte[] TTypeToCompactType = new byte[16];

        /// <summary>
        ///     Used to keep track of the last field for the current and previous structs, so we can do the delta stuff.
        /// </summary>
        private readonly Stack<short> _lastField = new Stack<short>(15);

        /// <summary>
        ///     If we encounter a boolean field begin, save the TField here so it can have the value incorporated.
        /// </summary>
        private TField? _booleanField;

        /// <summary>
        ///     If we Read a field header, and it's a boolean field, save the boolean value here so that ReadBool can use it.
        /// </summary>
        private bool? _boolValue;

        private short _lastFieldId;

        public TCompactProtocol(TTransport trans)
            : base(trans)
        {
            TTypeToCompactType[(int) TType.Stop] = Types.Stop;
            TTypeToCompactType[(int) TType.Bool] = Types.BooleanTrue;
            TTypeToCompactType[(int) TType.Byte] = Types.Byte;
            TTypeToCompactType[(int) TType.I16] = Types.I16;
            TTypeToCompactType[(int) TType.I32] = Types.I32;
            TTypeToCompactType[(int) TType.I64] = Types.I64;
            TTypeToCompactType[(int) TType.Double] = Types.Double;
            TTypeToCompactType[(int) TType.String] = Types.Binary;
            TTypeToCompactType[(int) TType.List] = Types.List;
            TTypeToCompactType[(int) TType.Set] = Types.Set;
            TTypeToCompactType[(int) TType.Map] = Types.Map;
            TTypeToCompactType[(int) TType.Struct] = Types.Struct;
        }

        public void Reset()
        {
            _lastField.Clear();
            _lastFieldId = 0;
        }

        private void WriteByteDirect(byte b)
        {
            // Writes a byte without any possibility of all that field header nonsense. 
            // Used internally by other writing methods that know they need to Write a byte.

            var buf = new[] {b};
            Trans.Write(buf);
        }

        /// <summary>
        ///     Writes a byte without any possibility of all that field header nonsense.
        /// </summary>
        private void WriteByteDirect(int n)
        {
            WriteByteDirect((byte) n);
        }

        private void WriteVarInt32(uint n)
        {
            // Write an i32 as a varint.Results in 1 - 5 bytes on the wire.
            var i32Buf = new byte[5];

            var idx = 0;
            while (true)
            {
                if ((n & ~0x7F) == 0)
                {
                    i32Buf[idx++] = (byte) n;
                    // WriteByteDirect((byte)n);
                    break;
                    // return;
                }
                i32Buf[idx++] = (byte) ((n & 0x7F) | 0x80);
                // WriteByteDirect((byte)((n & 0x7F) | 0x80));
                n >>= 7;
            }
            Trans.Write(i32Buf, 0, idx);
        }

        /// <summary>
        ///     Write a message header to the wire. Compact Protocol messages contain the protocol version so we can migrate
        ///     forwards in the future if need be.
        /// </summary>
        public override void WriteMessageBegin(TMessage message)
        {
            WriteByteDirect(ProtocolId);
            WriteByteDirect((byte) ((Version & VersionMask) | (((uint) message.Type << TypeShiftAmount) & TypeMask)));
            WriteVarInt32((uint) message.SeqID);
            WriteString(message.Name);
        }

        public override Task WriteMessageEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Write a struct begin. This doesn't actually put anything on the wire. We
        ///     use it as an opportunity to put special placeholder markers on the field
        ///     stack so we can get the field id deltas correct.
        /// </summary>
        public override void WriteStructBegin(TStruct strct)
        {
            _lastField.Push(_lastFieldId);
            _lastFieldId = 0;
        }

        public override Task WriteStructBeginAsync(TStruct struc, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteStructEnd()
        {
            /*
            Write a struct end. This doesn't actually put anything on the wire. We use
            this as an opportunity to pop the last field from the current struct off
            of the field stack.
            */

            _lastFieldId = _lastField.Pop();
        }

        public override Task WriteStructEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteFieldBegin(TField field)
        {
            if (field.Type == TType.Bool)
                _booleanField = field;
            else
                WriteFieldBeginInternal(field, 0xFF);
        }

        private void WriteFieldBeginInternal(TField field, byte typeOverride)
        {
            // short lastField = lastField_.Pop();

            // if there's a exType override, use that.
            var typeToWrite = typeOverride == 0xFF ? GetCompactType(field.Type) : typeOverride;

            // check if we can use delta encoding for the field id
            if ((field.ID > _lastFieldId) && (field.ID - _lastFieldId <= 15))
            {
                // Write them together
                WriteByteDirect(((field.ID - _lastFieldId) << 4) | typeToWrite);
            }
            else
            {
                // Write them separate
                WriteByteDirect(typeToWrite);
                WriteI16(field.ID);
            }

            _lastFieldId = field.ID;
            // lastField_.push(field.id);
        }

        public override Task WriteFieldEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteFieldStop()
        {
            WriteByteDirect(Types.Stop);
        }

        public override Task WriteFieldStopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteMapBegin(TMap map)
        {
            if (map.Count == 0)
            {
                WriteByteDirect(0);
            }
            else
            {
                WriteVarInt32((uint) map.Count);
                WriteByteDirect((GetCompactType(map.KeyType) << 4) | GetCompactType(map.ValueType));
            }
        }

        public override Task WriteMapEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteListBegin(TList list)
        {
            WriteCollectionBegin(list.ElementType, list.Count);
        }

        public override Task WriteListEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteSetBegin(TSet set)
        {
            WriteCollectionBegin(set.ElementType, set.Count);
        }


        public override Task WriteSetEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteBool(bool b)
        {
            /*
            Write a boolean value. Potentially, this could be a boolean field, in
            which case the field header info isn't written yet. If so, decide what the
            right exType header is for the value and then Write the field header.
            Otherwise, Write a single byte.
            */

            if (_booleanField != null)
            {
                // we haven't written the field header yet
                WriteFieldBeginInternal(_booleanField.Value, b ? Types.BooleanTrue : Types.BooleanFalse);
                _booleanField = null;
            }
            else
            {
                // we're not part of a field, so just Write the value.
                WriteByteDirect(b ? Types.BooleanTrue : Types.BooleanFalse);
            }
        }

        public override Task WriteBoolAsync(bool b, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(sbyte b)
        {
            WriteByteDirect((byte) b);
        }

        public override Task WriteByteAsync(sbyte b, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteI16(short i16)
        {
            WriteVarInt32(IntToZigZag(i16));
        }

        public override Task WriteI16Async(short i16, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteI32(int i32)
        {
            WriteVarInt32(IntToZigZag(i32));
        }

        public override Task WriteI32Async(int i32, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteI64(long i64)
        {
            WriteVarint64(LongToZigzag(i64));
        }

        public override Task WriteI64Async(long i64, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteDouble(double dub)
        {
            var data = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
            FixedLongToBytes(BitConverter.DoubleToInt64Bits(dub), data, 0);
            Trans.Write(data);
        }

        public override Task WriteDoubleAsync(double d, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            WriteBinary(bytes, 0, bytes.Length);
        }

        public override void WriteBinary(byte[] bin)
        {
            WriteBinary(bin, 0, bin.Length);
        }

        private void WriteBinary(byte[] buf, int offset, int length)
        {
            WriteVarInt32((uint) length);
            Trans.Write(buf, offset, length);
        }

        public override Task WriteMessageBeginAsync(TMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteMessageEnd()
        {
        }

        public override Task WriteMapBeginAsync(TMap map, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteMapEnd()
        {
        }

        public override Task WriteListBeginAsync(TList list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteListEnd()
        {
        }

        public override Task WriteSetBeginAsync(TSet set, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteSetEnd()
        {
        }

        public override Task WriteFieldBeginAsync(TField field, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void WriteFieldEnd()
        {
        }


        protected void WriteCollectionBegin(TType elemType, int size)
        {
            /**
             * Abstract method for writing the start of lists and sets. List and sets on
             * the wire differ only by the exType indicator.
             */

            if (size <= 14)
            {
                WriteByteDirect((size << 4) | GetCompactType(elemType));
            }
            else
            {
                WriteByteDirect(0xf0 | GetCompactType(elemType));
                WriteVarInt32((uint) size);
            }
        }


        private void WriteVarint64(ulong n)
        {
            // Write an i64 as a varint. Results in 1-10 bytes on the wire.

            var buf = new byte[10];
            var idx = 0;
            while (true)
            {
                if ((n & ~(ulong) 0x7FL) == 0)
                {
                    buf[idx++] = (byte) n;
                    break;
                }
                buf[idx++] = (byte) ((n & 0x7F) | 0x80);
                n >>= 7;
            }
            Trans.Write(buf, 0, idx);
        }


        private ulong LongToZigzag(long n)
        {
            // Convert l into a zigzag long. This allows negative numbers to be represented compactly as a varint.

            return (ulong) (n << 1) ^ (ulong) (n >> 63);
        }


        private uint IntToZigZag(int n)
        {
            // Convert n into a zigzag int. This allows negative numbers to be represented compactly as a varint.

            return (uint) (n << 1) ^ (uint) (n >> 31);
        }


        private void FixedLongToBytes(long n, byte[] buf, int off)
        {
            // Convert a long into little-endian bytes in buf starting at off and going until off+7.

            buf[off + 0] = (byte) (n & 0xff);
            buf[off + 1] = (byte) ((n >> 8) & 0xff);
            buf[off + 2] = (byte) ((n >> 16) & 0xff);
            buf[off + 3] = (byte) ((n >> 24) & 0xff);
            buf[off + 4] = (byte) ((n >> 32) & 0xff);
            buf[off + 5] = (byte) ((n >> 40) & 0xff);
            buf[off + 6] = (byte) ((n >> 48) & 0xff);
            buf[off + 7] = (byte) ((n >> 56) & 0xff);
        }

        public override Task WriteBinaryAsync(byte[] b, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TMessage ReadMessageBegin()
        {
            var protocolId = (byte) ReadByte();


            if (protocolId != ProtocolId)
            {
                throw new TProtocolException($"Expected protocol id {ProtocolId:X} but got {protocolId:X}");
            }

            var versionAndType = (byte) ReadByte();
            var version = (byte) (versionAndType & VersionMask);

            if (version != Version)
            {
                throw new TProtocolException($"Expected version {Version} but got {version}");
            }

            var type = (byte) ((versionAndType >> TypeShiftAmount) & TypeBits);
            var seqid = (int) ReadVarInt32();
            var messageName = ReadString();

            return new TMessage(messageName, (TMessageType) type, seqid);
        }

        public override Task ReadMessageEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TStruct ReadStructBegin()
        {
            /*
            Read a struct begin. There's nothing on the wire for this, but it is our
            opportunity to push a new struct begin marker onto the field stack.
            */

            _lastField.Push(_lastFieldId);
            _lastFieldId = 0;
            return AnonymousStruct;
        }

        public override Task<TStruct> ReadStructBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadStructEnd()
        {
            /*
            Doesn't actually consume any wire data, just removes the last field for
            this struct from the field stack.
            */

            // consume the last field we Read off the wire.
            _lastFieldId = _lastField.Pop();
        }

        public override Task ReadStructEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TField ReadFieldBegin()
        {
            // Read a field header off the wire.

            var type = (byte) ReadByte();

            // if it's a stop, then we can return immediately, as the struct is over.
            if (type == Types.Stop)
            {
                return Tstop;
            }

            short fieldId;

            // mask off the 4 MSB of the exType header. it could contain a field id delta.
            var modifier = (short) ((type & 0xf0) >> 4);
            if (modifier == 0)
            {
                fieldId = ReadI16();
            }
            else
            {
                fieldId = (short) (_lastFieldId + modifier);
            }

            var field = new TField("", GetTType((byte) (type & 0x0f)), fieldId);

            // if this happens to be a boolean field, the value is encoded in the exType
            if (IsBoolType(type))
            {
                _boolValue = (byte) (type & 0x0f) == Types.BooleanTrue;
            }

            // push the new field onto the field stack so we can keep the deltas going.
            _lastFieldId = field.ID;
            return field;
        }


        public override Task ReadFieldEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TMap ReadMapBegin()
        {
            /*
            Read a map header off the wire. If the size is zero, skip Reading the key
            and value exType. This means that 0-length maps will yield TMaps without the
            "correct" types.
            */

            var size = (int) ReadVarInt32();
            var keyAndValueType = size == 0 ? (byte) 0 : (byte) ReadByte();
            return new TMap(GetTType((byte) (keyAndValueType >> 4)), GetTType((byte) (keyAndValueType & 0xf)), size);
        }


        public override Task ReadMapEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TList ReadListBegin()
        {
            /*
            Read a list header off the wire. If the list size is 0-14, the size will
            be packed into the element exType header. If it's a longer list, the 4 MSB
            of the element exType header will be 0xF, and a varint will follow with the
            true size.
            */

            var sizeAndType = (byte) ReadByte();
            var size = (sizeAndType >> 4) & 0x0f;
            if (size == 15)
            {
                size = (int) ReadVarInt32();
            }
            var type = GetTType(sizeAndType);
            return new TList(type, size);
        }


        public override Task ReadListEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override TSet ReadSetBegin()
        {
            /*
            Read a set header off the wire. If the set size is 0-14, the size will
            be packed into the element exType header. If it's a longer set, the 4 MSB
            of the element exType header will be 0xF, and a varint will follow with the
            true size.
            */

            return new TSet(ReadListBegin());
        }


        public override Task ReadSetEndAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ReadBool()
        {
            /*
            Read a boolean off the wire. If this is a boolean field, the value should
            already have been Read during ReadFieldBegin, so we'll just consume the
            pre-stored value. Otherwise, Read a byte.
            */

            if (_boolValue != null)
            {
                var result = _boolValue.Value;
                _boolValue = null;
                return result;
            }
            return ReadByte() == Types.BooleanTrue;
        }


        public override Task<bool> ReadBoolAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override sbyte ReadByte()
        {
            // Read a single byte off the wire. Nothing interesting here.

            var buf = new byte[1];
            Trans.ReadAll(buf, 0, 1);
            return (sbyte) buf[0];
        }

        public override Task<sbyte> ReadByteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override short ReadI16()
        {
            // Read an i16 from the wire as a zigzag varint.

            return (short) ZigzagToInt(ReadVarInt32());
        }


        public override Task<short> ReadI16Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override int ReadI32()
        {
            // Read an i32 from the wire as a zigzag varint.

            return ZigzagToInt(ReadVarInt32());
        }


        public override Task<int> ReadI32Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override long ReadI64()
        {
            // Read an i64 from the wire as a zigzag varint.

            return ZigzagToLong(ReadVarint64());
        }


        public override Task<long> ReadI64Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override double ReadDouble()
        {
            // No magic here - just Read a double off the wire.

            var longBits = new byte[8];
            Trans.ReadAll(longBits, 0, 8);
            return BitConverter.Int64BitsToDouble(BytesToLong(longBits));
        }


        public override Task<double> ReadDoubleAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override string ReadString()
        {
            // Reads a byte[] (via ReadBinary), and then UTF-8 decodes it.

            var length = (int) ReadVarInt32();

            if (length == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(ReadBinary(length));
        }


        public override byte[] ReadBinary()
        {
            // Read a byte[] from the wire.

            var length = (int) ReadVarInt32();
            if (length == 0)
            {
                return new byte[0];
            }

            var buf = new byte[length];
            Trans.ReadAll(buf, 0, length);
            return buf;
        }

        public override Task<byte[]> ReadBinaryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        private byte[] ReadBinary(int length)
        {
            // Read a byte[] of a known length from the wire.

            if (length == 0)
            {
                return new byte[0];
            }

            var buf = new byte[length];
            Trans.ReadAll(buf, 0, length);
            return buf;
        }

        public override Task<TMessage> ReadMessageBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadMessageEnd()
        {
        }

        public override Task<TField> ReadFieldBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadFieldEnd()
        {
        }

        public override Task<TMap> ReadMapBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadMapEnd()
        {
        }

        public override Task<TList> ReadListBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadListEnd()
        {
        }

        public override Task<TSet> ReadSetBeginAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void ReadSetEnd()
        {
        }

        private uint ReadVarInt32()
        {
            /*
            Read an i32 from the wire as a varint. The MSB of each byte is set
            if there is another byte to follow. This can Read up to 5 bytes.
            */

            uint result = 0;
            var shift = 0;
            while (true)
            {
                var b = (byte) ReadByte();
                result |= (uint) (b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }
            return result;
        }

        private ulong ReadVarint64()
        {
            /*
            Read an i64 from the wire as a proper varint. The MSB of each byte is set
            if there is another byte to follow. This can Read up to 10 bytes.
            */

            var shift = 0;
            ulong result = 0;
            while (true)
            {
                var b = (byte) ReadByte();
                result |= (ulong) (b & 0x7f) << shift;
                if ((b & 0x80) != 0x80) break;
                shift += 7;
            }

            return result;
        }

        private static int ZigzagToInt(uint n)
        {
            return (int) (n >> 1) ^ -(int) (n & 1);
        }

        private static long ZigzagToLong(ulong n)
        {
            return (long) (n >> 1) ^ -(long) (n & 1);
        }

        private static long BytesToLong(byte[] bytes)
        {
            /*
            Note that it's important that the mask bytes are long literals,
            otherwise they'll default to ints, and when you shift an int left 56 bits,
            you just get a messed up int.
            */

            return
                ((bytes[7] & 0xffL) << 56) |
                ((bytes[6] & 0xffL) << 48) |
                ((bytes[5] & 0xffL) << 40) |
                ((bytes[4] & 0xffL) << 32) |
                ((bytes[3] & 0xffL) << 24) |
                ((bytes[2] & 0xffL) << 16) |
                ((bytes[1] & 0xffL) << 8) |
                (bytes[0] & 0xffL);
        }

        private static bool IsBoolType(byte b)
        {
            var lowerNibble = b & 0x0f;
            return (lowerNibble == Types.BooleanTrue) || (lowerNibble == Types.BooleanFalse);
        }

        private static TType GetTType(byte type)
        {
            // Given a TCompactProtocol.Types constant, convert it to its corresponding TType value.

            switch ((byte) (type & 0x0f))
            {
                case Types.Stop:
                    return TType.Stop;
                case Types.BooleanFalse:
                case Types.BooleanTrue:
                    return TType.Bool;
                case Types.Byte:
                    return TType.Byte;
                case Types.I16:
                    return TType.I16;
                case Types.I32:
                    return TType.I32;
                case Types.I64:
                    return TType.I64;
                case Types.Double:
                    return TType.Double;
                case Types.Binary:
                    return TType.String;
                case Types.List:
                    return TType.List;
                case Types.Set:
                    return TType.Set;
                case Types.Map:
                    return TType.Map;
                case Types.Struct:
                    return TType.Struct;
                default:
                    throw new TProtocolException($"Don't know what exType: {(byte) (type & 0x0f)}");
            }
        }


        private static byte GetCompactType(TType ttype)
        {
            // Given a TType value, find the appropriate TCompactProtocol.Types constant.

            return TTypeToCompactType[(int) ttype];
        }

        /// <summary>
        ///     All of the on-wire exType codes.
        /// </summary>
        private static class Types
        {
            public const byte Stop = 0x00;
            public const byte BooleanTrue = 0x01;
            public const byte BooleanFalse = 0x02;
            public const byte Byte = 0x03;
            public const byte I16 = 0x04;
            public const byte I32 = 0x05;
            public const byte I64 = 0x06;
            public const byte Double = 0x07;
            public const byte Binary = 0x08;
            public const byte List = 0x09;
            public const byte Set = 0x0A;
            public const byte Map = 0x0B;
            public const byte Struct = 0x0C;
        }

        public class Factory : TProtocolFactory
        {
            public TProtocol GetProtocol(TTransport trans)
            {
                return new TCompactProtocol(trans);
            }
        }
    }
}