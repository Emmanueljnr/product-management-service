using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProductManagementAPI.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


// Make internals visible to the test project (replace "ProdManagementAPI.Tests" with the actual name of your test project)
#if DEBUG
[assembly: InternalsVisibleTo("ProdManagementAPI.Tests")]
#endif

namespace ProductManagementAPI.Services
{
    public class DailyUpdateService : BackgroundService
    {
        //private readonly ExternalProductService _externalProductService;
        private readonly IExternalProductService _externalProductService; //now using the interface instead of the concrete class
        private readonly IFileService _fileService;

        public DailyUpdateService(IExternalProductService externalProductService, IFileService fileService)
        {
            _externalProductService = externalProductService ?? throw new ArgumentNullException(nameof(externalProductService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        //Makes an API Call at 3AM everynight 
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.UtcNow;
                // Schedule task to run at 3 AM GMT (convert this to your local time if needed)
                var targetTime = DateTime.Today.AddHours(3);
                if (currentTime > targetTime)
                {
                    targetTime = targetTime.AddDays(1);
                }

                var delay = targetTime - currentTime;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                // Your logic to fetch data and update the seedData.json file goes here
                await FetchAndUpdateData();

                // Wait for 24 hours to run the task again
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }



        //For Testing : Runs 1 minute after the App starts 
        /*
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Delay the first run for 1 minute after the application starts
            var firstRunDelay = TimeSpan.FromMinutes(1);
            await Task.Delay(firstRunDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Your logic to fetch data and update the seedData.json file goes here
                await FetchAndUpdateData();

                // Wait for 24 hours to run the task again
                // For testing purposes, you can change this to a smaller interval
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        */

        // Changed to internal for testing purposes
        internal async Task FetchAndUpdateData()
        {
            var headphones = await _externalProductService.FetchProductsAsync("Active Noise Cancelling Wireless Headphones, Over- Ear Bluetooth Headphones, milky white, gold,");
            var earphones = await _externalProductService.FetchProductsAsync("Wireless Earbuds, In Ear with Noise Cancelling Mic, 2023 Bluetooth Earphones Mini HI-FI Stereo Sound, LED Display gold green pink");

            var products = headphones.Concat(earphones).ToList();
            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "seedData.json");
            await _fileService.WriteAllTextAsync(jsonFilePath, json); // Use IFileService to write to file
        }
    }

}
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<Original Code (Difficult to write Unit Tests for )>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProductManagementAPI.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DailyUpdateService : BackgroundService
{
    private readonly ExternalProductService _externalProductService;

    public DailyUpdateService(ExternalProductService externalProductService)
    {
        _externalProductService = externalProductService;
    }

    //Runs | Makes an API Call at 3AM everynight 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentTime = DateTime.UtcNow;
            // Schedule task to run at 3 AM GMT (convert this to your local time if needed)
            var targetTime = DateTime.Today.AddHours(3);
            if (currentTime > targetTime)
            {
                targetTime = targetTime.AddDays(1);
            }

            var delay = targetTime - currentTime;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
            }

            // Your logic to fetch data and update the seedData.json file goes here
            await FetchAndUpdateData();

            // Wait for 24 hours to run the task again
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }



    //For Testing : Runs 1 minute after the App starts 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Delay the first run for 1 minute after the application starts
        var firstRunDelay = TimeSpan.FromMinutes(1);
        await Task.Delay(firstRunDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Your logic to fetch data and update the seedData.json file goes here
            await FetchAndUpdateData();

            // Wait for 24 hours to run the task again
            // For testing purposes, you can change this to a smaller interval
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task FetchAndUpdateData()
    {
        var headphones = await _externalProductService.FetchProductsAsync("Active Noise Cancelling Wireless Headphones, Over- Ear Bluetooth Headphones, milky white, gold,");
        var earphones = await _externalProductService.FetchProductsAsync("Wireless Earbuds, In Ear with Noise Cancelling Mic, 2023 Bluetooth Earphones Mini HI-FI Stereo Sound, LED Display gold green pink");

        var products = headphones.Concat(earphones).ToList();
        var json = JsonConvert.SerializeObject(products, Formatting.Indented);

        var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "seedData.json");
        await File.WriteAllTextAsync(jsonFilePath, json);
    }
}
*/







