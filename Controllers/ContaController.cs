using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;
using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace BancoVirtual.Controllers
{
    public class ContaController : Controller
    {
        private readonly IConfiguration _configuration;

        public ContaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Ação para exibir o formulário de criação de conta
        public IActionResult Index()
        {
            return View("conta");
        }

        private string GerarNumeroDaConta()
        {
            // Crie uma instância da classe Random
            Random random = new Random();

            // Gere um número de conta aleatório de 10 dígitos
            int numeroAleatorio = random.Next(100000000, 999999999);

            return numeroAleatorio.ToString();
        }
        // Ação para processar o formulário de criação de conta
        [HttpPost]
        public IActionResult Criar(AccountViewModel model, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            if (ModelState.IsValid)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using MySqlConnection connection = new MySqlConnection(connectionString);

                try
                {
                    connection.Open();

                    // Recupere o UserId da sessão
                    var userId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");

                    if (userId.HasValue)
                    {
                        // Insira os dados na tabela Accounts
                        string accountNumber = GerarNumeroDaConta();
                        string query = "INSERT INTO Accounts (UserId, AccountNumber, Balance, AccountType, OpenedDate) VALUES (@UserId, @AccountNumber, @Balance, @AccountType, @OpenedDate)";

                        using MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        cmd.Parameters.AddWithValue("@Balance", model.Balance);
                        cmd.Parameters.AddWithValue("@AccountType", model.AccountType);
                        cmd.Parameters.AddWithValue("@OpenedDate", DateTime.Now);
                        cmd.ExecuteNonQuery();

                        Console.WriteLine("Conta Criada com sucesso");

                        // Redirecione para uma página de sucesso ou para outra ação
                        return RedirectToAction("Index", "Conta");
                    }
                    else
                    {
                        Console.WriteLine("UserId não encontrado");
                        return View("conta");
                    }
                }
                catch (Exception ex)
                {
                    // Lide com erros de forma apropriada, como exibir uma mensagem de erro
                    Console.WriteLine("Ocorreu um erro ao criar a conta: " + ex.Message);
                    return View(model);
                }
          
            }

            else
            {
                // O modelo não é válido; registre as mensagens de erro
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Erro de validação: {error.ErrorMessage}");
                    }
                }
                return View("conta");
            }

        }

        // Outros métodos do controlador, como "Sucesso" ou outros, se necessário
    }
}
