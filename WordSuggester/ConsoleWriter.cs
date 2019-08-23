using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordSuggester {
    class ConsoleWriter {
        public List<string> toPrint;

        Task printTask;
        AutoResetEvent waitToPrint;

        public ConsoleWriter() {
            toPrint = new List<string>();
            waitToPrint = new AutoResetEvent(true);
            StartAgent();
        }

        public void UpdateConsole() {
            //Console.WriteLine("Updating console {0}, {1}, {2}", waitToPrint == null, printTask == null, toPrint.Count);
            if (waitToPrint != null)
                waitToPrint.Set();
        }

        public void AddItem(string item, bool insertToHead) {
            if (insertToHead)
                toPrint.Insert(0, item);
            else toPrint.Add(item);
        }

        public void ClearList() {
            if (toPrint != null)
                toPrint.Clear();
        }

        public void PrintConsole() {
            Console.Clear();
            foreach (string item in toPrint) {
                Console.Write(item);
            }
        }

        private void StartAgent() {
            printTask = Task.Factory.StartNew(() => {
                while (true) {
                    waitToPrint.WaitOne();
                    //Console.Clear();
                    foreach (string item in toPrint) {
                        Console.Write(item);
                    }
                    //toPrint.Clear();
                }
            });
        }
    }
}
