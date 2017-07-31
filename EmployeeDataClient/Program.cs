using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataClient.Exceptions;
using EmployeeDataClient.Types;

namespace EmployeeDataClient
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            InitialMessage();

            while (true)
            {
                try
                {
                    Write(">");
                    var command = Console.ReadLine();
                    if (string.IsNullOrEmpty(command))
                        continue;
                    var commandRecognized = false;
                    FunctionsList.ForEach(f =>
                    {
                        if (f.Handle(command))
                            commandRecognized = true;
                    });
                    if (!commandRecognized)
                        Wline("Unrecognized command. Type \"help\" to get list of available commands.");
                }
                catch (BreakException ex)
                {
                    Wline("Operation aborted");
                }
                catch (Exception ex)
                {
                    Wline(ex.Message);
                }
            }
        }

        private static Token _token;

        private static readonly List<Functions> FunctionsList = new List<Functions>
        {
            new Functions(GlobalComands.HELP, Help),
            new Functions(GlobalComands.LOG_IN, LogIn),
            new Functions(GlobalComands.SIGN_UP, SignUp),
            new Functions(GlobalComands.ALL, All),
            new Functions(GlobalComands.ADD, Add),
            new Functions(GlobalComands.ADD_PROFESSION, AddProfession),
            new Functions(GlobalComands.FIND, Find),
            new Functions(GlobalComands.UPDATE, Update)
        };

    }


    public delegate void Temp();

    internal class Functions
    {

        public Functions(string command, Temp del)
        {
            _command = command;
            _del = del;
        }

        public bool Handle(string command)
        {
            if (command.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
            {
                _del.Invoke();
                return true;
            }
            return false;
        }

        private string _command;
        private Temp _del;
    }

    public static class GlobalComands
    {
        public const string HELP = "help";

        public const string LOG_IN = "login";

        public const string SIGN_UP = "signup";

        public const string ALL = "all";

        public const string ADD = "add";

        public const string ADD_PROFESSION = "addprof";

        public const string FIND = "find";

        public const string UPDATE = "update";

    }

}
