
using Bot.Api;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpSetupSlashCommands
    {
        private readonly IBotSetup m_botSetup;

        public DSharpSetupSlashCommands(IBotSetup botSetup)
        {
            m_botSetup = botSetup;
        }

        [Command("createtown")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Create a new Town on this server")]
        public async Task CreateTownCommand(SlashCommandContext ctx,
            [Description("Town Name")] string townName,
            [Description("Server Player Role - only they can see the town")] DiscordRole? playerRole = null,
            [Description("Server Storyteller Role - only they can see control channels")] DiscordRole? stRole = null,
            [Description("If true, a Night category full of cottages will be created")] bool useNight = true)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            var wrappedPlayerRole = playerRole == null ? null : new DSharpRole(playerRole);
            var wrappedStRole = stRole == null ? null : new DSharpRole(stRole);

            await m_botSetup.CreateTownAsync(new DSharpInteractionContext(ctx), townName, wrappedPlayerRole, wrappedStRole, useNight);
        }

        [Command("towninfo")]
        [Description("Get info about any town registered for this server and channel")]
        public async Task TownInfoCommand(SlashCommandContext ctx)
        {
            if (!await DSharpCommandPermissions.RequireStorytellerOrAdministratorAsync(ctx))
            {
                return;
            }

            await m_botSetup.TownInfoAsync(new DSharpInteractionContext(ctx));
        }

        [Command("destroytown")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Destroy any channels and roles created by /createtown for the town with the given name")]
        public async Task DestroyTownCommand(SlashCommandContext ctx,
            [Description("Town Name")] string townName)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            await m_botSetup.DestroyTownAsync(new DSharpInteractionContext(ctx), townName);
        }

        [Command("modifytown")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Modify one of the optional details of a town")]
        public async Task ModifyTownCommand(SlashCommandContext ctx,
            [Description("Set the text chat channel for this town")] DiscordChannel? chatChannel = null,
            [Description("Set the Night category for this town")] DiscordChannel? nightCat = null)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            List<string> errors = new();

            if (chatChannel != null && chatChannel.Type != DiscordChannelType.Text)
            {
                errors.Add($"- Chat channel `{chatChannel.Name}` is not a text channel!");
            }

            if (nightCat != null && nightCat.Type != DiscordChannelType.Category)
            {
                errors.Add($"- Night category `{nightCat.Name}` is not a category!");
            }

            if (errors.Count > 0)
            {
                await ctx.RespondAsync("Couldn't modify the town due to the following errors:\n" + string.Join("\n", errors), true);
                return;
            }

            var chatChanWrapped = chatChannel == null ? null : new DSharpChannel(chatChannel);
            var nightCatWrapped = nightCat == null ? null : new DSharpChannelCategory(nightCat);

            await m_botSetup.ModifyTownAsync(new DSharpInteractionContext(ctx), chatChanWrapped, nightCatWrapped);
        }

        [Command("addtown")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Add a new town composed of existing channels and roles on this server")]
        public async Task AddTownCommand(SlashCommandContext ctx,
            [Description("Control channel (must be text)")] DiscordChannel controlChannel,
            [Description("Town Square channel (must be voice)")] DiscordChannel townSquare,
            [Description("Day Category (must contain control and town square channels)")] DiscordChannel dayCategory,
            [Description("Role to grant storytellers in this town during an active game")] DiscordRole stRole,
            [Description("Role to grant villagers in this town during an active game")] DiscordRole villagerRole,
            [Description("Night Category (optional)")] DiscordChannel? nightCategory = null,
            [Description("Chat channel (optional, must be text)")] DiscordChannel? chatChannel = null)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            List<string> errors = new();

            if (controlChannel.Type != DiscordChannelType.Text)
            {
                errors.Add($"- Control channel `{controlChannel.Name}` is not a text channel!");
            }

            if (dayCategory.Type != DiscordChannelType.Category)
            {
                errors.Add($"- Day category `{dayCategory.Name}` is not a category!");
            }

            if (townSquare.Type != DiscordChannelType.Voice)
            {
                errors.Add($"- Town Square channel `{townSquare.Name}` is not a voice channel!");
            }

            if (dayCategory.Type == DiscordChannelType.Category && !dayCategory.Children.Contains(controlChannel))
            {
                errors.Add($"- Control channel `{controlChannel.Name}` is not in the **{dayCategory.Name}** category!");
            }

            if (dayCategory.Type == DiscordChannelType.Category && !dayCategory.Children.Contains(townSquare))
            {
                errors.Add($"- Town Square channel `{townSquare.Name}` is not in the **{dayCategory.Name}** category!");
            }

            if (nightCategory != null && nightCategory.Type != DiscordChannelType.Category)
            {
                errors.Add($"- Night category `{nightCategory.Name}` is not a category!");
            }

            if (chatChannel != null && chatChannel.Type != DiscordChannelType.Text)
            {
                errors.Add($"- Chat channel `{chatChannel.Name}` is not a text channel!");
            }

            if (chatChannel != null && dayCategory.Type == DiscordChannelType.Category && !dayCategory.Children.Contains(chatChannel))
            {
                errors.Add($"- Chat channel `{chatChannel.Name}` is not in the **{dayCategory.Name}** category!");
            }

            if (errors.Count > 0)
            {
                string msg = "Couldn't add a town due to the following errors:\n" + string.Join("\n", errors);
                await ctx.RespondAsync(msg, true);
                return;
            }

            var controlChan = new DSharpChannel(controlChannel);
            var townChan = new DSharpChannel(townSquare);
            var dayCat = new DSharpChannelCategory(dayCategory);

            var nightCat = nightCategory == null ? null : new DSharpChannelCategory(nightCategory);
            var stRoleWrapped = new DSharpRole(stRole);
            var villagerRoleWrapped = new DSharpRole(villagerRole);
            var chatChan = chatChannel == null ? null : new DSharpChannel(chatChannel);

            await m_botSetup.AddTownAsync(new DSharpInteractionContext(ctx), controlChan, townChan, dayCat, nightCat, stRoleWrapped, villagerRoleWrapped, chatChan);
        }

        [Command("removetown")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Unregister a town on this server without deleting any channels or roles")]
        public async Task RemoveTownCommand(SlashCommandContext ctx,
            [Description("Town to remove - if blank, must be run from the town's control channel")] DiscordChannel? dayCat = null)
        {
            if (!await DSharpCommandPermissions.RequireAdministratorAsync(ctx))
            {
                return;
            }

            if (dayCat != null && dayCat.Type != DiscordChannelType.Category)
            {
                await ctx.RespondAsync($"**Error:** Channel `{dayCat.Name}` isn't a category!", true);
                return;
            }

            DSharpChannelCategory? dayCatWrapped = dayCat == null ? null : new DSharpChannelCategory(dayCat);

            await m_botSetup.RemoveTownAsync(new DSharpInteractionContext(ctx), dayCatWrapped);
        }

        [Command("playercount")]
        [Description("Show the player count chart for this town")]
        public Task PlayerCountCommand(SlashCommandContext ctx)
        {
            return m_botSetup.PlayerCountAsync(new DSharpInteractionContext(ctx));
        }
    }
}
