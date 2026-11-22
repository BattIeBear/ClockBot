
using Bot.Api.Database;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    internal static class DSharpCommandPermissions
    {
        public static bool IsAdministrator(SlashCommandContext ctx)
        {
            return ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false;
        }

        public static async Task<bool> RequireAdministratorAsync(SlashCommandContext ctx)
        {
            if (IsAdministrator(ctx))
            {
                return true;
            }

            await DenyAsync(ctx, "You must have the Discord **Administrator** permission to use this command.");
            return false;
        }

        public static async Task<bool> RequireStorytellerAsync(SlashCommandContext ctx)
        {
            if (ctx.Guild == null || ctx.Member == null)
            {
                await DenyAsync(ctx, "This command can only be used in a server.");
                return false;
            }

            var townDb = ctx.ServiceProvider.GetService(typeof(ITownDatabase)) as ITownDatabase;

            if (townDb == null)
            {
                throw new System.InvalidOperationException("ITownDatabase is not registered in the service provider.");
            }

            var townRecord = await townDb.GetTownRecordAsync(ctx.Guild.Id, ctx.Channel.Id);

            if (townRecord == null ||
                townRecord.StorytellerRoleId == 0 ||
                !ctx.Member.Roles.Any(role => role.Id == townRecord.StorytellerRoleId))
            {
                await DenyAsync(ctx, "You must have this town's **Storyteller** role to use this command.");
                return false;
            }

            return true;
        }

        public static async Task<bool> RequireStorytellerOrAdministratorAsync(SlashCommandContext ctx)
        {
            if (IsAdministrator(ctx))
            {
                return true;
            }

            return await RequireStorytellerAsync(ctx);
        }

        private static async Task DenyAsync(SlashCommandContext ctx, string message)
        {
            await ctx.RespondAsync(message, true);
        }
    }
}
