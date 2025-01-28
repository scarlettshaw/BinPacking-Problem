using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Xml.Linq;
public class Bin
{
    //IF PROGRAM DOES NOT RUN ON OPENING USE ABSOLUTE PATH FOR TEXT FILE
    LinkedList<(int, double, bool)> binlist = new LinkedList<(int, double, bool)>();
    Dictionary<int, double> bincapmap = new Dictionary<int, double>();
    int binamount = 1;//sum of weights 1312/130 capacity = 11 bins needed
    int bincapacity = 130;
    double fitnessbest = 1000;
    public Bin()
    {
        for (int i = 0; i < binamount; i++)//bins are added to my binlist
        {
            binlist.AddLast((i + 1, 0, true));//add last to list, bin number, weight(0), is it a bin
            bincapmap.Add(i + 1, bincapacity);//map capacity to this new bin
        }



    }

    public void addtoBin(double weight, int tracker)//adding items into bins
    {
        var bin = binlist.First;//store first bin
        bool itemadded = false;//checks if an item can be added to bin
        while (bin != null)//while binlist has nt reached it's end
        {
            (int binNumber, double binWeight, bool isbin) = bin.Value;//values of the bin are named

            if (!isbin)//if the list finds an item we skip over it
            {
                bin = bin.Next;
                continue;
            }

           

            if (bincapmap[binNumber] >= weight)//if the capacity of bin is bigger than the items weight
            {

                
                binlist.AddAfter(bin, (tracker, weight, false));//add item after bin with it's weight and a flag that it is an item
                bincapmap[binNumber] -= weight;//bin capacity - weight of item
               
                itemadded = true;//itemadded = true end
                return;
            }
           
            bin = bin.Next;//if bin couldnt accomodate move to next bin
        }
        int i = 0;//finds next available space for bin
        foreach ((int binnumber, double binweight, bool isBin) in binlist)//for each bin in binlist
        {
            if (isBin)
            {
                i++;//counts amount of bins
            }
        }
        int newBinNumber = i + 1;//adds a new bin according to the amount of bins
        binlist.AddLast((newBinNumber, 0, true));//intialise this new bin
        bincapmap[newBinNumber] = bincapacity;//set its new capacity
        if(weight < 130)//if item has an acceptable weight
        {
            binlist.AddLast((tracker, weight, false));//item number and data is added after this bin
            bincapmap[newBinNumber] -= weight;//capacity is adjusted
            this.binamount = newBinNumber;//bin amount has been updated

        }
        else
        {
            Console.WriteLine("Cannot add");//if over 130 the item will never be able to be added
        }


        //https://www.geeksforgeeks.org/linked-list-implementation-in-c-sharp/
        //https://stackoverflow.com/questions/25190515/how-to-use-the-linkedlist-class-addafter-method
        //these two links allowed me to refresh my memory on how linked lists could be added to after a bin or added first or last etc tro help with my bin population
    }

