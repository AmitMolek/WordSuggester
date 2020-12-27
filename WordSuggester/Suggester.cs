using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester {
    class Suggester {

        MatchTree<string> matchTree;

        SearchQueue<string> searchQueue;

        InputHandler inputHandler;

        Task awakeTask;
        Task startTask;
        Task updateTask;
        Task drawTask;
        int drawTaskSleep = 250;

        ManualResetEvent mainThreadEvent;

        const string dictionaryFilePath = "words.txt";

        public Suggester(ManualResetEvent mainThreadEvent) {
            this.mainThreadEvent = mainThreadEvent;
            awakeTask = Task.Factory.StartNew(() => { Awake(); });
        }

        // Called first on creating the Suggester object, run once
        // Used for first initialization
        private void Awake() {
            inputHandler = new InputHandler();
            startTask = Task.Factory.StartNew(() => { Start(); });
        }

        // Called after Awake is done, run once
        // Used for second initialization
        private async void Start() {
            matchTree = SuggesterHelper.InitBKTree(dictionaryFilePath);
            //CancellationTokenSource cts = new CancellationTokenSource();
            //
            //var searchTask = matchTree.root.SearchSubTree("hello", 3, cts.Token);
            //Dictionary<string, int> foundMatches = await searchTask;
            //
            //foreach (KeyValuePair<string, int> pair in foundMatches) {
            //    Console.WriteLine("    - {0}", pair.Key);
            //}
            searchQueue = new SearchQueue<string>();
            drawTask = Task.Factory.StartNew(() => { while (true) {
                    Draw();
                    Thread.Sleep(drawTaskSleep);
            }});
            updateTask = Task.Factory.StartNew(() => { while (true) Update(); });
        }

        // Run continuously, used for updating
        private void Update() {
            if (inputHandler.GetInputFromConsole()) {
                string input = inputHandler.GetInputString();
                SearchTask<string> newSearch = new SearchTask<string>(matchTree.root, input, (input.Length / 2) + 1);
                searchQueue.StartNewSearch(newSearch);
                Draw();
            }
        }

        // Run continuously, used for drawing to the console
        private void Draw() {
            Console.Clear();
            Console.WriteLine("Input: {0}", inputHandler.GetInputString());
            Dictionary<string, int> searchResult;
            searchQueue.TryGetResult(out searchResult);
            for (int i = 0; i < 10; i++) {
                if (i < searchResult.Keys.Count)
                    Console.WriteLine("     - {0}", searchResult.Keys.ToList()[i]);
            }
            if (searchResult.Keys.Count > 0)
                Console.WriteLine("     - ...");

        }

    }
}
