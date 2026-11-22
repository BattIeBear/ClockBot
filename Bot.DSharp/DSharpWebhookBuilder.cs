using Bot.Api;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.DSharp
{
    public class DSharpWebhookBuilder : DiscordWrapper<DiscordWebhookBuilder>, IBotWebhookBuilder
    {
        public DSharpWebhookBuilder(DiscordWebhookBuilder wrapped)
            : base(wrapped)
        {}

        public IBotWebhookBuilder WithContent(string content)
        {
            var w2 = Wrapped.WithContent(content);
            if (w2 != Wrapped) throw new ApplicationException("Unexpected chained call did not return itself");
            return this;
        }


        public IBotWebhookBuilder AddComponents(params IBotComponent[] components)
        {
            if (!components.All(x => x is DSharpComponent))
                throw new InvalidOperationException("Unexpected type of IComponent!");

            var realComponents = components.Cast<DSharpComponent>().Select(x => x.Wrapped).ToList();
            var buttons = new List<DiscordButtonComponent>();

            void AddButtons()
            {
                while (buttons.Count > 0)
                {
                    var row = buttons.Take(5).ToList();
                    var w2 = Wrapped.AddActionRowComponent(row);

                    if (w2 != Wrapped)
                        throw new ApplicationException("Unexpected chained call did not return itself");

                    buttons.RemoveRange(0, row.Count);
                }
            }

            foreach (var component in realComponents)
            {
                if (component is DiscordButtonComponent button)
                {
                    buttons.Add(button);
                }
                else if (component is BaseDiscordSelectComponent select)
                {
                    AddButtons();

                    var w2 = Wrapped.AddActionRowComponent(select);

                    if (w2 != Wrapped)
                        throw new ApplicationException("Unexpected chained call did not return itself");
                }
                else
                {
                    throw new InvalidOperationException("Unsupported component type!");
                }
            }

            AddButtons();

            return this;
        }

        public IBotWebhookBuilder AddEmbeds(IEnumerable<IEmbed> embeds)
        {
            var typed = embeds.Select(e => e as DSharpEmbed);
            if (!typed.All(e => e != null)) throw new InvalidOperationException("Expected to be passed only embeds of DSharp types");

            var w2 = Wrapped.AddEmbeds(typed.Select(t => t!.Wrapped));
            if (w2 != Wrapped) throw new ApplicationException("Unexpected chained call did not return itself");
            return this;
        }
    }
}
