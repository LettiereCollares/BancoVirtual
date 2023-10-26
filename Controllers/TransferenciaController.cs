using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BancoVirtual.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace BancoVirtual.Controllers
{
    public class TransferenciaController : Controller
    {
        private readonly IConfiguration _configuration;

        public TransferenciaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Lógica do controlador aqui, se necessário
            return View("~/Views/Transferencia/transferencia.cshtml");
        }

        [HttpPost]
        public IActionResult RealizarTransferencia(TransferenciaViewModel model, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            if (ModelState.IsValid)
            {
                var remetenteUserId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");

                if (remetenteUserId.HasValue)
                {
                    if (int.TryParse(model.ID, out int destinatarioUserId))
                    {
                        var connectionString = _configuration.GetConnectionString("DefaultConnection");

                        using MySqlConnection connection = new MySqlConnection(connectionString);

                        try
                        {
                            connection.Open();

                            // Encontre o AccountId correspondente ao UserId do remetente
                            string remetenteQuery = "SELECT AccountId FROM Accounts WHERE UserId = @UserId";
                            using MySqlCommand remetenteCmd = new MySqlCommand(remetenteQuery, connection);
                            remetenteCmd.Parameters.AddWithValue("@UserId", remetenteUserId);

                            var remetenteAccountId = remetenteCmd.ExecuteScalar() is int remetenteId ? remetenteId : 0;

                            Console.WriteLine("RemetenteAccountId: " + remetenteAccountId); // Adicione esta linha

                            if (remetenteAccountId != 0)
                            {
                                // Encontre o AccountId correspondente ao UserId do destinatário
                                string destinatarioQuery = "SELECT AccountId FROM Accounts WHERE UserId = @UserId";
                                using MySqlCommand destinatarioCmd = new MySqlCommand(destinatarioQuery, connection);
                                destinatarioCmd.Parameters.AddWithValue("@UserId", destinatarioUserId);



                                var destinatarioAccountId = destinatarioCmd.ExecuteScalar() is int destinatarioId ? destinatarioId : 0;

                                Console.WriteLine("DestinatarioAccountId: " + destinatarioAccountId); // Adicione esta linha

                                if (destinatarioAccountId != 0)
                                {
                                    using var transaction = connection.BeginTransaction();

                                    // Insira registros na tabela Transactions para o remetente e destinatário
                                    string insertRemetenteQuery = "INSERT INTO Transactions (AccountId, TransactionType, Amount, Date, OtherAccount) VALUES (@AccountId, @TransactionType, @Amount, @Date, @OtherAccount)";
                                    using MySqlCommand insertRemetenteCmd = new MySqlCommand(insertRemetenteQuery, connection);
                                    insertRemetenteCmd.Parameters.AddWithValue("@AccountId", remetenteAccountId);
                                    insertRemetenteCmd.Parameters.AddWithValue("@TransactionType", "Deposit"); // Defina o valor diretamente
                                    insertRemetenteCmd.Parameters.AddWithValue("@Amount", -model.Valor); // Valor negativo para o remetente
                                    insertRemetenteCmd.Parameters.AddWithValue("@Date", DateTime.Now);
                                    insertRemetenteCmd.Parameters.AddWithValue("@OtherAccount", destinatarioAccountId);

                                    insertRemetenteCmd.ExecuteNonQuery();

                                    string insertDestinatarioQuery = "INSERT INTO Transactions (AccountId, TransactionType, Amount, Date, OtherAccount) VALUES (@AccountId, @TransactionType, @Amount, @Date, @OtherAccount)";
                                    using MySqlCommand insertDestinatarioCmd = new MySqlCommand(insertDestinatarioQuery, connection);
                                    insertDestinatarioCmd.Parameters.AddWithValue("@AccountId", destinatarioAccountId);
                                    insertDestinatarioCmd.Parameters.AddWithValue("@TransactionType", "Deposit"); // Defina o valor diretamente
                                    insertDestinatarioCmd.Parameters.AddWithValue("@Amount", model.Valor); // Valor positivo para o destinatário
                                    insertDestinatarioCmd.Parameters.AddWithValue("@Date", DateTime.Now);
                                    insertDestinatarioCmd.Parameters.AddWithValue("@OtherAccount", remetenteAccountId);

                                    insertDestinatarioCmd.ExecuteNonQuery();

                                    // Atualize os saldos das contas do remetente e destinatário
                                    AtualizarSaldos(remetenteUserId.Value, destinatarioUserId, remetenteAccountId, destinatarioAccountId, model.Valor, connection);
                                    transaction.Commit();

                                    decimal saldo = ObterSaldo(remetenteAccountId, connection);

                                    ViewBag.Saldo = saldo;

                                    

                                    // Redirecione para uma página de sucesso ou outra ação
                                    return RedirectToAction("Index");

                                }
                                else
                                {
                                    ModelState.AddModelError("ID", "ID do destinatário inválido.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Conta do remetente não encontrada.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Lide com erros de forma apropriada, como exibir uma mensagem de erro
                            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao processar a transferência.");
                            Console.WriteLine("Erro na transferência: " + ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ID", "ID do destinatário inválido.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário não identificado. Faça login para continuar.");
                }
            }

            return View("transferencia", model);
        }
        private void AtualizarSaldos(int remetenteUserId, int destinatarioUserId, int remetenteAccountId, int destinatarioAccountId, decimal valor, MySqlConnection connection)
        {
            // Recupere os saldos atuais do remetente e do destinatário
            decimal saldoRemetente = ObterSaldo(remetenteAccountId, connection);
            decimal saldoDestinatario = ObterSaldo(destinatarioAccountId, connection);

            // Atualize os saldos após a transferência
            saldoRemetente -= valor;
            saldoDestinatario += valor;

            // Atualize os saldos no banco de dados
            AtualizarSaldo(remetenteAccountId, saldoRemetente, connection);
            AtualizarSaldo(destinatarioAccountId, saldoDestinatario, connection);
        }

        private decimal ObterSaldo(int accountId, MySqlConnection connection)
        {
            string query = "SELECT Balance FROM Accounts WHERE AccountId = @AccountId";
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@AccountId", accountId);
            var saldo = cmd.ExecuteScalar();
            ViewBag.Saldo = saldo;
            return (saldo is DBNull) ? 0 : Convert.ToDecimal(saldo);
        }

        private void AtualizarSaldo(int accountId, decimal novoSaldo, MySqlConnection connection)
        {
            string query = "UPDATE Accounts SET Balance = @Balance WHERE AccountId = @AccountId";
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Balance", novoSaldo);
            cmd.Parameters.AddWithValue("@AccountId", accountId);
            cmd.ExecuteNonQuery();

        }
    }
}

