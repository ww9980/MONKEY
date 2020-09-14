using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace csMONKEY
{
    public partial class FormMain : Form
    {

        private List<Tuple<string, List<Mat>>> CombiList = new List<Tuple<string, List<Mat>>>();

        public FormMain()
        {
            InitializeComponent();
            LoadMatXML();
            var newX = new Axis { Title = "Wavelength (nm)" };
            cartesianChart1.AxisX.Add(newX);

        }
        public void LoadMatXML()
        {
            List<Mat> MatList = new List<Mat>();
            XmlReader xmlReader = XmlReader.Create( Environment.CurrentDirectory + "\\OptPropForED.xml");
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "ROW"))
                {
                    if (xmlReader.HasAttributes)
                    //Console.WriteLine(xmlReader.GetAttribute("currency") + ": " + xmlReader.GetAttribute("rate"));
                    {
                        var RowMat = new Mat();
                        RowMat.MatName = xmlReader.GetAttribute("MaterialName");
                        RowMat.MatSymbol = xmlReader.GetAttribute("Symbol");
                        RowMat.Wavelength = Convert.ToDouble(xmlReader.GetAttribute("Wavelength"));
                        RowMat.Ri = new MathNet.Numerics.Complex32((float)Convert.ToDouble(xmlReader.GetAttribute("RI")), (float)Convert.ToDouble(xmlReader.GetAttribute("ExtCoeff")));
                        MatList.Add(RowMat);
                    }
                }   
            }

            string templast = "";
            var lastList = new List<Mat>();
            foreach (var item in MatList)
            {
                if (item.MatName == templast)
                {
                    lastList.Add(item);
                }
                else
                {
                    CombiList.Add(new Tuple<string, List<Mat>>(templast, lastList));
                    templast = item.MatName;
                    lastList = new List<Mat>();
                }
            }
            CombiList.RemoveAt(0);

            StatusText.Text = "Loaded " + CombiList.Count() + " material items."; 
            
            listBox1.DataSource = CombiList;
            listBox1.DisplayMember = "Item1";
            listBox1.ValueMember = "Item1";
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBox1.Items.Count == 0)
                return;

            int index = listBox1.IndexFromPoint(e.X, e.Y);
            string s = listBox1.Items[index].ToString();
            DragDropEffects dde1 = DoDragDrop(s, DragDropEffects.All);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                return;

            foreach (var item in CombiList)
            {
                if (listBox1.SelectedValue.ToString() == item.Item1)
                {
                    var RIin = new ChartValues<ObservablePoint>();
                    var ECin = new ChartValues<ObservablePoint>();
                    var WLx = new Axis();
                    foreach (var wlitem in item.Item2)
                    {
                        RIin.Add(new ObservablePoint(wlitem.Wavelength, wlitem.Ri.Real));
                        ECin.Add(new ObservablePoint(wlitem.Wavelength, wlitem.Ri.Imaginary));
                    }

                    var SR = new LineSeries { Title = item.Item1 + "Real", Values = RIin, PointGeometry = DefaultGeometries.Circle };
                    var SI = new LineSeries { Title = item.Item1 + "Imag", Values = ECin, PointGeometry = DefaultGeometries.Cross };

                    plotSelected(SR, SI);
                    StatusText.Text = item.Item1;
                    break;
                }
            }
            
        }

        private void plotSelected(LineSeries RIin, LineSeries ECin)
        {
            cartesianChart1.Series = new LiveCharts.SeriesCollection
            {
                RIin, ECin
            };
        }
    }
}
