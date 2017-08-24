// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers.Helpers;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Exceptions;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models;
using System;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class MessagesController : Controller
    {
        private readonly IMessages messageService;
        private readonly ILogger log;

        public MessagesController(
            IMessages messageService,
            ILogger logger)
        {
            this.messageService = messageService;
            this.log = logger;
        }

        [HttpGet]
        public MessageListApiModel Get(
            string from,
            string to,
            string order,
            int? skip,
            int? limit,
            string devices)
        {
            DateTimeOffset? fromDate = DateHelper.parseDate(from);
            DateTimeOffset? toDate = DateHelper.parseDate(to);

            if (order == null) order = "asc";
            if (skip == null) skip = 0;
            if (limit == null) limit = 1000;

            // TODO: move this logic to the storage engine, depending on the
            // storage type the limit will be different. 200 is DocumentDb
            // limit for the IN clause.
            string[] deviceIds = new string[0];
            if (devices != null)
            {
                deviceIds = devices.Split(',');
            }
            if (deviceIds.Length > 200)
            {
                log.Warn("The client requested too many devices: {}", () => new { deviceIds.Length });
                throw new BadRequestException("The number of devices cannot exceed 200");
            }

            return new MessageListApiModel(
                this.messageService.GetList(
                    fromDate,
                    toDate,
                    order,
                    skip.Value,
                    limit.Value,
                    deviceIds));
        }
    }
}
