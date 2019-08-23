using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester {
    class Suggester {

        MatchTree<string> matchTree;

        Task awakeTask;
        Task startTask;
        Task updateTask;
        Task drawTask;

        ManualResetEvent mainThreadEvent;

        const string dictionaryFilePath = "words.txt";

        public Suggester(ManualResetEvent mainThreadEvent) {
            this.mainThreadEvent = mainThreadEvent;
            awakeTask = Task.Factory.StartNew(() => { Awake(); });
        }

        // Called first on creating the Suggester object, run once
        // Used for first initialization
        private void Awake() {
            startTask = Task.Factory.StartNew(() => { Start(); });
        }

        // Called after Awake is done, run once
        // Used for second initialization
        private async void Start() {
            matchTree = SuggesterHelper.InitBKTree(dictionaryFilePath);
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.WriteLine("1");
            var searchTask = matchTree.root.SearchSubTree("hello", 3, cts.Token);
            Dictionary<string, int> foundMatches = await searchTask;
            Console.WriteLine("2");
            //Task.WaitAll(searchTask);
            foreach (KeyValuePair<string, int> pair in foundMatches) {
                Console.WriteLine("    - {0}", pair.Key);
            }
            Console.WriteLine("3");
        }

        // Run continuously, used for updating
        private void Update() { }

        // Run continuously, used for drawing to the console
        private void Draw() { }

    }
}
