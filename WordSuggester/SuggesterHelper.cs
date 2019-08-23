using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace WordSuggester {
    class SuggesterHelper {

        // Returns a BK-Tree (String match tree)
        public static MatchTree<string> InitBKTree(string filePath) {
            bool finishedBuildingTree = false;
            int addedLines = 0;

            MatchTree<string> bkTree = new MatchTree<string>();

            try {
                // Reads the words dictionary file, eeach line contains different word
                string[] readLines = File.ReadAllLines(filePath);

                // Start a new Task to print to the console the progress of the building
                Task writeToConsoleTask = Task.Factory.StartNew(() => {
                    while (!finishedBuildingTree)
                        Console.WriteLine("Reading word dictionary {0}/{1}", addedLines, readLines.Length);
                });

                Console.WriteLine("Started building tree");
                foreach (string line in readLines) {
                    bkTree.add(line);
                    addedLines++;
                }
                finishedBuildingTree = true;
                Console.Clear();
                Console.WriteLine("Finished building tree");
            } catch (IOException ioe) {
                Console.WriteLine("Something went wrong reading the input file.");
                Console.WriteLine(ioe.Message);
            } catch (Exception e) {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(e.Message);
            }

            return bkTree;
        }

    }
}
