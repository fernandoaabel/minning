using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Avaliador_Textual.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar de mim?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "A senha informada está diferente da confirmação da senha.")]
        public string ConfirmPassword { get; set; }
    }
}
