using Azure.Messaging.ServiceBus;

namespace ExTest
{
	internal class ServiceBusWriter : IAsyncDisposable
	{
		private const int SessionCount = 10;
		private const int MessageCount = 100;

		private readonly ServiceBusClient _serviceBusClient;
		private readonly ServiceBusSender _serviceBusSender;
		private bool m_disposed = false;

		public ServiceBusWriter(string connectionString, string queueName)
		{
			_serviceBusClient = new ServiceBusClient(connectionString);
			_serviceBusSender = _serviceBusClient.CreateSender(queueName);
		}

		public async ValueTask DisposeAsync()
		{
			if (m_disposed)
			{
				return;
			}

			await _serviceBusSender.DisposeAsync();
			await _serviceBusClient.DisposeAsync();

			m_disposed = true;
		}

		public async Task WriteAsync(CancellationToken cancellationToken)
		{
			using ServiceBusMessageBatch batch = await _serviceBusSender.CreateMessageBatchAsync(cancellationToken);
			for (int i = 0; i < MessageCount; i++)
			{
				ServiceBusMessage newMessage = new () { SessionId = Random.Shared.Next(SessionCount).ToString() };
				batch.TryAddMessage(newMessage);
			}
			await _serviceBusSender.SendMessagesAsync(batch, cancellationToken);
		}
	}
}
