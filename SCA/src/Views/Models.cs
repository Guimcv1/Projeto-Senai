using System;

namespace Keys_manager___Tester
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Perfil { get; set; } // "Admin" ou "User"
        public bool IsAtivo { get; set; }
    }

    public class AmbienteTemp
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CorStatus { get; set; } // Ex: "#16a34a" (Verde)
    }
}