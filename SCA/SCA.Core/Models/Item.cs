using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Core.Models
{
    //Declaração o nome da tabela do banco de dados
    [Table("Item")]

    public class Item
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

        // FK para a Sala vinculada a este Item
        public int SalaId { get; set; }
        public virtual Sala Sala { get; set; } = null!;

        public virtual ICollection<EmprestimoItem> EmprestimoItem { get; set; } = null!;

    }
}
