using Azure.Messaging.ServiceBus;
using FabioMuniz.AzureServiceBus.Blazor.Models;
using System.Text.Json;

namespace FabioMuniz.AzureServiceBus.Blazor.Services;

public class MessageBusListenerService : IAsyncDisposable
{
	ILogger<MessageBusListenerService> _logger;
	private readonly string? _connectionString;
	private readonly string _queueName = "message-bus-poc";
	private readonly ServiceBusClient _serviceBusClient;
	private readonly ServiceBusProcessor _processor;
	private readonly List<MessageModel> _messages = new List<MessageModel>();

	public event Action<MessageModel> OnMessageReceived;

	public MessageBusListenerService(ILogger<MessageBusListenerService> logger, IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

		_logger = logger;
		_connectionString = configuration.GetConnectionString("AzureServiceBus");

		_serviceBusClient = new ServiceBusClient(_connectionString);
		_processor = _serviceBusClient.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

		_processor.ProcessMessageAsync += MessageHandler;
		_processor.ProcessErrorAsync += ErrorHandler;
	}

	private async Task ErrorHandler(ProcessErrorEventArgs args)
	{
		_logger.LogError(args.Exception.Message, args);	
		await Task.CompletedTask;
	}

	public async Task MessageHandler(ProcessMessageEventArgs args)
	{
		_logger.LogInformation($"Process message {args.Message.MessageId}");

		var body = JsonSerializer.Deserialize<MessageModel>(args.Message.Body.ToString());
		_messages.Add(body);
		OnMessageReceived?.Invoke(body);

		await args.CompleteMessageAsync(args.Message);
	}

	public async Task StartProcessingAsync()
	{
		await _processor.StartProcessingAsync();
	}

	public async Task StopProcessingAsync()
	{
		await _processor.StopProcessingAsync();
	}

	public async ValueTask DisposeAsync()
	{
		await _processor.DisposeAsync();
		await _serviceBusClient.DisposeAsync();
	}
}
