using HtmlAgilityPack;
using RabotaScraper.Models;
using RabotaScraper.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RabotaScraper.Commands;

public class ScrapeCommand : ICommand
{
    private bool _isExecuting;

    public bool IsExecuting
    {
        get
        {
            return _isExecuting;
        }
        set
        {
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public event EventHandler? CanExecuteChanged;

    public static Dictionary<string, string> UrlMaps { get; set; } = new Dictionary<string, string>()
    {
        { "c#, 1-3 years, Homel" , @"https://rabota.by/search/vacancy?area=1003&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&experience=between1And3&text=c%23&L_save_area=true" },
        { "c#, 1-3 years, Homel/Minsk", @"https://rabota.by/search/vacancy?area=1003&area=1002&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&experience=between1And3&text=c%23"},
        { "angular, 0 years, Homel", @"https://rabota.by/search/vacancy?ored_clusters=true&order_by=publication_time&area=1003&hhtmFrom=vacancy_search_list&experience=noExperience&professional_role=96&search_field=name&search_field=company_name&search_field=description&text=angular&enable_snippets=false&L_save_area=true" },
        { "angular .net, 1-3 years, Homel/Minsk", @"https://rabota.by/search/vacancy?ored_clusters=true&order_by=publication_time&area=1002&area=1003&hhtmFrom=vacancy_search_list&experience=between1And3&professional_role=96&search_field=name&search_field=company_name&search_field=description&text=angular+.net&enable_snippets=false"},
        { "angular .net, 0 years, Homel/Minsk", @"https://rabota.by/search/vacancy?area=1002&area=1003&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&experience=noExperience&professional_role=96&text=angular+.net" },
    };

    private MainWindowViewModel _mainWindowViewModel;

    public ScrapeCommand(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public bool CanExecute(object? parameter)
    {
        return !IsExecuting;
    }

    public async void Execute(object? parameter)
    {
        IsExecuting = true;
        try
        {
            await Task.Run(() =>
            {
                if (parameter == null) return;
                var url = UrlMaps[(string)parameter];

                var web = new HtmlWeb();
                var doc = web.Load(url);

                var jobsNodes = doc.DocumentNode.SelectNodes("//div[@class='serp-item']");

                var jobs = new List<Job>();

                if (jobsNodes == null)
                {
                    _mainWindowViewModel.ScrapeStatusMessage = "No jobs found";
                    _mainWindowViewModel.Jobs = new ObservableCollection<Job>(jobs);
                    return;
                }

                foreach (var node in jobsNodes)
                {
                    string title = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").InnerText.Trim();
                    string company = node.SelectSingleNode(".//a[contains(@class,'bloko-link bloko-link_kind-tertiary')]").InnerText.Trim();
                    string link = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").GetAttributeValue("href", "");
                    string city = node.SelectSingleNode(".//div[@data-qa='vacancy-serp__vacancy-address']").InnerText.Trim();
                    jobs.Add(new(title, company, city, link));
                }
                _mainWindowViewModel.ScrapeStatusMessage = "Success!";
                _mainWindowViewModel.Jobs = new ObservableCollection<Job>(jobs);
            });
        }
        catch (Exception ex)
        {
            _mainWindowViewModel.ScrapeStatusMessage = ex.Message;
        }
        finally
        {
            IsExecuting = false;
        }
    }
}
