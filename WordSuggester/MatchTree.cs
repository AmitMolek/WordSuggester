using System;
using System.Collections.Generic;
using System.Text;
using WordSuggester.dao;

/*
BK-Tree you can read more on it here:
    https://en.wikipedia.org/wiki/BK-tree
    https://www.geeksforgeeks.org/bk-tree-introduction-implementation/
*/

namespace WordSuggester {
    class MatchTree<T> {
        // The root of the tree
        public BKNode<T> root = null;

        public MatchTree() { }

        // Adds term to the tree
        public void add(T term) {
            if (root != null) {
                root.add(term);
            } else {
                root = new BKNode<T>(term);
            }
        }

        // Adds a list of terms to the tree
        public void add(List<T> lst) {
            foreach (T obj in lst) this.add(obj);
        }

        // Clears the tree
        public void clear() {
            root.children.Clear();
            root = null;
        }

    }
}
