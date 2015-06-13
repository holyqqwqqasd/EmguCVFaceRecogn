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
    public partial class FormTrain : Form
    {
        private class ListItem
        {
            public ListItem(Rectangle face, String name)
            {
                Face = face;
                Name = name;
            }

            public Rectangle Face { set; get; }
            public String Name { set; get; }

            public override string ToString()
            {
                return Name;
            }
        }

        ListItem selectedItem;
        Image<Bgr, Byte> image;
        Classifier_Train train;

        public FormTrain()
        {
            InitializeComponent();
        }

        public void SetFaces(Classifier_Train _train, Rectangle[] _faces, Image<Bgr, Byte> _img)
        {
            train = _train;
            image = _img;
            foreach (Rectangle r in _faces)
            {
                listBox1.Items.Add(new ListItem(r, r.ToString()));
            }
            pictureBox1.Image = _img.Bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedItem == null) return;
            Image<Gray, Byte> g = train.PrepareImageToTrain(image, selectedItem.Face);
            train.AddImageForTrain(g, textBox1.Text);
            train.Retrain();
            MessageBox.Show("Complete!", "Train");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem item = (ListItem)listBox1.SelectedItem;
            if (item == null || item.Face == null) return;
            Bitmap bmp = image.ToBitmap();
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(Pens.Red, item.Face);
            pictureBox1.Image = bmp;
            selectedItem = item;
            label1.Text = selectedItem.Name;
        }
    }
}
