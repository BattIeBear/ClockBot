using Bot.Api;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.DSharp
{
    class DSharpInteractionResponseBuilder : DiscordWrapper<DiscordInteractionResponseBuilder>, IInteractionResponseBuilder
    {
        public DiscordModalBuilder Modal { get; } = new DiscordModalBuilder();

        public DSharpInteractionResponseBuilder(DiscordInteractionResponseBuilder wrapped)
            : base(wrapped)
        {
        }

        public IInteractionResponseBuilder WithContent(string content)
        {
            Wrapped.WithContent(content);
            return this;
        }

        public IInteractionResponseBuilder WithTitle(string title)
        {
            Modal.WithTitle(title);
            return this;
        }

        public IInteractionResponseBuilder WithCustomId(string customId)
        {
            Modal.WithCustomId(customId);
            return this;
        }

        public IInteractionResponseBuilder AddComponents(params IBotComponent[] components)
        {
            if (!components.All(x => x is DSharpComponent))
                throw new InvalidOperationException("Unexpected type of IComponent!");

            var typed = components.Cast<DSharpComponent>().ToList();

            if (typed.All(x => x.Wrapped is DiscordTextInputComponent))
            {
                foreach (var component in typed)
                {
                    if (string.IsNullOrWhiteSpace(component.ModalLabel))
                        throw new InvalidOperationException("Modal text inputs must have a label!");

                    Modal.AddTextInput((DiscordTextInputComponent)component.Wrapped, component.ModalLabel);
                }
            }
            else
            {
                if (typed.Any(x => x.Wrapped is DiscordTextInputComponent))
                    throw new InvalidOperationException("Modal text inputs cannot be mixed with message components!");

                foreach (var component in typed)
                {
                    if (component.Wrapped is DiscordButtonComponent button)
                    {
                        Wrapped.AddActionRowComponent(button);
                    }
                    else if (component.Wrapped is BaseDiscordSelectComponent select)
                    {
                        Wrapped.AddActionRowComponent(select);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported interaction component type!");
                    }
                }
            }

            return this;
        }

        public IInteractionResponseBuilder AddEmbeds(IEnumerable<IEmbed> embeds)
        {
            var typed = embeds.Select(e => e as DSharpEmbed).ToList();

            if (!typed.All(e => e != null))
                throw new InvalidOperationException("Expected to be passed only embeds of DSharp types");

            Wrapped.AddEmbeds(typed.Select(t => t!.Wrapped));

            return this;
        }
    }
}