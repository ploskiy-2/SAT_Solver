using System;
namespace src;
public class Matrix {
    public int column;
    public int rows;
    public List<int> pos_literal_ans = new List<int>();
    public List<int> neg_literal_ans = new List<int>();
    public List<Clause> consid_clause = new List<Clause>();

    //Method to remove row (if we have unit clause or we can drop this line according  
    //to DPLL algorithm)
    public void RemoveRow(int index){

    }

    //removing each clause containing a unit clause's literal and 
    //in discarding the complement of a unit clause's literal
    public void UnitPropagation(int index, bool f){
    }

}
public class Clause{
    public bool is_consider = true;
    public List<int> pos_literals = new List<int>();
    public List<int> neg_literals = new List<int>();

}
class Program
{
    public static Matrix GetVars(string path){
        Matrix formula = new Matrix();
        using (FileStream stream = File.OpenRead(path))
        using (var reader = new StreamReader(stream))
        {
            string line;
            int n,m;
            m=n=0;
            while ((line = reader.ReadLine()) != null){
                if (line[0]=='p'){
                string[] subs = line.Split(' ');
                n = Convert.ToInt32(subs[2]); // Count of column 
                m = Convert.ToInt32(subs[3]); // Count of rows
                break;
            }}
            formula.column = n;
            formula.rows = m;
            while ((line = reader.ReadLine()) != null && line[0]!='c' && line[0]!='p'){
                string[] subs = line.Split(' ');

                Clause clause = new Clause();
                ///We iterate with -1 because the last char in string is terminate symbol
                for (int i=0; i<subs.Count()-1; i++){
                    int t = Convert.ToInt32(subs[i]);               

                    if (t>0){
                        clause.pos_literals.Add(t);
                    }
                    else{
                        clause.neg_literals.Add(t);
                    }
                }
                formula.consid_clause.Add(clause);
            }
        }
        return formula;
    }
    
    static void Main(string[] args)
    {
        string path = @"C:\Users\VLADIMIR\Desktop\My_SAT_solver\test.txt";
        Matrix input = GetVars(path);
        foreach (Clause cl in input.consid_clause){
            foreach (var l in cl.pos_literals){
                Console.Write(l + " ");
            }
            foreach (var l in cl.neg_literals){
                Console.Write(l + " ");
            }
            Console.WriteLine();
        }
    }
}

