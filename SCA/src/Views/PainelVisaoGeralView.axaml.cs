using SCA.Core.Data;
using Avalonia;
using Microsoft.EntityFrameworkCore;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Media;
using SCA.Core.Models;
using SCA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCA.Views;

public partial class PainelVisaoGeralView : UserControl, IReloadableView
{
    private JanelaPrincipal? _parent;
    private List<Sala> _todasSalas = new();

    private void RegistrarAcao(string acao, string tipoAcao = "Item", int? usuarioId = null)
    {
        int id = usuarioId ?? _parent?.CurrentAdmin?.Id ?? 0;
        if (id > 0)
        {
            SCA.Core.Services.LogService.RegistrarLog(acao, tipoAcao, id);
        }
    }

    public PainelVisaoGeralView()
    {
        InitializeComponent();
    }

    public PainelVisaoGeralView(JanelaPrincipal parent) : this()
    {
        _parent = parent;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadSalas();
    }

    public void Reload()
    {
        LoadSalas();
    }

    private void LoadSalas()
    {
        try
        {
            _todasSalas = SalaService.ListarSala();
            FilterDashboard();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar ambientes: {ex.Message}");
        }
    }

    private void FilterDashboard()
    {
        if (icAmbientes == null || txtSearch == null) return;

        string searchText = txtSearch.Text?.ToLower() ?? "";

        var salasFiltradas = _todasSalas.Where(s =>
            (string.IsNullOrEmpty(searchText) ||
             (s.Descricao != null && s.Descricao.ToLower().Contains(searchText)))
        ).ToList();

        var ambientesUI = new List<AmbienteTemp>();

        using var context = new BancoContext();
        var todosItens = context.Itens.ToList();
        var todosEmprestimos = context.Emprestimos.ToList();

        foreach (var sala in salasFiltradas)
        {
            string cor = GetRoomColor(sala.Id, todosItens, todosEmprestimos);

            ambientesUI.Add(new AmbienteTemp
            {
                Id = sala.Id,
                Nome = sala.Descricao + (!sala.isAtivo ? " (Inativo)" : ""),
                CorStatus = !sala.isAtivo ? "#94a3b8" : cor
            });
        }

        icAmbientes.ItemsSource = ambientesUI;
    }

    private string GetRoomColor(int salaId, List<Item> itens, List<Emprestimos> emprestimos)
    {
        var itensDaSala = itens.Where(i => i.SalaId == salaId).ToList();
        if (itensDaSala.Count == 0) return "#94a3b8";

        int total = itensDaSala.Count;
        int available = itensDaSala.Count(i => i.Estado == Estados.Livre);
        int borrowed = itensDaSala.Count(i => i.Estado == Estados.Emprestado);
        int pending = itensDaSala.Count(i => i.Estado == Estados.Analise);

        if (pending == total) return "#94a3b8";
        if (available == total) return "#16a34a";
        if (borrowed == total) return "#dc2626";
        if (available > 0) return "#ea580c";

        return "#dc2626";
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterDashboard();
    }

