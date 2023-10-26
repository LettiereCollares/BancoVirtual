using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;
using System;
using Microsoft.AspNetCore.Http;

namespace BancoVirtual.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View("~/Views/Home/pagina.cshtml");
        }

        [HttpPost]
        [ActionName("Login")]
        public IActionResult Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("O Modelo é Valido");
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                // Verifique se o email e senha correspondem a um registro na tabela Users
                string query = "SELECT UserId FROM users WHERE Email = @Email AND Password = @Senha";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Senha", model.Password);

                var userId = cmd.ExecuteScalar();

                if (userId != null)
                {

                    HttpContext.Session.SetInt32("UserId", (int)userId);
                    return RedirectToAction("Index", "PaginaInicial");
                }
                else
                {
                    Console.WriteLine("Email ou senha inválidos, tente novamente.");
                    return View("~/Views/Home/pagina.cshtml");
                }
            }
            catch (Exception ex)
            {
                // Registre o erro
                Console.WriteLine("Erro ao efetuar login." + ex.Message);
                return View("~/Views/Home/pagina.cshtml");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}