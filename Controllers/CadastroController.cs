using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;
using System;

namespace BancoVirtual.Controllers
{
    public class CadastroController : Controller
    {
        private readonly IConfiguration _configuration;

        public CadastroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View("cadastro");
        }

        [HttpPost]
        public IActionResult Cadastrar(User user)
        {

            if (ModelState.IsValid)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using MySqlConnection connection = new MySqlConnection(connectionString);

                try
                {
                    connection.Open();

                    // Inserir os dados na tabela Users
                    string query = "INSERT INTO Users (NomeCompleto, Endereco, CPF, Email, Password, Celular) VALUES (@NomeCompleto, @Endereco, @CPF, @Email, @Password, @Celular)";

                    using MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@NomeCompleto", user.NomeCompleto);
                    cmd.Parameters.AddWithValue("@Endereco", user.Endereco);
                    cmd.Parameters.AddWithValue("@CPF", user.CPF);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Celular", user.Celular);
                    cmd.ExecuteNonQuery();

                    // Após o sucesso do cadastro, redirecione para outra página ou ação
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Lide com erros de forma apropriada, como exibir uma mensagem de erro
                    Console.WriteLine("Ocorreu um erro ao cadastrar o usuário: " + ex.Message);
                    return View("cadastro");
                }
                finally
                {
                    connection.Close();
                }
            }

            else
            {
                return View("cadastro",user);
            }
        }

        // Ação para exibir a página de sucesso de cadastro
        public IActionResult CadastroSucesso()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
