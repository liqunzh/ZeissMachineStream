using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeissMachineStream.Exceptions;
using ZeissMachineStream.Models;

namespace ZeissMachineStream.Controllers
{
    [ApiController]
    [Route("api/MachineStreamStatus")]
    public class MachineStatusController : ControllerBase
    {
        private readonly ILogger<MachineStatusController> _logger;

        public MachineStatusController(ILogger<MachineStatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<StatusSummary> GetSummary(string status)
        {
            return StatusDataRepository.GetSummary(status);
        }

        [HttpGet("{machineId}")]
        public IEnumerable<StatusDataDetail> GetMachineStatusDetail(string machineId, int count)
        {
            var result = StatusDataRepository.GetMachineDetail(machineId, count);
            if (result == null)
                throw new ResourceNotFoundException("Cannot find specified machine id");
            return result;
        }
    }
}
