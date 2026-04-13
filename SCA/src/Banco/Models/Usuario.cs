using Microsoft.EntityFrameworkCore; 
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Back.Data
{
    //Declaração o nome da tabela do banco de dados
    [Table("usuarios")]

    public class Usuario
    {
        //Declaração do id como chave primária e auto-incrementável
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /*Declaração dos campos da tabela com suas respectivas validações
           Required: O campo é obrigatório
         */
        [Required, MaxLength(200)]
        //o String.Empty é para quando copilar não dar erro de valor nulo, pois o campo é obrigatório
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        //Aqui não precisa ter o Required porque o valor padrão é false
        public bool IsAdmin { get; set; } = false;

        public bool IsAtivo { get; set; } = true;

        //Base para o Fk
        public virtual ICollection<Emprestimos> Emprestimos { get; set; } = null!;

        public virtual ICollection<Logs> Logs { get; set; } = null!;
    }
}
