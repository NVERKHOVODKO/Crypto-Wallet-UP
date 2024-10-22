using Up.Core.Models;
using Up.Infrastructure.Repositories.Interfaces;
using UP.Migrations.Services.Interfaces;

namespace Up.Infrastructure.Services;

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