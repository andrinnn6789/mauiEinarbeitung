using MauiEinarbeitung.ViewModels.Dto;

namespace MauiEinarbeitung.Models;

public class TransactionCompletedEventArgs : EventArgs
{
    public TransactionInformation TransactionInformation { get; }

    public TransactionCompletedEventArgs(TransactionInformation transactionInformation)
    {
        TransactionInformation = transactionInformation;
    }
}