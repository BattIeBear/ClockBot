
using Bot.Api;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpMessagingSlashCommands
    {
        private readonly IBotMessaging m_botMessaging;

        public DSharpMessagingSlashCommands(IBotMessaging botMessaging)
        {
            m_botMessaging = botMessaging;
        }

        [Command("evil")]
        [Description("Send a message informing the evil team of each other")]
        public async Task EvilCommand(
            SlashCommandContext ctx,
            [Description("The demon for this game")] DiscordUser demon,
            [Description("A minion for this game")] DiscordUser minion1,
            [Description("A minion for this game (optional)")] DiscordUser? minion2 = null,
            [Description("A minion for this game (optional)")] DiscordUser? minion3 = null,
            [Description("If a Magician is in this game, specify them here (optional)")] DiscordUser? magician = null)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            var allMinions = new[] { minion1, minion2, minion3 };

            await m_botMessaging.CommandEvilMessageAsync(
                new DSharpInteractionContext(ctx),
                new DSharpMember((DiscordMember)demon),
                allMinions
                    .Where(x => x != null)
                    .Cast<DiscordMember>()
                    .Select(x => new DSharpMember(x))
                    .ToList(),
                magician == null ? null : new DSharpMember((DiscordMember)magician));
        }

        [Command("lunatic")]
        [Description("Send a message to the Lunatic that *looks* like they're the demon")]
        public async Task LunaticCommand(
            SlashCommandContext ctx,
            [Description("The Lunatic!")] DiscordUser lunatic,
            [Description("A fake minion player")] DiscordUser fakeMinion1,
            [Description("A fake minion player")] DiscordUser? fakeMinion2 = null,
            [Description("A fake minion player")] DiscordUser? fakeMinion3 = null)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            var allMinions = new[] { fakeMinion1, fakeMinion2, fakeMinion3 };

            await m_botMessaging.CommandLunaticMessageAsync(
                new DSharpInteractionContext(ctx),
                new DSharpMember((DiscordMember)lunatic),
                allMinions
                    .Where(x => x != null)
                    .Cast<DiscordMember>()
                    .Select(x => new DSharpMember(x))
                    .ToList());
        }
    }
}