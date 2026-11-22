
using Bot.Api;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpLookupSlashCommands
    {
        private readonly IBotLookupService m_botLookup;

        public DSharpLookupSlashCommands(IBotLookupService botLookup)
        {
            m_botLookup = botLookup;
        }

        [Command("role")]
        [Description("Look up a role")]
        public Task LookupCommand(
            SlashCommandContext ctx,
            [Description("Role you want to look up")] string lookupString)
        {
            return m_botLookup.LookupAsync(new DSharpInteractionContext(ctx), lookupString);
        }

        [Command("addscript")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Add a script json URL for later lookup")]
        public async Task AddScriptCommand(
            SlashCommandContext ctx,
            [Description("URL pointing at a json file for the script")] string scriptJsonUrl)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            await m_botLookup.AddScriptAsync(new DSharpInteractionContext(ctx), scriptJsonUrl);
        }

        [Command("removescript")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Remove script json URL previously added")]
        public async Task RemoveScriptCommand(
            SlashCommandContext ctx,
            [Description("URL pointing at a json file for the script")] string scriptJsonUrl)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            await m_botLookup.RemoveScriptAsync(new DSharpInteractionContext(ctx), scriptJsonUrl);
        }

        [Command("listscripts")]
        [Description("List script json URLs added to this server")]
        public Task ListScriptsCommand(SlashCommandContext ctx)
        {
            return m_botLookup.ListScriptsAsync(new DSharpInteractionContext(ctx));
        }

        [Command("refreshscripts")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Refresh scripts registered for this server")]
        public async Task RefreshScriptsCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            await m_botLookup.RefreshScriptsAsync(new DSharpInteractionContext(ctx));
        }
    }
}
