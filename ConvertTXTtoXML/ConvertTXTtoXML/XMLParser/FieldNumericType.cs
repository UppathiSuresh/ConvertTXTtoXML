using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtility
{
    public class FieldNumericType : FieldType
    {
        protected int integerLength;
        protected int decimalLength;
        protected string decimalSymbol;
        protected string thousandSymbol;
        protected float? min;
        protected float? max;
        
        public FieldNumericType(int length)
        {
            Initialize(length,0,"","",null,null);
        }

        public FieldNumericType(int integerLength, int decimalLength, string decimalSymbol, string thousandSymbol, float? min, float? max)
        {
            Initialize(integerLength, decimalLength, decimalSymbol, thousandSymbol, min, max);
        }

        public FieldNumericType(int integerLength, int decimalLength, string decimalSymbol, string thousandSymbol)
        {
            Initialize(integerLength, decimalLength, decimalSymbol, thousandSymbol, null, null);
        }

        protected void Initialize(int integerLength, int decimalLength, string decimalSymbol, string thousandSymbol, float? min, float? max)
        {
            this.length = integerLength + decimalLength;
            this.integerLength = integerLength;
            this.decimalLength = decimalLength;
            this.decimalSymbol = decimalSymbol;
            this.thousandSymbol = thousandSymbol;
            this.min = min;
            this.max = max;
        }

    }
}
