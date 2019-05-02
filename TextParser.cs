using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace SystemUtility
{
    public class TextParser
    {
        public XmlDocument specXML;
        public XmlDocument XML;
         protected FixedSpecification specTxt;
         protected int lineNumber;
        public static int SegmentLine = 0;
         public string newxml,tempxml;
        //public static List<Tuple<string, string>> SpecficiationStored = new List<Tuple<string, string>>();
        public static List<Tuple<string, string, int, bool, string>> SpecficiationStored = new List<Tuple<string, string, int, bool, string>>();

        //protected XmlDocument logDOM;
        public XmlDocument resultXML;

        public bool ProcessFile(string textfile, string lineDiv, string msgType)
        {
            bool result = false;
            lineNumber = 0;
            specXML = new XmlDocument();
            string tempFile = textfile.Replace(lineDiv, "\r\n");
            //string file = System.IO.File.ReadAllText(("C:\\New folder\\COPARNXML_spec.xml"));

            //specXML.LoadXml(file);
            //XmlNode specification = specXML.SelectSingleNode("/root");

            //specTxt = new FixedSpecification("", true, "", true, 0, 1, 1, "");
            bool isLoadSpec = LoadSpecification(tempFile, msgType);
           
            string stxtOutput = "";
            try
            {
                stxtOutput += parseBuffer2(textfile);
            }
            catch (ParseException pex)
            {

                result = false;
            }
                
            return result;
 
        }
        protected bool LoadSpecification(string buffer, string msgType)
        {

            string[] parseCode = { "\r\n" };
            string[] rows = buffer.Split(parseCode, StringSplitOptions.RemoveEmptyEntries);

            //string spec = rows[0];
            XmlDocument mapspec = new XmlDocument();
            XmlNodeList MapNodesList;

            string sSpecFilesPath = msgType.Substring(0, msgType.LastIndexOf("\\") + 1);
            string sfile1 = msgType.Substring(msgType.LastIndexOf("\\") + 1);
            string sSpecFilename = sfile1.Substring(0, sfile1.LastIndexOf(".") );


            //string sSpecFilesPath = @"C:\MSCLink Apps\EDI\EDIMessage\XMLSpecifications\";
            string filePath = "";
            string sSpecificationName = "";

            bool isLoad = false;
            mapspec.Load(sSpecFilesPath + @"\EDIMappingSpecification.xml");
            XmlNodeList mappingList = mapspec.SelectNodes("/root/EDImappingspecification/mapping");


            foreach (XmlNode node in mappingList)
            {

                int countNodeLine = 0;
                int partialMatch = 0;
                string nameNode = "";
                string versionNode = "";
                string fileNode = "";
                string spName = "";
                string spCallSP = "";
                MapNodesList = node.ChildNodes;
                foreach (XmlNode xn in MapNodesList)
                {

                    XmlAttributeCollection mapCollection = xn.Attributes;
                    if (mapCollection != null)
                    {
                        string number = "0";
                        string column = "0";
                        string length = "0";
                        string value = "";



                        if (xn.Name == "line")
                        {
                            countNodeLine++;
                            foreach (XmlAttribute xa in mapCollection)
                            {
                                if (xa.Name == "number")
                                {
                                    number = xa.Value;
                                }
                                else if (xa.Name == "column")
                                {
                                    column = xa.Value;
                                }
                                else if (xa.Name == "length")
                                {
                                    length = xa.Value;
                                }
                                else if (xa.Name == "value")
                                {
                                    value = xa.Value;
                                }

                            }

                            int numberN = 0;
                            int columnN = 0;
                            int lengthN = 0;

                            int.TryParse(number, out numberN);
                            int.TryParse(column, out columnN);
                            int.TryParse(length, out lengthN);

                            // Retrieve the row number from the File Array in Memory
                            if (rows.Length < numberN)
                            {
                                partialMatch = 0;
                                break;
                            }
                            string row = rows[numberN - 1].Trim();
                            string lineValue = "";
                            if (row.Length == lengthN)
                            {
                                lineValue = row.Substring(columnN - 1, lengthN);
                            }
                            else
                            {
                                lineValue = "";
                            }
                            if (value.Trim() == lineValue.Trim() && lineValue != "")
                            {
                                partialMatch++;
                            }
                        }
                        else if (xn.Name == "specification")
                        {
                            foreach (XmlAttribute xa in mapCollection)
                            {
                                if (xa.Name == "name")
                                {
                                    nameNode = xa.Value;

                                }
                                else if (xa.Name == "version")
                                {
                                    versionNode = xa.Value;
                                }
                                else if (xa.Name == "length")
                                {
                                    length = xa.Value;
                                }
                                else if (xa.Name == "file")
                                {
                                    fileNode = xa.Value;
                                }

                            }

                        }
                        else if (xn.Name == "storedprocedure")
                        {
                            foreach (XmlAttribute xa in mapCollection)
                            {
                                if (xa.Name == "name")
                                {
                                    spName = xa.Value;
                                }


                            }
                        }
                        else if (xn.Name == "callstoredprocedure")
                        {
                            foreach (XmlAttribute xa in mapCollection)
                            {
                                if (xa.Name == "name")
                                {
                                    spCallSP = xa.Value;
                                }

                            }
                        }
                    }

                    if (nameNode == sSpecFilename)
                    {
                        filePath = fileNode;
                        sSpecificationName = nameNode;
                    }

                }
            }

            if (filePath != "")
            {

                filePath = filePath = filePath.Replace("%SPECPATH%", sSpecFilesPath);//123
                string file = System.IO.File.ReadAllText((filePath));
                specXML.LoadXml(file);

                XmlNode specification = specXML.SelectSingleNode("/root");

                //foreach (XmlAttribute xa in specification.Attributes)
                //{
                //    if (xa.Name == "name")
                //    {
                //        name = xa.Value;
                //    }
                //    else if (xa.Name == "version")
                //    {
                //        version = xa.Value;
                //    }
                //    else if (xa.Name == "type")
                //    {
                //        type = xa.Value;
                //    }
                //}


                specTxt = new FixedSpecification("", true, "", true, 0, 1, 1, "",0, false, "");

                foreach (XmlNode childNode in specification.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        ProcessSpecification(childNode, specTxt);
                    }
                }

                BasicValidationBeforeProcessingStart(buffer);

                
                //Console.WriteLine(" ");
                //Console.WriteLine("-----------------------XML NODE GENERATION START--------------------------------------------------");
                //Console.WriteLine(" ");
                //Console.WriteLine(" ");

                isLoad = true;
            }

            return isLoad;
        }
        protected string parseBuffer2(string buffer)
        {
            string result = "";
            string[] parseCode = { "'" };
            string[] rows = buffer.Split(parseCode, StringSplitOptions.RemoveEmptyEntries);

            SpecificationParser sp = new SpecificationParser(specTxt);

            sp.Init();
            XML = new XmlDocument();
            //FixedSpecification currentRow = specTxt;
            FixedSpecification currentRow = null;

            foreach (string row in rows)
            {
                if(row.Contains("DGS") || row.Contains("FTX"))
                {
                    string temp="";
                }
                 
                lineNumber++;
                if (currentRow == null)
                {
                    // First Iteration
                    currentRow = specTxt;
                }
                else
                {
                    // Rest of Iterations
                    if (currentRow != null)
                    {
                        FixedSpecification parent = currentRow.parent;
                        // Now we try to identify the row with the "first" specification
                        // If the row match, we will start looping again and we will take parent.Specifications[0]
                        if (parent != null)
                        {
                            FixedSpecification firstSibling = parent.Specifications[0];

                            // If the row doesn't match, we will go to the parent node
                            FixedSpecification nextFromParent = sp.FindNextSpecification2(currentRow, row);
                            

                            // If the row belongs to a simple node (not group node nor virtual node) and the specification has maxOccurrences > 1 (Still Pending Validation)
                            FixedSpecification mySelf = currentRow;

                            // So this is the final assignment priority:
                            // 1) If row is mySelf and the Specification allows more than 1 occurrence, we keep this one, if not, we have to throw exception "maxOccurrence already reached" + row
                            // 2) If row matches with firstSibling (Group) and the MaxOccurrences of it is > 1, we keep this one, if not, we have to throw exception "maxOccurrence already reached" + row

                            if (sp.IdentifyRow(mySelf, row))
                            {
                                // If mySelf match with the row, we are going to iterate throw same prefix again
                                currentRow = mySelf;
                            }
                            else if (sp.IdentifyRow(firstSibling, row))
                            {
                                // If firstSibling match with the row, we are going to iterate throw same group again
                                currentRow = firstSibling;
                            }
                            else
                            {
                                // If firstSibling doesn't match, we will go to parent
                                currentRow = nextFromParent;
                            }
                        }
                        else
                        {
                            // ?????????????????????????????????????????????????
                            currentRow = sp.FindNextSpecification(currentRow);
                        }


                    }
                }

                if (currentRow != null)
                {
                    sp.TextOutput = "";

                    //Console.WriteLine(" ");
                    //Console.WriteLine(" ");
                    SegmentLine++;
                    //Console.WriteLine("ROW :{0} and Line :{1}" , row, SegmentLine);
                    ConvertTXTtoXML.ConvertTXTtoXML.CurrentSegment = row;
                    //Console.WriteLine("-----------------------------------------------------------------------------------------");
                    //Console.WriteLine("Before ROW current row prefix :{0} and isvirutal : {1} and virtualprefix :{2} ", currentRow.Prefix, currentRow.IsVirtual, currentRow.VirtualPrefix);
                    //Console.WriteLine("-----------------------------------------------------------------------------------------");
                    currentRow = sp.ParseRow2(row, currentRow);

                    result += sp.TextOutput;
                    newxml = sp.mDOM.InnerXml;
                    //Console.WriteLine("");
                    //Console.WriteLine("--------------------------------------------");
                    //Console.WriteLine("So for XML Complete Generation : {0}", newxml);
                    //Console.WriteLine("--------------------------------------------");

                }

                if (currentRow == null)
                {
                    break;
                }
            }
            resultXML = sp.mDOM;
            return result;
        }
        protected FixedSpecification ProcessSpecification(XmlNode node, FixedSpecification fs)
        {
            string isVirtual = "";
            string maxGroup = "";
            string groupisMandatory = "";
            string isGroup = "";
            string startPosition = "";
            string minOccurrences = "";
            string maxOccurrences = "";
            string fieldType = "";
            string UnderGroup = "";
            FixedSpecification result = null;
            
            foreach (XmlAttribute xa in node.Attributes)
            {
                if (xa.Name == "isvirtual")
                {
                    isVirtual = xa.Value;
                }
                /*else if (xa.Name == "maxgroup")
                {
                    maxGroup = xa.Value;
                }
                else if (xa.Name == "groupismandatory")
                {
                    groupisMandatory = xa.Value;
                }*/
                else if (xa.Name == "isgroup")
                {
                    isGroup = xa.Value;
                }
                else if (xa.Name == "startposition")
                {
                    startPosition = xa.Value;
                }
                else if (xa.Name == "minoccurrences")
                {
                    minOccurrences = xa.Value;
                }
                else if (xa.Name == "maxoccurrences")
                {
                    maxOccurrences = xa.Value;
                }
                else if (xa.Name == "type")
                {
                    fieldType = xa.Value;
                }
                else if (xa.Name == "undergroup")
                {
                    UnderGroup = xa.Value;
                }
                
            }


            if (node.ChildNodes != null)
            {

                if (isVirtual == "1")
                {
                    FixedSpecification fsResult = null;
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.NodeType == XmlNodeType.Element)
                        {
                            if (fsResult == null)
                            {
                                fsResult = ProcessSpecification(childNode, fs);
                            }
                            else
                            {
                                ProcessSpecification(childNode, fsResult.parent);
                            }
                        }
                    }
                }
                else
                {
                    string virtualPrefix = "";
                    bool nodeIsGroup = false;
                    int nodeStartPosition = 0;
                    int nodeMinOccurrences = 0;
                    int nodeMaxOccurrences = 0;
                    int nodemaxGroup = 0;
                    bool nodeGroupIsMandatory = false;
                    if (isGroup == "1")
                    {
                        virtualPrefix = node.ParentNode.Name;
                        nodeIsGroup = true;

                        foreach (XmlAttribute ya in node.ParentNode.Attributes)
                        {
                            if (ya.Name == "maxgroup")
                            {
                                maxGroup = ya.Value;
                            }
                            else if (ya.Name == "groupismandatory")
                            {
                                groupisMandatory = ya.Value;
                            }

                        }
                        
                        int.TryParse(maxGroup, out nodemaxGroup);
                        if (groupisMandatory == "1")
                        {
                            nodeGroupIsMandatory = true;
                        }
                        
                    }


                    //int.TryParse(startPosition, out nodeStartPosition);
                    int.TryParse(minOccurrences, out nodeMinOccurrences);
                    int.TryParse(maxOccurrences, out nodeMaxOccurrences);

                    //FixedSpecification fspec = new FixedSpecification(node.Name, virtualPrefix, nodeIsGroup, nodeStartPosition, nodeMinOccurrences, nodeMaxOccurrences);
                    
                    //Console.WriteLine("Fixed Spec XML :{0} , virtualPrefix : {1}, isgroup : {2}, startposition : {3}, minocc :{4}, maxOcc : {5}, MaxGroup : {6}, GroupMandatory : {7}, undergroup : {8} ", node.Name, virtualPrefix, nodeIsGroup, nodeStartPosition, nodeMinOccurrences, nodeMaxOccurrences, nodemaxGroup, nodeGroupIsMandatory, UnderGroup);
                    FixedSpecification fspec = fs.AddSpecification(node.Name, virtualPrefix, nodeIsGroup, nodeStartPosition, nodeMinOccurrences, nodeMaxOccurrences, "", nodemaxGroup, nodeGroupIsMandatory, UnderGroup);
                    if (!string.IsNullOrEmpty(node.Name))
                    {
                        SpecficiationStored.Add(new Tuple<string, string, int, bool, string>(node.Name, virtualPrefix, nodeMinOccurrences, nodeGroupIsMandatory, UnderGroup));
                    }
                    

                    ProcessFields(node, fspec);

                    result = fspec;

                    
                }
            }


            

            return result;
        }
        protected void ProcessFields(XmlNode node, FixedSpecification fs)
        {
            string description = "";
            string startPosition = "";
            string type = "";
            string length = "";
            string isMandatory = "";
            string isIdRow = "";
            string idValue = "";
            string fieldname = "";
            string MaxGroup = "";
            string GroupIsMandatory = "";
            string UnderGroup = "";

            foreach (XmlNode childNode in node.ChildNodes)
            {
                fieldname = "";
                description = "";
                startPosition = "";
                type = "";
                length = "";
                isMandatory = "";
                isIdRow = "";
                idValue = "";
                MaxGroup = "";
                GroupIsMandatory = "";
                UnderGroup = "";

                foreach (XmlAttribute xa in childNode.Attributes)
                {
                    if (xa.Name == "fieldname")
                    {
                        fieldname = xa.Value;
                    }
                    if (xa.Name == "description")
                    {
                        description = xa.Value;
                    }
                    else if (xa.Name == "startposition")
                    {
                        startPosition = xa.Value;
                    }
                    else if (xa.Name == "type")
                    {
                        type = xa.Value;
                    }
                    else if (xa.Name == "length")
                    {
                        length = xa.Value;
                    }
                    else if (xa.Name == "ismandatory")
                    {
                        isMandatory = xa.Value;
                    }
                    else if (xa.Name == "isidrow")
                    {
                        isIdRow = xa.Value;
                    }
                    else if (xa.Name == "idvalue")
                    {
                        idValue = xa.Value;
                    }
                    else if (xa.Name == "maxgroup")
                    {
                        MaxGroup = xa.Value;
                    }
                    else if (xa.Name == "groupismandatory")
                    {
                        GroupIsMandatory = xa.Value;
                    }
                    else if (xa.Name == "undergroup")
                    {
                        UnderGroup = xa.Value;
                    }
                    

                }

                // Start creating the Fields
                FieldType ft = null;
                int fieldLength = 0;
                if (int.TryParse(length, out fieldLength))
                {
                    if (type == "S")
                    {
                        ft = new FieldStringType(fieldLength);
                    }
                    else if (type == "N")
                    {
                        ft = new FieldNumericType(fieldLength);
                    }
                    else if (type == "B")
                    {
                        ft = new FieldBooleanType();
                    }
                }

                int fieldStartPosition = 0;
                if (int.TryParse(startPosition, out fieldStartPosition))
                {
                }
                int fieldlength = 0;
                if (int.TryParse(length, out fieldlength))
                {
                }

                int fieldMaxGroup = 0;
                if (int.TryParse(MaxGroup, out fieldMaxGroup))
                {
                }

                bool fieldGroupIsMandatory = false;
                if (GroupIsMandatory == "1")
                {
                    fieldGroupIsMandatory = true;
                }
                bool fieldIsMandatory = false;
                if (isMandatory == "1")
                {
                    fieldIsMandatory = true;
                }
                bool fieldIsIdRow = false;
                if (isIdRow == "1")
                {
                    fieldIsIdRow = true;
                }
                bool fieldMainKey = false;
                bool fieldAlternativeKey = false;

                FixedField ff = new FixedField(childNode.Name, fieldname, description, type, fieldStartPosition, fieldLength, ft, fieldIsMandatory, fieldIsIdRow, fieldMainKey, fieldAlternativeKey, idValue, fieldMaxGroup, fieldGroupIsMandatory, UnderGroup);

                fs.AddFixedField(ff);
            }
        }

        public static void BasicValidationBeforeProcessingStart(string buffer)
        {

            string SpecificationsFirstSegment = SpecficiationStored[0].Item1;

            string SpecificationsLastSegment = SpecficiationStored[SpecficiationStored.Count - 1].Item1;

            String[] DataFileSplit = File.ReadAllLines(ConvertTXTtoXML.ConvertTXTtoXML.Globalfilepath);
            string[] DataFile = DataFileSplit[0].Split('\'').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            string firstsegment = DataFile.First().Substring(0, DataFile.First().IndexOf("+"));

            string lastsegment = DataFile.Last().Substring(0, DataFile.Last().IndexOf("+"));

            if (SpecificationsFirstSegment != firstsegment )
            {
                ConvertTXTtoXML.ConvertTXTtoXML.Error.Add(new Tuple<string>("<ERROR01> Specification's First Segment (" + SpecificationsFirstSegment + ") and EDI File's First Segment (" + firstsegment + ") is Mismatched  </ERROR01>"));
            }
            if (SpecificationsLastSegment != lastsegment)
            {
                ConvertTXTtoXML.ConvertTXTtoXML.Error.Add(new Tuple<string>("<ERROR01> Specification's Last Segment (" + SpecificationsLastSegment + ") and EDI File's Last Segment (" + lastsegment + ") is Mismatched  </ERROR01>"));
            }

            for(int k=0; k<DataFile.Count();k++)
            {
                //Console.WriteLine("cnt :{0}", k);

                if (!DataFile[k].Contains("+"))
                {
                    string IllogicalError = "Current Line :" + DataFile[k] + " and Previous Line :" + DataFile[k-1];
                    throw new System.InvalidOperationException("EDI File having Illogical Errors - " + IllogicalError);
                }

                string FirstSegment = DataFile[k].Substring(0, DataFile[k].IndexOf("+"));

                var SpecCheck = SpecficiationStored.Where(p => p.Item1.Equals(FirstSegment));
                if(SpecCheck.Count() <=0 )
                {
                    ConvertTXTtoXML.ConvertTXTtoXML.Error.Add(new Tuple<string>("<ERROR01> Invalid Row ("+ DataFile[k] + ").  "+FirstSegment+" Segment is not Defined in EDI Specification.  </ERROR01>"));
                }
                
            }

            var ffS = SpecficiationStored.Where(p => p.Item5.Equals("MSG1") && p.Item3 == 1);
            //var cehcekek = ffS.Where(k => k.Item1.Contains(checkrows.Substring(0,2)));

            /*for (int i = 0; i < SpecficiationStored.Count; i++)
            {
                string kk = SpecficiationStored[i].Item1;
                string ll = SpecficiationStored[i].Item2;
                int mm = SpecficiationStored[i].Item3;
                bool nn = SpecficiationStored[i].Item4;
                string oo = SpecficiationStored[i].Item5;
            }*/

        }

    }

}
