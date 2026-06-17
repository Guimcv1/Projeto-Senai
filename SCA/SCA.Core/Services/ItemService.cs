using SCA.Core.Data;
using SCA.Core.Models;
using Microsoft.EntityFrameworkCore;

using UserServe = SCA.Core.Services.UsuarioService;
using Estados = SCA.Core.Models.Estados;

namespace SCA.Core.Services
{
    public class AdminService
    {

        //InativarIntens - Inativa o Intes
        public static bool InativaIntens(int id, int? usuarioLogadoId = null) 
        {
            try
            {
                using var context = new BancoContext();
                var intes = context.Itens.Find(id);

                if (intes != null) { intes.IsAtivo = false; }

                context.SaveChanges();

                if (usuarioLogadoId.HasValue && usuarioLogadoId.Value > 0 && intes != null)
                {
                    LogService.RegistrarLog("Inativou item", AcaoTipo.Item, usuarioLogadoId.Value, alvo: intes.Descricao);
                }

                Console.WriteLine($"Item ID {id} inativada com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Item: {ex.Message}");
                return false;
            }
        }

        //CriarInten - Adiciona os intnes
        public static bool CriarIntens(string descricao, int salaId, int? usuarioLogadoId = null)
        {
            try
            {
                //Declarando um objeto para acessar a db
                using var context = new BancoContext();

                //Verifica se o item já existe
                if (context.Itens.Any(u => u.Descricao == descricao))
                {
                    Console.WriteLine($"Erro: descricao '{descricao}' já existe.");
                    return false;
                }

                // Verifica se a sala existe
                var sala = context.Salas.Find(salaId);
                if (sala == null)
                {
                    Console.WriteLine($"Erro: Sala com ID {salaId} não encontrada.");
                    return false;
                }

                var item = new Item
                {
                    Descricao = descricao,
                    Estado = Estados.Livre,
                    IsAtivo = true,
                    SalaId = salaId
                };

                context.Itens.Add(item);
                context.SaveChanges();

                if (usuarioLogadoId.HasValue && usuarioLogadoId.Value > 0)
                {
                    LogService.RegistrarLog("Criou item", AcaoTipo.Item, usuarioLogadoId.Value, alvo: $"{item.Descricao} (Sala {salaId})");
                }

                Console.WriteLine($"Item '{descricao}' criado com sucesso na Sala {salaId}!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar item: {ex.Message}");
                return false;
            }
        }

        //EditarInten - Alterar os Item no banco
        public static bool EditarIntens(int id, string? novaDescricao = null, string? NewStatos = null, int? novaSalaId = null, int? usuarioLogadoId = null)
        {
            try
            {
                using var context = new BancoContext();
                var inten = context.Itens.Find(id);

                if (inten == null)
                {
                    Console.WriteLine($"Erro: Iten com ID \"{id}\" não encontrado.");
                    return false;
                }

                //Atualiza a descrição se foi fornecido
                if (!string.IsNullOrEmpty(novaDescricao))
                {
                    //Verifica se a nova descrição já existe
                    if (context.Itens.Any(u => u.Descricao == novaDescricao && u.Id != id))
                    {
                        Console.WriteLine($"Erro: Item \"{novaDescricao}\" já existe.");
                        return false;
                    }
                    inten.Descricao = novaDescricao;
                }

                if (!string.IsNullOrEmpty(NewStatos) && Array.Exists(Estados.TodosEstados, e => e == NewStatos))
                {
                    inten.Estado = NewStatos;
                }

                if (novaSalaId.HasValue)
                {
                    var sala = context.Salas.Find(novaSalaId.Value);
                    if (sala != null)
                    {
                        inten.SalaId = novaSalaId.Value;
                    }
                }

                context.SaveChanges();

                if (usuarioLogadoId.HasValue && usuarioLogadoId.Value > 0)
                {
                    LogService.RegistrarLog("Atualizou item", AcaoTipo.Item, usuarioLogadoId.Value, alvo: $"{inten.Descricao} (ID {inten.Id})");
                }

                Console.WriteLine($"Item ID {id} atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar itens: {ex.Message}");
                return false;
            }
        }

        //ListarIntens - Listar os Item do Banco
        public static List<Item> ListarIntens()
        {
            try
            {
                using var context = new BancoContext();

                Console.WriteLine($"Item bancos = {context.Itens.ToList()}");

                return context.Itens.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar os Item: {ex.Message}");
                return new List<Item>();
            }
        }

        //BuscarPorIdIntens - Buscar usuário por ID
        public static Item? BuscarPorIdIntens(int id)
        {
            try
            {
                using var context = new BancoContext();
                return context.Itens.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Item: {ex.Message}");
                return null;
            }
        }

    }
}