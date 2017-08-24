// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers
{
    public class QueryBuilder
    {
        public static String buildSQL(
            String schemaName,
            String byId,
            String byIdPropertyName,
            DateTimeOffset? from,
            String fromProperty,
            DateTimeOffset? to,
            String toProperty,
            String order,
            String orderProperty,
            int skip,
            int limit,
            String[] devices,
            String devicesProperty)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT TOP " + (skip + limit) + " * FROM c WHERE (c[`doc.schema`] = `" + schemaName + "`");

            if (devices.Length > 0)
            {
                String ids = String.Join("`,`", devices);
                queryBuilder.Append(" AND c[`" + devicesProperty + "`] IN (`" + ids + "`)");
            }

            if (byId != null)
            {
                queryBuilder.Append(" AND c[`" + byIdPropertyName + "`] = `" + byId + "`");
            }

            if (from.HasValue)
            {
                DateTimeOffset fromDate = from ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[`" + fromProperty + "`] >= " + fromDate.ToUnixTimeMilliseconds());
            }
            if (to.HasValue)
            {
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
