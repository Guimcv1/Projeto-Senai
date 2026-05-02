using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Core.Models
{
    //Declaração o nome da tabela do banco de dados
    [Table("EmprestimoItem")]

    public class EmprestimoItem
    {
        //Declaração do id como chave primária e auto-incrementável
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //FK para o Item
        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;

        //FK para o Emprestimo
        public int EmprestimoId { get; set; }
        public virtual Emprestimos Emprestimos { get; set; } = null!;

    }
}