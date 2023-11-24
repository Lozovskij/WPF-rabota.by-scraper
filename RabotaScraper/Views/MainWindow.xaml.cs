using RabotaScraper.Stores;
using RabotaScraper.ViewModels;
using System.Windows;

namespace RabotaScraper;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var jobStore = new JobStore();
        var mainWindowViewModel = new MainWindowViewModel(jobStore);
        this.DataContext = mainWindowViewModel;
    }
}
