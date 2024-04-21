using System;
using System.Collections.Generic;
using System.Linq;
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
        
        return formula;
        }}
}
public class Matrix {
    public int column;
    public int rows;
    public List<int> literal_ans = new List<int>();
    public List<Clause> consid_clause = new List<Clause>();

    //removing each clause containing a unit clause's literal and 
    //in discarding the complement of a unit clause's literal
    private void UnitPropagation(){
        List<int> ch_int = new List<int>();
        foreach (Clause c in consid_clause){
            if (c.neg_literals.Count()==1 && c.pos_literals.Count()==0 ){
                int t = c.neg_literals[0];
                literal_ans.Add(t);
                ch_int.Add(t);
            }
            if (c.pos_literals.Count()==1 && c.neg_literals.Count()==0 ){
                int t = c.pos_literals[0];
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
        foreach (Clause cl in consid_clause){
            if (cl.pos_literals.Contains(lit) && lit>0){
                rem_clause.Add(cl);
            }
            if (cl.pos_literals.Contains(-lit) && lit<0){
                cl.pos_literals.Remove(-lit);
            }
            if (cl.neg_literals.Contains(lit) && lit<0){
                rem_clause.Add(cl);
            }
            if (cl.neg_literals.Contains(-lit) && lit>0){
                cl.neg_literals.Remove(-lit);
            }          
        }
        foreach (Clause cl in rem_clause){
            consid_clause.Remove(cl);
        }
    }
    
    private void PureLiteralElimination(){
        for (int i=1; i<=column; i++){
            if (!literal_ans.Contains(i) &&  !literal_ans.Contains(-i)){
                List<Clause> plusClauses = consid_clause.Where(c => c.pos_literals.Contains(i) && !c.neg_literals.Contains(-i)).ToList();
                List<Clause> minusClauses = consid_clause.Where(c => c.neg_literals.Contains(-i) && !c.pos_literals.Contains(i)).ToList();          
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

        Matrix falseMatrix = CloneMatrix();

        
        if (trueMatrix.DPLL()){
            CopyFrom(trueMatrix);
            return true;
        }
        falseMatrix.ChangeClauses(-literal);
        falseMatrix.literal_ans.Add(-literal);
        if (falseMatrix.DPLL())
        {
            CopyFrom(falseMatrix);
            return true;
        }   
        
        return false;
    }

    private int SelectLiteral()
    {
        if (consid_clause[0].pos_literals.Count()>0){
            return consid_clause[0].pos_literals[0];
        }
        else if(consid_clause[0].neg_literals.Count()>0){
            return consid_clause[0].neg_literals[0];
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
            literal_ans = new List<int>(literal_ans),
            consid_clause = consid_clause.Select(clause => clause.CloneClause()).ToList()
        };
    }

    private void CopyFrom(Matrix other)
    {
        literal_ans = new List<int>(other.literal_ans);
        consid_clause = other.consid_clause.Select(clause => clause.CloneClause()).ToList();
    }

    public IEnumerable<int> GetSolution(){      
        if (!(literal_ans.Count()==column)){
            for (int i=1; i<=column;i++){
                if (!literal_ans.Contains(i) && !literal_ans.Contains(-i)){
                    literal_ans.Add(i);
                }
            }
        }     
        for(int i=0; i<literal_ans.Count(); i++)
        {
            for(int j=i; j<literal_ans.Count(); j++)
            {
                if(Math.Abs(literal_ans[i]) < Math.Abs(literal_ans[j]))
                {
                    int tmp = literal_ans[i];
                    literal_ans[i] = literal_ans[j];
                    literal_ans[j] = tmp;
                }    
            }
        }
        literal_ans = literal_ans.Select(c=>c).Distinct().ToList();
        literal_ans.Reverse();
        return literal_ans;
    }
}

public class Clause{
    public List<int> pos_literals = new List<int>();
    public List<int> neg_literals = new List<int>();
    public void PrintClause(){
        foreach (var m in this.pos_literals){
            Console.Write(m + " ");
        }
        foreach (var m in this.neg_literals){
            Console.Write(m + " ");
        }
        Console.WriteLine();       
    }
    public Clause CloneClause()
    {
        return new Clause
        {
            pos_literals = new List<int>(pos_literals),
            neg_literals = new List<int>(neg_literals)
        };
    }

    public bool IsEmpty()
    {
        return !(pos_literals.Any() || neg_literals.Any());
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

