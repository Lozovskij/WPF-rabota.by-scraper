using RabotaScraper.Commands;
using RabotaScraper.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace RabotaScraper.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Job> _jobs;
    private string _selectedScrapeOption;
    private string _scrapeStatusMessage;

    public string ScrapeStatusMessage
    {
        get { return _scrapeStatusMessage; }
        set
        {
            _scrapeStatusMessage = value;
            OnPropertyChanged(nameof(ScrapeStatusMessage));
        }
    }

    public ObservableCollection<Job> Jobs
    {
        get => _jobs;
        set
        {
            _jobs = value;
            OnPropertyChanged(nameof(Jobs));
        }
    }

    public ObservableCollection<string> ScrapeOptions { get; set; }

    public string SelectedScrapeOption
    {
        get { return _selectedScrapeOption; }
        set
        {
            _selectedScrapeOption = value;
            OnPropertyChanged(nameof(SelectedScrapeOption));
        }
    }

    public ICommand ScrapeCommand { get; set; }
    public ICommand OpenLinkCommand { get; set; }

    public MainWindowViewModel()
    {
        ScrapeCommand = new ScrapeCommand(this);
        OpenLinkCommand = new RelayCommand(OpenLink, CanOpenLink);
        ScrapeOptions = new ObservableCollection<string>(Commands.ScrapeCommand.UrlMaps.Keys.ToArray());
        SelectedScrapeOption = Commands.ScrapeCommand.UrlMaps.Keys.First();
    }

    private bool CanOpenLink(object obj)
    {
        return true;
    }

    private void OpenLink(object obj)
    {
        var url = (string)obj;
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
