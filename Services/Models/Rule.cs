// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models
{
    public class Rule : IComparable<Rule>
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";
        [JsonIgnore] //comes from the StorageAdapter document and not the serialized rule
        public string ETag { get; set; } = string.Empty;
        [JsonIgnore] //comes from the StorageAdapter document and not the serialized rule
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DateCreated { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);
        public string DateModified { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);
        public bool Enabled { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public IList<Condition> Conditions { get; set; } = new List<Condition>();

        public Rule() { }

        public int CompareTo(Rule other)
        {
            if (other == null) return 1;

            return DateTimeOffset.Parse(other.DateCreated)
                .CompareTo(DateTimeOffset.Parse(this.DateCreated));
        }
    }
}
