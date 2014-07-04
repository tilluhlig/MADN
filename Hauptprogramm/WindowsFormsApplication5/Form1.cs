using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace WindowsFormsApplication5
{

    public partial class Form1 : Form
    {
        public int Protokoll_anzahl = 0;
        public int Spielzug = 0;
        Task Looping;
        public int Spiele = 1;

        public int[] lastmovefrom = { -1, -1, -1, -1, -1 }; // 0 = start, 1 = im feld, 2 = in save
        public int[] lastmoveto = { -1, -1, -1, -1, -1 }; // 0 = start, 1 = im feld, 2 = in save
        public int[] lastmovetoID = { 0, 0, 0, 0, 0 };
        public int[] lastmovefromID = { 0, 0, 0, 0, 0 };
        public int[] lastplayer = { 0, 0, 0, 0, 0 };

        // Statistik
        public int[] InSave = new int[6];
        public int[] FigurenVerloren = new int[6];
        public int[] FigurenBesiegt = new int[6];
        public int[] SpieleGewonnen = new int[6];
        public int[] Fehler = new int[6];
        public int Spiel = 0;

        public void setLastMove(int lp, int lmf, int lmfI, int lmt, int lmtI)
        {
            for (int i = 0; i < lastplayer.Length - 1; i++)
            {
                lastplayer[lastplayer.Length - 1 - i] = lastplayer[lastplayer.Length - 2 - i];
                lastmovefrom[lastplayer.Length - 1 - i] = lastmovefrom[lastplayer.Length - 2 - i];
                lastmovefromID[lastplayer.Length - 1 - i] = lastmovefromID[lastplayer.Length - 2 - i];
                lastmoveto[lastplayer.Length - 1 - i] = lastmoveto[lastplayer.Length - 2 - i];
                lastmovetoID[lastplayer.Length - 1 - i] = lastmovetoID[lastplayer.Length - 2 - i];
            }
            lastplayer[0] = lp;
            lastmovefrom[0] = lmf;
            lastmovefromID[0] = lmfI;
            lastmoveto[0] = lmt;
            lastmovetoID[0] = lmtI;
        }
        Spiel_MADN Spielfeld;
        Spiel_Monopoly SpielfeldM = new Spiel_Monopoly();
        public static TextBox[] eingabe = new TextBox[4];
        public Form1()
        {
            eingabe[0] = textBox4;
            eingabe[1] = textBox3;
            eingabe[2] = textBox2;
            eingabe[3] = textBox1;
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            ReloadSpielerKI();
            maskedTextBox1.Text = "1";
            int[] liste = { comboBox2.SelectedIndex, comboBox3.SelectedIndex, comboBox4.SelectedIndex, comboBox5.SelectedIndex };
           Spielfeld = new Spiel_MADN(liste);
           reset();
        }

        public void reset()
        {
            for (int i = 0; i < 6; i++)
            {
                InSave[i] = 0;
                FigurenVerloren[i] = 0;
                FigurenBesiegt[i] = 0;
                SpieleGewonnen[i] = 0;
                Fehler[i] = 0;
            }
            for (int i = 0; i < lastplayer.Length; i++)
            {
                lastmovefrom[i] = -1;
                lastmoveto[i] = -1;
                lastmovetoID[i] = 0;
                lastmovefromID[i] = 0;
                lastplayer[i] = 0;
            }
            Protokoll_anzahl = 0; 
            Spielzug = 0;
            richTextBox1.Clear();
            richTextBox2.Clear();
            Spielfeld.frm1 = this;
            SpielfeldM.frm1 = this;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (checkBox6.Checked) return;
            if (Spiel == 0)
            {

                    Spielfeld.Next();
                
            }

            if (Spiel == 1) SpielfeldM.Next();

            if (Spiele == 0)
            {
                button2_Click(null, null);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Spiele = Convert.ToInt32(maskedTextBox1.Text);
            if (button1.Text == "Pause")
            {
                button1.Text = "Spiel Starten";
                timer1.Enabled = false;
            }
            else
            {
                button1.Text = "Pause";
                if (checkBox6.Checked == true) timer1.Interval = 1000;
                if (Spiel == 0)  Spielfeld.AktualisiereSpielfeld(); 
                if (Spiel == 1) SpielfeldM.AktualisiereSpielfeld();
                timer1.Enabled = true;
                timer3.Enabled = true;
                if (checkBox6.Checked == true)
                {
                    Action<object> action2 = (object obj) =>
              {
                  if (Spiel == 0) Spielfeld.Alpha = new RichTextBox();
                  if (Spiel == 1) SpielfeldM.Alpha = new RichTextBox();
                  for (; ; )
                  {
                      if (timer1.Enabled == false) break;
                      if (Spiel == 0) Spielfeld.Next();
                      if (Spiel == 1) SpielfeldM.Next();
                  }
                  timer2.Enabled = true;
                  if (Spiel == 0) Spielfeld.Alpha.SaveFile(Application.StartupPath + "\\Log.rtf");
                  if (Spiel == 1) SpielfeldM.Alpha.SaveFile(Application.StartupPath + "\\Log.rtf");
              };
                    Looping = new Task(action2, "loop");
                    Looping.Start();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox2.SaveFile(Application.StartupPath + "\\Log.rtf");
            }
            catch (System.IO.IOException)
            {
                if (Spiel == 0) Spielfeld.SystemMessage2("Speichern war nicht möglich", false);
                if (Spiel == 1) SpielfeldM.SystemMessage2("Speichern war nicht möglich", false);
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                timer1.Interval = 1;
            }
            else
            {
                timer1.Interval = 1000;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            // for (int i=0;i<Spielfeld.Alpha.Lines.Count();i++)richTextBox2.
            timer3.Enabled = false;
            label3.Text = "Spielzug: " + Convert.ToString(Spielzug);
            maskedTextBox1.Text = Spiele.ToString();
            label1.Text = "Fehler: " + Convert.ToString(Protokoll_anzahl);
            //richTextBox2 = Spielfeld.Alpha;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Pause")
            {
                label3.Text = "Spielzug: " + Convert.ToString(Spielzug);
                maskedTextBox1.Text = Spiele.ToString();
                label1.Text = "Fehler: " + Convert.ToString(Protokoll_anzahl);
            }
        }

        public void ReloadSpielerKI()
        {
            ComboBox[] liste = { comboBox2, comboBox3, comboBox4, comboBox5 };
            if (comboBox1.SelectedIndex == 0)
            {
                KI[] SpielerSorten = { new Spieler1(), new Spieler2(), new Spieler3(), new Spieler4() };
                for (int i = 0; i < liste.Count(); i++)
                {
                    liste[i].Items.Clear();
                    for (int b = 0; b < SpielerSorten.Count(); b++)
                    {
                        liste[i].Items.Add(SpielerSorten[b].Name);
                    }
                    liste[i].Items.Add("Mensch");
                    liste[i].SelectedIndex = i >= liste[i].Items.Count ? liste[i].Items.Count-1 : i;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (button1.Text != "Spiel Starten")
            {
                this.button1_Click(null, null);
            }

            if (comboBox1.SelectedIndex == 0)
            {// MADN
                pictureBox1.BringToFront();
                Spiel = 0;
                int[] liste = { comboBox2.SelectedIndex, comboBox3.SelectedIndex, comboBox4.SelectedIndex, comboBox5.SelectedIndex };
                Spielfeld = new Spiel_MADN(liste);
                reset();
                Bitmap temp = new Bitmap(pictureBox2.Image);
                Graphics g = Graphics.FromImage(temp);
                pictureBox1.Image = temp;
                g.Dispose();

                panel1.Show();
            }
            else
                if (comboBox1.SelectedIndex == 1)
                {// Monopoly
                    pictureBox1.BringToFront();
                    Spiel = 1;
                    SpielfeldM = new Spiel_Monopoly();
                    reset();
                    Bitmap temp = new Bitmap(pictureBox3.Image);
                    Graphics g = Graphics.FromImage(temp);
                    pictureBox1.Image = temp;
                    g.Dispose();

                    panel1.Hide();
                }
            ReloadSpielerKI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                pictureBox1.BringToFront();
                Spiel = 0;
                int[] liste = { comboBox2.SelectedIndex, comboBox3.SelectedIndex, comboBox4.SelectedIndex, comboBox5.SelectedIndex };
                Spielfeld = new Spiel_MADN(liste);
                reset();
                Bitmap temp = new Bitmap(pictureBox2.Image);
                Graphics g = Graphics.FromImage(temp);
                pictureBox1.Image = temp;
                g.Dispose();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox[] liste = { comboBox2, comboBox3, comboBox4, comboBox5 };
            eingabe[0] = textBox4;
            eingabe[1] = textBox3;
            eingabe[2] = textBox2;
            eingabe[3] = textBox1;

            if (comboBox1.SelectedIndex == 0)
            {
                // MADN
                bool found = false;
                for (int i = 0; i < liste.Count(); i++)
                {
                    if (liste[i].SelectedIndex == 4)
                    {
                        found = true;
                        eingabe[i].Show();
                        eingabe[i].Text = Spielfeld.Spieler[i].Name;
                        // break;
                    }
                    else
                        eingabe[i].Hide();
                }

                if (found)
                {
                    timer4.Enabled = true;
                    checkBox2.Hide();
                    checkBox6.Hide();
                }
                else
                {
                    timer4.Enabled = false;
                    checkBox2.Show();
                    checkBox6.Show();
                }
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (Spielfeld.Mensch)
            {

                if (!Spielfeld.Spieler[Spielfeld.GetAktuellerSpieler()].BewegungEinerMoeglich(Spielfeld.MWurf))
                {
                    if (Spielfeld.MWurf != 6)
                    {
                        Spielfeld.Mensch = false;
                        timer4.Enabled = false;
                        timer1.Enabled = true;
                        timer1_Tick(null, null);
                        pictureBox4.Hide();
                        return;
                    }
                    else
                    {
                        Spielfeld.MWurf = Spielfeld.Wuerfeln();
                        Spielfeld.SystemMessage(Spielfeld.Name(Spielfeld.GetAktuellerSpieler()) + " Würfelt eine " + Convert.ToString(Spielfeld.MWurf), true);
                        Spielfeld.AktualisiereSpielfeld();
                    }
                }

                Spielfeld.AktualisiereSpielfeld();
            }
            timer4.Enabled = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            // Maus auswerten
            if (comboBox1.SelectedIndex == 0)
            {
                float scale = (float) pictureBox2.Image.Height / pictureBox2.Height;
                int X = (int) (e.X*scale);
                int Y = (int) (e.Y*scale);

                // MADN
                int AktuellerSpieler = Spielfeld.GetAktuellerSpieler();
                if (Spielfeld.IsHuman[AktuellerSpieler] && Spielfeld.Mensch)
                {
                    bool found = false;
                        int i = AktuellerSpieler;
                        for (int b = 0; b < 4; b++)
                        {
                            if (Spielfeld.Spieler[i].EigenePosition[b]>=40  && Spielfeld.Spieler[i].BewegungMoeglich(b, Spielfeld.MWurf))
                            {
                                int q = Spielfeld.Spieler[i].EigenePosition[b] - 40;
                                if (X >= Spiel_MADN.savex[i * 4 + q] && X <= Spiel_MADN.savex[i * 4 + q] + 80 && Y >= Spiel_MADN.savey[i * 4 + q] && Y <= Spiel_MADN.savey[i * 4 + q] + 80)
                                {
                                    // im save bereich gedrückt
                                    Spielfeld.BewegeFigur(b, Spielfeld.MWurf);
                                    found = true;
                                    break;
                                }

                            }
                        }
                        
                        if (!found)
                        {
                            i = AktuellerSpieler;
                            int akt = 0;
                            for (int b = 0; b < 4; b++)
                            {
                                if (Spielfeld.Spieler[i].GetEigenePosition(b) < 0){

                                    if (Spielfeld.MWurf == 6 && Spielfeld.Spieler[i].BewegungMoeglich(b, Spielfeld.MWurf))
                                    {
                                        int x = Spiel_MADN.startx[i * 4 + akt];
                                        int y = Spiel_MADN.starty[i * 4 + akt];
                                        if (X >= x && X <= x + 80 && Y >= y && Y <= y + 80)
                                        {
                                            // im start bereich gedrückt
                                            Spielfeld.SetNextFigur();
                                            if (!checkBox5.Checked) Spielfeld.SystemMessage(Spielfeld.Name(AktuellerSpieler) + " setzt neue Figur", true);
                                            found = true;
                                            break;
                                        }
                                    }
                                    akt++;
                                }
                            }
                        }

                        if (!found)
                        {
                            i = AktuellerSpieler;
                            for (int b = 0; b < 4; b++)
                            {
                                int d = -1;
                                for (int q = 0; q < 4; q++)
                                    if (Spielfeld.Spieler[i].GetEigenePosition(q) == 0)
                                    { d = q; break; }

                                if (Spielfeld.Spieler[i].GetEigenePosition(b) >= 0 && Spielfeld.Spieler[i].BewegungMoeglich(b, Spielfeld.MWurf) && ((d > -1 && b == d) || d == -1 || (d > -1 && !Spielfeld.Spieler[i].BewegungMoeglich(d, Spielfeld.MWurf))))
                                {
                                    int x = Spiel_MADN.datax[(Spielfeld.Spieler[i].GetEigenePosition(b) + 10 * i) % 40];
                                    int y = Spiel_MADN.datay[(Spielfeld.Spieler[i].GetEigenePosition(b)+ 10*i)%40];
                                    if (X >= x && X <= x + 80 && Y >= y && Y <= y + 80)
                                    {
                                        // Auf dem Spielfeld gedrückt
                                        Spielfeld.BewegeFigur(b, Spielfeld.MWurf);
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    

                    if (found)
                    {
                        Spielfeld.AktualisiereSpielfeld();
                        if (Spielfeld.MWurf != 6)
                        {
                            Spielfeld.Mensch = false;
                            timer4.Enabled = false;
                            pictureBox4.Hide();
                            timer1.Enabled = true;
                        }
                        else
                        {
                            Spielfeld.MWurf = Spielfeld.Wuerfeln();
                            Spielfeld.SystemMessage(Spielfeld.Name(Spielfeld.GetAktuellerSpieler()) + " Würfelt eine " + Convert.ToString(Spielfeld.MWurf), true);
                            Spielfeld.AktualisiereSpielfeld();
                        }
                    }
                }

            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < eingabe.Count(); i++)
            {
                if (eingabe[i] == sender)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        Spielfeld.Spieler[i].Name = eingabe[i].Text;
                    }
                    break;
                }
            }
        }
    }

    public class Spiel_MADN
    {
        public static int[] datax = { 80, 230, 400, 565, 770, 770, 770, 770, 770, 950, 1125, 1125, 1125, 1125, 1125, 1280, 1460, 1630, 1800, 1800, 1800, 1630, 1460, 1280, 1125, 1125, 1125, 1125, 1125, 950, 770, 770, 770, 770, 770, 565, 400, 230, 80, 80 }; // 782
        public static int[] datay = { 780, 780, 780, 780, 770, 590, 420, 245, 85, 85, 80, 245, 410, 570, 770, 770, 770, 770, 770, 960, 1130, 1130, 1130, 1130, 1130, 1300, 1470, 1630, 1820, 1820, 1810, 1650, 1470, 1300, 1130, 1130, 1130, 1130, 1130, 960 }; // 420
        public static int[] savex = { 245, 420, 595, 765, 950, 950, 945, 950, 1635, 1470, 1295, 1125, 950, 950, 945, 950 };
        public static int[] savey = { 960, 960, 960, 960, 245, 420, 595, 770, 960, 960, 955, 960, 1650, 1475, 1300, 1130 };
        public static int[] startx = { 75, 255, 255, 75, 1620, 1800, 1805, 1620, 1620, 1800, 1805, 1620, 75, 255, 255, 75 };
        public static int[] starty = { 85, 80, 245, 245, 80, 80, 245, 245, 1645, 1640, 1830, 1825, 1645, 1640, 1830, 1830 };
        public static int[] startxB = { 255, 1620, 1620, 255 };
        public static int[] startyB = { 245, 245, 1645, 1640 };


        int[,] Geworfen = new int[4, 6];
        int[] Spielfeld = new int[40];
        KI SpielerA = new Spieler1();
        KI SpielerB = new Spieler1();
        KI SpielerC = new Spieler1();
        KI SpielerD = new Spieler1();

        // Mensch
        public bool Mensch = false;
        public int MWurf=1;
        public bool mode = false;


        public bool[] IsHuman = new bool[4];
        public KI[] Spieler = new KI[4];
        private System.Random rand = new System.Random();
        public RichTextBox Alpha = new RichTextBox();
        private int Sicherheitszahl = -1;
        System.Drawing.Font fntFont = new Font("Courier New", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 178, false);
        int[] liste2;

        private Form1 _frm1;
        public Form1 frm1
        {
            set { this._frm1 = value; }
            get { return _frm1; }
        }

        public void Unlock()
        {
            if (Sicherheitszahl == -1) { Random a = new Random(); Sicherheitszahl = a.Next(0, int.MaxValue); }
            for (int i = 0; i < 4; i++) Spieler[i].Secured = 1200;
        }

        public void Lock()
        {
            for (int i = 0; i < 4; i++) Spieler[i].Secured = 0;
        }

        public void SystemMessage(String Text, bool player)
        {
            Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox1.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox1.SelectionColor = Color.Black;
            }

            if (frm1.checkBox3.Checked)
            {
                if (!frm1.checkBox6.Checked)
                {
                    frm1.richTextBox2.AppendText(Text + "\n");
                    frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                    if (player)
                    {
                        frm1.richTextBox2.SelectionColor = Farb[AktuellerSpieler];
                    }
                    else
                        frm1.richTextBox2.SelectionColor = Color.Black;
                }
                else
                {
                    Alpha.AppendText(Text + "\n");
                    Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                    if (player)
                    {
                        Alpha.SelectionColor = Farb[AktuellerSpieler];
                    }
                    else
                        Alpha.SelectionColor = Color.Black;
                    Alpha.SelectionFont = fntFont;
                }


            }

            Protokol(Text);
        }

        public void SystemMessage2(String Text, bool player)
        {
            Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox1.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox1.SelectionColor = Color.Black;
            }

            if (!frm1.checkBox6.Checked)
            {
                frm1.richTextBox2.AppendText(Text + "\n");
                frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox2.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox2.SelectionColor = Color.Black;
            }
            else
            {
                Alpha.AppendText(Text + "\n");
                Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    Alpha.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    Alpha.SelectionColor = Color.Black;
                Alpha.SelectionFont = fntFont;
            }
            Protokol(Text);
        }

        public void SystemMessageF(String Text)
        {
            if (!Spieler[AktuellerSpieler].GetFehlerAktiv() && !frm1.checkBox4.Checked) return;

            if (frm1.checkBox1.Checked && !frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                frm1.richTextBox1.SelectionColor = Color.Violet;
            }

            frm1.Fehler[AktuellerSpieler]++;
            for (int c = 0; c < 4; c++) Spieler[c].Fehler[AktuellerSpieler]++;

            if (frm1.checkBox3.Checked)
            {
                if (!frm1.checkBox6.Checked)
                {
                    frm1.richTextBox2.AppendText(Text + "\n");
                    frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                    frm1.richTextBox2.SelectionColor = Color.Violet;
                }
                else
                {
                    Alpha.AppendText(Text + "\n");
                    Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                    Alpha.SelectionColor = Color.Violet;
                    Alpha.SelectionFont = fntFont;
                }

            }
            frm1.Protokoll_anzahl++;
            Protokol(Text);
            Lock();
            Spieler[AktuellerSpieler].OnFailure();
            Unlock();
            for (int c = 0; c < KI.Daten.SystemMessage.Count; c++) SystemMessage(KI.Daten.SystemMessage[c], true);
            KI.Daten.SystemMessage.Clear();
            fehler = true;
        }

        private void Protokol(String Text)
        {
            if (!frm1.checkBox6.Checked) frm1.label1.Text = "Fehler: " + Convert.ToString(frm1.Protokoll_anzahl);
        }

        private void ProtClear()
        {
            String a = frm1.richTextBox1.Text;
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
            }
        }

        private int SiegerPruefen()
        {
            for (int i = 0; i < 4; i++)
            {
                bool sieg = true;
                for (int b = 0; b < 4; b++)
                {
                    if (Spieler[i].Spielfeld[43 - b] != Spieler[i].GetFarbe()) { sieg = false; break; }
                }

                if (sieg == true) return i + 1;
            }

            return 0;
        }

        private int AktuellerSpieler = -1;

        public int GetAktuellerSpieler()
        {
            return AktuellerSpieler;
        }

        public void Init(int[] liste)
        {
           liste2 = liste;
           // ComboBox[] liste = { frm1.comboBox2, frm1.comboBox3, frm1.comboBox4, frm1.comboBox5 };
            KI[] SpielerSorten = { new Spieler1(), new Spieler2(), new Spieler3(), new Spieler4() };
            for (int i = 0; i < liste2.Count(); i++)
            {
                switch (liste[i]){
                    case 0:
                        Spieler[i] = new Spieler1();IsHuman[i] = false;
                          break;
                    case 1:
                          Spieler[i] = new Spieler2();IsHuman[i] = false;
                          break;
                    case 2:
                          Spieler[i] = new Spieler3();IsHuman[i] = false;
                          break;
                    case 3:
                          Spieler[i] = new Spieler4();IsHuman[i] = false;
                          break;
                    default:
                          Spieler[i] = new Spieler4();
                          if (Form1.eingabe[i]!=null) Spieler[i].Name = Form1.eingabe[i].Text;
                          IsHuman[i] = true;
                          break;

                }
            }
            for (int i = 0; i < 4; i++) { AktuellerSpieler = i; Unlock(); Unlock(); Spieler[i].SetFarbe(i + 1); Spieler[i].init(); }
            for (int i = 0; i < 40; i++) Spielfeld[i] = 0;
            //  for (int i = 0; i < 10; i++) Wuerfeln();
            AktuellerSpieler = rand.Next(0, 4);

        }

        public Spiel_MADN(int[] liste)
        {

            Init(liste);

        }

        public int Wuerfeln()
        {
            int a = rand.Next(1, 7);
            return a;
        }

        public bool BewegeFigur(int ID, int wurf)
        {
            if (Spieler[AktuellerSpieler].GetEigenePosition(ID) < 0) { SystemMessageF("Fehler 10: Figur ist nicht auf dem Feld"); return false; }
            if (Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf >= 40)
            {// will in save

                if (Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf >= 44) { SystemMessageF("Fehler 15: Ausserhalb des Save"); return false; }
                int start = Spieler[AktuellerSpieler].GetEigenePosition(ID) + 1;
                if (start < 40) start = 40;
                for (int i = start; i <= Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf && i < 44; i++)
                {
                    if (Spieler[AktuellerSpieler].Spielfeld[i] > 0)
                    {
                        SystemMessageF("Fehler 16: Save ist Besetzt"); return false;
                    }
                }

                int a = 0, b = 0;

                if (Spieler[AktuellerSpieler].GetEigenePosition(ID) >= 40)
                {
                    Spieler[AktuellerSpieler].Spielfeld[Spieler[AktuellerSpieler].GetEigenePosition(ID)] = 0;
                    a = 2;
                    b = Spieler[AktuellerSpieler].GetEigenePosition(ID) - 40;
                }
                else
                {
                    Spielfeld[(40 + Spieler[AktuellerSpieler].GetEigenePosition(ID) + (AktuellerSpieler * 10)) % 40] = 0;
                    a = 1;
                    b = (40 + Spieler[AktuellerSpieler].GetEigenePosition(ID) + (AktuellerSpieler * 10)) % 40;
                    frm1.InSave[AktuellerSpieler]++;
                    for (int c = 0; c < 4; c++) Spieler[c].InSave[AktuellerSpieler]++;
                }



                frm1.setLastMove(AktuellerSpieler, a, b, 2, Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf - 40);
                Spieler[AktuellerSpieler].EigenePosition[ID] = Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf;
                Spieler[AktuellerSpieler].Spielfeld[Spieler[AktuellerSpieler].GetEigenePosition(ID)] = Spieler[AktuellerSpieler].GetFarbe();
                AktualisiereSpielfeld2(); // ???

            }
            else
            {

                if (!SetFigurAufPosition(ID, (Spieler[AktuellerSpieler].GetEigenePosition(ID) + wurf + (AktuellerSpieler * 10)) % 40)) { SystemMessageF("Fehler 3: Bewegen nicht möglich"); return false; }
                else
                {

                    AktualisiereSpielfeld2(); // ???
                }
            }
            return true;
        }

        private bool SetFigurAufPosition(int ID, int pos)
        {
            if (pos < 0 || pos >= 40) { SystemMessageF("Fehler 4: Fehlerhafte Eingabe (" + Convert.ToString(pos) + ")"); return false; }
            if (Spielfeld[pos] == Spieler[AktuellerSpieler].GetFarbe()) { SystemMessageF("Fehler 5: Mit eigenem Spieler belegt"); return false; }
            if (Spielfeld[pos] > 0 && (pos == 4 || pos == 14 || pos == 24 || pos == 34)) { SystemMessageF("Fehler 13: Zielfeld ist Sicherheitsfeld"); return false; }

            if (Spielfeld[pos] > 0)
            {
                // Figur platt gemacht
                int betroffen = Spielfeld[pos] - 1;
                for (int i = 0; i < 4; i++)
                {
                    if (Spieler[betroffen].GetEigenePosition(i) >= 40) continue;
                    if (Spieler[betroffen].GetEigenePosition(i) < 0) continue;
                    if ((40 + Spieler[betroffen].GetEigenePosition(i) + (betroffen * 10)) % 40 == pos)
                    {
                        frm1.setLastMove(betroffen, 1, pos, 0, 0);
                        Spieler[betroffen].EntferneFigur(i);
                        frm1.FigurenVerloren[betroffen]++;
                        frm1.FigurenBesiegt[AktuellerSpieler]++;
                        for (int c = 0; c < 4; c++)
                        {
                            Spieler[c].FigurenVerloren[betroffen]++;
                            Spieler[c].FigurenBesiegt[AktuellerSpieler]++;
                        }
                        if (!frm1.checkBox5.Checked) SystemMessage(Name(betroffen) + " verliert Figur", true);
                        break;
                    }

                }


            }

            int a = 0, b = 0;
            if (Spieler[AktuellerSpieler].EigenePosition[ID] >= 0)
            {
                a = 1;
                b = (Spieler[AktuellerSpieler].GetEigenePosition(ID) + (AktuellerSpieler * 10)) % 40;
                Spielfeld[(Spieler[AktuellerSpieler].GetEigenePosition(ID) + (AktuellerSpieler * 10)) % 40] = 0;
            }
            else
            {
                a = 0;
                b = 0;
            }
            frm1.setLastMove(AktuellerSpieler, a, b, 1, pos);

            Spielfeld[pos] = AktuellerSpieler + 1;
            Spieler[AktuellerSpieler].EigenePosition[ID] = (40 + pos - (AktuellerSpieler * 10)) % 40;
            AktualisiereSpielfeld2(); // ???
            return true;

        }

        public void AktualisiereSpielfeld2()
        {
            for (int i = 0; i < 4; i++)
            {
                KI.Freie[i] = Spieler[i].GetEigeneFrei();
                for (int b = 0; b < 4; b++)
                    KI.Saved[i, b] = Spieler[i].Spielfeld[40 + b];

                for (int b = 0; b < 40; b++)
                {
                    Spieler[i].Spielfeld[(40 + (b - (i) * 10)) % 40] = Spielfeld[b];
                }
            }

        }

        public void AktualisiereSpielfeld()
        {
            AktualisiereSpielfeld2();


            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {

                Bitmap temp = new Bitmap(frm1.pictureBox2.Image);
                Graphics g = Graphics.FromImage(temp);

                Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
                mode = mode == false ? true : false;

                for (int i = 0; i < 4; i++)
                {
                    for (int b = 0; b < 4; b++)
                    {
                        if (Spieler[i].Spielfeld[40 + b] > 0)
                        {
                            SolidBrush blackBrush = new SolidBrush(Color.Black);
                            Pen a = new Pen(Farb[i], 10);
                            int c=0;
                            for (; c < 4; c++)
                                if (Spieler[i].GetEigenePosition(c) == 40 + b)
                                    break;

                            if (mode && IsHuman[i] && AktuellerSpieler == i &&  Spieler[i].BewegungMoeglich(c, MWurf))
                            {
                                blackBrush = new SolidBrush(Farb[i]);
                                a = new Pen(Color.Black, 10);
                            }
                            
                            g.FillEllipse(blackBrush, savex[i * 4 + b], savey[i * 4 + b], 80, 80);
                            blackBrush.Dispose();
                            g.DrawEllipse(a, savex[i * 4 + b], savey[i * 4 + b], 80, 80);
                            a.Dispose();
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                        int anz = 0;
                        for (int b = 0; b < 4; b++)
                        {
                            if (Spieler[i].GetEigenePosition(b) < 0)
                            {
                                SolidBrush blackBrush = new SolidBrush(Color.Black);
                                Pen a = new Pen(Farb[i], 10);
                                if (mode && IsHuman[i] && AktuellerSpieler == i && MWurf==6 && Spieler[i].BewegungMoeglich(b, MWurf))
                                {
                                    blackBrush = new SolidBrush(Farb[i]);
                                    a = new Pen(Color.Black, 10);
                                }

                                g.FillEllipse(blackBrush, startx[i * 4 + anz], starty[i * 4 + anz], 80, 80);
                                blackBrush.Dispose();
                                

                                g.DrawEllipse(a, startx[i * 4 + anz], starty[i * 4 + anz], 80, 80);
                                a.Dispose();
                                anz++;
                            }
                        }
                    
                }

                for (int b = 0; b < datax.Length; b++)
                {
                    if (Spielfeld[b] > 0)
                    {
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        Pen a = new Pen(Farb[Spielfeld[b] - 1], 10);
                        int i = Spielfeld[b] - 1;

                        int c = 0; // Korrekt ???
                        for (; c < 4; c++)
                            if ((Spieler[i].GetEigenePosition(c)+10*i)%40 == b)
                                break;
                        int d=-1;
                        if (Spieler[i].GetEigeneSave() > 0)
                        {
                            for (int q = 0; q < 4; q++)
                                if (Spieler[i].GetEigenePosition(q) == 0)
                                { d = q; break; }
                        }

                        if (mode && IsHuman[i] && AktuellerSpieler == i && Spieler[i].BewegungMoeglich(c, MWurf) && ((d>-1 && c==d) || d==-1 || (d>-1 && !Spieler[i].BewegungMoeglich(d, MWurf))))
                        {
                            blackBrush = new SolidBrush(Farb[i]);
                            a = new Pen(Color.Black, 10);
                        }

                        
                        g.FillEllipse(blackBrush, datax[b], datay[b], 80, 80);
                        blackBrush.Dispose();
                        
                        g.DrawEllipse(a, datax[b], datay[b], 80, 80);
                        a.Dispose();
                    }

                }



                int size = 15;
                for (int i = 0; i < frm1.lastplayer.Length; i++, size -= 3)
                {
                    if (frm1.lastmovefrom[i] != -1 && frm1.lastmoveto[i] != -1)
                    {
                        int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                        if (frm1.lastmovefrom[i] == 0)
                        {
                            x1 = startxB[frm1.lastplayer[i]]; y1 = startyB[frm1.lastplayer[i]];

                        }
                        else
                            if (frm1.lastmovefrom[i] == 1)
                            {
                                x1 = datax[frm1.lastmovefromID[i]]; y1 = datay[frm1.lastmovefromID[i]];

                            }
                            else
                                if (frm1.lastmovefrom[i] == 2)
                                {
                                    x1 = savex[frm1.lastplayer[i] * 4 + frm1.lastmovefromID[i]]; y1 = savey[frm1.lastplayer[i] * 4 + frm1.lastmovefromID[i]];

                                }

                        if (frm1.lastmoveto[i] == 0)
                        {
                            x2 = startxB[frm1.lastplayer[i]]; y2 = startyB[frm1.lastplayer[i]];
                        }
                        else
                            if (frm1.lastmoveto[i] == 1)
                            {
                                x2 = datax[frm1.lastmovetoID[i]]; y2 = datay[frm1.lastmovetoID[i]];

                            }
                            else
                                if (frm1.lastmoveto[i] == 2)
                                {
                                    x2 = savex[frm1.lastplayer[i] * 4 + frm1.lastmovetoID[i]]; y2 = savey[frm1.lastplayer[i] * 4 + frm1.lastmovetoID[i]];

                                }

                        Pen a = new Pen(Farb[frm1.lastplayer[i]], size);
                        g.DrawLine(a, x1 + 40, y1 + 40, x2 + 40, y2 + 40);
                        a.Dispose();
                    }

                }

                if (Mensch)
                {
                    float scale = (float)frm1.pictureBox2.Image.Height / frm1.pictureBox2.Height;
                    frm1.pictureBox4.Load("Bilder\\" + MWurf.ToString() + ".png");
                   // frm1.pictureBox4.
                    Bitmap bmp = new Bitmap(frm1.pictureBox4.Width, frm1.pictureBox4.Height);
                    frm1.pictureBox4.DrawToBitmap(bmp, new Rectangle(0, 0, frm1.pictureBox4.Width, frm1.pictureBox4.Height));

                    frm1.pictureBox4.Left -= frm1.pictureBox2.Left;
                    frm1.pictureBox4.Top -= frm1.pictureBox2.Top;
                    g.DrawImage(bmp, new Rectangle((int) (frm1.pictureBox4.Left * scale), (int) (frm1.pictureBox4.Top * scale), (int) ((float)frm1.pictureBox4.Width * scale/2), (int) ((float) frm1.pictureBox4.Height * scale/2)));
                    frm1.pictureBox4.Left += frm1.pictureBox2.Left;
                    frm1.pictureBox4.Top += frm1.pictureBox2.Top;
                }


                frm1.pictureBox1.Image = temp;
                g.Dispose();
            }
        }

        public bool SetNextFigur()
        {

            if (Spieler[AktuellerSpieler].GetEigeneFrei() <= 0) { SystemMessageF("Fehler 1: Keine Figuren vorhanden"); return false; }
            if (Spieler[AktuellerSpieler].Spielfeld[0] == Spieler[AktuellerSpieler].GetFarbe()) { SystemMessageF("Fehler 2: Start ist Besetzt"); return false; }
            for (int i = 0; i < 4; i++)
            {
                if (Spieler[AktuellerSpieler].GetEigenePosition(i) < 0)
                {
                    SetFigurAufPosition(i, (0 + AktuellerSpieler * 10) % 40);
                    return true;
                }
            }


            return false;
        }

        String Make10(String Text, int anz)
        {
            for (; Text.Length < anz; ) Text = Text + " ";
            if (Text.Length > anz) Text = Text.Substring(0, anz);
            return Text;
        }

        public String Name(int id)
        {
            return Spieler[id].Name == "Unbenannt" ? "Spieler " + Convert.ToString(Spieler[id].GetFarbe()) : Spieler[id].Name;
        }

        public bool Sieger()
        {
            if (SiegerPruefen() > 0)
            {
                if (!frm1.checkBox5.Checked) SystemMessage("Sieger ist: " + Name(SiegerPruefen() - 1), false);
                frm1.SpieleGewonnen[SiegerPruefen() - 1]++;
                for (int c = 0; c < 4; c++)
                {
                    Spieler[c].SpieleGewonnen[SiegerPruefen() - 1]++;
                }

                frm1.Spiele--;
                if (frm1.Spiele > 0)
                {
                    Init(liste2);
                    for (int i = 0; i < 4; i++) Spieler[i].init();
                    AktualisiereSpielfeld();
                    frm1.timer1.Enabled = true;
                }
                else
                {
                    SystemMessage(" ", false);
                    SystemMessage(" ", false);
                    SystemMessage2("---Auswertung---", false);
                    for (int i = 0; i < 4; i++)
                    {
                        AktuellerSpieler = i;
                        SystemMessage2(Name(AktuellerSpieler), true);
                        SystemMessage2("Spiele gewonnen: " + Convert.ToString(frm1.SpieleGewonnen[i]), true);
                        SystemMessage2("Figuren besiegt: " + Convert.ToString(frm1.FigurenBesiegt[i]), true);
                        SystemMessage2("Figuren verloren: " + Convert.ToString(frm1.FigurenVerloren[i]), true);
                        SystemMessage2("Gesicherte Figuren: " + Convert.ToString(frm1.InSave[i]), true);
                        SystemMessage2("Fehler: " + Convert.ToString(frm1.Fehler[i]), true);
                        SystemMessage2(" ", false);
                    }
                    SystemMessage(" ", false);
                    SystemMessage2("---Würfel---", false);
                    for (int i = 0; i < 4; i++)
                    {
                        AktuellerSpieler = i;
                        SystemMessage2(Name(AktuellerSpieler), true);
                        for (int b = 0; b < 6; b++)
                        {
                            SystemMessage2(Convert.ToString(b + 1) + ": " + Geworfen[i, b], true);
                        }
                    }
                    int GesamtSpiele = 0;
                    for (int i = 0; i < 4; i++) GesamtSpiele += frm1.SpieleGewonnen[i];
                    SystemMessage(" ", false);
                    SystemMessage2("---Statistik---", false);
                    String text = ""; for (int i = 0; i < 4; i++) text = text + Make10(Name(i), 15); SystemMessage2("Name: " + text, false);
                    SystemMessage(" ", false);
                    SystemMessage2("pro Spiel:", false);
                    text = ""; for (int i = 0; i < 4; i++) text = text + Make10(Convert.ToString(Math.Round((double)frm1.InSave[i] / GesamtSpiele, 2)), 13) + "  "; SystemMessage2("Save: " + text, false);
                    text = ""; for (int i = 0; i < 4; i++) text = text + Make10(Convert.ToString(Math.Round((double)frm1.FigurenVerloren[i] / GesamtSpiele, 2)), 13) + "  "; SystemMessage2("Verl: " + text, false);
                    text = ""; for (int i = 0; i < 4; i++) text = text + Make10(Convert.ToString(Math.Round((double)frm1.FigurenBesiegt[i] / GesamtSpiele, 2)), 13) + "  "; SystemMessage2("Besi: " + text, false);

                    frm1.timer1.Enabled = false;
                    frm1.timer4.Enabled = false;
                }
                return true;
            }
            return false;
        }

        bool fehler = false;
        public void Next()
        {
            Unlock();
            if (frm1.Spiele == 0) { frm1.timer1.Enabled = false; return; }
            AktualisiereSpielfeld2(); // ?

            if (Sieger()) return;

            AktuellerSpieler++;
            frm1.Spielzug++;
            if (AktuellerSpieler >= 4) AktuellerSpieler = 0;
            int Wurf = 0;
            if (Spieler[AktuellerSpieler].GetEigeneFigurenAnzahl() - Spieler[AktuellerSpieler].GetEigeneSave() <= 0 && !Spieler[AktuellerSpieler].BewegungEinerMoeglich(1))
            {

                for (int i = 0; i < 3 && Wurf != 6; i++)
                {
                    Wurf = Wuerfeln();
                }

            }
            else
                Wurf = Wuerfeln();

            Geworfen[AktuellerSpieler, Wurf - 1]++; // geworfene zählen

            if (IsHuman[AktuellerSpieler])
            {
                // Spielfeld.NextHuman();
                frm1.timer1.Enabled = false;
                Mensch = true;
                frm1.timer4.Enabled = true;
                MWurf = Wurf;
                SystemMessage(Name(AktuellerSpieler) + " Würfelt eine " + Convert.ToString(Wurf), true);
                return;
            }
            else
            {
                frm1.timer4.Enabled = false;
            }

            for (; ; )
            {
                fehler = false;
                // int Wuerfel, int[] Spielfeld, int[] EigenePosition, int Farbe
                if (!frm1.checkBox5.Checked) SystemMessage(Name(AktuellerSpieler) + " Würfelt eine " + Convert.ToString(Wurf), true);
                Lock();
                int temp = Wurf;

                int res = 0;
                if (!IsHuman[AktuellerSpieler])
                {
                    res = Spieler[AktuellerSpieler].Aufruf(Wurf);
                }
                else
                {
                    int ee = 0; int qqweqwr = ee + 1;
                }

                Wurf = temp;
                Unlock();
                for (int c = 0; c < KI.Daten.SystemMessage.Count; c++) SystemMessage(KI.Daten.SystemMessage[c], true);
                KI.Daten.SystemMessage.Clear();
                for (int c = 0; c < KI.Daten.SystemMessageF.Count; c++) SystemMessage(KI.Daten.SystemMessageF[c], true);
                KI.Daten.SystemMessageF.Clear();

                bool move = false;

                // Bearbeitung
                if (res >= 0 && res <= 5)
                {
                    bool start = false;
                    int startID = -1;
                    for (int i = 0; i < 4; i++) if (Spieler[AktuellerSpieler].GetEigenePosition(i) == 0) { startID = i; break; }

                    if (Spieler[AktuellerSpieler].Spielfeld[0] == Spieler[AktuellerSpieler].GetFarbe() && Spieler[AktuellerSpieler].BewegungMoeglich(startID, Wurf)) start = true;

                    if (res == 4)
                    {
                        // Auf Feld setzen
                        if (Wurf == 6)
                        {
                            if (!SetNextFigur()) { SystemMessageF("Fehler 8: Konnte Figur nicht auf Feld setzen"); }
                            else
                            { if (!frm1.checkBox5.Checked) SystemMessage(Name(AktuellerSpieler) + " setzt neue Figur", true); move = true; }
                        }
                        else
                            SystemMessageF("Fehler 9: Falsche Würfelzahl");
                    }
                    else
                    {
                        if (res != 5)
                        {
                            if (!BewegeFigur(res, Wurf)) { SystemMessageF("Fehler 11: Bewegen erfolglos"); }
                            else { move = true; }
                        }
                    }

                    AktualisiereSpielfeld();
                    if (start && Spieler[AktuellerSpieler].Spielfeld[0] == Spieler[AktuellerSpieler].GetFarbe()) { SystemMessageF("Fehler 17: Start muss frei gemacht werden"); }

                    if (move == false && Spieler[AktuellerSpieler].BewegungEinerMoeglich(Wurf))
                    {
                        SystemMessageF("Fehler 12: Bewegung wäre möglich gewesen");
                    }
                }
                else
                    SystemMessageF("Fehler 6: Fehlerhafte Rückgabe");

                AktualisiereSpielfeld();
                if (fehler)
                {
                    SystemMessage(Name(AktuellerSpieler) + " (" + Spieler[AktuellerSpieler].Name + ")>>Wurf: " + Wurf.ToString() + ">>Antwort: " + res.ToString(), false);
                }


                if (Wurf != 6) break;
                Wurf = Wuerfeln();

            }

        }

    }

    public class Spiel_Monopoly
    {
        int[,] Geworfen = new int[4, 6];
        //  int[] Spielfeld = new int[40];
        KI_Monopoly SpielerA = new Spieler1_Monopoly();
        KI_Monopoly SpielerB = new Spieler2_Monopoly();
        KI_Monopoly SpielerC = new Spieler3_Monopoly();
        KI_Monopoly SpielerD = new Spieler4_Monopoly();
        KI_Monopoly SpielerE = null;
        KI_Monopoly SpielerF = null;
        public KI_Monopoly[] Spieler = new KI_Monopoly[6];
        private System.Random rand = new System.Random();
        public RichTextBox Alpha = new RichTextBox();
        private int Sicherheitszahl = -1;
        System.Drawing.Font fntFont = new Font("Courier New", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 178, false);

        private Form1 _frm1;
        public Form1 frm1
        {
            set { this._frm1 = value; }
            get { return _frm1; }
        }

        public void Unlock()
        {
            if (Sicherheitszahl == -1) { Random a = new Random(); Sicherheitszahl = a.Next(0, int.MaxValue); }
            for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; Spieler[i].Secured = Sicherheitszahl; }
        }

        public void Lock()
        {
            for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; Spieler[i].Secured = 0; }
        }

        public void SystemMessage(String Text, bool player)
        {
            Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox1.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox1.SelectionColor = Color.Black;
            }

            if (frm1.checkBox3.Checked)
            {
                if (!frm1.checkBox6.Checked)
                {
                    frm1.richTextBox2.AppendText(Text + "\n");
                    frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                    if (player)
                    {
                        frm1.richTextBox2.SelectionColor = Farb[AktuellerSpieler];
                    }
                    else
                        frm1.richTextBox2.SelectionColor = Color.Black;
                }
                else
                {
                    Alpha.AppendText(Text + "\n");
                    Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                    if (player)
                    {
                        Alpha.SelectionColor = Farb[AktuellerSpieler];
                    }
                    else
                        Alpha.SelectionColor = Color.Black;
                    Alpha.SelectionFont = fntFont;
                }


            }

            Protokol(Text);
        }

        public void SystemMessage2(String Text, bool player)
        {
            Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange };
            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox1.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox1.SelectionColor = Color.Black;
            }

            if (!frm1.checkBox6.Checked)
            {
                frm1.richTextBox2.AppendText(Text + "\n");
                frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    frm1.richTextBox2.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    frm1.richTextBox2.SelectionColor = Color.Black;
            }
            else
            {
                Alpha.AppendText(Text + "\n");
                Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                if (player)
                {
                    Alpha.SelectionColor = Farb[AktuellerSpieler];
                }
                else
                    Alpha.SelectionColor = Color.Black;
                Alpha.SelectionFont = fntFont;
            }
            Protokol(Text);
        }

        public void SystemMessageF(String Text)
        {
            if (!Spieler[AktuellerSpieler].GetFehlerAktiv() && !frm1.checkBox4.Checked) return;

            if (frm1.checkBox1.Checked && !frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                ProtClear();
                frm1.richTextBox1.AppendText(Text + "\n");
                frm1.richTextBox1.Select(frm1.richTextBox1.Text.Length - Text.Length - 1, Text.Length + 1);
                frm1.richTextBox1.SelectionColor = Color.Violet;
            }

            frm1.Fehler[AktuellerSpieler]++;
            for (int c = 0; c < 4; c++) Spieler[c].Fehler[AktuellerSpieler]++;

            if (frm1.checkBox3.Checked)
            {
                if (!frm1.checkBox6.Checked)
                {
                    frm1.richTextBox2.AppendText(Text + "\n");
                    frm1.richTextBox2.Select(frm1.richTextBox2.Text.Length - Text.Length - 1, Text.Length + 1);
                    frm1.richTextBox2.SelectionColor = Color.Violet;
                }
                else
                {
                    Alpha.AppendText(Text + "\n");
                    Alpha.Select(Alpha.Text.Length - Text.Length - 1, Text.Length + 1);
                    Alpha.SelectionColor = Color.Violet;
                    Alpha.SelectionFont = fntFont;
                }

            }
            frm1.Protokoll_anzahl++;
            Protokol(Text);
            Lock();
            Spieler[AktuellerSpieler].OnFailure();
            Unlock();
            for (int c = 0; c < KI.Daten.SystemMessage.Count; c++) SystemMessage(KI.Daten.SystemMessage[c], true);
            KI.Daten.SystemMessage.Clear();
            fehler = true;
        }

        private void Protokol(String Text)
        {
            if (!frm1.checkBox6.Checked) frm1.label1.Text = "Fehler: " + Convert.ToString(frm1.Protokoll_anzahl);
        }

        private void ProtClear()
        {
            String a = frm1.richTextBox1.Text;
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
            }
        }

        private int SiegerPruefen()
        {
            bool sieg = true;
            int found = -1;
            for (int i = 0; i < 6; i++)
            {
                if (Spieler[i] == null) continue;
                if (Spieler[i].Aktiv == true && found == -1)
                {
                    found = i;
                }
                else
                    if (Spieler[i].Aktiv == true && found != -1)
                    {
                        sieg = false; break;
                    }
            }

            if (sieg == true) return found + 1;
            return 0;
        }

        private int AktuellerSpieler = -1;

        public int GetAktuellerSpieler()
        {
            return AktuellerSpieler;
        }

        public void Init()
        {
            Spieler[0] = SpielerA;
            Spieler[1] = SpielerB;
            Spieler[2] = SpielerC;
            Spieler[3] = SpielerD;
            Spieler[4] = SpielerE;
            Spieler[5] = SpielerF;
            for (int i = 0; i < 6; i++)
            {
                if (Spieler[i] == null) continue;
                AktuellerSpieler = i;
                Unlock(); Unlock();
                Spieler[i].SetFarbe(i + 1);
                Spieler[i].init();
                for (int b = 0; b < 6; b++) Spieler[i].SetSpieler(Spieler[b], b);
            }

            // for (int i = 0; i < 40; i++) Spielfeld[i] = 0;
            bool check = false;
            for (int i = 0; i < 6; i++) if (Spieler[i] != null) { check = true; break; }
            if (check)
            {
                AktuellerSpieler = -1;
                for (; AktuellerSpieler == -1 || Spieler[AktuellerSpieler] == null; ) AktuellerSpieler = rand.Next(0, 6);
            }
            else
                AktuellerSpieler = 0;
        }

        public Spiel_Monopoly()
        {
            Init();
        }

        public void AktualisiereSpielfeld2()
        {
            /*for (int i = 0; i < 4; i++)
            {
                KI.Freie[i] = Spieler[i].GetEigeneFrei();
                for (int b = 0; b < 4; b++)
                    KI.Saved[i, b] = Spieler[i].Spielfeld[40 + b];

                for (int b = 0; b < 40; b++)
                {
                    Spieler[i].Spielfeld[(40 + (b - (i) * 10)) % 40] = Spielfeld[b];
                }
            }*/

        }

        public void AktualisiereSpielfeld()
        {
            AktualisiereSpielfeld2();

            if (!frm1.checkBox2.Checked && !frm1.checkBox6.Checked)
            {
                Bitmap temp = new Bitmap(frm1.pictureBox3.Image);
                Graphics g = Graphics.FromImage(temp);


                int[] datax = { 850, 740, 670, 598, 523, 452, 379, 306, 233, 159, 015, 014, 014, 014, 014, 014, 014, 014, 014, 014, 014, 160, 231, 306, 378, 451, 524, 598, 671, 742, 835, 812, 812, 812, 812, 812, 812, 812, 812, 812 }; // 782
                int[] datay = { 800, 822, 822, 822, 822, 822, 822, 822, 822, 822, 884, 739, 671, 598, 526, 453, 378, 307, 234, 157, 061, 016, 016, 016, 016, 016, 016, 016, 016, 016, 016, 157, 234, 307, 378, 453, 526, 598, 671, 739 }; // 420
                int[] def = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                int[] datax2 = { 709, 570, 423, 349, 204, 129, 107, 107, 107, 107, 107, 107, 107, 107, 129, 276, 351, 426, 499, 570, 645, 719, 790, 790, 790, 790, 790, 790 };
                int[] datay2 = { 793, 793, 793, 793, 793, 793, 715, 645, 569, 497, 423, 351, 206, 131, 107, 107, 107, 107, 107, 107, 107, 107, 131, 206, 351, 423, 573, 715 };
                int[] def2 = { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1 };
                int prisonx = 73;
                int prisony = 793;

                Color[] Farb = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Yellow, Color.Brown };
                int[] Spielfeld = new int[40];
                for (int i = 0; i < 40; i++) Spielfeld[i] = 0;

                int size = 10;
                for (int i = 0; i < frm1.lastplayer.Length; i++, size -= 2)
                {
                    if (frm1.lastmovefrom[i] != -1 && frm1.lastmoveto[i] != -1)
                    {
                        int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                        x1 = datax[frm1.lastmovefromID[i]]; y1 = datay[frm1.lastmovefromID[i]];
                        x2 = datax[frm1.lastmovetoID[i]]; y2 = datay[frm1.lastmovetoID[i]];
                        Pen a = new Pen(Farb[frm1.lastplayer[i]], size);
                        g.DrawLine(a, x1 - 10, y1 - 10, x2 - 10, y2 - 10);
                        a.Dispose();
                    }

                }

                /*for (int i = 0; i < 28; i++)
                {
                    if (KI_Monopoly.Straßen[i].Typ != 0) continue;
                    KI_Monopoly.Straßen[i].Haus = 5;
                    KI_Monopoly.Straßen[i].Besitzer = 1;
                }*/

                for (int b = 0; b < 40; b++)
                {
                    if (KI_Monopoly.Felder[b].Typ != 0) continue;
                    if (KI_Monopoly.Straßen[KI_Monopoly.Felder[b].Straße].Besitzer <= 0) continue;
                   // if (KI_Monopoly.Straßen[KI_Monopoly.Felder[b].Data].Typ != 0) continue;

                    int owner = KI_Monopoly.Straßen[KI_Monopoly.Felder[b].Straße].Besitzer - 1;
                    int pos = KI_Monopoly.Felder[b].Straße;
                    int anz = KI_Monopoly.Straßen[KI_Monopoly.Felder[b].Straße].Haus;

                    if (anz == 0)
                    {
                        Pen colorBrush = new Pen(Farb[owner], 4);
                        if (def2[pos] == 1)
                        {


                            SolidBrush blackBrush = new SolidBrush(Color.White);
                            g.FillRectangle(blackBrush, datax2[pos] - 4, datay2[pos], 8, 18 * 4 - 8 - 10);
                            blackBrush.Dispose();

                            g.DrawLine(colorBrush, (float)datax2[pos], (float)datay2[pos], (float)datax2[pos], (float)datay2[pos] + 18 * 4 - 8 - 10);
                        }
                        else
                            if (def2[pos] == 0)
                            {

                                SolidBrush blackBrush = new SolidBrush(Color.White);
                                g.FillRectangle(blackBrush, datax2[pos], datay2[pos] - 4, 18 * 4 - 6 - 10, 8);
                                blackBrush.Dispose();
                                g.DrawLine(colorBrush, (float)datax2[pos], (float)datay2[pos], (float)datax2[pos] + 18 * 4 - 6 - 10, (float)datay2[pos]);

                            }
                    }

                    

                   
                    if (anz == 0) continue;
                    bool hotel = anz > 4 ? true : false;
          

                    if (hotel) anz -= 4;
                    if (def2[pos] == 1)
                    {
                        for (int c = 0; c < anz; c++)
                        {

                            SolidBrush blackBrush = new SolidBrush(Color.White);
                            g.FillRectangle(blackBrush, datax2[pos] - 7, datay2[pos] + 18 * c - 8, 15, hotel ? 30 : 15);
                            blackBrush.Dispose();

                            SolidBrush blackBrush2 = new SolidBrush(Farb[owner]);
                            g.FillRectangle(blackBrush2, datax2[pos] - 6, datay2[pos] + 18 * c - 7, 15, hotel ? 28 : 13);
                            blackBrush2.Dispose();
                        }
                    }
                    else
                        if (def2[pos] == 0)
                        {
                            for (int c = 0; c < anz; c++)
                            {

                                SolidBrush blackBrush = new SolidBrush(Color.White);
                                g.FillRectangle(blackBrush, datax2[pos] + 18 * c - 7, datay2[pos] - 8, hotel ? 30 : 15, 15);
                                blackBrush.Dispose();

                                SolidBrush blackBrush2 = new SolidBrush(Farb[owner]);
                                g.FillRectangle(blackBrush2, datax2[pos] + 18 * c - 6, datay2[pos] - 7, hotel ? 28 : 13, 13);
                                blackBrush2.Dispose();

                            }
                        }
                }

                for (int b = 0; b < 6; b++)
                {
                    if (Spieler[b] == null) continue;
                    int pos = Spieler[b].EigenePosition;
                    if (pos == KI_Monopoly.Gefängnis && Spieler[b].InJail)
                    {
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        g.FillEllipse(blackBrush, prisonx - 10, prisony + 10 * Spielfeld[pos] - 10, 20, 20);
                        blackBrush.Dispose();
                        Pen a = new Pen(Farb[b], 10);
                        g.DrawEllipse(a, prisonx - 10, prisony + 10 * Spielfeld[pos] - 10, 20, 20);
                        a.Dispose();
                    }
                    else
                        if (def[pos] == 0)
                        {
                            SolidBrush blackBrush = new SolidBrush(Color.Black);
                            g.FillEllipse(blackBrush, datax[pos] - 10, datay[pos] + 10 * Spielfeld[pos] - 10, 20, 20);
                            blackBrush.Dispose();
                            Pen a = new Pen(Farb[b], 10);
                            g.DrawEllipse(a, datax[pos] - 10, datay[pos] + 10 * Spielfeld[pos] - 10, 20, 20);
                            a.Dispose();
                        }
                        else
                            if (def[pos] == 1)
                            {
                                SolidBrush blackBrush = new SolidBrush(Color.Black);
                                g.FillEllipse(blackBrush, datax[pos] + 10 * Spielfeld[pos] - 10, datay[pos] - 10, 20, 20);
                                blackBrush.Dispose();
                                Pen a = new Pen(Farb[b], 10);
                                g.DrawEllipse(a, datax[pos] + 10 * Spielfeld[pos] - 10, datay[pos] - 10, 20, 20);
                                a.Dispose();
                            }
                    Spielfeld[pos]++;
                }

                frm1.pictureBox1.Image = temp;
                g.Dispose();
            }
        }

        String Make10(String Text, int anz)
        {
            for (; Text.Length < anz; ) Text = Text + " ";
            if (Text.Length > anz) Text = Text.Substring(0, anz);
            return Text;
        }

        bool fehler = false;
        public void Next()
        {
            Unlock();
            if (frm1.Spiele == 0) { frm1.timer1.Enabled = false; return; }
            AktualisiereSpielfeld2(); // ?
            if (SiegerPruefen() > 0)
            {
                if (!frm1.checkBox5.Checked) SystemMessage("Sieger ist: " + Convert.ToString(SiegerPruefen()), false);
                frm1.SpieleGewonnen[SiegerPruefen() - 1]++;
                for (int c = 0; c < 6; c++)
                {
                    if (Spieler[c] == null || !Spieler[c].Aktiv) continue;
                    Spieler[c].SpieleGewonnen[SiegerPruefen() - 1]++;
                }

                frm1.Spiele--;
                if (frm1.Spiele > 0)
                {
                    Init();
                    for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; Spieler[i].init(); }
                    AktualisiereSpielfeld();
                    frm1.timer1.Enabled = true;
                }
                else
                {
                    SystemMessage(" ", false);
                    SystemMessage(" ", false);
                    SystemMessage2("---Auswertung---", false);
                    for (int i = 0; i < 6; i++)
                    {
                        if (Spieler[i] == null) continue;
                        AktuellerSpieler = i;
                        SystemMessage2("Spieler " + Convert.ToString(AktuellerSpieler + 1) + " (" + Spieler[AktuellerSpieler].Name + ")", true);
                        SystemMessage2("Spiele gewonnen: " + Convert.ToString(frm1.SpieleGewonnen[i]), true);
                        SystemMessage2("Figuren besiegt: " + Convert.ToString(frm1.FigurenBesiegt[i]), true);
                        SystemMessage2("Figuren verloren: " + Convert.ToString(frm1.FigurenVerloren[i]), true);
                        SystemMessage2("Gesicherte Figuren: " + Convert.ToString(frm1.InSave[i]), true);
                        SystemMessage2("Fehler: " + Convert.ToString(frm1.Fehler[i]), true);
                        SystemMessage2(" ", false);
                    }
                    SystemMessage(" ", false);
                    SystemMessage2("---Würfel---", false);
                    for (int i = 0; i <6; i++)
                    {
                        if (Spieler[i] == null) continue;
                        AktuellerSpieler = i;
                        SystemMessage2("Spieler " + Convert.ToString(AktuellerSpieler + 1) + " (" + Spieler[AktuellerSpieler].Name + ")", true);
                        for (int b = 0; b < 6; b++)
                        {
                            SystemMessage2(Convert.ToString(b + 1) + ": " + Geworfen[i, b], true);
                        }
                    }

                    int GesamtSpiele = 0;
                    for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; GesamtSpiele += frm1.SpieleGewonnen[i]; }
                    SystemMessage(" ", false);
                    SystemMessage2("---Statistik---", false);
                    String text = ""; for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; text = text + Make10(Spieler[i].Name, 15); SystemMessage2("Name: " + text, false); }
                    SystemMessage(" ", false);
                    SystemMessage2("pro Spiel:", false);
                    // text = ""; for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; text = text + Make10(Convert.ToString((double)frm1.InSave[i] / GesamtSpiele), 13) + "  "; SystemMessage2("Save: " + text, false); }
                    // text = ""; for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; text = text + Make10(Convert.ToString((double)frm1.FigurenVerloren[i] / GesamtSpiele), 13) + "  "; SystemMessage2("Verl: " + text, false); }
                    // text = ""; for (int i = 0; i < 6; i++) { if (Spieler[i] == null) continue; text = text + Make10(Convert.ToString((double)frm1.FigurenBesiegt[i] / GesamtSpiele), 13) + "  "; SystemMessage2("Besi: " + text, false); }
                    frm1.timer1.Enabled = false;

                }
                return;
            }
            bool check = false;
            for (int i = 0; i < 6; i++) if (Spieler[i] != null) { check = true; break; }
            if (check)
            {
                int old = AktuellerSpieler;
                for (; AktuellerSpieler == old || Spieler[AktuellerSpieler] == null || !Spieler[AktuellerSpieler].Aktiv; ) { AktuellerSpieler++; if (AktuellerSpieler >= 6) AktuellerSpieler = 0; }
                frm1.Spielzug++;
            }
            else
                AktuellerSpieler = 0;

            fehler = false;
            //if (!frm1.checkBox5.Checked) SystemMessage("Spieler " + Convert.ToString(Spieler[AktuellerSpieler].GetFarbe()) + " Würfelt eine " + Convert.ToString(Wurf), true);
            // Lock();
            //int temp = Wurf;
            int old2 = Spieler[AktuellerSpieler].EigenePosition;
            if (Spieler[AktuellerSpieler] != null) Spieler[AktuellerSpieler].Spielen(Sicherheitszahl);
            frm1.setLastMove(AktuellerSpieler, 0, old2, 0, Spieler[AktuellerSpieler].EigenePosition);

            //Unlock();
            for (int c = 0; c < KI_Monopoly.Daten.SystemMessage.Count; c++) SystemMessage(KI_Monopoly.Daten.SystemMessage[c], true);
            KI_Monopoly.Daten.SystemMessage.Clear();
            for (int c = 0; c < KI_Monopoly.Daten.SystemMessageF.Count; c++) SystemMessageF(KI_Monopoly.Daten.SystemMessageF[c]);
            KI_Monopoly.Daten.SystemMessageF.Clear();

            SystemMessage("Money: " + Spieler[AktuellerSpieler].Money.ToString(),true);

            AktualisiereSpielfeld();
            /*if (fehler && Spieler[AktuellerSpieler] != null)
            {
                SystemMessage("Spieler: " + Spieler[AktuellerSpieler].GetFarbe().ToString() + " (" + Spieler[AktuellerSpieler].Name + ")>>Wurf: " + (0).ToString() + ">>Antwort: " + (0).ToString(), false);
            }*/

        }

    }

}