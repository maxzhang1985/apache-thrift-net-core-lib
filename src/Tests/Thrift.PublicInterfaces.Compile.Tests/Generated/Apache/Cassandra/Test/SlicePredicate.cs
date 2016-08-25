/**
 * Autogenerated by Thrift Compiler (@PACKAGE_VERSION@)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Thrift;
using Thrift.Collections;
#if !SILVERLIGHT
using System.Xml.Serialization;
#endif
//using System.ServiceModel;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Apache.Cassandra.Test
{

  /// <summary>
  /// A SlicePredicate is similar to a mathematic predicate (see http://en.wikipedia.org/wiki/Predicate_(mathematical_logic)),
  /// which is described as "a property that the elements of a set have in common."
  /// 
  /// SlicePredicate's in Cassandra are described with either a list of column_names or a SliceRange.  If column_names is
  /// specified, slice_range is ignored.
  /// 
  /// @param column_name. A list of column names to retrieve. This can be used similar to Memcached's "multi-get" feature
  ///                     to fetch N known column names. For instance, if you know you wish to fetch columns 'Joe', 'Jack',
  ///                     and 'Jim' you can pass those column names as a list to fetch all three at once.
  /// @param slice_range. A SliceRange describing how to range, order, and/or limit the slice.
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  [DataContract(Namespace="")]
  public partial class SlicePredicate : TBase
  {
    private List<byte[]> _column_names;
    private SliceRange _slice_range;

    [DataMember(Order = 0)]
    public List<byte[]> Column_names
    {
      get
      {
        return _column_names;
      }
      set
      {
        __isset.column_names = true;
        this._column_names = value;
      }
    }

    [DataMember(Order = 0)]
    public SliceRange Slice_range
    {
      get
      {
        return _slice_range;
      }
      set
      {
        __isset.slice_range = true;
        this._slice_range = value;
      }
    }


    [XmlIgnore] // XmlSerializer
    [DataMember(Order = 1)]  // XmlObjectSerializer, DataContractJsonSerializer, etc.
    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    [DataContract]
    public struct Isset {
      [DataMember]
      public bool column_names;
      [DataMember]
      public bool slice_range;
    }

    #region XmlSerializer support

    public bool ShouldSerializeColumn_names()
    {
      return __isset.column_names;
    }

    public bool ShouldSerializeSlice_range()
    {
      return __isset.slice_range;
    }

    #endregion XmlSerializer support

    public SlicePredicate() {
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
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
              if (field.Type == TType.List) {
                {
                  Column_names = new List<byte[]>();
                  TList _list8 = iprot.ReadListBegin();
                  for( int _i9 = 0; _i9 < _list8.Count; ++_i9)
                  {
                    byte[] _elem10;
                    _elem10 = iprot.ReadBinary();
                    Column_names.Add(_elem10);
                  }
                  iprot.ReadListEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.Struct) {
                Slice_range = new SliceRange();
                Slice_range.Read(iprot);
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
        TStruct struc = new TStruct("SlicePredicate");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Column_names != null && __isset.column_names) {
          field.Name = "column_names";
          field.Type = TType.List;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.String, Column_names.Count));
            foreach (byte[] _iter11 in Column_names)
            {
              oprot.WriteBinary(_iter11);
            }
            oprot.WriteListEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (Slice_range != null && __isset.slice_range) {
          field.Name = "slice_range";
          field.Type = TType.Struct;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          Slice_range.Write(oprot);
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
      StringBuilder __sb = new StringBuilder("SlicePredicate(");
      bool __first = true;
      if (Column_names != null && __isset.column_names) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Column_names: ");
        __sb.Append(Column_names);
      }
      if (Slice_range != null && __isset.slice_range) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Slice_range: ");
        __sb.Append(Slice_range== null ? "<null>" : Slice_range.ToString());
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
