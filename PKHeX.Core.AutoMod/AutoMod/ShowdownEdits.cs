using System;
using System.Linq;

namespace PKHeX.Core.AutoMod
{
    /// <summary>
    /// Modifications for a <see cref="PKM"/> based on a <see cref="ShowdownSet"/>
    /// </summary>
    public static class ShowdownEdits
    {
        /// <summary>
        /// Quick Gender Toggle
        /// </summary>
        /// <param name="pk">PKM whose gender needs to be toggled</param>
        /// <param name="set">Showdown Set for Gender reference</param>
        public static void FixGender(this PKM pk, ShowdownSet set)
        {
            pk.ApplySetGender(set);
            var la = new LegalityAnalysis(pk);
            if (la.Valid)
                return;
            string Report = la.Report();

            if (Report.Contains(LegalityCheckStrings.LPIDGenderMismatch))
                pk.Gender = pk.Gender == 0 ? 1 : 0;

            if (pk.Gender != 0 && pk.Gender != 1)
                pk.Gender = pk.GetSaneGender();
        }

        /// <summary>
        /// Set Nature and Ability of the pokemon
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="set">Showdown Set to refer</param>
        /// <param name="preference">Ability index (1/2/4) preferred; &lt;= 0 for any</param>
        public static void SetNatureAbility(this PKM pk, ShowdownSet set, int preference = -1)
        {
            // Values that are must for showdown set to work, IVs should be adjusted to account for this
            var val = Math.Min((int) Nature.Quirky, Math.Max((int) Nature.Hardy, set.Nature));
            pk.SetNature(val);
            var orig = pk.Nature;
            if (orig != val)
            {
                pk.Nature = val;
                if (pk.Species == (int)Species.Toxtricity && pk.AltForm != EvolutionMethod.GetAmpLowKeyResult(pk.Nature))
                    pk.Nature = orig;
                var la = new LegalityAnalysis(pk);
                if (la.Info.Parse.Any(z => z.Identifier == CheckIdentifier.Nature && !z.Valid))
                    pk.Nature = orig;
            }
            if (pk.Ability != set.Ability)
                pk.SetAbility(set.Ability);

            if (preference > 0)
            {
                // Set preferred ability number if applicable
                var abilities = pk.PersonalInfo.Abilities;
                pk.AbilityNumber = abilities[preference >> 1] == set.Ability ? preference : pk.AbilityNumber;
            }
        }

        /// <summary>
        /// Set Species and Level with nickname (Helps with PreEvos)
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="set">Set to use as reference</param>
        /// <param name="Form">Form to apply</param>
        /// <param name="enc">Encounter detail</param>
        public static void SetSpeciesLevel(this PKM pk, ShowdownSet set, int Form, IEncounterable enc)
        {
            pk.Species = set.Species;
            pk.ApplySetGender(set);
            pk.SetAltForm(Form);
            pk.SetFormArgument(enc);

            var gen = new LegalityAnalysis(pk).Info.Generation;
            var nickname = Legal.GetMaxLengthNickname(gen, LanguageID.English) < set.Nickname.Length ? set.Nickname.Substring(0, Legal.GetMaxLengthNickname(gen, LanguageID.English)) : set.Nickname;
            if (!WordFilter.IsFiltered(nickname, out _))
                pk.SetNickname(nickname);
            else
                pk.ClearNickname();
            pk.CurrentLevel = set.Level;
            if (pk.CurrentLevel == 50)
                pk.CurrentLevel = 100; // VGC Override
        }

        private static void SetFormArgument(this PKM pk, IEncounterable enc)
        {
            if (pk is IFormArgument f)
                f.FormArgument = GetSuggestedFormArgument(pk, enc.Species);
        }

        public static uint GetSuggestedFormArgument(PKM pk, int origSpecies = 0)
        {
            return pk.Species switch
            {
                (int)Species.Hoopa when pk.AltForm != 0 => 3,
                (int)Species.Furfrou when pk.AltForm != 0 => 5,
                (int)Species.Runerigus when origSpecies != (int)Species.Runerigus => 49,
                _ => 0
            };
        }

        private static void ApplySetGender(this PKM pk, ShowdownSet set)
        {
            if (!string.IsNullOrWhiteSpace(set.Gender))
                pk.Gender = set.Gender == "M" ? 0 : 1;
            else
                pk.Gender = pk.GetSaneGender();
        }

        /// <summary>
        /// Set Moves, EVs and Items for a specific PKM. These should not affect legality after being vetted by GeneratePKMs
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="set">Showdown Set to refer</param>
        public static void SetMovesEVs(this PKM pk, ShowdownSet set)
        {
            if (set.Moves[0] != 0)
                pk.SetMoves(set.Moves, true);
            pk.CurrentFriendship = set.Friendship;
            if (pk is IAwakened pb7)
            {
                pb7.SetSuggestedAwakenedValues(pk);
            }
            else
            {
                pk.EVs = set.EVs;
                var la = new LegalityAnalysis(pk);
                if (la.Parsed && !pk.WasEvent)
                    pk.SetRelearnMoves(la.GetSuggestedRelearnMoves());
            }
            pk.SetCorrectMetLevel();
        }

        public static void SetHeldItem(this PKM pk, ShowdownSet set)
        {
            pk.ApplyHeldItem(set.HeldItem, set.Format);
            pk.FixInvalidFormItems(); // arceus, silvally, giratina, genesect fix
            if (!ItemRestrictions.IsHeldItemAllowed(pk) || pk is PB7)
                pk.HeldItem = 0; // Remove the item if the item is illegal in its generation
        }

        private static void FixInvalidFormItems(this PKM pk)
        {
            switch ((Species)pk.Species)
            {
                case Species.Arceus:
                    int forma = GetArceusFormFromHeldItem(pk.HeldItem, pk.Format);
                    pk.HeldItem = pk.AltForm != forma ? 0 : pk.HeldItem;
                    pk.AltForm = pk.AltForm != forma ? 0 : forma;
                    break;
                case Species.Silvally:
                    int forms = GetSilvallyFormFromHeldItem(pk.HeldItem);
                    pk.HeldItem = pk.AltForm != forms ? 0 : pk.HeldItem;
                    pk.AltForm = pk.AltForm != forms ? 0 : forms;
                    break;
                case Species.Genesect:
                    int formg = GetGenesectFormFromHeldItem(pk.HeldItem);
                    pk.HeldItem = pk.AltForm != formg ? 0 : pk.HeldItem;
                    pk.AltForm = pk.AltForm != formg ? 0 : formg;
                    break;
                case Species.Giratina when pk.AltForm == 1 && pk.HeldItem != 112:
                    pk.HeldItem = 122;
                    break;
                default:
                    break;
            }
        }

        private static int GetArceusFormFromHeldItem(int item, int format)
        {
            if (777 <= item && item <= 793)
                return Array.IndexOf(Legal.Arceus_ZCrystal, (ushort)item) + 1;

            int form = 0;
            if ((298 <= item && item <= 313) || item == 644)
                form = Array.IndexOf(Legal.Arceus_Plate, (ushort)item) + 1;
            if (format == 4 && form >= 9)
                return form + 1; // ??? type Form shifts everything by 1
            return form;
        }

        private static int GetSilvallyFormFromHeldItem(int item)
        {
            if ((904 <= item && item <= 920) || item == 644)
                return item - 903;
            return 0;
        }

        private static int GetGenesectFormFromHeldItem(int item)
        {
            if (116 <= item && item <= 119)
                return item - 115;
            return 0;
        }
    }
}