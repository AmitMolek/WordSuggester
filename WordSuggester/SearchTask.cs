using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WordSuggester.dao;

namespace WordSuggester {
    class SearchTask<T> {

        T searchTerm;
        int searchThreshold;

        BKNode<T> bkRoot;

        Dictionary<T, int> result;

        CancellationTokenSource cts;

        Task<Dictionary<T, int>> searchTask;
        Task waitTask;

        public SearchTask(BKNode<T> root, T searchTerm, int searchThreshold) {
            bkRoot = root;
            this.searchTerm = searchTerm;
            this.searchThreshold = searchThreshold;
            cts = new CancellationTokenSource();
            result = new Dictionary<T, int>();
        }

        public void StartSearch() {
            waitTask = Task.Factory.StartNew(async () => {
                searchTask = bkRoot.SearchSubTree(searchTerm, searchThreshold, cts.Token);
                try {
                    result = await searchTask;
                } catch (OperationCanceledException oce) { }
            });
        }

        public Dictionary<T, int> GetResult() {
            return result;
        }

        public bool IsSearchDone() {
            return (waitTask.IsCompleted);
        }

        public void CancelSearch() {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }
    }
}
