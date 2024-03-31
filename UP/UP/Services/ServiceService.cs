using Repository;
using UP.Migrations.Services.Interfaces;
using UP.Models;

namespace UP.Migrations.Services;

public class ServiceService : IServiceService
{
    private readonly IDbRepository _repository;

    public ServiceService(IDbRepository repository)
    {
        _repository = repository;
    }
    
    public Task<IEnumerable<Service>> GetServices()
    {
        return Task.FromResult<IEnumerable<Service>>(_repository.GetAll<Service>());
    }
}