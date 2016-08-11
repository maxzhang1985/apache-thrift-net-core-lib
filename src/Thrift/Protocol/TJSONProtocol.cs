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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Thrift.Transport;

namespace Thrift.Protocol
{
    /// <summary>
    /// JSON protocol implementation for thrift.
    ///
    /// This is a full-featured protocol supporting Write and Read.
    ///
    /// Please see the C++ class header for a detailed description of the
    /// protocol's wire format.
    ///
    /// Adapted from the Java version.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TJSONProtocol : TProtocol
    {
        /// <summary>
        /// Factory for JSON protocol objects
        /// </summary>
        public class Factory : TProtocolFactory
        {
            public TProtocol GetProtocol(TTransport trans)
            {
                return new TJSONProtocol(trans);
            }
        }

        private static readonly byte[] Comma = {(byte) ','};
        private static readonly byte[] Colon = {(byte) ':'};
        private static readonly byte[] Lbrace = {(byte) '{'};
        private static readonly byte[] Rbrace = {(byte) '}'};
        private static readonly byte[] Lbracket = {(byte) '['};
        private static readonly byte[] Rbracket = {(byte) ']'};
        private static readonly byte[] Quote = {(byte) '"'};
        private static readonly byte[] Backslash = {(byte) '\\'};

        private readonly byte[] _escseq = {(byte) '\\', (byte) 'u', (byte) '0', (byte) '0'};

        private const long Version = 1;

        private readonly byte[] _jsonCharTable =
        {
            0, 0, 0, 0, 0, 0, 0, 0, (byte) 'b', (byte) 't', (byte) 'n', 0, (byte) 'f', (byte) 'r', 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, (byte) '"', 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private readonly char[] _escapeChars = "\"\\/bfnrt".ToCharArray();

        private readonly byte[] _escapeCharVals =
        {
            (byte) '"', (byte) '\\', (byte) '/', (byte) '\b', (byte) '\f', (byte) '\n', (byte) '\r', (byte) '\t'
        };

        private const int DefStringSize = 16;

        private static readonly byte[] NameBool = {(byte) 't', (byte) 'f'};
        private static readonly byte[] NameByte = {(byte) 'i', (byte) '8'};
        private static readonly byte[] NameI16 = {(byte) 'i', (byte) '1', (byte) '6'};
        private static readonly byte[] NameI32 = {(byte) 'i', (byte) '3', (byte) '2'};
        private static readonly byte[] NameI64 = {(byte) 'i', (byte) '6', (byte) '4'};
        private static readonly byte[] NameDouble = {(byte) 'd', (byte) 'b', (byte) 'l'};
        private static readonly byte[] NameStruct = {(byte) 'r', (byte) 'e', (byte) 'c'};
        private static readonly byte[] NameString = {(byte) 's', (byte) 't', (byte) 'r'};
        private static readonly byte[] NameMap = {(byte) 'm', (byte) 'a', (byte) 'p'};
        private static readonly byte[] NameList = {(byte) 'l', (byte) 's', (byte) 't'};
        private static readonly byte[] NameSet = {(byte) 's', (byte) 'e', (byte) 't'};

        private static byte[] GetTypeNameForTypeId(TType typeId)
        {
            switch (typeId)
            {
                case TType.Bool:
                    return NameBool;
                case TType.Byte:
                    return NameByte;
                case TType.I16:
                    return NameI16;
                case TType.I32:
                    return NameI32;
                case TType.I64:
                    return NameI64;
                case TType.Double:
                    return NameDouble;
                case TType.String:
                    return NameString;
                case TType.Struct:
                    return NameStruct;
                case TType.Map:
                    return NameMap;
                case TType.Set:
                    return NameSet;
                case TType.List:
                    return NameList;
                default:
                    throw new TProtocolException(TProtocolException.NOT_IMPLEMENTED,
                        "Unrecognized exType");
            }
        }

        private static TType GetTypeIdForTypeName(byte[] name)
        {
            var result = TType.Stop;
            if (name.Length > 1)
            {
                switch (name[0])
                {
                    case (byte) 'd':
                        result = TType.Double;
                        break;
                    case (byte) 'i':
                        switch (name[1])
                        {
                            case (byte) '8':
                                result = TType.Byte;
                                break;
                            case (byte) '1':
                                result = TType.I16;
                                break;
                            case (byte) '3':
                                result = TType.I32;
                                break;
                            case (byte) '6':
                                result = TType.I64;
                                break;
                        }
                        break;
                    case (byte) 'l':
                        result = TType.List;
                        break;
                    case (byte) 'm':
                        result = TType.Map;
                        break;
                    case (byte) 'r':
                        result = TType.Struct;
                        break;
                    case (byte) 's':
                        if (name[1] == (byte) 't')
                        {
                            result = TType.String;
                        }
                        else if (name[1] == (byte) 'e')
                        {
                            result = TType.Set;
                        }
                        break;
                    case (byte) 't':
                        result = TType.Bool;
                        break;
                }
            }
            if (result == TType.Stop)
            {
                throw new TProtocolException(TProtocolException.NOT_IMPLEMENTED,
                    "Unrecognized exType");
            }
            return result;
        }

        ///<summary>
        /// Base class for tracking JSON contexts that may require
        /// inserting/Reading additional JSON syntax characters
        /// This base context does nothing.
        ///</summary>
        protected class JsonBaseContext
        {
            protected TJSONProtocol Proto;

            public JsonBaseContext(TJSONProtocol proto)
            {
                Proto = proto;
            }

            public virtual void Write()
            {
            }

            public virtual void Read()
            {
            }

            public virtual bool EscapeNumbers()
            {
                return false;
            }
        }

        ///<summary>
        /// Context for JSON lists. Will insert/Read commas before each item except
        /// for the first one
        ///</summary>
        protected class JsonListContext : JsonBaseContext
        {
            public JsonListContext(TJSONProtocol protocol)
                : base(protocol)
            {
            }

            private bool _first = true;

            public override void Write()
            {
                if (_first)
                {
                    _first = false;
                }
                else
                {
                    Proto.Trans.Write(Comma);
                }
            }

            public override void Read()
            {
                if (_first)
                {
                    _first = false;
                }
                else
                {
                    Proto.ReadJsonSyntaxChar(Comma);
                }
            }
        }

        ///<summary>
        /// Context for JSON records. Will insert/Read colons before the value portion
        /// of each record pair, and commas before each key except the first. In
        /// addition, will indicate that numbers in the key position need to be
        /// escaped in quotes (since JSON keys must be strings).
        ///</summary>
        protected class JsonPairContext : JsonBaseContext
        {
            public JsonPairContext(TJSONProtocol proto)
                : base(proto)
            {
            }

            private bool _first = true;
            private bool _colon = true;

            public override void Write()
            {
                if (_first)
                {
                    _first = false;
                    _colon = true;
                }
                else
                {
                    Proto.Trans.Write(_colon ? Colon : Comma);
                    _colon = !_colon;
                }
            }

            public override void Read()
            {
                if (_first)
                {
                    _first = false;
                    _colon = true;
                }
                else
                {
                    Proto.ReadJsonSyntaxChar(_colon ? Colon : Comma);
                    _colon = !_colon;
                }
            }

            public override bool EscapeNumbers()
            {
                return _colon;
            }
        }

        ///<summary>
        /// Holds up to one byte from the transport
        ///</summary>
        protected class LookaheadReader
        {
            protected TJSONProtocol Proto;

            public LookaheadReader(TJSONProtocol proto)
            {
                Proto = proto;
            }

            private bool _hasData;
            private readonly byte[] _data = new byte[1];

            ///<summary>
            /// Return and consume the next byte to be Read, either taking it from the
            /// data buffer if present or getting it from the transport otherwise.
            ///</summary>
            public byte Read()
            {
                if (_hasData)
                {
                    _hasData = false;
                }
                else
                {
                    Proto.Trans.ReadAll(_data, 0, 1);
                }
                return _data[0];
            }

            ///<summary>
            /// Return the next byte to be Read without consuming, filling the data
            /// buffer if it has not been filled alReady.
            ///</summary>
            public byte Peek()
            {
                if (!_hasData)
                {
                    Proto.Trans.ReadAll(_data, 0, 1);
                }
                _hasData = true;
                return _data[0];
            }
        }

        // Default encoding
        protected Encoding Utf8Encoding = Encoding.UTF8;

        // Stack of nested contexts that we may be in
        protected Stack<JsonBaseContext> ContextStack = new Stack<JsonBaseContext>();

        // Current context that we are in
        protected JsonBaseContext Context;

        // Reader that manages a 1-byte buffer
        protected LookaheadReader Reader;

        ///<summary>
        /// Push a new JSON context onto the stack.
        ///</summary>
        protected void PushContext(JsonBaseContext c)
        {
            ContextStack.Push(Context);
            Context = c;
        }

        ///<summary>
        /// Pop the last JSON context off the stack
        ///</summary>
        protected void PopContext()
        {
            Context = ContextStack.Pop();
        }

        ///<summary>
        /// TJSONProtocol Constructor
        ///</summary>
        public TJSONProtocol(TTransport trans)
            : base(trans)
        {
            Context = new JsonBaseContext(this);
            Reader = new LookaheadReader(this);
        }

        // Temporary buffer used by several methods
        private readonly byte[] _tempBuffer = new byte[4];

        ///<summary>
        /// Read a byte that must match b[0]; otherwise an exception is thrown.
        /// Marked protected to avoid synthetic accessor in JSONListContext.Read
        /// and JSONPairContext.Read
        ///</summary>
        protected void ReadJsonSyntaxChar(byte[] b)
        {
            var ch = Reader.Read();
            if (ch != b[0])
            {
                throw new TProtocolException(TProtocolException.INVALID_DATA,
                    "Unexpected character:" + (char) ch);
            }
        }

        ///<summary>
        /// Convert a byte containing a hex char ('0'-'9' or 'a'-'f') into its
        /// corresponding hex value
        ///</summary>
        private static byte HexVal(byte ch)
        {
            if ((ch >= '0') && (ch <= '9'))
            {
                return (byte) ((char) ch - '0');
            }
            if ((ch >= 'a') && (ch <= 'f'))
            {
                ch += 10;
                return (byte) ((char) ch - 'a');
            }
            throw new TProtocolException(TProtocolException.INVALID_DATA,
                "Expected hex character");
        }

        ///<summary>
        /// Convert a byte containing a hex value to its corresponding hex character
        ///</summary>
        private static byte HexChar(byte val)
        {
            val &= 0x0F;
            if (val < 10)
            {
                return (byte) ((char) val + '0');
            }
            val -= 10;
            return (byte) ((char) val + 'a');
        }

        ///<summary>
        /// Write the bytes in array buf as a JSON characters, escaping as needed
        ///</summary>
        private void WriteJsonString(byte[] b)
        {
            Context.Write();
            Trans.Write(Quote);
            var len = b.Length;
            for (var i = 0; i < len; i++)
            {
                if ((b[i] & 0x00FF) >= 0x30)
                {
                    if (b[i] == Backslash[0])
                    {
                        Trans.Write(Backslash);
                        Trans.Write(Backslash);
                    }
                    else
                    {
                        Trans.Write(b, i, 1);
                    }
                }
                else
                {
                    _tempBuffer[0] = _jsonCharTable[b[i]];
                    if (_tempBuffer[0] == 1)
                    {
                        Trans.Write(b, i, 1);
                    }
                    else if (_tempBuffer[0] > 1)
                    {
                        Trans.Write(Backslash);
                        Trans.Write(_tempBuffer, 0, 1);
                    }
                    else
                    {
                        Trans.Write(_escseq);
                        _tempBuffer[0] = HexChar((byte) (b[i] >> 4));
                        _tempBuffer[1] = HexChar(b[i]);
                        Trans.Write(_tempBuffer, 0, 2);
                    }
                }
            }
            Trans.Write(Quote);
        }

        ///<summary>
        /// Write out number as a JSON value. If the context dictates so, it will be
        /// wrapped in quotes to output as a JSON string.
        ///</summary>
        private void WriteJsonInteger(long num)
        {
            Context.Write();
            var str = num.ToString();

            var escapeNum = Context.EscapeNumbers();
            if (escapeNum)
                Trans.Write(Quote);

            Trans.Write(Utf8Encoding.GetBytes(str));

            if (escapeNum)
                Trans.Write(Quote);
        }

        ///<summary>
        /// Write out a double as a JSON value. If it is NaN or infinity or if the
        /// context dictates escaping, Write out as JSON string.
        ///</summary>
        private void WriteJsonDouble(double num)
        {
            Context.Write();
            var str = num.ToString("G17", CultureInfo.InvariantCulture);
            var special = false;

            switch (str[0])
            {
                case 'N': // NaN
                case 'I': // Infinity
                    special = true;
                    break;
                case '-':
                    if (str[1] == 'I')
                    {
                        // -Infinity
                        special = true;
                    }
                    break;
            }

            var escapeNum = special || Context.EscapeNumbers();

            if (escapeNum)
                Trans.Write(Quote);

            Trans.Write(Utf8Encoding.GetBytes(str));

            if (escapeNum)
                Trans.Write(Quote);
        }

        ///<summary>
        /// Write out contents of byte array b as a JSON string with base-64 encoded
        /// data
        ///</summary>
        private void WriteJsonBase64(byte[] b)
        {
            Context.Write();
            Trans.Write(Quote);

            var len = b.Length;
            var off = 0;

            // Ignore padding
            var bound = len >= 2 ? len - 2 : 0;
            for (var i = len - 1; i >= bound && b[i] == '='; --i)
            {
                --len;
            }
            while (len >= 3)
            {
                // Encode 3 bytes at a time
                TBase64Utils.Encode(b, off, 3, _tempBuffer, 0);
                Trans.Write(_tempBuffer, 0, 4);
                off += 3;
                len -= 3;
            }
            if (len > 0)
            {
                // Encode remainder
                TBase64Utils.Encode(b, off, len, _tempBuffer, 0);
                Trans.Write(_tempBuffer, 0, len + 1);
            }

            Trans.Write(Quote);
        }

        private void WriteJsonObjectStart()
        {
            Context.Write();
            Trans.Write(Lbrace);
            PushContext(new JsonPairContext(this));
        }

        private void WriteJsonObjectEnd()
        {
            PopContext();
            Trans.Write(Rbrace);
        }

        private void WriteJsonArrayStart()
        {
            Context.Write();
            Trans.Write(Lbracket);
            PushContext(new JsonListContext(this));
        }

        private void WriteJsonArrayEnd()
        {
            PopContext();
            Trans.Write(Rbracket);
        }

        public override void WriteMessageBegin(TMessage message)
        {
            WriteJsonArrayStart();
            WriteJsonInteger(Version);

            var b = Utf8Encoding.GetBytes(message.Name);
            WriteJsonString(b);

            WriteJsonInteger((long) message.Type);
            WriteJsonInteger(message.SeqID);
        }

        public override void WriteMessageEnd()
        {
            WriteJsonArrayEnd();
        }

        public override void WriteStructBegin(TStruct str)
        {
            WriteJsonObjectStart();
        }

        public override void WriteStructEnd()
        {
            WriteJsonObjectEnd();
        }

        public override void WriteFieldBegin(TField field)
        {
            WriteJsonInteger(field.ID);
            WriteJsonObjectStart();
            WriteJsonString(GetTypeNameForTypeId(field.Type));
        }

        public override void WriteFieldEnd()
        {
            WriteJsonObjectEnd();
        }

        public override void WriteFieldStop()
        {
        }

        public override void WriteMapBegin(TMap map)
        {
            WriteJsonArrayStart();
            WriteJsonString(GetTypeNameForTypeId(map.KeyType));
            WriteJsonString(GetTypeNameForTypeId(map.ValueType));
            WriteJsonInteger(map.Count);
            WriteJsonObjectStart();
        }

        public override void WriteMapEnd()
        {
            WriteJsonObjectEnd();
            WriteJsonArrayEnd();
        }

        public override void WriteListBegin(TList list)
        {
            WriteJsonArrayStart();
            WriteJsonString(GetTypeNameForTypeId(list.ElementType));
            WriteJsonInteger(list.Count);
        }

        public override void WriteListEnd()
        {
            WriteJsonArrayEnd();
        }

        public override void WriteSetBegin(TSet set)
        {
            WriteJsonArrayStart();
            WriteJsonString(GetTypeNameForTypeId(set.ElementType));
            WriteJsonInteger(set.Count);
        }

        public override void WriteSetEnd()
        {
            WriteJsonArrayEnd();
        }

        public override void WriteBool(bool b)
        {
            WriteJsonInteger(b ? 1 : 0);
        }

        public override void WriteByte(sbyte b)
        {
            WriteJsonInteger(b);
        }

        public override void WriteI16(short i16)
        {
            WriteJsonInteger(i16);
        }

        public override void WriteI32(int i32)
        {
            WriteJsonInteger(i32);
        }

        public override void WriteI64(long i64)
        {
            WriteJsonInteger(i64);
        }

        public override void WriteDouble(double dub)
        {
            WriteJsonDouble(dub);
        }

        public override void WriteString(string str)
        {
            var b = Utf8Encoding.GetBytes(str);
            WriteJsonString(b);
        }

        public override void WriteBinary(byte[] bin)
        {
            WriteJsonBase64(bin);
        }

        /**
         * Reading methods.
         */

        ///<summary>
        /// Read in a JSON string, unescaping as appropriate.. Skip Reading from the
        /// context if skipContext is true.
        ///</summary>
        private byte[] ReadJsonString(bool skipContext)
        {
            var buffer = new MemoryStream();
            var codeunits = new List<char>();


            if (!skipContext)
            {
                Context.Read();
            }
            ReadJsonSyntaxChar(Quote);
            while (true)
            {
                var ch = Reader.Read();
                if (ch == Quote[0])
                {
                    break;
                }

                // escaped?
                if (ch != _escseq[0])
                {
                    buffer.Write(new[] {ch}, 0, 1);
                    continue;
                }

                // distinguish between \uXXXX and \?
                ch = Reader.Read();
                if (ch != _escseq[1]) // control chars like \n
                {
                    var off = Array.IndexOf(_escapeChars, (char) ch);
                    if (off == -1)
                    {
                        throw new TProtocolException(TProtocolException.INVALID_DATA,
                            "Expected control char");
                    }
                    ch = _escapeCharVals[off];
                    buffer.Write(new[] {ch}, 0, 1);
                    continue;
                }


                // it's \uXXXX
                Trans.ReadAll(_tempBuffer, 0, 4);
                var wch = (short) ((HexVal(_tempBuffer[0]) << 12) +
                                   (HexVal(_tempBuffer[1]) << 8) +
                                   (HexVal(_tempBuffer[2]) << 4) +
                                   HexVal(_tempBuffer[3]));
                if (char.IsHighSurrogate((char) wch))
                {
                    if (codeunits.Count > 0)
                    {
                        throw new TProtocolException(TProtocolException.INVALID_DATA,
                            "Expected low surrogate char");
                    }
                    codeunits.Add((char) wch);
                }
                else if (char.IsLowSurrogate((char) wch))
                {
                    if (codeunits.Count == 0)
                    {
                        throw new TProtocolException(TProtocolException.INVALID_DATA,
                            "Expected high surrogate char");
                    }
                    codeunits.Add((char) wch);
                    var tmp = Utf8Encoding.GetBytes(codeunits.ToArray());
                    buffer.Write(tmp, 0, tmp.Length);
                    codeunits.Clear();
                }
                else
                {
                    var tmp = Utf8Encoding.GetBytes(new[] {(char) wch});
                    buffer.Write(tmp, 0, tmp.Length);
                }
            }


            if (codeunits.Count > 0)
            {
                throw new TProtocolException(TProtocolException.INVALID_DATA,
                    "Expected low surrogate char");
            }

            return buffer.ToArray();
        }

        ///<summary>
        /// Return true if the given byte could be a valid part of a JSON number.
        ///</summary>
        private bool IsJsonNumeric(byte b)
        {
            switch (b)
            {
                case (byte) '+':
                case (byte) '-':
                case (byte) '.':
                case (byte) '0':
                case (byte) '1':
                case (byte) '2':
                case (byte) '3':
                case (byte) '4':
                case (byte) '5':
                case (byte) '6':
                case (byte) '7':
                case (byte) '8':
                case (byte) '9':
                case (byte) 'E':
                case (byte) 'e':
                    return true;
            }
            return false;
        }

        ///<summary>
        /// Read in a sequence of characters that are all valid in JSON numbers. Does
        /// not do a complete regex check to validate that this is actually a number.
        ///</summary>
        private string ReadJsonNumericChars()
        {
            var strbld = new StringBuilder();
            while (true)
            {
                var ch = Reader.Peek();
                if (!IsJsonNumeric(ch))
                {
                    break;
                }
                strbld.Append((char) Reader.Read());
            }
            return strbld.ToString();
        }

        ///<summary>
        /// Read in a JSON number. If the context dictates, Read in enclosing quotes.
        ///</summary>
        private long ReadJsonInteger()
        {
            Context.Read();
            if (Context.EscapeNumbers())
            {
                ReadJsonSyntaxChar(Quote);
            }
            var str = ReadJsonNumericChars();
            if (Context.EscapeNumbers())
            {
                ReadJsonSyntaxChar(Quote);
            }
            try
            {
                return long.Parse(str);
            }
            catch (FormatException)
            {
                throw new TProtocolException(TProtocolException.INVALID_DATA,
                    "Bad data encounted in numeric data");
            }
        }

        ///<summary>
        /// Read in a JSON double value. Throw if the value is not wrapped in quotes
        /// when expected or if wrapped in quotes when not expected.
        ///</summary>
        private double ReadJsonDouble()
        {
            Context.Read();
            if (Reader.Peek() == Quote[0])
            {
                var arr = ReadJsonString(true);
                var dub = double.Parse(Utf8Encoding.GetString(arr, 0, arr.Length), CultureInfo.InvariantCulture);

                if (!Context.EscapeNumbers() && !double.IsNaN(dub) &&
                    !double.IsInfinity(dub))
                {
                    // Throw exception -- we should not be in a string in this case
                    throw new TProtocolException(TProtocolException.INVALID_DATA,
                        "Numeric data unexpectedly quoted");
                }
                return dub;
            }
            if (Context.EscapeNumbers())
            {
                // This will throw - we should have had a quote if escapeNum == true
                ReadJsonSyntaxChar(Quote);
            }
            try
            {
                return double.Parse(ReadJsonNumericChars(), CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new TProtocolException(TProtocolException.INVALID_DATA,
                    "Bad data encounted in numeric data");
            }
        }

        ///<summary>
        /// Read in a JSON string containing base-64 encoded data and decode it.
        ///</summary>
        private byte[] ReadJsonBase64()
        {
            var b = ReadJsonString(false);
            var len = b.Length;
            var off = 0;
            var size = 0;
            // reduce len to ignore fill bytes
            while ((len > 0) && (b[len - 1] == '='))
            {
                --len;
            }
            // read & decode full byte triplets = 4 source bytes
            while (len > 4)
            {
                // Decode 4 bytes at a time
                TBase64Utils.Decode(b, off, 4, b, size); // NB: decoded in place
                off += 4;
                len -= 4;
                size += 3;
            }
            // Don't decode if we hit the end or got a single leftover byte (invalid
            // base64 but legal for skip of regular string exType)
            if (len > 1)
            {
                // Decode remainder
                TBase64Utils.Decode(b, off, len, b, size); // NB: decoded in place
                size += len - 1;
            }
            // Sadly we must copy the byte[] (any way around this?)
            var result = new byte[size];
            Array.Copy(b, 0, result, 0, size);
            return result;
        }

        private void ReadJsonObjectStart()
        {
            Context.Read();
            ReadJsonSyntaxChar(Lbrace);
            PushContext(new JsonPairContext(this));
        }

        private void ReadJsonObjectEnd()
        {
            ReadJsonSyntaxChar(Rbrace);
            PopContext();
        }

        private void ReadJsonArrayStart()
        {
            Context.Read();
            ReadJsonSyntaxChar(Lbracket);
            PushContext(new JsonListContext(this));
        }

        private void ReadJsonArrayEnd()
        {
            ReadJsonSyntaxChar(Rbracket);
            PopContext();
        }

        public override TMessage ReadMessageBegin()
        {
            var message = new TMessage();
            ReadJsonArrayStart();
            if (ReadJsonInteger() != Version)
            {
                throw new TProtocolException(TProtocolException.BAD_VERSION,
                    "Message contained bad version.");
            }

            var buf = ReadJsonString(false);
            message.Name = Utf8Encoding.GetString(buf, 0, buf.Length);
            message.Type = (TMessageType) ReadJsonInteger();
            message.SeqID = (int) ReadJsonInteger();
            return message;
        }

        public override void ReadMessageEnd()
        {
            ReadJsonArrayEnd();
        }

        public override TStruct ReadStructBegin()
        {
            ReadJsonObjectStart();
            return new TStruct();
        }

        public override void ReadStructEnd()
        {
            ReadJsonObjectEnd();
        }

        public override TField ReadFieldBegin()
        {
            var field = new TField();
            var ch = Reader.Peek();
            if (ch == Rbrace[0])
            {
                field.Type = TType.Stop;
            }
            else
            {
                field.ID = (short) ReadJsonInteger();
                ReadJsonObjectStart();
                field.Type = GetTypeIdForTypeName(ReadJsonString(false));
            }
            return field;
        }

        public override void ReadFieldEnd()
        {
            ReadJsonObjectEnd();
        }

        public override TMap ReadMapBegin()
        {
            var map = new TMap();
            ReadJsonArrayStart();
            map.KeyType = GetTypeIdForTypeName(ReadJsonString(false));
            map.ValueType = GetTypeIdForTypeName(ReadJsonString(false));
            map.Count = (int) ReadJsonInteger();
            ReadJsonObjectStart();
            return map;
        }

        public override void ReadMapEnd()
        {
            ReadJsonObjectEnd();
            ReadJsonArrayEnd();
        }

        public override TList ReadListBegin()
        {
            var list = new TList();
            ReadJsonArrayStart();
            list.ElementType = GetTypeIdForTypeName(ReadJsonString(false));
            list.Count = (int) ReadJsonInteger();
            return list;
        }

        public override void ReadListEnd()
        {
            ReadJsonArrayEnd();
        }

        public override TSet ReadSetBegin()
        {
            var set = new TSet();
            ReadJsonArrayStart();
            set.ElementType = GetTypeIdForTypeName(ReadJsonString(false));
            set.Count = (int) ReadJsonInteger();
            return set;
        }

        public override void ReadSetEnd()
        {
            ReadJsonArrayEnd();
        }

        public override bool ReadBool()
        {
            return (ReadJsonInteger() != 0);
        }

        public override sbyte ReadByte()
        {
            return (sbyte) ReadJsonInteger();
        }

        public override short ReadI16()
        {
            return (short) ReadJsonInteger();
        }

        public override int ReadI32()
        {
            return (int) ReadJsonInteger();
        }

        public override long ReadI64()
        {
            return ReadJsonInteger();
        }

        public override double ReadDouble()
        {
            return ReadJsonDouble();
        }

        public override string ReadString()
        {
            var buf = ReadJsonString(false);
            return Utf8Encoding.GetString(buf, 0, buf.Length);
        }

        public override byte[] ReadBinary()
        {
            return ReadJsonBase64();
        }
    }
}