    private void RoomCard_Click(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is AmbienteTemp ambiente)
        {
            var sala = _todasSalas.FirstOrDefault(s => s.Id == ambiente.Id);
            if (sala != null && !sala.isAtivo)
            {
                return;
            }

            if (sala != null)
            {
                OpenRoomDetails(ambiente.Id, sala.Descricao);
                RegistrarAcao($"Abriu detalhes da sala {sala.Id}: {sala.Descricao}", AcaoTipo.Sala);
            }
        }
    }

    private void OpenRoomDetails(int salaId, string salaName)
    {
        if (txtDialogTitle == null || dgItems == null || RoomDialogOverlay == null) return;

        txtDialogTitle.Text = $"Item em: {salaName}";

        using var context = new BancoContext();
        var itensDaSala = context.Itens.Where(i => i.SalaId == salaId).ToList();

        var dialogItems = new List<ItemUI>();

        foreach (var item in itensDaSala)
        {
            var uiItem = new ItemUI
            {
                Id = item.Id,
                Descricao = item.Descricao,
                EstadoOrigem = item.Estado
            };

            if (item.Estado == Estados.Livre)
            {
                uiItem.BadgeText = "Disponível";
                uiItem.BadgeColor = "#16a34a";
                uiItem.CanSelect = true;
            }
            else if (item.Estado == Estados.Analise)
            {
                uiItem.BadgeText = "Em Análise";
                uiItem.BadgeColor = "#94a3b8";
                uiItem.CanSelect = false;
            }
            else if (item.Estado == Estados.Emprestado)
            {
                uiItem.BadgeText = "Emprestado";
                uiItem.BadgeColor = "#dc2626";
                uiItem.CanSelect = true;
            }

            dialogItems.Add(uiItem);
        }

        dgItems.ItemsSource = dialogItems;
        RoomDialogOverlay.IsVisible = true;
    }

    private void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        if (RoomDialogOverlay != null) RoomDialogOverlay.IsVisible = false;
        RegistrarAcao("Fechou dialogo de detalhes da sala", AcaoTipo.Sala);
    }

    private List<ItemUI> _pendingSelectedItems = new();

    private void ProcessSelectedItems_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems == null || RoomDialogOverlay == null || LoginDialogOverlay == null) return;

        var dialogItems = dgItems.ItemsSource as List<ItemUI>;
        _pendingSelectedItems = dialogItems?.Where(i => i.IsSelected).ToList() ?? new();

        if (!_pendingSelectedItems.Any())
        {
            _parent?.ShowMessage("Nenhum item selecionado.");
            return;
        }

        // Close room details and open login
        RoomDialogOverlay.IsVisible = false;

        txtRequestLogin.Text = "";
        txtRequestPassword.Text = "";
        LoginDialogOverlay.IsVisible = true;
        RegistrarAcao($"Selecionou {_pendingSelectedItems.Count} itens para solicitar", AcaoTipo.Item);
    }

    private void CloseLoginDialog_Click(object sender, RoutedEventArgs e)
    {
        if (LoginDialogOverlay != null) LoginDialogOverlay.IsVisible = false;
        RegistrarAcao("Fechou dialogo de solicitacao", AcaoTipo.Usuario);
    }

    private bool _isRequestPasswordVisible = false;
    private void ToggleRequestPasswordVisibility_Click(object sender, RoutedEventArgs e)
    {
        _isRequestPasswordVisible = !_isRequestPasswordVisible;
        if (txtRequestPassword != null && iconRequestPasswordVisibility != null)
        {
            if (_isRequestPasswordVisible)
            {
                txtRequestPassword.PasswordChar = '\0';
                iconRequestPasswordVisibility.Kind = Material.Icons.MaterialIconKind.Eye;
            }
            else
            {
                txtRequestPassword.PasswordChar = '*';
                iconRequestPasswordVisibility.Kind = Material.Icons.MaterialIconKind.EyeOff;
            }
        }
        RegistrarAcao("Alternou visibilidade da senha de solicitacao", AcaoTipo.Usuario);
    }

    private void ConfirmLoanRequest_Click(object sender, RoutedEventArgs e)
    {
        string login = txtRequestLogin.Text?.Trim() ?? "";
        string senha = txtRequestPassword.Text ?? "";

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
        {
            _parent?.ShowMessage("Informe login e senha.");
            return;
        }

        var usuario = UsuarioService.LoginUser(login, senha);
        if (usuario == null)
        {
            _parent?.ShowMessage("Usuário ou senha incorretos.");
            var usuarioTentativa = UsuarioService.ListarUser().FirstOrDefault(u => u.Login == login);
            if (usuarioTentativa != null)
            {
                RegistrarAcao($"Tentativa de autenticacao de solicitacao com falha para {usuarioTentativa.Login}", AcaoTipo.Usuario, usuarioTentativa.Id);
            }
            return;
        }

        int currentUserId = usuario.Id;
        RegistrarAcao($"Autenticou solicitacao com usuario {usuario.Login}", AcaoTipo.Usuario, currentUserId);
        LoginDialogOverlay.IsVisible = false;

        // Determine if we are requesting Loans or Returns
        var itemsToLoan = _pendingSelectedItems.Where(i => i.EstadoOrigem == Estados.Livre).Select(i => i.Id).ToList();
        var itemsToReturn = _pendingSelectedItems.Where(i => i.EstadoOrigem == Estados.Emprestado).ToList();

        if (itemsToLoan.Any())
        {
            using var context = new BancoContext();
            var firstItem = context.Itens.Find(itemsToLoan.First());
            int salaId = firstItem?.SalaId ?? 0;

            if (salaId != 0)
            {
                if (EmprestimoService.SolicitarEmprestimo(currentUserId, salaId, itemsToLoan))
                {
                    _parent?.ShowMessage($"Solicitado empréstimo de {itemsToLoan.Count} itens.", false);
                }
            }
        }

        if (itemsToReturn.Any())
        {
            using var context = new BancoContext();
            int returnCount = 0;
            foreach (var item in itemsToReturn)
            {
                var activeLoan = context.Emprestimos
                    .Include(e => e.EmprestimoItem)
                    .Where(e => e.Estado == Estados.Emprestado && e.EmprestimoItem.Any(ei => ei.ItemId == item.Id))
                    .OrderByDescending(e => e.DataEstado)
                    .FirstOrDefault();

                if (activeLoan != null)
                {
                    if (EmprestimoService.SolicitarDevolucao(activeLoan.Id, currentUserId))
                    {
                        returnCount++;
                    }
                }
            }
            if (returnCount > 0)
            {
                _parent?.ShowMessage($"Solicitada devolução de {returnCount} itens.", false);
            }
        }

        FilterDashboard();
    }
}

public class AmbienteTemp
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string CorStatus { get; set; } = ""; // Ex: "#16a34a" (Verde)
}

public class ItemUI
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public bool IsSelected { get; set; }
    public bool CanSelect { get; set; }
    public string BadgeText { get; set; } = "";
    public string BadgeColor { get; set; } = "";
    public string EstadoOrigem { get; set; } = "";
}