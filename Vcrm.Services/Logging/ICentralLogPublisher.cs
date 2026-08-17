using Vcrm.Models.Logging;

namespace Vcrm.Services.Logging;

public interface ICentralLogPublisher
{
    Task PublishAsync(CentralLogEntry entry, CancellationToken cancellationToken = default);
}
