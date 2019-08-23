using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WordSuggester.dao;

namespace WordSuggester {
    class SearchObject<T> {
        BKNode<T> treeRoot;
        public Task searchTask;
        public Task waitTask;
        CancellationTokenSource cts;
        public Dictionary<T, int> matches;
        public object matchesLock;
        public T input;

        public SearchObject(BKNode<T> treeRoot) {
            this.treeRoot = treeRoot;
        }

        private void initSearch(T input) {
            this.input = input;
            cts = new CancellationTokenSource();
            matches = new Dictionary<T, int>();
            matchesLock = new object();
        }

        public void StartSearch(T input, int threshold) {
            if (searchTask != null) {
                if (cts != null)
                    cts.Cancel();

                while (!searchTask.IsCompleted && !waitTask.IsCompleted) ;
            }

            waitTask = Task.Factory.StartNew(() => {
                initSearch(input);
                searchTask = Task.Factory.StartNew(() => {
                    if (treeRoot != null)
                        treeRoot.SearchMatches(input, threshold, matches, matchesLock, cts.Token);
                });
                Task.WaitAll(searchTask);
            });
        }

        public Task StartSearchAsync(T input, int threshold) {
            initSearch(input);
            searchTask = Task.Factory.StartNew(() => {
                if (treeRoot != null)
                    treeRoot.SearchMatches(input, threshold, matches, matchesLock, cts.Token);
            });
            return searchTask;
        }

        // Cancels the search
        public void CancelSearch() {
            if (cts != null) {
                cts.Cancel();
            }
        }
    }
}
