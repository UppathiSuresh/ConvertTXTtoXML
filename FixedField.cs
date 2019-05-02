using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtility
{
    public class FixedField : Field
    {
        protected string code;
        public string Code
        {
            get
            {
                return code;
            }
        }
        protected string fieldname;
        public string Fieldname
        {
            get
            {
                return fieldname;
            }
        }
        protected string description;
        public string Description
        {
            get
            {
                return description;
            }
        }
        protected string type;
        public string Type
        {
            get
            {
                return type;
            }
        }
        protected FieldType fieldType;
        public FieldType FieldType
        {
            get
            {
                return fieldType;
            }
        }
        protected bool isMandatory;
        public bool IsMandatory
        {
            get
            {
                return isMandatory;
            }
        }

        protected bool groupIsMandatory;
        public bool groupismandatory
        {
            get
            {
                return groupIsMandatory;
            }
        }

        protected string underGroup;
        public string undergroup
        {
            get
            {
                return underGroup;
            }
        }

        protected bool isIdRow;
        public bool IsIdRow
        {
            get
            {
                return isIdRow;
            }
        }
        protected bool mainKey;
        protected bool alternativeKey;
        protected int startPosition;
        public int StartPosition
        {
            get
            {
                return startPosition;
            }
        }
        protected int fieldLength;
        public int FieldLength
        {
            get
            {
                return fieldLength;
            }
        }

        protected int maxgroup;
        public int maxGroup
        {
            get
            {
                return maxgroup;
            }
        }

        protected string idValue;
        public string IdValue
        {
            get
            {
                return idValue;
            }
        }

        public FixedField(string code, string fieldname,string description,string type, int startPosition,int fieldLength ,FieldType fieldType, bool isMandatory, bool isIdRow, bool mainKey, bool alternativeKey, string idValue, int maxgroup, bool groupIsMandatory, string underGroup)
        {

            this.code = code;
            this.fieldname = fieldname;
            this.description = description;
            this.fieldType = fieldType;
            this.isMandatory = isMandatory;
            this.isIdRow = isIdRow;
            this.mainKey = mainKey;
            this.alternativeKey = alternativeKey;
            this.startPosition = startPosition;
            this.fieldLength = fieldLength;
            this.idValue = idValue;
            this.type = type;
            this.maxgroup = maxgroup;
            this.groupIsMandatory = groupIsMandatory;
            this.underGroup = underGroup;
        }
        
    }
}
