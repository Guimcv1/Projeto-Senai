using SCA.Back.Data;
using SCA.Back.Execel;
using Microsoft.EntityFrameworkCore;

namespace SCA.Back.Services
{
    public class EmprestimosService
    {
        //Ajudar a permite só uma das escritas(evitando erros de digitação)
        public enum TipoSolicitacao { Emprestimo, Devolucao }

        //Filtrar Emprestimos - Filtro básico de tempo e categoria compatível com o TipoExeport
        public static List<Emprestimos> FiltrarEmprestimo(ExportarExecel.TipoExeport tipo, DateTime? inicio = null, DateTime? fim = null)
        {
            try
            {
                using var context = new BancoContext();
                var query = context.Emprestimos
                    .Include(e => e.Usuario)
                    .Include(e => e.Sala)
                    .Include(e => e.EmprestimoIntens)
                    .ThenInclude(ei => ei.Itens)
                    .AsQueryable();

                if (inicio.HasValue)
                    query = query.Where(e => e.DataEstado >= inicio.Value);
                if (fim.HasValue)
                    query = query.Where(e => e.DataEstado <= fim.Value);

                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao filtrar empréstimos: {ex.Message}");
                return new List<Emprestimos>();
            }
        }

        //SolicitarEmprestimo - O usuário requisita os itens
        public static bool SolicitarEmprestimo(int usuarioId, int salaId, List<int> itensIds)
        {
            try
            {
                using var context = new BancoContext();

                //Cria o empréstimo com o status inicial 'Analise' (Aguardando aprovação)
                var emprestimo = new Emprestimos
                {
                    UsuarioId = usuarioId, SalaId = salaId, Estado = Estados.Analise,
                    DataEstado = DateTime.Now
                };

                context.Emprestimos.Add(emprestimo);
                context.SaveChanges();

                //Salva todos os itens vinculados a esse empréstimo
                foreach (var i in itensIds)
                {
                    var emprestimoItem = new EmprestimoIntens
                    {
                        EmprestimoId = emprestimo.Id,
                        ItemId = i
                    };
                    context.EmprestimoIntens.Add(emprestimoItem);

                    var item = context.Itens.Find(i);
                    //Atualiza o estado do Item para Analise
                    if (item != null) { item.Estado = Estados.Analise; }

                }

                context.SaveChanges();
                Console.WriteLine($"Empréstimo ID {emprestimo.Id} solicitado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao solicitar empréstimo: {ex.Message}");
                return false;
            }
        }

        //SolicitarDevolucao - O usuário consegue solicitar devolver os itens
        public static bool SolicitarDevolucao(int emprestimoId)
        {
            try
            {
                using var context = new BancoContext();
                var emprestimo = context.Emprestimos.Find(emprestimoId);

                if (emprestimo == null)
                {
                    Console.WriteLine($"Erro: Empréstimo com ID \"{emprestimoId}\" não encontrado.");
                    return false;
                }

                //Retorna o estado para Analise, indicando que deseja devolver
                emprestimo.Estado = Estados.Analise;
                emprestimo.DataEstado = DateTime.Now;

                context.SaveChanges();
                Console.WriteLine($"Devolução do Empréstimo ID {emprestimoId} solicitada com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao solicitar devolução: {ex.Message}");
                return false;
            }
        }

        //AprovarSolicitacao - Aprova/Negar a solicitação do user
        public static bool AprovarSolicitacao(int emprestimoId, TipoSolicitacao tipo, bool isAprovado = true)
        {
            try
            {
                using var context = new BancoContext();

                #region emprestimo
                var emprestimo = context.Emprestimos
                    .Include(e => e.EmprestimoIntens)
                    .ThenInclude(ei => ei.Itens)
                    .FirstOrDefault(e => e.Id == emprestimoId);
                #endregion

                if (emprestimo == null) return false;

                //Usando Switch Expression para definir os estados é criar as tudas var(novoEstadoEmprestimo, novoEstadoItem)
                var (novoEstadoEmprestimo, novoEstadoItem) = (tipo, isAprovado) switch
                {
                    //se for dó tipo Emprestimo e foi aprovado 
                    (TipoSolicitacao.Emprestimo, true) => (Estados.Emprestado, Estados.Emprestado),
                    //se for dó tipo Emprestimo e foi recusado
                    (TipoSolicitacao.Emprestimo, false) => (Estados.Livre, Estados.Livre),
                    //se for dó tipo Devolucao e foi ativado
                    (TipoSolicitacao.Devolucao, true) => (Estados.Livre, Estados.Livre),
                    //se for dó tipo Devolucao e foi recusado
                    (TipoSolicitacao.Devolucao, false) => (Estados.Emprestado, Estados.Emprestado),

                    //Lança um erro 
                    _ => throw new ArgumentException($"Tipo {tipo} no momento não pode ser {(isAprovado ? "aprovada" : "negada")}")
                };

                emprestimo.Estado = novoEstadoEmprestimo;
                emprestimo.DataEstado = DateTime.Now;

                foreach (var i in emprestimo.EmprestimoIntens)
                {
                    if (i.Itens != null) { i.Itens.Estado = novoEstadoItem; }
                }

                context.SaveChanges();

                Console.WriteLine($"Solicitação de {tipo} do Empréstimo ID {emprestimoId} {(isAprovado ? "aprovada" : "negada")} com sucesso!");
                return true;
            }   
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar solicitação: {ex.Message}");
                return false;
            }
        }

        //ListarEmprestimos - Listar os Emprestimos do Banco
        public static List<Emprestimos> ListarEmprestimo()
        {
            try
            {
                using var context = new BancoContext();
                return context.Emprestimos
                    .Include(e => e.Usuario)
                    .Include(e => e.Sala)
                    .Include(e => e.EmprestimoIntens)
                    .ThenInclude(ei => ei.Itens)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar os Empréstimos: {ex.Message}");
                return new List<Emprestimos>();
            }
        }

        //BuscarPorIdEmprestimo - Buscar empréstimo por ID
        public static Emprestimos? BuscarPorIdEmprestimo(int id)
        {
            try
            {
                using var context = new BancoContext();
                return context.Emprestimos
                    .Include(e => e.Usuario)
                    .Include(e => e.Sala)
                    .Include(e => e.EmprestimoIntens)
                    .ThenInclude(ei => ei.Itens)
                    .FirstOrDefault(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Empréstimo: {ex.Message}");
                return null;
            }
        }
    }
}