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
        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7152/api/Admin/GetAllActiveOrders";
            HttpResponseMessage response = client.GetAsync(URL).Result;

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

            HttpContent content = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;
            return View(data);
        }

        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order ID";
                worksheet.Cell(1, 2).Value = "Customer Email";
                worksheet.Cell(1, 3).Value = "Delivery Type";
                worksheet.Cell(1, 4).Value = "Address";
                worksheet.Cell(1, 5).Value = "City";
                worksheet.Cell(1, 6).Value = "Phone";

                HttpClient client = new HttpClient();
                string URL = "https://localhost:7152/api/Admin/GetAllActiveOrders";
                HttpResponseMessage response = client.GetAsync(URL).Result;

                var data = response.Content.ReadAsAsync<List<Order>>().Result;

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];

                    worksheet.Cell(i + 2, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 2, 2).Value = item.User.Email;
                    worksheet.Cell(i + 2, 3).Value = item.DeliveryType;
                    worksheet.Cell(i + 2, 4).Value = item.DeliveryAddress;
                    worksheet.Cell(i + 2, 5).Value = item.DeliveryCity;
                    worksheet.Cell(i + 2, 6).Value = item.DeliveryPhone;

                    for (int p = 0; p < item.ProductInOrders.Count(); p++)
                    {
                        worksheet.Cell(1, p + 7).Value = "Product-" + (p + 1);
                        worksheet.Cell(i + 2, p + 7).Value = item.ProductInOrders.ElementAt(p).OrderedProduct.ProductName;
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

            HttpContent content = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", data.Id.ToString());
            document.Content.Replace("{{UserName}}", data.User.UserName);
            document.Content.Replace("{{DeliveryType}}", data.DeliveryType ?? "");
            document.Content.Replace("{{DeliveryAddress}}", data.DeliveryAddress ?? "");
            document.Content.Replace("{{DeliveryCity}}", data.DeliveryCity ?? "");
            document.Content.Replace("{{DeliveryPhone}}", data.DeliveryPhone ?? "");

            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;

            foreach (var item in data.ProductInOrders)
            {
                totalPrice += item.Quantity * item.OrderedProduct.Price;
                sb.AppendLine(item.OrderedProduct.ProductName + " with quantity of: " + item.Quantity + " and price of: " + item.OrderedProduct.Price + " мкд");
            }

            document.Content.Replace("{{ProductList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + " мкд");

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}