using System;
using System.Collections.Generic;
using weka.core.stopwords;

namespace Avaliador_Textual.Models.Outros
{
    public class MyStopwordsHandler : StopwordsHandler
    {
        private HashSet<String> myStopWords;

        public const int linguaInglesa    = 0;
        public const int linguaPortuguesa = 1;
        public int Lingua;
        
        public MyStopwordsHandler(int lingua)
        {
            Lingua = lingua;
            CarregarStopWords();
        }

        public void RemoverStopWords(List<Token> listaPalavrasArquivo)
        {
            // Percorre as palavras do texto
            foreach (var stopword in myStopWords)
            {
                if (listaPalavrasArquivo.Exists(t => t.Palavra == stopword))
                {
                    listaPalavrasArquivo.RemoveAll(t => t.Palavra == stopword);
                }
            }
        }

        //se "str" é um stopword, retorna true
        public bool isStopword(string str)
        {
            return myStopWords.Contains(str);
        }

        public void CarregarStopWords()
        {
            // Língua Inglesa
            if (Lingua == linguaInglesa) 
            {
                myStopWords = new HashSet<string>(new string[] {"a","going","get","certain","ever","back","anything","de", "another","rather","away", "about","better","aren","even","behind","besides",
                                            "beyond","high","never","else","enough","however","got","instead","least","great","less","later","just","above","might","put","many",
                                            "one","little","maybe","must","much","neither","new","old","two","nothing","often","set","perhaps","snt","since","several","according",
                                            "three","soon","something","sometimes","still","therefore","yet","thing","though","towards","together", "whether","whose", "whole","us",
                                            "upon","without","across", "actually", "although","along", "already","always","among", "also","while","ours", "which","e", "u", "s",
                                            "after", "again", "against", "all", "almost", "am", "an", "and", "within", "any", "are", "aren't", "as", "at", "be", "because",
                                            "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't", "did", "didn't", "do",
                                            "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further", "had", "hadn't", "has", "hasn't",
                                            "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his",
                                            "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me",
                                            "more", "most", "mustn't", "my", "myself", "no", "nor", "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our",
                                            "ourselves", "out", "over", "own", "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such",
                                            "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "",
                                            "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "please",
                                            "we'd", "we'll", "we're", "may", "will", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's","subject:", "subject",
                                            "who", "who's", "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've",
                                            "your", "yours", "yourself", "yourselves","n't", "label", "keywords", "autor", "abstract"});
            }
            // Língua Portuguesa
            else if (Lingua == linguaPortuguesa)
            {
                myStopWords = new HashSet<string>(new string[] {"de","a","o","que","e","do","da","em","um","para","é","com","não","uma","os","no","se","na",
                                                                "por","mais","as","dos","como","mas","foi","ao","ele","das","tem","à","ser","seu","sua","ou",
                                                                "quando","muito","há","nos","já","está","eu","também","só","pelo","pela","até","isso","ela",
                                                                "entre","era","depois","sem","mesmo","aos","ter","seus","quem","nas","me","esse","eles","estão",
                                                                "você","tinha","foram","essa","num","nem","suas","meu","às","minha","têm","numa","pelos","elas",
                                                                "havia","seja","qual","será","nós","tenho","lhe","deles","essas","esses","pelas","este","fosse",
                                                                "dele","tu","te","vocês","vos","lhes","meus","minhas","teu","tua","teus","tuas","nosso","nossa",
                                                                "nossos","nossas","dela","delas","esta","estes","estas","aquele","aquela","aqueles","aquelas",
                                                                "isto","aquilo","estou","está","estamos","estão","estive","esteve","estivemos","estiveram",
                                                                "estava","estávamos","estavam","estivera","estivéramos","esteja","estejamos","estejam",
                                                                "estivesse","estivéssemos","estivessem","estiver","estivermos","estiverem","hei","há","havemos",
                                                                "hão","houve","houvemos","houveram","houvera","houvéramos","haja","hajamos","hajam","houvesse",
                                                                "houvéssemos","houvessem","houver","houvermos","houverem","houverei","houverá","houveremos","houverão",
                                                                "houveria","houveríamos","houveriam","sou","somos","são","era","éramos","eram","fui","foi","fomos",
                                                                "foram","fora","fôramos","seja","sejamos","sejam","fosse","fôssemos","fossem","for","formos","forem",
                                                                "serei","será","seremos","serão","seria","seríamos","seriam","tenho","tem","temos","têm","tinha",
                                                                "tínhamos","tinham","tive","teve","tivemos","tiveram","tivera","tivéramos","tenha","tenhamos","tenham",
                                                                "tivesse","tivéssemos","tivessem","tiver","tivermos","tiverem","terei","terá","teremos","terão",
                                                                "teria","teríamos","teriam","seu","de","que","as","se","no","das","ou","é"});
            }
        }
        
    }
}