using System;
using System.ComponentModel.DataAnnotations;

namespace BancoVirtual.Models
{
    public class ExtratoModel
    {
        public int TransactionId { get; set; }

        [Display(Name = "ID da Conta")]
        public int AccountId { get; set; }

        [Display(Name = "Tipo de Transação")]
        public string TransactionType { get; set; }

        [Display(Name = "Valor")]
        public decimal Amount { get; set; }

        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Display(Name = "Outra Conta")]
        public int? OtherAccount { get; set; }
    }
}
