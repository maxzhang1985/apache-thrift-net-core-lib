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

namespace Thrift.Test
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  [DataContract(Namespace="")]
  public partial class VersioningTestV1 : TBase
  {
    private int _begin_in_both;
    private string _old_string;
    private int _end_in_both;

    [DataMember(Order = 0)]
    public int Begin_in_both
    {
      get
      {
        return _begin_in_both;
      }
      set
      {
        __isset.begin_in_both = true;
        this._begin_in_both = value;
      }
    }

    [DataMember(Order = 0)]
    public string Old_string
    {
      get
      {
        return _old_string;
      }
      set
      {
        __isset.old_string = true;
        this._old_string = value;
      }
    }

    [DataMember(Order = 0)]
    public int End_in_both
    {
      get
      {
        return _end_in_both;
      }
      set
      {
        __isset.end_in_both = true;
        this._end_in_both = value;
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
      public bool begin_in_both;
      [DataMember]
      public bool old_string;
      [DataMember]
      public bool end_in_both;
    }

    #region XmlSerializer support

    public bool ShouldSerializeBegin_in_both()
    {
      return __isset.begin_in_both;
    }

    public bool ShouldSerializeOld_string()
    {
      return __isset.old_string;
    }

    public bool ShouldSerializeEnd_in_both()
    {
      return __isset.end_in_both;
    }

    #endregion XmlSerializer support

    public VersioningTestV1() {
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
              if (field.Type == TType.I32) {
                Begin_in_both = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.String) {
                Old_string = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 12:
              if (field.Type == TType.I32) {
                End_in_both = iprot.ReadI32();
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
        TStruct struc = new TStruct("VersioningTestV1");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.begin_in_both) {
          field.Name = "begin_in_both";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(Begin_in_both);
          oprot.WriteFieldEnd();
        }
        if (Old_string != null && __isset.old_string) {
          field.Name = "old_string";
          field.Type = TType.String;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Old_string);
          oprot.WriteFieldEnd();
        }
        if (__isset.end_in_both) {
          field.Name = "end_in_both";
          field.Type = TType.I32;
          field.ID = 12;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(End_in_both);
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
      StringBuilder __sb = new StringBuilder("VersioningTestV1(");
      bool __first = true;
      if (__isset.begin_in_both) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Begin_in_both: ");
        __sb.Append(Begin_in_both);
      }
      if (Old_string != null && __isset.old_string) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Old_string: ");
        __sb.Append(Old_string);
      }
      if (__isset.end_in_both) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("End_in_both: ");
        __sb.Append(End_in_both);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
