using Azure.Messaging.ServiceBus;

namespace ExTest
{
	internal class ServiceBusReader : IAsyncDisposable
	{
		private readonly ServiceBusClient _serviceBusClient;
		private readonly ServiceBusSessionProcessor _serviceBusSessionProcessor;
		private bool m_disposed = false;

		public ServiceBusReader(string connectionString, string queueName)
		{
			_serviceBusClient = new ServiceBusClient(connectionString);
			_serviceBusSessionProcessor = _serviceBusClient.CreateSessionProcessor(queueName, new ServiceBusSessionProcessorOptions() { MaxConcurrentSessions = 1, MaxConcurrentCallsPerSession = 3, AutoCompleteMessages = false });
			_serviceBusSessionProcessor.ProcessMessageAsync += ProcessMessageAsync;
			_serviceBusSessionProcessor.ProcessErrorAsync += ProcessErrorAsync;
			_serviceBusSessionProcessor.SessionInitializingAsync += SessionInitializingAsync;
			_serviceBusSessionProcessor.SessionClosingAsync += SessionClosingAsync;
		}

		public async ValueTask DisposeAsync()
		{
			if (m_disposed)
			{
				return;
			}

			await _serviceBusSessionProcessor.StopProcessingAsync(CancellationToken.None);
			await _serviceBusSessionProcessor.DisposeAsync();
			await _serviceBusClient.DisposeAsync();

			m_disposed = true;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _serviceBusSessionProcessor.StartProcessingAsync(cancellationToken);
		}

		private async Task ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
		{
			await Task.Delay(TimeSpan.FromSeconds(2));
			arg.ReleaseSession();
			await arg.CompleteMessageAsync(arg.Message);
		}

		private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
			Console.WriteLine(arg.Exception);
			return Task.CompletedTask;
		}

		private Task SessionInitializingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"SessionInitializingAsync({arg.SessionId})");
			return Task.CompletedTask;
		}
		private Task SessionClosingAsync(ProcessSessionEventArgs arg)
		{
			Console.WriteLine($"SessionClosingAsync({arg.SessionId})");
			return Task.CompletedTask;
		}
	}
}
