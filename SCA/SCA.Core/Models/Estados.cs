using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace SCA.Core.Models
{ 
    public class Estados
    {
        public const string Livre = "Livre";
        public const string Analise = "Analise";
        public const string AnaliseDevolucao = "AnaliseDevolucao";
        public const string Emprestado = "Emprestado";

        public const string Empty = "";

        public static readonly string[] TodosEstados = { Livre, Analise, AnaliseDevolucao, Emprestado, Empty };
    }

    public class AcaoTipo
    {
        public const string Usuario = "Usuario";
        public const string Item = "Item";
        public const string Sala = "Sala";
        public const string Emprestado = "Emprestado";

        public const string Empty = "";

        public static readonly string[] TodosTipoAcao = { Usuario, Item, Sala, Emprestado, Empty };

    }
}