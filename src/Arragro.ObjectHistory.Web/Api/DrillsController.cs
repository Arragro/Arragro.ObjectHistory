﻿using Arragro.ObjectHistory.Web.ClientModels;
using Arragro.ObjectHistory.Web.Core.Entities;
using Arragro.ObjectHistory.Web.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Api
{
    [Route("api/drills")]
    public class DrillsController : Controller
    {
        private readonly ITrainingSessionRepository _sessionRepository;

        public DrillsController(ITrainingSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [HttpGet("forsession/{sessionId}")]
        public async Task<IActionResult> ForSession(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound(sessionId);
            }

            var result = session.Drills.Select(drill => new DrillDTO()
            {
                Id = drill.Id,
                Name = drill.Name,
                Description = drill.Description,
                Duration = drill.Duration
            }).ToList();

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]NewDrillModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _sessionRepository.GetByIdAsync(model.SessionId);

            if (session == null)
            {
                return NotFound(model.SessionId);
            }

            var drill = new Drill()
            {
                Duration = model.Duration,
                Description = model.Description,
                Name = model.Name
            };
            session.AddDrill(drill);

            var unmodifiedSession = await _sessionRepository.GetByIdNoTrackingAsync(model.SessionId);

            await _sessionRepository.UpdateAsync(session, unmodifiedSession);

            return Ok(session);
        }
    }
}
