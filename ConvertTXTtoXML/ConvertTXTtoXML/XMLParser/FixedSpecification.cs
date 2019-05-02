using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtility
{
    public class FixedSpecification : Specification
    {

        protected string prefix;
        public string Prefix
        {
            get
            {
                return prefix;
            }
        }
        protected bool isVirtual;
        public bool IsVirtual
        {
            get
            {
                return isVirtual;
            }
        }

        
        protected string undergroup;
        public string Undergroup
        {
            get
            {
                return undergroup;
            }
        }

        protected string virtualPrefix;
        public string VirtualPrefix
        {
            get
            {
                return virtualPrefix;
            }
        }
        protected string fieldType;
        public string FieldType
        {
            get
            {
                return fieldType;
            }
        }

        protected bool isGroup;
        public bool IsGroup
        {
            get
            {
                return this.isGroup;
            }
        }

        protected int startPosition;
        public int StartPosition
        {
            get
            {
                return this.startPosition;
            }
        }

        protected int fieldLength;
        public int FieldLength
        {
            get
            {
                return this.fieldLength;
            }
        }

        protected int maxGroup;
        public int MaxGroup
        {
            get
            {
                return this.maxGroup;
            }
        }
        //groupIsMandatory
        protected bool groupIsMandatory;
        public bool GroupIsMandatory
        {
            get
            {
                return this.groupIsMandatory;
            }
        }
        protected List<FixedField> fields;
        public List<FixedField> Fields
        {
            get
            {
                return fields;
            }
        }
        protected List<FixedSpecification> specifications;
        public List<FixedSpecification> Specifications
        {
            get
            {
                return specifications;
            }
        }
        protected int minOccurrences;
        public int MinOccurrences
        {
            get
            {
                return this.minOccurrences;
            }
        }
        protected int maxOccurrences;
        public int MaxOccurrences
        {
            get
            {
                return this.maxOccurrences;
            }
        }
        public FixedSpecification parent;

        public const int UNLIMITED = 99999;

        public FixedSpecification(string prefix, bool isVirtual, string virtualPrefix, bool isGroup, int startPosition, int minOccurrences, int maxOccurrences, string fieldType, int MaxGroup, bool GroupIsMandatory, string Undergroup)
        {
            this.prefix = prefix;
            this.isVirtual = isVirtual;
            this.virtualPrefix = virtualPrefix;
            this.isGroup = isGroup;
            this.startPosition = startPosition;
            this.minOccurrences = minOccurrences;
            this.maxOccurrences = maxOccurrences;
            this.fieldType = fieldType;
            this.maxGroup = MaxGroup;
            this.groupIsMandatory = GroupIsMandatory;
            this.undergroup = Undergroup;
            this.fields = new List<FixedField>();
            this.specifications = new List<FixedSpecification>();
        }

        public FixedSpecification AddSpecification(string prefix, string virtualPrefix, bool isGroup, int startPosition, int minOcurrences, int maxOcurrences, string fieldType, int nodemaxGroup, bool nodeGroupIsMandatory, string undergroup)
        {
            FixedSpecification group = null;
            FixedSpecification result = null;
            if (isGroup)
            {
                group = new FixedSpecification(virtualPrefix,isVirtual, virtualPrefix, isGroup, startPosition, minOcurrences, maxOcurrences,fieldType, nodemaxGroup, nodeGroupIsMandatory, undergroup);
                group.parent = this;
                this.specifications.Add(group);
                result = group.AddSpecification(prefix, virtualPrefix, false, startPosition, minOcurrences, maxOcurrences,fieldType, nodemaxGroup, nodeGroupIsMandatory, undergroup);
            }
            else
            {
                result = new FixedSpecification(prefix, isVirtual, virtualPrefix, isGroup, startPosition, minOcurrences, maxOcurrences, fieldType, nodemaxGroup, nodeGroupIsMandatory, undergroup);
                result.parent = this;
                this.specifications.Add(result);
            }
            return result;
        }

        public FixedField AddFixedField(FixedField fixedField)
        {
            FixedField result = fixedField;
            fields.Add(fixedField);

            return result;
        }
    }
}
