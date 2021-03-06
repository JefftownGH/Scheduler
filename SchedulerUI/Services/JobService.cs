﻿using AutoMapper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using SchedulerAPI.Dtos;
using SchedulerAPI.Models;
using SchedulerUI.Dtos;
using SchedulerUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SchedulerUI.Services
{
    public class JobService : IJobService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _storageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public JobService(HttpClient httpClient,
            ILocalStorageService storageService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _storageService = storageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<PaginationResponseDto<Job>> GetJobs(int recordsPerPage = 10, int pageNumber = 1)
        {
            // Awaits response from database
            var response = await _httpClient.GetAsync($"Jobs/GetJobs?quantityPerPage={recordsPerPage}&page={pageNumber}");

            // If successful
            if (response.IsSuccessStatusCode)
            {
                var totalPageQuantity = int.Parse(response.Headers.GetValues("pagesQuantity").FirstOrDefault());
                var jobs = JsonConvert.DeserializeObject<List<Job>>(response.Content.ReadAsStringAsync().Result);

                // Creates dto from response
                var paginationResponse = new PaginationResponseDto<Job>()
                {
                    TotalPagesQuantity = totalPageQuantity,
                    Items = jobs
                };

                // Returns dto
                return paginationResponse;
            }
            else
            {
                return null;
            }
        }

        public async Task<Job> GetJob(int id)
        {
            // Attempts to get project data from database
            var response = await _httpClient.GetAsync("jobs/" + id);

            // If response is successful
            if (response.IsSuccessStatusCode)
            {
                var job = JsonConvert.DeserializeObject<Job>(response.Content.ReadAsStringAsync().Result);
                return job;
            }
            else
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> UpdateJob(JobDto job)
        {
            var json = JsonConvert.SerializeObject(job);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("jobs/" + job.Id, data);

            return response;
        }

        public async Task<HttpResponseMessage> AddJob(JobDto job)
        {
            // Converts user object to json for http post
            var json = JsonConvert.SerializeObject(job);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Calls web api job post
            var response = await _httpClient.PostAsync("jobs/", data);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteJob(int jobId)
        {
            // Attempts to delete job from database
            var response = await _httpClient.DeleteAsync($"jobs/{jobId}");

            // Returns response
            return response;
        }
    }
}