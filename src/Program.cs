using System;
namespace src;
public class Matrix {
    public bool is_real;
    public int column;
    public int rows;
    public List<int> pos_literal_ans = new List<int>();
    public List<int> neg_literal_ans = new List<int>();
    public List<Clause> consid_clause = new List<Clause>();

    //removing each clause containing a unit clause's literal and 
    //in discarding the complement of a unit clause's literal
    public void UnitPropagation(){
        List<int> ch_int = new List<int>();
        foreach (Clause c in consid_clause){
            if (c.neg_literals.Count()==1 && c.pos_literals.Count()==0 ){
                int t = c.neg_literals[0];
                neg_literal_ans.Add(t);
                if (neg_literal_ans.Contains(t) && pos_literal_ans.Contains(t)){
                    is_real = false;
                }
                ch_int.Add(t);
            }
            else if (c.pos_literals.Count()==1 && c.neg_literals.Count()==0 ){
                int t = c.pos_literals[0];
                pos_literal_ans.Add(t);
                if (neg_literal_ans.Contains(t) && pos_literal_ans.Contains(t)){
                    is_real = false;
                }
                ch_int.Add(t);
            }
        }
        foreach (var t in ch_int){
            ChangeClauses(t);
        }
    }

    //if we know some literal is true or false, we need change each clause where 
    //this literal is used 
    public void ChangeClauses(int lit){
        List<Clause> rem_clause = new List<Clause>();
        foreach (Clause cl in consid_clause){
            if (cl.pos_literals.Contains(lit) && lit>0){
                rem_clause.Add(cl);
            }
            else if (cl.pos_literals.Contains((-1)*lit) && lit<0){
                cl.pos_literals.Remove((-1)*lit);
            }
            else if (cl.neg_literals.Contains(lit) && lit<0){
                rem_clause.Add(cl);
            }
            else if (cl.neg_literals.Contains((-1)*lit) && lit>0){
                cl.neg_literals.Remove((-1)*lit);
            }          
        }
        foreach (Clause cl in rem_clause){
            consid_clause.Remove(cl);
            cl.is_consider = false;
        }
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

    }
}

