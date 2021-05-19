using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szerver
{
     class Aruk
    {
        private string akie;
        public string Akie
        {
            get { return akie; }
            set { akie = value; }
        }

        private string nev;
        public string Nev
        {
            get { return nev; }
            set { nev = value; }
        }

        private int ar;
        public int Ar
        {
            get { return ar; }
            set { ar = value; }
        }

        private string felhasznalo=null;

        public string Felhasznalo
        {
            get { return felhasznalo; }
            set { felhasznalo = value; }
        }

        public Aruk(string akie,int ar, string nev)
        {
            this.akie = akie;
            this.ar = ar;
            this.nev = nev;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}", akie, nev, ar, (felhasznalo == null) ? "Erre a termékre még nem érkezett licit" : felhasznalo);
        }

    }
}
