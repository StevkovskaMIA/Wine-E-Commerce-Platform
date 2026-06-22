using ClosedXML.Excel;
using EShopAdminApplication.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace EShopAdminApplication.Controllers
{
    public class OrderController : Controller
    {
        public OrderController() {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7152/api/Admin/GetAllActiveOrders";
            HttpResponseMessage  response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;
            return View(data);
        }
        public IActionResult Details(Guid id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7152/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };


            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;
            return View(data);
        }


        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            
            using(var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet= workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order ID"; 
                worksheet.Cell(1, 2).Value = "Customer Email";

                HttpClient client = new HttpClient();
                string URL = "https://localhost:7152/api/Admin/GetAllActiveOrders";
                HttpResponseMessage response = client.GetAsync(URL).Result;

                var data = response.Content.ReadAsAsync<List<Order>>().Result;

                for (int i = 1; i <= data.Count(); i++)
                {
                    var item = data[i-1];
                    worksheet.Cell(i+1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i+1, 2).Value = item.User.Email;

                    for(int p = 0; p < item.ProductInOrders.Count(); p++)
                    {
                        worksheet.Cell(1, p+3).Value = "Product-" + (p+1);
                        worksheet.Cell(i + 1, p + 3).Value = item.ProductInOrders.ElementAt(p).OrderedProduct.ProductName;

                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);

                }
            }
            
        }
        public FileContentResult CreateInvoice(Guid id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7152/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };


            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);


            document.Content.Replace("{{OrderNumber}}", data.Id.ToString());
            document.Content.Replace("{{UserName}}", data.User.UserName);


            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;
            foreach(var item in data.ProductInOrders)
            {
                totalPrice += item.Quantity * item.OrderedProduct.Price;
                sb.AppendLine(item.OrderedProduct.ProductName + " with quantity of: " + item.Quantity + " and price of: " + item.OrderedProduct.Price + "мкд");
            }
            document.Content.Replace("{{ProductList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "мкд");


            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(),new PdfSaveOptions().ContentType, "ExportInovice.pdf");
        }
    }
}
