using N1che.Domain.Models.Niches;

namespace N1che.Domain.Interfaces.Services.Niches;

public interface INicheRetrievalService
{
    ValueTask<IReadOnlyCollection<NicheModel>> GetAllAsync(CancellationToken cancellationToken);
}
