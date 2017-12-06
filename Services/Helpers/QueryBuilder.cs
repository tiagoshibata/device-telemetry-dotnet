// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Exceptions;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers
{
    public class QueryBuilder
    {
        private const string INVALID_CHARACTER = @"[^A-Za-z0-9:;.,_\-]";

        public static SqlQuerySpec GetDocumentsSql(
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
            var queryBuilder = new StringBuilder("SELECT TOP @top * FROM c WHERE (c[\"doc.schema\"] = @schemaName");

            if (devices.Length > 0)
            {
                queryBuilder.Append(" AND c[@devicesProperty] IN @devices");
            }

            if (byId != null)
            {
                queryBuilder.Append(" AND c[@byIdPropertyName] = @byId");
            }

            if (from.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset fromDate = from ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[@fromProperty] >= " + fromDate.ToUnixTimeMilliseconds());
            }

            if (to.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset toDate = to ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[@toProperty] <= " + toDate.ToUnixTimeMilliseconds());
            }

            queryBuilder.Append(")");

            if (order == null || string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase))
            {
                queryBuilder.Append(" ORDER BY c[@orderProperty] DESC");
            }
            else
            {
                queryBuilder.Append(" ORDER BY c[@orderProperty] ASC");
            }

            return new SqlQuerySpec(queryBuilder.ToString(),
                new SqlParameterCollection(new SqlParameter[] {
                    new SqlParameter { Name = "@top", Value = skip + limit },
                    new SqlParameter { Name = "@schemaName", Value = schemaName },
                    new SqlParameter { Name = "@devicesProperty", Value = devicesProperty },
                    new SqlParameter { Name = "@devices", Value = devices },
                    new SqlParameter { Name = "@byIdPropertyName", Value = byIdPropertyName },
                    new SqlParameter { Name = "@byId", Value = byId},
                    new SqlParameter { Name = "@fromProperty", Value = fromProperty},
                    new SqlParameter { Name = "@toProperty", Value = toProperty},
                    new SqlParameter { Name = "@orderProperty", Value = orderProperty },
                }));
        }

        public static SqlQuerySpec GetCountSql(
            string schemaName,
            string byId,
            string byIdProperty,
            DateTimeOffset? from,
            string fromProperty,
            DateTimeOffset? to,
            string toProperty,
            string[] devices,
            string devicesProperty,
            string[] filterValues,
            string filterProperty) 
        {
            // check for illegate characters in input
            ValidateInput(ref schemaName);
            ValidateInput(ref byId);
            ValidateInput(ref byIdProperty);
            ValidateInput(ref fromProperty);
            ValidateInput(ref toProperty);
            ValidateInput(ref devicesProperty);
            ValidateInput(ref filterProperty);

            // build query
            // TODO - GROUPBY and DISTINCT are not supported by documentDB yet, improve query once supported
            // https://github.com/Azure/device-telemetry-dotnet/issues/58
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT VALUE COUNT(1) FROM c WHERE (c[\"doc.schema\"] = @schemaName");

            if (devices.Length > 0)
            {
                queryBuilder.Append(" AND c[@devicesProperty] IN @devices");
            }

            if (byId != null)
            {
                queryBuilder.Append(" AND c[@byIdProperty] = @byId");
            }

            if (from.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset fromDate = from ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[@fromProperty] >= " + fromDate.ToUnixTimeMilliseconds());
            }

            if (to.HasValue)
            {
                // TODO: left operand is never null
                DateTimeOffset toDate = to ?? default(DateTimeOffset);
                queryBuilder.Append(" AND c[@toProperty] <= " + toDate.ToUnixTimeMilliseconds());
            }

            if (filterValues.Length > 0)
            {
                queryBuilder.Append(" AND c[@filterProperty] IN @filterValues");
            }

            queryBuilder.Append(")");

            return new SqlQuerySpec(queryBuilder.ToString(),
                new SqlParameterCollection(new SqlParameter[] {
                    new SqlParameter { Name = "@schemaName", Value = schemaName },
                    new SqlParameter { Name = "@devicesProperty", Value = devicesProperty },
                    new SqlParameter { Name = "@devices", Value = devices},
                    new SqlParameter { Name = "@byIdProperty", Value = byIdProperty },
                    new SqlParameter { Name = "@byId", Value = byId },
                    new SqlParameter { Name = "@fromProperty", Value = fromProperty },
                    new SqlParameter { Name = "@toProperty", Value = toProperty },
                    new SqlParameter { Name = "@filterProperty", Value = filterProperty },
                    new SqlParameter { Name = "@filterValues", Value = filterValues },
                }));
        }

        private static void ValidateInput(ref string input)
        {
            input = input.Trim();

            if (Regex.IsMatch(input, INVALID_CHARACTER))
            {
                throw new InvalidInputException(
                    "input '" + input + "' contains invalid characters.");
            }
        }
    }
}
