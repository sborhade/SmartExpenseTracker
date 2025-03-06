using Domain.Entities;

public interface IKafkaProducerService
{
    public Task PublishExpenseAsync(Expense expense);
}
