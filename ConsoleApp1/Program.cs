using System.Net;

namespace ConsoleApp1;

public class Program
{
    static async Task Main(string[] args)
    {
        // Increase the number of allowed connections
        ServicePointManager.DefaultConnectionLimit = 100;

        // Base URL of your API
        const string apiUrl = "https://localhost:5001/api/video/get/7b6f653b-b326-46d9-be59-8cad8aee15a4";



        // Ask for the number of API calls to make
        Console.WriteLine("Enter the number of API calls to make:");
        if (!int.TryParse(Console.ReadLine(), out var numberOfCalls))
        {
            Console.WriteLine("Invalid number. Exiting...");
            return;
        }

        // Create an HttpClientHandler to bypass SSL certificate validation (for dev environments)
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };

        // Create an instance of HttpClient using the handler
        using (var httpClient = new HttpClient(handler))
        {
            // Loop to make the API calls
            var tasks = new Task[numberOfCalls];
            for (var i = 0; i < numberOfCalls; i++)
            {
                tasks[i] = MakeApiCall(httpClient, apiUrl, i + 1);
            }

            // Wait for all calls to complete
            await Task.WhenAll(tasks);
        }

        Console.ReadLine();
    }

    static async Task MakeApiCall(HttpClient httpClient, string apiUrl, int callNumber)
    {
        try
        {
            Console.WriteLine($"Making API call {callNumber}...");
            Thread.Sleep(500);
            var response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Call {callNumber}: Success, got response:\n{content}\n");
            }
            else
            {
                Console.WriteLine($"Call {callNumber}: Failed with status code {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Call {callNumber}: Exception occurred - {ex.Message}");
        }
    }
}