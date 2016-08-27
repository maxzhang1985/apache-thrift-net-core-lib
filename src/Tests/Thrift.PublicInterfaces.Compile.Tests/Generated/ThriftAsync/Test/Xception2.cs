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

  public partial class Xception2 : TException, TBase
  {
    private int _errorCode;
    private Xtruct _struct_thing;

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
    public Xtruct Struct_thing
    {
      get
      {
        return _struct_thing;
      }
      set
      {
        __isset.struct_thing = true;
        this._struct_thing = value;
      }
    }


    [DataMember(Order = 1)]
    public Isset __isset;
    [DataContract]
    public struct Isset
    {
      [DataMember]
      public bool errorCode;
      [DataMember]
      public bool struct_thing;
    }

    #region XmlSerializer support

    public bool ShouldSerializeErrorCode()
    {
      return __isset.errorCode;
    }

    public bool ShouldSerializeStruct_thing()
    {
      return __isset.struct_thing;
    }

    #endregion XmlSerializer support

    public Xception2() {
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
              if (field.Type == TType.I32) {
                ErrorCode = await iprot.ReadI32Async(cancellationToken);
              } else { 
               await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
              }
              break;
            case 2:
              if (field.Type == TType.Struct) {
                Struct_thing = new Xtruct();
                await Struct_thing.ReadAsync(iprot, cancellationToken);
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
        var struc = new TStruct("Xception2");
        await oprot.WriteStructBeginAsync(struc, cancellationToken);
        var field = new TField();
        if (__isset.errorCode) {
          field.Name = "errorCode";
          field.Type = TType.I32;
          field.ID = 1;
          await oprot.WriteFieldBeginAsync(field, cancellationToken);
          await oprot.WriteI32Async(ErrorCode, cancellationToken);
          await oprot.WriteFieldEndAsync(cancellationToken);
        }
        if (Struct_thing != null && __isset.struct_thing) {
          field.Name = "struct_thing";
          field.Type = TType.Struct;
          field.ID = 2;
          await oprot.WriteFieldBeginAsync(field, cancellationToken);
          await Struct_thing.WriteAsync(oprot, cancellationToken);
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
      var sb = new StringBuilder("Xception2(");
      bool __first = true;
      if (__isset.errorCode) {
        if(!__first) { sb.Append(", "); }
        __first = false;
        sb.Append("ErrorCode: ");
        sb.Append(ErrorCode);
      }
      if (Struct_thing != null && __isset.struct_thing) {
        if(!__first) { sb.Append(", "); }
        __first = false;
        sb.Append("Struct_thing: ");
        sb.Append(Struct_thing== null ? "<null>" : Struct_thing.ToString());
      }
      sb.Append(")");
      return sb.ToString();
    }

  }


  [DataContract]
  public partial class Xception2Fault
  {
    private int _errorCode;
    private Xtruct _struct_thing;

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
    public Xtruct Struct_thing
    {
      get
      {
        return _struct_thing;
      }
      set
      {
        this._struct_thing = value;
      }
    }

  }

}
