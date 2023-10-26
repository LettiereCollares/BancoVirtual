using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;

namespace BancoVirtual.Controllers
{
    public class ExtratoController : Controller
    {
        private readonly IConfiguration _configuration;

        public ExtratoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ExtratoModel> transferencias = new List<ExtratoModel>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using MySqlConnection connection = new MySqlConnection(connectionString);

            connection.Open();

            string query = "SELECT * FROM transactions";

            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ExtratoModel transferencia = new ExtratoModel
                {
                    TransactionId = rdr.GetInt32("TransactionId"),
                    AccountId = rdr.GetInt32("AccountId"),
                    TransactionType = rdr.GetString("TransactionType"),
                    Amount = rdr.GetDecimal("Amount"),
                    Date = rdr.GetDateTime("Date"),
                    OtherAccount = rdr.IsDBNull(rdr.GetOrdinal("OtherAccount")) ? null : (int?)rdr.GetInt32("OtherAccount")
                };

                transferencias.Add(transferencia);
            }

            return View("~/Views/Extrato/extrato.cshtml", transferencias);
        }

    }
}
