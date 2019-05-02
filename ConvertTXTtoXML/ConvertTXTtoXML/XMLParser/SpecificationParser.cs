using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SystemUtility
{
    class SpecificationParser
    {
        public XmlDocument mDOM;
        protected XmlNode oHeaderNode;
        protected XmlNode oHeaderPrefix;
        protected XmlNode oChildsCollection1;
        protected XmlNode oChildsCollection2;
        protected Specification specification;
        protected Specification currentParsingRow;
        public string TextOutput;
        public static string PriorSegment = "", CurrSegment = "", CurrGroup = "";
        public Stack<XmlNode> NodeTrace;
        public SpecificationParser(Specification specification)
        {
            this.specification = specification;
        }

        public void Init()
        {
            currentParsingRow = null;
            mDOM = new XmlDocument();
            NodeTrace = new Stack<XmlNode>();
        }

        public bool ParseRow(string row)
        {
            bool result = false;

            if (currentParsingRow == null)
            {
                currentParsingRow = specification;
            }

            FixedSpecification fspec = currentParsingRow as FixedSpecification;
           // DynamicSpecification dspec = currentParsingRow as DynamicSpecification;

            // Parse if it is a FixedSpecification


            // if virtualPrefix is not empty we treat it like group. Later we are going to add a property isGroup or isVirtual to it.
            if (fspec != null)
            {
                if (fspec.VirtualPrefix == fspec.Prefix)
                {
                    // We are in a group node
                    //foreach(FixedSpecification fs in fspec.Specifications)
                    for (int i = 0; i < fspec.Specifications.Count; i++)
                    {
                        FixedSpecification fs = fspec.Specifications[i];
                        currentParsingRow = fs;
                        result = ParseRow(row);
                        if (result) break;
                    }
                }
                else
                {
                    // We are in a normal node and we have to parse the fields
                    string error = "";
                    foreach (FixedField ff in fspec.Fields)
                    {
                        if (row.Length < ff.StartPosition && ff.IsMandatory)
                        {
                            error += "Field " + ff.Description + "(" + ff.Code + ") is Missing\r\n";
                        }

                    }

                    if (error != "")
                    {
                        throw new ParseException(error);
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            else
            {
                // Raise an Exception
            }


            return result;
        }

        public FixedSpecification ParseRow2(string row, FixedSpecification fspec)
        {
            //Console.WriteLine("ParseRow2 METHOD Begins :{0}", row);
            FixedSpecification result = null;
            int minOccurrences = 0;
            minOccurrences = fspec.MinOccurrences;
            row = row.Replace("\r", "");
            //Console.WriteLine("Tag Start :{0} ", fspec.Prefix);
            
            if (fspec.Prefix == "")
            {
                // If this is the Top Most node, we will take the first one
                for (int specCount = 0; specCount < fspec.Specifications.Count; specCount++)
                {
                    FixedSpecification fsp = fspec.Specifications[specCount];
                    oHeaderNode = mDOM.CreateElement("EDI", "");
                    NodeTrace.Push(oHeaderNode);
                    mDOM.AppendChild(oHeaderNode);
                    result = ParseRow2(row, fsp);
                    break; //?????
                }

            }
            else
            {
                
                if (fspec.Prefix == fspec.VirtualPrefix)
                {
                    //Console.WriteLine("Child Node Start :{0}", fspec.Prefix);

                    // If this is a Virtual Node, we will check if this node is Mandatory and then, go to the parsing of the subspecification files
                    bool virtualMandatory = false;
                    bool GroupVirtualMandatory = false;

                    if (fspec.Specifications[0].MinOccurrences > 0)
                    {
                        virtualMandatory = true;
                    }
                    if(!string.IsNullOrEmpty(fspec.VirtualPrefix))
                    {
                        GroupVirtualMandatory = fspec.Specifications[0].GroupIsMandatory;
                        //Console.WriteLine("Group is mandatory :{0}", GroupVirtualMandatory);
                    }

                    FixedSpecification fsnext = fspec.Specifications[0];
                    /*if (!IdentifyRow(fspec.Specifications[0], row) && !virtualMandatory)
                    {
                        fsnext = FindNextSpecification2(fspec.Specifications[fspec.Specifications.Count - 1], row);

                    }*/
                    if (!IdentifyRow(fspec.Specifications[0], row) && !GroupVirtualMandatory)
                    {
                        fsnext = FindNextSpecification2(fspec.Specifications[fspec.Specifications.Count - 1], row);
                    }

                    result = ParseRow2(row, fsnext);

                }
                else
                {

                    if(!string.IsNullOrEmpty(fspec.VirtualPrefix))
                    {
                        //Console.WriteLine(" ");
                        //Console.WriteLine(" GROUP START ==========> {0}", fspec.VirtualPrefix);
                        //Console.WriteLine(" ");
                        CurrGroup = fspec.VirtualPrefix;

                    }
                    // If this is a Normal Node we will check if this node is Mandatory and then go to the parsing of the fields

                    // First select the identify fields
                    FixedField[] ffList = IdentifyFields(fspec);
                    
                    string error = "";
                    if (ffList.Count() == 0)
                    {
                        error += "Node " + fspec.Prefix + " doesn't have and Identify Field\r\n";
                    }

                    int matchCount = 0;
                    foreach (FixedField ff in ffList)
                    {

                        if (row.Length < ff.StartPosition)
                        {
                            error += "Identify Field " + ff.Description + "(" + ff.Code + ") is Missing\r\n";
                        }
                        else
                        {
                            string fieldValue = "";
                            FieldStringType fst = ff.FieldType as FieldStringType;
                            if (fst == null)
                            {
                                error += "Identify Field " + ff.Description + "(" + ff.Code + ") must be defined like String\r\n";
                            }
                            else
                            {
                                if ((ff.StartPosition + fst.Length) > row.Length)
                                {
                                    // Take the StartPosition from the Row up to the end of the row
                                    fieldValue = row.Substring(ff.StartPosition);
                                }
                                else
                                {
                                    // Take the StartPosition from the Row up to the length of the FieldType
                                    fieldValue = row.Substring(ff.StartPosition, fst.Length);
                                }
                            }

                            if (ff.IdValue.Trim() == "")
                            {
                                error += "Identify Field " + ff.Description + "(" + ff.Code + ") must have an IdValue defined\r\n";
                            }
                            else
                            {

                                //Console.WriteLine("Current Segment :{0} ---> Next Segment {1}", ff.IdValue.Trim(), fieldValue.Trim());
                                PriorSegment = ff.IdValue.Trim();
                                CurrSegment = fieldValue.Trim();
                                if (ff.IdValue.Trim() == fieldValue.Trim())
                                {
                                    //Console.WriteLine("MATCH COUNT");
                                    matchCount++;
                                }
                            }

                        }

                    }

                    if (ffList.Count() == matchCount)
                    {
                        result = fspec;


                        int count = fspec.parent.Specifications.Count;

                        if (fspec.parent != null && count > 0)
                        {
                            if (fspec.parent.Specifications[0].Prefix == fspec.Prefix)
                            {
                                oHeaderPrefix = mDOM.CreateElement(fspec.parent.Prefix, "");
                                XmlNode parentNode = NodeTrace.Peek();

                                // Measure the distance to the root of the specification and compare it to the stack
                                int distance = GetDistance(fspec.parent);

                                int stackCount = NodeTrace.Count;

                                for (int loop = 0; loop < (stackCount - distance); loop++)
                                {
                                    NodeTrace.Pop();
                                    parentNode = NodeTrace.Peek();
                                }

                                parentNode.AppendChild(oHeaderPrefix);
                                //Console.WriteLine(" ");
                                //Console.WriteLine(" GROUP wise XML Generate, before the tag "+ PriorSegment + "");
                                //Console.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                                //Console.WriteLine(parentNode.OuterXml);
                                NodeTrace.Push(oHeaderPrefix);
                            }

                        }

                        // Measure the distance to the root of the specification and compare it to the stack
                        int distance2 = GetDistance(fspec.parent);

                        int stackCount2 = NodeTrace.Count;

                        for (int loop = 0; loop < (stackCount2 - distance2 -1); loop++)
                        {
                            NodeTrace.Pop();
                            oHeaderPrefix = NodeTrace.Peek();
                        }


                        oChildsCollection1 = mDOM.CreateElement(fspec.Prefix, "");

                        oHeaderPrefix.AppendChild(oChildsCollection1);
                        NodeTrace.Push(oChildsCollection1);
                        row = row.Replace("\r", "");
                        Dictionary<string, string> FieldValueMap = new Dictionary<string, string>();
                         string[] parseCode = { "+" };
                        string[] FieldValue = row.Split(parseCode,  StringSplitOptions.None);
                        int index = 0;
                        //foreach (FixedField ff in fspec.Fields)
                        //{
                        //    //for( int i = 0; i < FieldValue.Count(); i++)
                        //    //{
                        //        if(index <FieldValue.Count())
                        //        {
                        //            string sValue = "";
                        //            sValue = FieldValue[index];
                        //            if(!FieldValueMap.ContainsKey(ff.Fieldname))
                        //            {
                        //                FieldValueMap.Add(ff.Code, sValue);
                        //            }
                        //        }
                        //            index++;

                        //    //}
                        //}                        

                        //Parse the each specif field and start building up the final XML
                        foreach (FixedField ff in fspec.Fields)
                        {
                            if(index < FieldValue.Count())
                            {
                                string sValue = "";
                                sValue = FieldValue[index];
                                sValue = sValue.Replace("\r","");
                                if (!FieldValueMap.ContainsKey(ff.Code))
                                {
                                    FieldValueMap.Add(ff.Code, sValue);
                                }
                            }
                            index++;
                            string fieldValue = "";
                            if (row.Length < ff.StartPosition)
                            {
                                if (ff.IsMandatory)
                                {
                                    error += "The Field " + ff.Description + "(" + ff.Code + ") is Missing\r\n";
                                }
                            }
                            else
                            {
                                if ((ff.StartPosition + ff.FieldType.Length) > row.Length)
                                {
                                    // Take the StartPosition from the Row up to the end of the row
                                    fieldValue = row.Substring(ff.StartPosition);
                                }
                                else
                                {
                                    // Take the StartPosition from the Row up to the length of the FieldType
                                    fieldValue = row.Substring(ff.StartPosition, ff.FieldType.Length);
                                }
                            }


                            string spath = "";

                            spath += ParentPath(fspec);

                            //spath += "\\" + ff.Code + "(" + ff.Description + " (" + (ff.StartPosition + 1).ToString() + ", " + ff.FieldType.Length + "))" + " = " + fieldValue;

                            //TextOutput += spath + "\r\n";
                            //oChildsCollection2  XmlNode(); 

                            oChildsCollection2 = mDOM.CreateElement(ff.Code, "");

                            if (ff.Fieldname != "")
                            {
                                XmlAttribute name = mDOM.CreateAttribute("fieldname");
                                name.Value = ff.Fieldname;
                                oChildsCollection2.Attributes.Append(name);
                            }

                            if (FieldValueMap.ContainsKey(ff.Code))
                            {
                                FieldValueMap.TryGetValue(ff.Code, out fieldValue);
                            }
                            oChildsCollection2.InnerText = fieldValue.Trim();
                            oChildsCollection1.AppendChild(oChildsCollection2);
                            if (index == FieldValue.Count())
                            {
                                //Console.WriteLine(" ");
                                //Console.WriteLine(" ");
                                //Console.WriteLine(" Node Created for Each Segment line for : {0} ", ff.Code);
                                //Console.WriteLine(" --------------------------------- ");
                                //Console.WriteLine(oChildsCollection1.OuterXml);
                                //Console.WriteLine(" ");
                                
                            }
                            //System.Windows.Forms.MessageBox.Show(spath);

                        }

                        // POP the XMLNode from NodeTrace
                        NodeTrace.Pop();
                    }
                    else
                    {
                        if ( fspec.MinOccurrences > 0 )
                        {

                            //Console.WriteLine("MinOccurrences if :{0} and {1} and {2} and {3}", fspec.MinOccurrences, fspec.GroupIsMandatory, PriorSegment, row);
                            //if (matchCount == 0)
                            //{
                            //    error += "Row " + fspec.Prefix + " expected but cannot be found: " + row + "\r\n";
                            //}
                            //else
                            //{
                            //    error += "Row " + fspec.Prefix + " cannot be Completely Identified\r\n";
                            //}
                        }
                        else if(fspec.MinOccurrences == 0)
                        {
                            //Console.WriteLine("MinOccurrences else :{0} and {1}", fspec.MinOccurrences, fspec.GroupIsMandatory);
                            // If it is not mandatory, we should go to next node, try to match it up and return it
                            FixedSpecification nextSpecification = null;
                            nextSpecification = FindNextSpecification2(fspec, row);

                            if (nextSpecification == null)
                            {
                                error += "The following Row cannot be identified: " + row + "\r\n";
                            }
                            else
                            {

                                result = ParseRow2(row, nextSpecification);

                            }
                        }
                    }

                    if (error != "")
                    {
                        throw new ParseException(error);
                    }
                }

            }
            return result;
        }

        public string ParentPath(FixedSpecification ff)
        {
            string result = "";
            FixedSpecification ffparent = ff.parent;
            if (ffparent != null)
            {
                result += ParentPath(ff.parent) + "\\" + ff.Prefix;
            }
            else
            {
                result = ff.Prefix;
            }

            return result;
        }

        public FixedSpecification FindNextSpecification(FixedSpecification fs)
        {
            FixedSpecification result = null;
            // We have to check minOccurrences and maxOccurences to avoid loops and detect not completed files
            FixedSpecification parent = fs.parent;

            FixedSpecification child = null;
            int currentPosition = -1;
            if (parent != null)
            {
                for (int i = 0; i < parent.Specifications.Count; i++)
                {
                    child = parent.Specifications[i];
                    if (child.Prefix == fs.Prefix)
                    {
                        currentPosition = i;
                        break;
                    }
                }
            }
            else
            {
                // We are in the top most node
                for (int i = 0; i < fs.Specifications.Count; i++)
                {
                    child = fs.Specifications[i];
                    if (child.Prefix == fs.Prefix)
                    {
                        currentPosition = i;
                        break;
                    }
                }
            }


            if (currentPosition != -1)
            {
                // We will try to find the next one available
                if ((currentPosition + 1) < parent.Specifications.Count)
                {
                    // If there is still more available, we are going to return the next one
                    result = parent.Specifications[currentPosition + 1];
                }
                else
                {
                    // If there is no more available, we have to go to the parent, check minOccurrences and maxOccurrences (Still Pending), and then decide 
                    // if we try the same parent again, or if we go to the next "Parent" node
                    if (parent != null)
                    {
                        result = FindNextSpecification(parent);
                    }
                }
            }

            return result;
        }

        public FixedSpecification FindNextSpecification2(FixedSpecification fs, string row)
        {
            FixedSpecification result = null;
            // We have to check minOccurrences and maxOccurences to avoid loops and detect not completed files
            FixedSpecification parent = fs.parent;

            FixedSpecification child = null;
            int currentPosition = -1;
            if (parent != null)
            {
                for (int i = 0; i < parent.Specifications.Count; i++)
                {
                    child = parent.Specifications[i];
                    
                    if (child.Prefix == fs.Prefix)
                    {
                        
                        currentPosition = i;
                        break;
                    }
                }
            }
            else
            {
                // We are in the top most node
                for (int i = 0; i < fs.Specifications.Count; i++)
                {
                    child = fs.Specifications[i];
                    
                    if (child.Prefix == fs.Prefix)
                    {
                        currentPosition = i;
                        break;
                    }
                }
            }


            if (currentPosition != -1)
            {
                // We will try to find the next one available
                if ((currentPosition + 1) < parent.Specifications.Count)
                {
                    // If there is still more available, we are going to return the next one
                    result = parent.Specifications[currentPosition + 1];
                }
                else
                {
                    // If there is no more available, we have to go to the parent, check minOccurrences and maxOccurrences (Still Pending), and then decide 
                    // if we try the same parent again, or if we go to the next "Parent" node
                    if (parent != null)
                    {
                        // Now we try to identify the row with the "first" specification
                        // If the row match, we will start looping again and we will take parent.Specifications[0]
                        FixedSpecification firstSibling = parent.Specifications[0];

                        // If the row doesn't match, we will go to the parent node
                        FixedSpecification nextFromParent = FindNextSpecification2(parent, row);

                        if (IdentifyRow(firstSibling, row))
                        {
                            // If firstSibling match with the row, we are going to iterate throw same group again
                            result = firstSibling;
                        }
                        else
                        {
                            result = nextFromParent;
                        }
                    }
                }
            }

            return result;
        }


        public bool IdentifyRow(FixedSpecification fspec, string row)
        {
            bool result = false;

            FixedField[] ffList = IdentifyFields(fspec);
            string error = "";
            if (ffList.Count() == 0)
            {
                error += "Node " + fspec.Prefix + " doesn't have and Identify Field\r\n";

            }

            int matchCount = 0;
            foreach (FixedField ff in ffList)
            {
                if (row.Length < ff.StartPosition)
                {
                    error += "Identify Field " + ff.Description + "(" + ff.Code + ") is Missing\r\n";

                }
                else
                {
                    string fieldValue = "";
                    FieldStringType fst = ff.FieldType as FieldStringType;
                    if (fst == null)
                    {
                        error += "Identify Field " + ff.Description + "(" + ff.Code + ") must be defined like String\r\n";

                    }
                    else
                    {
                        if ((ff.StartPosition + fst.Length) > row.Length)
                        {
                            // Take the StartPosition from the Row up to the end of the row
                            fieldValue = row.Substring(ff.StartPosition);
                        }
                        else
                        {
                            // Take the StartPosition from the Row up to the length of the FieldType
                            fieldValue = row.Substring(ff.StartPosition, fst.Length);
                        }
                    }

                    if (ff.IdValue.Trim() == "")
                    {
                        error += "Identify Field " + ff.Description + "(" + ff.Code + ") must have an IdValue defined\r\n";

                    }
                    else
                    {
                        if (ff.IdValue.Trim() == fieldValue.Trim())
                        {
                            matchCount++;
                        }
                    }

                }

            }

            if (ffList.Count() == matchCount)
            {
                result = true;
            }


            if (error != "")
            {
                throw new ParseException(error);
            }

            return result;
        }

        protected FixedField[] IdentifyFields(FixedSpecification fspec)
        {
            List<FixedField> result = new List<FixedField>();

            foreach (FixedField ff in fspec.Fields)
            {
                if (ff.IsIdRow)
                {
                    result.Add(ff);
                    //Console.WriteLine("First Segment of Each Line :{0}", result[0].IdValue);
                }
            }

            return result.ToArray();
        }

        protected int GetDistance(FixedSpecification fspec)
        {
            int result =1;

            if (fspec.parent != null)
            {
                result += GetDistance(fspec.parent);
            }
            else
            {
                result = 0;
            }

            return result;
        }
    }
}
