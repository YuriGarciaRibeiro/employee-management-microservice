namespace EmployeeManagement.Cadastro.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : class;
}