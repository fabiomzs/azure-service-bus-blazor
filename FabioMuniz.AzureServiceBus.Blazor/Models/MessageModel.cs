namespace FabioMuniz.AzureServiceBus.Blazor.Models;

public class MessageModel
{
	public string Id { get; set; }
	public DateTime Date { get; set; }
	public string Message { get; set; } = string.Empty;

	public MessageModel()
	{
		Id = Guid.NewGuid().ToString();
		Date = DateTime.UtcNow;
		Message = string.Empty;
	}
}