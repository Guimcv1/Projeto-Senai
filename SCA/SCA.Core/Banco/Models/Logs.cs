// O "SQLAlchemy" do C#
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SCA.Back.Data
{
    //Declaração o nome da tabela do banco de dados
    [Table("Logs")]

    public class Logs
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
        public string Acao { get; set; } = string.Empty;

        public DateTime DataAcao { get; set; } = DateTime.Now;

        [Required]
        public string TipoAcao { get; set; } = AcaoTipo.Empty;

        //FK para o Usuário
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;


    }
}
