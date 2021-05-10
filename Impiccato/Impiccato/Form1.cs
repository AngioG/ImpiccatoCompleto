using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Speech.Synthesis;
using NAudio.Wave.SampleProviders;

namespace Impiccato
{
    public partial class Form1 : Form
    {


        private static IWavePlayer waveoutdevice;
        private static AudioFileReader audioFileReader;

        string parola = default;
        int tentativo = default;
        string[] lettere = new string[30];
        int nLettere = 0;
        bool start = true;
        public static int num = 0;
        public static string[] parole = new string[100000];

        public Form1()
        {
            InitializeComponent();
            waveoutdevice = new WaveOut();
            waveoutdevice.Volume = 0.25F;
        }
        /*
        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader miofile;
            miofile = new StreamReader("parole.txt");
            int x = 0;

            while (miofile.EndOfStream == false)
            {
                parole[x] = miofile.ReadLine();

                x += 1;
            }

            num = x;

            miofile.Close();
        }
        */
        private void txt_ins_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)13)
                    btn_try.PerformClick();
                return;
            }

            if (Char.IsLetter(e.KeyChar))
            {
                e.KeyChar = Char.ToLower(e.KeyChar);
                return;
            }

            e.Handled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            lbl_error.Text = "";

            if (start)
            {
                Random random = new Random();
                int a = random.Next(1, num);
                parola = parole[a];

                txt_ins.Enabled = true;
                for (int i = 0; i <= parola.Length; i++)
                    txt_parola.Text += "_ ";

                txt_ins.Text = "";
                tentativo = -1;
                lbl_info.Visible = true;
                btn_try.Text = "Prova a indovinare";
                start = false;

                nLettere = 0;
            }

            else
            {
                bool inserito = false;

                for (int i = 0; i < nLettere; i++)
                    if (txt_ins.Text == lettere[i])
                    {
                        inserito = true;
                        break;
                    }

                if (inserito || txt_ins.Text.Length == 0)
                {
                    Canzone("nope.mp3");

                    if (txt_ins.Text.Length == 0)
                        lbl_error.Text = "Non hai inserito nulla";
                    else if (txt_ins.Text.Length == 1)
                        lbl_error.Text = "Hai già inserito questa lettera";
                    else
                        lbl_error.Text = "Hai già provato questa parola";
                    txt_ins.Text = "";

                    return;
                }

                lettere[nLettere] = txt_ins.Text;
                nLettere += 1;

                if (txt_ins.Text.Length == 1)
                {
                    string finale = default;

                    if (parola.Contains(txt_ins.Text))
                    {
                        txt_parola.Text = "";
                        int x = 0;

                        while (x < parola.Length)   //Sistema parola
                        {
                            bool found = false;
                            int y = 0;

                            while (y < nLettere)
                            {
                                if (parola.Substring(x, 1) == lettere[y])
                                {
                                    found = true;
                                    break;
                                }
                                y++;
                            }

                            if (found)
                            {
                                txt_parola.Text += lettere[y] + " ";
                                finale += lettere[y];
                            }

                            if (!found)
                                txt_parola.Text += "_ ";

                            x++;
                        }

                        if (parola == finale)
                        {
                            Canzone("win.mp3");

                            txt_ins.Text = "";
                            txt_parola.Text = "";
                            lbl_error.Text = "Bravo! Hai indovinato!";
                            start = true;
                            btn_try.Text = "Gioca di nuovo";
                            txt_ins.Enabled = false;
                            lbl_info.Visible = false;
                        }

                        else
                        {
                            txt_ins.Text = "";
                            Canzone("let_guess.mp3");
                        }
                        return;

                    }

                }

                else if (parola != txt_ins.Text)
                    lbl_error.Text = $"La parola non è {txt_ins.Text}";

                else if (parola == txt_ins.Text)
                {
                    Canzone("win.mp3");


                    txt_ins.Text = "";
                    txt_parola.Text = "";
                    lbl_error.Text = "Bravo! Hai indovinato!";
                    start = true;
                    btn_try.Text = "Gioca di nuovo";
                    txt_ins.Enabled = false;
                    lbl_info.Visible = false;

                    return;
                }
            }

            tentativo += 1;

            if (tentativo >= 0 && tentativo <= 9)
                pictureBox1.Load($"{tentativo}.png");

            //Hai perso
            if (tentativo == 9)
            {
                Canzone("lose.mp3");
                lbl_error.Text = $"Hai perso, la parola era {parola}";
                start = true;
                btn_try.Text = "Gioca di nuovo";
                txt_ins.ReadOnly = true;
                lbl_info.Visible = false;
            }

            else if (tentativo != 0)
            {
                Canzone("let_wrong.mp3");
                lbl_error.Text = "La parola non contiene questa lettera";
            }

            txt_ins.Text = "";
        }

        private void Canzone(string titolo)
        {
            if (waveoutdevice.PlaybackState == PlaybackState.Playing)
                waveoutdevice.Stop();

            audioFileReader = new AudioFileReader(titolo);
            waveoutdevice.Init(audioFileReader);

            Task.Run(() => waveoutdevice.Play());
        }


    }
}
