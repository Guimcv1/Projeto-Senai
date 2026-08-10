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
        if (RoomDialogOverlay?.IsVisible == true || LoginDialogOverlay?.IsVisible == true)
            return;
        
        LoadSalas();
    }

    private void LoadSalas()
    {
        try
        {
            _todasSalas = SalaService.ListarSala();
            if (txtSearch != null)
            {
                txtSearch.ItemsSource = _todasSalas.Select(s => s.Descricao).Where(d => d != null).ToList();
                txtSearch.ItemFilter = (search, item) => 
                {
                    if (string.IsNullOrEmpty(search)) return true;
                    if (item == null) return false;
                    return NormalizeString(item.ToString()).Contains(NormalizeString(search));
                };
            }
            FilterDashboard();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar ambientes: {ex.Message}");
        }
    }

    private string NormalizeString(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark && c != '-')
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToUpper();
    }

    private (string Block, int Number) ParseRoomCode(string? roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName)) return ("", 0);
        
        var match = System.Text.RegularExpressions.Regex.Match(roomName, @"^([A-Za-z]+)[-\s]*(\d+)");
        if (match.Success)
        {
            string block = match.Groups[1].Value.ToUpper();
            int number = int.Parse(match.Groups[2].Value);
            return (block, number);
        }
        
        return (roomName.ToUpper(), 0);
    }

    private void FilterDashboard()
    {
        if (icAmbientes == null || txtSearch == null) return;

        string searchText = NormalizeString(txtSearch.Text);

        var salasFiltradas = _todasSalas.Where(s =>
            (string.IsNullOrEmpty(searchText) ||
             (s.Descricao != null && NormalizeString(s.Descricao).Contains(searchText)))
        )
        .OrderBy(s => ParseRoomCode(s.Descricao).Block)
        .ThenBy(s => ParseRoomCode(s.Descricao).Number)
        .ToList();

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
                Nome = sala.Descricao?.ToUpper() + (!sala.isAtivo ? " (INATIVO)" : ""),
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
        int pending = itensDaSala.Count(i => i.Estado == Estados.Analise || i.Estado == Estados.AnaliseDevolucao);

        if (pending > 0) return "#94a3b8";
        if (available == total) return "#16a34a";
        if (borrowed == total) return "#dc2626";
        if (available > 0) return "#ea580c";

        return "#dc2626";
    }

    private void Search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        FilterDashboard();
    }

    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        if (txtSearch == null) return;

        txtSearch.Text = "";
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
            }
        }
    }

    private void OpenRoomDetails(int salaId, string salaName)
    {
        if (txtDialogTitle == null || dgItems == null || RoomDialogOverlay == null) return;

        txtDialogTitle.Text = $"Item em: {salaName?.ToUpper()}";

        using var context = new BancoContext();
        var itensDaSala = context.Itens.Where(i => i.SalaId == salaId).ToList();

        var activeLoans = context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.EmprestimoItem)
            .Where(e => e.Estado == Estados.Emprestado || e.Estado == Estados.Analise || e.Estado == Estados.AnaliseDevolucao)
            .ToList();

        var dialogItems = new List<ItemUI>();

        foreach (var item in itensDaSala)
        {
            var activeLoan = activeLoans
                .Where(e => e.EmprestimoItem != null && e.EmprestimoItem.Any(ei => ei.ItemId == item.Id))
                .OrderByDescending(e => e.DataEstado)
                .FirstOrDefault();

            string responsavel = activeLoan?.Usuario?.Nome ?? "-";

            var uiItem = new ItemUI
            {
                Id = item.Id,
                Descricao = item.Descricao?.ToUpper() ?? "",
                EstadoOrigem = item.Estado,
                QuemPegou = responsavel
            };

            if (item.Estado == Estados.Livre)
            {
                uiItem.BadgeText = "Disponível";
                uiItem.BadgeColor = "#16a34a";
                uiItem.CanSelect = true;
            }
            else if (item.Estado == Estados.Analise)
            {
                uiItem.BadgeText = responsavel == "-" ? "Em Análise" : $"Análise ({responsavel})";
                uiItem.BadgeColor = "#94a3b8";
                uiItem.CanSelect = true;
            }
            else if (item.Estado == Estados.AnaliseDevolucao)
            {
                uiItem.BadgeText = responsavel == "-" ? "Devolução em análise" : $"Devolução em análise ({responsavel})";
                uiItem.BadgeColor = "#94a3b8";
                uiItem.CanSelect = true;
            }
            else if (item.Estado == Estados.Emprestado)
            {
                uiItem.BadgeText = responsavel == "-" ? "Emprestado" : $"Emprestado ({responsavel})";
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
    }

    private List<ItemUI> _pendingSelectedItems = new();

    private void ChkSelectAll_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox chk && dgItems != null && dgItems.ItemsSource is List<ItemUI> items)
        {
            bool isChecked = chk.IsChecked == true;
            foreach (var item in items)
            {
                if (item.CanSelect)
                {
                    item.IsSelected = isChecked;
                }
            }
            
            // Reassign to force UI update
            dgItems.ItemsSource = null;
            dgItems.ItemsSource = items;
        }
    }

    private void DevolverTodos_Click(object sender, RoutedEventArgs e)
    {
        if (dgItems == null || RoomDialogOverlay == null || LoginDialogOverlay == null) return;

        var dialogItems = dgItems.ItemsSource as List<ItemUI>;
        if (dialogItems == null || !dialogItems.Any())
        {
            _parent?.ShowMessage("Nenhum item disponível neste ambiente.");
            return;
        }

        var borrowedItems = dialogItems.Where(i => i.EstadoOrigem == Estados.Emprestado || i.EstadoOrigem == Estados.Analise || i.EstadoOrigem == Estados.AnaliseDevolucao).ToList();
        if (!borrowedItems.Any())
        {
            _parent?.ShowMessage("Não há nenhum item emprestado neste ambiente para devolver.");
            return;
        }

        foreach (var item in borrowedItems)
        {
            item.IsSelected = true;
        }

        _pendingSelectedItems = borrowedItems;

        RoomDialogOverlay.IsVisible = false;
        txtRequestLogin.Text = "";
        txtRequestPassword.Text = "";

        if (txtLoginDialogTitle != null) txtLoginDialogTitle.Text = "Identificação para Devolução";
        if (txtLoginDialogSubtitle != null) txtLoginDialogSubtitle.Text = "Por favor, informe suas credenciais para solicitar a devolução de todos os itens do ambiente.";
        if (btnConfirmLoanRequest != null)
        {
            btnConfirmLoanRequest.Content = "Devolver Todos";
            btnConfirmLoanRequest.Background = Brush.Parse("#EA580C");
        }

        LoginDialogOverlay.IsVisible = true;
    }

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

        bool hasLoans = _pendingSelectedItems.Any(i => i.EstadoOrigem == Estados.Livre);
        bool hasReturns = _pendingSelectedItems.Any(i => i.EstadoOrigem == Estados.Emprestado || i.EstadoOrigem == Estados.Analise || i.EstadoOrigem == Estados.AnaliseDevolucao);

        if (hasReturns && !hasLoans)
        {
            if (txtLoginDialogTitle != null) txtLoginDialogTitle.Text = "Identificação para Devolução";
            if (txtLoginDialogSubtitle != null) txtLoginDialogSubtitle.Text = "Por favor, informe suas credenciais para devolver os itens selecionados.";
            if (btnConfirmLoanRequest != null)
            {
                btnConfirmLoanRequest.Content = "Devolver Selecionados";
                btnConfirmLoanRequest.Background = Brush.Parse("#EA580C");
            }
        }
        else
        {
            if (txtLoginDialogTitle != null) txtLoginDialogTitle.Text = "Identificação Necessária";
            if (txtLoginDialogSubtitle != null) txtLoginDialogSubtitle.Text = "Por favor, informe suas credenciais para solicitar os itens.";
            if (btnConfirmLoanRequest != null)
            {
                btnConfirmLoanRequest.Content = "Confirmar Solicitação";
                btnConfirmLoanRequest.Background = Brush.Parse("#16A34A");
            }
        }

        LoginDialogOverlay.IsVisible = true;
    }

    private void CloseLoginDialog_Click(object sender, RoutedEventArgs e)
    {
        if (LoginDialogOverlay != null) LoginDialogOverlay.IsVisible = false;
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
            return;
        }

        int currentUserId = usuario.Id;
        LoginDialogOverlay.IsVisible = false;

        // Determine if we are requesting Loans or Returns
        var itemsToLoan = _pendingSelectedItems.Where(i => i.EstadoOrigem == Estados.Livre).Select(i => i.Id).ToList();
        var itemsToReturn = _pendingSelectedItems.Where(i => i.EstadoOrigem == Estados.Emprestado || i.EstadoOrigem == Estados.Analise || i.EstadoOrigem == Estados.AnaliseDevolucao).ToList();

        if (itemsToLoan.Any())
        {
            using var context = new BancoContext();
            var firstItem = context.Itens.Find(itemsToLoan.First());
            int salaId = firstItem?.SalaId ?? 0;

            if (salaId != 0)
            {
                var (success, message) = EmprestimoService.SolicitarEmprestimoComMensagem(currentUserId, salaId, itemsToLoan);
                _parent?.ShowMessage(message, !success);
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
                    .Where(e => (e.Estado == Estados.Emprestado || e.Estado == Estados.Analise || e.Estado == Estados.AnaliseDevolucao) && e.EmprestimoItem.Any(ei => ei.ItemId == item.Id))
                    .OrderByDescending(e => e.DataEstado)
                    .FirstOrDefault();

                if (activeLoan != null)
                {
                    if (EmprestimoService.SolicitarDevolucao(activeLoan.Id))
                    {
                        returnCount++;
                    }
                }
            }
            if (returnCount > 0)
            {
                _parent?.ShowMessage($"Solicitada devolução de {returnCount} item(ns).", false);
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
    public string QuemPegou { get; set; } = "-";
}