﻿
using System.Collections.Generic;
using System.Threading.Tasks;
using Events;
using Jobs.Api.Models;
using Jobs.Api.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jobs.api.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly IBus _bus;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobRepository jobRepository, IBus bus, ILogger<JobsController> logger)
        {
            _jobRepository = jobRepository;
            _bus = bus;
            this._logger = logger;

        }

        // GET api/jobs
        [HttpGet]
        public async Task<IEnumerable<Job>> Get()
        {
            this._logger.LogInformation("start action api/jobs");
            var jobs = await _jobRepository.GetAll();
            this._logger.LogInformation("stop action api/jobs");
            return jobs;
        }

        // GET api/jobs/5
        [HttpGet("{id}")]
        public async Task<Job> Get(int id)
        {
            return await _jobRepository.Get(id);
        }

        // POST api/values
        [HttpPost("/api/jobs/applicants")]
        public async Task<IActionResult> Post([FromBody]JobApplicant model)
        {
            // fetch the job data
            var job = await _jobRepository.Get(model.JobId);
            var id = await _jobRepository.AddApplicant(model);
            //var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://rabbitmq/dotnetgigs"));  //?bind=true&queue=dotnetgigs
            //await endpoint.Send<ApplicantAppliedEvent>(new  { model.JobId,model.ApplicantId,job.Title});
            await _bus.Publish<ApplicantAppliedEvent>(new { model.JobId, model.ApplicantId, job.Title });
            return Ok(id);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
