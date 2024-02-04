using UP.Models;

namespace UP.Migrations.Services.Interfaces;

public interface IServiceService
{
    public Task<IEnumerable<Service>> GetServices();
}