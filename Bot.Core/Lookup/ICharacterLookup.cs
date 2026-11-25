using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Core.Lookup
{
    public interface ICharacterLookup
    {
        Task<LookupCharacterResult> LookupCharacterAsync(ulong guildId, string charString);
        Task RefreshCharactersAsync(ulong guildId);
        Task<IReadOnlyCollection<string>> GetCharacterNamesAsync(ulong guildId, string searchString);
    }
}
