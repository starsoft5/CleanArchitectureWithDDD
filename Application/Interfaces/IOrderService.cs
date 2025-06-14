using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderService
{
    List<Order> GetAll();
    Order? GetById(int id);
    void Create(Order order);
    void Update(Order order);
    void Delete(int id);
}
