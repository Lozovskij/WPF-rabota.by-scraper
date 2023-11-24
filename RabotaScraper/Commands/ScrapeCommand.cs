using HtmlAgilityPack;
using RabotaScraper.Models;
using RabotaScraper.Stores;
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
        { "c#, Homel", @"https://rabota.by/search/vacancy?area=1003&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&text=c%23" },
        { "c#, Minsk", @"https://rabota.by/search/vacancy?area=1002&ored_clusters=true&order_by=publication_time&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&text=c%23" },
        { "angular, Homel", @"https://rabota.by/search/vacancy?ored_clusters=true&order_by=publication_time&area=1003&hhtmFrom=vacancy_search_list&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&text=angular" },
        { "angular, Minsk", @"https://rabota.by/search/vacancy?ored_clusters=true&order_by=publication_time&area=1002&hhtmFrom=vacancy_search_list&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true&text=angular" },
        { "IT, no experience", @"https://rabota.by/search/vacancy?area=16&ored_clusters=true&order_by=publication_time&disableBrowserCache=true&hhtmFrom=vacancy_search_list&experience=noExperience&professional_role=96&search_field=name&search_field=company_name&search_field=description&enable_snippets=false&L_save_area=true" },
    };

    private MainWindowViewModel _mainWindowViewModel;
    private readonly JobStore _jobStore;

    public ScrapeCommand(MainWindowViewModel mainWindowViewModel, JobStore jobStore)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _jobStore = jobStore;
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
            _mainWindowViewModel.ScrapeStatusMessage = "Loading...";
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
                    _jobStore.InvokeJobsScraped(jobs);
                    return;
                }

                var excludeStr = "Опыт ";
                foreach (var node in jobsNodes)
                {
                    string title = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").InnerText.Trim();
                    string company = node.SelectSingleNode(".//a[contains(@class,'bloko-link bloko-link_kind-tertiary')]").InnerText.Trim();
                    string link = node.SelectSingleNode(".//a[contains(@class,'serp-item__title')]").GetAttributeValue("href", "");
                    string city = node.SelectSingleNode(".//div[@data-qa='vacancy-serp__vacancy-address']").InnerText.Trim();
                    string experience = node.SelectSingleNode(".//div[@data-qa='vacancy-serp__vacancy-work-experience']").InnerText.Trim();

                    experience = experience.Contains(excludeStr) ? experience.Replace(excludeStr, "") : experience;
                    city = city.Split(new char[] { ',', ' ' })[0];
                    
                    jobs.Add(new(title, company, city, experience, link));
                }
                _mainWindowViewModel.ScrapeStatusMessage = "Success!";
                _jobStore.InvokeJobsScraped(jobs);
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
