using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace WindowsFormsApplication5
{
    public class DatenPaket
    {
        public List<String> SystemMessage = new List<String>();
        public List<String> SystemMessageF = new List<String>();
    }

    public interface IKI_Monopoly
    {
        void Aufruf();
        void OnFailure();
    }

    // Letzter Fehler ------ > 68
    // Letzter Fehler ------ > 68
    // Letzter Fehler ------ > 68
    // Letzter Fehler ------ > 68
    // Letzter Fehler ------ > 68

    public class Straße
    {
        public String Name;
        public int Preis;
        public int Partner;
        public int Partner2;
        public int Partner3;
        public int[] Miete = new int[6];
        public int HausPreis;
        public int HotelPreis;
        public int Besitzer = 0;
        public bool Hypothek = false;
        public int Haus = 0;
        public int Feld = 0;
        public int Typ = 0; // 0 == Straße, 1 == Bahnhof, 2 == Elek./Wasserwerk

        public Straße(String Bezeichnung, int Kaufpreis, int Preis0, int Preis1, int Preis2, int Preis3, int Preis4, int Preis5, int PreisHaus, int PreisHotel, int Sorte, int Nachbarstraße, int Nachbarstraße2, int Nachbarstraße3)
        {
            Name = Bezeichnung;
            Preis = Kaufpreis;
            Partner = Nachbarstraße;
            Partner2 = Nachbarstraße2;
            Partner3 = Nachbarstraße3;
            HausPreis = PreisHaus;
            HotelPreis = PreisHotel;
            Miete[0] = Preis0;
            Miete[1] = Preis1;
            Miete[2] = Preis2;
            Miete[3] = Preis3;
            Miete[4] = Preis4;
            Miete[5] = Preis5;
            Typ = Sorte;
        }
    }

    public class Feld
    {
        public int Typ = 0; // 0 == Straße, 1 == Ereignisfeld, 2 == Gemeinschaftsfeld, 3 == Start, 4 == Freies Parken, 5 == Gefängnis, 6 == Gehe ins Gefängnis, 7 == Steuer
        public int Straße = 0;
        public Feld(int Sorte, int Information)
        {
            Typ = Sorte;
            Straße = Information;
        }
    }

    abstract public class KI_Monopoly : IKI_Monopoly
    {
        public static Straße[] Straßen = new Straße[28];
        public static Feld[] Felder = new Feld[40];

        public class Secure<T>
        {
            private T[] arr;
            public Secure(int anz)
            {
                arr = new T[anz];
            }

            public T this[int i]
            {
                get
                {
                    return arr[i];
                }
                set
                {
                    if (Sec == Sec2) { arr[i] = value; } else SystemMessage("Betrug 2");
                }
            }
        }
        public class Secure2<T>
        {
            private T[,] arr;
            public Secure2(int anz, int anz2)
            {
                arr = new T[anz, anz2];
            }

            public T this[int i, int a]
            {
                get
                {
                    return arr[i, a];
                }
                set
                {
                    if (Sec == Sec2) { arr[i, a] = value; } else SystemMessage("Betrug 3");
                }
            }
        }

        public int Secured
        {
            set { if (sec_set == false) { Sec = value; sec_set = true; } else { Sec2 = value; } }

            get { SystemMessage("Betrug 1"); return -1; }
        }

        public bool Straße_Komplett(int Feld)
        { // id = Feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 40: Fehlerhafte FeldID!"); return false; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 41: Das ist keine Straße!"); return false; }
            Feld = Felder[Feld].Straße;
            int Besitzer = Straßen[Feld].Besitzer;
            if (Besitzer <= 0) return false;
            if (Straßen[Feld].Partner > 0 && Straßen[Straßen[Feld].Partner].Besitzer != Besitzer) return false;
            if (Straßen[Feld].Partner2 > 0 && Straßen[Straßen[Feld].Partner2].Besitzer != Besitzer) return false;
            if (Straßen[Feld].Partner3 > 0 && Straßen[Straßen[Feld].Partner3].Besitzer != Besitzer) return false;
            return true;
        }

        public const int FreiParken = 20;
        public const int Gefängnis = 10;
        public const int Start = 0;
        public const int GotoGefängnis = 30;

        public const int Wasserwerk = 20;
        public const int Kraftwerk = 7;
        public const int Südbahnhof = 2;
        public const int Westbahnhof = 10;
        public const int Nordbahnhof = 17;
        public const int Hauptbahnhof = 25;
        public const int Schlossallee = 27;
        public const int Opernplatz = 16;
        public const int Seestraße = 6;

        private static int Sec = -1;
        private static int Sec2 = -1;
        public static Secure<int> Spielfeld = new Secure<int>(6); // enthält die Positionen der Spieler
        private bool InJail2 = false;
        public bool InJail
        {
            set { if (Sec == Sec2) InJail2 = value; }
            get { return InJail2; }
        }
        private int Money2 = 1500;
        public int Money
        {
            set { if (Sec == Sec2) Money2 = value; }
            get { return Money2; }
        }
        private int EigenePosition2 = 0;
        public int EigenePosition
        {
            set { if (Sec == Sec2) EigenePosition2 = value; }
            get { return EigenePosition2; }
        }
        // public int AktuellerSpieler = 0;
        private bool Aktiv2 = true;
        public bool Aktiv
        {
            set { if (Sec == Sec2) Aktiv2 = value; }
            get { return Aktiv2; }
        }
        private bool GefängnisFreiKarte2 = true;
        public bool GefängnisFreiKarte
        {
            set { if (Sec == Sec2) GefängnisFreiKarte2 = value; }
            get { return GefängnisFreiKarte2; }
        }

        public Secure<int> SpieleGewonnen = new Secure<int>(6);
        public Secure<int> Fehler = new Secure<int>(6);
        private KI_Monopoly[] Spieler = new KI_Monopoly[6]; // zuweisen nicht vergessen

        private bool[] is_set = { false, false, false, false, false, false };
        public void SetSpieler(KI_Monopoly Player, int id)
        {
            if (id < 0 || id >= 6) { SystemMessage("Betrug 6"); return; }
            if (is_set[id] == false)
            {
                Spieler[id] = Player;
                is_set[id] = true;
            }
            else
                SystemMessage("Betrug 7");
        } // zum Zuweisen der Zeiger

        private int FarbeS = -1;
        private int Farbe
        {
            set { if (Sec == Sec2) FarbeS = value; }
            get { return FarbeS; }
        }

        protected bool FehlerAktiv = true;
        public static DatenPaket Daten = new DatenPaket();
        public String Name = "Unbenannt";
        private bool sec_set = false;

        public bool GetFehlerAktiv()
        {
            return FehlerAktiv;
        }

        public int GetFarbe()
        {
            return Farbe;
        }

        public static Random rand = new Random();
        private int Wuerfel()
        {
            return KI_Monopoly.rand.Next(1, 7);
        }

        public static void SystemMessage(String Text)
        {
            Daten.SystemMessage.Add(Text);
        }

        public void SystemMessageF(String Text)
        {
            Daten.SystemMessageF.Add(Text);
            for (int i = 0; i < 6; i++)
            {
                if (Spieler[i] == null) continue;
                // Spieler[GetFarbe()].Fehler[GetFarbe() - 1]++;
            }
        }

        private void ProtClear()
        {
            /*  String a = frm1.richTextBox1.Text;
              int first = -1;
              int anz = 0;
              for (int i = 0; i < a.Length; i++)
              {
                  if (a.Substring(i, 1) == "\n")
                  {
                      anz++;
                      if (first == -1) first = i;
                  }
              }

              if (anz >= 28)
              {
                  frm1.richTextBox1.Select(0, first + 1);
                  frm1.richTextBox1.SelectedText = " ";
              }*/
        }

        private void Protokol(String Text)
        {
            //frm1.label1.Text = "Fehler: " + Convert.ToString(frm1.Protokoll_anzahl);
        }

        public void SetFarbe(int Farb)
        {
            if (Farbe == -1) Farbe = Farb;
        }

        public void HausBauen(int Feld)
        {// id == Feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 38: Fehlerhafte FeldID!"); return; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 1: Das ist kein Straße!"); return; }
            if (InJail) { SystemMessageF("Fehler 2: Es kann vom Gefängnis aus kein Haus gebaut werden!"); return; }
            if (Straße_Komplett(Feld) && Straßen[Felder[Feld].Straße].Typ == 0 && Straßen[Felder[Feld].Straße].Besitzer == GetFarbe() && Straßen[Felder[Feld].Straße].Haus < 5 && (EigenePosition == Feld || EigenePosition == Straßen[Felder[Feld].Straße].Partner || EigenePosition == Straßen[Felder[Feld].Straße].Partner2))
            {
                int a = Straßen[Felder[Feld].Straße].Haus;
                int b = Straßen[Felder[Feld].Straße].Partner < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner].Haus;
                int c = Straßen[Felder[Feld].Straße].Partner2 < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner2].Haus;
                int summ = 0;
                int anz = 1;
                if (b > -1 && c == -1) { anz = 2; summ = a + b; }
                if (b > -1 && c > -1) { anz = 3; summ = a + b + c; }
                if (Straßen[Felder[Feld].Straße].Haus < (int)summ / anz)
                {
                    Money2 -= GetHausBauPreis(Feld);
                    SystemMessage("Spieler " + GetFarbe().ToString() + " baut Haus auf " + Straßen[Felder[Feld].Straße].Name + " (" + GetHausBauPreis(Feld).ToString() + ")");
                    Straßen[Felder[Feld].Straße].Haus++;
                }
                else
                    SystemMessageF("Fehler 3: Kein gleichmässiges Bauen!");
            }
            else
            {
                SystemMessageF("Fehler 4: Haus konnte nicht gebaut werden!");
            }
        }

        public void HausAbreissen(int Feld)
        {// id == Feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 37: Fehlerhafte FeldID!"); return; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 5: Das ist kein Straße!"); return; }
            if (InJail) { SystemMessageF("Fehler 6: Es kann vom Gefängnis aus kein Haus abgerissen werden!"); return; }
            if (Straßen[Felder[Feld].Straße].Haus > 0 && Straßen[Felder[Feld].Straße].Besitzer == GetFarbe())
            {
                int a = Straßen[Felder[Feld].Straße].Haus;
                int b = Straßen[Felder[Feld].Straße].Partner < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner].Haus;
                int c = Straßen[Felder[Feld].Straße].Partner2 < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner2].Haus;
                int summ = 0;
                int anz = 1;
                if (b > -1 && c == -1) { anz = 2; summ = a + b; }
                if (b > -1 && c > -1) { anz = 3; summ = a + b + c; }
                if (Straßen[Felder[Feld].Straße].Haus > (int)summ / anz)
                {
                    Money2 += (int)(GetLastHausBauPreis(Feld) * 0.9f);
                    SystemMessage("Spieler " + GetFarbe().ToString() + " reist Haus in " + Straßen[Felder[Feld].Straße].Name + " ab (" + ((int)(GetLastHausBauPreis(Feld) * 0.9f)).ToString() + ")");
                    Straßen[Felder[Feld].Straße].Haus--;
                }
                else
                    SystemMessageF("Fehler 7: Kein gleichmässiges abreissen!");
            }
            else
            {
                SystemMessageF("Fehler 8: Haus konnte nicht abgerissen werden!");
            }
        }

        public int GetMiete(int Feld, int WuerfelSumme)
        {// id == Feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 36: Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 9: Hier gibts keine Miete"); return 0; }
            if (Straßen[Felder[Feld].Straße].Besitzer <= 0) return 0;
            int miet = 0;
            if (Straßen[Felder[Feld].Straße].Typ == 0)
            { // Straße
                miet = Straßen[Felder[Feld].Straße].Miete[Straßen[Felder[Feld].Straße].Haus];
                if (Straße_Komplett(Feld) && Straßen[Felder[Feld].Straße].Haus == 0) miet += Straßen[Felder[Feld].Straße].Miete[Straßen[Felder[Feld].Straße].Haus];
            }
            else
                if (Straßen[Felder[Feld].Straße].Typ == 1)
                { // Bahnhof
                    int owner = Straßen[Felder[Feld].Straße].Besitzer;
                    if (owner > 0)
                    {
                        int hab = 0;
                        hab = Straßen[Straßen[Felder[Feld].Straße].Partner].Besitzer == owner ? 1 : 0 + Straßen[Straßen[Felder[Feld].Straße].Partner2].Besitzer == owner ? 1 : 0 + Straßen[Straßen[Felder[Feld].Straße].Partner3].Besitzer == owner ? 1 : 0;
                        miet = Straßen[Felder[Feld].Straße].Miete[hab];
                    }
                }
                else
                    if (Straßen[Felder[Feld].Straße].Typ == 2)
                    { // Wasserwerk
                        int owner = Straßen[Felder[Feld].Straße].Besitzer;
                        if (Straßen[Straßen[Felder[Feld].Straße].Partner].Besitzer == owner)
                        {
                            miet = WuerfelSumme * Straßen[Felder[Feld].Straße].Miete[1];
                        }
                        else
                        {
                            miet = WuerfelSumme * Straßen[Felder[Feld].Straße].Miete[0];
                        }
                    }
                    else
                    { SystemMessageF("Fehler 10: Hier gibts keine Miete"); return 0; }
            return miet;
        }

        public int GetHausBauPreis(int Feld)
        {// id == feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 35: Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 11: Das ist keine Straße!"); return 0; }
            if (Straßen[Felder[Feld].Straße].Haus >= 5) { SystemMessageF("Fehler 12: Kein Bauplatz Frei!"); return 0; }
            return Straßen[Felder[Feld].Straße].Haus >= 4 ? Straßen[Felder[Feld].Straße].HotelPreis : Straßen[Felder[Feld].Straße].HausPreis;
        }

        public int GetLastHausBauPreis(int Feld)
        {// id == feld
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 34: Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 13: Das ist kein Straße!"); return 0; }
            if (Straßen[Felder[Feld].Straße].Haus == 0) return 0;
            return Straßen[Felder[Feld].Straße].Haus == 5 ? Straßen[Felder[Feld].Straße].HotelPreis : Straßen[Felder[Feld].Straße].HausPreis;
        }

        public Straße GetStraße(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 33: Fehlerhafte FeldID!"); return null; }
            if (!IsStraße(Feld)) return null;
            return Straßen[Felder[Feld].Straße];
        }

        public int GetFeld(int straße)
        {
            if (straße < 0 || straße >= Straßen.Count()) { SystemMessageF("Fehler 61: Fehlerhafte StraßenID!"); return 0; }
            return Straßen[straße].Feld;
        }

        public int GetFeld(Straße straße)
        {
            if (straße == null) { SystemMessageF("Fehler 62: Fehlerhaftes Straßenobjekt!"); return 0; }
            return straße.Feld;
        }

        public bool IsStraße(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 32: Fehlerhafte FeldID!"); return false; }
            return Felder[Feld].Typ == 0 ? true : false;
        }

        public List<Straße> GetStraßenVonSpieler(int SpielerFarbe)
        {
            List<Straße> temp = new List<Straße>();
            if (SpielerFarbe <= 0 || SpielerFarbe >= 7) { SystemMessageF("Fehler 26: SpielerID existiert nicht!"); return null; }
            for (int i = 0; i < Felder.Count(); i++)
                if (IsStraße(i) && Straßen[Felder[i].Straße].Besitzer == SpielerFarbe)
                    temp.Add(Straßen[Felder[i].Straße]);
            return temp;
        }

        public int GetGebauteHaeuserVonSpieler(int SpielerFarbe)
        {
            if (SpielerFarbe <= 0 || SpielerFarbe >= 7) { SystemMessageF("Fehler 27: SpielerID existiert nicht!"); return 0; }
            int summ = 0;
            for (int i = 0; i < Felder.Count(); i++)
                if (IsStraße(i) && Straßen[Felder[i].Straße].Besitzer == SpielerFarbe)
                    summ += Straßen[Felder[i].Straße].Haus;
            return summ;
        }

        public int GetWertHaeuser(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 28: Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 39: Das ist keine Straße!"); return 0; }
            return Straßen[Felder[Feld].Straße].Haus == 5 ? Straßen[Felder[Feld].Straße].HotelPreis + 4 * Straßen[Felder[Feld].Straße].HausPreis : Straßen[Felder[Feld].Straße].HausPreis * Straßen[Felder[Feld].Straße].Haus;
        }

        public int GetStraßenPreis(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 42: Fehlerhafte FeldID"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 43: Das ist keine Straße!"); return 0; }
            return Straßen[Felder[Feld].Straße].Preis;
        }

        public bool GetStraßeFrei(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 44: Fehlerhafte FeldID"); return false; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 45: Das ist keine Straße!"); return false; }
            return Straßen[Felder[Feld].Straße].Besitzer <= 0 ? true : false;
        }

        public int GetStraßenBesitzer(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 46: Fehlerhafte FeldID"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 47: Das ist keine Straße!"); return 0; }
            return Straßen[Felder[Feld].Straße].Besitzer;
        }

        public List<int> GetStraßenPartner(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 48:Fehlerhafte FeldID"); return null; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 49: Das ist keine Straße!"); return null; }
            List<int> temp = new List<int>();
            temp.Add(Felder[Feld].Straße);
            if (Straßen[Felder[Feld].Straße].Partner >= 0) temp.Add(Straßen[Felder[Feld].Straße].Partner);
            if (Straßen[Felder[Feld].Straße].Partner2 >= 0) temp.Add(Straßen[Felder[Feld].Straße].Partner2);
            if (Straßen[Felder[Feld].Straße].Partner3 >= 0) temp.Add(Straßen[Felder[Feld].Straße].Partner3);
            return temp;
        }

        public List<int> GetFelderPartner(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 63:Fehlerhafte FeldID"); return null; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 64: Das ist keine Straße!"); return null; }
            List<int> temp = new List<int>();
            temp.Add(Feld);
            if (Straßen[Felder[Feld].Straße].Partner >= 0) temp.Add(Straßen[Straßen[Felder[Feld].Straße].Partner].Feld);
            if (Straßen[Felder[Feld].Straße].Partner2 >= 0) temp.Add(Straßen[Straßen[Felder[Feld].Straße].Partner2].Feld);
            if (Straßen[Felder[Feld].Straße].Partner3 >= 0) temp.Add(Straßen[Straßen[Felder[Feld].Straße].Partner3].Feld);
            return temp;
        }

        public int GetAnzahlStraßenPartnerBesitz(int Feld, int SpielerFarbe)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 52: Fehlerhafte FeldID"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 53: Das ist keine Straße!"); return 0; }
            if (SpielerFarbe <= 0 || SpielerFarbe >= 7) { SystemMessageF("Fehler 54: SpielerID existiert nicht!"); return 0; }
            int summ = 0;
            if (Straßen[Felder[Feld].Straße].Besitzer == SpielerFarbe) summ++;
            if (Straßen[Felder[Feld].Straße].Partner >= 0 && Straßen[Straßen[Felder[Feld].Straße].Partner].Besitzer == SpielerFarbe) summ++;
            if (Straßen[Felder[Feld].Straße].Partner2 >= 0 && Straßen[Straßen[Felder[Feld].Straße].Partner2].Besitzer == SpielerFarbe) summ++;
            if (Straßen[Felder[Feld].Straße].Partner3 >= 0 && Straßen[Straßen[Felder[Feld].Straße].Partner3].Besitzer == SpielerFarbe) summ++;
            return summ;
        }

        public int GetMaxAnzahlStraßenPartner(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 50: Fehlerhafte FeldID"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 51: Das ist keine Straße!"); return 0; }
            int summ = 1;
            if (Straßen[Felder[Feld].Straße].Partner >= 0) summ++;
            if (Straßen[Felder[Feld].Straße].Partner2 >= 0) summ++;
            if (Straßen[Felder[Feld].Straße].Partner3 >= 0) summ++;
            return summ;
        }

        public bool KannSpielerHausBauen(int Feld, int SpielerFarbe)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 55: Fehlerhafte FeldID"); return false; }
            if (Felder[Feld].Typ != 0) { return false; }
            if (Straßen[Felder[Feld].Straße].Typ != 0) return false;
            if (SpielerFarbe <= 0 || SpielerFarbe > Spieler.Count()) { SystemMessageF("Fehler 57: SpielerID existiert nicht!"); return false; }
            if (Straßen[Felder[Feld].Straße].Typ == 0 && Straße_Komplett(Feld) && GetStraße(Feld).Besitzer == SpielerFarbe && Straßen[Felder[Feld].Straße].Haus < 5)
            {
                int a = Straßen[Felder[Feld].Straße].Haus;
                int b = Straßen[Felder[Feld].Straße].Partner < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner].Haus;
                int c = Straßen[Felder[Feld].Straße].Partner2 < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner2].Haus;
                int summ = 0;
                int anz = 1;
                if (b > -1 && c == -1) { anz = 2; summ = a + b; }
                if (b > -1 && c > -1) { anz = 3; summ = a + b + c; }
                if (Straßen[Felder[Feld].Straße].Haus < (int)summ / anz)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool KannSpielerHausAbreissen(int Feld, int SpielerFarbe)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 58: Fehlerhafte FeldID"); return false; }
            if (Felder[Feld].Typ != 0) { return false; }
            if (Straßen[Felder[Feld].Straße].Typ != 0) return false;
            if (SpielerFarbe <= 0 || SpielerFarbe > Spieler.Count()) { SystemMessageF("Fehler 60: SpielerID existiert nicht!"); return false; }
            if (Straßen[Felder[Feld].Straße].Typ == 0 && GetStraße(Feld).Besitzer == SpielerFarbe && Straßen[Felder[Feld].Straße].Haus > 0)
            {
                int a = Straßen[Felder[Feld].Straße].Haus;
                int b = Straßen[Felder[Feld].Straße].Partner < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner].Haus;
                int c = Straßen[Felder[Feld].Straße].Partner2 < 0 ? -1 : Straßen[Straßen[Felder[Feld].Straße].Partner2].Haus;
                int summ = 0;
                int anz = 1;
                if (b > -1 && c == -1) { anz = 2; summ = a + b; }
                if (b > -1 && c > -1) { anz = 3; summ = a + b + c; }
                if (Straßen[Felder[Feld].Straße].Haus > (int)summ / anz)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void GemeinschaftsKarteZiehen()
        {
            int zufall = KI_Monopoly.rand.Next(0, 16);
            switch (zufall)
            {
                case 0:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 100 Euro");
                    Money2 += 100;
                    break;

                case 1:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält Gefängnis-Frei-Karte");
                    GefängnisFreiKarte2 = true;
                    break;

                case 2:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht zum Start");
                    Money2 += 200;
                    EigenePosition = 0;
                    break;

                case 3:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt 50 Euro");
                    Money2 -= 50;
                    break;

                case 4:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 20 Euro");
                    Money2 += 20;
                    break;

                case 5:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält von jedem Mitspieler 10 Euro");
                    for (int i = 0; i < 6; i++)
                    {
                        if (Spieler[i] == null || i + 1 == GetFarbe()) continue;
                        Money2 += 10;
                        Spieler[i].Money2 -= 10;
                    }
                    break;

                case 6:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht ins Gefängnis");
                    InJail2 = true;
                    JailCooldown = 3;
                    EigenePosition = KI_Monopoly.Gefängnis;
                    break;

                case 7:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 100 Euro");
                    Money2 += 100;
                    break;

                case 8:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt 100 Euro");
                    Money2 -= 100;
                    break;

                case 9:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 100 Euro");
                    Money2 += 100;
                    break;

                case 10:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 50 Euro");
                    Money2 += 50;
                    break;

                case 11:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt für jedes Haus 40 Euro und jedes Hotel 115 Euro");
                    for (int i = 0; i < 28; i++)
                    {
                        if (Straßen[i].Typ != 0 || Straßen[i].Besitzer != GetFarbe()) continue;
                        Money2 -= Straßen[i].Haus <= 4 ? Straßen[i].Haus * 40 : 115;
                    }
                    break;

                case 12:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt 50 Euro");
                    Money2 -= 50;
                    break;

                case 13:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 25 Euro");
                    Money2 += 25;
                    break;

                case 14:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 200 Euro");
                    Money2 += 200;
                    break;

                case 15:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 10 Euro");
                    Money2 += 10;
                    break;
            }
        }

        int größte(int a, int b, int c, int d)
        {
            if (a >= b && a >= c && a >= d) return a;
            if (b >= a && b >= c && b >= d) return b;
            if (c >= b && c >= a && c >= d) return c;
            return d;
        }

        private void EreignisKarteZiehen(int Wuerfel, int Wuerfel1)
        {
            int zufall = KI_Monopoly.rand.Next(0, 16);
            switch (zufall)
            {
                case 0:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 50 Euro");
                    Money2 += 50;
                    break;

                case 1:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält Gefängnis-Frei-Karte");
                    GefängnisFreiKarte2 = true;
                    break;

                case 2:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht zum Start");
                    Money2 += 200;
                    EigenePosition = 0;
                    break;

                case 3:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt 15 Euro");
                    Money2 -= 15;
                    break;

                case 4:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " erhält 150 Euro");
                    Money2 += 150;
                    break;

                case 5:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt an jedem Mitspieler 50 Euro");
                    for (int i = 0; i < 6; i++)
                    {
                        if (Spieler[i] == null || i + 1 == GetFarbe() || !Spieler[i].Aktiv2) continue;
                        Money2 -= 50;
                        Spieler[i].Money2 += 50;
                    }
                    break;

                case 6:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht ins Gefängnis");
                    InJail2 = true;
                    JailCooldown = 3;
                    EigenePosition = KI_Monopoly.Gefängnis;
                    break;

                case 7:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht 3 Schritte zurück");
                    EigenePosition2 -= 3; if (EigenePosition2 < 0) EigenePosition2 = 40 + EigenePosition2;
                    Bewegen(0, 0);
                    break;

                case 8:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zum Opernplatz");
                    if (EigenePosition2 > KI_Monopoly.Opernplatz) Money2 += 200;
                    EigenePosition2 = KI_Monopoly.Opernplatz;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 9:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zum Südbahnhof");
                    if (EigenePosition2 > KI_Monopoly.Südbahnhof) Money2 += 200;
                    EigenePosition2 = KI_Monopoly.Südbahnhof;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 10:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zur Schlossallee");
                    if (EigenePosition2 > KI_Monopoly.Schlossallee) Money2 += 200;
                    EigenePosition2 = KI_Monopoly.Schlossallee;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 11:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt für jedes Haus 25 Euro und jedes Hotel 100 Euro");
                    for (int i = 0; i < 28; i++)
                    {
                        if (Straßen[i].Typ != 0 || Straßen[i].Besitzer != GetFarbe()) continue;
                        Money2 -= Straßen[i].Haus <= 4 ? Straßen[i].Haus * 25 : 100;
                    }
                    break;

                case 12:
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zur Seestraße");
                    if (EigenePosition2 > KI_Monopoly.Seestraße) Money2 += 200;
                    EigenePosition2 = KI_Monopoly.Seestraße;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 13:
                    int a = KI_Monopoly.Südbahnhof - EigenePosition2; int b = KI_Monopoly.Westbahnhof - EigenePosition2; int c = KI_Monopoly.Nordbahnhof - EigenePosition2; int d = KI_Monopoly.Hauptbahnhof - EigenePosition2;
                    int temp = größte(a, b, c, d) + EigenePosition2;
                    if (temp < 0) temp = 40 + temp;
                    int str = Felder[temp].Straße;
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zum " + Straßen[str].Name);
                    if (EigenePosition2 > temp) Money2 += 200;
                    EigenePosition2 = temp;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 14:
                    int a2 = KI_Monopoly.Südbahnhof - EigenePosition2; int b2 = KI_Monopoly.Westbahnhof - EigenePosition2; int c2 = KI_Monopoly.Nordbahnhof - EigenePosition2; int d2 = KI_Monopoly.Hauptbahnhof - EigenePosition2;
                    int temp2 = größte(a2, b2, c2, d2) + EigenePosition2;
                    if (temp2 < 0) temp2 = 40 + temp2;
                    int str2 = Felder[temp2].Straße;
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zum " + Straßen[str2].Name);
                    if (EigenePosition2 > temp2) Money2 += 200;
                    EigenePosition2 = temp2;
                    Test_zahlen(Wuerfel, Wuerfel1);
                    Test_zahlen(Wuerfel, Wuerfel1);
                    break;

                case 15:
                    int a3 = KI_Monopoly.Wasserwerk - EigenePosition; int b3 = KI_Monopoly.Kraftwerk - EigenePosition2;
                    int temp3 = größte(a3, b3, -100, -100) + EigenePosition2;
                    if (temp3 < 0) temp3 = 40 + temp3;
                    int str3 = Felder[temp3].Straße;
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zieht zum " + Straßen[str3].Name);
                    if (EigenePosition2 > temp3) Money2 += 200;
                    EigenePosition2 = temp3;
                    // Test_zahlen(Wuerfel, Wuerfel1);
                    int a4 = this.Wuerfel(); int b4 = this.Wuerfel();
                    if (Straßen[temp3].Besitzer > 0 && Straßen[temp3].Besitzer != GetFarbe())
                    {
                        Money2 -= (a4 + b4) * 10;
                    }
                    break;
            }
        }

        public void GefängnisFreiKarteNutzen()
        {
            if (GefängnisFreiKarte2)
            {
                GefängnisFreiKarte2 = false;
                InJail = false;
                JailCooldown = 0;
            }
            else
                SystemMessageF("Fehler 14: Es ist keine GefängnisFreiKarte vorhanden!");
        }

        public void GefängnisFreikaufen()
        {
            if (InJail)
            {
                Money2 -= 50;
                InJail = false;
                JailCooldown = 0;
                SystemMessage("Spieler " + GetFarbe().ToString() + " kauft sich aus dem Gefängnis frei");
            }
            else
                SystemMessageF("Fehler 15: Spieler befindet sich nicht im Gefängnis!");
        }

        public void HypothekErhalten(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 29: Fehlerhafte FeldID!"); return; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 16: Das ist kein Straße!"); return; }
            Feld = Felder[Feld].Straße;
            if (Straßen[Feld].Besitzer != GetFarbe()) { SystemMessageF("Fehler 17: Straße gehört einem anderen Spieler!"); return; }
            if (Straßen[Feld].Hypothek == true) { SystemMessageF("Fehler 18: Diese Straße ist bereits mit einer Hypothek belegt!"); return; }
            Straßen[Feld].Hypothek = true;
            Money2 += Straßen[Feld].Preis / 2;
            SystemMessage("Spieler " + GetFarbe().ToString() + " nutzt Hypothek für " + Straßen[Feld].Name);
        }

        public void HypothekAufheben(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 30: Fehlerhafte FeldID!"); return; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 19: Das ist kein Straße!"); return; }
            Feld = Felder[Feld].Straße;
            if (Straßen[Feld].Besitzer != GetFarbe()) { SystemMessageF("Fehler 20: Straße gehört einem anderen Spieler!"); return; }
            if (Straßen[Feld].Hypothek == false) { SystemMessageF("Fehler 21: Diese Straße hat keine Hypothek!"); return; }
            Straßen[Feld].Hypothek = false;
            Money2 -= (int)((double)Straßen[Feld].Preis / 2 + Straßen[Feld].Preis * 0.1f);
            SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt Hypothek zurück für " + Straßen[Feld].Name);
        }

        public float GetHypothekAufheben(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 65 Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 66: Das ist kein Straße!"); return 0; }
            return Straßen[Felder[Feld].Straße].Preis / 2 + Straßen[Felder[Feld].Straße].Preis * 0.1f;
        }

        public float GetHypothekErhalten(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 67: Fehlerhafte FeldID!"); return 0; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 68: Das ist kein Straße!"); return 0; }
            return Straßen[Felder[Feld].Straße].Preis / 2;
        }

        public void StraßeKaufen(int Feld)
        {
            if (Feld < 0 || Feld >= Felder.Count()) { SystemMessageF("Fehler 31: Fehlerhafte FeldID!"); return; }
            if (Felder[Feld].Typ != 0) { SystemMessageF("Fehler 22: Das ist kein Straße!"); return; }
            if (EigenePosition != Feld) { SystemMessageF("Fehler 23: Sie befinden sich nicht auf dem Feld!"); return; }
            if (InJail) { SystemMessageF("Fehler 24: Es kann vom Gefängnis aus keine Straße gekauft werden!"); return; }
            Feld = Felder[Feld].Straße;
            if (Straßen[Feld].Besitzer > 0) { SystemMessageF("Fehler 25: Straße gehört einem anderen Spieler!"); return; }
            Money2 -= Straßen[Feld].Preis;
            Straßen[Feld].Besitzer = GetFarbe();
            SystemMessage("Spieler " + GetFarbe().ToString() + " kauft " + Straßen[Feld].Name + " (" + Straßen[Feld].Preis.ToString() + ")");
        }

        private void Test_zahlen(int Wuerfel, int Wuerfel1)
        {
            if (Straßen[Felder[EigenePosition].Straße].Besitzer > 0)
                if (Straßen[Felder[EigenePosition].Straße].Hypothek == false && Spieler[Straßen[Felder[EigenePosition].Straße].Besitzer - 1].InJail == false)
                { // Straße getroffen
                    int temp = Felder[EigenePosition].Straße;
                    if (Straßen[temp].Besitzer > 0 && Straßen[temp].Besitzer != GetFarbe())
                    {
                        Money2 -= GetMiete(EigenePosition, Wuerfel + Wuerfel1);
                        Spieler[Straßen[Felder[EigenePosition].Straße].Besitzer - 1].Money2 += GetMiete(EigenePosition, Wuerfel + Wuerfel1);
                    }
                }
        }

        private int Bewegen(int Wuerfel, int Wuerfel1)
        {
            if (EigenePosition == Gefängnis && Wuerfel != Wuerfel1 && InJail && JailCooldown == 0) return 1;
            if (EigenePosition == Gefängnis && Wuerfel != Wuerfel1 && InJail) { if (JailCooldown > 0)JailCooldown--; return 0; }
            if (EigenePosition == Gefängnis && Wuerfel == Wuerfel1 && InJail) return 1;
            int oldPos = EigenePosition;
            EigenePosition = (EigenePosition + Wuerfel + Wuerfel1) % 40;
            KI_Monopoly.Spielfeld[GetFarbe() - 1] = EigenePosition;
            if (EigenePosition < oldPos) Money2 += Felder[Start].Straße;
            if (EigenePosition == FreiParken)
            {
                Money2 += Felder[FreiParken].Straße;
                SystemMessage("Spieler " + GetFarbe().ToString() + " erhält frei-Parken (" + Felder[FreiParken].Straße.ToString() + ")");
                Felder[FreiParken].Straße = 0;
            }
            else
                if (Felder[EigenePosition].Typ == 7)
                {
                    Money2 -= Felder[EigenePosition].Straße;
                    SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt Steuern (" + Felder[EigenePosition].Straße + ")");
                }
                else
                    if (Felder[EigenePosition].Typ == 1)
                    {
                        SystemMessage("Spieler " + GetFarbe().ToString() + " zieht Ereigniskarte");
                        EreignisKarteZiehen(Wuerfel, Wuerfel1);
                    }
                    else
                        if (Felder[EigenePosition].Typ == 2)
                        {
                            SystemMessage("Spieler " + GetFarbe().ToString() + " zieht Gemeinschaftskarte");
                            GemeinschaftsKarteZiehen();
                        }
                        else
                            if (Felder[EigenePosition].Typ == 0)
                            {
                                if (Straßen[Felder[EigenePosition].Straße].Besitzer > 0)
                                    if (Straßen[Felder[EigenePosition].Straße].Hypothek == false && Spieler[Straßen[Felder[EigenePosition].Straße].Besitzer - 1].InJail == false)
                                    { // Straße getroffen
                                        int temp = Felder[EigenePosition].Straße;
                                        if (Straßen[temp].Besitzer > 0 && Straßen[temp].Besitzer != GetFarbe())
                                        {
                                            Money2 -= GetMiete(EigenePosition, Wuerfel + Wuerfel1);
                                            Spieler[Straßen[Felder[EigenePosition].Straße].Besitzer - 1].Money2 += GetMiete(EigenePosition, Wuerfel + Wuerfel1);
                                            SystemMessage("Spieler " + GetFarbe().ToString() + " zahlt an Spieler " + Straßen[Felder[EigenePosition].Straße].Besitzer.ToString() + " (" + GetMiete(EigenePosition, Wuerfel + Wuerfel1).ToString() + "," + Straßen[Felder[EigenePosition].Straße].Name + ")");
                                        }
                                    }
                            }
                            else
                                if (EigenePosition == GotoGefängnis)
                                {
                                    SystemMessage("Spieler " + GetFarbe().ToString() + " geht ins Gefängnis (Feld)");
                                    EigenePosition = Gefängnis;
                                    InJail = true;
                                    JailCooldown = 3;
                                    KI_Monopoly.Spielfeld[GetFarbe() - 1] = EigenePosition;
                                }

            if (Wuerfel == Wuerfel1) return 2;
            return 0;
        }

        private void Check_Alive()
        {
            if (Money2 < 0)
            {
                Aktiv = false;
                for (int i = 0; i < 28; i++)
                {
                    if (GetFarbe() == Straßen[i].Besitzer)
                    {
                        Straßen[i].Besitzer = 0;
                        Straßen[i].Haus = 0;
                        Straßen[i].Hypothek = false;
                    }
                }
            }
        }

        public virtual void Aufruf() { }

        public virtual void OnFailure() { }

        public void init()
        {
            for (int i = 0; i < 6; i++)
            {
                KI_Monopoly.Spielfeld[i] = 0;
            }
            InJail = false;
            EigenePosition = 0;
            Money2 = 1500;
            Aktiv = true;

            // KI_Monopoly.Spielfeld = new Secure<int>(44);
            Straßen[0] = new Straße("Badstraße", 60, 2, 10, 30, 90, 160, 250, 50, 50, 0, 1, -1, -1);
            Straßen[1] = new Straße("Turmstraße", 60, 4, 20, 60, 180, 320, 450, 50, 50, 0, 0, -1, -1);
            Straßen[2] = new Straße("Süd-bahnhof", 200, 25, 50, 100, 200, 0, 0, 0, 0, 1, 10, 17, 25);
            Straßen[3] = new Straße("Chaussestraße", 100, 6, 30, 90, 270, 400, 550, 50, 50, 0, 4, 5, -1);
            Straßen[4] = new Straße("Elisenstraße", 100, 6, 30, 90, 270, 400, 550, 50, 50, 0, 3, 5, -1);
            Straßen[5] = new Straße("Poststraße", 120, 8, 40, 100, 300, 450, 600, 50, 50, 0, 3, 4, -1);
            Straßen[6] = new Straße("Seestraße", 140, 10, 50, 150, 450, 625, 750, 100, 100, 0, 8, 9, -1);
            Straßen[7] = new Straße("Elektrizitätswerk", 150, 4, 10, 0, 0, 0, 0, 0, 0, 2, 20, -1, -1);
            Straßen[8] = new Straße("Hafenstraße", 140, 10, 50, 150, 450, 625, 750, 100, 100, 0, 6, 9, -1);
            Straßen[9] = new Straße("Neue Straße", 160, 12, 60, 180, 500, 700, 900, 100, 100, 0, 6, 8, -1);
            Straßen[10] = new Straße("Westbahnhof", 200, 25, 50, 100, 200, 0, 0, 0, 0, 1, 2, 17, 25);
            Straßen[11] = new Straße("Münchner Straße", 180, 14, 70, 200, 550, 700, 900, 100, 100, 0, 12, 13, -1);
            Straßen[12] = new Straße("Wiener Straße", 180, 14, 70, 200, 550, 700, 900, 100, 100, 0, 11, 13, -1);
            Straßen[13] = new Straße("Berliner Straße", 200, 16, 80, 220, 600, 800, 1000, 100, 100, 0, 11, 12, -1);
            Straßen[14] = new Straße("Theaterstraße", 220, 18, 90, 250, 700, 875, 1050, 150, 150, 0, 15, 16, -1);
            Straßen[15] = new Straße("Museumsstraße", 220, 18, 90, 250, 700, 875, 1050, 150, 150, 0, 14, 16, -1);
            Straßen[16] = new Straße("Opernplatz", 240, 20, 100, 300, 750, 925, 1100, 150, 150, 0, 14, 15, -1);
            Straßen[17] = new Straße("Nordbahnhof", 200, 25, 50, 100, 200, 0, 0, 0, 0, 1, 10, 2, 25);
            Straßen[18] = new Straße("Lessingstraße", 260, 22, 110, 330, 800, 975, 1150, 150, 150, 0, 18, 21, -1);
            Straßen[19] = new Straße("Schillerstraße", 260, 22, 110, 330, 800, 975, 1150, 150, 150, 0, 19, 21, -1);
            Straßen[20] = new Straße("Wasserwerk", 150, 4, 10, 0, 0, 0, 0, 0, 0, 2, 7, -1, -1);
            Straßen[21] = new Straße("Goethestraße", 280, 24, 120, 360, 850, 1025, 1200, 150, 150, 0, 18, 19, -1);
            Straßen[22] = new Straße("Rathausplatz", 300, 26, 130, 390, 900, 1100, 1275, 200, 200, 0, 23, 24, -1);
            Straßen[23] = new Straße("Hauptstraße", 300, 26, 130, 390, 900, 1100, 1275, 200, 200, 0, 22, 24, -1);
            Straßen[24] = new Straße("Bahnhofstraße", 320, 28, 150, 450, 1000, 1200, 1400, 200, 200, 0, 22, 23, -1);
            Straßen[25] = new Straße("Hauptbahnhof", 200, 25, 50, 100, 200, 0, 0, 0, 0, 1, 10, 17, 2);
            Straßen[26] = new Straße("Parkstraße", 350, 35, 175, 500, 1100, 1300, 1500, 200, 200, 0, 26, -1, -1);
            Straßen[27] = new Straße("Schlossallee", 400, 50, 200, 600, 1400, 1700, 2000, 200, 200, 0, 27, -1, -1);

            // // 0 == Straße, 1 == Ereignisfeld, 2 == Gemeinschaftsfeld, 3 == Start, 4 == Freies Parken, 5 == Gefängnis, 6 == Gehe ins Gefängnis, 7 == Steuer
            Felder[0] = new Feld(3, 200);
            Felder[1] = new Feld(0, 0);
            Felder[2] = new Feld(2, 0);
            Felder[3] = new Feld(0, 1);
            Felder[4] = new Feld(7, 200);
            Felder[5] = new Feld(0, 2);
            Felder[6] = new Feld(0, 3);
            Felder[7] = new Feld(1, 0);
            Felder[8] = new Feld(0, 4);
            Felder[9] = new Feld(0, 5);
            Felder[10] = new Feld(5, 0);
            Felder[11] = new Feld(0, 6);
            Felder[12] = new Feld(0, 7);
            Felder[13] = new Feld(0, 8);
            Felder[14] = new Feld(0, 9);
            Felder[15] = new Feld(0, 10);
            Felder[16] = new Feld(0, 11);
            Felder[17] = new Feld(2, 0);
            Felder[18] = new Feld(0, 12);
            Felder[19] = new Feld(0, 13);
            Felder[20] = new Feld(4, 0);
            Felder[21] = new Feld(0, 14);
            Felder[22] = new Feld(1, 0);
            Felder[23] = new Feld(0, 15);
            Felder[24] = new Feld(0, 16);
            Felder[25] = new Feld(0, 17);
            Felder[26] = new Feld(0, 18);
            Felder[27] = new Feld(0, 19);
            Felder[28] = new Feld(0, 20);
            Felder[29] = new Feld(0, 21);
            Felder[30] = new Feld(6, 0);
            Felder[31] = new Feld(0, 22);
            Felder[32] = new Feld(0, 23);
            Felder[33] = new Feld(2, 0);
            Felder[34] = new Feld(0, 24);
            Felder[35] = new Feld(0, 25);
            Felder[36] = new Feld(1, 0);
            Felder[37] = new Feld(0, 26);
            Felder[38] = new Feld(7, 75);
            Felder[39] = new Feld(0, 27);

            // Felder an straßen zuweisen
            for (int i = 0; i < Felder.Count(); i++)
            {
                if (Felder[i].Typ == 0)
                {
                    Straßen[Felder[i].Straße].Feld = i;
                }
            }
        }

        private int Pasch = 0;
        private int JailCooldown = 0;

        public int GetJailTime()
        {
            return JailCooldown;
        }

        public void Spielen(int Zahl)
        {
            if (!Aktiv) return;
            if (Sec != Sec2) { SystemMessage("Betrug 4"); return; }
            if (Pasch == 3)
            {
                EigenePosition = Gefängnis;
                SystemMessage("Spieler " + GetFarbe().ToString() + " geht ins Gefängnis (Paschregel)");
                InJail = true;
                JailCooldown = 3;
                Pasch = 0;
                return;
            }

            int Wurf = Wuerfel();
            int Wurf2 = Wuerfel();
            SystemMessage("Spieler " + GetFarbe().ToString() + " Würfelt: " + Wurf.ToString() + " und " + Wurf2.ToString());
            if (Wurf != Wurf2) Pasch = 0;
            int answer = Bewegen(Wurf, Wurf2);
            if (answer == 1)
            {
                InJail = false;
                JailCooldown = 0;
                Money2 -= 50;
                Spielen(Zahl);
            }
            else
            {
                Secured = 0;
                Aufruf();
                Secured = Zahl;
                Check_Alive();
                if (answer == 2) { Pasch++; Spielen(Zahl); }
            }
        }
    }
}