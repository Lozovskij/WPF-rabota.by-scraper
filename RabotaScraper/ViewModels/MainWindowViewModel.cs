using RabotaScraper.Commands;
using RabotaScraper.Models.Services;
using System;
using System.Windows.Input;

namespace RabotaScraper.ViewModels;

public class MainWindowViewModel
{
    public ICommand ScrapeCommand { get; set; }

    public MainWindowViewModel()
    {
        ScrapeCommand = new RelayCommand(Scrape, CanScrape);
    }

    private bool CanScrape(object obj)
    {
        return true;
    }

    private void Scrape(object obj)
    {
        Scraper.Scrape();
    }
}
