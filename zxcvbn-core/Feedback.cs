using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// Generates feedback based on the match results.
    /// </summary>
    internal static class Feedback
    {
        private static readonly FeedbackItem DefaultFeedback = new FeedbackItem
        {
            Warning = string.Empty,
            Suggestions = new[]
            {
                // "Use a few words, avoid common phrases",
                // "No need for symbols, digits, or uppercase letters",
                "Utilisez quelques mots",
                "L'utilisation de symboles, chiffres ou des lettres capitales n'est pas obligatoire.",
            },
        };

        /// <summary>
        /// Gets feedback based on the provided score and matches.
        /// </summary>
        /// <param name="score">The score to assess.</param>
        /// <param name="sequence">The sequence of matches to assess.</param>
        /// <returns>Any warnings and suggestiongs about the password matches.</returns>
        public static FeedbackItem GetFeedback(int score, IEnumerable<Match> sequence)
        {
            if (!sequence.Any())
                return DefaultFeedback;

            if (score > 2)
            {
                return new FeedbackItem
                {
                    Warning = string.Empty,
                    Suggestions = new List<string>(),
                };
            }

            var longestMatch = sequence.OrderBy(c => c.Token.Length).Last();

            var feedback = GetMatchFeedback(longestMatch, sequence.Count() == 1);
            // var extraFeedback = "Add another word or two.  Uncommon words are better.";
            var extraFeedback = "Ajoutez un mot ou deux. Les mots peu communs sont préférables";

            if (feedback != null)
            {
                feedback.Suggestions.Insert(0, extraFeedback);
            }
            else
            {
                feedback = new FeedbackItem
                {
                    Warning = string.Empty,
                    Suggestions = new List<string> { extraFeedback },
                };
            }

            return feedback;
        }

        private static FeedbackItem GetDictionaryMatchFeedback(DictionaryMatch match, bool isSoleMatch)
        {
            var warning = string.Empty;

            if (match.DictionaryName == "passwords"
            || match.DictionaryName == "fr-password")
            {
                if (isSoleMatch && !match.L33t && !match.Reversed)
                {
                    if (match.Rank <= 10)
                        // warning = "This is a top-10 common password";
                        warning = "Ce mot de passe fait parti des 10 les plus utilisé";
                    else if (match.Rank <= 100)
                        // warning = "This is a top-100 common password";
                        warning = "Ce mot de passe fait parti des 100 les plus utilisé";
                    else
                        // warning = "This is a very common password";
                        warning = "Ce mot de passe est trés commun";
                }
                else if (
                    (match.DictionaryName == "english"
                     || match.DictionaryName == "fr" ) && isSoleMatch)
                {
                    // warning = "A word by itself is easy to guess";
                    warning = "Un mot, seul, est trés simple à deviner";
                }
                else if (match.DictionaryName == "surnames"
                         || match.DictionaryName == "male_names"
                         || match.DictionaryName == "female_names"
                         || match.DictionaryName == "nom_femme"
                         || match.DictionaryName == "nom_homme"           )
                {
                    if (isSoleMatch)
                        // warning = "Names and surnames by themselves are easy to guess";
                        warning = "Les noms propres sont trés simples à deviner";
                    else
                        // warning = "Common names and surnames are easy to guess";
                        warning = "Les noms propres sont trés simples à deviner";
                }
            }

            var suggestions = new List<string>();
            var word = match.Token;
            if (char.IsUpper(word[0]))
                // suggestions.Add("Capitalization doesn't help very much");
                suggestions.Add("Les lettres capitales n'avancent en rien.");
            else if (word.All(c => char.IsUpper(c)) && word.ToLower() != word)
                // suggestions.Add("All-uppercase is almost as easy to guess as all-lowercase");
                suggestions.Add("Tout en lettres capitales est aussi simple à deviner que tout en lettres minuscules");

            if (match.Reversed && match.Token.Length >= 4)
                // suggestions.Add("Reversed words aren't much harder to guess");
                suggestions.Add("Les mots inversés ne sont pas difficiles à deviner");
            if (match.L33t)
                // suggestions.Add("Predictable substitutions like '@' instead of 'a' don't help very much");
                suggestions.Add("Les substitutions simples comme un '@' à la place d'un 'a' sont simples à deviner");

            return new FeedbackItem
            {
                Suggestions = suggestions,
                Warning = warning,
            };
        }

        private static FeedbackItem GetMatchFeedback(Match match, bool isSoleMatch)
        {
            switch (match.Pattern)
            {
                case "dictionary":
                    return GetDictionaryMatchFeedback(match as DictionaryMatch, isSoleMatch);

                case "spatial":
                    return new FeedbackItem
                    {
                        // Warning = (match as SpatialMatch).Turns == 1 ? "Straight rows of keys are easy to guess" : "Short keyboard patterns are easy to guess",
                        Warning = (match as SpatialMatch).Turns == 1 ? "Les séquences sur le clavier sont simples à deviner" : "Les séquences de touches adjacentes sur le clavier sont simples à deviner",
                        Suggestions = new List<string>
                        {
                            // "Use a longer keyboard pattern with more turns",
                            "Utilisez une séquence plus longues avec plus de variation",
                        },
                    };

                case "repeat":
                    return new FeedbackItem
                    {
                        // Warning = (match as RepeatMatch).BaseToken.Length == 1 ? "Repeats like 'aaa' are easy to guess" : "Repeats like 'abcabcabc' are only slightly harder to guess than 'abc'",
                        Warning = (match as RepeatMatch).BaseToken.Length == 1 ? "Les répétitions de lettres comme 'aaa' sont simples à deviner" : "Les répétitions de lettres comme 'abcabcabc' ne sont pas beaucoup plus dure à deviner que 'abc'",
                        Suggestions = new List<string>
                        {
                            // "Avoid repeated words and characters",
                            "Évitez les répétitions de mots ou de lettres",
                        },
                    };

                case "regex":
                    if ((match as RegexMatch).RegexName == "recent_year")
                    {
                        return new FeedbackItem
                        {
                            // Warning = "Recent years are easy to guess",
                            Warning = "Les années récentes sont simples à deviner",
                            Suggestions = new List<string>
                            {
                                // "Avoid recent years",
                                // "Avoid years that are associated with you",
                                "Évitez les années récentes",
                                "évitez les années qui se rapportent à vous ou votre famille",
                            },
                        };
                    }

                    break;

                case "date":
                    return new FeedbackItem
                    {
                        // Warning = "Dates are often easy to guess",
                        Warning = "Les dates sont trop simples à deviner",
                        Suggestions = new List<string>
                        {
                            // "Avoid dates and years that are associated with you",
                            "Évitez les dates et années qui se rapportent à vous ou votre famille",
                        },
                    };
            }
            return null;
        }
    }
}
