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
using System.Threading;
using System.Threading.Tasks;
using Thrift;
using Thrift.Collections;
using System.ServiceModel;
using System.Runtime.Serialization;

using Thrift.Protocol;
using Thrift.Transport;


namespace ThriftAsync.Test
{

  [DataContract(Namespace="")]
  public partial class BoolTest : TBase
  {
    private bool _b;
    private string _s;

    [DataMember(Order = 0)]
    public bool B
    {
      get
      {
        return _b;
      }
      set
      {
        __isset.b = true;
        this._b = value;
      }
    }

    [DataMember(Order = 0)]
    public string S
    {
      get
      {
        return _s;
      }
      set
      {
        __isset.s = true;
        this._s = value;
      }
    }


    [DataMember(Order = 1)]
    public Isset __isset;
    [DataContract]
    public struct Isset
    {
      [DataMember]
      public bool b;
      [DataMember]
      public bool s;
    }

    #region XmlSerializer support

    public bool ShouldSerializeB()
    {
      return __isset.b;
    }

    public bool ShouldSerializeS()
    {
      return __isset.s;
    }

    #endregion XmlSerializer support

    public BoolTest() {
      this._b = true;
      this.__isset.b = true;
      this._s = "true";
      this.__isset.s = true;
    }

    public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        await iprot.ReadStructBeginAsync(cancellationToken);
        while (true)
        {
          field = await iprot.ReadFieldBeginAsync(cancellationToken);
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.Bool) {
                B = await iprot.ReadBoolAsync(cancellationToken);
              } else { 
               await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                S = await iprot.ReadStringAsync(cancellationToken);
              } else { 
               await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
              }
              break;
            default: 
              await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
              break;
          }
          await iprot.ReadFieldEndAsync(cancellationToken);
        }
        await iprot.ReadStructEndAsync(cancellationToken);
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public async Task WriteAsync(TProtocol oprot, CancellationToken cancellationToken) {
      oprot.IncrementRecursionDepth();
      try
      {
        var struc = new TStruct("BoolTest");
        await oprot.WriteStructBeginAsync(struc, cancellationToken);
        var field = new TField();
        if (__isset.b) {
          field.Name = "b";
          field.Type = TType.Bool;
          field.ID = 1;
          await oprot.WriteFieldBeginAsync(field, cancellationToken);
          await oprot.WriteBoolAsync(B, cancellationToken);
          await oprot.WriteFieldEndAsync(cancellationToken);
        }
        if (S != null && __isset.s) {
          field.Name = "s";
          field.Type = TType.String;
          field.ID = 2;
          await oprot.WriteFieldBeginAsync(field, cancellationToken);
          await oprot.WriteStringAsync(S, cancellationToken);
          await oprot.WriteFieldEndAsync(cancellationToken);
        }
        await oprot.WriteFieldStopAsync(cancellationToken);
        await oprot.WriteStructEndAsync(cancellationToken);
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      var sb = new StringBuilder("BoolTest(");
      bool __first = true;
      if (__isset.b) {
        if(!__first) { sb.Append(", "); }
        __first = false;
        sb.Append("B: ");
        sb.Append(B);
      }
      if (S != null && __isset.s) {
        if(!__first) { sb.Append(", "); }
        __first = false;
        sb.Append("S: ");
        sb.Append(S);
      }
      sb.Append(")");
      return sb.ToString();
    }

  }

}
