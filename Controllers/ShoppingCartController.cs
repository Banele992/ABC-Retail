
using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABCRetail.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly OrderService _tableStorageService;
        private readonly ProductService _productTableStorageService;
        private readonly BlobStorageService _blobStorageService;
        private readonly QueueStorageService _queueStorageService;

        public ShoppingCartController(OrderService tableStorageService, ProductService productTableStorageService, BlobStorageService blobStorageService, QueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _productTableStorageService = productTableStorageService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddToCart(string partitionKey, string rowKey)
        {
            var productToAdd = await _productTableStorageService.GetProductAsync(partitionKey, rowKey);

            if (productToAdd == null)
            {
                return NotFound();
            }

            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            var existingCartItem = cartItems.FirstOrDefault(item => item.Product.PartitionKey == partitionKey && item.Product.RowKey == rowKey);

            if (existingCartItem != null)
            {
                existingCartItem.Total++;
            }
            else
            {
                cartItems.Add(new ShoppingCartItem
                {
                    Product = productToAdd,
                    Total = 1
                });
            }

            HttpContext.Session.Set("Cart", cartItems);

            TempData["CartMessage"] = $"{productToAdd.ProductName} {productToAdd.ProductCategory} added to cart";

            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart()
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            var cartViewModel = new ShoppingCartView
            {
                CartItems = cartItems,
                TotalPrice = cartItems.Sum(item => item.Product.ProductPrice * item.Total),
            };

            ViewBag.CartMessage = TempData["CartMessage"];

            return View(cartViewModel);
        }

        public IActionResult RemoveItem(string partitionKey, string rowKey)
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();
            var itemToRemove = cartItems.FirstOrDefault(item => item.Product.PartitionKey == partitionKey && item.Product.RowKey == rowKey);

            if (itemToRemove != null)
            {
                if (itemToRemove.Total > 1)
                {
                    itemToRemove.Total--;
                }
                else
                {
                    cartItems.Remove(itemToRemove);
                }
            }

            HttpContext.Session.Set("Cart", cartItems);

            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> OrderItems()
        {
            var cartItems = HttpContext.Session.Get<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            foreach (var item in cartItems)
            {
                // Save the order details in Table Storage
                var order = new Order
                {
                    PartitionKey = "Order",
                    RowKey = Guid.NewGuid().ToString(),
                    Product = item.Product.RowKey,
                    Quantity = item.Total,
                    OrderDate = DateTime.Now,
                    //Total = item.Product.ProductPrice * item.Total,
                };
                await _tableStorageService.AddOrderAsync(order);

                // Create a message for transaction processing
                var transactionMessage = $"Ordered {item.Total} of {item.Product.ProductName} (ID: {item.Product.RowKey}) on {DateTime.Now}";
                await _queueStorageService.SendMessagesAsync(transactionMessage);

                // Optionally, create a message for inventory processing
                var inventoryMessage = $"Update Inventory for Product ID: {item.Product.RowKey}, Quantity: -{item.Total}";
                await _queueStorageService.SendMessagesAsync(inventoryMessage);
               
            }

            // Clear the cart
            HttpContext.Session.Set("Cart", new List<ShoppingCartItem>());

            return RedirectToAction("Index", "Home");
        }
    }
}
