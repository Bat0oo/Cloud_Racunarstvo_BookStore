using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Text.Json;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookStoresController : ControllerBase
    {
        [HttpGet]
        [Route("ListAvailableItems")]
        public async Task<IActionResult> ListAvailableItems()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));

            var books = new List<Book>();

            List<string> booksJson = await validationProxy.ListAvailableItems();

            booksJson.ForEach(x => books.Add(JsonSerializer.Deserialize<Book>(x)!));

            return Ok(books);
        }

        [HttpPost]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase(long bookId, uint count)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));

            return Ok(await validationProxy.EnlistPurchase(bookId, count));
        }

        [HttpGet]
        [Route("GetItemPrice/{id}")]
        public async Task<IActionResult> GetItemPrice(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));

            return Ok(await validationProxy.GetItemPrice(id));
        }

        [HttpGet]
        [Route("GetItem/{id}")]
        public async Task<IActionResult> GetItem(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookStore/ValidationService"));

            string bookJson = await validationProxy.GetItem(id);

            Book book = JsonSerializer.Deserialize<Book>(bookJson)!;

            return Ok(book);
        }
    }
}
