using MauiEinarbeitung.ViewModels.Dto;

namespace MauiEinarbeitung.Models;

public class ApplicationInformationChangedEventArgs : EventArgs
{
    public List<Brand> Brands { get; }

    public ApplicationInformationChangedEventArgs(List<Brand> brands)
    {
        Brands = brands;
    }
}