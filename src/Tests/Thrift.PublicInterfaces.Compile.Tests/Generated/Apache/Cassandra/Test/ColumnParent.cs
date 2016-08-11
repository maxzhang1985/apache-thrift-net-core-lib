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
  /// ColumnParent is used when selecting groups of columns from the same ColumnFamily. In directory structure terms, imagine
  /// ColumnParent as ColumnPath + '/../'.
  /// 
  /// See also <a href="cassandra.html#Struct_ColumnPath">ColumnPath</a>
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class ColumnParent : TBase
  {
    private byte[] _super_column;

    public string Column_family { get; set; }

    public byte[] Super_column
    {
      get
      {
        return _super_column;
      }
      set
      {
        __isset.super_column = true;
        this._super_column = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool super_column;
    }

    public ColumnParent() {
    }

    public ColumnParent(string column_family) : this() {
      this.Column_family = column_family;
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_column_family = false;
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
            case 3:
              if (field.Type == TType.String) {
                Column_family = iprot.ReadString();
                isset_column_family = true;
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.String) {
                Super_column = iprot.ReadBinary();
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
        if (!isset_column_family)
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
        TStruct struc = new TStruct("ColumnParent");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        field.Name = "column_family";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Column_family);
        oprot.WriteFieldEnd();
        if (Super_column != null && __isset.super_column) {
          field.Name = "super_column";
          field.Type = TType.String;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteBinary(Super_column);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("ColumnParent(");
      __sb.Append(", Column_family: ");
      __sb.Append(Column_family);
      if (Super_column != null && __isset.super_column) {
        __sb.Append(", Super_column: ");
        __sb.Append(Super_column);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
