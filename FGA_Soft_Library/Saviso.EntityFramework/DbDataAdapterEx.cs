using System.Data;
using System.Data.Common;

namespace Saviso.EntityFramework
{
    public class DbDataAdapterEx : DbDataAdapter
    {
        private readonly DbDataAdapter adapter;

        public DbDataAdapterEx(DbDataAdapter adapter)
        {
            this.adapter = adapter;
        }

        protected override void Dispose(bool disposing)
        {
            this.adapter.Dispose();
        }

        public override int Fill(DataSet dataSet)
        {
            if (base.SelectCommand != null)
            {
                this.adapter.SelectCommand = ((DbCommandEx) base.SelectCommand).Inner;
            }
            return this.adapter.Fill(dataSet);
        }

        public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            if (base.SelectCommand != null)
            {
                this.adapter.SelectCommand = ((DbCommandEx) base.SelectCommand).Inner;
            }
            return this.adapter.FillSchema(dataSet, schemaType);
        }

        public override IDataParameter[] GetFillParameters()
        {
            return this.adapter.GetFillParameters();
        }

        public override bool ShouldSerializeAcceptChangesDuringFill()
        {
            return this.adapter.ShouldSerializeAcceptChangesDuringFill();
        }

        public override bool ShouldSerializeFillLoadOption()
        {
            return this.adapter.ShouldSerializeFillLoadOption();
        }

        public override string ToString()
        {
            return this.adapter.ToString();
        }

        public override int Update(DataSet dataSet)
        {
            if (base.UpdateCommand != null)
            {
                this.adapter.UpdateCommand = ((DbCommandEx) base.UpdateCommand).Inner;
            }
            if (base.InsertCommand != null)
            {
                this.adapter.InsertCommand = ((DbCommandEx) base.InsertCommand).Inner;
            }
            if (base.DeleteCommand != null)
            {
                this.adapter.DeleteCommand = ((DbCommandEx) base.DeleteCommand).Inner;
            }
            return this.adapter.Update(dataSet);
        }

        public override bool ReturnProviderSpecificTypes
        {
            get
            {
                return this.adapter.ReturnProviderSpecificTypes;
            }
            set
            {
                this.adapter.ReturnProviderSpecificTypes = value;
            }
        }

        public override int UpdateBatchSize
        {
            get
            {
                return this.adapter.UpdateBatchSize;
            }
            set
            {
                this.adapter.UpdateBatchSize = value;
            }
        }
    }
}

