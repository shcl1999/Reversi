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
    //Variabelen declareren die nodig zijn in de class:
        int breedte, hoogte;
        int[,] vakjes;
        int d = 50; // diameter
        int beurt;
        bool valid;
        bool help;
        int array_x;
        int array_y;
        int exitgame = 0; //als beide spelers geen zetten meer kan zetten
        Panel gameboard = new Panel();
        Label beurtTekst = new Label();
        Label roodTekst = new Label();
        Label blauwTekst = new Label();
        Label grootte = new Label();
        Label grootteX = new Label();
        Label grootteY = new Label();
        Label nietmogelijk = new Label();
        Label borderror = new Label();
        TextBox invoerBreedte = new TextBox();
        TextBox invoerHoogte = new TextBox();
        Button bordgrootte = new Button();
        int rStenen;
        int bStenen;

        public Scherm()
        {
        // Layout panel
            breedte = 6;
            hoogte = 6;
            vakjes = new int[breedte, hoogte];
            beurt = 1;
            rStenen = 2;
            bStenen = 2;

        // Layout window
            Text = "Reversi";
            BackColor = Color.White;
            Size = new Size(550, 700);

        // Button nieuw
            Button nieuw = new Button();
            nieuw.Text = "Nieuw spel";
            nieuw.Location = new Point(115, 20);

        // Button help
            Button help = new Button();
            help.Text = "Help";
            help.Location = new Point(200, 20);

        // Label met tekst: niet mogelijke zet
            nietmogelijk.Location = new Point(115, 190);
            nietmogelijk.Text = "";
            nietmogelijk.AutoSize = true;

        // Label bord error
            borderror.Location = new Point(300, 190);
            borderror.Text = ""; //nu nog niks, geen error momenteel
            borderror.AutoSize = true;

        // Layout gameboard
            gameboard.Location = new Point(115, 220);
            gameboard.Size = new Size(breedte * d + 1, hoogte * d + 1);
            gameboard.BackColor = Color.Gainsboro;

        // Label beurt
            beurtTekst.Location = new Point(115, 170);
            beurtTekst.Size = new Size(100, 20);
            //beurtTekst.Text = beurtString(); *Dit komt beneden*

        // Labels grootte van het speelbord
            grootte.Location = new Point(300, 75);
            grootte.Text = "Bordgrootte (3x3 t/m 8x8)";
            grootte.AutoSize = true;

            grootteX.Location = new Point(300, 100);
            grootteX.Text = "Breedte:";
            grootteX.AutoSize = true;

            grootteY.Location = new Point(300, 125);
            grootteY.Text = "Hoogte:";
            grootteY.AutoSize = true;

        // Invoer grootte van speelbord
            invoerBreedte.Location = new Point(355, 100);
            invoerBreedte.Size = new Size(40, 15);

            invoerHoogte.Location = new Point(355, 125);
            invoerHoogte.Size = new Size(40, 15);

            bordgrootte.Location = new Point(300, 160);
            bordgrootte.Text = "Pas bord aan";
            bordgrootte.AutoSize = true;

            roodTekst.Location = new Point(160, 85);
            roodTekst.Size = new Size(50, 30);
            roodTekst.Text = rStenen.ToString();

            blauwTekst.Location = new Point(160, 127);
            blauwTekst.Size = new Size(100, 30);
            blauwTekst.Text = bStenen.ToString();

        // Alles toevoegen
            this.Controls.Add(gameboard);
            this.Controls.Add(beurtTekst);
            this.Controls.Add(roodTekst);
            this.Controls.Add(blauwTekst);
            this.Controls.Add(nieuw);
            this.Controls.Add(nietmogelijk);
            this.Controls.Add(help);
            this.Controls.Add(bordgrootte);
            this.Controls.Add(grootte);
            this.Controls.Add(grootteX);
            this.Controls.Add(grootteY);
            this.Controls.Add(invoerBreedte);
            this.Controls.Add(invoerHoogte);
            this.Controls.Add(borderror);

        // Events:
            //Deze methodes staan hieronder, het zijn kleine methodes
            beurtTekst.Text = beurtString();
            nieuw.Click += nieuw_Click;
            help.Click += Help_Click;
            bordgrootte.MouseClick += StelBordBij;
            this.Paint += Steentjes;
            beginstand();
            //Deze methodes (met andere functies zolas valid moves checkes) is te zien in andere file: Events.cs 
            //(Vind ik persoonlijk prettiger)
            gameboard.Paint += drawGameboard;
            gameboard.MouseClick += gameboard_Click;
        }


    // Methode om bordgrootte aan te passen
        public void StelBordBij(object sender, EventArgs e)
        {
            //Input
            breedte = int.Parse(invoerBreedte.Text);
            hoogte = int.Parse(invoerHoogte.Text);

            //Verwerking
            if (breedte >= 3 && breedte <= 8 && hoogte >= 3 && hoogte <= 8) //bord varieert van 3x3 t/m 8x8
            {
                borderror.Text = "";
                nietmogelijk.Text = "";
                vakjes = new int[breedte, hoogte];
                gameboard.Size = new Size(breedte * d + 1, hoogte * d + 1);
                beginstand();
                gameboard.Refresh();
            }
            else
            {
                borderror.Text = "Geen geldige invoer.";
            }
        }

    // Methode om steentjes te designen bovenaan de scherm, de teller dus
        public void Steentjes(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillEllipse(Brushes.Red, 115, 75, 30, 30);
            e.Graphics.FillEllipse(Brushes.Blue, 115, 115, 30, 30);
        }

    // Methode voor de beginstand
        public void beginstand()
        {
            //eerst alles wit maken
            for (int x = 0; x < breedte; x++)
                for (int y = 0; y < hoogte; y++)
                    vakjes[x, y] = 0;
            //midden "kleuren"
            int midx, midy;
            midx = breedte / 2 - 1;
            midy = hoogte / 2 - 1;
            //1 is rood en 2 is blauw
            vakjes[midx, midy] = 2;
            vakjes[midx + 1, midy] = 1;
            vakjes[midx, midy + 1] = 1;
            vakjes[midx + 1, midy + 1] = 2;
            //help uitzetten indien die aan staat
            help = false;
        }

    // Methode die een nieuw spel start
        private void nieuw_Click(object sender, EventArgs e)
        {
            beurt = 1;
            beurtTekst.Text = beurtString();
            rStenen = 2;
            bStenen = 2;
            roodTekst.Text = rStenen.ToString();
            blauwTekst.Text = bStenen.ToString();
            nietmogelijk.Text = "";
            beginstand();
            gameboard.Invalidate();
        }

    // Methode de aangeeft wie aan de beurt is
        private string beurtString()
        {
            if (beurt == 1)
                return "Rood is aan zet";
            else
                return "Blauw is aan zet";
        }

    // Methode om help te klikken, de mogelijkheden worden in drawGameboard verwerkt
        void Help_Click(object sender, EventArgs e)
        {
            if (help == false)
            {
                help = true;
                gameboard.Invalidate();
            }
            else
            {
                help = false;
                gameboard.Invalidate();
            }

            nietmogelijk.Text = "";
        }
    }
}