    public LinkedList<(int, double, bool)> getll()
    {
        return binlist;
    }
    public Dictionary<int, double> getdict()
    {
        return bincapmap;
    }


    
    public void smallChange()
    {
        Random rnd = new Random();
        int targetbin = rnd.Next(1, binamount);//finds a bin to move to
        int itemtomove = rnd.Next(1, 30);//finds an item to move
        (int,double, bool) currentbin = (0,0,false);//intialises variables for the current bin the item to move is in
        (int, double, bool) moveitem = (0, 0, false);
        (int, double, bool) bintarget= (0, 0, false);
        LinkedListNode<(int, double, bool)> currentBinNode = null;

        var ctuple = binlist.First;//current bin tuple is the first bin in list
    
        while (ctuple != null)//while bin list is not empty
        {
            (int binnumber, double binweight, bool isBin) = ctuple.Value;
            if (binnumber == itemtomove && !isBin)
            {
               
                while (binnumber == targetbin)//if the item is already in the bin we want tio move to we change the target bin
                {
                    targetbin = rnd.Next(1, binamount);
                    
                    //Console.WriteLine("target bin had to be changed to " + targetbin);
                }

               
                break;//break when item is found as currrent bin will then be found
            }
            if (isBin)
            {
                currentBinNode = ctuple;//will store the last bin until item is found
            }

            ctuple = ctuple.Next;
        }
       
        
      

        var itmtuple = binlist.First;//same premise to find the item to move and set its variables

        while (itmtuple != null)
        {
            (int binnumber, double binweight, bool isBin) = itmtuple.Value;
            if (binnumber == itemtomove && !isBin)
            {
                moveitem = itmtuple.Value;
                break;
            }

            itmtuple = itmtuple.Next;
        }

        var tbtuple = binlist.First;

        while (tbtuple != null)//same premise to find the data for the target bin
        {
            (int binnumber, double binweight, bool isBin) = tbtuple.Value;
            if (binnumber == targetbin && isBin)
            {

                bintarget = tbtuple.Value;
                break;
            }
            
            tbtuple = tbtuple.Next;
        }

        //Console.WriteLine(moveitem.ToString()+"attempt to move from"+ currentBinNode.Value.ToString()+ "to"+ bintarget.ToString());
        
        (int ibinnumber, double ibinweight, bool iisBin) = moveitem;
        (int tbinnumber, double tbinweight, bool tisBin) = bintarget;

        if (bincapmap[tbinnumber] > ibinweight && binamount//if the capacity of the target bin allows for the item to move, and movement is possible
            >2)
        {
            bincapmap[currentBinNode.Value.Item1] += ibinweight;//we add the weight back onto the current bin
            bincapmap[tbinnumber] -= ibinweight;//subtract the weight from the bin we are moving to
            


                binlist.Remove(itmtuple);//remove the item to move from list
                binlist.AddAfter(tbtuple, itmtuple);//add it after the target bin
            
            //Console.WriteLine("Attempt successful");
            if(fitnessbest < fitnessFunction())//if the fitness isnt improved
            {
                bincapmap[currentBinNode.Value.Item1] -= ibinweight;
                bincapmap[tbinnumber] += ibinweight;

                binlist.Remove(itmtuple);
                binlist.AddAfter(currentBinNode, itmtuple);//reverse these changes
                //Console.WriteLine("Attempt did not improve solution");

            }
            else
            {
                fitnessbest = fitnessFunction();
                //else this is the new best fitness
            }
            
        }
        
    }
    
    
    public int getbinamount()
    {
        return binamount;
    }
    public double fitnessFunction()
    {
        double proportion = 0;
      foreach (var bincap in bincapmap)
        {
            if(bincap.Value < 5)
            {
                proportion -= 0.1;
            }
            proportion += bincap.Value / bincapacity;
        }
      return proportion;
      

    }
    class Program
    {

        static void Main()
        {
            string name = "dataset.txt";
            if (File.Exists(name))
            {
                Bin myBin = new Bin();
                StreamReader sr = new StreamReader(File.OpenRead(name));
                int i = 0;
                while (!sr.EndOfStream)
                {
                    string[] weight = sr.ReadLine().Split(' ');
                    double w = Convert.ToDouble(weight[0]);
                    i++;
                    myBin.addtoBin(w, i);

                }
                foreach ((int binnumber, double binweight, bool isBin) in myBin.binlist)
                {
                    if (isBin)
                    {
                        Console.WriteLine("Bin" + binnumber + "\nRemaining Capacity: " + myBin.bincapmap[binnumber]);
                    }
                    else
                    {
                        Console.WriteLine("\tItem :" + binnumber + " Weight: " + binweight);
                    }
                }
                myBin.fitnessbest = myBin.fitnessFunction();
                double firstfitness = myBin.fitnessbest;
                Console.WriteLine(firstfitness);
                for (i = 0; i < 200; i++)
                {
                    myBin.smallChange();
                    Console.WriteLine(myBin.fitnessFunction());
                }
               

            }
        }

    }
}
