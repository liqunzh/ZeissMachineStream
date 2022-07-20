using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeissMachineStream.Helper;
using ZeissMachineStream.Models;

namespace ZeissMachineStream
{
    public class MachineStatusListeningService : IHostedService, IDisposable
    {
        private WebSocketsHelper _helper;
        private readonly ILogger<MachineStatusListeningService> _logger;
        private String _remoteAddress;

        public MachineStatusListeningService(ILogger<MachineStatusListeningService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _remoteAddress = configuration.GetValue<String>("MachineStreamAddress");
        }
        public void Dispose()
        {
            if (_helper != null)
                _helper.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Machine status listening service is running.");
            _logger.LogInformation("Try to connect {addr}", _remoteAddress);

            _helper = new WebSocketsHelper(_remoteAddress);
            _helper.OnMessage += OnMachineStatusUpdated;
            _helper.Open();

            _logger.LogInformation("Connected.");

            return Task.CompletedTask;
        }

        private void OnMachineStatusUpdated(object sender, string data)
        {
            _logger.LogInformation("Receive message: {data}", data);

            StatusData status = JsonConvert.DeserializeObject<StatusData>(data);

            StatusDataRepository.AddStatusData(status);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Machine status listening service is stopping.");

            return Task.CompletedTask;
        }
    }
}
