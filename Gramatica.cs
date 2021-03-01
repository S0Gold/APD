using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ConsoleApp2
{
    public class Gramatica {

        public string neterminal;
        public string prod;
        public string[] productii;

        public Gramatica(string line) {

            string[] words = line.Split(' ');
            productii = new string[words.Length - 1];
            neterminal = words[0];
            int _index = 0;

            foreach (var word in words.Skip(1)) {
                productii[_index++] = new string(word);             
            }        

        }

        public Gramatica(string neterminal, string[] productii1)  {
            this.neterminal = neterminal;
            productii = new string[productii1.Length];
            for(int i = 0; i < productii1.Length; i++) {
                productii[i] = new string(productii1[i]);
            }

        }

        public Gramatica(string neterminal, string productie) {
            this.neterminal = neterminal;
            prod = productie;
        }
        public  void AfisareProductii()
        {
            for (int i = 0; i < productii.Length; i++)
                Console.Write(productii[i] + " ");

        }


    }
}
