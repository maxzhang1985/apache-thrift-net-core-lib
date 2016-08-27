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


namespace Apache.Cassandra.Test
{

  [DataContract(Namespace="")]
  public partial class CounterSuperColumn : TBase
  {

    [DataMember(Order = 0)]
    public byte[] Name { get; set; }

    [DataMember(Order = 0)]
    public List<CounterColumn> Columns { get; set; }

    public CounterSuperColumn() {
    }

    public CounterSuperColumn(byte[] name, List<CounterColumn> columns) : this() {
      this.Name = name;
      this.Columns = columns;
    }

    public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        bool isset_name = false;
        bool isset_columns = false;
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
              if (field.Type == TType.String) {
                Name = await iprot.ReadBinaryAsync(cancellationToken);
                isset_name = true;
              } else { 
               await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
              }
              break;
            case 2:
              if (field.Type == TType.List) {
                {
                  Columns = new List<CounterColumn>();
                  TList _list4 = await iprot.ReadListBeginAsync(cancellationToken);
                  for(int _i5 = 0; _i5 < _list4.Count; ++_i5)
                  {
                    CounterColumn _elem6;
                    _elem6 = new CounterColumn();
                    await _elem6.ReadAsync(iprot, cancellationToken);
                    Columns.Add(_elem6);
                  }
                  await iprot.ReadListEndAsync(cancellationToken);
                }
                isset_columns = true;
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
        if (!isset_name)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
        if (!isset_columns)
          throw new TProtocolException(TProtocolException.INVALID_DATA);
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
        var struc = new TStruct("CounterSuperColumn");
        await oprot.WriteStructBeginAsync(struc, cancellationToken);
        var field = new TField();
        field.Name = "name";
        field.Type = TType.String;
        field.ID = 1;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        await oprot.WriteBinaryAsync(Name, cancellationToken);
        await oprot.WriteFieldEndAsync(cancellationToken);
        field.Name = "columns";
        field.Type = TType.List;
        field.ID = 2;
        await oprot.WriteFieldBeginAsync(field, cancellationToken);
        {
          await oprot.WriteListBeginAsync(new TList(TType.Struct, Columns.Count), cancellationToken);
          foreach (CounterColumn _iter7 in Columns)
          {
            await _iter7.WriteAsync(oprot, cancellationToken);
          }
          await oprot.WriteListEndAsync(cancellationToken);
        }
        await oprot.WriteFieldEndAsync(cancellationToken);
        await oprot.WriteFieldStopAsync(cancellationToken);
        await oprot.WriteStructEndAsync(cancellationToken);
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      var sb = new StringBuilder("CounterSuperColumn(");
      sb.Append(", Name: ");
      sb.Append(Name);
      sb.Append(", Columns: ");
      sb.Append(Columns);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
