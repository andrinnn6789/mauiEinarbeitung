using MauiEinarbeitung.ViewModels.Dto;

namespace MauiEinarbeitung.Models;

public class BalanceCompletedEventArgs : EventArgs
{
    public BalanceInformation BalanceInformation { get; }

    public BalanceCompletedEventArgs(BalanceInformation balanceInformation)
    {
        BalanceInformation = balanceInformation;
    }
}