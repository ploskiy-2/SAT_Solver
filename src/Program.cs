using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
namespace src;
public class Parser {
    public Matrix GetVars(string path){
        Matrix formula = new Matrix();
        using (FileStream stream = File.OpenRead(path))
        using (var reader = new StreamReader(stream))
        {
            string? line;
            int n,m;
            m=n=0;
            while ((line = reader.ReadLine()) != null && line.Length>0){
                if (line.Length>0 && line[0]=='p'){
                    string[] subs = line.Split(' ');
                    n = Convert.ToInt32(subs[2]); // Count of column 
                    m = Convert.ToInt32(subs[3]); // Count of rows
                    break;}
            }
            formula.column = n;
            formula.rows = m;
            while ((line = reader.ReadLine()) != null && line[0]!='c' && line[0]!='p'){
                string[] subs = line.Split(' ');

                Clause clause = new Clause(subs);
                formula.consid_clause.Add(clause);
            }
        
        return formula;
        }}
}
public class Matrix {
    public int column;
    public int rows;
    private HashSet<int> literal_ans = new HashSet<int>();
    public HashSet<Clause> consid_clause = new HashSet<Clause>();

    //removing each clause containing a unit clause's literal and 
    //in discarding the complement of a unit clause's literal
    private void UnitPropagation(){
        List<int> ch_int = new List<int>();
        foreach (Clause c in consid_clause){
            if (c.GetCountNeg()==1 && c.GetCountPos()==0 ){
                int t = c.GetFirstLiteral(-1);
                literal_ans.Add(t);
                ch_int.Add(t);
            }
            if (c.GetCountPos()==1 && c.GetCountNeg()==0 ){
                int t = c.GetFirstLiteral(1);
                literal_ans.Add(t);
                ch_int.Add(t);
            }
        }
        foreach (var tt in ch_int){
            ChangeClauses(tt);
        }
    }

    //if we know some literal is true or false, we need change each clause where 
    //this literal is used 
    public void ChangeClauses(int lit){
        List<Clause> rem_clause = new List<Clause>();
        List<Clause> add_clause = new List<Clause>();
        foreach (Clause cl in consid_clause){
            if (cl.IsContains(1,lit) && lit>0){
                rem_clause.Add(cl);
            }
            if (cl.IsContains(1,-lit) && lit<0){
                /*Clause n_cl = new Clause(1,-lit, cl.GetLiterals(1), cl.GetLiterals(-1));
                rem_clause.Add(cl);
                add_clause.Add(n_cl);*/
                cl.RemoveLiteral(1,-lit);
            }
            if (cl.IsContains(-1,lit) && lit<0){
                rem_clause.Add(cl);
            }
            if (cl.IsContains(-1,-lit) && lit>0){
                /*Clause n_cl = new Clause(-1, -lit, cl.GetLiterals(1), cl.GetLiterals(-1));
                rem_clause.Add(cl);
                add_clause.Add(n_cl);*/
                cl.RemoveLiteral(-1,-lit);

            }          
        }

        consid_clause.ExceptWith(rem_clause);
        //consid_clause.UnionWith(add_clause);
    }
    
    private void PureLiteralElimination(){
        for (int i=1; i<=column; i++){
            if (!literal_ans.Contains(i) &&  !literal_ans.Contains(-i)){
                List<Clause> plusClauses = consid_clause.Where(c => c.IsContains(1,i) && !c.IsContains(-1,-i)).ToList();
                List<Clause> minusClauses = consid_clause.Where(c => c.IsContains(-1,-i) && !c.IsContains(1,i)).ToList();          
                if (plusClauses.Count()>0 && minusClauses.Count()==0 ){
                    ChangeClauses(i);
                    literal_ans.Add(i);
                }
                if (minusClauses.Count()>0 && plusClauses.Count()==0){
                    ChangeClauses(-i);
                    literal_ans.Add(-i);
                }
            }
        }
    }

    public bool DPLL(){
        UnitPropagation(); 
        PureLiteralElimination();
              
        if (consid_clause.Any(clause => clause.IsEmpty()))
        {    
            return false; 
        }
        if (consid_clause.Count()==0)
        {
            return true;
        }      
        int literal = SelectLiteral();
     
        Matrix trueMatrix = CloneMatrix();
        trueMatrix.ChangeClauses(literal);
        trueMatrix.literal_ans.Add(literal);

        if (trueMatrix.DPLL()){
            CopyFrom(trueMatrix);
            return true;
        }

        ChangeClauses(-literal);
        literal_ans.Add(-literal);
        if (DPLL())
        {
            return true;
        }   
        
        return false;
    }

