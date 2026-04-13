using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Back.Data
{
    //Declaração o nome da tabela do banco de dados
    [Table("EmprestimoIntens")]

    public class EmprestimoIntens
    {
        //Declaração do id como chave primária e auto-incrementável
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //FK para o Item
        public int ItemId { get; set; }
        public virtual Itens Itens { get; set; } = null!;

        //FK para o Emprestimo
        public int EmprestimoId { get; set; }
        public virtual Emprestimos Emprestimos { get; set; } = null!;

    }
}