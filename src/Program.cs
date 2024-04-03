using System;
namespace src;
public class Clause{
    public int column;
    public int rows;
    public int[,] ints;
    public Clause(int m, int n, int[,] ints1){
        column = n;
        rows = m;
        ints = ints1;
    }

    //Method to remove row (if we have unit clause or we can drop this line according  
    //to DPLL algorithm)
    public void RemoveRow(int index){
        if (index<0 || index>this.rows){
            return;
        }
        int[,] new_ints = new int[this.rows-1,this.column];
        for (int i=0; i<rows; i++){
            if (i!=index){
                int[] n_int = new int[rows];
                for (int j=0; j<column;j++){
                    new_ints[i,j] = ints[i,j];
                }
            }
        }
        this.ints = new_ints; 
    }



}
class Program
{
    public static Clause GetVars(string path){
        int n,m,c;
        n=m=c=0;
        int[,] ints;
        using (FileStream stream = File.OpenRead(path))
        using (var reader = new StreamReader(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null){
                if (line[0]=='p'){
                string[] subs = line.Split(' ');
                n = Convert.ToInt32(subs[2]); // Count of column 
                m = Convert.ToInt32(subs[3]); // Count of rows
                break;
            }}
            ints = new int[m,n];
            while ((line = reader.ReadLine()) != null && line[0]!='c' && line[0]!='p'){
                string[] subs = line.Split(' ');
                ///We iterate with -1 because the last char in string is terminate symbol
                for (int i=0; i<subs.Count()-1; i++){
                    int t = Math.Abs(Convert.ToInt32(subs[i]));
                    ints[c, t-1] = Convert.ToInt32(subs[i]);
                }
                c+=1;
            }
        }
        //rows,column,ints
        Clause clause = new Clause(m,n,ints);
        return clause;
    }
    
    public static string SAT(Clause clause){
        List<int> pos_num = new List<int>();
        List<int> neg_num = new List<int>();

        return "";
    }
    
    public static int FindUnitClause(int[,] ints){
        return 0;
    }
    
    static void Main(string[] args)
    {
        string path = @"C:\Users\VLADIMIR\Desktop\My_SAT_solver\test.txt";
        Clause clause = GetVars(path);
    }
}

