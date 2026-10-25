using N1che.Domain.Models.Niches;

namespace N1che.Domain.Interfaces.Persistence.Readers.Niches;

public interface INichesReader
{
    Task<IReadOnlyCollection<NicheModel>> GetAll(CancellationToken cancellationToken);
}
