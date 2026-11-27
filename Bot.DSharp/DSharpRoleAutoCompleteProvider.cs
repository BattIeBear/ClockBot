using Bot.Api;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DSharp
{
    public class DSharpRoleAutoCompleteProvider : IAutoCompleteProvider
    {
        private readonly IBotLookupService m_botLookup;

        public DSharpRoleAutoCompleteProvider(IBotLookupService botLookup)
        {
            m_botLookup = botLookup;
        }

        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            //if (context.Guild is null || string.IsNullOrWhiteSpace(context.UserInput))
            //{
            //    return Enumerable.Empty<DiscordAutoCompleteChoice>();
            //}

            var names = await m_botLookup.GetCharacterNamesAsync(context.Guild.Id, context.UserInput);

            return names.Select(name => new DiscordAutoCompleteChoice(name, name));
        }
    }
}