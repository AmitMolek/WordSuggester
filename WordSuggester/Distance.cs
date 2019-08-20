using System;
using System.Collections.Generic;
using System.Text;

namespace WordSuggester {
    class Distance<T> {
        // Returns the levenshtein distance between two strings
        // Implementing Dynamic Programming
        public static int calculate(T a, T b) {
            // if the inputs are not strings, than we dont want to calculate the distance between them
            if (a.GetType() != typeof(string) || b.GetType() != typeof(string)) return -1;

            string aStr = a.ToString();
            string bStr = b.ToString();

            // Checks if the words are not empty
            // if one of the words is empty then the distance is the length
            // of the other word
            if (String.IsNullOrEmpty(aStr)) {
                if (String.IsNullOrEmpty(bStr)) return 0;
                return bStr.Length;
            }
            if (String.IsNullOrEmpty(bStr)) return aStr.Length;

            // Making sure that bStr is the longest string
            // The lazy way I know... don't judge me :(
            if (aStr.Length > bStr.Length) {
                string temp = bStr;
                bStr = aStr;
                aStr = temp;
            }

            // Building 2-dim array (matrix) to store the distances
            int m = bStr.Length;
            int n = aStr.Length;
            int[,] distance = new int[2, m + 1];

            // Init the distance matrix
            for (int j = 1; j <= m; j++) {
                distance[0, j] = j;
            }

            // Going over the 2 strings and calculates the levenshtein distance
            // The basic version is that it checks the cost for 3 options
            // and choses the minimum cost:
            // 1: if we delete a char from the string
            // 2: if the replace a char from the string
            // 3: if we add a char to the string
            int currentRow = 0;
            for (int i = 1; i <= n; ++i) {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int pervRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++) {
                    int cost = (bStr[j - 1] == aStr[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                        distance[pervRow, j] + 1,
                        distance[currentRow, j - 1] + 1),
                        distance[pervRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }
    }
}
