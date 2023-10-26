using System;
using System.ComponentModel.DataAnnotations;

namespace BancoVirtual.Models
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "O campo Saldo é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O saldo deve ser maior que 0.")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de conta.")]
        public string AccountType { get; set; }

    }
}
