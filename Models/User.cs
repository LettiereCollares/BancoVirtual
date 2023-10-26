using System;
using System.ComponentModel.DataAnnotations;

namespace BancoVirtual.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "O campo Nome Completo é obrigatório.")]
        public string NomeCompleto { get; set; }

        public string Endereco { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O campo CPF deve conter 11 dígitos.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\(\d{2}\) \d{4}-\d{4}$", ErrorMessage = "O campo Celular deve seguir o formato (xx) xxxx-xxxx.")]
        public string Celular { get; set; }
    }
}
