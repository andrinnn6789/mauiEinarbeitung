using MauiEinarbeitung.ViewModels.Dto;
using SIX.TimApi;
using SIX.TimApi.Constants;
using Brand = MauiEinarbeitung.ViewModels.Dto.Brand;
using TerminalStatus = MauiEinarbeitung.ViewModels.Dto.TerminalStatus;
using TransactionInformation = MauiEinarbeitung.ViewModels.Dto.TransactionInformation;

namespace MauiEinarbeitung.Models;

public class TimService
{
    private Terminal _terminal;

    public TimService(string terminalId)
    {
        var settings = new TerminalSettings();
        settings.TerminalId = terminalId;
        _terminal = new Terminal(settings);
        _terminal.TerminalStatusChanged += TerminalStatusChanged;
        _terminal.ActivateCompleted += ActivateCompleted;
        _terminal.DeactivateCompleted += DeactivateCompleted;
        _terminal.BalanceCompleted += BalanceCompleted;
        _terminal.TransactionCompleted += TransactionCompleted;
        _terminal.ApplicationInformationCompleted += ApplicationInformationCompleted;
    }

    #region Public Methodes
    
    public void Connect()
    {
        _terminal.Connect();
    }

    public void Disconnect()
    {
        _terminal.Disconnect();
    }

    public void ActivateAsync()
    {
        _terminal.ActivateAsync();
    }
    
    public void DeactivateAsync()
    {
        _terminal.DeactivateAsync();
    }
    
    public void BalanceAsync()
    {
        _terminal.BalanceAsync();
    }

    public void CreatePurchase(decimal amount, string currency)
    {
        CreateTransaction(TransactionType.Purchase, amount, currency);
    }

    public void CreateCredit(decimal amount, string currency)
    {
        CreateTransaction(TransactionType.Credit, amount, currency);
    }
    
    public void CreateReversal(decimal amount, string currency)
    {
        CreateTransaction(TransactionType.Reversal, amount, currency);
    }

    public void FetchApplicationInformationAsync()
    {
        _terminal.ApplicationInformationAsync();
    }
    
    private void CreateTransaction(TransactionType transactionType, decimal amount, string currency)
    {
        _terminal.TransactionAsync(transactionType, new Amount(amount, currency, 0));
    }
    
    #endregion

    #region Events

    public event EventHandler<TerminalStatusChangedEventArgs> TerminalStatusChangedEvent;

    public event EventHandler<ApplicationInformationChangedEventArgs> ApplicationInformationChangedEvent;
    
    public event EventHandler<TransactionCompletedEventArgs> TransactionCompletedEvent;
    
    public event EventHandler<BalanceCompletedEventArgs> BalanceCompletedEvent;
    
    private void TerminalStatusChanged(object sender, SIX.TimApi.TerminalStatus status) 
    {
        ConnectionStatus = status.ConnectionStatus;
        TransactionStatus = status.TransactionStatus;
        ManagementStatus = status.ManagementStatus;
        CardReaderStatus = status.CardReaderStatus;
        DisplayContent = status.DisplayContent;
        SwUpdateAvailable = status.SwUpdateAvailable;

        var terminalStatus = new TerminalStatus
        {
            ConnectionStatus = ConnectionStatus.ToString(),
            ManagementStatus = ManagementStatus.ToString(),
            TransactionStatus = TransactionStatus.ToString(),
            SwUpdateAvailable = SwUpdateAvailable,
            DisplayContent = DisplayContent,
            CardReaderStatus = CardReaderStatus.ToString(),
        };
        
        TerminalStatusChangedEvent?.Invoke(this, new TerminalStatusChangedEventArgs(terminalStatus));
    }

    private void ApplicationInformationCompleted(object sender, Terminal.ApplicationInformationCompletedEventArgs applicationInformationEventArgs)
    {
        var brands = _terminal.Brands.Select(terminalBrand => new Brand
        {
            Name = terminalBrand.BrandName,
            SupportedAids = terminalBrand.Applications.Select(application => application.Aid).ToList(),
            SupportedCurrencies = terminalBrand.Currencies.Select(currency => currency.Currency).ToList()
        }).ToList();
        
        ApplicationInformationChangedEvent?.Invoke(this, new ApplicationInformationChangedEventArgs(brands));
    }
    
    private void DeactivateCompleted(object sender, Terminal.DeactivateCompletedEventArgs deactivateCompletedEventArgs)
    {
        var x = deactivateCompletedEventArgs;
    }

    private void ActivateCompleted(object sender, Terminal.ActivateCompletedEventArgs activateCompletedEventArgs)
    {
        var x = activateCompletedEventArgs;
    }
    
    private void BalanceCompleted(object sender, Terminal.BalanceCompletedEventArgs balanceCompletedEventArgs)
    {
        var balanceInformation = new BalanceInformation()
        {
            Receipt = balanceCompletedEventArgs.BalanceResponse.PrintData.MerchantReceipt,
            Currency = balanceCompletedEventArgs.BalanceResponse.Counters.Currency,
            Counters = balanceCompletedEventArgs.BalanceResponse.Counters.Counters.Select(c => new Counters
            {
                Brand = c.BrandName,
                TransactionCount = c.Count,
                AmountSum = AmountSumValue(c)
            }).ToList(),
            TotalAmountSum = balanceCompletedEventArgs.BalanceResponse.Counters.Counters.Sum(AmountSumValue)
        };
        
        BalanceCompletedEvent?.Invoke(this, new BalanceCompletedEventArgs(balanceInformation));
    }

    private static double AmountSumValue(Counter c)
    {
        return c.Totals.First().AmountSum.Value / Math.Pow(10, c.Totals.First().AmountSum.Exponent);
    }

    private void TransactionCompleted(object sender, Terminal.TransactionCompletedEventArgs transactionCompletedEventArgs)
    {
        var transactionInformation = new TransactionInformation()
        {
            CardholderReceipt = transactionCompletedEventArgs.TransactionResponse.PrintData.CardholderReceipt,
            MerchantReceipt = transactionCompletedEventArgs.TransactionResponse.PrintData.MerchantReceipt,
            BrandName = transactionCompletedEventArgs.TransactionResponse.CardData.BrandName,
            Aid = transactionCompletedEventArgs.TransactionResponse.CardData.Aid,
            CardExpiryDate = transactionCompletedEventArgs.TransactionResponse.CardData.CardExpiryDate.ToString(),
            CardNumberPrintable = transactionCompletedEventArgs.TransactionResponse.CardData.CardNumberPrintable
        };
        
        TransactionCompletedEvent.Invoke(this, new TransactionCompletedEventArgs(transactionInformation));
    }
    
    #endregion
    
    #region Properties
    
    private ConnectionStatus ConnectionStatus { get; set; }
    private ManagementStatus ManagementStatus { get; set; }
    private TransactionStatus TransactionStatus { get; set; }
    private bool SwUpdateAvailable { get; set; }
    private List<string> DisplayContent { get; set; }
    private CardReaderStatus CardReaderStatus { get; set; }
    
    #endregion
}