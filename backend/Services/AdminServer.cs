using gerenciador_chaves.Back.Data;
using Microsoft.EntityFrameworkCore;

using UserServe = gerenciador_chaves.Back.Services.UsuarioService;

namespace gerenciador_chaves.Back.Services
{
    public class AdminService
    {
        //IsAdmin - Ver o ser o user é admin
        public static bool IsAdmin(Usuario useObje) { return useObje.IsAdmin; }

        //InativarIntens - Inativa o Intes
        public static bool InativaIntens(int id) { return EditarInten(id, null, Estados.Inativo); }

        //CriarInten - Adiciona os intnes
        public static bool CriarInten(string descricao)
        {
            try
            {
                //Declarando um objeto para acessar a db
                using var context = new BancoContext();

                //  Verifica se o inten já existe
                if (context.Intens.Any(u => u.Descricao == descricao))
                {
                    Console.WriteLine($"Erro: descricao '{descricao}' já existe.");
                    return false;
                }

                var item = new Itens
                {
                    Descricao = descricao,
                    Estado = Estados.Livre
                };

                context.Intens.Add(item);
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

        //EditarInten - Alterar os Intens no banco
        public static bool EditarInten(int id, string? novaDescricao = null, string? NewStatos = null)
        {
            try
            {
                using var context = new BancoContext();
                var inten = context.Intens.Find(id);

                if (inten == null)
                {
                    Console.WriteLine($"Erro: Iten com ID \"{id}\" não encontrado.");
                    return false;
                }

                //Atualiza a descrição se foi fornecido
                if (!string.IsNullOrEmpty(novaDescricao))
                {
                    //Verifica se a nova descrição já existe
                    if (context.Intens.Any(u => u.Descricao == novaDescricao && u.Id != id))
                    {
                        Console.WriteLine($"Erro: Inten \"{novaDescricao}\" já existe.");
                        return false;
                    }
                    inten.Descricao = novaDescricao;
                }

                if (!string.IsNullOrEmpty(NewStatos) && Estados.TodosEstados.Contains(NewStatos))
                {
                    inten.Estado = NewStatos;
                }


                context.SaveChanges();
                Console.WriteLine($"Intens ID {id} atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar intens: {ex.Message}");
                return false;
            }
        }

        //ListarIntens - Listar os Intens do Banco
        public static List<Itens> ListarIntens()
        {
            try
            {
                using var context = new BancoContext();

                Console.WriteLine($"Intens bancos = {context.Intens.ToList()}");

                return context.Intens.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar os Intens: {ex.Message}");
                return new List<Itens>();
            }
        }

        //BuscarPorIdIntens - Buscar usuário por ID
        public static Itens? BuscarPorIdIntens(int id)
        {
            try
            {
                using var context = new BancoContext();
                return context.Intens.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Intens: {ex.Message}");
                return null;
            }
        }

    }
}