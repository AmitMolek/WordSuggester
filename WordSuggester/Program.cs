using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WordSuggester {
    class Program {
        static void Main(string[] args) {
            //InputHandler handleInput = new InputHandler();
            ManualResetEvent mainThreadEvent = new ManualResetEvent(false);
            Suggester suggester = new Suggester(mainThreadEvent);
            mainThreadEvent.WaitOne();
        }
    }
}
