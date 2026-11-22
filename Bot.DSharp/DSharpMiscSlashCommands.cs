using Bot.Api;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpMiscSlashCommands
    {
        private readonly IAnnouncer m_announcer;

        public DSharpMiscSlashCommands(IAnnouncer announcer)
        {
            m_announcer = announcer;
        }

        [Command("announce")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Set whether this server wants to hear new version announcements")]
        public async Task AnnounceCommand(
            SlashCommandContext ctx,
            [Description("If true, this server will hear new version announcements")] bool hear)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            await m_announcer.CommandSetGuildAnnounce(new DSharpInteractionContext(ctx), hear);
        }
    }
}
