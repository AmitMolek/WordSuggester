using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester.dao {
    class BKNode<T> {
        // The data we want to hold
        T term;
        // Holds all the children of this node, can be N children
        public Dictionary<int, BKNode<T>> children;

        // CTOR
        public BKNode(T term) {
            this.term = term;
            children = new Dictionary<int, BKNode<T>>();
        }

        // Adds a child to this node
        // First calculates the distance (D) between this node and the one we want to add
        // Then 2 options:
        // if no child with D distance exist, add it to this node as a child
        // if this node already has a child with D distance, add it to the child with
        // the same D distance
        public void add(T term) {
            int score = Distance<T>.calculate(term, this.term);

            BKNode<T> child = null;
            children.TryGetValue(score, out child);
            if (child != null) {
                child.add(term);
            } else {
                children.Add(score, new BKNode<T>(term));
            }
        }

        // Searches for terms that are similar to the given term (Recursive function)
        // Searches the children for nodes that their distance is smaller than threshold
        // term: The given term we want to search similar to
        // threshold: The maximum distance we want to search for
        // collected: Stores all the nodes that fit
        // ct: Cancellation Token for the Task
        public void SearchMatches(T term, int threshold, Dictionary<T, int> matches, object collectionLock,CancellationToken ct) {
            // Calculates the distance of the given term from this node's term
            int distanceFromNode = Distance<T>.calculate(this.term, term);

            // Checks if the distance is within the given thershold
            if (distanceFromNode <= threshold) {
                // Lock the collection to prevent collisions
                lock (collectionLock) {
                    if (!matches.ContainsKey(this.term))
                        matches.Add(this.term, distanceFromNode);
                }
            }

            // Search for matches in the children
            for (int score = distanceFromNode - threshold; score <= threshold + distanceFromNode; score++) {
                BKNode<T> child = children.GetValueOrDefault(score);
                if (child != null) {
                    ct.ThrowIfCancellationRequested();
                    child.SearchMatches(term, threshold, matches, collectionLock, ct);
                }

            }
        }
    }
}
