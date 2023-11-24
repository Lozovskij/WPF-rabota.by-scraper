using RabotaScraper.Models;
using System;
using System.Collections.Generic;

namespace RabotaScraper.Stores;

public class JobStore
{
    public event Action<IEnumerable<Job>>? JobsScraped;

    public void InvokeJobsScraped(IEnumerable<Job> jobs)
    {
        JobsScraped?.Invoke(jobs);
    }
}
