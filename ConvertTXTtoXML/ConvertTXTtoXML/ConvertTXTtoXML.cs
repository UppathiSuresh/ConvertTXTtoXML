using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SystemUtility;

namespace ConvertTXTtoXML
{
    [TestClass]
    public class ConvertTXTtoXML
    {
        public static XmlDocument ResultXML;
        public static List<Tuple<string>> Error = new List<Tuple<string>>();
        public static string CurrentSegment = "";

        public static string sMsgType, sFile, xmlOutputPath,
                             //C:\Users\thandapani.a\source\repos\QA_Web3\MyMSCProject\ConversionTxt_to_XML_filepath\EDIFilePath\IFTSAI.txt
                             Globalfilepath = @"C:\Program Files\Bootloader\repos\ConvertTXTtoXML\EDIFile\IFTSTI.txt", 
                             GlobalEDISpecFilePath = @"C:\Program Files\Bootloader\repos\ConvertTXTtoXML\EDISpecPath\IFTSAI_00B.xml";
        //C:\Users\thandapani.a\source\repos\QA_Web3\MyMSCProject\ConversionTxt_to_XML_filepath\
        //Notes : EDI Template Path and EDI Specification Path both are same folder --GlobalEDISpecFilePath --xml file format
        //        EDI FILE Path --- txt or EDI file format only allowed --Globalfilepath --txt file format

        [TestMethod]
        public void XML()
        {
            ConvertTXTtoXMLMethod(GlobalEDISpecFilePath, Globalfilepath);
        }
        public static string ConvertTXTtoXMLMethod(string EDISpecFilePath, string filepath)
        {
            string output = "";
            string path = filepath.Substring(0, filepath.LastIndexOf("\\") + 1);
            Globalfilepath = filepath;

            sMsgType = EDISpecFilePath;
            sFile = filepath;

            TextParser pr = new TextParser();
            string file = System.IO.File.ReadAllText((sFile));
            file = file.Replace("?'", "");
            pr.ProcessFile(file, "'", sMsgType);
            ResultXML = pr.resultXML;

            string sSpecFilesPath = filepath + "\\";
            string Currentdate = string.Format(DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace("PM", "").Replace("AM", "").Replace(":", ""));
            string Startpoint = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string Endpoint = Startpoint.Substring(Startpoint.LastIndexOf("."));
            string Output = Startpoint.Substring(0, (Startpoint.Length - Endpoint.Length));

            //Adding Validation Part ---- Start
            String[] DataFileSplit = File.ReadAllLines(filepath);
            string[] DataFile = DataFileSplit[0].Split('\'').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            string lastsegment = DataFile.Last().Substring(0, DataFile.Last().IndexOf("+"));
            try
            {
                if (!pr.resultXML.InnerXml.Contains(lastsegment))
                {
                    Error.Add(new Tuple<string>("<ERROR01> File is Generated Partially. Error Identified in "+ TextParser.SegmentLine+ "th Line("+ CurrentSegment + ") </ERROR01>"));

                    if(!string.IsNullOrEmpty(SpecificationParser.PriorSegment))
                    {
                        Error.Add(new Tuple<string>("<ERROR01> In EDI SPECIFICATION, '"+ SpecificationParser.PriorSegment + "' Segment Defined as Mandatory, but It is missing in EDI File. </ERROR01>"));

                        if(!string.IsNullOrEmpty(SpecificationParser.CurrGroup))
                        {
                            Error.Add(new Tuple<string>("<ERROR01> Error Identified in '" + SpecificationParser.CurrGroup + "'  '"+ SpecificationParser.PriorSegment + "' is Mandatory </ERROR01>"));
                        }

                    }

                }
            }
            catch(NullReferenceException)
            {
                throw new System.InvalidOperationException("<ERROR01> File is Not Generated due to this Row : "+ CurrentSegment + " </ERROR01>");
            }


            if (Error.Count()>0)
            {
                string ErrorConsolidate = "";
                foreach (var b in Error)
                {
                    if (string.IsNullOrEmpty(ErrorConsolidate))
                    {
                        ErrorConsolidate = b.Item1;
                    }
                    else
                    {
                        ErrorConsolidate = ErrorConsolidate + b.Item1;
                    }
                }

                string VariableStore="";

                VariableStore = "<ERROR>" + ErrorConsolidate + pr.resultXML.InnerXml + "</ERROR>";

                xmlOutputPath = path + "ERROR_" + Output + "_" + Currentdate + ".xml";

                XmlDocument ResultWithErrorXML = new XmlDocument();
                ResultWithErrorXML.LoadXml(VariableStore);
                ResultWithErrorXML.Save(xmlOutputPath);

                throw new System.InvalidOperationException(" $$$$$$$$$ File is Generated with Error in "+ xmlOutputPath + " $$$$$$$$ ");

            }
            else
            {
                //Adding Validation Part ---- End

                xmlOutputPath = path + Output + "_" + Currentdate + ".xml";
                if (File.Exists(Path.GetFileName(xmlOutputPath)))
                    File.Delete(Path.GetFileName(xmlOutputPath));
                ResultXML.Save(xmlOutputPath);
                output = xmlOutputPath;
                //TestContext.AddResultFile(xmlOutputPath);
            }

            return output;

        }
    }
   
}
