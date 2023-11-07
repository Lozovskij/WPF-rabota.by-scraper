using RabotaScraper.Commands;
using RabotaScraper.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public ICommand OpenLinkCommand { get; set; }

    public MainWindowViewModel()
    {
        ScrapeCommand = new ScrapeCommand(this);
        OpenLinkCommand = new RelayCommand(OpenLink, CanOpenLink);
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
