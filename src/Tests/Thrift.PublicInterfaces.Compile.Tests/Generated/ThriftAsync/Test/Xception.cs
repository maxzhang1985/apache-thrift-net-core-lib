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

namespace ThriftAsync.Test
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Xception : TException, TBase
  {
    private int _errorCode;
    private string _message;

    [DataMember(Order = 0)]
    public int ErrorCode
    {
      get
      {
        return _errorCode;
      }
      set
      {
        __isset.errorCode = true;
        this._errorCode = value;
      }
    }

    [DataMember(Order = 0)]
    public string Message
    {
      get
      {
        return _message;
      }
      set
      {
        __isset.message = true;
        this._message = value;
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
      public bool errorCode;
      [DataMember]
      public bool message;
    }

    #region XmlSerializer support

    public bool ShouldSerializeErrorCode()
    {
      return __isset.errorCode;
    }

    public bool ShouldSerializeMessage()
    {
      return __isset.message;
    }

    #endregion XmlSerializer support

    public Xception() {
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
                ErrorCode = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                Message = iprot.ReadString();
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
        TStruct struc = new TStruct("Xception");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.errorCode) {
          field.Name = "errorCode";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(ErrorCode);
          oprot.WriteFieldEnd();
        }
        if (Message != null && __isset.message) {
          field.Name = "message";
          field.Type = TType.String;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Message);
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
      StringBuilder __sb = new StringBuilder("Xception(");
      bool __first = true;
      if (__isset.errorCode) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ErrorCode: ");
        __sb.Append(ErrorCode);
      }
      if (Message != null && __isset.message) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Message: ");
        __sb.Append(Message);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }


  #if !SILVERLIGHT
  [Serializable]
  #endif
  [DataContract]
  public partial class XceptionFault
  {
    private int _errorCode;
    private string _message;

    [DataMember(Order = 0)]
    public int ErrorCode
    {
      get
      {
        return _errorCode;
      }
      set
      {
        this._errorCode = value;
      }
    }

    [DataMember(Order = 0)]
    public string Message
    {
      get
      {
        return _message;
      }
      set
      {
        this._message = value;
      }
    }

  }

}
