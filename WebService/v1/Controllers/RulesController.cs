// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class RulesController : Controller
    {
        private readonly IRules ruleService;

        public RulesController(IRules ruleService)
        {
            this.ruleService = ruleService;
        }

        [HttpGet("{id}")]
        public async Task<RuleApiModel> GetAsync([FromRoute] string id)
        {
            return new RuleApiModel(await this.ruleService.GetAsync(id));
        }

        [HttpGet]
        public async Task<RuleListApiModel> ListAsync(
            [FromQuery] string order,
            [FromQuery] int? skip,
            [FromQuery] int? limit,
            [FromQuery] string groupId)
        {
            if (order == null) order = "asc";
            if (skip == null) skip = 0;
            if (limit == null) limit = 1000;

            return new RuleListApiModel(
               await this.ruleService.GetListAsync(
                   order,
                   skip.Value,
                   limit.Value,
                   groupId));
        }

        [HttpPost]
        public async Task<RuleApiModel> PostAsync(
            [FromQuery] string template,
            [FromBody] JToken body)
        {
            if (!string.IsNullOrEmpty(template))
            {
                // create rules from template
                await this.ruleService.CreateFromTemplateAsync(template);
                return null;
            }
            
            // create rule from request body
            Rule newRule = await this.ruleService.CreateAsync(new Rule(body));

            return new RuleApiModel(newRule);
        }

        [HttpPut("{id}")]
        public async Task<RuleApiModel> PutAsync(
            [FromRoute] string id,
            [FromBody] JToken body)
        {
            Rule updatedRule = await this.ruleService.UpdateAsync(
                new Rule(body));

            return new RuleApiModel(updatedRule);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync([FromRoute] string id)
        {
            await this.ruleService.DeleteAsync(id);
        }
    }
}
