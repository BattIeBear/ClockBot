using Bot.Api;
using DSharpPlus;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public interface IDiscordClient
    {
        Task<IGuild?> GetGuildAsync(ulong id);
        Task ConnectAsync();
        Task DisconnectAsync();
    }

    public class DiscordClientWrapper : IDiscordClient
    {
        private readonly DiscordClient m_wrapped;

        public DiscordClientWrapper(DiscordClient wrapped)
        {
            m_wrapped = wrapped;
        }

        public async Task<IGuild?> GetGuildAsync(ulong id)
        {
            var guild = await m_wrapped.GetGuildAsync(id);
            return guild is null ? null : new DSharpGuild(guild);
        }

        public Task ConnectAsync() => m_wrapped.ConnectAsync();

        public Task DisconnectAsync() => m_wrapped.DisconnectAsync();
    }
}