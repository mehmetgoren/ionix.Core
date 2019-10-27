namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    partial class EntityCommandSelect
    {
        protected internal async Task<object[]> QueryTemplateSingleAsync(IEntityMetaDataProvider provider, SqlQuery query, params Type[] types)
        {
            this.CheckParams(provider, query, types);

            IDataReader dr = null;
            try
            {
                dr = await this.DataAccess.CreateDataReaderAsync(query, CommandBehavior.SingleRow);

                if (dr.Read())
                {
                    return this.MapMulti(provider, types, dr);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return null;
        }
        protected internal virtual async Task<IList<object[]>> QueryTemplateAsync(IEntityMetaDataProvider provider, SqlQuery query, params Type[] types)
        {
            this.CheckParams(provider, query, types);

            List<object[]> ret = new List<object[]>();
            IDataReader dr = null;
            try
            {
                dr = await this.DataAccess.CreateDataReaderAsync(query, CommandBehavior.Default);

                while (dr.Read())
                {
                    ret.Add(this.MapMulti(provider, types, dr));
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }

        public async Task<(TEntity1, TEntity2)> QuerySingleAsync<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1]);
            }
            return default((TEntity1, TEntity2));
        }
        public async Task<(TEntity1, TEntity2, TEntity3)> QuerySingleAsync<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2]);
            }
            return default((TEntity1, TEntity2, TEntity3));
        }
        public async Task<(TEntity1, TEntity2, TEntity3, TEntity4)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3]);
            }
            return default((TEntity1, TEntity2, TEntity3, TEntity4));
        }
        public async Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4]);
            }
            return default((TEntity1, TEntity2, TEntity3, TEntity4, TEntity5));
        }

        public async Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4], (TEntity6)result[5]);
            }
            return default((TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6));
        }
        public async Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateSingleAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6), typeof(TEntity7));
            if (null != result)
            {
                return ((TEntity1)result[0], (TEntity2)result[1], (TEntity3)result[2], (TEntity4)result[3], (TEntity5)result[4], (TEntity6)result[5], (TEntity7)result[6]);
            }
            return default((TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7));
        }



        public async Task<IList<(TEntity1, TEntity2)>> QueryAsync<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2));
            var ret = new List<(TEntity1, TEntity2)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1]));
                }
            }
            return ret;
        }
        public async Task<IList<(TEntity1, TEntity2, TEntity3)>> QueryAsync<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3));
            var ret = new List<(TEntity1, TEntity2, TEntity3)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2]));
                }
            }
            return ret;
        }
        public async Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4));
            var ret = new List<(TEntity1, TEntity2, TEntity3, TEntity4)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3]));
                }
            }
            return ret;
        }
        public async Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5));
            var ret = new List<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4]));
                }
            }
            return ret;
        }
        public async Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            var ret = new List<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4], (TEntity6)arr[5]));
                }
            }
            return ret;
        }
        public async Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            var result = await this.QueryTemplateAsync(provider, query, typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(TEntity6));
            var ret = new List<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)>(result.Count);
            if (result.Count != 0)
            {
                foreach (object[] arr in result)
                {
                    ret.Add(((TEntity1)arr[0], (TEntity2)arr[1], (TEntity3)arr[2], (TEntity4)arr[3], (TEntity5)arr[4], (TEntity6)arr[5], (TEntity7)arr[6]));
                }
            }
            return ret;
        }
    }
}
