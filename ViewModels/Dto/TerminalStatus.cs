namespace MauiEinarbeitung.ViewModels.Dto;

public class TerminalStatus
{
    public string? ConnectionStatus { get; set; }
    public string? ManagementStatus { get; set; }
    public string? TransactionStatus { get; set; }
    public bool? SwUpdateAvailable { get; set; }
    public List<string>? DisplayContent { get; set; }
    public string? CardReaderStatus { get; set; }
}