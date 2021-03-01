using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleApp2 {
    class Matrice {

        List<Gramatica> gramatica = new List<Gramatica>();
        List<Gramatica> _I = new List<Gramatica>() ;
        List<List<Gramatica>> _C = new List<List<Gramatica>>();

        List<Functie> functii = new List<Functie>();
        public static Dictionary<string, string> tabel = new Dictionary<string, string>();
        int m = 0;


        public Matrice(Gramatica[] gramatica) {
            initDictionar(gramatica);

            constructC();

            string comb = Program.terminale + "$" + Program.neterminale;

            for (int i = 0; i < _C.Count; i++)
                for (int j = 0; j < comb.Length; j++) {
                    tabel.Add(i.ToString() + comb[j], "-");
                }

            constructMatrix();
         
            for (int j = 0; j < comb.Length; j++)
                Console.Write("{0,-6} ", comb[j]);

            Console.WriteLine();
            foreach (var kvp in tabel) {
                if (m != comb.Length) { Console.Write("{0,-6} ", kvp.Value); m++; }
                else { m = 0; Console.Write("\n{0, -6} ", kvp.Value); m++; }
            }
            Console.WriteLine();
        }

        public object getMatrix() {
            return tabel;
        }

        public void constructMatrix() {

            int k = 0;
            do {
                _I = _C[k];            
                do {
                    Gramatica alfa = _I[0];
                    _I.RemoveAt(0);
                    string[] splitAlfa = alfa.prod.Split('.');
                    
                    switch (caseFunction(alfa)) {
                        case 1:                      
                                tabel[k + splitAlfa[1].Substring(0, 1)] = findFunctie(k, splitAlfa[1].Substring(0, 1));
                            break;
                        case 2:                           
                                tabel[k + splitAlfa[1].Substring(0, 1)] = "d" + findFunctie(k, splitAlfa[1].Substring(0, 1));
                            break;
                        case 3:
                            if (splitAlfa[0] == Program.simbolInitial)
                                addAccept(k, splitAlfa[0]);                               
                            else 
                                addReducere(k, splitAlfa[0].Substring(splitAlfa[0].Length-1), findPord(alfa.neterminal, splitAlfa[0]));                          
                            break;
                    }


                } while (_I.Count > 0);

                k++;
            } while (k < _C.Count);

        }

        public void constructC() {
            _I.Add(new Gramatica(Program.simbolInitial + "'", "." + Program.simbolInitial));

            INC(_I[0].prod.Substring(1, 1), _I);

            _C.Add(_I);

            for (int i = 0; i < _C.Count; i++)
                for (int j = 0; j < _C[i].Count; j++) {
                    string[] split = _C[i][j].prod.Split('.');

                    if (split[1].Length > 0) {
                        List<Gramatica> _IBuffer = SALT(_C[i], split[1].Substring(0, 1));

                        if (canAdd(_IBuffer) && _IBuffer.Any()) {
                            _C.Add(_IBuffer);
                            functii.Add(new Functie(i, split[1].Substring(0, 1), _C.Count - 1));
                        }
                        else {
                            if (canAddFunction(i, split[1].Substring(0, 1), cantAdd(_IBuffer)))
                                functii.Add(new Functie(i, split[1].Substring(0, 1), cantAdd(_IBuffer)));
                        }
                    }

                }

        }

        private string findFunctie(int k, string v) {
           for(int i = 0; i < functii.Count; i++) {
                if (functii[i].second == v && functii[i].prim == k)
                    return functii[i].third.ToString();
            }
            return 0.ToString();
        }

        private int caseFunction(Gramatica alfa) {
            string[] split = alfa.prod.Split('.');
            if (split[1].Length > 0) {
                if (Program.neterminale.Contains(split[1].Substring(0,1)))
                    return 1;
                if (Program.terminale.Contains(split[1].Substring(0,1)))
                    return 2;
                return 3;
            }
            else
                return 3;
        }

        private bool canAddFunction(int i, string v1, int v2) {
           foreach(var fun in functii) {
                if (fun.prim == i && fun.second == v1 && fun.third == v2)
                    return false;
            }
           return true;
        }

        private bool canAdd(List<Gramatica> buffer) {

            for(int i = 0; i < _C.Count; i++) {
                if (listeEgale(buffer, _C[i]))
                    return false;
            }
            return true;
        }

        private int cantAdd(List<Gramatica> buffer) {

            for (int i = 0; i < _C.Count; i++) {
                if (listeEgale(buffer, _C[i]))
                    return i;
            }
            return 0;           
        }

        public List<Gramatica> SALT(List<Gramatica> list, string value) {
            List<Gramatica> buffer = new List<Gramatica>();

            for (int i = 0; i <list.Count; i++) {
                string[] split = list[i].prod.Split('.');
                if (split[1]!="")
                    if (split[1].Substring(0, 1) == value) {
                        if (split[1].Length >= 1)
                            buffer.Add(new Gramatica(list[i].neterminal, 
                                split[0] + split[1].Substring(0, 1) + "." + split[1].Substring(1) ) );
                        else
                            buffer.Add(new Gramatica(list[i].neterminal, split[1] + "."));
                    }           
            }

            for (int i = 0; i < buffer.Count; i++) {
                string[] split = buffer[i].prod.Split('.');
                if (split[1] != "") {
                    INC(split[1].Substring(0, 1), buffer);
                }
            }
            return buffer;

        }

        public void INC(string input, List<Gramatica> buffer) {


            int index = retrunIndex(input);
            if (index == -1)
                return;

            for(int j = 0; j < gramatica[index].productii.Length; j++)
                if( notExist(gramatica[index].productii[j], buffer) ) {
                    buffer.Add(new Gramatica(gramatica[index].neterminal, "." + gramatica[index].productii[j]));
                    INC(gramatica[index].productii[j].Substring(0, 1),  buffer);
                }
        }      

        private int retrunIndex(string input) {
            for (int i = 0; i < gramatica.Count ; i++)
                if (gramatica[i].neterminal == input)
                    return i;
            return -1;

        }

        private bool notExist( string v, List<Gramatica> buffer ) {
        
            for (int i = 0; i < buffer.Count; i++) {
                string[] split = buffer[i].prod.Split('.');
                if (split[1]== v)
                    return false;
            }
            return true;

            
        }

        public void initDictionar(Gramatica[] gramatica) {
            for (int i = 0; i < Program.neterminale.Length; i++) {
                    this.gramatica.Add(new Gramatica(gramatica[i].neterminal, gramatica[i].productii));
                
            }
        }

        public bool listeEgale(List<Gramatica> l1, List<Gramatica> l2) {
            if (l1.Count != l2.Count)
                return false;
            else
                for(int i = 0; i < l1.Count; i++) {
                    if (l1[i].prod != l2[i].prod || l1[i].neterminal != l2[i].neterminal)
                        return false;
                }
            return true;
        }

        private int findPord(string neterminal, string v) {

            int prodNumber = 1;

            for (int i = 0; i < gramatica.Count; i++) { 
                if (gramatica[i].neterminal != neterminal)
                    prodNumber += gramatica[i].productii.Length;
                else
                    for(int j=0;j < gramatica[i].productii.Length; j++) {
                        if (gramatica[i].productii[j] == v)
                            return prodNumber;
                        else
                            prodNumber++;
                    }
            }
            return 0;
                
        }

        public void addReducere(int i, string A, int nrRed) {
            List<string> urm = new List<string>();

            URM(A, urm);
          
            foreach(var l in urm) {
                tabel[i + l] = "r" + nrRed;
            }

        }

        public void addAccept(int i, string A) {
            List<string> urm = new List<string>();

            URM(A, urm);
          
            tabel[i + urm[0]] = "accept";

        }

        public void URM(string A, List<string> urm) {
            if (A == Program.simbolInitial)
                if(!urm.Exists(x=>x=="$"))
                    urm.Add("$");

            for (int i = 0; i < gramatica.Count; i++) {
                for (int j = 0; j < gramatica[i].productii.Length; j++) {
                    if (gramatica[i].productii[j].Contains(A)) {
                        string[] split = gramatica[i].productii[j].Split(A);

                        if (split[0].Length > 0 && split[1].Length > 0) {
                            if (Program.neterminale.Contains(split[1].Substring(0, 1)))
                                URM(gramatica[i].neterminal, urm);
                            else
                                urm.Add(split[1].Substring(0, 1));
                        }
                        if(split[1].Length == 0 && split[0].Length > 0) {
                            if (Program.terminale.Contains(split[0].Substring(split[0].Length-1, 1)))
                                urm.Add(split[0].Substring(split[0].Length - 1));
                            URM(gramatica[i].neterminal, urm);
                        }

                        if (split[0].Length == 0 && split[1].Length > 0) {                           
                            if(A != gramatica[i].neterminal)
                            URM(gramatica[i].neterminal, urm);
                        }

                        if (split[0].Length == 0 && split[1].Length == 0) {
                            if (Program.terminale.Contains(A))
                                URM(gramatica[i].neterminal, urm);
                            
                        }



                    }
                }

            }
        }

        public void CALCURM(string A, List<string> urm) {
            if (A == Program.simbolInitial)
                if (!urm.Exists(x => x == "$"))
                    urm.Add("$");

            for (int i = 0; i < gramatica.Count; i++) {
                for (int j = 0; j < gramatica[i].productii.Length; j++) {
                    if (gramatica[i].productii[j].Contains(A)) {
                        string[] split = gramatica[i].productii[j].Split(A);
                        if (split[1].Length > 0)
                            for(int k=0; k< split[1].Length;k++)
                                CALCPRIM(split[1].Substring(k, 1), urm);
                        else
                            CALCURM(gramatica[i].neterminal, urm);  
                    }
                }
            }
        }
        public void CALCPRIM(string A, List<string> urm) {
            if (Program.terminale.Contains(A))
                urm.Add(A);
            else
            for (int i = 0; i < gramatica.Count; i++) {
                if(gramatica[i].neterminal == A) { 
                    for (int j = 0; j < gramatica[i].productii.Length; j++) {
                            string car = gramatica[i].productii[j].Substring(0,1);
                            if (Program.terminale.Contains(car))
                                urm.Add(car);
                            else if (car != A)
                                CALCPRIM(car, urm);
                    }
                }
            }
        }
    }
   
}
