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

        Bgr clr = new Bgr(0, 0, 255);

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
            main.Send(capture_in_main, null);
            Thread.Sleep(250);
        }

        void capture_in_main(object state)
        {
            Mat img = new Mat();
            capture.Retrieve(img);
            Image<Bgr, Byte> pic = img.ToImage<Bgr, Byte>();
            
            Rectangle[] facesDetected = cascade.DetectMultiScale(img);
            if (facesDetected.Length > 0)
            {
                string[] labels = new string[facesDetected.Length];
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    labels[i] = String.Empty;
                    if (train.IsTrained)
                    {
                        Image<Gray, Byte> g = train.PrepareImageToTrain(pic, facesDetected[i]);
                        labels[i] = train.Recognise(g);
                    }
                }
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    pic.Draw(facesDetected[i], clr, 2);
                    pic.Draw(labels[i], facesDetected[i].Location, Emgu.CV.CvEnum.FontFace.HersheyPlain, 4, clr);
                }
            }
            pictureBox1.Image = pic.ToBitmap();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture.Stop();
        }
    }
}
