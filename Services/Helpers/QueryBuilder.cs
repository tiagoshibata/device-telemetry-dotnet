// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers
{
    public class QueryBuilder
    {
        public static string BuildSql(
            string schemaName,
            string byId,
            string byIdPropertyName,
            DateTimeOffset? from,
            string fromProperty,
            DateTimeOffset? to,
            string toProperty,
            string order,
            string orderProperty,
            int skip,
            int limit,
            string[] devices,
            string devicesProperty)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT TOP " + (skip + limit) + " * FROM c WHERE (c[`doc.schema`] = `" + schemaName + "`");

            if (devices.Length > 0)
            {
                var ids = string.Join("`,`", devices);
                queryBuilder.Append(" AND c[`" + devicesProperty + "`] IN (`" + ids + "`)");
            }

            if (byId != null)
            {
                queryBuilder.Append(" AND c[`" + byIdPropertyName + "`] = `" + byId + "`");
            }

            if (from.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset fromDate = from ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[`" + fromProperty + "`] >= " + fromDate.ToUnixTimeMilliseconds());
            }
            if (to.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset toDate = to ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[`" + toProperty + "`] <= " + toDate.ToUnixTimeMilliseconds());
            }
            queryBuilder.Append(")");

            if (order == null || string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase))
            {
                queryBuilder.Append(" ORDER BY c[`" + orderProperty + "`] DESC");
            }
            else
            {
                queryBuilder.Append(" ORDER BY c[`" + orderProperty + "`] ASC");
            }

            return queryBuilder.ToString().Replace("`", "\"");
        }
    }
}
