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

public interface IKI
{
    int Aufruf(int Wuerfel);
    void OnFailure();
}



abstract public class KI
{
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
                return arr[i,a];
            }
            set
            {
                if (Sec == Sec2) { arr[i,a] = value; } else SystemMessage("Betrug 3");
            }
        }
    }

    private static int Sec = -1;
    private static int Sec2 = -1;
    public Secure<int> Spielfeld = new Secure<int>(44);
    public Secure<int> EigenePosition = new Secure<int>(4);
    public static Secure<int> Freie = new Secure<int>(4);
    public static Secure2<int> Saved = new Secure2<int>(4, 4);


    public Secure<int> FigurenVerloren = new Secure<int>(4);
    public Secure<int> FigurenBesiegt = new Secure<int>(4);
    public Secure<int> SpieleGewonnen = new Secure<int>(4);
    public Secure<int> Fehler = new Secure<int>(4);
    public Secure<int> InSave = new Secure<int>(4);

    private int FarbeS = -1;
    private int Farbe
    {
        set { if (Sec == Sec2) FarbeS = value; }
        get {return FarbeS;}
    }


    protected bool FehlerAktiv = true;
    public static DatenPaket Daten = new DatenPaket();
    public String Name="Unbenannt";
    private bool sec_set=false;

    public int Secured
         {
             set { if (sec_set == false) { Sec = value; sec_set = true; } else { Sec2 = value; } }

             get { SystemMessage("Betrug 1"); return -1; }
        }

    public bool GetFehlerAktiv()
    {
        return FehlerAktiv;
    }

    public int GetFarbe()
    {
        return Farbe;
    }

    public static void SystemMessage(String Text)
    {
        Daten.SystemMessage.Add(Text);
    }

    public void SystemMessageF(String Text)
    {
        Daten.SystemMessageF.Add(Text);
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

    public bool GetOnField(int ID)
    {
        return (EigenePosition[ID] >= 0) ? true : false;
    }

    public void EntferneFigur(int ID)
    {
        if (EigenePosition[ID] > -1) Spielfeld[EigenePosition[ID]] = 0;
        EigenePosition[ID] = -1;
    }

    public int GetEigenePosition(int ID)
    {
        return EigenePosition[ID];
    }

    public bool BewegungMoeglich(int ID, int Wurf)
    {
        if (GetEigenePosition(ID) + Wurf >= 44) return false;
        if (Wurf == 6 && GetEigeneFrei() > 0 && Spielfeld[0] !=GetFarbe() && GetEigenePosition(ID)<=-1) return true;
        if (GetEigenePosition(ID) < 0) return false;
        if (GetEigeneFrei() > 0 && Spielfeld[0] == GetFarbe() && GetEigenePosition(ID) > 0) return false;

        if (Spielfeld[GetEigenePosition(ID) + Wurf] == GetFarbe()) return false;

        if (GetEigenePosition(ID) + Wurf >= 40)
        {
            int start = GetEigenePosition(ID) + 1;
            if (start < 40) start = 40;
            for (int i = start; i <= GetEigenePosition(ID) + Wurf && i < 44; i++)
            {
                if (Spielfeld[i] > 0)
                {
                    return false;
                }
            }
            return true;
        }


        if (GetEigenePosition(ID) + Wurf <= 39)
        {
            int a = GetEigenePosition(ID) + Wurf;
            int b = Spielfeld[a];
            if (b <= 0) return true;
            if (b == GetFarbe()) return false;
            if (b != GetFarbe() && (a == 4 || a == 14 || a == 24 || a == 34)) return false;
            return true;
        }

        return false;
    }

    public bool BewegungEinerMoeglich(int Wurf)
    {
        for (int i = 0; i < 4; i++) if (BewegungMoeglich(i, Wurf)) return true;
        return false;
    }

    public int GetEigeneFrei()
    {
        int anz = 0;
        for (int i = 0; i < 4; i++)
        {
            if (EigenePosition[i] <= -1) anz++;
        }
        return anz;

    }

    public int GetEigeneSave()
    {
        int anz = 0;
        for (int i = 0; i < 4; i++)
        {
            if (EigenePosition[i] >= 40) anz++;
        }
        return anz;

    }

    public int GetEigeneFigurenAnzahl()
    {
        int anz = 0;
        for (int i = 0; i < 4; i++)
        {
            if (GetOnField(i))
            {
                anz++;
            }
        }
        return anz;
    }

    public abstract int Aufruf(int Wuerfel);
    public abstract void OnFailure();

    public void init()
    {
        Spielfeld = new Secure<int>(44);
        EigenePosition = new Secure<int>(4);
        Freie = new Secure<int>(4);
        Saved = new Secure2<int>(4, 4);
        for (int i = 0; i < 44; i++)
        {
            Spielfeld[i] = 0;
        }

        for (int i = 0; i < 4; i++) EigenePosition[i] = -1;

    }

}
}