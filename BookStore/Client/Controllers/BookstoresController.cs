using Communication.Models;
using Communication.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookstoresController : ControllerBase
    {
        [HttpGet]
        [Route("ListAvailableItems")]
        public async Task<IActionResult> ListAvailableItems()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            var books = new List<Book>();

            List<string> booksJson = await validationProxy.ListAvailableItems();

            booksJson.ForEach(x => books.Add(JsonConvert.DeserializeObject<Book>(x)!));

            return Ok(books);
        }

        [HttpPost]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase(long bookId, uint count)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            return Ok(await validationProxy.EnlistPurchase(bookId, count));
        }

        [HttpGet]
        [Route("GetItemPrice/{id}")]
        public async Task<IActionResult> GetItemPrice(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            return Ok(await validationProxy.GetItemPrice(id));
        }

        [HttpGet]
        [Route("GetItem/{id}")]
        public async Task<IActionResult> GetItem(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            string bookJson = await validationProxy.GetItem(id);

            Book book = JsonConvert.DeserializeObject<Book>(bookJson)!;

            return Ok(book);
        }
    }
}
