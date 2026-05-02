using SCA.Core.Data;
using SCA.Core.Models;
using Microsoft.EntityFrameworkCore;

using UserServe = SCA.Core.Services.UsuarioService;
using Enum = SCA.Core.Models.Estados;

namespace SCA.Core.Services
{
    public class SalasService
    {

        //InativarSala- Inativa o Sala
        public static bool InativaSala(int id) 
        {
            try
            {
                using var context = new BancoContext();
                var sala = context.Salas.Find(id);

                if (sala != null) { sala.isAtivo = false; }

                context.SaveChanges();
                Console.WriteLine($"Sala ID {id} inativada com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Salas: {ex.Message}");
                return false;
            }
        }

        //CriarSala - Adiciona uma sala
        public static bool CriarSala(string desc)
        {
            try
            {
                using var context = new BancoContext();

                if (context.Salas.Any(s => s.Descricao == desc.ToLower()))
                {
                    Console.WriteLine($"Erro: Sala '{desc}' já existe.");
                    return false;
                }

                var item = new Sala
                {
                    Descricao = desc.ToLower(),
                    isAtivo = true
                };

                context.Salas.Add(item);
                context.SaveChanges();

                Console.WriteLine($"Sala '{desc}' criada com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar uma sala: {ex.Message}");
                return false;
            }
        }

        //EditarSala- Alterar a sala
        public static bool EditarSala(int id, string? novaDesc = null, bool? novoStatus = null)
        {
            try
            {
                using var context = new BancoContext();
                var sala = context.Salas.Find(id);

                if (sala == null) return false;

                if (!string.IsNullOrEmpty(novaDesc)) sala.Descricao = novaDesc.ToLower();
                if (novoStatus.HasValue) sala.isAtivo = novoStatus.Value;

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
                return false;
            }
        }

        //ListarSala - Listar as Salas do Banco
        public static List<Sala> ListarSala()
        {
            try
            {
                using var context = new BancoContext();
                return context.Salas.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar as Salas: {ex.Message}");
                return new List<Sala>();
            }
        }

        //BuscarPorIdSala - Buscar Sala por ID
        public static Sala? BuscarPorIdSala(int id)
        {
            try
            { 
                using var context = new BancoContext();
                return context.Salas.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Itens: {ex.Message}");
                return null;
            }
        }

        //BusacarPorDescSala - Busacar Sala Por Descrição
        public static Sala? BuscarPorDescSala(string desc)
        {
            try
            { 
                using var context = new BancoContext();
                return context.Salas.FirstOrDefault(s => s.Descricao == desc.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Itens: {ex.Message}");
                return null;
            }
        }

    }
}
