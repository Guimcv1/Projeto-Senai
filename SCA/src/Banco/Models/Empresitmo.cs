using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;


namespace SCA.Back.Data
{
    [Table("Emprestimos")]

    public class Emprestimos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime DataEstado { get; set; } = DateTime.Now;

        public string Estado { get; set; } = Estados.Empty;

        //FK para o Usuário
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;

        //FK para a Sala (A sala onde o item está ou para onde foi levado)
        public int SalaId { get; set; }
        public virtual Sala Sala { get; set; } = null!;

        //Base apra o Fk 
        public virtual ICollection<EmprestimoIntens> EmprestimoIntens { get; set; } = null!;

    }
}
