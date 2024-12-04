using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Pawel.LoadPanels.Contracts.ViewDtos;
using BowlingSys.Entities.UserDBEntities;
using System.Threading.Tasks;

namespace BowlingSys.Process.Controllers
{
    [ApiController]
    [Route("Api/Panels")]
    public class BowlingSysController : BaseController
    {
        public BowlingSysController(IMessageSession messageSession) : base(messageSession)
        {
        }

        [HttpGet("LoadBowlingPanels")]
        public async Task<IActionResult> CheckLogin(string? username)
        {

            try
            {
                ViewDto viewDto = new ViewDto
                {
                    username = username
                };

                var response = await _messageSession.Request<GetViews>(viewDto);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while processing the request: {ex.Message}");
            }
        }

    }
}
