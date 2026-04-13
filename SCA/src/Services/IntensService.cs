using SCA.Back.Data;
using Microsoft.EntityFrameworkCore;

using UserServe = SCA.Back.Services.UsuarioService;
using Enum = SCA.Back.Data.Estados;

namespace SCA.Back.Services
{
    public class AdminService
    {

        //InativarIntens - Inativa o Intes
        public static bool InativaIntens(int id) 
        {
            try
            {
                using var context = new BancoContext();
                var intes = context.Itens.Find(id);

                if (intes != null) { intes.IsAtivo = false; }

                context.SaveChanges();
                Console.WriteLine($"Itens ID {id} inativada com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Itens: {ex.Message}");
                return false;
            }
        }

        //CriarInten - Adiciona os intnes
        public static bool CriarIntens(string descricao)
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

                var item = new Itens
                {
                    Descricao = descricao,
                    Estado = Estados.Livre,
                    IsAtivo = true
                };

                context.Itens.Add(item);
                context.SaveChanges();

                Console.WriteLine($"Item '{descricao}' criado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar item: {ex.Message}");
                return false;
            }
        }

        //EditarInten - Alterar os Itens no banco
        public static bool EditarIntens(int id, string? novaDescricao = null, string? NewStatos = null)
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

                if (!string.IsNullOrEmpty(NewStatos) && Array.Exists(Enum.TodosEstados, e => e == NewStatos))
                {
                    inten.Estado = NewStatos;
                }


                context.SaveChanges();
                Console.WriteLine($"Itens ID {id} atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar itens: {ex.Message}");
                return false;
            }
        }

        //ListarIntens - Listar os Itens do Banco
        public static List<Itens> ListarIntens()
        {
            try
            {
                using var context = new BancoContext();

                Console.WriteLine($"Itens bancos = {context.Itens.ToList()}");

                return context.Itens.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar os Itens: {ex.Message}");
                return new List<Itens>();
            }
        }

        //BuscarPorIdIntens - Buscar usuário por ID
        public static Itens? BuscarPorIdIntens(int id)
        {
            try
            {
                using var context = new BancoContext();
                return context.Itens.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Itens: {ex.Message}");
                return null;
            }
        }

    }
}