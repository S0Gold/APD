using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp2 {
    class Program {
        public static Gramatica[] gramatica = new Gramatica[10];

        public static string sirIntrare;
        public static string neterminale;
        public static string terminale;
        public static string simbolInitial;

        public static Dictionary<string, string> tabel = new Dictionary<string, string>();

        public static string[] coloana = new string[9];
        public static string[] linie = new string[12];


        static List<string> stack = new List<string>() { "0" };


        static void Main(string[] args) {

            readFile();                 
            Matrice matrice = new Matrice(gramatica);
            tabel = (Dictionary<string, string>)matrice.getMatrix();
            automat_PUSH_DOWN2();
        }

        static public void readFile() {

            try {

                // Open the text file using a stream reader.
                using (var sr = new StreamReader("C:\\Users\\DxGod\\source\\repos\\ConsoleApp2\\input.txt")) {

                    string line;
                    sirIntrare = sr.ReadLine();
                    neterminale = sr.ReadLine();
                    terminale = sr.ReadLine();
                    simbolInitial = sr.ReadLine();
                    int indexLine = 0;

                    while ((indexLine < neterminale.Length) && (line = sr.ReadLine()) != null) {
                        gramatica[indexLine++] = new Gramatica(line);
                    }
                    Split(sr.ReadLine(), linie);
                    Split(sr.ReadLine(), coloana);
                    int l = 0, c = 0;
                    while ((line = sr.ReadLine()) != null) {
                        string[] words = line.Split(' ');
                        c = 0;
                        foreach (var word in words) {
                            tabel.Add(linie[l] + coloana[c++], word);
                        }
                        l++;

                    }
                }
            }
            catch (IOException e) {

                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }
        private static void Split(string v, string[] vect) {
            int _index = 0;
            string[] words = v.Split(' ');
            foreach (var word in words) {
                vect[_index++] = new string(word);
            }
        }       
        private static void automat_PUSH_DOWN2() {

            string combinatie = tabel[stack[stack.Count - 1] + sirIntrare[0]]; //Stabilim combinatia din tabela de actiuni
            Console.WriteLine("\n{0, -15} {1, -15} {2}", "Stiva", "Intrare", "Actiune rezultata "); //afisam capul de tabel
            Console.Write("\n{0, -15} {1, -15} {2}", printStack(), sirIntrare, combinatie);//Afisam elemetele

            while (combinatie != "accept") {

                //Testam daca este o deplasare
                if (combinatie.StartsWith('d')) {

                    stack.Add(sirIntrare[0].ToString()); //Adaugam in stiva caracterul din sirul de intrare
                    stack.Add(combinatie.Substring(1)); //Adaugam in stiva deplasamentul                  
                    sirIntrare = sirIntrare.Substring(1); //Eliminam primul caracter din sirul de intrare
                }
                else //Testam daca este o reducere
                if (combinatie.StartsWith('r')) {

                    int reducere = Int32.Parse(combinatie.Substring(1)); //preluam numarul reducerii din combinatie
                    int indexGramatica = -1;

                    while (reducere > 0) {
                        indexGramatica++;
                        if (reducere > gramatica[indexGramatica].productii.Length)
                            reducere -= gramatica[indexGramatica].productii.Length;//scandem din numarul reducerii pana cand este mai mic decat lungea unei liste de productii 
                        else {
                            //preluam indexul la care se afla primul caracter al productiei in stiva
                            int deleteIndex = stack.FindLastIndex(x => x == gramatica[indexGramatica].productii[reducere - 1].Substring(0, 1));
                            //O afisare
                            Console.Write(" => " + gramatica[indexGramatica].neterminal + "+TS(" + stack[deleteIndex - 1] + "," + gramatica[indexGramatica].neterminal + ")");
                            // stergem toate elementele din lista de la indexul in care se afla elementul
                            stack.RemoveRange(deleteIndex, stack.Count - deleteIndex);
                            //inlocuim elementul cu neterminalul productiei
                            stack.Add(gramatica[indexGramatica].neterminal);
                            break;
                        }
                    }
                    //adaugam in stiva elementul din tabela de salt dupa reducere
                    stack.Add(tabel[stack[stack.Count - 2] + stack[stack.Count - 1]]);
                }
                else {
                    Console.Write("\nNu a fost gait");
                    break;
                }

                if (Int32.TryParse(stack[stack.Count - 1], out int numar))
                    combinatie = tabel[stack[stack.Count - 1] + sirIntrare[0].ToString()];
                else
                    combinatie = tabel[stack[stack.Count - 2] + stack[stack.Count - 1]];



                Console.Write("\n{0, -15} {1, -15} {2}", printStack(), sirIntrare, combinatie);//Afisam elemetele

            }

        }
        static public string printStack() {
            string toPrint = "$ ";
            foreach (var elem in stack) {
                toPrint += elem;
            }
            return toPrint;
        }
        

     
    }
}
