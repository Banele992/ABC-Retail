# 🛍️☁️ ABC Retail Cloud Transformation

**ABC Retail** is a modernized, cloud-driven e-commerce platform designed to overcome the limitations of legacy infrastructure and deliver seamless order processing, efficient data handling, and real-time customer experiences — especially during peak seasons like Christmas and other holidays.

This project is built using **ASP.NET Core MVC** and **Microsoft Azure Services** to enhance scalability, performance, and reliability across the system.

---

## 🚛 Project Purpose

ABC Retail faced major challenges in:

* ⚙️ Handling high-volume transactions during peak shopping periods
* 🖼️ Managing product images via slow network drives
* 📦 Processing customer orders in real-time
* 📩 Delivering reliable messaging for inventory updates and order events
* 📊 Analyzing customer data for personalized marketing and sales optimization

**This solution re-architects ABC Retail’s operations on Azure**, enabling scalable, responsive, and intelligent digital retail services.

---

## 🧰 Key Features

* 🛒 Cloud-based customer orders and product catalog storage
* 🗃️ Product images stored and served via Azure Blob Storage
* 📬 Azure Queue Storage for decoupled, scalable order messaging
* ⚡ Azure Functions for real-time event processing
* 🔎 Azure Table Storage for quick access to structured data (e.g., profiles, inventory)
* 📊 Scalable analytics-ready architecture to support future Power BI or Synapse integration

---

## ☁️ Azure Services Used

* **Azure Blob Storage** → For fast, scalable image/file storage
* **Azure Table Storage** → For structured, NoSQL data like product details, customers
* **Azure Queue Storage** → For reliable, asynchronous messaging (order workflows)
* **Azure Functions** → Event-driven microservices for handling background tasks
* **Azure Files** → Centralized file access for reports, contracts, and logs
* **App Service** → To host the ASP.NET Core MVC web application
* **Key Vault** → For storing sensitive credentials securely

---

## 💻 Tech Stack

* **Frontend:** ASP.NET Core MVC + Razor Pages + Bootstrap 5
* **Backend:** C# .NET 7, Azure SDKs
* **Data:** Azure Storage (Tables, Blobs, Queues, Files)
* **Authentication:** ASP.NET Core Identity (optional if added)
* **Deployment:** Azure App Service

---

## 🖼️ Sample Screens

*📌 (Insert screenshots here: Home page, Order form, Product listing with images, Admin dashboard, etc.)*

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/abc-retail-cloud.git
cd abc-retail-cloud
```

### 2. Open the Project

Open the solution file `ABCRetailCloud.sln` in **Visual Studio 2022**.

### 3. Configure Azure Settings

Update `appsettings.json` with your Azure Storage connection strings and queue names:

```json
"AzureStorage": {
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=your_account;AccountKey=your_key;EndpointSuffix=core.windows.net",
  "QueueName": "orderqueue",
  "TableName": "ProductTable",
  "BlobContainerName": "product-images"
}
```

### 4. Run the App Locally

Click **Start** in Visual Studio or run:

```bash
dotnet run
```

---

## 📁 Project Structure Overview

```
ABCRetailCloud/
├── Controllers/            → MVC Controllers for Orders, Products, Uploads
├── Models/                 → Entity and View Models (Product, Order, Customer)
├── Services/               → Azure service wrappers (BlobService, QueueService, etc.)
├── Views/                  → Razor Views
├── Data/                   → Azure Table initializers and context helpers
├── wwwroot/                → Static content (CSS, JS, images)
├── appsettings.json        → Azure configuration settings
└── Program.cs / Startup.cs → Middleware and service configuration
```

---

## 📦 NuGet Packages

* `Azure.Storage.Blobs`
* `Azure.Data.Tables`
* `Azure.Storage.Queues`
* `Microsoft.Extensions.Azure`
* `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`
* `Microsoft.EntityFrameworkCore` (optional, if hybrid database used)

---

## 📊 Future Improvements

* 📈 Integrate Azure Synapse or Power BI for deeper analytics
* 🔐 Add Azure AD B2C for secure customer authentication
* 🛒 Implement Cosmos DB for global scalability and real-time product inventory
* 📱 Develop mobile app with Xamarin or MAUI for customer orders

---

## 🧑‍💼 For Developers and Students

This project demonstrates how to:

* 🔄 Migrate legacy systems to modern cloud architecture
* 🧠 Build decoupled, scalable systems using Azure messaging patterns
* 🎯 Store and retrieve structured/unstructured data efficiently
* 💻 Combine ASP.NET Core MVC with Azure SDK for enterprise-ready solutions

