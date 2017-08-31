// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers.Helpers;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Exceptions;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public class AlarmsController : Controller
    {
        private readonly IAlarms alarmService;
        private readonly ILogger log;

        private const int LIMIT = 200;

        public AlarmsController(
            IAlarms alarmService,
            ILogger logger)
        {
            this.alarmService = alarmService;
            this.log = logger;
        }

        [HttpGet]
        public AlarmListApiModel List(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] string order,
            [FromQuery] int? skip,
            [FromQuery] int? limit,
            [FromQuery] string devices)
        {
            DateTimeOffset? fromDate = DateHelper.parseDate(from);
            DateTimeOffset? toDate = DateHelper.parseDate(to);

            if (order == null) order = "asc";
            if (skip == null) skip = 0;
            if (limit == null) limit = 1000;

            string[] deviceIds = new string[0];
            if (!String.IsNullOrEmpty(devices))
            {
                deviceIds = devices.Split(',');
            }

            if (deviceIds.Length > LIMIT)
            {
                log.Warn("The client requested too many devices", () => new { devices.Length });
                throw new BadRequestException("The number of devices cannot exceed 200");
            }

            List<Alarm> alarmsList = this.alarmService.List(
                fromDate,
                toDate,
                order,
                skip.Value,
                limit.Value,
                deviceIds);

            return new AlarmListApiModel(alarmsList);
        }

        [HttpGet("{id}")]
        public AlarmApiModel Get([FromRoute] string id)
        {
            Alarm alarm = this.alarmService.Get(id);
            return new AlarmApiModel(alarm);
        }

        [HttpPatch("{id}")]
        public async Task<AlarmApiModel> PatchAsync(
            [FromRoute] string id,
            [FromBody] AlarmStatusApiModel body)
        {
            Alarm alarm = await this.alarmService.UpdateAsync(id, body.Status);
            return new AlarmApiModel(alarm);
        }

    }
}
