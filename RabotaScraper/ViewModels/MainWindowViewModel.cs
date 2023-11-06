using RabotaScraper.Commands;
using RabotaScraper.Models;
using RabotaScraper.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RabotaScraper.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Job> _jobs;

    public ObservableCollection<Job> Jobs
    {
        get => _jobs;
        set
        {
            _jobs = value;
            OnPropertyChanged(nameof(Jobs));
        }
    }

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
        Jobs = Scraper.GetJobsWithLinks();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
