using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModProg_Opdr3_Reversi
{
    public partial class Scherm : Form
    {
        // Methode om bord te tekenen, en help steentjes tonen als helpknopje aanstaat
        public void drawGameboard(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //var die bijhoudt voor geval 0 valid moves in help functie
            int VM = 0;
            
            for (int x = 0; x < breedte; x++)
            {
                for (int y = 0; y < hoogte; y++)
                {
                    // Tekent tegels
                    e.Graphics.DrawRectangle(Pens.Black, x * d, y * d, d, d);

                    // Tekent steentjes
                    if (vakjes[x, y] == 1)
                    {
                        e.Graphics.FillEllipse(Brushes.Red, x * d, y * d, d, d);
                    }
                    if (vakjes[x, y] == 2)
                    {
                        e.Graphics.FillEllipse(Brushes.Blue, x * d, y * d, d, d);
                    }
                    // Tekent hulpcirkels als er op help is geklikt
                    if (help == true)
                    {
                        CheckOmringende(x, y);
                        if (valid == true)
                        {
                            e.Graphics.DrawEllipse(Pens.Black, x * d, y * d, d, d);
                        }
                        else
                        {
                            VM++;
                        }
                    }
                    if (VM == breedte * hoogte) //er zijn geen enkele valid moves
                    {
                        Beurtwissel();
                        beurtTekst.Text = beurtString();
                        nietmogelijk.Text = "Er waren geen mogelijk zetten, dus wissel van beurt";
                        help = false;
                        exitgame = exitgame + 1;
                    }
                    if (exitgame == 2) //als beide spelers niet meer kunnen: eindigt het spel
                    {
                        nietmogelijk.Text = "Er waren geen mogelijk zetten, dus het spel eindigt";
                        Endgame();
                    }
                }
            }
        }

        // Methode voor als er geklikt wordt op het scherm
        public void gameboard_Click(object sender, MouseEventArgs mea)
        {
            //functie uitzetten indien die aan staat
            help = false;

            //locatie van muis krijgen
            int loc_x = mea.X;
            int loc_y = mea.Y;
            //ints worden naar beneden afgerond. Zo weet het programma op welk vakje de speler heeft klikt.
            array_x = loc_x / d;
            array_y = loc_y / d;

            //check of de plek een valid move is
            CheckOmringende(array_x, array_y);

            if (valid == true)
            {
                //Kleurt het vakje met de steen van degene die aan de beurt is
                vakjes[array_x, array_y] = beurt;
                //Flippen
                Flippen(array_x, array_y);
                //Updaten
                gameboard.Invalidate();
                //De andere speler is nu aan de beurt
                Beurtwissel();

                //Speler heeft gezet, dus exitgame wordt gereset (pas bij 2x achter elkaar exit hij)
                exitgame = 0;

                //Tekst van beurt updaten
                beurtTekst.Text = beurtString();
                nietmogelijk.Text = "";

            }
            else
            {
                //als er geen mogelijke zetten zijn wisselt het pas wanneer de speler op help drukt.
                nietmogelijk.Text = "Probeer andere zet of klik help"; 
            }

            //Tellen van het aantal stenen op het bord
            int[] totaal = Tellen();
            rStenen = totaal[1];
            bStenen = totaal[2];
            roodTekst.Text = rStenen.ToString();
            blauwTekst.Text = bStenen.ToString();

            if (rStenen + bStenen == breedte * hoogte)
            {
                Endgame();
            }
        }

        // Methode wanneer spel geeidigd is
        public void Endgame()
        {

            if (rStenen < bStenen)
                beurtTekst.Text = "Blauw wint";
            else if (bStenen < rStenen)
                beurtTekst.Text = "Rood wint";
            else
                beurtTekst.Text = "Remisse";

        }

        // Methode om beurten te switchen
        public void Beurtwissel()
        {
            if (beurt == 1)
            {
                beurt = 2;
            }
            else if (beurt == 2)
            {
                beurt = 1;
            }
        }

        // Methode die bijhoudt hoeveel steentjes er zijn
        public int[] Tellen()
        {
            int[] teller = { 0, 0, 0 };

            for (int x = 0; x < breedte; x++)
            {
                for (int y = 0; y < hoogte; y++)
                {
                    teller[vakjes[x, y]]++;
                }
            }
            return teller;
        }

    // Methode die steentjes flipt
        public void Flippen(int i, int j)
        {
            FlipLijn(-1, -1, i, j, 0);
            FlipLijn(0, -1, i, j, 0);
            FlipLijn(1, -1, i, j, 0);

            FlipLijn(-1, 0, i, j, 0);
            FlipLijn(1, 0, i, j, 0);

            FlipLijn(-1, 1, i, j, 0);
            FlipLijn(0, 1, i, j, 0);
            FlipLijn(1, 1, i, j, 0);
        }

    // Methode om te flippen
        public void FlipLijn(int dx, int dy, int x, int y, int aantalflips)
        {
            //Moet binnen grens liggen
            if (x + dx < 0 || x + dx > breedte - 1 || y + dy < 0 || y + dy > hoogte - 1)
            {
                return;
            }
            else
            {
                if (vakjes[x + dx, y + dy] == 0) //als leeg vakje is return
                {
                    return;
                }
                else if (vakjes[x + dx, y + dy] == beurt) //flip tussen eerste beurtkleur en tweede beurtkleur
                {
                    for (int i = 0; i < aantalflips; i++)
                    {
                        vakjes[x, y] = beurt;
                        //de vorige ook laten flippen als die er zijn
                        x = x - dx;
                        y = y - dy;
                    }
                }
                else //Houdt bij hoeveel steentjes geflipt moeten worden
                {
                    aantalflips++;
                    FlipLijn(dx, dy, x + dx, y + dy, aantalflips);
                }
            }
        }

//Hierna 3 methodes, voor legale moves:

    // Opbouw methode die legale zetten checkt
        public void CheckOmringende(int i, int j)
        {
            valid = false;


            if (vakjes[i, j] == 0)
            {
                bool linksboven = ValidMoves(-1, -1, i, j);
                bool boven = ValidMoves(0, -1, i, j);
                bool rechtsboven = ValidMoves(1, -1, i, j);

                bool links = ValidMoves(-1, 0, i, j);
                bool rechts = ValidMoves(1, 0, i, j);

                bool linksonder = ValidMoves(-1, 1, i, j);
                bool onder = ValidMoves(0, 1, i, j);
                bool rechtsonder = ValidMoves(1, 1, i, j);

                if (linksboven || boven || rechtsboven || links || rechts || linksonder || onder || rechtsonder)
                {
                    valid = true;
                }
            }
        }

    // Methode die legale zetten checkt
        bool ValidMoves(int dx, int dy, int x, int y)
        {
            //Moet binnen de grenzen liggen van het bord, anders false
            if (x + dx < 0 || x + dx > breedte - 1 || y + dy < 0 || y + dy > hoogte - 1)
            {
                return false;
            }
            //Moet tegen een steen van andere kleur aanliggen, anders false
            if (vakjes[x + dx, y + dy] == beurt || vakjes[x + dx, y + dy] == 0)
            {
                return false;
            }
            else 
            {
                //Check verder of ook de volgende na volgende binnen grens ligt of niet
                if (x + dx + dx < 0 || x + dx + dx > breedte - 1 || y + dy + dy < 0 || y + dy + dy > hoogte - 1)
                {
                    return false;
                }
                else 
                {
                    return CheckLine(dx, dy, x + dx + dx, y + dy + dy);
                }
            }
        }

    // Methode die checkt of aan het eind wel dezelfde steen ligt of niet
        bool CheckLine(int dx, int dy, int x, int y)
        {

            //Moet binnen de grenzen liggen van het bord, anders false
            if (x < 0 || x > breedte - 1 || y < 0 || y > hoogte - 1)
            {
                return false;
            }

            //Als de huidige je eigen kleur is en (daar buiten leeg of volgende check buiten de range komt dan true)
            if (vakjes[x, y] == beurt)
            {
                return true;
            }
            else if (vakjes[x, y] == 0)
            {
                return false;
            }
            else //als je nog een steentje tegenkomt van de tegenstander
            {
                return CheckLine(dx, dy, x + dx, y + dy); //recursie, doorchecken
            }
        }
    }
}

/*
                  //checkt of er uberhaubt valid moves zijn
           int[] Zijnervalidmoves = new int[breedte * hoogte]; //bijhouden of er validmoves zijn in een array
           int runner = 0;
           for (int x = 0; x < breedte; x++)
           {
               for (int y = 0; y < hoogte; y++)
               {
                   CheckOmringende(x, y);
                   if (valid == true)
                   {
                       Zijnervalidmoves[runner] = 1;
                       runner++;
                   }
                   else
                   {
                       Zijnervalidmoves[runner] = 0;
                       runner++;
                   }
               }
           }
           //bevat er een valid move???
           for (int i = 0; i < Zijnervalidmoves.Length; i++)
           {
               if (Zijnervalidmoves[i] == 1)
               {
                   break;
               }
               else 
               {
                   Beurtwissel();
                   exitgame = exitgame + 1;
                   if (exitgame == 2)
                   {
                       Endgame();
                   }
               }
           } 
   */
