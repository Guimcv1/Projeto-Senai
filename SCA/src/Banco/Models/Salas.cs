using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Back.Data
{

    //Declaração o nome da tabela do banco de dados
    [Table("Sala")]

    public class Sala
    {
        //Declaração do id como chave primária e auto-incrementável
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        //o String.Empty é para quando copilar é não dar erro de valor nulo, pois o campo é obrigatório
        public string Descricao { get; set; } = string.Empty;

        public bool isAtivo { get; set; } = false;

        public virtual ICollection<Emprestimos> Emprestimos { get; set; } = null!;
    }
}
