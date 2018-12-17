using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rebo.Model.CheckWord
{
    public class CheckWord
    {
        public string[] checkWordBefore
        {
            get
            {
                return new string[]
                {
              "defined in",
              "provided in",
              "pursuant to this",
              "contained in",
              "under this",
              "in this",
              "provisions of",
              "stated in",
              "provided for in",
              "pursuant to",
              "provisions of this",
              "Provisions",
              "of",
              "reflected on",
              "reference as",
              "year",
              "years",
              "and",
              "or",
              "subject to",
              "to"
            };
            }
        }
        public string[] afterCheckWord
        {
            get
            {
                return new string[] {

                "day",
                "month",
                "year",
                "days",
                "months",
                "years"
            };
            }
        }
        public string[] uppercaseAlpha
        {
            get
            {
                return new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            }
        }
        public string[] lowercaseAlpha
        {
            get
            {
                return new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            }
        }
        public string[] number
        {

            get
            {
                return new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100" };
            }

        }
        public string[] upperCaseRoman
        {
            get
            {
                return new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX", "XXXI", "XXXII", "XXXIII", "XXXIV", "XXXV", "XXXVI", "XXXVII", "XXXVIII", "XXXIX", "XXXX" };
            }
        }
        public string[] lowerCaseRoman
        {
            get
            {
                return new string[] { "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix", "x", "xi", "xii", "xiii", "xiv", "xv", "xvi", "xvii", "xviii", "xix", "xx", "xxi", "xxii", "xxiii", "xxiv", "xxv", "xxvi", "xxvii", "xxviii", "xxix", "xxx", "xxxi", "xxxii", "xxxiii", "xxxiv", "xxxv", "xxxvi", "xxxvii", "xxxviii", "xxxix", "xxxx" };
            }
        }
        public string[] specialChar
        {
            get
            {
                return new string[] { "\\.(?!\\S)", "\\(", "\\)", "\\[", "\\]", "\\:" };
            }
        }
        public string[] wrongReadCharList
        {
            get
            {
                return new string[] { "v", "x", "V", "X", "S", "5", "8", "l", "e", "s", "h", "9", "1", "11" };
            }
        }
        public string[] wrongReadStartCharList
        {
            get
            {
                return new string[] { "i", "I", "a", "1" };
            }
        }



    }

}
