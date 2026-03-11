using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace gerenciador_chaves.Back.Data
{
    public static class Estados
    {
        public const string Livre = "Livre";
        public const string Analise = "Analise";
        public const string Emprestado = "Emprestado";
        public const string Inativo = "Inativo";

        public static readonly string[] TodosEstados = { Livre, Analise, Inativo, Emprestado };
    }


    //Declaração o nome da tabela do banco de dados
    [Table("itens")]

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

    }
}
