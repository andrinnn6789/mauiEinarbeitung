using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiEinarbeitung.Models;
using MauiEinarbeitung.ViewModels.Dto;

namespace MauiEinarbeitung.ViewModels;

public class PointOfSaleViewModel : INotifyPropertyChanged
{
    private readonly TimService _timService;
    
    public PointOfSaleViewModel()
    {
        _timService = new TimService("25291126");

        ConnectWithTerminalCommand = new Command(ConnectWithTerminalCommandExecute, ConnectWithTerminalCommandCanExecute);
        DisConnectWithTerminalCommand = new Command(DisConnectWithTerminalCommandExecute, ConnectWithTerminalCommandCanExecute);
        ActivateTerminalCommand = new Command(ActivateTerminalCommandExecute, ConnectWithTerminalCommandCanExecute);
        DeactivateTerminalCommand = new Command(DeactivateTerminalCommandExecute, ConnectWithTerminalCommandCanExecute);
        BalanceCommand = new Command(FinalBalanceCommandExecute, ConnectWithTerminalCommandCanExecute);
        CreatePurchaseTransactionCommand = new Command(CreatePurchaseTransactionCommandExecute, ConnectWithTerminalCommandCanExecute);
        GetBrandsCommand = new Command(GetBrandsCommandExecute, ConnectWithTerminalCommandCanExecute);
        
        _timService.TerminalStatusChangedEvent += UpdateTerminalProperties;
        _timService.ApplicationInformationChangedEvent += UpdateBrandProperties;
        _timService.TransactionCompletedEvent += UpdateTransactionInformationProperties;
        _timService.BalanceCompletedEvent += UpdateBalanceProperties;
    }

    private bool ConnectWithTerminalCommandCanExecute()
    {
        return true;
    }

    private void ConnectWithTerminalCommandExecute()
    {
        _timService.Connect();
    }
    
    private void DisConnectWithTerminalCommandExecute()
    {
        _timService.Disconnect();
    }

    private void ActivateTerminalCommandExecute()
    {
        _timService.ActivateAsync();
    }
    
    private void DeactivateTerminalCommandExecute()
    {
        _timService.DeactivateAsync();
    }
    
    private void FinalBalanceCommandExecute()
    {
        _timService.BalanceAsync();
    }
    
    private void CreatePurchaseTransactionCommandExecute()
    {
        _timService.CreatePurchase(214, "CHF");
    }    

    private void GetBrandsCommandExecute()
    {
        _timService.FetchApplicationInformationAsync();
    }

    private void UpdateTerminalProperties(object? sender, TerminalStatusChangedEventArgs terminalStatus)
    {
        ConnectionStatus = terminalStatus.Status.ConnectionStatus;
        ManagementStatus = terminalStatus.Status.ManagementStatus;
        TransactionStatus = terminalStatus.Status.TransactionStatus;
        SwUpdateAvailable = terminalStatus.Status.SwUpdateAvailable;
        DisplayContent = terminalStatus.Status.DisplayContent!.Aggregate(string.Empty, (current, content) => current + (content + Environment.NewLine));
        CardReaderStatus = terminalStatus.Status.CardReaderStatus;
    }
    
    private void UpdateBrandProperties(object? sender, ApplicationInformationChangedEventArgs applicationInformation)
    {
        Brands = applicationInformation.Brands;
    }

    private void UpdateTransactionInformationProperties(object? sender, TransactionCompletedEventArgs transactionInformation)
    {
        TransactionInformation = transactionInformation.TransactionInformation;
    }
    
    private void UpdateBalanceProperties(object? sender, BalanceCompletedEventArgs balanceInformation)
    {
        BalanceInformation = balanceInformation.BalanceInformation;
    }

    private string? _connectionStatus;
    private string? _managementStatus;
    private string? _transactionStatus;
    private bool? _swUpdateAvailable;
    private string? _displayContent;
    private string? _cardReaderStatus;
    private IList<Brand> _brands;
    private Brand? _selectedBrand;
    private TransactionInformation? _transactionInformation;
    private BalanceInformation? _balanceInformation;

    public string? ConnectionStatus
    {
        get => _connectionStatus;
        private set => SetField(ref _connectionStatus, value);
    }

    public string? ManagementStatus
    {
        get => _managementStatus;
        private set => SetField(ref _managementStatus, value);
    }

    public string? TransactionStatus
    {
        get => _transactionStatus;
        private set => SetField(ref _transactionStatus, value);
    }

    public bool? SwUpdateAvailable
    {
        get => _swUpdateAvailable;
        private set => SetField(ref _swUpdateAvailable, value);
    }
    
    public string? DisplayContent
    {
        get => _displayContent;
        private set => SetField(ref _displayContent!, value);
    }

    public string? CardReaderStatus
    {
        get => _cardReaderStatus;
        private set => SetField(ref _cardReaderStatus, value);
    }

    public IList<Brand> Brands
    {
        get => _brands;
        private set => SetField(ref _brands, value);
    }
    
    public Brand? SelectedBrand
    {
        get => _selectedBrand;
        set => SetField(ref _selectedBrand, value);
    }
    
    public TransactionInformation? TransactionInformation
    {
        get => _transactionInformation;
        set => SetField(ref _transactionInformation, value);
    }
    
    public BalanceInformation? BalanceInformation
    {
        get => _balanceInformation;
        set => SetField(ref _balanceInformation, value);
    }
    
    public Command ConnectWithTerminalCommand { get; }
    public Command DisConnectWithTerminalCommand { get; }   
    public Command ActivateTerminalCommand { get; }    
    public Command DeactivateTerminalCommand { get; }   
    public Command BalanceCommand { get; }
    public Command CreatePurchaseTransactionCommand { get; }
    public Command GetBrandsCommand { get; }

    #region Mvvm Properties and Functions

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    #endregion
    
}