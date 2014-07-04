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
    public int[] Spielfeld = new int[44];
    public int[] EigenePosition = new int[4];
    public static int[] Freie = new int[4];
    public static int[,] Saved = new int[4, 4];
    private int Farbe = -1;
    protected bool FehlerAktiv = true;
    public DatenPaket Daten = new DatenPaket();

    public bool GetFehlerAktiv()
    {
        return FehlerAktiv;
    }

    public int GetFarbe()
    {
        return Farbe;
    }

    public void SystemMessage(String Text)
    {
     
       /* Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
        ProtClear();
        frm1.richTextBox1.AppendText(Text + "\n");
        frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
        frm1.richTextBox1.SelectionColor = Farb[GetFarbe() - 1];

        if (frm1.checkBox3.Checked)
        {
            frm1.richTextBox2.AppendText(Text + "\n");
            frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
            frm1.richTextBox2.SelectionColor = Farb[GetFarbe() - 1];
        }
        Protokol(Text);*/
        Daten.SystemMessage.Add(Text);
    }

    public void SystemMessageF(String Text)
    {
     /*   if (frm1.checkBox1.Checked)
        {
            ProtClear();
            frm1.richTextBox1.AppendText(Text + "\n");
            frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
            frm1.richTextBox1.SelectionColor = Color.Violet;
        }

        if (frm1.checkBox3.Checked)
        {
            frm1.richTextBox2.AppendText(Text + "\n");
            frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
            frm1.richTextBox2.SelectionColor = Color.Violet;
        }
        frm1.Protokoll_anzahl++;
        Protokol(Text);*/
        Daten.SystemMessageF.Add(Text);
        OnFailure();
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
        if (GetEigenePosition(ID) < 0) return false;
        if (GetEigenePosition(ID) + Wurf >= 44) return false;
        if (Wurf == 6 && GetEigeneFrei() > 0 && Spielfeld[0] <= 0) return true;
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
        for (int i = 0; i < 44; i++)
        {
            Spielfeld[i] = 0;
        }

        for (int i = 0; i < 4; i++) EigenePosition[i] = -1;
    }

}
}