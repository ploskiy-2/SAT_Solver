﻿namespace src;

public static class Parser
{
    public static Matrix CNFPars(string path)
    {
        HashSet<Clause> consid_clause = new HashSet<Clause>();
        using (FileStream stream = File.OpenRead(path))
        using (var reader = new StreamReader(stream))
        {
            string? line;
            int n, m;
            m = n = 0;
            while ((line = reader.ReadLine()) != null && line.Length > 0)
            {
                if (line.Length > 0 && line[0] == 'p')
                {
                    string[] subs = line.Split(' ');
                    n = Convert.ToInt32(subs[2]); // Count of column 
                    m = Convert.ToInt32(subs[3]); // Count of rows
                    break;
                }
            }
            int column = n;
            int rows = m;
            while ((line = reader.ReadLine()) != null && line[0] != 'c' && line[0] != 'p')
            {
                string[] subs = line.Split(' ');

                Clause clause = new Clause(subs);
                consid_clause.Add(clause);
            }


            return new Matrix(column, rows, consid_clause);
        }
    }
}

public class Matrix
{
    private int column;
    private int rows;
    private HashSet<int> literal_ans = new HashSet<int>();
    private HashSet<Clause> consid_clause =  new HashSet<Clause>();

    public Matrix() { }
    public Matrix(int c, int r, HashSet<Clause> con)
    {
        this.column = c;
        this.rows = r;
        this.consid_clause = con;
    }

    //removing each clause containing a unit clause's literal and 
    //in discarding the complement of a unit clause's literal
    private void UnitPropagation()
    {
        HashSet<int> ch_int = new HashSet<int>();
        foreach (Clause c in consid_clause)
        {
            if (c.Count() == 1)
            {
                int t = c.GetFirstLiteral();
                literal_ans.Add(t);
                ch_int.Add(t);
            }
        }
        foreach (var tt in ch_int)
        {
            ChangeClauses(tt);
        }
    }

    //if we know some literal is true or false, we need change each clause where 
    //this literal is used 
    private void ChangeClauses(int lit)
    {
        List<Clause> rem_clause = new List<Clause>();
        foreach (Clause cl in consid_clause)
        {
            if (cl.Contains(lit))
            {
                rem_clause.Add(cl);
            }
            if (cl.Contains(-lit))
            {
                cl.RemoveLiteral(-lit);
            }
        }
        consid_clause.ExceptWith(rem_clause);
    }

    private void PureLiteralElimination()
    {
        for (int i = 1; i <= column; i++)
        {
            if (!literal_ans.Contains(i) && !literal_ans.Contains(-i))
            {
                HashSet<Clause> plusClauses = consid_clause.Where(c => c.Contains(i) && !c.Contains(-i)).ToHashSet();
                HashSet<Clause> minusClauses = consid_clause.Where(c => c.Contains(-i) && !c.Contains(i)).ToHashSet();
                if (plusClauses.Count() > 0 && minusClauses.Count() == 0)
                {
                    consid_clause.ExceptWith(plusClauses);
                    literal_ans.Add(i);
                }
                if (minusClauses.Count() > 0 && plusClauses.Count() == 0)
                {
                    consid_clause.ExceptWith(minusClauses);
                    literal_ans.Add(-i);
                }
            }
        }
    }

    private bool DPLLHelper()
    {
        UnitPropagation();
        PureLiteralElimination();

        if (consid_clause.Any(clause => clause.IsEmpty()))
        {
            return false;
        }
        if (consid_clause.Count() == 0)
        {
            return true;
        }
        int literal = SelectLiteral();

        Matrix trueMatrix = CloneMatrix();
        trueMatrix.ChangeClauses(literal);
        trueMatrix.literal_ans.Add(literal);

        if (trueMatrix.DPLLHelper())
        {
            CopyFrom(trueMatrix);
            return true;
        }

        ChangeClauses(-literal);
        literal_ans.Add(-literal);
        if (DPLLHelper())
        {
            return true;
        }

        return false;
    }

    public IEnumerable<int> DPLL()
    {
        if (DPLLHelper())
        {
            return GetSolution();
        }
        return Enumerable.Empty<int>();
    }
    private int SelectLiteral()
    {
        return consid_clause.First().GetFirstLiteral();
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

    private IEnumerable<int> GetSolution()
    {
        if (!(literal_ans.Count() == column))
        {
            for (int i = 1; i <= column; i++)
            {
                if (!literal_ans.Contains(i) && !literal_ans.Contains(-i))
                {
                    literal_ans.Add(i);
                }
            }
        }
        List<int> fin_lit_ans = new List<int>(literal_ans);
        for (int i = 0; i < literal_ans.Count(); i++)
        {
            for (int j = i; j < literal_ans.Count(); j++)
            {
                if (Math.Abs(fin_lit_ans[i]) < Math.Abs(fin_lit_ans[j]))
                {
                    int tmp = fin_lit_ans[i];
                    fin_lit_ans[i] = fin_lit_ans[j];
                    fin_lit_ans[j] = tmp;
                }
            }
        }
        fin_lit_ans = fin_lit_ans.Select(c => c).Distinct().ToList();
        fin_lit_ans.Reverse();
        return fin_lit_ans;
    }
}

public class Clause
{
    private HashSet<int> pos_literals = new HashSet<int>();
    private HashSet<int> neg_literals = new HashSet<int>();

    public Clause(string[] subs)
    {
        ///We iterate with -1 because the last char in string is terminate symbol
        for (int i = 0; i < subs.Count() - 1; i++)
        {
            int t = Convert.ToInt32(subs[i]);
            if (t > 0)
            {
                pos_literals.Add(t);
            }
            else
            {
                neg_literals.Add(t);
            }
        }
    }

    public Clause() { }

    public void PrintClause()
    {
        foreach (var m in pos_literals)
        {
            Console.Write(m + " ");
        }
        foreach (var m in neg_literals)
        {
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

    public int Count()
    {
        return neg_literals.Count() + pos_literals.Count();
    }

    public bool Contains(int r)
    {
        return pos_literals.Contains(r) || neg_literals.Contains(r);
    }

    public int GetFirstLiteral()
    {
        if (pos_literals.Count() > 0)
        {
            return pos_literals.First();
        }
        return neg_literals.First();
    }

    public void RemoveLiteral(int r)
    {
        if (r > 0)
        {
            pos_literals.Remove(r);
        }
        else
        {
            neg_literals.Remove(r);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string path = args[0];

        Matrix input = Parser.CNFPars(path);
        IEnumerable<int> solution = input.DPLL();

        if (solution.Count() == 0)
        {
            Console.WriteLine("s UNSATISFIABLE");
            return;
        }

        Console.WriteLine("s SATISFIABLE");
        Console.Write("v ");

        foreach (var x in solution)
        {
            Console.Write(x + " ");
        }
        Console.Write("0");
    }
}

