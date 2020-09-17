using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecureApiClient
{
    class Program
    {
         static void Main(string[] args)
        {
            var config = AuthConfig.ReadJsonFromFile("appSettings.json");
            Console.WriteLine($"Authority: {config.Authority}");
            RunAsync().GetAwaiter().GetResult();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
        private static async Task RunAsync()
        {
            var config = AuthConfig.ReadJsonFromFile("appSettings.json");

            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();
            var resourceIds = new string[] { config.ResourceId };
            AuthenticationResult result = null;

            try
            {
                result = await app.AcquireTokenForClient(resourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token Aquired /n");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
            }
            catch (MsalClientException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                throw;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config.BaseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
                var response= await client.GetAsync("/weatherforecast");
                string strReply = await response.Content.ReadAsStringAsync();
                Console.WriteLine(strReply);
            }
        
        }

    }
}
