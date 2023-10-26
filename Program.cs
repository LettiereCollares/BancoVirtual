using System;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BancoVirtual
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") 
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso!");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao conectar ao banco de dados: " + ex.Message);
                return; // Encerra o programa em caso de falha na conexão.
            }
            finally
            {
                connection.Close();
            }

            // Agora você pode iniciar seu aplicativo ASP.NET Core
            var host = CreateHostBuilder(args).Build();

            // Abra o navegador padrão com o endereço da página HTML
            var url = "http://localhost:51611"; // Substitua "porta" pela porta do seu servidor
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
