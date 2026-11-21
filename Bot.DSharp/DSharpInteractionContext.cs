using Bot.Api;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpInteractionContext : DiscordWrapper<InteractionContext>, IBotInteractionContext
    {
        private readonly DSharpGuild m_guild;
        private readonly DSharpChannel m_channel;
        private readonly DSharpMember m_member;
        private readonly bool m_ephemeral;

        public DSharpInteractionContext(InteractionContext wrapped, bool ephemeral = false)
            :base(wrapped)
        {
            m_guild = new DSharpGuild(Wrapped.Guild);
            m_channel = new DSharpChannel(Wrapped.Channel);
            m_member = new DSharpMember(Wrapped.Member);
            m_ephemeral = ephemeral;
        }

        public IServiceProvider Services => Wrapped.Services;
        public IGuild Guild => m_guild;
        public IChannel Channel => m_channel;
        public IMember Member => m_member;

        // Non-component interactions have a null ID here
        public string? ComponentCustomId => null;

        public IEnumerable<string> ComponentValues => Enumerable.Empty<string>();

        public Task DeferInteractionResponse() => Wrapped.DeferAsync(m_ephemeral);

        public Task EditResponseAsync(IBotWebhookBuilder webhookBuilder)
        {
            if (webhookBuilder is DSharpWebhookBuilder irb)
                return ExceptionWrap.WrapExceptionsAsync(() => Wrapped.EditResponseAsync(irb.Wrapped));

            throw new InvalidOperationException("Passed an incorrect builder!");
        }

        public Task UpdateOriginalMessageAsync(IInteractionResponseBuilder builder)
		{
            if (builder is DSharpInteractionResponseBuilder d)
                return ExceptionWrap.WrapExceptionsAsync(() => Wrapped.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, d.Wrapped));

            throw new InvalidOperationException("Passed an incorrect builder!");
		}

        public Task ShowModalAsync(IInteractionResponseBuilder builder)
        {
            if (builder is DSharpInteractionResponseBuilder d)
                return ExceptionWrap.WrapExceptionsAsync(() => Wrapped.CreateResponseAsync(InteractionResponseType.Modal, d.Wrapped));

            throw new InvalidOperationException("Passed an incorrect builder!");
        }
    }
}
