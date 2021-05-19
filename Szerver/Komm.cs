using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


//Először login szolgáltatás írása -> be kell kérnie két paramétert logged igazra és felhasználónevet beállítani
//termékek szolgáltatás ->termékek metódusok : feladata a szerveren lévő összes terméket kiírni
//Addoláshoz fontos a lista lockolása
// Add bekér 2 paramétert ár, mit  -> a felhasználó tudnunk kell mert be vagyunk jelentkezve
//licit -> kinek a termékére, mire és mennyivel licitálunk
//bye metódus -> elköszönés

namespace Szerver
{
    class Komm
    {
        bool logged = false;
        string felhasznalo = string.Empty;
        string jelszo = string.Empty;
        int aruszam = 0;
        TcpClient bejovo = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        static List<Aruk> termekek = new List<Aruk> { new Aruk("Márk", 2111, "Dezodor") };

        public Komm(TcpClient kliens)
        {
            bejovo = kliens;
            sr = new StreamReader(bejovo.GetStream(), Encoding.UTF8);
            sw = new StreamWriter(bejovo.GetStream(), Encoding.UTF8);
        }


        public void Komm_Indit()
        {
            Console.WriteLine("Felcsatlakozott egy kliens!");
            bool end = false;
            string keres = string.Empty;
            sw.WriteLine("Üdv a szerveren!");
            sw.Flush();
            string[] keresekparameterei;
            while (!end)
            {
                try
                {
                    keres = sr.ReadLine();
                    keresekparameterei = keres.Split('|');
                    switch (keresekparameterei[0])
                    {
                        case "INFO":
                            sw.WriteLine("OK*");
                            sw.WriteLine("LOGIN|felhasznalonev|jelszo - bejelentkezés");
                            sw.WriteLine("LIST - termékek kilistázása");
                            sw.WriteLine("ADD|mit|ára - Termékhozzás adás (mit,ár)");
                            sw.WriteLine("LICIT - licitálás (kinek, terméknév, licit)");

                            sw.WriteLine("OK!");


                            break;

                        case "LOGIN":
                            Login(sw, keresekparameterei[1], keresekparameterei[2]);
                            break;
                        case "ADD":
                            Add(sw, felhasznalo, keresekparameterei[1], int.Parse(keresekparameterei[2]));
                            break;
                        case "LIST":
                            List(sw);
                            break;
                        case "LICIT":
                            Licit(sw, keresekparameterei[1], keresekparameterei[2], int.Parse(keresekparameterei[3]));

                            break;

                        case "BYE":
                            Bye(sw);
                            break;


                        default:
                            sw.WriteLine("OK");
                            sw.WriteLine("Nincs ilyen szolgáltatás!");
                            break;

                    }
                    sw.Flush();
                }
                catch (Exception e)
                {
                    if (bejovo.Connected)
                    {
                        sw.WriteLine("ERR|{0}", e.Message);
                        sw.Flush();
                    }
                    else
                    {
                        end = true;
                    }
                }
            }


        }
        public void Login(StreamWriter sw, string nev, string jelszo)
        {
            if (logged)
            {
                sw.WriteLine("OK");
                sw.WriteLine("Már be vagy jelentkezve: {0} néven!", felhasznalo);

            }

            else
            {
                logged = true;
                felhasznalo = nev;
                this.jelszo = jelszo;
                sw.WriteLine("OK*");
                sw.WriteLine("Sikeres bejelentkezés!");
                sw.WriteLine("A felhasználóneved: {0}", nev);
                sw.WriteLine("OK!");
            }

        }

        public void Add(StreamWriter sw, string felhasznalo, string termeknev, int ar)
        {
            if (!logged)
            {
                sw.WriteLine("OK");
                sw.WriteLine("A szolgáltatás használatához jelentkezz be!");

            }

            else
            {
                bool letezik = false;
                for (int i = 0; i < termekek.Count(); i++)
                {
                    if (termekek[i].Akie == felhasznalo && termekek[i].Nev == termeknev)
                    {
                        letezik = true;
                    }

                }

                if (letezik)
                {
                    sw.WriteLine("OK");
                    sw.WriteLine("A felhasználóhoz már tartozik {1} termék!", felhasznalo, termeknev);
                }
                else
                {
                    lock (termekek)
                    {
                        termekek.Add(new Aruk(felhasznalo, ar, termeknev));
                    }
                    sw.WriteLine("OK");
                    sw.WriteLine("A {0} termék sikeresen hozzáadva!", termeknev);
                }

            }
        }

        public void List(StreamWriter sw)
        {



            if (termekek.Count == 0)
            {
                sw.WriteLine("OK*");
                sw.WriteLine("A termékek listája üres!");
                sw.WriteLine(" Hozzáadhat új elemet az ADD|paraméter|paraméter paranccsal!");
                sw.WriteLine("OK!");
            }

            else
            {
                sw.WriteLine("OK*");
                sw.WriteLine("Tulajdonos      Terméknév       Jelenlegi ára       Nyertes");
                for (int i = 0; i < termekek.Count; i++)
                {
                    sw.WriteLine("{0}          {1}            {2}                 {3}", termekek[i].Akie, termekek[i].Nev, termekek[i].Ar, termekek[i].Felhasznalo);
                }
                sw.WriteLine("OK!");
            }

        }

        public void Licit(StreamWriter sw, string tulajdonos, string termek, int licit)
        {
            if (!logged)
            {
                sw.WriteLine("OK");
                sw.WriteLine("A szolgáltatás használatához jelentkezz be!");

            }
            else
            {
                sw.WriteLine("OK*");
                if (termekek.Count == 0)
                {

                    sw.WriteLine("A termékek listája üres!");
                    sw.WriteLine(" Hozzáadhat új elemet az ADD|paraméter|paraméter paranccsal!");

                }

                else
                {
                    for (int i = 0; i < termekek.Count; i++)
                    {
                        if (termekek[i].Akie == tulajdonos && termekek[i].Nev == termek)
                        {
                            if (licit <= termekek[i].Ar)
                            {
                                sw.WriteLine("A licitnek magasabbank kell lennie, a termék áránál!");

                            }
                            else
                            {

                                sw.WriteLine("Sikeres licitálás {0} összeggel!", licit);
                                termekek[i].Felhasznalo = this.felhasznalo;
                                termekek[i].Ar = licit;
                            }
                            break;

                        }
                        if (i == termekek.Count - 1)
                        {
                            sw.WriteLine("Ilyen termék nincsen a listában!");
                            sw.WriteLine("Próbálkozzon újra!");
                        }


                    }
                }

                sw.WriteLine("OK!");
            }
        }

        public void Bye(StreamWriter sw)
        {
            sw.WriteLine("BYE");
            
        }


    }
}
