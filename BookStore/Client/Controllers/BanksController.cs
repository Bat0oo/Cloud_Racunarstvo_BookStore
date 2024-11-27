using Communication.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BanksController : ControllerBase
    {
        [HttpGet]
        [Route("ListClients")]
        public async Task<IActionResult> ListClients()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            var clients = new List<Communication.Models.Client>();

            List<string> clientsJson = await validationProxy.ListClients();

            clientsJson.ForEach(x => clients.Add(JsonConvert.DeserializeObject<Communication.Models.Client>(x)!));

            return Ok(clients);
        }

        [HttpPost]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer(long userSend, long? userReceive, double amount)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            return Ok(await validationProxy.EnlistMoneyTransfer(userSend, userReceive, amount));
        }
    }
}
