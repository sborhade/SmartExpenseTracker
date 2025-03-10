using System.Text.Json;
using Confluent.Kafka;
using Domain.Entities;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService()
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishExpenseAsync(Expense expense)
    {
        var message = new Message<Null, string> { Value = JsonSerializer.Serialize(expense) };
        await _producer.ProduceAsync("expenses", message);
    }
}
