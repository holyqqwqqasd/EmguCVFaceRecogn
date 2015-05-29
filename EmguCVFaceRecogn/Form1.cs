using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EmguCVFaceRecogn
{
    public partial class Form1 : Form
    {
        SynchronizationContext main;
        Capture capture;
        CascadeClassifier cascade;

        Bgr clrSuccess = new Bgr(0, 255, 0);
        Bgr clrFailure = new Bgr(0, 0, 255);

        Classifier_Train train;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            main = SynchronizationContext.Current;
            
            cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

            train = new Classifier_Train("TrainedFaces");

            capture = new Capture();
            capture.Start();
            capture.ImageGrabbed += capture_ImageGrabbed;
        }

        void capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                main.Send(capture_in_main, null);
                Thread.Sleep(250);
            }
            catch { }
        }

        void capture_in_main(object state)
        {
            Image<Bgr, Byte> img;
            if (Clipboard.ContainsImage())
            {
                img = new Image<Bgr, Byte>((Bitmap)Clipboard.GetImage());
            }
            else
            {
                img = new Image<Bgr, Byte>(capture.Width, capture.Height);
                capture.Retrieve(img);
            }
            
            Rectangle[] facesDetected = cascade.DetectMultiScale(img);
            if (facesDetected.Length > 0)
            {
                string[] labels = new string[facesDetected.Length];
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    labels[i] = String.Empty;
                    if (train.IsTrained)
                    {
                        Image<Gray, Byte> g = train.PrepareImageToTrain(img, facesDetected[i]);
                        labels[i] = train.Recognise(g);
                    }
                }
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    img.Draw(facesDetected[i], (labels[i].Length > 0) ? clrSuccess : clrFailure, 2);
                    if (labels[i].Length > 0) img.Draw(labels[i], facesDetected[i].Location, Emgu.CV.CvEnum.FontFace.HersheyPlain, 4, clrSuccess);
                }
            }
            pictureBox1.Image = img.ToBitmap();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image<Bgr, Byte> img;
            if (Clipboard.ContainsImage()) {
                img = new Image<Bgr, Byte>((Bitmap)Clipboard.GetImage());
            } else {
                img = new Image<Bgr, Byte>(capture.Width, capture.Height);
                capture.Retrieve(img);
            }

            Rectangle[] facesDetected = cascade.DetectMultiScale(img);
            if (facesDetected.Length > 0)
            {
                capture.Pause();
                FormTrain frm = new FormTrain();
                frm.SetFaces(train, facesDetected, img);
                frm.ShowDialog();
                capture.Start();
            }
        }
    }
}
