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

namespace Thrift.Protocol
{
/**
         * TProtocolDecorator forwards all requests to an enclosed TProtocol instance,
         * providing a way to author concise concrete decorator subclasses.  While it has
         * no abstract methods, it is marked abstract as a reminder that by itself,
         * it does not modify the behaviour of the enclosed TProtocol.
         *
         * See p.175 of Design Patterns (by Gamma et al.)
         * See TMultiplexedProtocol
         */
    // ReSharper disable once InconsistentNaming
    public abstract class TProtocolDecorator : TProtocol
    {
        private readonly TProtocol _wrappedProtocol;

        /**
         * Encloses the specified protocol.
         * @param protocol All operations will be forward to this protocol.  Must be non-null.
         */

        protected TProtocolDecorator(TProtocol protocol)
            : base(protocol.Transport)
        {
            _wrappedProtocol = protocol;
        }

        public override void WriteMessageBegin(TMessage tMessage)
        {
            _wrappedProtocol.WriteMessageBegin(tMessage);
        }

        public override void WriteMessageEnd()
        {
            _wrappedProtocol.WriteMessageEnd();
        }

        public override void WriteStructBegin(TStruct tStruct)
        {
            _wrappedProtocol.WriteStructBegin(tStruct);
        }

        public override void WriteStructEnd()
        {
            _wrappedProtocol.WriteStructEnd();
        }

        public override void WriteFieldBegin(TField tField)
        {
            _wrappedProtocol.WriteFieldBegin(tField);
        }

        public override void WriteFieldEnd()
        {
            _wrappedProtocol.WriteFieldEnd();
        }

        public override void WriteFieldStop()
        {
            _wrappedProtocol.WriteFieldStop();
        }

        public override void WriteMapBegin(TMap tMap)
        {
            _wrappedProtocol.WriteMapBegin(tMap);
        }

        public override void WriteMapEnd()
        {
            _wrappedProtocol.WriteMapEnd();
        }

        public override void WriteListBegin(TList tList)
        {
            _wrappedProtocol.WriteListBegin(tList);
        }

        public override void WriteListEnd()
        {
            _wrappedProtocol.WriteListEnd();
        }

        public override void WriteSetBegin(TSet tSet)
        {
            _wrappedProtocol.WriteSetBegin(tSet);
        }

        public override void WriteSetEnd()
        {
            _wrappedProtocol.WriteSetEnd();
        }

        public override void WriteBool(bool b)
        {
            _wrappedProtocol.WriteBool(b);
        }

        public override void WriteByte(sbyte b)
        {
            _wrappedProtocol.WriteByte(b);
        }

        public override void WriteI16(short i)
        {
            _wrappedProtocol.WriteI16(i);
        }

        public override void WriteI32(int i)
        {
            _wrappedProtocol.WriteI32(i);
        }

        public override void WriteI64(long l)
        {
            _wrappedProtocol.WriteI64(l);
        }

        public override void WriteDouble(double v)
        {
            _wrappedProtocol.WriteDouble(v);
        }

        public override void WriteString(string s)
        {
            _wrappedProtocol.WriteString(s);
        }

        public override void WriteBinary(byte[] bytes)
        {
            _wrappedProtocol.WriteBinary(bytes);
        }

        public override TMessage ReadMessageBegin()
        {
            return _wrappedProtocol.ReadMessageBegin();
        }

        public override void ReadMessageEnd()
        {
            _wrappedProtocol.ReadMessageEnd();
        }

        public override TStruct ReadStructBegin()
        {
            return _wrappedProtocol.ReadStructBegin();
        }

        public override void ReadStructEnd()
        {
            _wrappedProtocol.ReadStructEnd();
        }

        public override TField ReadFieldBegin()
        {
            return _wrappedProtocol.ReadFieldBegin();
        }

        public override void ReadFieldEnd()
        {
            _wrappedProtocol.ReadFieldEnd();
        }

        public override TMap ReadMapBegin()
        {
            return _wrappedProtocol.ReadMapBegin();
        }

        public override void ReadMapEnd()
        {
            _wrappedProtocol.ReadMapEnd();
        }

        public override TList ReadListBegin()
        {
            return _wrappedProtocol.ReadListBegin();
        }

        public override void ReadListEnd()
        {
            _wrappedProtocol.ReadListEnd();
        }

        public override TSet ReadSetBegin()
        {
            return _wrappedProtocol.ReadSetBegin();
        }

        public override void ReadSetEnd()
        {
            _wrappedProtocol.ReadSetEnd();
        }

        public override bool ReadBool()
        {
            return _wrappedProtocol.ReadBool();
        }

        public override sbyte ReadByte()
        {
            return _wrappedProtocol.ReadByte();
        }

        public override short ReadI16()
        {
            return _wrappedProtocol.ReadI16();
        }

        public override int ReadI32()
        {
            return _wrappedProtocol.ReadI32();
        }

        public override long ReadI64()
        {
            return _wrappedProtocol.ReadI64();
        }

        public override double ReadDouble()
        {
            return _wrappedProtocol.ReadDouble();
        }

        public override string ReadString()
        {
            return _wrappedProtocol.ReadString();
        }

        public override byte[] ReadBinary()
        {
            return _wrappedProtocol.ReadBinary();
        }
    }
}