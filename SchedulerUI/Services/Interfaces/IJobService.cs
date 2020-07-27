﻿using SchedulerAPI.Dtos;
using SchedulerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchedulerUI.Services.Interfaces
{
    /// <summary>
    /// Service for handling communication with api in regards to jobs
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// Gets list of jobs from database
        /// </summary>
        /// <returns></returns>
        Task<List<Job>> GetJobs();

        /// <summary>
        /// Gets job with given id from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Job> GetJob(int id);

        /// <summary>
        /// Updates given job in the database
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> UpdateJob(JobDto job);
    }
}
