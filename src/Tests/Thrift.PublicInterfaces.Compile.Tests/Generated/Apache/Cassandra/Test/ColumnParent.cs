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

using Thrift.Protocols;
using Thrift.Protocols.Entities;
using Thrift.Protocols.Utilities;
using Thrift.Transports;
using Thrift.Transports.Client;
using Thrift.Transports.Server;


namespace Apache.Cassandra.Test
{

    /// <summary>
    /// ColumnParent is used when selecting groups of columns from the same ColumnFamily. In directory structure terms, imagine
    /// ColumnParent as ColumnPath + '/../'.
    /// 
    /// See also <a href="cassandra.html#Struct_ColumnPath">ColumnPath</a>
    /// </summary>
    [DataContract(Namespace="")]
    public partial class ColumnParent : TBase
    {
        private byte[] _super_column;

        [DataMember(Order = 0)]
        public string Column_family { get; set; }

        [DataMember(Order = 0)]
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


        [DataMember(Order = 1)]
        public Isset __isset;
        [DataContract]
        public struct Isset
        {
            [DataMember]
            public bool super_column;
        }

        #region XmlSerializer support

        public bool ShouldSerializeSuper_column()
        {
            return __isset.super_column;
        }

        #endregion XmlSerializer support

        public ColumnParent()
        {
        }

        public ColumnParent(string column_family) : this()
        {
            this.Column_family = column_family;
        }

        public async Task ReadAsync(TProtocol iprot, CancellationToken cancellationToken)
        {
            iprot.IncrementRecursionDepth();
            try
            {
                bool isset_column_family = false;
                TField field;
                await iprot.ReadStructBeginAsync(cancellationToken);
                while (true)
                {
                    field = await iprot.ReadFieldBeginAsync(cancellationToken);
                    if (field.Type == TType.Stop)
                    {
                        break;
                    }

                    switch (field.ID)
                    {
                        case 3:
                            if (field.Type == TType.String)
                            {
                                Column_family = await iprot.ReadStringAsync(cancellationToken);
                                isset_column_family = true;
                            }
                            else
                            {
                                await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
                            }
                            break;
                        case 4:
                            if (field.Type == TType.String)
                            {
                                Super_column = await iprot.ReadBinaryAsync(cancellationToken);
                            }
                            else
                            {
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
                if (!isset_column_family)
                {
                    throw new TProtocolException(TProtocolException.INVALID_DATA);
                }
            }
            finally
            {
                iprot.DecrementRecursionDepth();
            }
        }

        public async Task WriteAsync(TProtocol oprot, CancellationToken cancellationToken)
        {
            oprot.IncrementRecursionDepth();
            try
            {
                var struc = new TStruct("ColumnParent");
                await oprot.WriteStructBeginAsync(struc, cancellationToken);
                var field = new TField();
                field.Name = "column_family";
                field.Type = TType.String;
                field.ID = 3;
                await oprot.WriteFieldBeginAsync(field, cancellationToken);
                await oprot.WriteStringAsync(Column_family, cancellationToken);
                await oprot.WriteFieldEndAsync(cancellationToken);
                if (Super_column != null && __isset.super_column)
                {
                    field.Name = "super_column";
                    field.Type = TType.String;
                    field.ID = 4;
                    await oprot.WriteFieldBeginAsync(field, cancellationToken);
                    await oprot.WriteBinaryAsync(Super_column, cancellationToken);
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

        public override string ToString()
        {
            var sb = new StringBuilder("ColumnParent(");
            sb.Append(", Column_family: ");
            sb.Append(Column_family);
            if (Super_column != null && __isset.super_column)
            {
                sb.Append(", Super_column: ");
                sb.Append(Super_column);
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

}
