﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WindowsFormsApplication5
{
    public class Spieler1_Monopoly : KI_Monopoly, IKI_Monopoly
    {
        
        override public void OnFailure()
        {

        }

        public Spieler1_Monopoly()
        {
            FehlerAktiv = true;
            Name = "Unbenannt";
        }
        
        override public void Aufruf()
        {

        }

    }

}
