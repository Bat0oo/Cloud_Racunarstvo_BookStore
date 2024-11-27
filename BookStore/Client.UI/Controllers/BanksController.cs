using Client.UI.Models;
using Communication.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BanksController : Controller
    {
        [HttpGet]
        [Route("ListClients")]
        public async Task<IActionResult> ListClients()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            List<string> result = await validationProxy.ListClients();

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get clients." });
            }

            var clients = new List<ClientViewModel>();

            result.ForEach(x => clients.Add(JsonConvert.DeserializeObject<ClientViewModel>(x)!));

            return View(clients);
        }

        [HttpGet]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer(long userSendId)
        {
            return View(new EnlistMoneyTransferViewModel() { UserSendId = userSendId });
        }

        [HttpPost]
        [Route("EnlistMoneyTransfer")]
        public async Task<IActionResult> EnlistMoneyTransfer([FromForm] EnlistMoneyTransferViewModel model)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            string result = await validationProxy.EnlistMoneyTransfer(model.UserSendId, model.UserReceiveId, model.Amount);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot execute money transfer." });
            }

            return RedirectToAction("ListClients", "Banks");
        }
    }
}
