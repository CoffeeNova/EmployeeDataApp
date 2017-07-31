using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeDataClient.Exceptions;
using EmployeeDataClient.Extensions;
using Newtonsoft.Json;

namespace EmployeeDataClient
{
    public partial class Program
    {
        private static void GlobalCommands(string command)
        {
            CheckExit(command);
            CheckBreak(command);
            CheckHelp(command);
        }

        private static void CheckExit(string exitStr)
        {
            if (exitStr.EqualsAny(StringComparison.InvariantCultureIgnoreCase, "/q", "quit", "exit"))
            {
                Console.WriteLine("Push any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void CheckBreak(string helpStr)
        {
            if (helpStr
                .EqualsAny(StringComparison.InvariantCultureIgnoreCase, "/b", "break", "/break"))
                throw new BreakException();
        }

        private static void CheckHelp(string breakStr)
        {
            if (breakStr.EqualsAny(StringComparison.InvariantCultureIgnoreCase, "/h", "help", "/help"))
            {
                Help();
                throw new BreakException();
            }
        }

        private static void GlobalCommands(ConsoleKey key)
        {
            CheckExit(key);
            CheckBreak(key);
            CheckHelp(key);
        }

        private static void CheckExit(ConsoleKey exitKey)
        {
            if (exitKey.Equals(ConsoleKey.Q))
            {
                Console.WriteLine("Push any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void CheckBreak(ConsoleKey breakKey)
        {
            if (breakKey.Equals(ConsoleKey.B))
                throw new BreakException();
        }

        private static void CheckHelp(ConsoleKey helpKey)
        {

            if (helpKey.Equals(ConsoleKey.H))
            {
                Help();
                throw new BreakException();
            }
        }

        private static void Wline(string text)
        {
            Console.WriteLine(text);
        }

        private static void Write(string text)
        {
            Console.Write(text);
        }

        private static string StringFunction(string actionDescription = null, string wrongDataAnswer = null, 
            string successfulAnswer = null, Func<string, bool> validation = null, string defaultValue = null)
        {
            string result;
            while (true)
            {
                Write(actionDescription);
                SendKeys.SendWait(defaultValue);
                var answer = Console.ReadLine();
                //Console.WriteLine();
                GlobalCommands(answer);
                if (validation != null && !validation(answer))
                {
                    if (wrongDataAnswer != null)
                        Wline(wrongDataAnswer);
                }
                else
                {
                    if (successfulAnswer != null)
                        Wline(successfulAnswer);
                    result = answer;
                    break;
                }
            }
            return result;
        }

        private static string KeyFunction(Dictionary<ConsoleKey, string> keyActions, string actionDescription = null, 
            string wrongDataAnswer = null, string successfulAnswer = null, string defValueDescription = null)
        {
            if (keyActions == null)
                throw new ArgumentNullException(nameof(keyActions));

            Wline(actionDescription);
            ConsoleKeyInfo answerKey;
            while (true)
            {
                Wline(defValueDescription);
                answerKey = Console.ReadKey(false);
                Console.WriteLine();
                if (answerKey.Key.EqualsAny(keyActions.Select(k => k.Key).ToList()))
                    break;
                GlobalCommands(answerKey.Key);
                if (wrongDataAnswer != null)
                    Console.Write(wrongDataAnswer);
            }
            if (successfulAnswer != null)
                Console.Write(successfulAnswer);
            return keyActions.Single(k => k.Key == answerKey.Key).Value;
        }

        private static bool NameValidator(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (!IsStringOnlyLetters(name))
                return false;
            return true;
        }

        private static bool AgeValidator(string age)
        {
            if (string.IsNullOrEmpty(age))
                return true;
            int ageInt;
            if (int.TryParse(age, out ageInt))
                return IsAgeInRange(ageInt);
            return false;
        }

        private static bool ProfessionsValidator(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            var profArr = value.Split(new string[] {",  ", ", ", ","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var prof in profArr)
            {
                if (!IsStringOnlyLetters(prof))
                    return false;
            }
            return true;
        }

        private static bool IdValidator(string id)
        {
            if (string.IsNullOrEmpty(id))
                return true;
            int idInt;
            if (int.TryParse(id, out idInt))
                return idInt > 0;
            return false;
        }

        private static bool IsStringOnlyLetters(string str)
        {
            var pattern = @"^[a-zA-Z]+$";
            var regex = new Regex(pattern);
            return regex.IsMatch(str);
        }

        private static bool IsAgeInRange(int age)
        {
            return age >= 14 && age <= 99;
        }

        private static bool IsAgeInRange(string age, out int ageInt)
        {
            if (int.TryParse(age, out ageInt))
                return IsAgeInRange(ageInt);
            return false;
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private class Content
        {
            public void Add(string name, string value)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (Json)
                {
                    JsonData.Add(name, value);
                    return;
                }
                Data.Add(new KeyValuePair<string, string>(name, value));
            }

            public List<KeyValuePair<string, string>> Data = new List<KeyValuePair<string, string>>();

            public Dictionary<string, string> JsonData = new Dictionary<string, string>();

            public static implicit operator List<KeyValuePair<string, string>>(Content obj)
            {
                return obj.Data;
            }

            public static implicit operator Dictionary<string, string>(Content obj)
            {
                return obj.JsonData;
            }

            public bool Json { get; set; }
        }
    }
}
