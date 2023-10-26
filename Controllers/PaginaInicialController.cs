using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;
using System;

namespace BancoVirtual.Controllers
{
    public class PaginaInicialController : Controller
    {
        private readonly IConfiguration _configuration;

        public PaginaInicialController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                string customerName = GetCustomerName(userId.Value);
                ViewBag.CustomerName = customerName;
            }

            return View("~/Views/Inicial/paginainicial.cshtml");
        }

        public string GetCustomerName(int userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                // Consulta para obter o nome do cliente com base no UserId
                string query = "SELECT NomeCompleto FROM Users WHERE UserId = @UserId";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);

                var customerName = cmd.ExecuteScalar() as string;

                return customerName;
            }
            catch (Exception ex)
            {
                // Lide com erros de forma apropriada, como exibir uma mensagem de erro
                Console.WriteLine("Ocorreu um erro ao obter o nome do cliente: " + ex.Message);
                return "Nome do Cliente Desconhecido";
            }
        }
    }
}
