using System;
using System.Collections.Generic;
using System.Text;

namespace WordSuggester {
    class SearchQueue<T> {
        SearchTask<T> prevSearch;
        SearchTask<T> currentSearch;

        public SearchQueue() {
            prevSearch = null;
            currentSearch = null;
        }

        public void StartNewSearch(SearchTask<T> searchTask) {
            prevSearch = currentSearch;
            if (prevSearch != null)
                prevSearch.CancelSearch();
            currentSearch = searchTask;
            currentSearch.StartSearch();
        }

        public bool TryGetResult(out Dictionary<T, int> result) {
            if (currentSearch != null) {
                if (currentSearch.IsSearchDone()) {
                    result = currentSearch.GetResult();
                    return true;
                }
            }
            result = new Dictionary<T, int>();
            return false;
        }

    }
}
