using Client.UI.Models;
using Communication.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookstoresController : Controller
    {
        [HttpGet]
        [Route("ListAvailableItems")]
        public async Task<IActionResult> ListAvailableItems()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            List<string> result = await validationProxy.ListAvailableItems();

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get list of books." });
            }

            var books = new List<BookViewModel>();

            result.ForEach(x => books.Add(JsonConvert.DeserializeObject<BookViewModel>(x)!));

            return View(books);
        }

        [HttpGet]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase(long bookId, uint count)
        {
            return View(new EnlistPurchaseViewModel() { BookId = bookId, Count = count });
        }

        [HttpPost]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase([FromForm] EnlistPurchaseViewModel model)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            string result = await validationProxy.EnlistPurchase(model.BookId, model.Count);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot purchase." });
            }

            return RedirectToAction("ListAvailableItems", "Bookstores");
        }

        [HttpGet]
        [Route("GetItemPrice/{id}")]
        public async Task<IActionResult> GetItemPrice(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            var result = await validationProxy.GetItemPrice(id);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get book's price." });
            }

            double price = JsonConvert.DeserializeObject<double>(result);

            return View(price);
        }

        [HttpGet]
        [Route("GetItem/{id}")]
        public async Task<IActionResult> GetItem(long id)
        {
            var validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            string result = await validationProxy.GetItem(id);

            if (result is null)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = "Cannot get book." });
            }

            BookViewModel book = JsonConvert.DeserializeObject<BookViewModel>(result)!;

            return View(book);
        }
    }
}
