using Arragro.ObjectHistory.Web.ClientModels;
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

            var drills = session.Drills.Select(drill => new DrillDto
            {
                Id = drill.Id,
                Name = drill.Name,
                Description = drill.Description,
                Duration = drill.Duration,
                DateCreated = drill.DateCreated
            }).ToList();

            return Ok(new SessionDrillContainer(session, drills));
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
                Name = model.Name,
                DateCreated = model.DateCreated
            };
            session.AddDrill(drill);

            var unmodifiedSession = await _sessionRepository.GetByIdNoTrackingAsync(model.SessionId);

            await _sessionRepository.UpdateAsync(session, unmodifiedSession);

            var drills = session.Drills.Select(x => new DrillDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Duration = x.Duration,
                DateCreated = x.DateCreated
            }).ToList();

            return Ok(new SessionDrillContainer(session, drills));
        }
    }
}
