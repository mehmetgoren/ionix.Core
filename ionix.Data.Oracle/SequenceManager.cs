namespace Ionix.Data.Oracle
{
    using Utils;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal static class SequenceManager
    {
        private static readonly CultureInfo EnCulture = new CultureInfo("en-US");
        private static readonly Dictionary<string, string> dic = new Dictionary<string, string>(8);

        private static readonly object SyncRoot = new object();
        internal static string GetSequenceName(IDbAccess dataAccess, string tableName, string pkColumn)
        {
            lock (SyncRoot)
            {
                tableName = tableName.ToUpper(EnCulture);//Oracle Büyük Harf istiyor.

                string sequenceName;
                if (!dic.TryGetValue(tableName, out sequenceName))
                {
                    sequenceName = SequenceManager.GetSequenceName(dataAccess, tableName, pkColumn, true).ToString();

                    dic.Add(tableName, sequenceName);
                }

                return sequenceName;
            }
        }

        private static string GetSequenceName(IDbAccess dataAccess, string tableName, string pkColumnName, bool checkSequence)
        {
            if (dataAccess == null)
                throw new ArgumentNullException(nameof(dataAccess));
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrEmpty(pkColumnName))
                throw new ArgumentNullException(nameof(pkColumnName));

            string[] arr = tableName.Split('.');
            bool withUser = arr.Length > 1;
            string sequenceName = null;
            if (withUser)
                sequenceName = arr[0] + ".SQE_" + arr[1];
            else
                sequenceName = "SQE_" + tableName;

            if (checkSequence)
            {
                object temp = null;
                decimal ret = 0M, minVal = 1M, curVal = 0M;

                SqlQuery query = new SqlQuery();
                StringBuilder text = query.Text;

                try
                {
                    text.Append("SELECT MAX(");
                    text.Append(tableName);
                    text.Append('.');
                    text.Append(pkColumnName);
                    text.Append(") FROM ");
                    text.Append(tableName);
                    temp = dataAccess.ExecuteScalar(query);
                    if (temp != null && temp.GetType() != CachedTypes.DBNull)
                    {
                        minVal = ((Decimal)temp) + 1M;
                    }

                    query.Clear();
                    text = query.Text;

                    text.Append(" SELECT COUNT(*) FROM");
                    if (withUser)
                    {
                        text.Append(" ALL_SEQUENCES T WHERE T.SEQUENCE_OWNER = :SEQUENCE_OWNER AND");
                        query.Parameters.Add("SEQUENCE_OWNER", arr[0]);
                        query.Parameters.Add("SEQUENCE_NAME", "SQE_" + arr[1]);
                    }
                    else
                    {
                        text.Append(" USER_SEQUENCES T WHERE");
                        query.Parameters.Add("SEQUENCE_NAME", sequenceName);
                    }
                    text.Append(" T.SEQUENCE_NAME = :SEQUENCE_NAME");
                    ret = (Decimal)dataAccess.ExecuteScalar(query);
                    if (ret == 0M)
                    {
                        query.Clear();
                        text = query.Text;

                        text.Append("CREATE SEQUENCE ");
                        text.Append(sequenceName);
                        text.AppendLine();
                        text.Append("MINVALUE 0");
                        text.AppendLine();
                        text.Append("MAXVALUE 9999999999999999999999999");
                        text.AppendLine();
                        text.Append("START WITH ");
                        text.Append(minVal);
                        text.AppendLine();
                        text.Append("INCREMENT BY 1");
                        text.AppendLine();
                        text.Append("CACHE 20");
                        text.AppendLine();

                        dataAccess.ExecuteNonQuery(query);
                    }
                    else
                    {
                        query.Clear();
                        text = query.Text;

                        text.Append("SELECT T.LAST_NUMBER FROM");
                        if (withUser)
                        {
                            text.Append(" ALL_SEQUENCES T WHERE T.SEQUENCE_OWNER = :SEQUENCE_OWNER AND T.SEQUENCE_NAME = :SEQUENCE_NAME");
                            query.Parameters.Add("SEQUENCE_OWNER", arr[0]);
                            query.Parameters.Add("SEQUENCE_NAME", "SQE_" + arr[1]);
                        }
                        else
                        {
                            text.Append(" USER_SEQUENCES T WHERE T.SEQUENCE_NAME = :SEQUENCE_NAME");
                            query.Parameters.Add("SEQUENCE_NAME", sequenceName);
                        }
                        curVal = (Decimal)dataAccess.ExecuteScalar(query);
                        if (minVal > curVal)
                        {
                            query.Clear();
                            text = query.Text;

                            text.Append("ALTER SEQUENCE ");
                            text.Append(sequenceName);
                            text.Append(" INCREMENT BY :INC;");
                            query.Parameters.Add("INC", minVal - curVal);
                            text.AppendLine();
                            text.Append("SELECT ");
                            text.Append(sequenceName);
                            text.Append(".NEXTVAL FROM DUAL;");
                            text.AppendLine();
                            text.Append("ALTER SEQUENCE ");
                            text.Append(sequenceName);
                            text.Append(" INCREMENT BY 1;");
                            dataAccess.ExecuteScalar(query);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"An error occurred while creating a sequence for '{tableName}' Oracle table. Error detail: '{ex.Message}'");
                }
            }
            return sequenceName;
        }
    }
}
