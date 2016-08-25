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
  [DataContract(Namespace="")]
  public partial class ListBonks : TBase
  {
    private List<Bonk> _bonk;

    [DataMember(Order = 0)]
    public List<Bonk> Bonk
    {
      get
      {
        return _bonk;
      }
      set
      {
        __isset.bonk = true;
        this._bonk = value;
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
      public bool bonk;
    }

    #region XmlSerializer support

    public bool ShouldSerializeBonk()
    {
      return __isset.bonk;
    }

    #endregion XmlSerializer support

    public ListBonks() {
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
                  Bonk = new List<Bonk>();
                  TList _list128 = iprot.ReadListBegin();
                  for( int _i129 = 0; _i129 < _list128.Count; ++_i129)
                  {
                    Bonk _elem130;
                    _elem130 = new Bonk();
                    _elem130.Read(iprot);
                    Bonk.Add(_elem130);
                  }
                  iprot.ReadListEnd();
                }
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
        TStruct struc = new TStruct("ListBonks");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Bonk != null && __isset.bonk) {
          field.Name = "bonk";
          field.Type = TType.List;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.Struct, Bonk.Count));
            foreach (Bonk _iter131 in Bonk)
            {
              _iter131.Write(oprot);
            }
            oprot.WriteListEnd();
          }
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
      StringBuilder __sb = new StringBuilder("ListBonks(");
      bool __first = true;
      if (Bonk != null && __isset.bonk) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Bonk: ");
        __sb.Append(Bonk);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
