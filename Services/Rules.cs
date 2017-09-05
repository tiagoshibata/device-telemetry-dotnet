// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rule = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Rule;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services
{
    public interface IRules
    {
        Task CreateFromTemplateAsync(string template);

        Task DeleteAsync(string id);

        Task<Rule> GetAsync(string id);

        Task<List<Rule>> GetListAsync(
            string order,
            int skip,
            int limit,
            string groupId);

        Task<Rule> CreateAsync(Rule rule);

        Task<Rule> UpdateAsync(Rule rule);
    }

    public class Rules : IRules
    {
        private const string StorageCollection = "rules";

        private const string INVALID_CHARACTER = @"[^A-Za-z0-9:;.,_\-]";
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";

        private readonly IStorageAdapterClient storage;
        private readonly ILogger log;

        public Rules(
            IStorageAdapterClient storage,
            ILogger logger)
        {
            this.storage = storage;
            this.log = logger;
        }

        public async Task CreateFromTemplateAsync(string template)
        {
            string pathToTemplate = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                @"Data\Rules\" + template + ".json");

            if (RuleTemplateValidator.IsValid(pathToTemplate))
            {
                var file = JToken.Parse(File.ReadAllText(pathToTemplate));

                foreach (var item in file["Rules"])
                {
                    Rule newRule = new Rule(item);

                    await this.CreateAsync(newRule);
                }
            }
        }

        public async Task DeleteAsync(string id)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                log.Debug("id contains illegal characters.", () => new { id });
                throw new InvalidInputException("id contains illegal characters.");
            }

            await this.storage.DeleteAsync(StorageCollection, id);
        }

        public async Task<Rule> GetAsync(string id)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                log.Debug("id contains illegal characters.", () => new { id });
                throw new InvalidInputException("id contains illegal characters.");
            }

            var item = await this.storage.GetAsync(StorageCollection, id);
            var rule = JsonConvert.DeserializeObject<Rule>(item.Data);

            rule.ETag = item.ETag;
            rule.Id = item.Key;

            return rule;
        }

        public async Task<List<Rule>> GetListAsync(
            string order,
            int skip,
            int limit,
            string groupId)
        {
            var data = await this.storage.GetAllAsync(StorageCollection);
            var ruleList = new List<Rule>();
            foreach (var item in data.Items)
            {
                try
                {
                    var rule = JsonConvert.DeserializeObject<Rule>(item.Data);
                    rule.ETag = item.ETag;
                    rule.Id = item.Key;

                    if (string.IsNullOrEmpty(groupId) ||
                        rule.GroupId.Equals(groupId, StringComparison.OrdinalIgnoreCase))
                    {
                        ruleList.Add(rule);
                    }
                }
                catch (Exception e)
                {
                    log.Debug("Could not parse result from Key Value Storage",
                        () => new { e });
                    throw new InvalidDataException(
                        "Could not parse result from Key Value Storage", e);
                }
            }

            // sort based on DateCreated, default descending
            ruleList.Sort();

            if (order.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                ruleList.Reverse();
            }

            if (skip >= ruleList.Count)
            {
                this.log.Debug("Skip value greater than size of list returned",
                    () => new { skip, ruleList.Count });

                return new List<Rule>();
            }
            else if ((limit + skip) >= ruleList.Count)
            {
                // if requested values are out of range, return remaining items
                return ruleList.GetRange(skip, ruleList.Count - skip);
            }

            return ruleList.GetRange(skip, limit);
        }

        public async Task<Rule> CreateAsync(Rule rule)
        {
            var item = JsonConvert.SerializeObject(rule);
            var result = await this.storage.CreateAsync(StorageCollection, item);

            Rule newRule = new Rule(JToken.Parse(result.Data));
            newRule.ETag = result.ETag;
            newRule.Id = result.Key;

            return newRule;
        }

        public async Task<Rule> UpdateAsync(Rule rule)
        {
            rule.DateModified = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

            var item = JsonConvert.SerializeObject(rule);
            var result = await this.storage.UpdateAsync(
                StorageCollection,
                rule.Id,
                item,
                rule.ETag);

            Rule updatedRule = new Rule(JToken.Parse(result.Data));

            updatedRule.ETag = result.ETag;
            updatedRule.Id = result.Key;

            return updatedRule;
        }
    }
}
