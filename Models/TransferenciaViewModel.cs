using System.ComponentModel.DataAnnotations;

namespace BancoVirtual.Models
{
    public class TransferenciaViewModel
    {
        [Required(ErrorMessage = "O campo ID do destinatário é obrigatório.")]
        [Display(Name = "ID do Destinatário")]
        public string ID { get; set; }

        [Required(ErrorMessage = "O campo Valor é obrigatório.")]
        [Display(Name = "Valor da Transferência")]
        public decimal Valor { get; set; }
    }
}
