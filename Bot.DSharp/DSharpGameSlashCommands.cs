
using Bot.Api;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class EmptyCommands
    {
    }

    public class DSharpGameSlashCommands
    {
        private readonly IBotGameplayInteractionHandler m_botGameplayHandler;

        public DSharpGameSlashCommands(IBotGameplayInteractionHandler botGameplayHandler)
        {
            m_botGameplayHandler = botGameplayHandler;
        }

        [Command("night")]
        [Description("Move all active players from Town Square into Cottages for the night")]
        public async Task NightCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.CommandNightAsync(new DSharpInteractionContext(ctx));
        }

        [Command("day")]
        [Description("Move all active players from Cottages to Town Square")]
        public async Task DayCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.CommandDayAsync(new DSharpInteractionContext(ctx));
        }

        [Command("vote")]
        [Description("Move all active players to Town Square for voting")]
        public async Task VoteCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.CommandVoteAsync(new DSharpInteractionContext(ctx));
        }

        [Command("votetimer")]
        [Description("Move all active players to Town Square for voting after a provided amount of time")]
        public async Task VoteTimerCommand(
            SlashCommandContext ctx,
            [Description("Time string, such as \"5m30s\" or \"2 minutes\". Valid times are between 10 seconds and 20 minutes.")]
            string timeString)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.RunVoteTimerAsync(new DSharpInteractionContext(ctx), timeString);
        }

        [Command("stopvotetimer")]
        [Description("Cancels an outstanding call to `/votetimer`.")]
        public async Task StopVoteTimerCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.RunStopVoteTimerAsync(new DSharpInteractionContext(ctx));
        }

        [Command("endgame")]
        [Description("End any current game, removing roles etc")]
        public async Task EndGameCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            await m_botGameplayHandler.CommandEndGameAsync(new DSharpInteractionContext(ctx));
        }

        [Command("storytellers")]
        [Description("Explicitly list which users should be Storytellers")]
        public async Task StorytellersCommand(
            SlashCommandContext ctx,
            [Description("Storyteller")] DiscordUser user1,
            [Description("Further storyteller")] DiscordUser? user2 = null,
            [Description("An additional storyteller")] DiscordUser? user3 = null,
            [Description("Yet another storyteller")] DiscordUser? user4 = null,
            [Description("Hopefully the last storyteller")] DiscordUser? user5 = null)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerAsync(ctx))
            {
                return;
            }

            var allUsers = new[] { user1, user2, user3, user4, user5 };

            await m_botGameplayHandler.CommandSetStorytellersAsync(
                new DSharpInteractionContext(ctx),
                allUsers
                    .Where(x => x != null)
                    .Cast<DiscordMember>()
                    .Select(x => new DSharpMember(x))
                    .ToList());
        }
    }
}