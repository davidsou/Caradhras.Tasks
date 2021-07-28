using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caradhras.Tasks.Domain.Entities;
using Caradhras.Tasks.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caradhras.Tasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ISFTPService _sftpService;

        public TaskController(ISFTPService sftpService)
        {
            _sftpService = sftpService;
        }

        [HttpPost]
        [Route("RequestFile")]
        public async Task<IActionResult> RequestData(Filter filter)
        {

            var result = await _sftpService.RequestFile(filter);

            if (!result.Sucess)
                return BadRequest(new { success = result.Sucess, message = string.Join(';', result.Messages) });

            return Ok(result.Data);


        }

        [HttpGet]
        [Route("CheckStatusRequest")]
        public async Task<IActionResult> CheckStatusRequest(string ticket)
        {

            var result = await _sftpService.CheckRequest(ticket);

            if (!result.Sucess)
                return BadRequest(new { success = result.Sucess, message = string.Join(';', result.Messages) });

            return Ok(result.Data);
        }


        [HttpPost]
        [Route("DownloadFile")]
        public async Task<IActionResult> Download(string ticket)
        {

            var result = await _sftpService.DownloadFile(ticket);

            if (!result.Sucess)
                return BadRequest(new { success = result.Sucess, message = string.Join(';', result.Messages) });

            return Ok(result.Data);


        }


        [HttpPost]
        [Route("ManualUpdate")]
        public async Task<IActionResult> ManualUpdate(string ticket)
        {

            var result = await _sftpService.ManualUpdate(ticket);

            if (!result.Sucess)
                return BadRequest(new { success = result.Sucess, message = string.Join(';', result.Messages) });

            return Ok(result.Data);


        }

    }
}