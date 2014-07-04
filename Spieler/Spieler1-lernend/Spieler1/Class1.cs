using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WindowsFormsApplication5
{
    public class Spieler1 : KI, IKI
    {
        int[] dat = {7, // 0 - 30
                     10, // 0 - 30
                     5, // 0 - 30
                    };

       /* int[] dat = {15, // 0 - 30
                     11, // 0 - 30
                     8, // 0 - 30
                    };*/

        int[] datmax = { 30, 20, 24 };
        int oldwert = 0;
        int oldgame = 0;

        bool lernen = true;

        override public void OnFailure()
        {
            for (int i = 0; i < 44; i++) SystemMessage(Convert.ToString(i) + "--" + Convert.ToString(Spielfeld[i]));
        }

        public Spieler1()
        {
            FehlerAktiv = true;
            Name = "Anton";
        }
        private int Gefahrlevel(int pos)
        {
            int Achtung = 0;
            if (pos > 39)
                return -25;
            for (int i = 0; i < 4; i++)
                if(pos==4+10*i)
                    return 0;
            for (int i = 1; i <dat[0]; i++)
                if (pos - i >= 0)
                    if (Spielfeld[pos - i] != GetFarbe() && Spielfeld[pos - i] != 0)
                        Achtung++;
            if (pos % 10 == 0)
                Achtung++;
            //return 1; // geändert
            return Achtung;
        }

        private int GetValue(int pos, int Player)
        {
            return (40 + pos - (Player - GetFarbe()) * 10) % 40; 
        }

        private bool IsBest(int TurnA, int TurnB, int TurnC, int TurnD)
        {
            if (TurnA >= TurnB && TurnA >= TurnC && TurnA >= TurnD)
                return true;
            return false;
        }

        private int GesamtSpiele()
        {
            int summ = 0;
            for (int i = 0; i < 4; i++) summ += SpieleGewonnen[i];
            return summ;
        }

        private int summdat(int [] dat)
        {
            int summ = 0;
            for (int i = 0; i < dat.Length; i++) summ+= dat[i];
            return summ;
        }

        override public int Aufruf(int Wuerfel)
        {
            int[] TurnValue = { 0, 0, 0, 0 };
            int[] OwnValue = { 0, 0, 0, 0 };

            // Lernprozess
            #region LERNEN
             int Gesamt = GesamtSpiele();
            if (Gesamt == 0 || oldgame != Gesamt)
            {
                oldgame = Gesamt;
                if (Gesamt % 500 == 0 && lernen)
                {
                    // neuen Datensatz speichern
                    if (File.Exists("Anton.dat"))
                    {
                        StreamReader myFile = new StreamReader("Anton.dat");
                        lernen = myFile.ReadLine() == "Fertig" ? false : true;
                        if (Gesamt == 0)
                        {
                            if (lernen)
                            {
                                myFile.ReadLine();
                                for (int i = 0; i < dat.Count(); i++) myFile.ReadLine();
                                myFile.ReadLine();
                                for (int i = 0; i < dat.Count(); i++) dat[i] = Convert.ToInt32(myFile.ReadLine());
                                myFile.Close();
                            }
                            else
                            {
                                myFile.ReadLine();
                                for (int i = 0; i < dat.Count(); i++) dat[i] = Convert.ToInt32(myFile.ReadLine());
                                myFile.Close();
                            }
                        }
                        else
                        {
                            int[] dat2 = new int[dat.Length];
                            // int[] dat3 = new int[dat.Length];
                            int[] dat4 = new int[dat.Length];
                            for (int i = 0; i < dat.Count(); i++) { dat2[i] = 0; dat4[i] = 0; }

                            int a = Convert.ToInt32(myFile.ReadLine());
                            for (int i = 0; i < dat.Count(); i++) dat2[i] = Convert.ToInt32(myFile.ReadLine());
                            int b = Convert.ToInt32(myFile.ReadLine());
                            for (int i = 0; i < dat.Count(); i++) { dat4[i] = dat[i]; }
                            myFile.Close();

                            dat4[0]++;
                            for (int i = 0; i < dat.Length && dat4[i] >= datmax[i]; i++)
                            {
                                dat4[i] = 0;
                                if (i + 1 < dat.Length) dat4[i + 1]++;
                            }

                            StreamWriter myFile2 = new StreamWriter("Anton.dat");
                            if (summdat(dat4) == 0 || !lernen) { myFile2.WriteLine("Fertig"); } else myFile2.WriteLine("-"); // Fertig?
                            if (a < InSave[GetFarbe() - 1] - oldwert)
                            {                        // new best
                                myFile2.WriteLine((InSave[GetFarbe() - 1] - oldwert));
                                for (int i = 0; i < dat.Count(); i++) myFile2.WriteLine(dat[i]);
                                myFile2.WriteLine((InSave[GetFarbe() - 1] - oldwert));
                                for (int i = 0; i < dat.Count(); i++) myFile2.WriteLine(dat[i]);
                            }
                            else
                            {
                                myFile2.WriteLine(a);
                                for (int i = 0; i < dat.Count(); i++) myFile2.WriteLine(dat2[i]);
                                myFile2.WriteLine((InSave[GetFarbe() - 1] - oldwert));
                                for (int i = 0; i < dat.Count(); i++) myFile2.WriteLine(dat[i]);
                            }
                            oldwert = InSave[GetFarbe() - 1];
                            for (int i = 0; i < dat.Count(); i++) dat[i] = dat4[i];
                            myFile2.Close();
                        }
                    }
                    else
                    {
                        dat[0] = 3;
                        dat[1] = 3;
                        dat[2] = 3;
                        StreamWriter myFile = new StreamWriter("Anton.dat");
                        myFile.WriteLine("-"); // Fertig?
                        // best
                        myFile.WriteLine(0); // Wert
                        myFile.WriteLine(0); // dat[0]
                        myFile.WriteLine(0); // dat[1]
                        myFile.WriteLine(0); // dat[2]
                        // last
                        myFile.WriteLine(0); // Wert
                        myFile.WriteLine(3); // dat[0]
                        myFile.WriteLine(3); // dat[1]
                        myFile.WriteLine(3); // dat[2]
                        myFile.Close();
                    }

                }
            }
            #endregion

            if (BewegungEinerMoeglich(Wuerfel))
            {
                //Start freimachen
                for (int i = 0; i < 4; i++)
                    if (GetEigenePosition(i) == 0)
                    {
                //kann nicht freimachen
                        if (!BewegungMoeglich(i, Wuerfel))
                            break;
                        return i;
                    }
                //Figur setzen
                if (GetEigeneFrei() != 0 && Wuerfel == 6&&Spielfeld[0]!=GetFarbe())
                {
                    return 4;
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!BewegungMoeglich(i, Wuerfel))
                    {
                        TurnValue[i] = -10000000;
                        continue;
                    }
                    //in welche Gefahr begebe ich mich?
                    int Achtung = Gefahrlevel(GetEigenePosition(i) + Wuerfel) - Gefahrlevel(GetEigenePosition(i));
                    int Valuediff = 0;
                    OwnValue[i] = GetValue(GetEigenePosition(i) + Wuerfel, GetFarbe());

                    //kann ich einen Gegner werfen?
                    if (Spielfeld[GetEigenePosition(i) + Wuerfel] != GetFarbe() && Spielfeld[GetEigenePosition(i) + Wuerfel] != -1)
                    {
                        Valuediff = GetValue(GetEigenePosition(i) + Wuerfel, Spielfeld[GetEigenePosition(i) + Wuerfel]);
                        //Valuediff = 1;
                        //if(Achtung>0)//lohnt es sich diesen Stein für den anderen zu opfern?
                        //Valuediff -= OwnValue[i];
                    }

                    TurnValue[i] = -Achtung * (OwnValue[i] / 10 + Valuediff*10);
                    if (GetEigenePosition(i) >= 40) // Im Save? + 10
                        TurnValue[i] += dat[1];

                    for (int j = 0; j < 4; j++) // Sicherheitsfeld? + 5
                        if (GetEigenePosition(i) + Wuerfel == 4 + 10 * j)
                            TurnValue[i] += dat[2];
                }


                for (int i = 0; i < 4; i++)
                {
                    if (IsBest(OwnValue[i], OwnValue[(i + 1) % 4], OwnValue[(i + 2) % 4], OwnValue[(i + 3) % 4]))
                        TurnValue[i]++;
                    if (IsBest(TurnValue[i], TurnValue[(i + 1) % 4], TurnValue[(i + 2) % 4], TurnValue[(i + 3) % 4]))
                        return i;
                }
            }
            return 5;
        }

    }

}
