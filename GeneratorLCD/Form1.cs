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
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace GeneratorLCD
{

  



    public partial class Form1 : Form
    {
        short[] line = new short[16];

        short H = 16;
        short W = 16;

        public Form1()
        {
            InitializeComponent();

            for (int y = 0; y < 16; y++)
            {
                for (int x = 15; x >= 0; x--)
                {
                    Button newbtn = new Button();
                    newbtn.FlatAppearance.BorderSize = 1;
                    newbtn.FlatStyle = FlatStyle.Flat;
                    newbtn.Location = new Point((15 - x) * 20 + 22, 22 + y* 20);
                    newbtn.Name = "newbtn";
                    newbtn.Size = new Size(20, 20);
                    String s;
                    s = string.Format("{0:X1}", x)+string.Format("{0:X1}", y);
                    newbtn.Tag = s;
                    newbtn.UseVisualStyleBackColor = true;
                    newbtn.BackColor = 0xFF1e2327.ToColor();
                    Controls.Add(newbtn);
                }

                Label l = new Label();
                l.Text = (y+1).ToString();
                l.Size = new Size(24, 20);
                l.Location = new Point(1, y * 20+22);
                l.ForeColor = 0xFFFFFFFF.ToColor();
                l.TextAlign = ContentAlignment.TopRight;
                Controls.Add(l);

                Label l1 = new Label();
                l1.Text = (y + 1).ToString();
                l1.Size = new Size(24, 20);
                l1.Location = new Point(y * 20 + 20, 2);
                l1.ForeColor = 0xFFFFFFFF.ToColor();
                l1.TextAlign = ContentAlignment.BottomCenter;
                Controls.Add(l1);

            }

            numericUpDownH.Value = 8;
            numericUpDownW.Value = 16;

            foreach (var c in Controls)
            {
                if (c is Button)
                {
                    ((Button)c).Click += (sender, e) =>
                    {
                        Button b = (Button)sender;
                        if (b.Tag == null) return;       
                        String s = b.Tag.ToString();
                        short intX = short.Parse(s[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                        short intY = short.Parse(s[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                        line[intY] = (short)(line[intY] ^ ((short) (1 << intX)));// инверсия бита                                                            
                        if ((line[intY] & ((short)(1 << intX))) != 0)
                            b.BackColor = 0xFF23c1ff.ToColor();     
                        else
                            b.BackColor = 0xFF1e2327.ToColor();
                    };
                }
            }

        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            String s = "";
            s += string.Format("0x{0:X4}", (short)(numericUpDownH.Value)) + ", //H=" + numericUpDownH.Value.ToString() + Environment.NewLine;
            s += string.Format("0x{0:X4}", (short)(numericUpDownW.Value)) + ", //W=" + numericUpDownW.Value.ToString() + Environment.NewLine;

            for (int i = 0;  i < H; i++)
            {
                s += string.Format("0x{0:X4}", line[i]) + ", ";
            }
            textBox1.Text = s;
        }

        private void save_Click(object sender, EventArgs e)
        {
            String s = textBox1.Text;
            saveFileDialog1.Filter = "LCD Icon|*.icon";
            saveFileDialog1.Title = "Save an LCD Icon File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, s);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            String s = "";
            openFileDialog1.Filter = "LCD Icon|*.icon";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    s = sr.ReadToEnd();
                    textBox1.Text = s;

                    convertStrintToLine(s);
                    lineToButton();
            }
        }

        private void convertStrintToLine(String s)
        {
            String[] strline = new String[18];
            Regex regex = new Regex(@"0x*(\w*)");
            MatchCollection matches = regex.Matches(s);
            if (matches.Count == 18)
            {
                for (int i = 0; i < 18; i++) {
                    strline[i] = matches[i].Value;
                    strline[i] = strline[i].Substring(2);
                }
                H = short.Parse(strline[0], System.Globalization.NumberStyles.HexNumber); 
                W = short.Parse(strline[1], System.Globalization.NumberStyles.HexNumber);

                numericUpDownH.Value = H;
                numericUpDownW.Value = W;

                for (int i = 0; i < 16; i++)
                    line[i] = short.Parse(strline[2+i], System.Globalization.NumberStyles.HexNumber);
            }
        }

        private void lineToButton()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    if (c.Tag != null)
                    {
                        String str = c.Tag.ToString();
                        short intX = short.Parse(str[0].ToString(), System.Globalization.NumberStyles.HexNumber);
                        short intY = short.Parse(str[1].ToString(), System.Globalization.NumberStyles.HexNumber);

                        if ((line[intY] & ((short)(1 << intX))) != 0)
                            c.BackColor = 0xFF23c1ff.ToColor();
                        else
                            c.BackColor = 0xFF1e2327.ToColor();
                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }

    static class ExtensionMethods
    {
        public static Color ToColor(this uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }
    }
}
