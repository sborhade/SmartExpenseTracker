namespace Application.Interfaces;
public interface IKafkaProducer
{
    public Task ProduceAsync(string topic, string message);
}