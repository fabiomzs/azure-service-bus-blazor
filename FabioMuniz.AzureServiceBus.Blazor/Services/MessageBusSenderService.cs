using Azure.Messaging.ServiceBus;
using FabioMuniz.AzureServiceBus.Blazor.Models;
using System.Text.Json;

namespace FabioMuniz.AzureServiceBus.Blazor.Services;

public class MessageBusSenderService
{
	ILogger<MessageBusSenderService> _logger;
	private readonly string? _connectionString;
	private readonly string _queueName = "message-bus-poc";

	public MessageBusSenderService(ILogger<MessageBusSenderService> logger, IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

		_logger = logger;
		_connectionString = configuration.GetConnectionString("AzureServiceBus");
	}

	public async Task SendMessageAsync(MessageModel messageModel)
	{
		await using var client = new ServiceBusClient(_connectionString);
		ServiceBusSender sender = client.CreateSender(_queueName);

		try
		{
			var json = JsonSerializer.Serialize(messageModel);

			ServiceBusMessage message = new ServiceBusMessage(json);

			await sender.SendMessageAsync(message);

			_logger.LogInformation($"send message: {message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message, ex);
			throw;
		}
	}
}
