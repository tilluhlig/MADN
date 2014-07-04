using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication5
{
    public class Spieler3 : KI, IKI
    {
        override public void OnFailure()
        {

        }

        public Spieler3()
        {
            FehlerAktiv = true;
        }

        override public int Aufruf(int Wuerfel)
        {
            // Quellcode
            /* int GetEigeneFigurenAnzahl();
             * Gibt die Anzahl der eigenen Figuren auf dem Feld aus, auch Figuren im Save-Bereich
             */

            /* int GetEigeneFrei();
             * Gibt aus, wieviele Figuren noch im Haus sind, sich also noch nicht auf dem Spielfeld befinden
             */

            /* int GetEigenePosition(int ID);
             * Gibt die Position einer eigenen Figur ID, auf dem Spielfeld zurück
             */

            /* int GetEigeneSave();
             * Gibt an, wieviele der Eigenen Figuren sich bereits im Save-Bereich befinden
             */

            /* int GetFarbe();
             * Gibt die Eigene Spieler-ID bzw. die Spielfarbe zurück
             */

            /* Bool BewegungMoeglich(int ID);
             * Gibt zurück, ob sich eine eigene Figur ID bewegen kann. ACHTUNG: beinhaltet auch das setzten aus dem Haus aufs Feld
             */

            /* Bool BewegungEinerMoeglich();
             * Gibt zurück, ob sich eine der eigenen Figuren bewegen kann
             */

            /* Bool GetOnField(int ID);
             * Gibt zurück, ob sich eine eigene Figur ID auf dem Feld befindet
             */

            /* int Spielfeld[44]
             * NICHT SCHREIBEN - stellt das eigene Spielfeld dar, von 0 - eigener Start, bis 40,41,42,43... Save-Bereiche
             */

            /* int EigenePosition[4]
             * NICHT BENUTZEN - wird intern verarbeitet, zugriff über int GetEigenePosition(int ID)
             */

            /* int Aufruf(int Wuerfel)
             * NICH BENUTZEN - ist der Aufruf der KI
             */

            /* EntferneFigur(int ID)
             * NICHT BENUTZEN - wird zum entfernen einer Figur vom Spielfeld genutzt
             */

            /* SetFarbe(int ID)
             * NICHT BENUTZEN - wird intern zur festlegung der Spielerfarbe/ID genutzt
             */


            System.Random ran = new System.Random();
            return ran.Next(0, 5);
        }

    }

}