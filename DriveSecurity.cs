using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Timers;

namespace DriveSecurity
{
    public enum DriveStatusChange
    {
        Added,
        Removed
    }
    
    
    public class Drive
    {
        public string path = "";
        public bool validkey = false;
    

        public void UseAsKey()
        {
            string p = $@"{path}Code.TXT";
            Console.WriteLine("Generating a large key..");
            Console.WriteLine("Generating a large key..");
            FileStream s = File.Create(p);
            s.Close();
            //Create the whole key
            Random rng = new Random();
            int slope = rng.Next(10, 99);
            int ycept = rng.Next(10, 99);
            int index = 0;
            string[] ench = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" }; //number to letter conversion letters
            string[] ps = new string[] { "A", "B", "C", "D", "E", "F", "G" }; //point seperators
            string[] ss = new string[] { "k", "l", "m", "n", "o", "p" }; //number seperators
            string opt = $"{slope}{ycept}";
            int lim = 50000; //changing this number changes code length, try to keep it under 99,999,999 since the largest 32-bit integer is around 2 billion.
                             //A lim value of any more than 999999 may take a long time to generate and may take up a lot of space.
            while (index < lim)
            {
                //Start gen here
                //Console.Clear();
               // Console.WriteLine($"On number {index}/{lim}");
                opt += $"{index}{ss[rng.Next(0, ss.Length - 1)]}{(index * slope) + ycept}{ps[rng.Next(0, ps.Length - 1)]}";

                //End gen here
                index++;
            }
            opt = opt.Remove(opt.Length - 1, 1);

            opt = opt.Replace("1", "a");
            opt = opt.Replace("2", "b");
            opt = opt.Replace("4", "d");
            opt = opt.Replace("6", "f");
            opt = opt.Replace("9", "i");
            opt = opt.Replace("0", "j");

            File.WriteAllText(p, opt);
        }
        public void CheckKeyValidity()
        {
            string p = $@"{path}Code.TXT";
            if (!File.Exists(p))
            {
                validkey = false;
            }
            else
            {
                string p2 = $@"{path}After.TXT";

                string txt = File.ReadAllText(p);

                txt = txt.Replace("a", "1");
                txt = txt.Replace("b", "2");
                txt = txt.Replace("d", "4");
                txt = txt.Replace("f", "6");
                txt = txt.Replace("i", "9");
                txt = txt.Replace("j", "0");

                foreach (string i in new string[] { "A", "B", "C", "D", "E", "F", "G" })
                {
                    txt = txt.Replace(i, "/");
                }
                foreach (string i in new string[] { "k", "l", "m", "n", "o", "p" })
                {
                    txt = txt.Replace(i, ",");
                }

                int slope = Convert.ToInt32($"{txt[0]}{txt[1]}");
                int ycept = Convert.ToInt32($"{txt[2]}{txt[3]}");

                //File.WriteAllText(p2, txt);

                txt = txt.Remove(0, 4);





                string[] points = txt.Split(Convert.ToChar("/"));

                bool valid = true;
                foreach (string pt in points)
                {
                    try
                    {
                        string[] xy = pt.Split(Convert.ToChar(","));

                        int x = Convert.ToInt32(xy[0]);
                        int y = Convert.ToInt32(xy[1]);
                        // Console.Write($"pt is {pt}, ");
                        int dex = (y - ycept) / slope;
                        //  Console.WriteLine($"{dex} is found x when slope is {slope} and ycept is {ycept}");
                        if (dex != x)
                        {
                            valid = false;
                        }
                    }
                    catch
                    {
                        valid = false;
                    }


                }

                if (valid)
                {
                    validkey = true;
                }
                else
                {
                    validkey = false;
                }
            }
            
            
        }
    }
    public class DriveChangeArgs : EventArgs
    {
        // The drive object, drive is null if it is being removed
        public Drive drive;

        // The drive event type, which is either added or removed
        public DriveStatusChange status = DriveStatusChange.Added;
    }
    public class DriveWatcher
    {
        public List<Drive> drives = new List<Drive>(); // All the drives

        public event EventHandler<DriveChangeArgs> DriveChange;
        public void Start()
        {
            Timer t = new Timer();
            t.Elapsed += Tick;
            t.Interval = 100;
            t.Start();
        }
        private List<string> pdrives = new List<string>();
        
        public string mrdrive = "";
        private void Tick(object sender, ElapsedEventArgs e)
        {
            List<string> ddvs = new List<string>();
            foreach (string dl in @"A:\ B:\ C:\ D:\ E:\ F:\ G:\ H:\ I:\ J:\ K:\ L:\ M:\ N:\ O:\ P:\ Q:\ R:\ S:\ T:\ U:\ V:\ W:\ X:\ Y:\ Z:\".Split(Convert.ToChar(" ")))
            {
                if (Directory.Exists(dl))
                {
                    ddvs.Add(dl);
                }
            }
            foreach (string d in ddvs)
            {
                if (!pdrives.Contains(d))
                {
                    pdrives.Add(d);
                    DriveChangeArgs DA = new DriveChangeArgs();
                    Drive dr = new Drive();
                    dr.path = d;
                    dr.CheckKeyValidity();
                    DA.drive = dr;
                    DA.status = DriveStatusChange.Added;
                    DriveChange(this,DA);
                }
            }
            List<string> rl = new List<string>();
            foreach (string d in pdrives)
            {
                if (!ddvs.Contains(d))
                {
                    rl.Add(d);
                    DriveChangeArgs DA = new DriveChangeArgs();
                    DA.status = DriveStatusChange.Removed;
                    Drive dr = new Drive();
                    dr.path = d;
                    DA.drive = dr;
                    DriveChange(this,DA);
                }
            }
            foreach (string d in rl)
            {
                pdrives.Remove(d);
            }
            
            List<Drive> pred = new List<Drive>();
            foreach (string d in pdrives)
            {
                Drive dr = new Drive();
                dr.path = d;
                dr.CheckKeyValidity();
                pred.Add(dr);
            }
            drives = pred;
        }
    }
}

