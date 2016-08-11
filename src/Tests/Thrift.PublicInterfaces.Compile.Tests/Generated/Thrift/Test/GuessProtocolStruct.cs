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

namespace Thrift.Test
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class GuessProtocolStruct : TBase
  {
    private Dictionary<string, string> _map_field;

    public Dictionary<string, string> Map_field
    {
      get
      {
        return _map_field;
      }
      set
      {
        __isset.map_field = true;
        this._map_field = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool map_field;
    }

    public GuessProtocolStruct() {
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
            case 7:
              if (field.Type == TType.Map) {
                {
                  Map_field = new Dictionary<string, string>();
                  TMap _map65 = iprot.ReadMapBegin();
                  for( int _i66 = 0; _i66 < _map65.Count; ++_i66)
                  {
                    string _key67;
                    string _val68;
                    _key67 = iprot.ReadString();
                    _val68 = iprot.ReadString();
                    Map_field[_key67] = _val68;
                  }
                  iprot.ReadMapEnd();
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
        TStruct struc = new TStruct("GuessProtocolStruct");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Map_field != null && __isset.map_field) {
          field.Name = "map_field";
          field.Type = TType.Map;
          field.ID = 7;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteMapBegin(new TMap(TType.String, TType.String, Map_field.Count));
            foreach (string _iter69 in Map_field.Keys)
            {
              oprot.WriteString(_iter69);
              oprot.WriteString(Map_field[_iter69]);
            }
            oprot.WriteMapEnd();
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
      StringBuilder __sb = new StringBuilder("GuessProtocolStruct(");
      bool __first = true;
      if (Map_field != null && __isset.map_field) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Map_field: ");
        __sb.Append(Map_field);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
