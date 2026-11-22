
using Bot.Api.Database;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    internal static class DSharpCommandPermissions
    {
        public static bool IsAdministrator(InteractionContext ctx)
        {
            return ctx.Member != null && ctx.Member.Permissions.HasPermission(Permissions.Administrator);
        }

        public static async Task<bool> RequireAdministratorAsync(InteractionContext ctx)
        {
            if (IsAdministrator(ctx))
            {
                return true;
            }

            await DenyAsync(ctx, "You must have the Discord **Administrator** permission to use this command.");
            return false;
        }

        public static async Task<bool> RequireStorytellerAsync(InteractionContext ctx)
        {
            if (ctx.Guild == null || ctx.Member == null)
            {
                await DenyAsync(ctx, "This command can only be used in a server.");
                return false;
            }

            var townDb = ctx.Services.GetService(typeof(ITownDatabase)) as ITownDatabase;

            if (townDb == null)
            {
                throw new System.InvalidOperationException("ITownDatabase is not registered in the service provider.");
            }

            var townRecord = await townDb.GetTownRecordAsync(ctx.Guild.Id, ctx.Channel.Id);

            if (townRecord == null || townRecord.StorytellerRoleId == 0 || !ctx.Member.Roles.Any(role => role.Id == townRecord.StorytellerRoleId))
            {
                await DenyAsync(ctx, "You must have this town's **Storyteller** role to use this command.");
                return false;
            }

            return true;
        }

        public static async Task<bool> RequireStorytellerOrAdministratorAsync(InteractionContext ctx)
        {
            if (IsAdministrator(ctx))
            {
                return true;
            }

            return await RequireStorytellerAsync(ctx);
        }

        private static Task DenyAsync(InteractionContext ctx, string message)
        {
            return ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral());
        }
    }
}