using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SystemUtility
{
     public class XMLParser
    {
        public static XMLParser xmlParser { get; } = new XMLParser();
        static XmlDocument doc = new XmlDocument();
        static XmlNode EdiNode, MsgNode, FirstSegmentNode, SplitedNode, MSG1Node;
        static string SegmentSplit, FirstSegment = "", FilePath, FileName = "Coparn";

        public void ConvertTXTtoXML(string txtFileName, string xmlFolder)
        {
            //FilePath = txtFileName; //@"C:\Program Files\Visual Studio 2015\Projects\PRACTISE\XmlCreate\" + FileName + ".txt";
            String[] DataFiles = File.ReadAllLines(txtFileName);
            string[] DataFile = DataFiles[0].Split('\'').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            int FileCount = DataFile.Count();
            if (FileCount > 0)
            {
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", null, null);
                doc.AppendChild(docNode);
                XMLCreateTag("", "EDI");
                XMLCreateTag("", "MSG");

                for (int i = 0; i < FileCount; i++)
                {
                    string datastore = DataFile[i];
                    string[] splitstring = datastore.Split('+');

                    for (int j = 0; j < splitstring.Count(); j++)
                    {
                        int cnt = j + 1;
                        if (j == 0)
                        {
                            FirstSegment = splitstring[j];

                            XMLCreateTag(FirstSegment, "CreateNode");
                        }
                        if (cnt <= 9)
                        {
                            SegmentSplit = string.Copy(FirstSegment) + "0" + cnt;
                        }
                        else if (cnt > 9)
                        {
                            SegmentSplit = string.Copy(FirstSegment) + cnt;
                        }

                        string SplitedTextSegment = splitstring[j];

                        XMLCreateTag(SegmentSplit, "SplitedNode");
                        XMLCreateTag(SplitedTextSegment, "TextNode");
                    }

                    XMLCreateTag("", "append-CreateNode");

                }

                XMLCreateTag("", "append-MSG");
                XMLCreateTag("", "append-EDI");

                doc.Save(xmlFolder+@"\"+ FileName +".XML");
            }

        }

        public static void XMLCreateTag(string Segment, string Identify)
        {

            if (Identify.Equals("EDI"))
            {
                EdiNode = doc.CreateElement("EDI");
            }

            if (Identify.Equals("MSG"))
            {
                MsgNode = doc.CreateElement("MSG");
            }

            if (Identify.Equals("CreateNode"))
            {

                FirstSegmentNode = doc.CreateElement(Segment);
            }


            if (Identify.Equals("SplitedNode"))
            {
                SplitedNode = doc.CreateElement(Segment);
            }

            if (Identify.Equals("TextNode"))
            {
                SplitedNode.AppendChild(doc.CreateTextNode(Segment));
                FirstSegmentNode.AppendChild(SplitedNode);
            }


            if (Identify.Equals("append-CreateNode"))
            {
                MsgNode.AppendChild(FirstSegmentNode);
            }

            if (Identify.Equals("append-MSG"))
            {
                EdiNode.AppendChild(MsgNode);
            }

            if (Identify.Equals("append-EDI"))
            {
                doc.AppendChild(EdiNode);
            }

        }
    }
}
