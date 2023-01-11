using ExTest;

string connectionString = "";
string queueName = "";

Console.Write("Enter 1 to write or enter any other number to read: ");
if (int.TryParse(Console.ReadLine(), out int input))
{
	if (input == 1)
	{
		try
		{
			await using ServiceBusWriter writer = new(connectionString, queueName);
			await writer.WriteAsync(CancellationToken.None);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
	else
	{
		Console.WriteLine("Press any key to stop reading");
		try
		{
			await using ServiceBusReader reader = new(connectionString, queueName);
			await reader.StartAsync(CancellationToken.None);
			Console.ReadKey(true);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}