    private int SelectLiteral()
    {
        if (consid_clause.First().GetCountPos()>0){
            return consid_clause.First().GetFirstLiteral(1);
        }
        else if(consid_clause.First().GetCountNeg()>0){
            return consid_clause.First().GetFirstLiteral(-1);
        }
        else{
            return 0;
        }
    }

    private Matrix CloneMatrix()
    {
        return new Matrix
        {
            column = column,
            rows = rows,
            literal_ans = new HashSet<int>(literal_ans),
            consid_clause = consid_clause.Select(clause => clause.CloneClause()).ToHashSet(),
        };
    }

    private void CopyFrom(Matrix other)
    {
        literal_ans = new HashSet<int>(other.literal_ans);
        consid_clause = other.consid_clause.Select(clause => clause.CloneClause()).ToHashSet();
    }

    public IEnumerable<int> GetSolution(){      
        if (!(literal_ans.Count()==column)){
            for (int i=1; i<=column;i++){
                if (!literal_ans.Contains(i) && !literal_ans.Contains(-i)){
                    literal_ans.Add(i);
                }
            }
        }     
        List<int> fin_lit_ans = new List<int>(literal_ans);
        for(int i=0; i<literal_ans.Count(); i++)
        {
            for(int j=i; j<literal_ans.Count(); j++)
            {
                if(Math.Abs(fin_lit_ans[i]) < Math.Abs(fin_lit_ans[j]))
                {
                    int tmp = fin_lit_ans[i];
                    fin_lit_ans[i] = fin_lit_ans[j];
                    fin_lit_ans[j] = tmp;
                }    
            }
        }
        fin_lit_ans = fin_lit_ans.Select(c=>c).Distinct().ToList();
        fin_lit_ans.Reverse();
        return fin_lit_ans;
    }
}

public class Clause{
    private HashSet<int> pos_literals = new HashSet<int>();
    private HashSet<int> neg_literals = new HashSet<int>();
    
    public Clause(string[] subs ){
        ///We iterate with -1 because the last char in string is terminate symbol
        for (int i=0; i<subs.Count()-1; i++){
            int t = Convert.ToInt32(subs[i]);               
            if (t>0){
                pos_literals.Add(t);
            }
            else{
                neg_literals.Add(t);
            }
        }
    }
    
    public Clause(){}
    
    public Clause(int t, int r, HashSet<int> pos, HashSet<int> neg){
        if (t>0){
            pos.Remove(r);
            pos_literals = pos;
            neg_literals = neg;
        }
        else{
            neg.Remove(r);
            pos_literals = pos;
            neg_literals = neg;         
        }
    }
    
    public void PrintClause(){
        foreach (var m in pos_literals){
            Console.Write(m + " ");
        }
        foreach (var m in neg_literals){
            Console.Write(m + " ");
        }
        Console.WriteLine();       
    }
    
    public Clause CloneClause()
    {
        return new Clause
        {
            pos_literals = new HashSet<int>(pos_literals),
            neg_literals = new HashSet<int>(neg_literals)
        };
    }
    
    public bool IsEmpty()
    {
        return !(pos_literals.Any() || neg_literals.Any());
    }
  
    public int GetCountPos(){
        return pos_literals.Count();
    }
  
    public int GetCountNeg(){
        return neg_literals.Count();
    }

    public bool IsContains(int t, int r){
        if (t>0){
            return pos_literals.Contains(r);
        }
        return neg_literals.Contains(r);
    }

    public int GetFirstLiteral(int t){
        if (t>0){
            return pos_literals.First();
        }
        return neg_literals.First();
    }

    public HashSet<int> GetLiterals(int t){
        if (t>0){
            return pos_literals;
        }
        return neg_literals;
    }

    //I used to create new element of Clause instead of this method, but it was really slowly
    public void RemoveLiteral(int t,int r){
        if (t>0){
            pos_literals.Remove(r);
        }
        else{
            neg_literals.Remove(r);
        }
    }
}

class Program
{    
    static void Main(string[] args)
    {
        string path = args[0];

        Parser parser = new Parser();
        Matrix input = parser.GetVars(path);
            
        bool flag = input.DPLL();
        
        if (!flag){
            Console.WriteLine("s UNSATISFIABLE");
            return;
        }
        Console.WriteLine("s SATISFIABLE");
        Console.Write("v ");
        IEnumerable<int> solution = input.GetSolution();
        foreach (var x in solution)   
        {
            Console.Write(x + " ");
        }
        Console.Write("0");
    }
}

