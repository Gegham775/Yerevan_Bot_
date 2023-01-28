using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Yerevan_Housing_API.Services;

namespace Yerevan_Housing_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        [HttpGet("GetHouse")]
        public async Task<IActionResult> GetHouse()
        {
            try
            {
                var house = await GoogleCloudSheetService.ReadEntries();
                if (house != null)
                {
                    TelegramService bot = new TelegramService();
                    await bot.SendCaption(house);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Ok();
        }
    }
}
