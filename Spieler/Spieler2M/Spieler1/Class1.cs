using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WindowsFormsApplication5
{
    public class Spieler2_Monopoly : KI_Monopoly, IKI_Monopoly
    {
        bool FELDWAHRSCHEINLICHKEIT = false;
        int temp = 0;
        int[] Data;

        bool First_Aufruf = true;

        // Feldwahrscheinlichkeiten
        int[] Wahrscheinlich;
        int AnzWahrscheinlich = 1;
        int HighestWahrscheinlich = 0;

        int HighestPreis = 0;

        override public void OnFailure()
        {
            // wird bei fehlern aufgerufen
        }

        public Spieler2_Monopoly()
        {
            FehlerAktiv = true;
            Name = "Till";
        }

        private int get_Ke()
        {
            int Ke = 0;
            for (int i = 0; i < Felder.Count(); i++)
            {
                if (IsStraße(i) && GetStraßenBesitzer(i) == GetFarbe())
                {
                    Ke += (int) (GetWertHaeuser(i) * 0.9f);
                }
            }
            return Ke;
        }

        private int get_Kh()
        {
            int Wert = 0;
            List<Straße> list = GetStraßenVonSpieler(GetFarbe());
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Hypothek)
                {
                    Wert += list[i].Preis / 2;
                }
            }
            return Wert;
        }

        private int Straße_Wert(int Position)
        {
            if (IsStraße(Position))
            {
                float Kv = Money * 1.0f;
                float Ke = get_Ke() * 0.5f;
                float Kh = get_Kh() * 0.25f;
                int B = GetStraßenPreis(Position);
                if (Kv + Ke + Kh < B) return 0;
                float Fb = GetAnzahlStraßenPartnerBesitz(Position, GetFarbe()); Fb = Fb == 0 ? 0 : Fb == 1 ? 2 : Fb == 2 ? 4 : 4; // 40%
                float Fp = Wahrscheinlich[Position] / HighestWahrscheinlich * 4.0f; // 40%
                float Fe = Straßen[Felder[Position].Straße].Preis/HighestPreis * 2.0f; // 20%
                float Wert = Fb + Fp + Fe;
                return (int)((Wert / 10) * (Kv + Ke + Kh));;
            }
            else
                return 0;
        }

        private void Geld_auftreiben(int Betrag)
        {
            // noch schreiben
            // noch schreiben
            // noch schreiben
        }

        private int Haus_Wert(int Position)
        {
            // noch schreiben
            // noch schreiben
            // noch schreiben
            return 0;
        }

        private void Hypotheken_auflösen()
        {
            // noch schreiben
            // noch schreiben
            // noch schreiben
        }

        override public void Aufruf()
        {
            if (First_Aufruf)
            {
                First_Aufruf = false;

                Wahrscheinlich = new int[Felder.Count()];
                if (File.Exists("TillMonopolyWahrscheinlichkeit.dat"))
                {
                    StreamReader datei = new StreamReader("TillMonopolyWahrscheinlichkeit.dat");
                    AnzWahrscheinlich = Convert.ToInt32(datei.ReadLine());
                    HighestWahrscheinlich = Convert.ToInt32(datei.ReadLine());
                    for (int i = 0; !datei.EndOfStream; i++) Wahrscheinlich[i] = Convert.ToInt32(datei.ReadLine());
                }
                else
                {
                    for (int i = 0; i < Wahrscheinlich.Count(); i++) Wahrscheinlich[i] = 0;
                    AnzWahrscheinlich = 1;
                    HighestWahrscheinlich = 1;
                }

                for (int i = 0; i < Straßen.Count(); i++)
                {
                    if (Straßen[i].Miete[0] > HighestPreis && IsStraße(Straßen[i].Feld))
                    {
                        HighestPreis = Straßen[i].Miete[0];
                    }
                }
            }

            if ((FELDWAHRSCHEINLICHKEIT || !File.Exists("TillMonopolyWahrscheinlichkeit.dat")) && temp < 1000000)
            {
                FELDWAHRSCHEINLICHKEIT=true;
                if (temp == 0) Data = new int[Felder.Count()];
                if (IsStraße(EigenePosition))
                {
                    Data[EigenePosition]++;
                    temp++;
                    if (temp % 10000 == 0)
                    {
                        int highest = 0;
                        for (int i = 0; i < Data.Count(); i++)
                            if (Data[i] > highest) highest = Data[i];

                        StreamWriter datei = new StreamWriter("TillMonopolyWahrscheinlichkeit.dat");
                        datei.WriteLine(temp.ToString());
                        datei.WriteLine(highest.ToString());
                        for (int i = 0; i < Data.Count(); i++)
                            datei.WriteLine(Data[i].ToString());
                        datei.Close();
                    }
                }
            }

            if (!FELDWAHRSCHEINLICHKEIT)
            {
                // Straße kaufen
                if (IsStraße(EigenePosition) && GetStraßeFrei(EigenePosition))
                {
                    if (Straße_Wert(EigenePosition) >= GetStraßenPreis(EigenePosition))
                    {
                        // kaufe die Straße
                        if (Money < GetStraßenPreis(EigenePosition)) Geld_auftreiben(GetStraßenPreis(EigenePosition) - Money);
                        if (Money>=GetStraßenPreis(EigenePosition))StraßeKaufen(EigenePosition);
                    }}

                // Haus bauen
                if (IsStraße(EigenePosition) && GetStraßenBesitzer(EigenePosition) == GetFarbe())
                {
                    if (Straße_Komplett(EigenePosition))
                    {
                        List<int> list = GetFelderPartner(EigenePosition);
                        for (int i=0;i<list.Count;i++){
                            if (KannSpielerHausBauen(list[i], GetFarbe()))
                            {
                                if (Haus_Wert(EigenePosition) >= GetHausBauPreis(EigenePosition))
                                {
                                    // baue das Haus
                                    if (Money < GetHausBauPreis(EigenePosition)) Geld_auftreiben(GetHausBauPreis(EigenePosition) - Money);
                                    if (Money >= GetHausBauPreis(EigenePosition)) HausBauen(EigenePosition);
                                }}}}}

                if (Money < 0) Geld_auftreiben(-Money);
            }

        }

    }

}
