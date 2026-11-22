
using Bot.Api;
using Bot.Api.Database;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpClient : IBotClient
    {
        private const ulong DevGuildId = 1572983022158876765ul;

        private readonly IComponentService m_componentService;
        private readonly IDiscordClientFactory m_discordFactory;
        private readonly DiscordClientBuilder m_builder;
        private readonly string m_deployType;

        private IDiscordClient? m_discord;

        public event EventHandler<EventArgs>? Connected;
        public event EventHandler<Bot.Api.MessageCreatedEventArgs>? MessageCreated;

        public DSharpClient(IServiceProvider serviceProvider)
        {
            serviceProvider.Inject(out m_componentService);

            var environment = (IEnvironment)serviceProvider.GetService(typeof(IEnvironment))!;
            m_deployType = environment.GetEnvironmentVariable("DEPLOY_TYPE") ?? "none";

            var token = environment.GetEnvironmentVariable("DISCORD_TOKEN");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidDiscordTokenException();
            }

            m_builder = DiscordClientBuilder.CreateSharded(token, DiscordIntents.AllUnprivileged);

            m_builder.ConfigureEventHandlers(b => b
                .HandleSessionCreated((sender, e) =>
                {
                    Connected?.Invoke(this, EventArgs.Empty);
                    return Task.CompletedTask;
                })
                .HandleMessageCreated(OnMessageCreated)
                .HandleComponentInteractionCreated(ComponentInteractionCreated)
                .HandleModalSubmitted(ModalSubmitted));

            m_discordFactory = (IDiscordClientFactory)serviceProvider.GetService(typeof(IDiscordClientFactory))!;
        }

        public async Task ConnectAsync(IServiceProvider botServices)
        {
            if (m_discord != null)
            {
                throw new InvalidOperationException("The Discord client has already been created.");
            }

            if (m_deployType != "dev" && m_deployType != "prod")
            {
                throw new InvalidOperationException("Bot must be configured with either 'dev' or 'prod' DEPLOY_TYPE!");
            }

            m_builder.ConfigureServices(services =>
            {
                services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
                    logging.AddFilter("DSharpPlus.Clients.IShardOrchestrator", LogLevel.Critical);
                    //logging.AddFilter("DSharpPlus", LogLevel.Warning);
                });

                services.AddSingleton((IBotSetup)botServices.GetService(typeof(IBotSetup))!);
                services.AddSingleton((IBotGameplayInteractionHandler)botServices.GetService(typeof(IBotGameplayInteractionHandler))!);
                services.AddSingleton((IBotMessaging)botServices.GetService(typeof(IBotMessaging))!);
                services.AddSingleton((IBotLookupService)botServices.GetService(typeof(IBotLookupService))!);
                services.AddSingleton((IAnnouncer)botServices.GetService(typeof(IAnnouncer))!);
                services.AddSingleton((ITownDatabase)botServices.GetService(typeof(ITownDatabase))!);
            });

            var commandTypes = new[]
            {
                typeof(DSharpGameSlashCommands),
                typeof(DSharpMessagingSlashCommands),
                typeof(DSharpLookupSlashCommands),
                typeof(DSharpMiscSlashCommands),
                typeof(DSharpSetupSlashCommands)
            };

            m_builder.UseCommands(
                (serviceProvider, extension) =>
                {
                    extension.AddCommands(commandTypes);
                    extension.AddProcessor(
                        new SlashCommandProcessor(
                            new SlashCommandConfiguration
                            {
                                UnconditionallyOverwriteCommands = true
                            }));
                },
                new CommandsConfiguration
                {
                    RegisterDefaultCommandProcessors = false,
                    DebugGuildId = m_deployType == "dev" ? DevGuildId : 0
                });

            m_discord = m_discordFactory.CreateClient(m_builder);
            await m_discord.ConnectAsync();
        }

        private Task OnMessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreatedEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                MessageCreated?.Invoke(this, new Bot.Api.MessageCreatedEventArgs(
                    new DSharpChannel(e.Channel),
                    e.Message.Content));
            }

            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            return m_discord?.DisconnectAsync() ?? Task.CompletedTask;
        }

        public Task<IGuild?> GetGuildAsync(ulong id)
        {
            if (m_discord == null)
            {
                throw new InvalidOperationException("The Discord client has not been created.");
            }

            return m_discord.GetGuildAsync(id);
        }

        private Task ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreatedEventArgs e)
        {
            return m_componentService.CallAsync(new DSharpComponentContext(e.Interaction));
        }

        private Task ModalSubmitted(DiscordClient sender, ModalSubmittedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public class InvalidDiscordTokenException : Exception { }
    }
}
