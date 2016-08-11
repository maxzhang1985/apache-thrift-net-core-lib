/**
 * Autogenerated by Thrift Compiler (0.9.3)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Apache.Cassandra.Test
{

  /// <summary>
  /// A KeySlice is key followed by the data it maps to. A collection of KeySlice is returned by the get_range_slice operation.
  /// 
  /// @param key. a row key
  /// @param columns. List of data represented by the key. Typically, the list is pared down to only the columns specified by
  ///                 a SlicePredicate.
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class KeySlice : TBase
  {

    public byte[] Key { get; set; }

    public List<ColumnOrSuperColumn> Columns { get; set; }

    public KeySlice() {
    }

    public KeySlice(byte[] key, List<ColumnOrSuperColumn> columns) : this() {
      this.Key = key;
      this.Columns = columns;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_key = false;
        bool isset_columns = false;
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.String) {
                Key = iprot.ReadBinary();
                isset_key = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.List) {
                {
                  Columns = new List<ColumnOrSuperColumn>();
                  TList _list16 = iprot.ReadListBegin();
                  for( int _i17 = 0; _i17 < _list16.Count; ++_i17)
                  {
                    ColumnOrSuperColumn _elem18;
                    _elem18 = new ColumnOrSuperColumn();
                    _elem18.Read(iprot);
                    Columns.Add(_elem18);
                  }
                  iprot.ReadListEnd();
                }
                isset_columns = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
        if (!isset_key)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_columns)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("KeySlice");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "key";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteBinary(Key);
        oprot.WriteFieldEnd();
        field.Name = "columns";
        field.Type = TType.List;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, Columns.Count));
          foreach (ColumnOrSuperColumn _iter19 in Columns)
          {
            _iter19.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("KeySlice(");
      __sb.Append(", Key: ");
      __sb.Append(Key);
      __sb.Append(", Columns: ");
      __sb.Append(Columns);
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
