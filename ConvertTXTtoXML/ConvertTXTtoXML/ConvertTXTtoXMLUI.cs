using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using SystemUtility;

namespace ConvertTXTtoXML
{
    public partial class ConvertTXTtoXMLUI : Form
    {
        public ConvertTXTtoXMLUI()
        {
            InitializeComponent();
            //    public static XmlDocument ResultXML;
            //public static List<Tuple<string>> Error = new List<Tuple<string>>();
            //public static string CurrentSegment = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string name=openFileDialog.FileName;
                
            }

        }
    }
}
