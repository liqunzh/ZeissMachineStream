using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ZeissMachineStream
{
    public class MachineStatusListeningService : IHostedService, IDisposable
    {
        private WebSocketsHelper _helper;
        private readonly ILogger<MachineStatusListeningService> _logger;

        public MachineStatusListeningService(ILogger<MachineStatusListeningService> logger)
        {
            _logger = logger;
        }
        public void Dispose()
        {
            if (_helper != null)
                _helper.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Machine status listening service is running.");

            _helper = new WebSocketsHelper("ws://machinestream.herokuapp.com/ws");
            _helper.OnMessage += OnMachineStatusUpdated;
            _helper.Open();

            return Task.CompletedTask;
        }

        private void OnMachineStatusUpdated(object sender, string data)
        {
            _logger.LogInformation("Receive message: {data}", data);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Machine status listening service is stopping.");

            return Task.CompletedTask;
        }
    }
}
