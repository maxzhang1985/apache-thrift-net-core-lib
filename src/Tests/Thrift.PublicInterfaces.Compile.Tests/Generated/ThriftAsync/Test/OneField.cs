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


namespace ThriftAsync.Test
{

    [DataContract(Namespace="")]
    public partial class OneField : TBase
    {
        private EmptyStruct _field;

        [DataMember(Order = 0)]
        public EmptyStruct Field
        {
            get
            {
                return _field;
            }
            set
            {
                __isset.field = true;
                this._field = value;
            }
        }


        [DataMember(Order = 1)]
        public Isset __isset;
        [DataContract]
        public struct Isset
        {
            [DataMember]
            public bool field;
        }

        #region XmlSerializer support

        public bool ShouldSerializeField()
        {
            return __isset.field;
        }

        #endregion XmlSerializer support

        public OneField()
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
                                Field = new EmptyStruct();
                                await Field.ReadAsync(iprot, cancellationToken);
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
                var struc = new TStruct("OneField");
                await oprot.WriteStructBeginAsync(struc, cancellationToken);
                var field = new TField();
                if (Field != null && __isset.field)
                {
                    field.Name = "field";
                    field.Type = TType.Struct;
                    field.ID = 1;
                    await oprot.WriteFieldBeginAsync(field, cancellationToken);
                    await Field.WriteAsync(oprot, cancellationToken);
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
            var sb = new StringBuilder("OneField(");
            bool __first = true;
            if (Field != null && __isset.field)
            {
                if(!__first) { sb.Append(", "); }
                __first = false;
                sb.Append("Field: ");
                sb.Append(Field== null ? "<null>" : Field.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

}
