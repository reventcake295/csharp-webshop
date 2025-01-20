namespace Store.Ui;

internal static class UiHelper
{
    /// <summary>
    /// Ask the user a question with a group of choices available that the user may select and auto formats the response to the given format
    /// </summary>
    /// <param name="questId">The questionId</param>
    /// <param name="answer">Out parameter. The option that was chosen in the given type format</param>
    /// <param name="defaultValue">The default value to give back when no actual input is given in the end</param>
    /// <param name="choices">Optional. List of options that the user may choose. Default: negated</param>
    /// <param name="optional">Optional. Should this Question be optional? Default: false</param>
    /// <param name="hidden">Optional. Should the input be shown in the interface? Default: true</param>
    /// <param name="min">Optional. The minimum length of the response. Default: negated</param>
    /// <param name="max">Optional. The maximum length of the response. Default: negated</param>
    /// <param name="caseSensitive">Optional. Best combined with the param choices for the matching of strings Default: true</param>
    /// <typeparam name="T">The type to format too</typeparam>
    /// <returns>Returns true if successful, false otherwise</returns>
    internal static bool AskQuestion<T>(string questId, out T answer, T defaultValue, List<T>? choices = null, bool optional = false, bool hidden = false, int min = 0, int max = 0, bool caseSensitive = true)
    {
        // This function used to be two and later when more functionality was getting required became longer and longer, at the point right before combining the methods I had 5 methods taking up a total of more than 200 lines,
        // right after combining them it was exactly 100 lines in total, extra options that were present then were: <T>, choices, optional, hidden, min, max, although <T> type of enums was not working then, there was also no bool matching present
        answer = defaultValue;
        for (int i = 0; i < Settings.MaxInputLoop; i++)
        {
            T responseCast;
            Console.Write($"{Lang.GetSpecificOrDefaultString(questId, Lang.StringType.Question)}: ");
            string? response = !hidden ? Console.ReadLine() : AskSecret();
            if (string.IsNullOrWhiteSpace(response))
            {
                if (optional) return true;
                Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionEmpty));
                continue;
            }
            if (!caseSensitive) response = response.ToLower();
            // because they are optional parameters, there must also be a test if they are not set to 0 alongside the lenght check
            if (min != 0 && !(response.Length >= min))
            {
                Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionWrong));
                continue;
            }

            if (max != 0 && !(response.Length <= max))
            {
                Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionWrong));
                continue;
            }

            // if the type is bool, then try to match the response to the preset list, otherwise use response
            if (typeof(T) == typeof(bool))
                response = response.ToLower() switch
                {
                    "y" or "yes" or "true" or "on"  => bool.TrueString,
                    "n" or "false" or "off" or "no" => bool.FalseString,
                    _                               => response
                };
            // if it is a decimal, then ensure that when either a ',' or '.'
            // are used as second or 3rd last place,
            // then ensure that it is an actual decimal parsed that way
            else if (typeof(T) == typeof(decimal))
            {
                // first transform all '.' to ',' to ensure that all instances that can cause problems are accounted for
                response = response.Replace(".", ",");
                // see if the string contains at least a ','
                if (response.Contains(','))
                {
                    // split the string into its separate parts
                    string[] responses = response.Split(',');
                    // and then cast the string back into a whole in sequence
                    string responseCat = "";
                    // if the length of the array is more than two, then we need
                    // to first combine the parts up to the second last one
                    if (responses.Length > 2)
                        for (int it = 0; it < (responses.Length - 1); it++)
                            responseCat += responses[it];
                    // and then add the last part after adding in a '.' because it is a decimal
                    response = responseCat + '.' + responses[^1];
                }
            }

            // To convert the response string to the given type.
            // I would like to add the exception for string when it is one.
            // However, I can't figure out how to do that without a lot of problems popping up
            // because it doesn't recognize an if comparison as a valid check for it
            try
            {
                if (typeof(T).IsEnum)
                    responseCast = (T)Enum.Parse(typeof(T), response, ignoreCase: true);
                else
                    responseCast = (T)Convert.ChangeType(response, typeof(T));
            }
            catch (Exception e) when (e is InvalidOperationException
                                          or InvalidCastException // InvalidCastException — This conversion is not supported.
                                          // -or- value is null and conversionType is a value type.
                                          // -or- value does not implement the IConvertible interface.
                                          or FormatException // FormatException — value is not in a format recognized by conversionType.
                                          or OverflowException // OverflowException — value represents a number that is out of the range of conversionType.
                                          or ArgumentNullException) // ArgumentNullException — conversionType is null
            {
                Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionWrong));
                continue;
            }
            if (choices != null) 
            {
                if (choices.Contains(responseCast))
                {
                    answer = responseCast;
                    return true;
                }
                Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionWrong));
            }
            else
            {
                answer = responseCast;
                return true;
            }
        }
        Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, Lang.StringType.QuestionMaxLoop));
        return false;
    }

    /// <summary>
    /// Gets a line of input from the user without displaying what's entered on screen  
    /// </summary>
    /// <returns>Returns the string that has been entered</returns>
    private static string AskSecret()
    {
        string input = "";
        ConsoleKeyInfo key;
        do {
            // To prevent the input from being read on the screen, we do not use Console.ReadLine() But Console.ReadKey(true)
            // this also causes the debug mode to crash when set to vscode debugConsole due to redirected input,
            // set the following to such: launch.json:"configurations": ["console": "integratedTerminal"] to prevent problems
            key = Console.ReadKey(true);
            if (ConsoleKey.Backspace == key.Key) {
                if (input.Length >= 1)
                    input = input[..^1];
                continue;
            }
            // ensure that none of the keys that will cause problems do not get entered into the password string
            if ((key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt ||
                (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control ||
                key.KeyChar == '\u0000' ||
                key.KeyChar.GetHashCode() == 0||
                key.Key == ConsoleKey.Enter)
                continue;
            input += key.KeyChar;
            // If The Enter key is used, that means the input is finished
        } while (key.Key != ConsoleKey.Enter);
        Console.Write("\n");
        return input;
    }

    internal static void DisplayResult(string questId, Lang.StringType stringType)
    {
        Console.WriteLine(Lang.GetSpecificOrDefaultString(questId, stringType));
    }
    internal static void DisplayOption(string key, string value)
    {
        Console.WriteLine($"    {key}: {Lang.GetLangString(value)}");
    }

    internal static void DisplayCustomOption(string key, string value)
    {
        Console.WriteLine($"    {key}: {value}");
    }

    internal static void DisplayMenu(Dictionary<string, UiItem> menu, string menuId)
    {
        Console.WriteLine("\n" + Lang.GetSpecificOrDefaultString(menuId, Lang.StringType.Header) + "\n");
        List<string> activeItems = [];
        activeItems.AddRange(from item in menu
                             where item.Value.DisplayItem(item.Key)
                             select item.Key);
        Console.WriteLine("");
        if (!AskQuestion(menuId, out string answer, "", choices:activeItems)) return;
        if (string.IsNullOrWhiteSpace(answer)) return;
        Console.WriteLine();
        menu[answer].Execute();
    }

    
}