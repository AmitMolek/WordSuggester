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

        public SearchObject(BKNode<T> treeRoot) {
            this.treeRoot = treeRoot;
        }

        private void initSearch() {
            cts = new CancellationTokenSource();
            matches = new Dictionary<T, int>();
            matchesLock = new object();
        }

        public void StartSearch(T input, int threshold) {
            waitTask = Task.Factory.StartNew(() => {
                initSearch();
                searchTask = Task.Factory.StartNew(() => {
                    if (treeRoot != null)
                        treeRoot.SearchMatches(input, threshold, matches, matchesLock, cts.Token);
                });
                Task.WaitAll(searchTask);
            });
        }

        // Cancels the search
        public void CancelSearch() {
            if (cts != null) {
                cts.Cancel();
            }
        }
    }
}
