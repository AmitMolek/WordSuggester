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
        Task updaterTask;

        Task startTask;
        Task updateTask;
        Task drawTask;

        SearchObject<string> searchObject;

        ManualResetEvent getInputEvent;
        ManualResetEvent searchEvent;
        ManualResetEvent printInputEvent;
        AutoResetEvent updaterEvent;

        ConsoleWriter consoleWriter;
        ConsoleKeyInfo cki;

        const string dictionaryFilePath = "words.txt";

        public InputHandler() {
            inputString = new StringBuilder();
            inputStringLock = new object();
            bkTree = new MatchTree<string>();
            getInputEvent = new ManualResetEvent(false);
            searchEvent = new ManualResetEvent(false);
            printInputEvent = new ManualResetEvent(false);
            consoleWriter = new ConsoleWriter();
            updaterEvent = new AutoResetEvent(false);
            cki = new ConsoleKeyInfo();

            startTask = Task.Factory.StartNew(() => { Start(); });
            Task.WaitAll(startTask);
        }

        public void Start() {
            InitBKTree();

            updateTask = Task.Factory.StartNew(() => { while(true) Update(); });
            drawTask = Task.Factory.StartNew(() => { while(true) Draw(); });
            Task.WaitAll(updateTask);
        }

        public void Update() {
            //Console.WriteLine("Update");
            GetInputFromConsole(cki);
        }

        public void Draw() {
            //Console.WriteLine("Draw");
            PrintInput();
            //PrintResult();
            //consoleWriter.UpdateConsole();
            consoleWriter.PrintConsole();
        }

        public void HandleInput() {
            List<Task> tasksToWait = new List<Task>();
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            Task initTreeTask = Task.Factory.StartNew(() => { InitBKTree(); });
            Task.WaitAll(initTreeTask);

            //while (true) {
                tasksToWait.Clear();
                getInputTask = Task.Factory.StartNew(() => { GetInputFromConsole(cki); });
                printInputTask = Task.Factory.StartNew(() => { PrintInput(); });
                searchObjTask = Task.Factory.StartNew(() => { StartSearch(); });
                printResultTask = Task.Factory.StartNew(() => { PrintResult(); });
                updaterTask = Task.Factory.StartNew(() => { Update(); });
                tasksToWait.Add(getInputTask);
                tasksToWait.Add(printInputTask);
                tasksToWait.Add(printResultTask);
                Task.WaitAll(tasksToWait.ToArray());
            //}
        }

        private void InitBKTree() {
            int addedLines = 0;
            List<string> dictionaryFile = File.ReadLines(dictionaryFilePath).ToList();
            bool finishJob = false;
            Task writeToConsoleTask = Task.Factory.StartNew(() => {
                while (!finishJob) {
                    Console.WriteLine("Read {0}/{1}", addedLines, dictionaryFile.Count());
                }
            });
            try {
                Console.WriteLine("Started reading dictionary file.");
                foreach (string line in dictionaryFile) {
                    bkTree.add(line);
                    addedLines++;
                }
                finishJob = true;
                Console.Clear();
                Console.WriteLine("Finished reading dictionary file.");
            } catch(IOException ioe) {
                Console.WriteLine("Something went wrong reading the input file.");
            }
        }

        // Prints the new input to the console
        private void PrintInput() {
            //lock (inputStringLock) {
            if (Monitor.TryEnter(inputStringLock)) {
                consoleWriter.AddItem(String.Format("Input: {0}\n", inputString.ToString()), true);
                Monitor.Exit(inputStringLock);
            }
            //}
        }

        private void StartSearch() {
            if (searchObject != null && searchObject.searchTask.Status == TaskStatus.Running)
                searchObject.CancelSearch();
            searchObject = new SearchObject<string>(bkTree.root);
            searchObject.StartSearch(inputString.ToString(), inputString.Length);
            Task.WaitAll(searchObject.waitTask);
        }

        private async void SearchAsync() {
            await new SearchObject<string>(bkTree.root).StartSearchAsync(inputString.ToString(), inputString.Length);
        }

        private void PrintResult() {
            List<string> matchesValues = new List<string>();
            if (searchObject.waitTask.IsCompletedSuccessfully) {
                if (searchObject.matches != null)
                    matchesValues = searchObject.matches.Keys.ToList();

                for (int i = 0; i < 3; i++) {
                    if (i < matchesValues.Count) {
                        string similarTerm = matchesValues[i];
                        consoleWriter.AddItem(String.Format("{1} - {0}\n", similarTerm, searchObject.input), false);
                    }
                }
            }
        }

        // Updates the inputString based on the input from the console
        private void GetInputFromConsole(ConsoleKeyInfo cki) {
            if (!Console.KeyAvailable) return;

            if (searchObject != null)
                searchObject.CancelSearch();

            cki = Console.ReadKey(true);

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
            //StartSearch();
        }

    }
}
