using Arragro.ObjectHistory.WebExample.Core.Interfaces;
using Arragro.ObjectHistory.WebExample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.WebExample.Controllers
{
    public class SessionController : Controller
    {
        private readonly ITrainingSessionRepository _sessionRepository;

        public SessionController(ITrainingSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(actionName: nameof(Index), controllerName: "Home");
            }

            var session = await _sessionRepository.GetByIdAsync(id.Value);
            if (session == null)
            {
                return Content("Session not found.");
            }

            var viewModel = new TrainingSessionViewModel()
            {
                DateCreated = session.DateCreated,
                Name = session.Name,
                Id = session.Id,
                DrillCount = session.Drills.Count()
            };

            return View(viewModel);
        }

        public ActionResult SpaIndex()
        {
            return View();
        }
    }
}
