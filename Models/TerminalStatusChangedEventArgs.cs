using MauiEinarbeitung.ViewModels.Dto;

namespace MauiEinarbeitung.Models;

public class TerminalStatusChangedEventArgs : EventArgs
{
    public TerminalStatus Status { get; }

    public TerminalStatusChangedEventArgs(TerminalStatus status)
    {
        Status = status;
    }
}