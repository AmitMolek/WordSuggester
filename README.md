# WordSuggester
Suggest words that are similar to the input (like auto-complete or auto-correct)
Uses .NET Task Parallel Library (TPL) for multi-threaded operation. Based on [Levenshtein distance](https://en.wikipedia.org/wiki/Levenshtein_distance#:~:text=Informally%2C%20the%20Levenshtein%20distance%20between,considered%20this%20distance%20in%201965.) and [BK-Tree](https://en.wikipedia.org/wiki/BK-tree).

# Examples
Currently the program will present you with 10 similar words (yep I know, the front-end needs an uplift).

![Dictionary loading](WordSuggester/docs/imgs/loading_dictionary.jpg?raw=true)  
![Hello results](WordSuggester/docs/imgs/hello_10_results.jpg?raw=true)  
![Cycle results](WordSuggester/docs/imgs/cycle_10_results.jpg?raw=true)  
