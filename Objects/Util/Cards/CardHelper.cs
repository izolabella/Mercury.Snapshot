﻿using Mercury.Snapshot.Objects.Structures.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Snapshot.Objects.Util.Cards
{
    internal class CardHelper
    {
        internal CardHelper(List<ICard> Cards)
        {
            List<List<EmbedFieldBuilder>> CardsBuilder = new();
            foreach(ICard Card in Cards)
            {
                IReadOnlyList<EmbedFieldBuilder> ThisSet = Card.Render();
                CardsBuilder.Add(ThisSet.ToList());
            }
            this.cards = CardsBuilder;
        }

        private readonly List<List<EmbedFieldBuilder>> cards;
        internal List<List<EmbedFieldBuilder>> Cards => this.cards;


        internal List<EmbedFieldBuilder> CorrectWhitespacing()
        {
            List<EmbedFieldBuilder> Finished = new();
            foreach(List<EmbedFieldBuilder> Set in this.Cards.ToList())
            {
                if(Set != this.Cards.Last())
                {
                    string? TrimmedValue = Set.Last().Value.ToString()?.Trim(new[] { '\u200b', ' ', '\n' });
                    string? TrimmedName = Set.Last().Name.ToString()?.Trim(new[] { '\u200b', ' ', '\n' });
                    Set.Last().Value = TrimmedValue ?? Set.Last().Value;
                    Set.Last().Name = TrimmedName ?? Set.Last().Name;
                    Set.Last().Value += "\u200b\n────────⊹⊱-☿-⊰⊹────────";
                }
                Finished.AddRange(Set);
            }
            return Finished;
        }
    }
}