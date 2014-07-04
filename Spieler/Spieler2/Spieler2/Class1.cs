using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication5
{
        public class Spieler2 : KI, IKI
        {
            override public void OnFailure()
        {
            for (int i = 0; i < 44; i++) SystemMessage(Convert.ToString(i) + "--" + Convert.ToString(Spielfeld[i]));
            
        }

               public Spieler2()
        {
            FehlerAktiv = true;
            Name = "Till";
        }

               int RankSpieler(int ID) // Farbe übergeben
               {
                   ID--;
                   int wert = 0;
                   for (int i = 0; i < 40; i++)
                   {
                       if (Spielfeld[i] == ID + 1)
                       {
                           wert += (40 + (i - ID) * 10) % 40;
                       }
                   }

                   for (int i = 0; i < 4; i++)
                       if (KI.Saved[ID, i] > 0)
                           wert += 40 + i;

                   return wert;
               }

               int ConvertStein(int Pos, int Player)
               {
                  return (40 + Pos - (Player - GetFarbe()) * 10)%40;
               }

               bool OnFieleMove(int ID, int Wuerfel)
               {
                   return Spielfeld[GetEigenePosition(ID) + Wuerfel] == GetFarbe() ? false : true;
               }

               int GegnerAufmFeld(int ID)
               {
                   int summ = 0;
                   for (int i = 0; i < 40; i++)
                   {
                       if (Spielfeld[i] == ID) summ++;
                   }
                   return summ;
               }

               double GetBedrohung(int pos)
               {
                   double wert = 0;
                   if (pos >= 40 || pos < 0) return wert;
                   if (pos == 4 || pos == 14 || pos == 24 || pos == 34) return wert;

                   // Gegnerisches Haus prüfen
                       if (pos % 10 == 0 && pos != 0)
                       {
                           int ID = (GetFarbe() + pos%10) % 4  - 1;
                           if (Freie[pos%10]>0){
                               // kann mich normal werfen
                           wert += 1/6;
                           bool check = false;
                           int add = Spielfeld[pos] > 0 && Spielfeld[pos] != GetFarbe() ? 1 : 0;
                           if (GegnerAufmFeld(ID + 1)-add > 0) check = true;
                           for (int b = 2; b > 0 && check == false; b--)
                           {
                               if (Saved[ID, b + 1] <= 0 && Saved[ID, b] > 0)
                               {
                                   check = true;
                               }
                           }

                           if (check == false)
                           { // Gegner darf 3 mal würfeln
                               wert += 2/6;
                           }
                           }
                       }

                   for (int i = 0; i < 40; i++)
                   {
                       if (i == pos) continue;

                       if (Spielfeld[i] <= 0 || Spielfeld[i]==GetFarbe()) continue;
                       if (i > pos)
                       {
                           wert += (double)1 / pot((int) ((double)(i + 40 - pos) / 6) + 1);
                       }
                       else
                       {
                           wert += (double)1 / pot((int) ((double)(i - pos) / 6) + 1);
                       }
                   }
                   return wert;
               }

               int pot(int anz)
               {
                   int summ = 6;
                   for (int i = 1; i < anz; i++) summ *= 6;
                   return summ;
               }

               override public int Aufruf(int Wuerfel)
               {
                   // Quellcode
                   double[] Bedrohung = new double[4];
                   double[] NextBedrohung = new double[4];
                //   double[] Better = new double[4];
                   if (!BewegungEinerMoeglich(Wuerfel)) { return 5; }//SystemMessage("Keine Bewegung möglich"); 

                   if (Spielfeld[0] == GetFarbe())
                   {
                       int start = -1;
                       for (int i = 0; i < 4; i++) if (GetEigenePosition(i) == 0) { start = i; break; }
                       if (start >= 0 && BewegungMoeglich(start, Wuerfel) && OnFieleMove(start, Wuerfel))
                       {                         
                           return start;
                       }
                   }

                   // 1. Überprüfen ob Save möglich
                   int wide = -1;
                   for (int i = 0; i < 4; i++)
                   {
                       if (GetEigenePosition(i) <= -1 || !BewegungMoeglich(i, Wuerfel)) continue;
                       if (GetEigenePosition(i) + Wuerfel >= 40 && BewegungMoeglich(i, Wuerfel) && OnFieleMove(i, Wuerfel))
                       {
                           if (wide == -1)
                           {
                               wide = i;
                           }
                           else
                               if (GetEigenePosition(i) + Wuerfel > GetEigenePosition(wide) + Wuerfel) wide = i;
                       }
                   }
                   if (wide > -1) {  return wide; }

                   // 2. ob Figur raussetzen
                   if (Wuerfel == 6 && Spielfeld[0] != GetFarbe() && GetEigeneFigurenAnzahl() - GetEigeneSave() < 2 && GetEigeneFrei() > 0) return 4;

                   // 3. Bewegen
                   wide = -1;
                   for (int i = 0; i < 4; i++)
                   {
                       if (!BewegungMoeglich(i, Wuerfel)) continue; 

                      if (GetEigenePosition(i) <= -1) continue;
                       Bedrohung[i] = GetBedrohung(GetEigenePosition(i));
                       NextBedrohung[i] = GetBedrohung(GetEigenePosition(i) + Wuerfel);
                           if (wide == -1)
                           {
                               wide = i;
                           }
                           else
                           {
                               if (Bedrohung[i] - NextBedrohung[i] > Bedrohung[wide] - NextBedrohung[wide])
                               {
                                   wide = i;
                               }
                       }
                   }
                   if (wide > -1) return wide;

                   if (Wuerfel == 6 && Spielfeld[0] != GetFarbe() && GetEigeneFrei() > 0) return 4;
                   return 5;
               }

        }

    }

