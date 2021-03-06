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
    /// A Mutation is either an insert (represented by filling column_or_supercolumn) or a deletion (represented by filling the deletion attribute).
    /// @param column_or_supercolumn. An insert to a column or supercolumn (possibly counter column or supercolumn)
    /// @param deletion. A deletion of a column or supercolumn
    /// </summary>
    [DataContract(Namespace="")]
    public partial class Mutation : TBase
    {
        private ColumnOrSuperColumn _column_or_supercolumn;
        private Deletion _deletion;

        [DataMember(Order = 0)]
        public ColumnOrSuperColumn Column_or_supercolumn
        {
            get
            {
                return _column_or_supercolumn;
            }
            set
            {
                __isset.column_or_supercolumn = true;
                this._column_or_supercolumn = value;
            }
        }

        [DataMember(Order = 0)]
        public Deletion Deletion
        {
            get
            {
                return _deletion;
            }
            set
            {
                __isset.deletion = true;
                this._deletion = value;
            }
        }


        [DataMember(Order = 1)]
        public Isset __isset;
        [DataContract]
        public struct Isset
        {
            [DataMember]
            public bool column_or_supercolumn;
            [DataMember]
            public bool deletion;
        }

        #region XmlSerializer support

        public bool ShouldSerializeColumn_or_supercolumn()
        {
            return __isset.column_or_supercolumn;
        }

        public bool ShouldSerializeDeletion()
        {
            return __isset.deletion;
        }

        #endregion XmlSerializer support

        public Mutation()
        {
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
                    if (field.Type == TType.Stop)
                    {
                        break;
                    }

                    switch (field.ID)
                    {
                        case 1:
                            if (field.Type == TType.Struct)
                            {
                                Column_or_supercolumn = new ColumnOrSuperColumn();
                                await Column_or_supercolumn.ReadAsync(iprot, cancellationToken);
                            }
                            else
                            {
                                await TProtocolUtil.SkipAsync(iprot, field.Type, cancellationToken);
                            }
                            break;
                        case 2:
                            if (field.Type == TType.Struct)
                            {
                                Deletion = new Deletion();
                                await Deletion.ReadAsync(iprot, cancellationToken);
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
                var struc = new TStruct("Mutation");
                await oprot.WriteStructBeginAsync(struc, cancellationToken);
                var field = new TField();
                if (Column_or_supercolumn != null && __isset.column_or_supercolumn)
                {
                    field.Name = "column_or_supercolumn";
                    field.Type = TType.Struct;
                    field.ID = 1;
                    await oprot.WriteFieldBeginAsync(field, cancellationToken);
                    await Column_or_supercolumn.WriteAsync(oprot, cancellationToken);
                    await oprot.WriteFieldEndAsync(cancellationToken);
                }
                if (Deletion != null && __isset.deletion)
                {
                    field.Name = "deletion";
                    field.Type = TType.Struct;
                    field.ID = 2;
                    await oprot.WriteFieldBeginAsync(field, cancellationToken);
                    await Deletion.WriteAsync(oprot, cancellationToken);
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
            var sb = new StringBuilder("Mutation(");
            bool __first = true;
            if (Column_or_supercolumn != null && __isset.column_or_supercolumn)
            {
                if(!__first) { sb.Append(", "); }
                __first = false;
                sb.Append("Column_or_supercolumn: ");
                sb.Append(Column_or_supercolumn== null ? "<null>" : Column_or_supercolumn.ToString());
            }
            if (Deletion != null && __isset.deletion)
            {
                if(!__first) { sb.Append(", "); }
                __first = false;
                sb.Append("Deletion: ");
                sb.Append(Deletion== null ? "<null>" : Deletion.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

}
