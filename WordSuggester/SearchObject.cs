using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WordSuggester.dao;

namespace WordSuggester {
    class SearchObject<T> {
        BKNode<T> bkRoot;

        Task<Dictionary<T, int>> searchTask;

        CancellationTokenSource cts;

        public SearchObject(BKNode<T> root) {
            bkRoot = root;
            cts = new CancellationTokenSource();
        }

        public Task<Dictionary<T, int>> GetSearchTask() {
            return searchTask;
        }

        public void InitSearch() {
            if (searchTask != null)
                if (!searchTask.IsCompleted) {
                    CancelSearch();
                    searchTask.Wait();
                }
        }

        public void StartSearch(T searchTerm, int threshold) {
            InitSearch();
            searchTask = bkRoot.SearchSubTree(searchTerm, threshold, cts.Token);
        }

        public async Task<Dictionary<T, int>> GetSearchResultAsync() {
            try {
                if (searchTask != null && searchTask.Status != TaskStatus.Canceled)
                    return await searchTask;
            } catch (OperationCanceledException oce) {
                Console.WriteLine("oPeRaTiOn CaNcElEd");
            }
            return new Dictionary<T, int>();
        }

        // Cancels the search
        public void CancelSearch() {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }
    }
}
