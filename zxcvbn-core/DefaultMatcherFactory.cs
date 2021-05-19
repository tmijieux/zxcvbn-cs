using System.Collections.Generic;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// Creates the default matchers.
    /// </summary>
    internal static class DefaultMatcherFactory
    {
        private static readonly IEnumerable<IMatcher> BuiltInMatchers = BuildBuiltInMatchers();

        /// <summary>
        /// Gets all of the built in matchers, as well as matchers for the custom dictionaries.
        /// </summary>
        /// <param name="userInputs">Enumerable of user information.</param>
        /// <returns>Enumerable of matchers to use.</returns>
        public static IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            var userInputDict = new DictionaryMatcher("user_inputs", userInputs);
            var leetUser = new L33tMatcher(userInputDict);

            return new List<IMatcher>(BuiltInMatchers) { userInputDict, leetUser };
        }

        private static IEnumerable<IMatcher> BuildBuiltInMatchers()
        {
            var files = new List<string>
            {
                "fr-passwords",
                "fr",
                "nom_homme",
                "nom_femme",

                "passwords",
                "english",
                "male_names",
                "female_names",
                "surnames",
                "us_tv_and_film",
            };

            var dictionaryMatchers = new List<IMatcher>();
            foreach (var s in files)
            {
                dictionaryMatchers.Add(new DictionaryMatcher(s, $"{s}.lst.gz"));
            }

            foreach (var s in files)
            {
                dictionaryMatchers.Add(new ReverseDictionaryMatcher(s, $"{s}.lst.gz"));
            }

            return new List<IMatcher>(dictionaryMatchers)
            {
                new RepeatMatcher(),
                new SequenceMatcher(),
                new RegexMatcher("19\\d\\d|200\\d|201\\d", "recent_year"),
                new DateMatcher(),
                new SpatialMatcher(),
                new L33tMatcher(dictionaryMatchers),
            };
        }
    }
}
