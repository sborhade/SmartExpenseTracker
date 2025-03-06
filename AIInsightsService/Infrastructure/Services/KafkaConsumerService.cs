using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using Domain.Entities;
using Domain.Interfaces;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic, _group, _bootstrapServers;

    public KafkaConsumerService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;

        _topic = _configuration["Kafka:expenses-topic"]?.ToString() ?? string.Empty;
        _group = _configuration["Kafka:group-id"]?.ToString() ?? string.Empty;
        _bootstrapServers = _configuration["Kafka:bootstrap-servers"]?.ToString() ?? string.Empty;

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _group,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Consuming Kafka message...");
                Console.WriteLine("_topic: " + _topic + " groupiid: " + _group + " bootstrap: " + _bootstrapServers);
                var consumeResult = _consumer.Consume(stoppingToken);
                Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
                var expense = JsonSerializer.Deserialize<Expense>(consumeResult.Message.Value);

                using var scope = _serviceScopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IExpenseProcessor>();

                if (expense != null)
                {
                    Console.WriteLine("Processing expense...");
                    await processor.ProcessExpenseAsync(expense);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"  Consume error: {e.Error.Reason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error processing Kafka message: {ex.Message}");
            }
        }
    }
}
