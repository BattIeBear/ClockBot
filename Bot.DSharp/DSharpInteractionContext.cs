using Bot.Api;
using DSharpPlus.Commands.Processors.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpInteractionContext : DiscordWrapper<SlashCommandContext>, IBotInteractionContext
    {
        private readonly DSharpGuild m_guild;
        private readonly DSharpChannel m_channel;
        private readonly DSharpMember m_member;
        private readonly bool m_ephemeral;

        public DSharpInteractionContext(SlashCommandContext wrapped, bool ephemeral = true)
            : base(wrapped)
        {
            var guild = Wrapped.Guild ?? throw new InvalidOperationException("This interaction must occur in a server.");
            var member = Wrapped.Member ?? throw new InvalidOperationException("This interaction must be performed by a server member.");

            m_guild = new DSharpGuild(guild);
            m_channel = new DSharpChannel(Wrapped.Channel);
            m_member = new DSharpMember(member);
            m_ephemeral = ephemeral;
        }

        public IServiceProvider Services => Wrapped.ServiceProvider;
        public IGuild Guild => m_guild;
        public IChannel Channel => m_channel;
        public IMember Member => m_member;

        public string? ComponentCustomId => null;

        public IEnumerable<string> ComponentValues => Enumerable.Empty<string>();

        public async Task DeferInteractionResponse()
        {
            await Wrapped.DeferResponseAsync(m_ephemeral);
        }

        public Task EditResponseAsync(IBotWebhookBuilder webhookBuilder)
        {
            if (webhookBuilder is DSharpWebhookBuilder d)
            {
                return ExceptionWrap.WrapExceptionsAsync(async () =>
                {
                    await Wrapped.EditResponseAsync(d.Wrapped);
                });
            }

            throw new InvalidOperationException("Passed an incorrect builder!");
        }

        public Task UpdateOriginalMessageAsync(IInteractionResponseBuilder builder)
        {
            if (builder is DSharpInteractionResponseBuilder d)
            {
                return ExceptionWrap.WrapExceptionsAsync(async () =>
                {
                    await Wrapped.RespondAsync(d.Wrapped);
                });
            }

            throw new InvalidOperationException("Passed an incorrect builder!");
        }

        public Task ShowModalAsync(IInteractionResponseBuilder builder)
        {
            if (builder is DSharpInteractionResponseBuilder d)
            {
                return ExceptionWrap.WrapExceptionsAsync(async () =>
                {
                    await Wrapped.RespondWithModalAsync(d.Modal);
                });
            }

            throw new InvalidOperationException("Passed an incorrect builder!");
        }
    }
}