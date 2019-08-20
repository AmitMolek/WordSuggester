using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester {
    class InputHandler {

        MatchTree<string> bkTree;

        StringBuilder inputString;
        object inputStringLock;

        Task getInputTask;
        Task printInputTask;
        Task printResultTask;
        Task searchObjTask;

        SearchObject<string> searchObject;

        ManualResetEvent getInputEvent;

        const string dictionaryFilePath = "words.txt";

        public InputHandler() {
            inputString = new StringBuilder();
            inputStringLock = new object();
            bkTree = new MatchTree<string>();
            getInputEvent = new ManualResetEvent(false);
        }

        public void HandleInput() {
            List<Task> tasksToWait = new List<Task>();

            Task initTreeTask = Task.Factory.StartNew(() => { InitBKTree(); });
            Task.WaitAll(initTreeTask);

            while (true) {
                ConsoleKeyInfo cki = new ConsoleKeyInfo();
                tasksToWait.Clear();
                getInputTask = Task.Factory.StartNew(() => { GetInputFromConsole(cki); });
                printInputTask = Task.Factory.StartNew(() => { PrintInput(); });
                searchObjTask = Task.Factory.StartNew(() => { StartSearch(); });
                printResultTask = Task.Factory.StartNew(() => { PrintResult(); });
                //tasksToWait.Add(getInputTask);
                tasksToWait.Add(printInputTask);
                tasksToWait.Add(printResultTask);
                Task.WaitAll(tasksToWait.ToArray());
            }
        }

        private void InitBKTree() {
            try {
                Console.WriteLine("Started reading dictionary file.");
                foreach (string line in File.ReadLines(dictionaryFilePath)) {
                    bkTree.add(line);
                }
                Console.WriteLine("Finished reading dictionary file.");
            } catch(IOException ioe) {
                Console.WriteLine("Something went wrong reading the input file.");
            }
        }

        // Prints the new input to the console
        private void PrintInput() {
            //Task.WaitAll(getInputTask);
            getInputEvent.WaitOne();
            Console.Clear();
            lock (inputStringLock) {
                Console.WriteLine("Input: {0}", inputString.ToString());
            }
        }

        private void StartSearch() {
            //Task.WaitAll(getInputTask);
            getInputEvent.WaitOne();
            if (searchObject != null)
                searchObject.CancelSearch();
            searchObject = new SearchObject<string>(bkTree.root);
            searchObject.StartSearch(inputString.ToString(), inputString.Length / 2);
            Task.WaitAll(searchObject.waitTask);
        }

        private void PrintResult() {
            Task.WaitAll(searchObjTask);
            List<string> matchesValues = new List<string>();
            if (searchObject.waitTask.IsCompletedSuccessfully) {
                if (searchObject.matches != null)
                    matchesValues = searchObject.matches.Keys.ToList();
            }
            for (int i = 0; i < 3; i++) {
                if (i < matchesValues.Count) {
                    string similarTerm = matchesValues[i];
                    Console.WriteLine("- {0}", similarTerm);
                }
            }
        }

        // Updates the inputString based on the input from the console
        private void GetInputFromConsole(ConsoleKeyInfo cki) {
            while (true) {
                if (Console.KeyAvailable)
                    if (searchObject != null)
                        searchObject.CancelSearch();

                cki = Console.ReadKey(true);
                getInputEvent.Reset();
                if (cki.Key == ConsoleKey.Delete || cki.Key == ConsoleKey.Backspace) {
                    lock (inputStringLock) {
                        if (inputString.Length > 0)
                            inputString.Remove(inputString.Length - 1, 1);
                    }
                } else {
                    lock (inputStringLock) {
                        inputString.Append(cki.KeyChar);
                    }
                }
                getInputEvent.Set();
            }
        }

    }
}
