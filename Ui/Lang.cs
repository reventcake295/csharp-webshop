using Microsoft.Extensions.Configuration;

namespace Store.Ui;

internal static class Lang
{
    private static Dictionary<string, LangCollection> _langMap;

    private static LangCollection _defaultStrings;
    static Lang()
    {
        _langMap = new();
        _defaultStrings = new("default");
        _loadLangMap("en_us");
    }
    
    internal static string GetLangString(string key)
    {
        return _langMap.TryGetValue(key, out LangCollection? result) ? result.GetString() : "";
    }

    internal static string GetLangGroupString(string key, StringType stringType)
    {
        return _langMap.TryGetValue(key, out LangCollection? result) ? result.GetGroupString(stringType) : "";
    }

    private static bool LangGroupExists(string key)
    {
        return _langMap.ContainsKey(key) && _langMap[key].HasGroupString();
    }

    internal static void ChangeLang(string lang)
    {
        _loadLangMap(lang);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="stringType"></param>
    /// <returns></returns>
    internal static string GetSpecificOrDefaultString(string key, StringType stringType)
    {
        // this is done with nesting to reduce the number of return statements that are required 
        if (LangGroupExists(key) && _langMap.TryGetValue(key, out LangCollection? result))
        {
            // try to acquire the string and see if an empty was returned or not, if not empty return the string
            string res = result.GetGroupString(stringType);
            if (!string.IsNullOrEmpty(res))
                return res;
        }
        // when the given group either does not exist or returns an empty string get the string from the default group
        return _defaultStrings.GetGroupString(stringType);
    }
    
    private static void _loadLangMap(string lang)
    {
        // ensure that the lang file is present
        if (!File.Exists($"{Directory.GetCurrentDirectory()}/lang_{lang}.json")) throw new FileNotFoundException($"Lang file {lang} not found in {Directory.GetCurrentDirectory()}");
        // acquire the file itself and the subsection of the language
        IConfigurationBuilder builder = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile($"lang_{lang}.json", optional: false);
        IConfiguration config = builder.Build();
        IConfigurationSection dbSection = config.GetRequiredSection(lang);
        // convert it into and IEnumerable for its children and start looping through them
        IEnumerable<IConfigurationSection> strings = dbSection.GetChildren();
        foreach (IConfigurationSection text in strings)
        {
            // acquire the default lang group and store it separately for usage 
            if (text.Key == "default")
            {
                _defaultStrings = new LangCollection(text.GetChildren());
                continue;
            }
            // sadly cannot combine the two below due to the fact that there are two separate checks required for each that must happen in sequence
            // first see if there are subSections to the current item if yes then give it as an IEnumerable to the LangCollection, if not continue
            if (text.GetChildren().Any())
            {
                _langMap.Add(text.Key, new LangCollection(text.GetChildren()));
                continue;
            } 
            // check if the value exist if not then continue to the next item, otherwise give it as an string to the LangCollection
            if (text.Value == null) continue;
            _langMap.Add(text.Key, new LangCollection(text.Value));
        }
    }

    private class LangCollection
    {
        private readonly string? _langString;
        private readonly Dictionary<StringType, LangCollection> _stringGroup = new();

        internal string GetString()
        {
            return _langString ?? "";
        }

        internal string GetGroupString(StringType stringType)
        {
            return _stringGroup.TryGetValue(stringType, out LangCollection? groupString) ? groupString.GetString() : "";
        }
        
        internal bool HasGroupString() => _stringGroup.Count != 0;
        
        internal LangCollection(string text)
        {
            _langString = text;
        }

        internal LangCollection(IEnumerable<IConfigurationSection> section)
        {
            foreach (IConfigurationSection text in section)
            {
                // at this point I am certain that there are no sub sections so all values are strings
                if (!Enum.TryParse(text.Key, out StringType stringType)) 
                    continue;
                if (text.Value != null) _stringGroup.Add(stringType, new LangCollection(text.Value));
            }
        }
        
    }

    internal enum StringType
    {
        Header,
        Question,
        QuestionMaxLoop,
        QuestionEmpty,
        QuestionWrong,
        ResultSuccess,
        ResultFailure,
        ResultNoMatch
    }
    
    
}