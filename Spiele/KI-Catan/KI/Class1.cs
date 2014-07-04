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

    public interface IKI_Catan
    {
        void Aufruf();
        void OnFailure();
        Punkt4 OnStraße();
        Punkt2 OnSiedlung();
        Punkt2 OnStadt();
        Punkt2 OnRäuberSetzen();
        Punkt5 OnRäuberAblegen(int ablegen);
    }

    public class Punkt2
    {
        public int x = -1;
        public int y = -1;
        public Punkt2(int a, int b)
        {
            x = a;
            y = b;
        }

        public bool AufKarte()
        {
            if (x < 0 || y < 0) return false;
            if (x >= 40 || y >= 40) return false;
            return true;
        }
    }

    public class Punkt5
    {
        public int[] data = { -1, -1, -1, -1, -1 };

        public Punkt5(int a, int b, int c, int d, int e)
        {
            data[0] = a;
            data[1] = b;
            data[2] = c;
            data[3] = d;
            data[4] = e;
        }

        public int Summe()
        {
            int summ = 0;
            for (int i = 0; i < data.Length; i++) summ += data[i];
            return summ;
        }
    }

    public class Punkt4
    {
        public int[] data = { -1, -1, -1, -1 };

        public Punkt4(int a, int b, int c, int d)
        {
            data[0] = a;
            data[1] = b;
            data[2] = c;
            data[3] = d;
        }

        public int Summe()
        {
            int summ = 0;
            for (int i = 0; i < data.Length; i++) summ += data[i];
            return summ;
        }

        public bool AufKarte()
        {
            for (int i = 0; i < data.Length; i++) if (data[i] < 0) return false;
            for (int i = 0; i < data.Length; i++) if (data[i] >= 40) return false;
            return true;
        }
    }

    public class Punkt3
    {
        public int[] data = { -1, -1, -1 };

        public bool IsMember(int FeldID)
        {
            if (data[0] == FeldID || data[1] == FeldID || data[2] == FeldID) return true;
            return false;
        }
    }

    public class Feld
    {
        int ID = -1;
        int Typ = 0; // 0==Wald, 1==Steinbruch, 2==Feld, 3==Weide, 4==Bergwerk, 5==Wüste
        int Zahl = 0;
        public Punkt2[] Kreuze = new Punkt2[6];
        public bool Räuber=false;

        public Feld(int Ident, int Sorte, int Zahlenchip)
        {
            ID = Ident;
            Typ = Sorte;
            Zahl = Zahlenchip;
        }

        public int GetID()
        {
            return ID;
        }
        public int GetTyp()
        {
            return Typ;
        }
        public int GetZahl()
        {
            return Zahl;
        }
    }

    public class Kreuz
    {
        public Punkt2[] Straßen = { new Punkt2(-1, -1), new Punkt2(-1, -1), new Punkt2(-1, -1) };
        public int[] StraßenBesitzer = { 0, 0, 0 };
        public Punkt2[] Nachbar = { new Punkt2(-1, -1), new Punkt2(-1, -1), new Punkt2(-1, -1) };
        public int[] Würfel = { -1, -1, -1 };
        public int[] Rohstoff = { -1, -1, -1 };
        public bool[] Räuber = { false,false,false };
        public bool Bebaut = false;
        public int BauTyp = 0; // 0 == Siedlung, 1 == Stadt
        public int Besitzer = 0; // 1 bis 6
        public int x = -1;
        public int y = -1;

        public Kreuz(int posx, int posy, Punkt2[] Straße, int[] StraßeBesitzer, Punkt2[] Nachbarn, int[] Würfels, bool Gebaut, int GebautTyp, int Owner, int[] Rohstoffe)
        {
            Straßen = Straße;
            StraßenBesitzer = StraßeBesitzer;
            Nachbar = Nachbarn;
            Würfel = Würfels;
            Bebaut = Gebaut;
            BauTyp = GebautTyp;
            Rohstoff = Rohstoffe;
            Besitzer = Owner;
            x = posx;
            y = posy;
        }

        public bool IsNachbar(int x, int y)
        {
            for (int i = 0; i < 3; i++) if (Nachbar[i].x == x && Nachbar[i].y == y) return true;
            return false;
        }

        public bool HatStraße(int x, int y)
        {
            for (int i = 0; i < 3; i++)if (Straßen[i].x == x && Straßen[i].y == y) return true;
            return false;
        }

        public bool AddStraße(int Besitzer, int x, int y)
        {
            for (int i = 0; i < 3; i++) 
                if (Straßen[i].x == -1 && Straßen[i].y == -1)
                {
                    Straßen[i].x = x;
                    Straßen[i].y = y;
                    StraßenBesitzer[i] = Besitzer;
                    break;
                }
            return false;
        }

        public Punkt5 GetErtrag(int Gewürfelt, int Spieler)
        {
            Punkt5 res = new Punkt5(0, 0, 0, 0, 0);
            if (!Bebaut) return res;
            if (Besitzer != Spieler) return res;

            for (int i = 0; i < 3; i++)
            {
                if (Würfel[i] == Gewürfelt)
                {
                    if (Rohstoff[i] == -1 || Rohstoff[i] >= 5 || BauTyp > 2) continue;
                    res.data[Rohstoff[i]] += BauTyp;
                }
            }
            return res;
        }
    }

    abstract public class KI_Catan : IKI_Catan
    {
        public const int CHolz = 0;
        public const int CLehm = 1;
        public const int CGetreide = 2;
        public const int CSchafe = 3;
        public const int CEisen = 4;

        public static Secure<Feld> Felder = new Secure<Feld>(30);
        public static Secure2<Kreuz> Spielfeld = new Secure2<Kreuz>(40, 40); // speichert die übergänge zu den anderen Punkten
        public Feld Räuber=null;
        private int[] Zahlenchips = { 2, 5, 4, 6, 3, 9, 8, 11, 11, 10, 6, 3, 8, 4, 8, 10, 11, 12, 10, 5, 4, 9, 5, 9, 12, 3, 2, 6 }; // 28
        private int[] DefAnzahlFelder = { 6, 5, 6, 6, 5, 2 }; // 0==Wald, 1==Steinbruch, 2==Feld, 3==Weide, 4==Bergwerk, 5==Wüste

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

        private static int Sec = -1;
        private static int Sec2 = -1;

        public Secure<int> SpieleGewonnen = new Secure<int>(6);
        public Secure<int> Fehler = new Secure<int>(6);
        private KI_Catan[] Spieler = new KI_Catan[6]; // zuweisen nicht vergessen

        public Secure<List<Punkt2>> Produktion = new Secure<List<Punkt2>>(2); // 0 == Siedlungen, 1 == Stadt

        private bool[] is_set = { false, false, false, false, false, false };
        public void SetSpieler(KI_Catan Player, int id)
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

        private int Sondersiegpunkte2 = 0;
        public int Sondersiegpunkte
        {
            set { if (Sec == Sec2) Sondersiegpunkte2 = value; }
            get { return Sondersiegpunkte2; }
        }

        private int Rittermacht2 = 0;
        public int Rittermacht
        {
            set { if (Sec == Sec2) Rittermacht2 = value; }
            get { return Rittermacht2; }
        }

        private int Ritterkarten2 = 0;
        public int Ritterkarten
        {
            set { if (Sec == Sec2) Ritterkarten2 = value; }
            get { return Ritterkarten2; }
        }

        private int Straßenbaukarten2 = 0;
        public int Straßenbaukarten
        {
            set { if (Sec == Sec2) Straßenbaukarten2 = value; }
            get { return Straßenbaukarten2; }
        }

        private int Monopolkarten2 = 0;
        public int Monopolkarten
        {
            set { if (Sec == Sec2) Monopolkarten2 = value; }
            get { return Monopolkarten2; }
        }

        private int Erfindungskarten2 = 0;
        public int Erfindungskarten
        {
            set { if (Sec == Sec2) Erfindungskarten2 = value; }
            get { return Erfindungskarten2; }
        }

        private bool GrößteRittermacht2 = false;
        public bool GrößteRittermacht
        {
            set { if (Sec == Sec2) GrößteRittermacht2 = value; }
            get { return GrößteRittermacht2; }
        }

        private bool GrößteHandelsstraße2 = false;
        public bool GrößteHandelsstraße
        {
            set { if (Sec == Sec2) GrößteHandelsstraße2 = value; }
            get { return GrößteHandelsstraße2; }
        }

        private int Holz2 = 0;
        public int Holz
        {
            set { if (Sec == Sec2) Holz2 = value; }
            get { return Holz2; }
        }

        private int Lehm2 = 0;
        public int Lehm
        {
            set { if (Sec == Sec2) Lehm2 = value; }
            get { return Lehm2; }
        }

        private int Getreide2 = 0;
        public int Getreide
        {
            set { if (Sec == Sec2) Getreide2 = value; }
            get { return Getreide2; }
        }

        private int Schafe2 = 0;
        public int Schafe
        {
            set { if (Sec == Sec2) Schafe2 = value; }
            get { return Schafe2; }
        }

        private int Eisen2 = 0;
        public int Eisen
        {
            set { if (Sec == Sec2) Eisen2 = value; }
            get { return Eisen2; }
        }

        protected bool FehlerAktiv = true;
        public static DatenPaket Daten = new DatenPaket();
        public String Name = "Unbenannt";
        private bool sec_set = false;

        public void Kaufe_Straße()
        {
            if (Holz2 >= 1 && Lehm2 >= 1)
            {
                Holz2--; Lehm2--;
                PrüfeStraßeSetzen();
                PrüfeLängsteHandelsstraße();
            }
            else
                SystemMessageF("Nicht genügend Rohstoffe!");
        }

        public void Kaufe_Siedlung()
        {
            if (Holz2 >= 1 && Lehm2 >= 1 && Getreide2 >= 1 && Schafe2 >= 1)
            {
                Holz2--; Lehm2--; Getreide2--; Schafe2--;
                PrüfeSiedlungSetzen();
            }
            else
                SystemMessageF("Nicht genügend Rohstoffe!");
        }

        public void Kaufe_Stadt()
        {
            if (Getreide2 >= 2 && Eisen2 >= 3)
            {
                Getreide2 -= 2; Eisen2 -= 3;
                PrüfeStadtSetzen();
            }
            else
                SystemMessageF("Nicht genügend Rohstoffe!");
        }

        private void PrüfeRäuberAblegenAlle()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Spieler[i] == null) continue;
                int summ = Spieler[i].GetAlleRohstoffeAnzahl();
                int ablegen = summ <= 7 ? 0 : summ / 2;
                if (ablegen > 0)
                {
                    Punkt5 antwort = Spieler[i].OnRäuberAblegen(ablegen);
                    if (antwort.Summe() < ablegen) SystemMessageF("Nicht genug Rohstoffkarten abgelegt!");
                    if (antwort.Summe() > ablegen) SystemMessageF("Zuviele Rohstoffkarten abgelegt!");
                    for (int b = 0; b < 5; b++) Spieler[i].ErhöheRohstoff(b, -antwort.data[b]);
                }
            }
        }

        public int GetAlleRohstoffeAnzahl()
        {
            return GetAnzahlRohstoff(0) + GetAnzahlRohstoff(1) + GetAnzahlRohstoff(2) + GetAnzahlRohstoff(3) + GetAnzahlRohstoff(4);
        }

        private void PrüfeStraßeSetzen()
        {
            Punkt4 antwort = OnStraße();
            if (!antwort.AufKarte()) { SystemMessageF("Fehlerhafte Koordinaten!"); return; }
            if (Spielfeld[antwort.data[0], antwort.data[1]] == null) { SystemMessageF("Keine Kreuzung!"); return; }
            if (Spielfeld[antwort.data[2], antwort.data[3]] == null) { SystemMessageF("Keine Kreuzung!"); return; }
            if (!AnEigenerStraße(antwort.data[0], antwort.data[1], GetFarbe()) && !AnEigenerStraße(antwort.data[2], antwort.data[3], GetFarbe())) { SystemMessageF("Keine Straßenanbindung!"); return; }
            if (!Spielfeld[antwort.data[0], antwort.data[1]].IsNachbar(antwort.data[2],antwort.data[2])){ SystemMessageF("Punkte sind keine Nachbarn!"); return; }
            if (Spielfeld[antwort.data[0], antwort.data[1]].HatStraße(antwort.data[2],antwort.data[3])) { SystemMessageF("Hier steht bereits etwas!"); return; }
                Spielfeld[antwort.data[0], antwort.data[1]].AddStraße(GetFarbe(),antwort.data[2],antwort.data[3]);
                Spielfeld[antwort.data[2], antwort.data[3]].AddStraße(GetFarbe(), antwort.data[0], antwort.data[1]);
        }

        private void PrüfeSiedlungSetzen()
        {
            Punkt2 antwort = OnSiedlung();
            if (!antwort.AufKarte()) { SystemMessageF("Fehlerhafte Koordinaten!"); return; }
            if (Spielfeld[antwort.x, antwort.y] == null) { SystemMessageF("Keine Kreuzung!"); return; }
            if (!AnEigenerStraße(antwort.x, antwort.y, GetFarbe())) { SystemMessageF("Keine Straßenanbindung!"); return; }
            if (Spielfeld[antwort.x, antwort.y].Bebaut) { SystemMessageF("Hier steht bereits etwas!"); return; }
            if (AbstandsregelEingehalten(antwort.x, antwort.y))
            {
                Spielfeld[antwort.x, antwort.y].Besitzer = GetFarbe();
                Spielfeld[antwort.x, antwort.y].Bebaut = true;
                Spielfeld[antwort.x, antwort.y].BauTyp = 1;
            }
            else
            { SystemMessageF("Abstandsregel nicht eingehalten!"); return; }
        }

        private void PrüfeStadtSetzen()
        {
            Punkt2 antwort = OnStadt();
            if (!antwort.AufKarte()) { SystemMessageF("Fehlerhafte Koordinaten!"); return; }
            if (Spielfeld[antwort.x, antwort.y] == null) { SystemMessageF("Keine Kreuzung!"); return; }
            if (!AnEigenerStraße(antwort.x, antwort.y, GetFarbe())) { SystemMessageF("Keine Straßenanbindung!"); return; }
            if (Spielfeld[antwort.x, antwort.y].Bebaut) { SystemMessageF("Hier steht bereits etwas!"); return; }
            if (AbstandsregelEingehalten(antwort.x, antwort.y))
            {
                Spielfeld[antwort.x, antwort.y].Besitzer = GetFarbe();
                Spielfeld[antwort.x, antwort.y].Bebaut = true;
                Spielfeld[antwort.x, antwort.y].BauTyp = 2;
            }
            else
            { SystemMessageF("Abstandsregel nicht eingehalten!"); return; }
        }

        public bool AnEigenerStraße(int x, int y, int Besitzer)
        {
            if (Besitzer <= 0 || Besitzer >= 6) return false;
            if (x < 0 || y < 0 || x >= 40 || y >= 40) return false;
            for (int i = 0; i < 3; i++) if (Spielfeld[x, y].StraßenBesitzer[i] == Besitzer) return true;
            return false;
        }

        public bool AbstandsregelEingehalten(int x, int y)
        {
            if (x < 0 || y < 0 || x >= 40 || y >= 40) return true;
            if (Spielfeld[x, y] == null) return true; // Kreuz existiert nicht
            bool check = true;
            for (int i = 0; i < 3; i++)
            {
                if (!Spielfeld[x, y].Nachbar[i].AufKarte()) continue;
                if (Spielfeld[Spielfeld[x, y].Nachbar[i].x, Spielfeld[x, y].Nachbar[i].y].Bebaut) { check = false; break; }
            }

            return check;
        }

        private void PrüfeRäuberSetzen()
        {
            // noch erstellen
            Punkt2 antwort = OnRäuberSetzen();
            if (Räuber != null)
            {
                
            }

        }

        private void Ziehe_Entwicklungskarte()
        {
            // noch erstellen
            int[] Karten = { 20, 25, 28, 31, 34 }; // Ritter, Siegpunkt, Strassenbau, Monopol, Erfindung
            int zug = KI_Catan.rand.Next(0, 34);
            for (int i = 0; i < 5; i++)
            {
                if (zug < Karten[i])
                {
                    switch (i)
                    {
                        case 0:
                            Ritterkarten2++;
                            break;
                        case 1:
                            Sondersiegpunkte2++;
                            break;
                        case 2:
                            Straßenbaukarten2++;
                            break;
                        case 3:
                            Monopolkarten2++;
                            break;
                        case 4:
                            Erfindungskarten2++;
                            break;
                    }
                    return;
                }
            }
        }

        public void Kaufe_Entwicklung()
        {
            if (Eisen2 >= 1 && Getreide2 >= 1 && Schafe2 >= 1)
            {
                Eisen2--; Getreide2--; Schafe2--;
                Ziehe_Entwicklungskarte();
            }
            else
                SystemMessageF("Nicht genügend Rohstoffe!");
        }

        public void Spiele_Ritter()
        {
            if (Ritterkarten2 > 0)
            {
                Rittermacht2++;
                Ritterkarten2--;
                PrüfeRäuberSetzen();
            }
            else
                SystemMessageF("Keine Ritterkarte vorhanden!");
        }

        public int GetLängsteStraße(int SpielerFarbe)
        {
            // noch erstellen
            return 0;
        }

        private void SpielerRohstoffeZuteilen(int Wuerfel, int Wuerfel2)
        {
            if (Wuerfel + Wuerfel2 == 7) return;
            // noch erstellen
            for (int i = 0; i < 30; i++)
            {
                if (Felder[i].GetTyp() == 5) continue;
                if (Felder[i].GetZahl() == Wuerfel + Wuerfel2)
                {
                    /*for (int b = 0; b < 6; b++)
                    {
                        if (
                    }
                    Spieler[player].ErhöheRohstoff(Felder[i].Typ, anz);*/
                }
            }
        }

        private void PrüfeLängsteHandelsstraße()
        {
            int user = -1;
            int longest = 0;
            bool doppel = false;
            for (int i = 0; i < 6; i++)
            {
                if (Spieler[i] == null) continue;
                if (Spieler[i].GrößteHandelsstraße2 == true) Spieler[i].GrößteHandelsstraße2 = false;
                if (user == -1)
                {
                    user = i;
                    longest = GetLängsteStraße(i + 1);
                    doppel = false;
                }
                else
                    if (longest < GetLängsteStraße(i + 1))
                    {
                        user = i;
                        longest = GetLängsteStraße(i + 1);
                        doppel = false;
                    }
                    else
                        if (longest == GetLängsteStraße(i + 1))
                        {
                            doppel = true;
                        }
            }
            if (!doppel)
            {
                Spieler[user].GrößteHandelsstraße2 = true;
            }
        }

        public void Spiele_Strassenbau()
        {
            if (Straßenbaukarten2 > 0)
            {
                Straßenbaukarten2--;
                PrüfeStraßeSetzen();
                PrüfeLängsteHandelsstraße();
            }
            else
                SystemMessageF("Keine Straßenbaukarte vorhanden!");
        }

        public void Spiele_Monopol(int Rohstoff)
        {
            if (Monopolkarten2 > 0)
            {
                Monopolkarten2--;
                for (int i = 0; i < 6; i++)
                {
                    if (Spieler[i] == null || i + 1 == GetFarbe()) continue;
                    ErhöheRohstoff(Rohstoff, Spieler[i].GetAnzahlRohstoff(Rohstoff));
                    Spieler[i].ErhöheRohstoff(Rohstoff, -Spieler[i].GetAnzahlRohstoff(Rohstoff));
                }
            }
            else
                SystemMessageF("Keine Monopolkarte vorhanden!");
        }

        private void ErhöheRohstoff(int Rohstoff, int anz)
        {
            switch (Rohstoff)
            {
                case CHolz:
                    Holz2 += anz;
                    if (Holz2 < 0) { Holz2 = 0; SystemMessageF("Negativer Holzvorrat!"); }
                    break;
                case CLehm:
                    Lehm2 += anz;
                    if (Lehm2 < 0) { Lehm2 = 0; SystemMessageF("Negativer Lehmvorrat!"); }
                    break;
                case CSchafe:
                    Schafe2 += anz;
                    if (Schafe2 < 0) { Schafe2 = 0; SystemMessageF("Negativer Schafvorrat!"); }
                    break;
                case CGetreide:
                    Getreide2 += anz;
                    if (Getreide2 < 0) { Getreide2 = 0; SystemMessageF("Negativer  Getreidevorrat!"); }
                    break;
                case CEisen:
                    Eisen2 += anz;
                    if (Eisen2 < 0) { Eisen2 = 0; SystemMessageF("Negativer Eisenvorrat!"); }
                    break;
                default:
                    SystemMessageF("Rohstoffart exisitert nicht!");
                    break;
            }
        }

        public int GetAnzahlRohstoff(int Rohstoff)
        {
            switch (Rohstoff)
            {
                case CHolz:
                    return Holz2;
                case CLehm:
                    return Lehm2;
                case CSchafe:
                    return Schafe2;
                case CGetreide:
                    return Getreide2;
                case CEisen:
                    return Eisen2;
                default:
                    SystemMessageF("Rohstoffart exisitert nicht!");
                    break;
            }
            return 0;
        }

        public void Spiele_Erfindung(int RohstoffA, int RohstoffB)
        {
            if (Erfindungskarten2 > 0)
            {
                Erfindungskarten2--;
                ErhöheRohstoff(RohstoffA, 1);
                ErhöheRohstoff(RohstoffB, 1);
            }
            else
                SystemMessageF("Keine Erfindungskarte vorhanden!");
        }

        #region General
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
            return KI_Catan.rand.Next(1, 7);
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
                Spieler[i].Fehler[GetFarbe() - 1]++;
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
        public virtual void Aufruf() { }
        public virtual void OnFailure() { }
        public virtual Punkt4 OnStraße() { return new Punkt4(-1, -1, -1, -1); }
        public virtual Punkt2 OnSiedlung() { return new Punkt2(-1, -1); }
        public virtual Punkt2 OnStadt() { return new Punkt2(-1, -1); }
        public virtual Punkt2 OnRäuberSetzen() { return new Punkt2(0, 0); }
        public virtual Punkt5 OnRäuberAblegen(int ablegen) { return new Punkt5(0, 0, 0, 0, 0); }
        #endregion

        public void init()
        {
            // Felder[0] = new Feld(0, 0);

        }

        public void Spielen(int Zahl)
        {
            if (Sec != Sec2) { SystemMessage("Betrug 4"); return; }
            int Wurf = Wuerfel();
            int Wurf2 = Wuerfel();
            SystemMessage("Spieler " + GetFarbe().ToString() + " Würfelt: " + Wurf.ToString() + " und " + Wurf2.ToString());
            Spielen(Zahl);
        }

    }
}