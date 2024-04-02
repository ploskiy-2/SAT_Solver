using System;
namespace src;
class Program
{
    public static int[,] GetVars(string path){
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
                for (int i=0; i<subs.Count()-1; i++){
                    int t = Math.Abs(Convert.ToInt32(subs[i]));
                    ints[c, t-1] = Convert.ToInt32(subs[i]);
                }
                c+=1;
            }
        }
        return ints;
    }
    
    static void Main(string[] args)
    {
        string path = @"C:\Users\VLADIMIR\Desktop\My_SAT_solver\test.txt";
        int[,] clauses = GetVars(path);
        
    }
}
