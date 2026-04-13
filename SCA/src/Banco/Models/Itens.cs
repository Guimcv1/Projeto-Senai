using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Back.Data
{
    //Declaração o nome da tabela do banco de dados
    [Table("Itens")]

    public class Itens
    {
        //Declaração do id como chave primária e auto-incrementável
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /*[Required]
        public string Quantindade { get; set; }*/
        
        [Required, MaxLength(200)]
        //o String.Empty é para quando copilar não dar erro de valor nulo, pois o campo é obrigatório
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = Estados.Livre;

        public bool IsAtivo { get; set; } = false;

        public virtual ICollection<EmprestimoIntens> EmprestimoIntens { get; set; } = null!;

    }
}
