using ABCRetail.Services;

namespace ABCRetail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Retrieve the connection string from the appsettings.json file
            var storageConnectionString = builder.Configuration.GetConnectionString("StorageConnectionString")
                ?? throw new InvalidOperationException("Storage connection string is missing");

            // Register the services with valid names
            builder.Services.AddSingleton(new CustomerService(storageConnectionString, "Customer"));
            builder.Services.AddSingleton(new ProductService(storageConnectionString, "Product"));
            builder.Services.AddSingleton(new OrderService(storageConnectionString, "Order"));
            builder.Services.AddSingleton(new BlobStorageService(storageConnectionString, "customerprofilepictures"));
            builder.Services.AddSingleton(new BlobStorageService(storageConnectionString, "productimages"));
            builder.Services.AddSingleton(new QueueStorageService(storageConnectionString, "customerlogmessages"));
            builder.Services.AddSingleton(new QueueStorageService(storageConnectionString, "orderlogmessages"));
            builder.Services.AddSingleton(new QueueStorageService(storageConnectionString, "productlogmessages"));
            builder.Services.AddSingleton(new FileShareStorageService(storageConnectionString, "customerlogfiles"));
            builder.Services.AddSingleton(new FileShareStorageService(storageConnectionString, "orderlogfiles"));
            builder.Services.AddSingleton(new FileShareStorageService(storageConnectionString, "productlogfiles")); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
