using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Text.Json;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        [HttpGet]
        [Route("ListClients")]
        public async Task<IActionResult> ListClients()
        {

            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));
            if (validationProxy == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create ServiceProxy.");
            }

            var clients = new List<Common.Models.Client>();

            List<string> clientsJson = await validationProxy.ListClients();

            clientsJson.ForEach(x => clients.Add(JsonSerializer.Deserialize<Common.Models.Client>(x)!));

            return Ok(clients);
        }

        [HttpPost]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer(long userSend, long? userReceive, double amount)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));

            return Ok(await validationProxy.EnlistMoneyTransfer(userSend, userReceive, amount));
        }
    }
}
