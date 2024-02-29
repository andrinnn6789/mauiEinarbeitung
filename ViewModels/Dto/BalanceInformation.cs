namespace MauiEinarbeitung.ViewModels.Dto;

public class BalanceInformation
{
    public string? Receipt { get; set; }
    public string? Currency { get; set; }
    public List<Counters>? Counters { get; set; }
    public double? TotalAmountSum { get; set; }
}