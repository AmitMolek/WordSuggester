using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester {
    class InputHandler {
        StringBuilder inputString;
        //object inputStringLock;
        ConsoleKeyInfo cki;

        public InputHandler() {
            inputString = new StringBuilder();
        }

        public string GetInputString() {
            return inputString.ToString();
        }

        // Updates the inputString based on the input from the console
        public bool GetInputFromConsole() {
            if (!Console.KeyAvailable) return false;

            cki = Console.ReadKey(true);

            if (cki.Key == ConsoleKey.Delete || cki.Key == ConsoleKey.Backspace) {
                //lock (inputStringLock) {
                    if (inputString.Length > 0)
                        inputString.Remove(inputString.Length - 1, 1);
                //}
            } else {
                //lock (inputStringLock) {
                    inputString.Append(cki.KeyChar);
                //}
            }
            return true;
            //StartSearch();
        }

    }
}
