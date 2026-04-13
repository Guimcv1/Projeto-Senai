using SCA.Back.Data;
using Microsoft.EntityFrameworkCore;

using UserServe = SCA.Back.Services.UsuarioService;
using Enum = SCA.Back.Data.Estados;

namespace SCA.Back.Services
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
                //Declarando um objeto para acessar a db
                using var context = new BancoContext();

                //Verifica se a sala j� existe
                if (context.Salas.Any(s => s.Descricao == desc.ToLower()))
                {
                    Console.WriteLine($"Erro: Nome '{desc}' j� existe.");
                    return false;
                }

                var item = new Sala
                {
                    Descricao = desc.ToLower(),
                    isAtivo = true
                };

                //Add no banco e salva no mesmo
                context.Salas.Add(item);
                context.SaveChanges();

                Console.WriteLine($"Item '{desc}' criado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar uma sala: {ex.Message}");
                return false;
            }
        }

        //EditarSala- Alterar a sala
        public static bool EditarSala(int id, string? novaDesc = null, string? NewStatos = null)
        {
            try
            {
                using var context = new BancoContext();
                var sala = context.Salas.Find(id);

                if (sala == null)
                {
                    Console.WriteLine($"Erro: Sala com ID \"{id}\" n�o encontrado.");
                    return false;
                }

                //Atualiza a descri��o se foi fornecido
                if (!string.IsNullOrEmpty(novaDesc))
                {
                    //Verifica se a nova descri��o j� existe e o Id � valido
                    if (context.Salas.Any(s => s.Descricao == novaDesc && s.Id != id))
                    {
                        Console.WriteLine($"Erro: Sala: \"{novaDesc}\" j� existe.");
                        return false;
                    }
                    sala.Descricao = novaDesc.ToLower();
                    
                }

                context.SaveChanges();
                Console.WriteLine($"Sala ID {id} atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Salas: {ex.Message}");
                return false;
            }
        }

        //ListarSala - Listar as Salas do Banco
        public static List<Sala> ListarSala()
        {
            try
            {
                using var context = new BancoContext();

                Console.WriteLine($"Sala bancos = {context.Salas.ToList()}");

                return context.Salas.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar os Itens: {ex.Message}");
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

        //BusacarPorDescSala - Busacar Sala Por Descri��o
        public static Sala? BuscarPorDescSala(string desc)
        {
            try
            { 
                using var context = new BancoContext();

                /*Retorna o que ele encontra no banco, caso ele n�o encontra ira retornar null
                   ToLower -> deixa toda str em minusculo
                 */
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
