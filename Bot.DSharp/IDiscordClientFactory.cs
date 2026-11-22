using DSharpPlus;

namespace Bot.DSharp
{
    public interface IDiscordClientFactory
    {
        IDiscordClient CreateClient(DiscordClientBuilder builder);
    }

    public class DiscordClientFactory : IDiscordClientFactory
    {
        public IDiscordClient CreateClient(DiscordClientBuilder builder)
        {
            return new DiscordClientWrapper(builder.Build());
        }
    }
}