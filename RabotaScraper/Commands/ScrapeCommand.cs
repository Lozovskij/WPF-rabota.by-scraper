using HtmlAgilityPack;
using RabotaScraper.Models;
using RabotaScraper.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RabotaScraper.Commands;

public class ScrapeCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public static Dictionary<string, string> UrlMaps { get; set; } = new Dictionary<string, string>()
    {
        { "c#, 1-3 years, Homel" , @"https://rabota.by/search/vacancy?area=1003&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&experience=between1And3&text=c%23&L_save_area=true" },
        { "wpf, 1-3 years, Minsk", @"https://rabota.by/search/vacancy?text=wpf&salary=&ored_clusters=true&order_by=publication_time&experience=between1And3&area=1002&hhtmFrom=vacancy_search_list" }
    };

    private MainWindowViewModel _mainWindowViewModel;

    public ScrapeCommand(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter == null) return;
        var url = UrlMaps[(string)parameter];

        var web = new HtmlWeb();
        var doc = web.Load(url);

        var jobsNodes = doc.DocumentNode.SelectNodes("//div[@class='serp-item']");

        var jobs = new List<Job>();

        foreach (var node in jobsNodes)
        {
            string title = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").InnerText.Trim();
            string company = node.SelectSingleNode(".//a[contains(@class,'bloko-link bloko-link_kind-tertiary')]").InnerText.Trim();
            string link = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").GetAttributeValue("href", "");
            jobs.Add(new(title, company, link));
        }
        _mainWindowViewModel.Jobs = new ObservableCollection<Job>(jobs);
    }
}
