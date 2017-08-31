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

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public class AlarmsByRuleController : Controller
    {
        private readonly IAlarms alarmService;
        private readonly ILogger log;

        private const int DEVICE_LIMIT = 200;

        public AlarmsByRuleController(
            IAlarms alarmService,
            ILogger logger)
        {
            this.alarmService = alarmService;
            this.log = logger;
        }

        [HttpGet]
        public AlarmByRuleListApiModel List(
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

            /* TODO: move this logic to the storage engine, depending on the
             * storage type the limit will be different. 200 is DocumentDb
             * limit for the IN clause.
             */
            string[] deviceIds = new string[0];
            if (!String.IsNullOrEmpty(devices))
            {
                deviceIds = devices.Split(',');
            }

            if (deviceIds.Length > DEVICE_LIMIT)
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

            return new AlarmByRuleListApiModel(alarmsList);
        }

        [HttpGet("{id}")]
        public AlarmListByRuleApiModel Get(
            [FromRoute] string id,
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

            /* TODO: move this logic to the storage engine, depending on the
             * storage type the limit will be different. 200 is DocumentDb
             * limit for the IN clause.
             */
            string[] deviceIds = new string[0];
            if (!String.IsNullOrEmpty(devices))
            {
                deviceIds = devices.Split(',');
            }

            if (deviceIds.Length > DEVICE_LIMIT)
            {
                log.Warn("The client requested too many devices", () => new { devices.Length });
                throw new BadRequestException("The number of devices cannot exceed 200");
            }

            List<Alarm> alarmsList = this.alarmService.ListByRule(
                id,
                fromDate,
                toDate,
                order,
                skip.Value,
                limit.Value,
                deviceIds);

            return new AlarmListByRuleApiModel(alarmsList);
        }
    }
}
