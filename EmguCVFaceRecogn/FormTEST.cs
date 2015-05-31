using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmguCVFaceRecogn
{
    public partial class FormTEST : Form
    {
        public FormTEST()
        {
            InitializeComponent();
        }

        private void resetPos()
        {
            pictureBox2.Left = pictureBox1.Left + pictureBox1.Width + 25;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage()) return;
            Bitmap m = (Bitmap) Clipboard.GetImage();
            Image<Gray, Byte> img = new Image<Gray, Byte>(m);
            pictureBox1.Image = m;
            img._EqualizeHist();
            pictureBox2.Image = (Bitmap)img.Bitmap;
            resetPos();
        }
    }
}
