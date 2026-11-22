using Bot.Api;
using DSharpPlus.Entities;

namespace Bot.DSharp
{
    class DSharpComponent : DiscordWrapper<DiscordComponent>, IBotComponent
    {
        public DSharpComponent(DiscordComponent wrapped, string? modalLabel = null)
            : base(wrapped)
        {
            ModalLabel = modalLabel;
        }

        public string CustomId => Wrapped.CustomId;

        public string? ModalLabel { get; }
    }
}