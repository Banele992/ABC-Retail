/**
 * This method was adapted from The IIE 2024
 * Authour: Isaac Leshaba
 * Link: https://myvc.iielearn.ac.za/ultra/courses/_223566_1/cl/outline 
 */

using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;
using System.Text;

namespace ABCRetail.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareStorageService;

        public ProductController(ProductService tableStorageService, BlobStorageService blobStorageService, QueueStorageService queueStorageService, FileShareStorageService fileShareStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
            _fileShareStorageService = fileShareStorageService;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync(); 
            return View(products);
        }

        //GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                // Upload photo to blob and return the SAS URI
                if (photo != null)
                {
                    using var stream = photo.OpenReadStream();
                    product.ProductImage = await _blobStorageService.UploadPhotoAsync(Guid.NewGuid().ToString(), stream);
                }

                // Add product to table storage
                await _tableStorageService.AddProductAsync(product);

                // Send Message to the queue
                var message = new
                {
                    Action = "New product created",
                    Timestamp = DateTime.UtcNow,
                    Details = new
                    {
                        product.PartitionKey,
                        product.RowKey,
                        product.ProductName,
                        product.ProductPrice,
                        product.ProductCategory,
                        
                        product.ProductDescription                         
                    }
                };
                await _queueStorageService.SendMessagesAsync(message);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        //GET: Product/Details/{partitionKey} + {rowKey}
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //GET: Product/Edit/{partitionKey} + {rowKey}
        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            return View(product);
        }

        //POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile photo)
        {
            // Remove the photo error from ModelState if no new photo was uploaded
            if (photo == null || photo.Length == 0)
            {
                ModelState.Remove("photo");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for null partitionKey and rowKey
                    if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid product data.");
                        return View(product);
                    }

                    // Retrieve the existing product from Table Storage
                    var existingProduct = await _tableStorageService.GetProductAsync(product.PartitionKey, product.RowKey);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Check if a new photo was uploaded
                    if (photo != null && photo.Length > 0)
                    {
                        // Delete the old photo if it exists
                        if (!string.IsNullOrEmpty(existingProduct.ProductImage))
                        {
                            var blobName = Path.GetFileName(new Uri(existingProduct.ProductImage).AbsolutePath);
                            await _blobStorageService.DeletePhotoAsync(blobName);
                        }

                        // Upload the new photo
                        using var stream = photo.OpenReadStream();
                        product.ProductImage = await _blobStorageService.UploadPhotoAsync(Guid.NewGuid().ToString(), stream);
                    }
                    else
                    {
                        // Preserve the existing ProductImage
                        product.ProductImage = existingProduct.ProductImage;
                    }

                    // Update the product in Table Storage
                    await _tableStorageService.UpdateProductAsync(product);

                    // Send message to the queue
                    var message = new
                    {
                        Action = "Product updated",
                        Timestamp = DateTime.UtcNow,
                        Details = product
                    };
                    await _queueStorageService.SendMessagesAsync(message);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError(string.Empty, $"An error occurred while updating the product. {ex.Message}");
                }
            }

            // If we got this far, something failed; redisplay form
            return View(product);
        }

        //GET: Product/Delete/{partitionKey}/{rowKey}
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST: Product/Delete/{partitionKey}/{rowKey}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);

            if (product != null && !string.IsNullOrEmpty(product.ProductImage))
            {
                var blobName = Path.GetFileName(new Uri(product.ProductImage).AbsolutePath);
                await _blobStorageService.DeletePhotoAsync(blobName);
            }

            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);

            // Send message to queue
            var message = new
            {
                Action = "Product deleted",
                Timestamp = DateTime.UtcNow,
                Details = new
                {
                    ProductId = rowKey
                }
            };
            await _queueStorageService.SendMessagesAsync(message);

            return RedirectToAction(nameof(Index));
        }

        //GET: ProductLogs/Log
        [HttpGet]
        public async Task<IActionResult> Log()
        {
            var logMessages = await _queueStorageService.GetMessagesAsync();
            return View(logMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportLog()
        {
            var logMessages = await _queueStorageService.GetMessagesAsync();

            var filename = $"Log_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                // Write the header
                await writer.WriteLineAsync("MessageId,InsertionTime,MessageText");

                // Write each log message
                foreach (var log in logMessages)
                {
                    // Escape any double quotes in the message text
                    var messageText = log.MessageText?.Replace("\"", "\"\"");
                    // Ensure fields are enclosed in double quotes
                    await writer.WriteLineAsync($"\"{log.MessageId}\",\"{log.InsertionTime?.ToString("yyyy/MM/dd HH:mm:ss")}\",\"{messageText}\"");
                }
                await writer.FlushAsync();

                // Reset the stream position to the beginning before uploading
                stream.Position = 0;
                await _fileShareStorageService.UploadFileAsync(filename, stream);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
