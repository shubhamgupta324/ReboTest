using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReboProject
{
    public class Data
    {
        public string Word { get; set; }
        public double YAxis { get; set; }

        public double XAxis { get; set; }

        public double TextLength { get; set; }
        public Data()
            {
            }

        public Data(string word, double yAxis, double xAxis, double textLength)
        {
            Word = word;
            YAxis = yAxis;
            XAxis = xAxis;
            TextLength = textLength;
        }
    }
}