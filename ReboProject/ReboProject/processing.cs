using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Threading;

namespace ReboProject
{
    public class processing
    {
        #region -------------------------------------- Global Intialization ----------------------------------------------------------------
        
        //List<string> uppercaseAlpha = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        //List<string> lowercaseAlpha = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        //List<string> number = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100" };

        //List<string> upperCaseRoman = new List<string> { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX", "XXXI", "XXXII", "XXXIII", "XXXIV", "XXXV", "XXXVI", "XXXVII", "XXXVIII", "XXXIX", "XXXX" };

        //List<string> lowerCaseRoman = new List<string> { "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix", "x", "xi", "xii", "xiii", "xiv", "xv", "xvi", "xvii", "xviii", "xix", "xx", "xxi", "xxii", "xxiii", "xxiv", "xxv", "xxvi", "xxvii", "xxviii", "xxix", "xxx", "xxxi", "xxxii", "xxxiii", "xxxiv", "xxxv", "xxxvi", "xxxvii", "xxxviii", "xxxix", "xxxx" };
        Rebo.Model.Regex.Regex _regex = new Rebo.Model.Regex.Regex();

        #region commented code
        // regex to find the section in para
        //static Dictionary<string, int> treeCorrectionRegex = treeCorrectionRegexFn();
        //public static Dictionary<string, int> treeCorrectionRegexFn()
        //{
        //    Dictionary<string, int> treeCorrectionRegex = new Dictionary<string, int>(); // check the regex 

        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)article|art1c1e|art1cle|artic1e)\s\d+\.(?:\d+\.?)*)(?!\S)", 0); //  article 1.1 
        //    treeCorrectionRegex.Add(@"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)", 0); //    1.1  / 1.1 a)
        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)", 0); //    section 1.1 

        //    treeCorrectionRegex.Add(@"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)", 1); //    1./ 1. a)

        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", 1); //    section 1
        //    treeCorrectionRegex.Add(@"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", 1); //   article 1
        //    treeCorrectionRegex.Add(@"^[\s]*(?!0)([0-9]{1,2}[:])(?!\S)", 1); //   1:
        //    treeCorrectionRegex.Add(@"^[\s]*(?!0)([0-9]{1,2}[.])(?!\S)",1); //   1.
        //    treeCorrectionRegex.Add(@"^[\s]*([[(][\s]*(?!0)([0-9]{1,2})[\s]*[)])(?!\S)", 1); //   (1)
        //    treeCorrectionRegex.Add(@"^[\s]*(?!0)([0-9]{1,2}[]])(?!\S)", 1); //   1]
        //    treeCorrectionRegex.Add(@"^[\s]*([[\\[][\s]*(?!0)([0-9]{1,2})[\s]*[]])(?!\S)", 1); //   [1]
        //    treeCorrectionRegex.Add(@"^[\s]*(?!0)([0-9]{1,2}[)])(?!\S)", 1); //   1)

        //    treeCorrectionRegex.Add(@"^[\s]*[(][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[)](?!\S)", 2); //    (xvii)
        //    treeCorrectionRegex.Add(@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[)](?!\S)", 2); //    xvii)
        //    treeCorrectionRegex.Add(@"^[\s]*[[][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[]](?!\S)", 2); //    [xvii]
        //    treeCorrectionRegex.Add(@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[]](?!\S)", 2); //    xvii]
        //    treeCorrectionRegex.Add(@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[:](?!\S)", 2); //    xvii:
        //    treeCorrectionRegex.Add(@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.](?!\S)", 2); //    xvii.
        //    treeCorrectionRegex.Add(@"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", 2); //    section xvii
        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", 2); //    article xvii 

        //    treeCorrectionRegex.Add(@"^[\s]*[(][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[)](?!\S)", 3); //    (XVII)
        //    treeCorrectionRegex.Add(@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[)](?!\S)", 3); //    XVII)
        //    treeCorrectionRegex.Add(@"^[\s]*[[][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[]](?!\S)", 3); //    [XVII]
        //    treeCorrectionRegex.Add(@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[]](?!\S)", 3); //    XVII]
        //    treeCorrectionRegex.Add(@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[:](?!\S)", 3); //    XVII:
        //    treeCorrectionRegex.Add(@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.](?!\S)", 3); //    XVII.
        //    treeCorrectionRegex.Add(@"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", 3); //    section XVII
        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", 3); //    article XVII

        //    treeCorrectionRegex.Add(@"^[\s]*([a-z][\s]{0,1}[.])(?!\S)", 4);  //  a.
        //    treeCorrectionRegex.Add(@"^[\s]*([a-z][\s]{0,1}[:])(?!\S)", 4);  //  a:
        //    treeCorrectionRegex.Add(@"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)", 4);  //    (a)
        //    treeCorrectionRegex.Add(@"^[\s]*([a-z][\s]{0,1}[)])(?!\S)", 4);  // a)
        //    treeCorrectionRegex.Add(@"^[\s]*([[][\s]*[a-z][\s]{0,1}[]])(?!\S)", 4);  //   a]
        //    treeCorrectionRegex.Add(@"^[\s]*([a-z][]])(?!\S)", 4);  //     [a]
        //    treeCorrectionRegex.Add(@"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)", 4);  //      section a
        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)", 4);  //      article a

        //    treeCorrectionRegex.Add(@"^[\s]*([A-Z][\s]{0,1}[.])(?!\S)", 5);  // A.
        //    treeCorrectionRegex.Add(@"^[\s]*([A-Z][\s]{0,1}[:])(?!\S)", 5);  // A:
        //    treeCorrectionRegex.Add(@"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)", 5);  // (A)
        //    treeCorrectionRegex.Add(@"^[\s]*([A-Z][\s]{0,1}[)])(?!\S)", 5);  // A)
        //    treeCorrectionRegex.Add(@"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)", 5);  // A]
        //    treeCorrectionRegex.Add(@"^[\s]*([A-Z][]])(?!\S)", 5);  // [A]
        //    treeCorrectionRegex.Add(@"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)", 5);  // section A
        //    treeCorrectionRegex.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)", 5);  // ARTICLE A
        //    return treeCorrectionRegex;
        //}

        // match the section value with the list of defined section number
        #endregion
        //Dictionary<int, List<string>> regexMatch = null;// regexMatchFn();
        Rebo.Model.CheckWord.CheckWord _CheckWord = new Rebo.Model.CheckWord.CheckWord();
        public Dictionary<int, string[]> regexMatch()
        {
            Dictionary<int, string[]> regexMatch = new Dictionary<int, string[]>();
            regexMatch.Add(1, _CheckWord.number);
            regexMatch.Add(2, _CheckWord.lowerCaseRoman);
            regexMatch.Add(3, _CheckWord.upperCaseRoman);
            regexMatch.Add(4, _CheckWord.lowercaseAlpha);
            regexMatch.Add(5, _CheckWord.uppercaseAlpha);
            return regexMatch;
        }
        #region comment code
        // regex to ignore while checking tree
        //static List<string> regexNotToCheck = regexNotToCheckFn();
        //public static List<string> regexNotToCheckFn()
        //{
        //    List<string> regexMatch = new List<string>();
        //    regexMatch.Add(@"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)");  // 1.1  / 1.1 a)
        //    regexMatch.Add(@"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"); //  article 1.1 
        //    regexMatch.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
        //    regexMatch.Add(@"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
        //    regexMatch.Add(@"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section 1
        //    regexMatch.Add(@"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"); //   article 1
        //    regexMatch.Add(@"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section xvii
        //    regexMatch.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    article xvii 
        //    regexMatch.Add(@"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section XVII
        //    regexMatch.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    article XVII
        //    regexMatch.Add(@"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)");  //      section a
        //    regexMatch.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)");  //      article a
        //    regexMatch.Add(@"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)");  // section A
        //    regexMatch.Add(@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)");  // ARTICLE A

        //    return regexMatch;
        //}
        #endregion

        //List<string> specialChar = new List<string> { "\\.(?!\\S)", "\\(", "\\)", "\\[", "\\]", "\\:" };
        //List<string> wrongReadCharList = new List<string> { "v", "x", "V", "X", "S", "5", "8", "l", "e", "s", "h", "9", "1", "11" };
        //List<string> wrongReadStartCharList = new List<string> { "i", "I", "a", "1" };

        #endregion

        #region------------get the complete section once for the first tree structure-----------------
        public List<string> getSectionForPara(int lineNumber, string para, string lastLine, bool nextPara)
        {
            try
            {

           
            List<string> sectionFoundData = new List<string>(); // save the section number and regex...

            #region commented code
            //List<string> checkWordBefore = new List<string>(); // words to check before section number

            //checkWordBefore.Add("defined in");
            //checkWordBefore.Add("provided in");
            //checkWordBefore.Add("pursuant to this");
            //checkWordBefore.Add("contained in");
            //checkWordBefore.Add("under this");
            //checkWordBefore.Add("in this");
            //checkWordBefore.Add("provisions of");
            //checkWordBefore.Add("stated in");
            //checkWordBefore.Add("provided for in");
            //checkWordBefore.Add("pursuant to");
            //checkWordBefore.Add("provisions of this");
            //checkWordBefore.Add("Provisions");
            //checkWordBefore.Add("of");
            //checkWordBefore.Add("reflected on");
            //checkWordBefore.Add("reference as");
            //checkWordBefore.Add("year");
            //checkWordBefore.Add("years");
            //checkWordBefore.Add("and");
            //checkWordBefore.Add("or");
            //checkWordBefore.Add("subject to");
            //checkWordBefore.Add("to");

            //List<string> afterCheckWord = new List<string>(); // check the words after section number

            //afterCheckWord.Add("day");
            //afterCheckWord.Add("month");
            //afterCheckWord.Add("year");
            //afterCheckWord.Add("days");
            //afterCheckWord.Add("months");
            //afterCheckWord.Add("years");

            #endregion

            if (String.IsNullOrEmpty(lastLine) || nextPara == true) // if last line not there......(first page of pdf) or its a different para
            {
                sectionFoundData = regexLoop(para);
                if (sectionFoundData[0] != null)  // if no section found
                {
                    var paraCopy = para; // copy of para
                    var sectionNumber = sectionFoundData[0]; // get the section number
                    var sectionLength = sectionNumber.Length; // length of section number
                    var sentenceWithoutSection = paraCopy.Remove(0, sectionLength).Trim(); // remove section number from 
                    if (sentenceWithoutSection != "")
                    {
                        foreach (var item in _CheckWord.afterCheckWord) // ckeck words after section nuber
                        {
                            if (sentenceWithoutSection.IndexOf(item) == 0)
                            {
                                sectionFoundData = new List<string>();
                                sectionFoundData.Add(null);
                                sectionFoundData.Add(null);
                                break;
                            }
                        }
                    }
                }
            }

            else // if last line there
            {
                sectionFoundData = regexLoop(para);
                if (sectionFoundData[0] != null) // if section no there
                {
                    var paraCopy = para; // copy of para
                    var sectionNumber = sectionFoundData[0]; // get the section number
                    var sectionLength = sectionNumber.Length; // length of section number
                    var sentenceWithoutSection = paraCopy.Remove(0, sectionLength).Trim(); // remove section number from 
                    if (sentenceWithoutSection != "")
                    {
                        var firsTChar = sentenceWithoutSection[0];
                        if (firsTChar.ToString() == char.ToUpper(firsTChar).ToString()) // check first word starts with uppercase 
                        {
                            if (lastLine.EndsWith(".") | lastLine.EndsWith(";") | lastLine.EndsWith(",") | lastLine.EndsWith(":")) // check if last sentence ends with full stop or not
                            {
                                foreach (var item in _CheckWord.afterCheckWord) // ckeck words after section nuber
                                {
                                    if (sentenceWithoutSection.IndexOf(item) == 0)
                                    {
                                        sectionFoundData = new List<string>();
                                        sectionFoundData.Add(null);
                                        sectionFoundData.Add(null);
                                        break;
                                    }
                                }
                            }
                            else if (lineNumber != 0)
                            {
                                sectionFoundData = new List<string>();
                                sectionFoundData.Add(null);
                                sectionFoundData.Add(null);
                            }
                        }
                        else // if not upper case then section not found
                        {
                            sectionFoundData = new List<string>();
                            sectionFoundData.Add(null);
                            sectionFoundData.Add(null);
                        }
                    }
                    else if ((sentenceWithoutSection == ""))
                    {
                        if ((!lastLine.EndsWith(".") | !lastLine.EndsWith(";") | !lastLine.EndsWith(",")))
                        {
                            foreach (var item in _CheckWord.checkWordBefore)
                            {
                                if (lastLine.EndsWith(item))
                                {
                                    sectionFoundData = new List<string>();
                                    sectionFoundData.Add(null);
                                    sectionFoundData.Add(null);
                                    break;
                                }
                            }
                        }
                    }
                    else // if not upper case then section not found
                    {
                        sectionFoundData = new List<string>();
                        sectionFoundData.Add(null);
                        sectionFoundData.Add(null);
                    }
                }
            }
            return sectionFoundData; // return section number and regex
            }
            catch (Exception ex)
            {
                _Default.LogError("getSectionForPara", "", ex);
                throw;
            }
        }
        #endregion

        #region ---------------------------loop through all the regex to get the section----------------------------------
        public List<string> regexLoop(string para)
        {
            try
            {

           
            #region commented
            //Dictionary<string, string> regexCorrectionRegex = new Dictionary<string, string>();
            //regexCorrectionRegex.Add(@"^((?i)(section|article))?[\s]*((\d{1,2}|(I|l)(\d{1,1})|(\d{1,1})(I|l|O))[\s]{0,1}\.[\s]{0,1}(\d{1,2}|(I|l|O))([\s]{0,1}(I|l|O)|[\s]{0,1}[(?:\d+\.?)]*))[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.|•|-])?(?!\S)", @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)");
            //regexCorrectionRegex.Add(@"^((?i)(section|article))?[\s]*(((?!0)\d{1,2}|(I|T|l)(\d{1,1})|(\d{1,1})(I|T|l))[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)", @"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)");

            //
            //Dictionary<int, string> regexDictionary = new Dictionary<int, string>(); // check the regex 

            //regexDictionary.Add(44, @"^((?i)(section|article))?[\s]*((\d{1,2}|(I|l)(\d{1,1})|(\d{1,1})(I|l|O))[\s]{0,1}\.[\s]{0,1}(\d{1,2}|(I|l|O))([\s]{0,1}(I|l|O)|[\s]{0,1}[(?:\d+\.?)]*))[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.|•|-])?(?!\S)"); //    1.1  / 1.1 a)
            //regexDictionary.Add(1, @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)"); //    1.1  / 1.1 a)

            //regexDictionary.Add(45, @"^((?i)(section|article))?[\s]*(((?!0)\d{1,2}|(I|T|l)(\d{1,1})|(\d{1,1})(I|T|l))[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"); //    1./ 1. a)
            //regexDictionary.Add(2, @"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"); //    1./ 1. a)

            //regexDictionary.Add(3, @"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section 1
            //regexDictionary.Add(4, @"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
            //regexDictionary.Add(5, @"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"); //   article 1
            //regexDictionary.Add(6, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
            //regexDictionary.Add(7, @"^[\s]*(?!0)([0-9]{1,2}[:])(?!\S)"); //   1:
            //regexDictionary.Add(46, @"^[\s]*(?!0)([0-9]{1,2}[.])(?!\S)"); //   1.
            //regexDictionary.Add(8, @"^[\s]*([[(][\s]*(?!0)([0-9]{1,2})[\s]*[)])(?!\S)"); //   (1)
            //regexDictionary.Add(9, @"^[\s]*(?!0)([0-9]{1,2}[]])(?!\S)"); //   1]
            //regexDictionary.Add(10, @"^[\s]*([[\\[][\s]*(?!0)([0-9]{1,2})[\s]*[]])(?!\S)"); //   [1]
            //regexDictionary.Add(11, @"^[\s]*(?!0)([0-9]{1,2}[)])(?!\S)"); //   1)

            //regexDictionary.Add(12, @"^[\s]*[(][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[)](?!\S)"); //    (xvii)
            //regexDictionary.Add(13, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[)](?!\S)"); //    xvii)
            //regexDictionary.Add(14, @"^[\s]*[[][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[]](?!\S)"); //    [xvii]
            //regexDictionary.Add(15, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[]](?!\S)"); //    xvii]
            //regexDictionary.Add(16, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[:](?!\S)"); //    xvii:
            //regexDictionary.Add(17, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.](?!\S)"); //    xvii.
            //regexDictionary.Add(18, @"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section xvii
            //regexDictionary.Add(19, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    article xvii

            //regexDictionary.Add(20, @"^[\s]*[(][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[)](?!\S)"); //    (XVII)
            //regexDictionary.Add(21, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[)](?!\S)"); //    XVII)
            //regexDictionary.Add(22, @"^[\s]*[[][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[]](?!\S)"); //    [XVII]
            //regexDictionary.Add(23, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[]](?!\S)"); //    XVII]
            //regexDictionary.Add(24, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[:](?!\S)"); //    XVII:
            //regexDictionary.Add(25, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.](?!\S)"); //    XVII.
            //regexDictionary.Add(26, @"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    section XVII
            //regexDictionary.Add(27, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"); //    article XVII

            //regexDictionary.Add(28, @"^[\s]*([a-z][\s]{0,1}[.])(?!\S)");  //  a.
            //regexDictionary.Add(29, @"^[\s]*([a-z][\s]{0,1}[:])(?!\S)");  //  a:
            //regexDictionary.Add(30, @"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)");  //    (a)
            //regexDictionary.Add(31, @"^[\s]*([a-z][\s]{0,1}[)])(?!\S)");  // a)
            //regexDictionary.Add(32, @"^[\s]*([[][\s]*[a-z][\s]*[]])(?!\S)");  //   a]
            //regexDictionary.Add(33, @"^[\s]*([a-z][\s]{0,1}[]])(?!\S)");  //     [a]
            //regexDictionary.Add(34, @"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)");  //      section a
            //regexDictionary.Add(35, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)");  //      article a

            //regexDictionary.Add(36, @"^[\s]*([A-Z][\s]{0,1}[.])(?!\S)");  // A.
            //regexDictionary.Add(37, @"^[\s]*([A-Z][\s]{0,1}[:])(?!\S)");  // A:
            //regexDictionary.Add(38, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)");  // (A)
            //regexDictionary.Add(39, @"^[\s]*([A-Z][\s]{0,1}[)])(?!\S)");  // A)
            //regexDictionary.Add(40, @"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)");  // A]
            //regexDictionary.Add(41, @"^[\s]*([A-Z][\s]{0,1}[]])(?!\S)");  // [A]
            //regexDictionary.Add(42, @"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)");  // section A
            //regexDictionary.Add(43, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)");  // ARTICLE A
            #endregion
            List<string> sectiongot = new List<string>();
            var regexFound = "";
            var sectionFound = "";
            foreach (var item in _regex.regexDictionary) // loop through all the regex
            {
                Regex regex = new Regex(item.Value);
                var foundSection = false;
                foreach (Match match in regex.Matches(para)) // found match
                {
                    // this process saves section and regex ...and also checks if the section is from regexCorrectionRegex or not
                    // if the section is from regexCorrectionRegex ...then it corrects the section/regex and gives the correct section/regex to that sentence
                    var toCheck = match.Value.Trim();
                    // to check if starts with section/article
                    toCheck = Regex.Replace(toCheck, "(?i)article", "");
                    toCheck = Regex.Replace(toCheck, "(?i)section", "");
                    if (toCheck.Trim() == "")
                        continue;

                    regexFound = item.Value;
                    sectionFound = match.Value.Trim();

                    var sectionCheck = match.Value.Trim();
                    sectionCheck = Regex.Replace(sectionCheck, "[(|<|\\[|)|\\]|>|:|;|.|,]", "");
                    sectionCheck = Regex.Replace(sectionCheck, "(?i)article", "");
                    sectionCheck = Regex.Replace(sectionCheck, "(?i)section", "");
                    sectionCheck = Regex.Replace(sectionCheck, "(-|•)", "");
                    if (sectionCheck != "0")
                    {
                        if (item.Key == 44 | item.Key == 45)
                        {
                            foreach (var regexVal in _regex.regexCorrectionRegex)
                            {
                                if (regexVal.Key == regexFound)
                                {
                                    Regex regexCheck = new Regex(regexVal.Value);
                                    var matchVal = regexCheck.Match(para); // check if match found
                                    if (matchVal.Success & sectionFound == matchVal.Value)
                                    {
                                        sectionFound = Regex.Replace(sectionFound, "(-|•)", "");
                                        sectionFound = matchVal.Value;
                                        regexFound = regexVal.Value;
                                    }
                                    else
                                    {
                                        sectionFound = Regex.Replace(sectionFound, "(?i)article", "");
                                        sectionFound = Regex.Replace(sectionFound, "(?i)section", "");
                                        sectionFound = Regex.Replace(sectionFound, "(-|•)", "");
                                        sectionFound = sectionFound.Replace(" ", "");
                                        sectionFound = Regex.Replace(sectionFound, "[T|I|l]", "1");
                                        regexFound = regexVal.Value;
                                    }
                                    break;
                                }
                            }
                        }
                        sectionFound = Regex.Replace(sectionFound, "(-|•)", "");
                        sectiongot.Add(sectionFound);
                        sectiongot.Add(regexFound);
                        foundSection = true;
                    }
                }
                if (foundSection == true)
                    break;
            }
            if (sectiongot.Count() == 0)
            {
                sectiongot.Add(null);
                sectiongot.Add(null);
            }
            return sectiongot;
            }
            catch (Exception ex)
            {
                _Default.LogError("regexLoop", "", ex);
                throw;
            }
        }
        #endregion

        #region ---------------loop through tree to check para and its section number-----------------
        public void checkParaToFindSection(Dictionary<string, string> sectionList, string sectionNoreadAllParaData, JToken subChild, out List<string> sectionListCopy, out bool foundSection, out bool readSection)
        {
            try
            {

                foundSection = false;
                readSection = true;
                sectionListCopy = new List<string>();
                foreach (var item in subChild)
                {
                    sectionListCopy = new List<string>();
                    var child = item["children"];
                    var sectioNo = item["section"].ToString();
                    var paraData = item["para"];
                    var parentCheck = (int)item["parentCheck"];

                    for (int j = 0; j < paraData.Count(); j++)
                    {
                        var paraVal = paraData[j].ToString().Trim();

                        if ((paraVal.EndsWith(sectionNoreadAllParaData.Trim()) | sectionNoreadAllParaData.Trim().EndsWith(paraVal) | paraVal.StartsWith(sectionNoreadAllParaData.Trim()) | sectionNoreadAllParaData.Trim().StartsWith(paraVal)) & paraVal.Length > 10)
                        {
                            if (!sectionListCopy.Contains(sectioNo) & !sectionList.ContainsKey(sectioNo))
                            {
                                sectionList.Add(sectioNo, "sectionNo");
                                foundSection = true;
                                if (parentCheck == 1)
                                {
                                    readSection = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (child.Count() > 0 & foundSection == false)
                    {
                        checkParaToFindSection(sectionList, sectionNoreadAllParaData, child, out sectionListCopy, out foundSection, out readSection);
                        if (sectionListCopy.Count > 0 & foundSection == true)
                        {
                            List<string> sectionListCopyList = new List<string>();
                            foreach (var sectionData in sectionListCopy)
                            {
                                sectionListCopyList.Add(sectionData);
                            }
                            if (readSection == true)
                                sectionListCopyList.Add(sectioNo);
                            if (parentCheck == 1)
                                readSection = false;
                            sectionListCopy = new List<string>();
                            sectionListCopy = sectionListCopyList;
                            foundSection = true;
                            break;
                        }
                    }
                    else if (foundSection == true)
                    {
                        sectionListCopy.Add(sectioNo);
                    }
                    if (foundSection == true)
                        break;
                }
            }
            catch (Exception ex)
            {
                _Default.LogError("checkParaToFindSection", "", ex);
                throw;
            }
        }
            #endregion

            // get the complete section for the para
            public string getCompleteParaSection(string SectionNoCount, JArray ja2, Dictionary<int, Dictionary<int, string>> saveSectionNoAllFiles, Dictionary<Dictionary<int, string>, int> saveAllSection, string outputPara, string DefaultSectionName, JToken SectionName, string singleFileSectionTree)
            {
                try
                {
                    var jarrayVal = ja2;
                    var pageNo = (int)ja2[0]["pageNo"]; // get the pageno of output
                    var paraNo = (int)ja2[0]["paraNo"]; //  get the para number
                    var readNextPara = (int)ja2[0]["readNextPara"]; // get para
                    var sectionNo = ja2[0]["sectionVal"].ToString();
                    var sentenceMergeCount = (int)ja2[0]["sentenceMergeCount"];
                    var sectionNoreadPara = ja2[0]["sectionNoreadPara"].ToString();
                    var sectionNoreadAllPara = sectionNoreadPara.Split('|');

                    var sectionNoreadAllParaData = "";
                    var list = new List<string>(sectionNoreadAllPara);
                    if (sectionNoreadAllPara.Count() == 3)
                    {
                        sectionNoreadAllParaData = list.ElementAt(1);
                    }
                    else if (sectionNoreadAllPara.Count() == 2)
                    {
                        if (list.ElementAt(0).Length < 80)
                            sectionNoreadAllParaData = list.ElementAt(1);
                        else
                            sectionNoreadAllParaData = list.ElementAt(0);
                    }
                    else
                        sectionNoreadAllParaData = list.ElementAt(0);

                    var foundPara = "";
                    if (outputPara == "")
                        foundPara = ja2[0]["output"].ToString();
                    else
                        foundPara = outputPara;
                    var finalSectionOutput = "";
                    var getFirstLine = "";

                    var completeTree = JObject.Parse(singleFileSectionTree.ToString())["children"];

                    Dictionary<string, string> sectionList = new Dictionary<string, string>();
                    List<string> sectionListCopy = new List<string>();

                    //----------get section data---------------

                    var toFInd = foundPara.Trim();
                    var CheckNextChild = true;
                    for (int i = 0; i < completeTree.Count(); i++) // loop through complete tree
                    {
                        var child = completeTree[i]["children"]; // get the children
                        var childSection = completeTree[i]["section"].ToString(); // get the section no
                        var SectionNameData = "";
                        if (completeTree[i]["SectionName"].ToString() != "")
                            SectionNameData = completeTree[i]["SectionName"].ToString(); // get the section name
                        else
                            SectionNameData = null;
                        var childCount = child.Count();

                        CheckNextChild = true;

                        foreach (var item in child) // loop through all the children of the section
                        {
                            var subChild = item["children"];
                            var sectioNo = item["section"].ToString();
                            var subChildCount = subChild.Count();
                            var paraData = item["para"];
                            var parentCheck = (int)item["parentCheck"];
                            var readSection = true;

                            for (int j = 0; j < paraData.Count(); j++)
                            {
                                var paraVal = paraData[j].ToString().Trim();

                                if ((paraVal.StartsWith(sectionNoreadAllParaData.Trim()) | sectionNoreadAllParaData.Trim().StartsWith(paraVal) | paraVal.EndsWith(sectionNoreadAllParaData.Trim()) | sectionNoreadAllParaData.Trim().EndsWith(paraVal)) & paraVal.Length > 20)
                                {
                                    if (!sectionListCopy.Contains(sectioNo) & readSection == true & !sectionList.ContainsKey(sectioNo))
                                    {
                                        sectionList.Add(sectioNo, "sectionNo");
                                        if (parentCheck == 1)
                                        {
                                            CheckNextChild = false;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (subChildCount > 0 & CheckNextChild == true)
                            {
                                var sectionNoReturn = new List<string>();
                                var foundSection = false;
                                checkParaToFindSection(sectionList, sectionNoreadAllParaData, subChild, out sectionListCopy, out foundSection, out readSection);
                                if (sectionListCopy.Count > 0 & foundSection == true)
                                {
                                    List<string> sectionListCopyList = new List<string>();
                                    foreach (var sectionData in sectionListCopy)
                                    {
                                        sectionListCopyList.Add(sectionData);
                                    }
                                    if (readSection == true)
                                        sectionListCopyList.Add(sectioNo);
                                    sectionListCopy = new List<string>();
                                    sectionListCopy = sectionListCopyList;
                                    foundSection = true;
                                    CheckNextChild = false;
                                }
                            }
                            if (sectionListCopy.Count() > 0 & CheckNextChild == false)
                            {
                                sectionList = new Dictionary<string, string>();
                                foreach (var sectionData in sectionListCopy)
                                {
                                    if (!sectionList.ContainsKey(sectionData))
                                        sectionList.Add(sectionData, "SectionNo");
                                }
                                if (SectionNameData != null)
                                    sectionList.Add(SectionNameData, "sectionParent");
                                break;
                            }
                            else if (sectionList.Count() > 0)
                            {
                                if (SectionNameData != null)
                                    sectionList.Add(SectionNameData, "sectionParent");
                                CheckNextChild = false;
                                break;
                            }
                        }
                        if (CheckNextChild == false)
                            break;
                    }

                    var finalSectionVal = new Dictionary<string, string>();
                    finalSectionVal = sectionList;
                    if (finalSectionVal.Count > 0)
                    {
                        if ((finalSectionVal.ElementAt(finalSectionVal.Count - 1).Key != null | finalSectionVal.ElementAt(finalSectionVal.Count - 1).Key != "") & finalSectionVal.ElementAt(finalSectionVal.Count - 1).Value == "sectionParent")
                        {
                            getFirstLine = finalSectionVal.ElementAt(finalSectionVal.Count - 1).Key;
                            finalSectionVal.Remove(getFirstLine);
                        }
                        else
                            getFirstLine = null;

                        var totalSectionShow = 0;
                        if (SectionNoCount == "0")
                            totalSectionShow = finalSectionVal.Count;
                        else
                            totalSectionShow = Int32.Parse(SectionNoCount);
                        var sectionCount = 0;
                        for (int i = finalSectionVal.Count - 1; i >= 0; i--)
                        {
                            sectionCount++;
                            if (finalSectionVal.ElementAt(i).Key != null)
                            {
                                var sectionNoVal = finalSectionVal.ElementAt(i).Key.Trim();
                                if (finalSectionOutput == "")
                                    finalSectionOutput = sectionNoVal;
                                else
                                    finalSectionOutput = finalSectionOutput + " " + sectionNoVal;
                            }
                            if (sectionCount == totalSectionShow)
                                break;
                        }
                    }
                    else
                        getFirstLine = null;

                    var foundSectionName = false;
                    if (getFirstLine != null)
                    {
                        var toSearch = "";
                        Dictionary<int, string> regexDictionary = new Dictionary<int, string>();
                        regexDictionary.Add(1, "[\\s]*([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]*|\\d{0,2}[\\s]*)$"); //
                        regexDictionary.Add(2, "[\\s]*[\"]([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]*|\\d{0,2}[\\s]*)[\"]$"); //
                        regexDictionary.Add(3, "[\\s]*([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}|\\d{0,2})$"); //
                        regexDictionary.Add(4, "[\\s]*[\"]([a-zA-Z]{1}[\\s]*|\\d{0,2}[\\s]*)[\"]$"); //
                        regexDictionary.Add(5, "[\\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\\s]?"); //

                        foreach (var SecName in SectionName) // loop through all the main head sections 
                        {
                            if (foundSectionName == false)
                            {
                                foreach (var regexVal in regexDictionary)
                                {
                                    toSearch = SecName["keyword"].ToString();
                                    Regex regex = new Regex("^(?i)" + toSearch + regexVal.Value);
                                    var match = regex.Match(getFirstLine); // check if match found
                                    if (match.Success)
                                    {
                                        Regex regex1 = new Regex("^(?i)" + toSearch);
                                        var match1 = regex1.Match(getFirstLine); // check if match found
                                        var removeHeadSectionName = match.Value.Replace(match1.Value, "").Trim();
                                        var finalHeadSectionName = match1.Value + " " + removeHeadSectionName;
                                        foundSectionName = true;
                                        finalSectionOutput = finalHeadSectionName.Trim() + " " + finalSectionOutput;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (foundSectionName == false)
                        finalSectionOutput = DefaultSectionName + " " + finalSectionOutput;
                    return finalSectionOutput;

                }
                catch (Exception ex)
                {
                _Default.LogError("getCompleteParaSection", "", ex);
                throw;
                }
            }

            #region -------------------get the section no for SECTION(multiple paragraph)   (PENDING) ------------------------------------------------------
            //// get the complete section number for SECTION......
            public string sectionValSection(string section)
            {
                try
                {


                    var sectionVal = "";
                    Dictionary<int, string> regexDictionary = new Dictionary<int, string>();

                    regexDictionary.Add(1, @"^[\s]*(((?i)section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
                    regexDictionary.Add(2, @"^[\s]*(((?i)sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
                    regexDictionary.Add(3, @"^[\s]*((?i)section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
                    regexDictionary.Add(4, @"^[\s]*((?i)section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
                    regexDictionary.Add(5, @"^[\s]*(((?i)section)[\s]*[A-Z])\s");  // section A
                    regexDictionary.Add(6, @"^[\s]*(((?i)section)[\s]*[a-z])");  //      section a
                    regexDictionary.Add(7, @"^[\s]*(((?i)article)[\s]*[a-z])");  //      article a
                    regexDictionary.Add(8, @"^[\s]*(((?i)ARTICLE)[\s]*[A-Z])");  // ARTICLE A  
                    regexDictionary.Add(9, @"^[\s]*((?i)article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII
                    regexDictionary.Add(10, @"^[\s]*((?i)article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii
                    regexDictionary.Add(11, @"^[\s]*(((?i)article)\s\d)"); //   article 1,
                    regexDictionary.Add(12, @"^[\s]*(((?i)article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1
                    regexDictionary.Add(13, @"^(\d{1,2}\.(:\d+\.?)*)\s"); //    1.
                    regexDictionary.Add(14, @"^(\d{1,2}\.\d(?:\d+\.?)*)"); //    1.1 
                    regexDictionary.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
                    regexDictionary.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
                    regexDictionary.Add(17, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
                    regexDictionary.Add(18, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]{0,1}[.]\s"); //    XVII.
                    regexDictionary.Add(19, @"^[\s]*([A-Z][\s]{0,1}[.])\s");  // A.

                    for (int i = 0; i < regexDictionary.Count(); i++)
                    {
                        Regex regexForNextSection = new Regex(regexDictionary[i + 1]);
                        var matchForNextSection = regexForNextSection.Match(section); // check if match found
                        if (matchForNextSection.Success)
                        {
                            sectionVal = matchForNextSection.Value;
                            break;
                        }
                    }

                    return sectionVal;
                }
                catch (Exception ex)
                {
                _Default.LogError("sectionValSection", "", ex);
                throw;
                }
            }
            #endregion

            //---------------------------------------------------------Tree structure ----------------------------------------------------------------

            #region ------------- tree view of pdf---------------------------------------------------------
            // create tree for pdf
            public void createTree(Dictionary<Dictionary<int, string>, int> saveSection, Dictionary<Dictionary<int, string>, int> saveSectionWithsectionNo, Dictionary<Dictionary<int, string>, int> saveSectionWithRegex, List<string> sectionNameFound, out string finalJson)
            {
                try
                {

                    Node rootTop = new Node("ROOT", -1);
                    // tree is created section wise.... there will be one root node and the section will be the branches for tree
                    for (int i = 0; i < saveSection.Count(); i++) // loop through section and create tree for the section
                    {
                        int defaultLevel = 0;
                        var regexVal = "";
                        var sectionName = "";
                        var parentCheck = 0;
                        List<string> paraList = new List<string>();
                        Node root = new Node("ROOT", defaultLevel, paraList, regexVal, sectionName, parentCheck);
                        Stack<KeyValuePair<string, Node>> stack = new Stack<KeyValuePair<string, Node>>();
                        List<string> matchedKeys = new List<string>();
                        Node lastNode = null;

                        // get the details for the section
                        var sectionPara = saveSection.ElementAt(i).Key;
                        var sectionSectioNo = saveSectionWithsectionNo.ElementAt(i).Key;
                        var SectionRegex = saveSectionWithRegex.ElementAt(i).Key;
                        Dictionary<int, List<string>> combineSectionPara = new Dictionary<int, List<string>>();
                        Dictionary<int, string> combineSectionSectioNo = new Dictionary<int, string>();
                        Dictionary<int, string> combineSectionRegex = new Dictionary<int, string>();
                        List<int> parentSectionNo = new List<int>();

                        // this method reconstructs the para and checks the parent and child section for the tree process 
                        paraConstruction(sectionPara, sectionSectioNo, SectionRegex, out combineSectionPara, out combineSectionSectioNo, out combineSectionRegex, out parentSectionNo);


                        List<int> levelSave = new List<int>();
                        List<string> allSection = new List<string>();
                        List<string> allregex = new List<string>();
                        for (int j = 0; j < combineSectionPara.Count(); j++)
                        {
                            var para = combineSectionPara.ElementAt(j).Value;
                            var sectioNo = combineSectionSectioNo.ElementAt(j).Value;
                            var regex = combineSectionRegex.ElementAt(j).Value;
                            var parent = parentSectionNo.ElementAt(j);
                            allSection.Add(sectioNo);
                            allregex.Add(regex);

                            var rootSectionName = "";
                            if (sectionNameFound.Count == 0)
                                rootSectionName = "";
                            else
                                rootSectionName = sectionNameFound.ElementAt(i);

                            if (sectioNo == "" & combineSectionPara.Count() == 1)
                            {
                                root.Value = sectioNo;
                                root.Para = para;
                                root.SectionName = rootSectionName;
                                levelSave.Add(0);
                            }
                            else
                            {
                                if (j == 0 & sectioNo == "")
                                {
                                    root.Value = sectioNo;
                                    root.Para = para;
                                    root.SectionName = rootSectionName;
                                    levelSave.Add(0);

                                }
                                else
                                {
                                    var nodeValue = sectioNo;

                                    if (stack.Count == 0)
                                    {
                                        var child = new Node(sectioNo, root.Level + 1, para, regex, "", parent);
                                        levelSave.Add(root.Level + 1);
                                        root.Children.Add(child);
                                        lastNode = child;
                                        stack.Push(new KeyValuePair<string, Node>(regex, root));
                                    }
                                    else
                                    {
                                        bool differentSection = false;
                                        if (matchedKeys.Contains(regex) & differentSection == false)
                                        {
                                            var breakFlag = false;
                                            while (breakFlag == false)
                                            {
                                                if (stack.Count == 0)
                                                    break;
                                                var top = stack.Peek();
                                                if (top.Key == regex)
                                                {
                                                    var child = new Node(sectioNo, top.Value.Level + 1, para, regex, "", parent);
                                                    levelSave.Add(top.Value.Level + 1);
                                                    top.Value.Children.Add(child);

                                                    lastNode = child;
                                                    breakFlag = true;
                                                }
                                                else
                                                {
                                                    matchedKeys.Remove(top.Key);
                                                    stack.Pop();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (lastNode != null)
                                            {
                                                var child = new Node(sectioNo, lastNode.Level + 1, para, regex, "", parent);
                                                levelSave.Add(lastNode.Level + 1);
                                                lastNode.Children.Add(child);
                                                stack.Push(new KeyValuePair<string, Node>(regex, lastNode));

                                                lastNode = child;
                                            }
                                        }
                                    }
                                    if (matchedKeys.Contains(regex) == false)
                                        matchedKeys.Add(regex);
                                }
                            }
                        }


                        defaultLevel = 0;
                        regexVal = "";
                        sectionName = "";
                        parentCheck = 0;
                        paraList = new List<string>();
                        root = new Node("ROOT", defaultLevel, paraList, regexVal, sectionName, parentCheck);
                        stack = new Stack<KeyValuePair<string, Node>>();
                        matchedKeys = new List<string>();
                        lastNode = null;


                        treeCorrection(parentSectionNo, levelSave, combineSectionPara, combineSectionSectioNo, combineSectionRegex, out combineSectionSectioNo, out combineSectionRegex, out combineSectionPara, out parentSectionNo);

                        levelSave = new List<int>();
                        allSection = new List<string>();
                        allregex = new List<string>();
                        for (int j = 0; j < combineSectionPara.Count(); j++)
                        {
                            var para = combineSectionPara.ElementAt(j).Value;
                            var sectioNo = combineSectionSectioNo.ElementAt(j).Value;
                            var regex = combineSectionRegex.ElementAt(j).Value;
                            var parent = parentSectionNo.ElementAt(j);
                            allSection.Add(sectioNo);
                            allregex.Add(regex);

                            var rootSectionName = "";
                            if (sectionNameFound.Count == 0)
                                rootSectionName = "";
                            else
                                rootSectionName = sectionNameFound.ElementAt(i);

                            if (sectioNo == "" & combineSectionPara.Count() == 1)
                            {
                                root.Value = sectioNo;
                                root.Para = para;
                                root.SectionName = rootSectionName;
                                levelSave.Add(0);
                            }
                            else
                            {
                                if (j == 0 & sectioNo == "")
                                {
                                    root.Value = sectioNo;
                                    root.Para = para;
                                    root.SectionName = rootSectionName;
                                    levelSave.Add(0);

                                }
                                else
                                {
                                    var nodeValue = sectioNo;

                                    if (stack.Count == 0)
                                    {
                                        var child = new Node(sectioNo, root.Level + 1, para, regex, "", parent);
                                        levelSave.Add(root.Level + 1);
                                        root.Children.Add(child);
                                        lastNode = child;
                                        stack.Push(new KeyValuePair<string, Node>(regex, root));
                                    }
                                    else
                                    {
                                        bool differentSection = false;
                                        if (matchedKeys.Contains(regex) & differentSection == false)
                                        {
                                            var breakFlag = false;
                                            while (breakFlag == false)
                                            {
                                                if (stack.Count == 0)
                                                    break;
                                                var top = stack.Peek();
                                                if (top.Key == regex)
                                                {
                                                    var child = new Node(sectioNo, top.Value.Level + 1, para, regex, "", parent);
                                                    levelSave.Add(top.Value.Level + 1);
                                                    top.Value.Children.Add(child);

                                                    lastNode = child;
                                                    breakFlag = true;
                                                }
                                                else
                                                {
                                                    matchedKeys.Remove(top.Key);
                                                    stack.Pop();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (lastNode != null)
                                            {
                                                var child = new Node(sectioNo, lastNode.Level + 1, para, regex, "", parent);
                                                levelSave.Add(lastNode.Level + 1);
                                                lastNode.Children.Add(child);
                                                stack.Push(new KeyValuePair<string, Node>(regex, lastNode));

                                                lastNode = child;
                                            }
                                        }
                                    }
                                    if (matchedKeys.Contains(regex) == false)
                                        matchedKeys.Add(regex);
                                }
                            }
                        }


                        rootTop.Children.Add(root);

                    }
                    //finalJson = Newtonsoft.Json.JsonConvert.SerializeObject(rootTop);
                    finalJson = CreateJson(rootTop).ToString();
                    //testc(rootTop);

                }
                catch (Exception ex)
                {
                _Default.LogError("createTree", "", ex);
                throw;
                }
            }

            // combine all para within section 
            public void paraConstruction(Dictionary<int, string> sectionPara, Dictionary<int, string> sectionSectioNo, Dictionary<int, string> sectionRegex, out Dictionary<int, List<string>> combineSectionPara, out Dictionary<int, string> combineSectionSectioNo, out Dictionary<int, string> combineSectionRegex, out List<int> parentSectionNo)
            {
                try
                {

                    List<string> doubleSectionRegex = new List<string>();
                    doubleSectionRegex.Add(@"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)");
                    doubleSectionRegex.Add(@"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)");


                    combineSectionPara = new Dictionary<int, List<string>>();
                    combineSectionSectioNo = new Dictionary<int, string>();
                    combineSectionRegex = new Dictionary<int, string>();
                    parentSectionNo = new List<int>();
                    List<string> savepara = new List<string>();
                    var count = 1;
                    var lastRegex = "";
                    var lastSectioNo = "";
                    var lastParent = 0;
                    int parent = 0;
                    for (int i = 0; i < sectionPara.Count(); i++)
                    {
                        var para = sectionPara.ElementAt(i).Value;
                        var sectioNo = sectionSectioNo.ElementAt(i).Value;
                        var regex = sectionRegex.ElementAt(i).Value;
                        parentCheck(regex, out parent); // check it rhe section is parent or child

                        var countSectionRegexCount = 0;
                        foreach (var item in doubleSectionRegex) // loop through the regex
                        {
                            countSectionRegexCount++;
                            if (item == regex)// if the regex matches then find the correct correct section number for the para
                            {
                                bool doubleSection = false;
                                // if the regex matches means there are 2 section no assigned to the para .....so saperate the section no and save the section saperatly
                                saperateSection(item, sectioNo, out System.Collections.Generic.List<string> sectionList, out List<string> regexList, out doubleSection);
                                if (doubleSection == true)
                                {
                                    if (lastSectioNo != "")
                                    {
                                        combineSectionPara.Add(count, savepara);
                                        combineSectionSectioNo.Add(count, lastSectioNo);
                                        combineSectionRegex.Add(count, lastRegex);
                                        parentSectionNo.Add(lastParent);
                                        savepara = new List<string>();
                                        count++;
                                    }
                                    List<string> savepara1 = new List<string>();
                                    combineSectionPara.Add(count, savepara1);
                                    combineSectionSectioNo.Add(count, sectionList.ElementAt(0));
                                    combineSectionRegex.Add(count, regexList.ElementAt(0));
                                    parentSectionNo.Add(lastParent);
                                    if (countSectionRegexCount == 2)
                                        parentSectionNo.Add(1);
                                    else
                                        parentSectionNo.Add(0);
                                    count++;
                                    sectioNo = sectionList.ElementAt(1);
                                    regex = regexList.ElementAt(1);
                                    parent = 0;
                                }
                            }
                        }

                        if (sectioNo == null)
                        {
                            savepara.Add(para);
                        }
                        else
                        {
                            if (savepara.Count() > 0)
                            {
                                combineSectionPara.Add(count, savepara);
                                combineSectionSectioNo.Add(count, lastSectioNo);
                                combineSectionRegex.Add(count, lastRegex);
                                parentSectionNo.Add(lastParent);
                                count++;
                                savepara = new List<string>();
                                savepara.Add(para);
                            }
                            else
                            {
                                savepara.Add(para);
                            }
                            lastRegex = regex;
                            lastSectioNo = sectioNo;
                            lastParent = parent;
                        }
                    }
                    if (savepara.Count() > 0)
                    {
                        combineSectionPara.Add(count, savepara);
                        combineSectionSectioNo.Add(count, lastSectioNo);
                        combineSectionRegex.Add(count, lastRegex);
                        parentSectionNo.Add(lastParent);
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("paraConstruction", "", ex);
                throw;
                }
            }

            // case handle eg:- 1.1(a) OR 1. VII)
            public void saperateSection(string regex, string sectionNo, out List<string> sectionList, out List<string> regexList, out bool doubleSection)
            {
                try
                {


                    doubleSection = false;
                    Dictionary<int, string> regexDictionary = new Dictionary<int, string>(); // check the regex 
                    regexDictionary.Add(1, @"^[\s]*(?!0)([0-9]{1,2}[:])(?!\S)"); //   1:
                    regexDictionary.Add(30, @"^[\s]*(?!0)([0-9]{1,2}[.])(?!\S)"); //   1.
                    regexDictionary.Add(2, @"^[\s]*(?!0)([0-9]{1,2}[)])(?!\S)"); //   1)
                    regexDictionary.Add(3, @"^[\s]*(?!0)([0-9]{1,2}[]])(?!\S)"); //   1]
                    regexDictionary.Add(4, @"^[\s]*([[\\[][\s]*(?!0)([0-9]{1,2})[\s]*[]])(?!\S)"); //   [1]
                    regexDictionary.Add(5, @"^[\s]*([[(][\s]*(?!0)([0-9]{1,2})[\s]*[)])(?!\S)"); //   (1)

                    regexDictionary.Add(6, @"^[\s]*[(][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[)](?!\S)"); //    (xvii)
                    regexDictionary.Add(7, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[)](?!\S)"); //    xvii)
                    regexDictionary.Add(8, @"^[\s]*[[][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[]](?!\S)"); //    [xvii]
                    regexDictionary.Add(9, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[]](?!\S)"); //    xvii]
                    regexDictionary.Add(10, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[:](?!\S)"); //    xvii:
                    regexDictionary.Add(11, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.](?!\S)"); //    xvii.

                    regexDictionary.Add(12, @"^[\s]*[(][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[)](?!\S)"); //    (XVII)
                    regexDictionary.Add(13, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[)](?!\S)"); //    XVII)
                    regexDictionary.Add(14, @"^[\s]*[[][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[]](?!\S)"); //    [XVII]
                    regexDictionary.Add(15, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[]](?!\S)"); //    XVII]
                    regexDictionary.Add(16, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[:](?!\S)"); //    XVII:
                    regexDictionary.Add(17, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.](?!\S)"); //    XVII.

                    regexDictionary.Add(18, @"^[\s]*([a-z][\s]{0,1}[.])(?!\S)");  //  a.
                    regexDictionary.Add(19, @"^[\s]*([a-z][\s]{0,1}[:])(?!\S)");  //  a:
                    regexDictionary.Add(20, @"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)");  //    (a)
                    regexDictionary.Add(21, @"^[\s]*([a-z][\s]{0,1}[)])(?!\S)");  // a)
                    regexDictionary.Add(22, @"^[\s]*([a-z][\s]{0,1}[]])(?!\S)");  //     [a]
                    regexDictionary.Add(23, @"^[\s]*([[][\s]*[a-z][\s]{0,1}[]])(?!\S)");  //   a]

                    regexDictionary.Add(24, @"^[\s]*([A-Z][\s]{0,1}[.])(?!\S)");  // A.
                    regexDictionary.Add(25, @"^[\s]*([A-Z][:])(?!\S)");  // A:
                    regexDictionary.Add(26, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)");  // (A)
                    regexDictionary.Add(27, @"^[\s]*([A-Z][\s]{0,1}[)])(?!\S)");  // A)
                    regexDictionary.Add(28, @"^[\s]*([A-Z][\s]{0,1}[]])(?!\S)");  // [A]
                    regexDictionary.Add(29, @"^[\s]*([[][\s]*[A-Z][\s]{0,1}[]])(?!\S)");  // A]


                    sectionList = new List<string>();
                    regexList = new List<string>();

                    Regex regexCheckVal = new Regex("(\\d{1,2}\\.(\\d(?:\\d+\\.?)*)?)"); // check if it has section number
                    var match = regexCheckVal.Match(sectionNo); // check if match found
                    if (match.Success) // if found then replace it
                    {
                        sectionList.Add(match.Value.Trim());
                        regexList.Add(regex);
                        foreach (var item in regexDictionary)
                        {
                            var secondSectionNo = sectionNo.Replace(match.Value.Trim(), "").Trim();
                            Regex innerRegex = new Regex(item.Value); // check if it has section number
                            var innerMatch = innerRegex.Match(secondSectionNo); // check if match found
                            if (innerMatch.Success) // if found then replace it
                            {
                                sectionList.Add(secondSectionNo);
                                regexList.Add(item.Value);
                                doubleSection = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                _Default.LogError("saperateSection", "", ex);
                throw;
                }
            }

            // check if the section is parent/child
            public void parentCheck(string regex, out int parent)
            {
                try
                {

                    parent = 0;
                    Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
                    // NUMBERS
                    matchRegexNumeric.Add(1, @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)"); //    1.1  / 1.1 a)
                    matchRegexNumeric.Add(2, @"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}[.|:|•|-]?(?!\S)"); //    section 1
                    matchRegexNumeric.Add(3, @"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
                    matchRegexNumeric.Add(4, @"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}[.|:|•|-]?(?!\S)"); //   article 1
                    matchRegexNumeric.Add(5, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
                    matchRegexNumeric.Add(6, @"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.|:|•|-]?(?!\S)"); //    section xvii
                    matchRegexNumeric.Add(7, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}[.|:|•|-]?(?!\S)"); //    article xvii
                    matchRegexNumeric.Add(8, @"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.|:|•|-]?(?!\S)"); //    section XVII
                    matchRegexNumeric.Add(9, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}[.|:|•|-]?(?!\S)"); //    article XVII
                    matchRegexNumeric.Add(10, @"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}[.|:|•|-]?(?!\S)");  //      section a
                    matchRegexNumeric.Add(11, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}[.|:|•|-]?(?!\S)");  //      article a
                    matchRegexNumeric.Add(12, @"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}[.|:|•|-]?(?!\S)");  // section A
                    matchRegexNumeric.Add(13, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}[.|:|•|-]?(?!\S)");  // ARTICLE A


                    if (matchRegexNumeric.Any(s => s.Value == regex))
                    {
                        parent = 1;
                    }
                    //foreach (var item in matchRegexNumeric)
                    //{
                    //    if (regex == item.Value)
                    //    {
                    //        parent = 1;
                    //        break;
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                _Default.LogError("parentCheck", "", ex);
                throw;
                }
            }

            #endregion

            #region -------------remove unewanted symbol while section number correction ------------------------------------------
            // remove unwanted symbols
            public void symbolCorrectDictionaryFn(string section, out string updatedSection)
            {
                try
                {


                    updatedSection = section;
                    List<string> symbolCorrection = new List<string>(); // check the regex 
                    symbolCorrection.Add("{");
                    symbolCorrection.Add("}");

                    foreach (var item in symbolCorrection)
                    {
                        if (section.IndexOf(item) != -1)
                        {
                            updatedSection = section.Replace(item, "");
                        }
                    }
                }
                catch (Exception ex)
                {
                _Default.LogError("symbolCorrectDictionaryFn", "", ex);
                throw;
                }
            }
            #endregion

            #region --------correcting section if read wrong----------------------------------
            // correcting the section number and regex 
            public void sectionCorrectionCheck(Stack<string> stackRegex, string onlySectionNo, Stack<string> stackSection, out string correctSectionNo, out string correctRegex)
            {
                try
                {

                    correctSectionNo = "";
                    correctRegex = "";

                    Dictionary<string, string> checkPrevSectionLevel1 = new Dictionary<string, string>(); // check the words after section number
                    checkPrevSectionLevel1.Add("u|iv", "v");
                    checkPrevSectionLevel1.Add("w|ix", "x");
                    checkPrevSectionLevel1.Add("U|IV", "V");
                    checkPrevSectionLevel1.Add("W|IX", "X");
                    checkPrevSectionLevel1.Add("R|7", "S");
                    checkPrevSectionLevel1.Add("4", "S");
                    checkPrevSectionLevel1.Add("7|R", "8");
                    checkPrevSectionLevel1.Add("4|R", "5");
                    checkPrevSectionLevel1.Add("k|e", "l");
                    checkPrevSectionLevel1.Add("d|b", "e");
                    checkPrevSectionLevel1.Add("r|4", "s");
                    checkPrevSectionLevel1.Add("g|a", "h");
                    checkPrevSectionLevel1.Add("8|f", "9");
                    checkPrevSectionLevel1.Add("j", "i");
                    checkPrevSectionLevel1.Add("I|i|k", "1");
                    checkPrevSectionLevel1.Add("10", "11");

                    Dictionary<string, string> checkPrevSectionLevel2 = new Dictionary<string, string>(); // check the words after section number
                    checkPrevSectionLevel2.Add("R", "S");
                    checkPrevSectionLevel2.Add("4", "5");
                    checkPrevSectionLevel2.Add("7", "8");
                    checkPrevSectionLevel2.Add("W", "X");
                    checkPrevSectionLevel2.Add("IX", "X");
                    checkPrevSectionLevel2.Add("U", "V");
                    checkPrevSectionLevel2.Add("IV", "V");
                    checkPrevSectionLevel2.Add("w", "x");
                    checkPrevSectionLevel2.Add("ix", "x");
                    checkPrevSectionLevel2.Add("u", "v");
                    checkPrevSectionLevel2.Add("iv", "v");
                    checkPrevSectionLevel2.Add("k", "l");
                    checkPrevSectionLevel2.Add("e", "f");
                    checkPrevSectionLevel2.Add("d", "e");
                    checkPrevSectionLevel2.Add("b", "c");
                    checkPrevSectionLevel2.Add("r", "s");
                    checkPrevSectionLevel2.Add("g", "h");
                    checkPrevSectionLevel2.Add("a", "b");
                    checkPrevSectionLevel2.Add("f", "g");
                    checkPrevSectionLevel2.Add("8", "9");
                    checkPrevSectionLevel2.Add("i", "j");
                    checkPrevSectionLevel2.Add("I", "J");
                    checkPrevSectionLevel2.Add("10", "11");

                    Dictionary<string, string> assumption = new Dictionary<string, string>(); // check the words after section number
                    assumption.Add("9", "g");
                    assumption.Add("11", "ii");

                    Dictionary<string, string> assumptionRegex = new Dictionary<string, string>(); // check the words after section number
                    assumptionRegex.Add("g", "[a-z]");
                    assumptionRegex.Add("ii", "(ix|iv|v?i{0,3})");

                    var lastSectionNumber = stackSection.Peek();
                    var lastRegex = stackRegex.Peek();
                    var checkNext = true;
                    foreach (var item in checkPrevSectionLevel1) // loop through level 1
                    {
                        if (checkNext == false)
                            break;
                        if (item.Value == onlySectionNo) // if section number matches
                        {
                            var data = item.Key.Split('|'); // split the condition
                            foreach (var dataCheck in data) // loop through conditions
                            {
                                if (checkNext == false)
                                    break;
                                if (lastSectionNumber == dataCheck) // if matches with condition
                                {
                                    foreach (var level2 in checkPrevSectionLevel2) // loop through level 2
                                    {
                                        if (dataCheck == level2.Key)
                                        {
                                            correctSectionNo = level2.Value;
                                            correctRegex = lastRegex;
                                            checkNext = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (correctSectionNo == "")
                    {
                        var count = 0;
                        foreach (var item in assumption)
                        {
                            if (item.Key == onlySectionNo & lastRegex.IndexOf(assumptionRegex.ElementAt(count).Value) != -1)
                            {
                                correctSectionNo = item.Value;
                                correctRegex = lastRegex;
                            }
                            count++;
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("sectionCorrectionCheck", "", ex);
                throw;
                }
            }
            #endregion

            public void checkSpecialCharForSection(string onlySectionNo, Dictionary<int, string> combineSectionRegex, Dictionary<int, string> combineSectionSectioNo, List<int> levelSave, int j, out string sectionNo, out string regex)
            {
                try
                {

                    sectionNo = "";
                    regex = "";
                    var levelSaveCopy = levelSave;
                    var predictedSectionNo = new Dictionary<string, string>();
                    var currentLevel = levelSave.ElementAt(j);
                    //var set = new List<int>();
                    var start = j + 1;
                    List<int> set = new List<int>(levelSave);
                    List<string> sectionSet = combineSectionSectioNo.Values.ToList();
                    List<string> regexSet = combineSectionRegex.Values.ToList();
                    var startIndex = j;
                    var endIndex = 0;
                    var hasChild = false;
                    List<string> saveSpecialChar = new List<string>();
                    for (int i = start; i < levelSave.Count(); i++)
                    {
                        var level = levelSave.ElementAt(i);
                        if (level == currentLevel)
                        {
                            var value = combineSectionSectioNo.ElementAt(i).Value.Trim();
                            foreach (var item in _CheckWord.specialChar) // get only the section number 
                            {
                                Regex regexVal = new Regex(item);
                                var match = regexVal.Match(value); // check if match found
                                if (match.Success)
                                {
                                    value = value.Replace(match.Value, "");
                                    saveSpecialChar.Add(match.Value);
                                }
                            }
                            if (value != onlySectionNo)
                            {
                                hasChild = true;
                            }
                            break;
                        }
                        if (level < currentLevel) // parent found
                        {
                            endIndex = i;
                            break;
                        }
                        if (level == currentLevel + 1 && i == start) // check for "2"
                        {
                            var nextSection = combineSectionSectioNo.ElementAt(start).Value.Trim();
                            if ((nextSection.IndexOf("2") == 0 | nextSection.IndexOf("2") == 1) & onlySectionNo == "I")
                            {
                                sectionNo = "1";
                                regex = combineSectionRegex.ElementAt(start).Value;
                                hasChild = true;
                                break;
                            }
                            else if (nextSection.IndexOf("ii") != -1 & onlySectionNo == "i")
                            {
                                sectionNo = "i";
                                regex = combineSectionRegex.ElementAt(start).Value;
                                hasChild = true;
                                break;
                            }
                        }
                    }
                    if (saveSpecialChar.Count() > 0 & onlySectionNo == "I" & (j == 0 || combineSectionSectioNo.ElementAt(j - 1).Value.Trim() == ""))
                    {
                        if (saveSpecialChar.Count() == 1)
                        {
                            sectionNo = "1";
                            regex = "^[\\s]*(?!0)([0-9]{1,2}[" + saveSpecialChar.ElementAt(0) + "])(?!\\S)"; //   1:;

                        }
                        else if (saveSpecialChar.Count() == 2)
                        {
                            sectionNo = "1";
                            regex = "^[\\s]*([\\" + saveSpecialChar.ElementAt(0) + "[\\s]*(?!0)([0-9]{1,2})[\\s]*[" + saveSpecialChar.ElementAt(1) + "])(?!\\S)"; //   [1];
                        }
                    }
                    if (hasChild != true)
                    {
                        if (endIndex != 0)
                        {
                            set.RemoveRange(endIndex + 1, levelSave.Count() - (endIndex + 1));
                            sectionSet.RemoveRange(endIndex + 1, levelSave.Count() - (endIndex + 1));
                            regexSet.RemoveRange(endIndex + 1, levelSave.Count() - (endIndex + 1));
                        }

                        List<int> reverseSet = new List<int>(set);
                        reverseSet.Reverse();
                        start = set.Count() - j;
                        endIndex = 0;
                        for (int i = start; i < set.Count(); i++)
                        {
                            var level = reverseSet.ElementAt(i);
                            if (level < currentLevel)
                            {
                                endIndex = i;
                                break;
                            }
                        }
                        if (endIndex == 0)
                        {
                            set.RemoveRange(0, j - 1);
                            sectionSet.RemoveRange(0, j - 1);
                            regexSet.RemoveRange(0, j - 1);
                        }
                        else
                        {
                            set.Reverse(); sectionSet.Reverse(); regexSet.Reverse();
                            set.RemoveRange(endIndex + 1, set.Count() - (endIndex + 1));
                            sectionSet.RemoveRange(endIndex + 1, sectionSet.Count() - (endIndex + 1));
                            regexSet.RemoveRange(endIndex + 1, regexSet.Count() - (endIndex + 1));
                            set.Reverse(); sectionSet.Reverse(); regexSet.Reverse();
                        }
                        if (set.ElementAt(0) == set.ElementAt(set.Count() - 1))
                        {
                            var sectionValPrev = sectionSet.ElementAt(0);
                            var sectionValNext = sectionSet.ElementAt(set.Count() - 1);

                            foreach (var item in _CheckWord.specialChar) // get only the section number 
                            {
                                Regex regexCheck = new Regex("(?i)" + item);
                                var match = regexCheck.Match(sectionValPrev); // check if match found
                                if (match.Success)
                                {
                                    sectionValPrev = sectionValPrev.Replace(match.Value, "").Trim();
                                    sectionValNext = sectionValNext.Replace(match.Value, "").Trim();
                                }
                            }

                            if (sectionValPrev == "d" | sectionValPrev == "e" & sectionValNext == "g" | sectionValNext == "h")
                            {
                                sectionNo = "f";
                                regex = regexSet.ElementAt(0);
                            }
                            else if (sectionValPrev == "j" | sectionValPrev == "k" | sectionValPrev == "i" & sectionValNext == "m" | sectionValNext == "n")
                            {
                                sectionNo = "l";
                                regex = regexSet.ElementAt(0);
                            }
                            else if (sectionValPrev == "H")
                            {
                                sectionNo = "I";
                                regex = regexSet.ElementAt(0);
                            }
                        }
                        else
                        {
                            var sectionValPrev = sectionSet.ElementAt(0);
                            foreach (var item in _CheckWord.specialChar) // get only the section number 
                            {
                                Regex regexCheck = new Regex("(?i)" + item);
                                var match = regexCheck.Match(sectionValPrev); // check if match found
                                if (match.Success)
                                {
                                    sectionValPrev = sectionValPrev.Replace(match.Value, "").Trim();
                                }
                            }
                            if (sectionValPrev == "c" | sectionValPrev == "d" | sectionValPrev == "e")
                            {
                                sectionNo = "f";
                                regex = regexSet.ElementAt(0);
                            }
                            else if (sectionValPrev == "j" | sectionValPrev == "k" | sectionValPrev == "i")
                            {
                                sectionNo = "l";
                                regex = regexSet.ElementAt(0);
                            }
                            else if (sectionValPrev == "H")
                            {
                                sectionNo = "I";
                                regex = regexSet.ElementAt(0);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("checkSpecialCharForSection", "", ex);
                throw;
                }
            }

            public void treeCorrection(List<int> parentSectionNo, List<int> levelSave, Dictionary<int, List<string>> combineSectionPara, Dictionary<int, string> combineSectionSectioNo, Dictionary<int, string> combineSectionRegex, out Dictionary<int, string> outputSection, out Dictionary<int, string> outputRegex, out Dictionary<int, List<string>> outputPara, out List<int> outputParent)
            {
                try
                {



                    outputRegex = new Dictionary<int, string>();
                    outputSection = new Dictionary<int, string>();
                    outputPara = new Dictionary<int, List<string>>();
                    outputParent = new List<int>();
                    Dictionary<int, List<string>> combineSectionParaCopy = new Dictionary<int, List<string>>();
                    Dictionary<int, string> combineSectionSectioNoCopy = new Dictionary<int, string>();
                    Dictionary<int, string> combineSectionRegexCopy = new Dictionary<int, string>();
                    Stack<string> stackRegex = new Stack<string>();
                    Stack<int> stackSectionIndex = new Stack<int>();
                    Stack<string> stackSection = new Stack<string>();
                    Stack<List<string>> stackpara = new Stack<List<string>>();
                    List<List<string>> matchType = new List<List<string>>();
                    List<string> matchedRegex = new List<string>();
                    List<List<string>> savePara = new List<List<string>>();

                    var paraEntered = 0;

                    for (int i = 0; i < combineSectionPara.Count(); i++) // list of all paragraphs 
                    {
                        if (combineSectionSectioNo[i + 1] != "") // check if only one value there or not
                        { // if more then one paragraph there
                            var sectioNo = combineSectionSectioNo[i + 1]; // get the section number
                            var regexVal = combineSectionRegex[i + 1]; // get the regex for section number
                            var paraData = combineSectionPara[i + 1]; // get the para associated with that section number


                            if ((sectioNo != null | sectioNo != "") & i == 0)
                            {
                                combineSectionParaCopy.Add(paraEntered, paraData); // save paragraph
                                combineSectionSectioNoCopy.Add(paraEntered, sectioNo); // save section number
                                combineSectionRegexCopy.Add(paraEntered, regexVal); // save regex 
                                outputParent.Add(1);
                                paraEntered++; // increment count
                                if (stackpara.Count() > 0)
                                    stackpara.Pop(); // pop para 
                                stackpara.Push(paraData); // add new para
                                continue;
                            }

                            var onlySectionNo = "";
                            var replacedData = "";
                            var sectionNoCopy = sectioNo; // copy of section number
                            symbolCorrectDictionaryFn(sectionNoCopy, out sectionNoCopy);
                            foreach (var item in _CheckWord.specialChar) // get only the section number 
                            {
                                Regex regex = new Regex(item);
                                var match = regex.Match(sectionNoCopy); // check if match found
                                if (match.Success)
                                {
                                    sectionNoCopy = sectionNoCopy.Replace(match.Value, "");
                                    if (replacedData == "")
                                        replacedData = item.Replace("(?!\\S)", "").Replace("\\", "");
                                    else
                                        replacedData = replacedData + "|" + item.Replace("(?!\\S)", "").Replace("\\", "");
                                }
                            }
                            onlySectionNo = sectionNoCopy.Trim();

                            var regexValue = 0;
                            //foreach (var item in _regex.treeCorrectionRegex) // loop through all the regex and get the matchtype value
                            //{
                            //    if (item.Key == regexVal)
                            //    {
                            //        regexValue = item.Value; // value of match type
                            //        break;
                            //    }
                            //}
                            //convert linq query
                            var checkvalue = _regex.treeCorrectionRegex.Where(s => s.Key == regexVal).FirstOrDefault();
                            if (checkvalue.Value != null)
                                regexValue = checkvalue.Value;

                        // if section / article is there..........or 1.1 ... continue
                        Regex regexSectionAndArticleMatch = new Regex("(?i)(section|article)");
                        if (_regex.regexNotToCheckFn.Contains(regexVal) | regexSectionAndArticleMatch.Match(sectioNo).Success)
                        {
                            combineSectionParaCopy.Add(paraEntered, paraData); // save paragraph
                            combineSectionSectioNoCopy.Add(paraEntered, sectioNo); // save section number
                            combineSectionRegexCopy.Add(paraEntered, regexVal); // save regex 
                            outputParent.Add(1);
                            paraEntered++; // increment count
                            if (stackpara.Count() > 0)
                                stackpara.Pop(); // pop para 
                            stackpara.Push(paraData); // add new para

                            if (stackRegex.Count() > 0)
                            {
                                stackRegex.Pop();
                                stackSection.Pop();
                                matchedRegex.RemoveAt(matchedRegex.Count() - 1);
                                matchType.RemoveAt(matchType.Count() - 1);
                                stackSectionIndex.Pop();
                            }
                            continue;
                        }

                        // coorect section
                        var correctSectionNo = "";
                        var correctRegex = "";
                        foreach (var item in _CheckWord.wrongReadCharList)
                        {
                            if (item == onlySectionNo && stackRegex.Count() > 0)
                            {
                                sectionCorrectionCheck(stackRegex, onlySectionNo, stackSection, out correctSectionNo, out correctRegex);
                                if (correctSectionNo != "" & correctRegex != "")
                                {
                                    sectioNo = correctSectionNo;
                                    regexVal = correctRegex;
                                    onlySectionNo = correctSectionNo;
                                    foreach (var itemSet in _regex.treeCorrectionRegex) // loop through all the regex and get the matchtype value
                                    {
                                        if (itemSet.Key == regexVal)
                                        {
                                            regexValue = itemSet.Value; // value of match type
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }

                        // loop for "I" and "i"
                        if (sectioNo.IndexOf("I") != -1 | onlySectionNo == "i")
                        {
                            var newSection = "";
                            var newRegex = "";
                            checkSpecialCharForSection(onlySectionNo, combineSectionRegex, combineSectionSectioNo, levelSave, i, out newSection, out newRegex);
                            if (newSection != "")
                            {
                                sectioNo = newSection;
                                regexVal = newRegex;
                                onlySectionNo = newSection;
                                foreach (var item in _regex.treeCorrectionRegex) // loop through all the regex and get the matchtype value
                                {
                                    if (item.Key == regexVal)
                                    {
                                        regexValue = item.Value; // value of match type
                                        break;
                                    }
                                }
                            }
                        }
                            
                        if (stackRegex.Count() == 0 & regexVal != "") // first section no in stack
                        {
                            if (regexValue != 0)
                            {
                                var listName = regexMatch()[regexValue]; // get the list from match type
                                var newSectionFound = new List<string>();
                                for (int u = 0; u < listName.Count(); u++) // loop throgh list till get the index
                                {
                                    if (listName.ElementAt(u) == onlySectionNo & u == 0) // check if the index is zero for section
                                    {
                                        // if yes save the values in stack
                                        stackRegex.Push(regexVal); // push regex in stack 
                                        stackSection.Push(onlySectionNo); // save section number
                                        matchedRegex.Add(regexVal); // save regex
                                        matchType.Add(listName.ToList()); // save list
                                        stackSectionIndex.Push(0); // save index of section
                                        if (stackpara.Count() > 0)
                                            stackpara.Pop(); // pop para 
                                        stackpara.Push(paraData); // add new para
                                                                    //-----------------
                                        combineSectionParaCopy.Add(paraEntered, stackpara.Peek()); // save paragraph
                                        combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek()); // save section number
                                        combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek()); // save regex 
                                        outputParent.Add(0);
                                        paraEntered++; // increment count
                                                        //------------------
                                    }
                                    else // if not the first index of section ...then search for all the section above it
                                    {
                                        var lastMatchType = listName.ToList();
                                        var index = lastMatchType.IndexOf(onlySectionNo);
                                        var getAllSectionInRange = new List<string>();
                                        getAllSectionInRange = lastMatchType.GetRange(0, index);

                                        var getTheParaToCheck = stackpara.Peek();
                                        if (getTheParaToCheck.Count() > 1)
                                        {
                                            foreach (var item in getAllSectionInRange)
                                            {
                                                List<string> toFind = new List<string>();
                                                toFind.Add(item);
                                                Dictionary<string, List<string>> paraList = new Dictionary<string, List<string>>();
                                                checkInParaForSection(replacedData, combineSectionParaCopy, regexValue, regexVal, getTheParaToCheck, toFind, out paraList);

                                                if (paraList.Count() > 1) // if section number found  
                                                {
                                                    combineSectionParaCopy[paraEntered - 1] = paraList.ElementAt(0).Value;
                                                    paraList.Remove(paraList.Keys.First());
                                                    for (int p = 0; p < paraList.Count(); p++)
                                                    {
                                                        ////-----------------
                                                        combineSectionParaCopy.Add(paraEntered, paraList.ElementAt(p).Value);// save paragraph
                                                        combineSectionSectioNoCopy.Add(paraEntered, (paraList.ElementAt(p).Key).ToString());// save section number
                                                        combineSectionRegexCopy.Add(paraEntered, regexVal);// save regex 
                                                        outputParent.Add(0);
                                                        paraEntered++;// increment count
                                                                        ////-----------------
                                                        stackpara.Pop();
                                                        stackpara.Push(paraList.ElementAt(p).Value);
                                                    }
                                                }

                                                getTheParaToCheck = stackpara.Peek();
                                            }
                                        }
                                        stackRegex.Push(regexVal);
                                        stackSection.Push(onlySectionNo);
                                        matchedRegex.Add(regexVal);
                                        matchType.Add(listName.ToList());
                                        stackSectionIndex.Push(index);
                                        stackpara.Pop();
                                        stackpara.Push(paraData);
                                        ////-----------------
                                        combineSectionParaCopy.Add(paraEntered, stackpara.Peek());// save paragraph
                                        combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());// save section number
                                        combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());// save regex 
                                        outputParent.Add(0);
                                        paraEntered++;// increment count
                                                        //--------------------
                                    }
                                    break;
                                }
                            }
                        }
                        else // not first section 
                        {
                            if (matchedRegex.Contains(regexVal)) // if the stack has that value (sibling found)
                            {
                                var lastSectionVal = stackRegex.Peek(); // last section number in stack
                                if (lastSectionVal == regexVal) // if matches
                                {
                                    var lastMatchType = matchType.ElementAt(matchType.Count - 1); // get the 
                                    var getTheCurrentSectionNo = "";
                                    if (lastMatchType.Count() >= stackSectionIndex.Peek() + 1)
                                    {
                                        var nextIndex = stackSectionIndex.Peek() + 1; // next index to search

                                        int index = lastMatchType.IndexOf(onlySectionNo);
                                        if (stackSectionIndex.Peek() + 1 > index)
                                        {
                                            stackSectionIndex.Pop();
                                            stackSectionIndex.Push(0);
                                        }
                                        if (stackSectionIndex.Peek() + 1 > lastMatchType.Count() - 1)
                                        {
                                            var currentIndexVal = lastMatchType.IndexOf(onlySectionNo);
                                            stackSectionIndex.Pop();
                                            stackSectionIndex.Push(currentIndexVal);
                                            nextIndex = currentIndexVal;
                                            getTheCurrentSectionNo = lastMatchType.ElementAt(currentIndexVal); // section section number to search
                                        }
                                        else
                                            getTheCurrentSectionNo = lastMatchType.ElementAt(stackSectionIndex.Peek() + 1); // section section number to search
                                        if (getTheCurrentSectionNo == onlySectionNo || index - (stackSectionIndex.Peek() + 1) < 0) // check if both section matches then save the section number 
                                        {
                                            stackSection.Pop();
                                            stackSection.Push(onlySectionNo);
                                            stackSectionIndex.Pop();
                                            stackSectionIndex.Push(nextIndex);
                                            stackpara.Pop();
                                            stackpara.Push(paraData);
                                            ////-----------------
                                            combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                            combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                            combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                            outputParent.Add(0);
                                            paraEntered++;
                                            ////-----------------
                                            //matchType.RemoveAt(matchType.Count()-1);
                                        }
                                        else
                                        {
                                            index = lastMatchType.IndexOf(onlySectionNo);
                                            var getAllSectionInRange = lastMatchType.GetRange(stackSectionIndex.Peek() + 1, index - (stackSectionIndex.Peek() + 1));

                                            var getTheParaToCheck = stackpara.Peek();
                                            if (getTheParaToCheck.Count() > 1)
                                            {
                                                foreach (var item in getAllSectionInRange)
                                                {
                                                    List<string> toFind = new List<string>();
                                                    toFind.Add(item);
                                                    Dictionary<string, List<string>> paraList = new Dictionary<string, List<string>>();
                                                    checkInParaForSection(replacedData, combineSectionParaCopy, regexValue, regexVal, getTheParaToCheck, toFind, out paraList);

                                                    if (paraList.Count() > 1) // if section number found  
                                                    {
                                                        combineSectionParaCopy[paraEntered - 1] = paraList.ElementAt(0).Value;
                                                        paraList.Remove(paraList.Keys.First());
                                                        for (int p = 0; p < paraList.Count(); p++)
                                                        {
                                                            ////-----------------
                                                            combineSectionParaCopy.Add(paraEntered, paraList.ElementAt(p).Value);// save
                                                            combineSectionSectioNoCopy.Add(paraEntered, (paraList.ElementAt(p).Key).ToString());// save section number
                                                            combineSectionRegexCopy.Add(paraEntered, regexVal);// save regex 
                                                            outputParent.Add(0);
                                                            paraEntered++;// increment count
                                                                            ////-----------------
                                                            stackpara.Pop();
                                                            stackpara.Push(paraList.ElementAt(p).Value);
                                                        }
                                                    }

                                                    getTheParaToCheck = stackpara.Peek();
                                                }
                                            }
                                            stackSection.Pop();
                                            stackSection.Push(onlySectionNo);
                                            stackSectionIndex.Pop();
                                            stackSectionIndex.Push(index);
                                            stackpara.Pop();
                                            stackpara.Push(paraData);
                                            combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                            combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                            combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                            outputParent.Add(0);
                                            paraEntered++;
                                            //break;
                                        }
                                    }
                                }
                                else // check next value in stack
                                {
                                    var checkNextRegex = true;
                                    while (checkNextRegex == true)
                                    {
                                        stackRegex.Pop();
                                        matchedRegex.RemoveAt(matchedRegex.Count() - 1);
                                        matchType.RemoveAt(matchType.Count() - 1);
                                        lastSectionVal = stackRegex.Peek();
                                        stackSectionIndex.Pop();
                                        stackSection.Pop();

                                        if (lastSectionVal == regexVal)
                                        {
                                            var lastMatchType = matchType.ElementAt(matchType.Count - 1);
                                            var getTheCurrentSectionNo = "";
                                            if (lastMatchType.Count() < stackSectionIndex.Peek() + 1)
                                            {
                                                stackSectionIndex.Pop();
                                                stackSectionIndex.Push(0);
                                            }
                                            if (lastMatchType.Count() >= stackSectionIndex.Peek() + 1)
                                            {
                                                var nextIndex = stackSectionIndex.Peek() + 1;
                                                if (stackSectionIndex.Peek() + 1 > lastMatchType.Count() - 1)
                                                {
                                                    var currentIndexVal = lastMatchType.IndexOf(onlySectionNo);
                                                    stackSectionIndex.Pop();
                                                    stackSectionIndex.Push(currentIndexVal);
                                                    nextIndex = currentIndexVal;
                                                    getTheCurrentSectionNo = lastMatchType.ElementAt(currentIndexVal); // section section number to search
                                                }
                                                else
                                                    getTheCurrentSectionNo = lastMatchType.ElementAt(stackSectionIndex.Peek() + 1); // section section number to search
                                                if (getTheCurrentSectionNo == onlySectionNo)
                                                {
                                                    stackSection.Pop();
                                                    stackSection.Push(onlySectionNo);
                                                    stackSectionIndex.Pop();
                                                    stackSectionIndex.Push(nextIndex);
                                                    stackpara.Pop();
                                                    stackpara.Push(paraData);
                                                    ////-----------------
                                                    combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                                    combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                                    combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                                    outputParent.Add(0);
                                                    paraEntered++;
                                                    ////-----------------
                                                    break;
                                                }
                                                else
                                                {
                                                    var index = lastMatchType.IndexOf(onlySectionNo);
                                                    var getAllSectionInRange = new List<string>();
                                                    if (stackSectionIndex.Peek() + 1 > index)
                                                    {
                                                        stackpara.Pop();
                                                        if (paraData.Count() == 0)
                                                        {
                                                            stackpara.Push(null);
                                                            combineSectionParaCopy.Add(paraEntered, null);
                                                        }
                                                        else
                                                        {
                                                            stackpara.Push(paraData);
                                                            combineSectionParaCopy.Add(paraEntered, paraData);
                                                        }


                                                        combineSectionSectioNoCopy.Add(paraEntered, sectioNo);
                                                        combineSectionRegexCopy.Add(paraEntered, regexVal);
                                                        outputParent.Add(0);
                                                        paraEntered++;
                                                        stackSection.Pop();
                                                        stackSection.Push(sectioNo);
                                                        stackSectionIndex.Pop();
                                                        stackSectionIndex.Push(nextIndex + 1);

                                                        stackpara.Push(paraData);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        getAllSectionInRange = lastMatchType.GetRange(stackSectionIndex.Peek() + 1, index - (stackSectionIndex.Peek() + 1));
                                                    }

                                                    var getTheParaToCheck = stackpara.Peek();
                                                    if (getTheParaToCheck.Count() > 1)
                                                    {
                                                        foreach (var item in getAllSectionInRange)
                                                        {
                                                            List<string> toFind = new List<string>();
                                                            toFind.Add(item);
                                                            Dictionary<string, List<string>> paraList = new Dictionary<string, List<string>>();
                                                            checkInParaForSection(replacedData, combineSectionParaCopy, regexValue, regexVal, getTheParaToCheck, toFind, out paraList);

                                                            if (paraList.Count() > 1) // if section number found  
                                                            {
                                                                combineSectionParaCopy[paraEntered - 1] = paraList.ElementAt(0).Value;
                                                                paraList.Remove(paraList.Keys.First());
                                                                for (int p = 0; p < paraList.Count(); p++)
                                                                {
                                                                    ////-----------------
                                                                    combineSectionParaCopy.Add(paraEntered, paraList.ElementAt(p).Value);// save paragraph
                                                                    combineSectionSectioNoCopy.Add(paraEntered, (paraList.ElementAt(p).Key).ToString());// save section number
                                                                    combineSectionRegexCopy.Add(paraEntered, regexVal);// save regex 
                                                                    outputParent.Add(0);
                                                                    paraEntered++;// increment count
                                                                                    ////-----------------
                                                                    stackpara.Pop();
                                                                    stackpara.Push(paraList.ElementAt(p).Value);
                                                                }
                                                            }

                                                            getTheParaToCheck = stackpara.Peek();
                                                        }
                                                    }
                                                    stackSection.Pop();
                                                    stackSection.Push(onlySectionNo);
                                                    stackSectionIndex.Pop();
                                                    stackSectionIndex.Push(nextIndex + 1);
                                                    stackpara.Pop();
                                                    stackpara.Push(paraData);
                                                    combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                                    combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                                    combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                                    outputParent.Add(0);
                                                    paraEntered++;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else // stack dont have that value (child found)
                            {
                                var listName = regexMatch()[regexValue]; // get the list from match type
                                var newSectionFound = new List<string>();
                                for (int u = 0; u < listName.Count(); u++) // loop throgh list ti get the index
                                {
                                    if (listName.ElementAt(u) == onlySectionNo & u == 0)
                                    {
                                        // if yes save the values in stack
                                        stackRegex.Push(regexVal);
                                        stackSection.Push(onlySectionNo);
                                        matchedRegex.Add(regexVal);
                                        matchType.Add(listName.ToList());
                                        stackSectionIndex.Push(0);
                                        stackpara.Pop();
                                        stackpara.Push(paraData);
                                        ////-----------------
                                        combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                        combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                        combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                        outputParent.Add(0);
                                        paraEntered++;
                                        ////-----------------
                                    }
                                    else
                                    {
                                        var lastMatchType = listName.ToList();
                                        var index = lastMatchType.IndexOf(onlySectionNo);
                                        var getAllSectionInRange = new List<string>();
                                        getAllSectionInRange = lastMatchType.GetRange(0, index);

                                        var getTheParaToCheck = stackpara.Peek();
                                        if (getTheParaToCheck.Count() > 1)
                                        {
                                            foreach (var item in getAllSectionInRange)
                                            {
                                                List<string> toFind = new List<string>();
                                                toFind.Add(item);
                                                Dictionary<string, List<string>> paraList = new Dictionary<string, List<string>>();
                                                checkInParaForSection(replacedData, combineSectionParaCopy, regexValue, regexVal, getTheParaToCheck, toFind, out paraList);

                                                if (paraList.Count() > 1) // if section number found  
                                                {
                                                    combineSectionParaCopy[paraEntered - 1] = paraList.ElementAt(0).Value;
                                                    paraList.Remove(paraList.Keys.First());
                                                    for (int p = 0; p < paraList.Count(); p++)
                                                    {
                                                        ////-----------------
                                                        combineSectionParaCopy.Add(paraEntered, paraList.ElementAt(p).Value);// save paragraph
                                                        combineSectionSectioNoCopy.Add(paraEntered, (paraList.ElementAt(p).Key).ToString());// save section number
                                                        combineSectionRegexCopy.Add(paraEntered, regexVal);// save regex 
                                                        outputParent.Add(0);
                                                        paraEntered++;// increment count
                                                                        ////-----------------
                                                        stackpara.Pop();
                                                        stackpara.Push(paraList.ElementAt(p).Value);
                                                    }
                                                }

                                                getTheParaToCheck = stackpara.Peek();
                                            }
                                        }
                                        stackRegex.Push(regexVal);
                                        stackSection.Push(onlySectionNo);
                                        matchedRegex.Add(regexVal);
                                        matchType.Add(listName.ToList());
                                        stackSectionIndex.Push(index);
                                        stackpara.Pop();
                                        stackpara.Push(paraData);
                                        ////-----------------
                                        combineSectionParaCopy.Add(paraEntered, stackpara.Peek());
                                        combineSectionSectioNoCopy.Add(paraEntered, stackSection.Peek());
                                        combineSectionRegexCopy.Add(paraEntered, stackRegex.Peek());
                                        outputParent.Add(0);
                                        paraEntered++;
                                        ////-----------------
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        combineSectionParaCopy.Add(paraEntered, combineSectionPara[i + 1]); // add paragraph in the list
                        combineSectionSectioNoCopy.Add(paraEntered, "");// add section number in the list
                        combineSectionRegexCopy.Add(paraEntered, "");// add regex in the list
                        outputParent.Add(0);
                        paraEntered++; // increment the count
                        stackpara.Push(combineSectionPara[i + 1]);// push paragraph in the stack
                    }

                    }
                    outputSection = combineSectionSectioNoCopy;
                    outputPara = combineSectionParaCopy;
                    outputRegex = combineSectionRegexCopy;
                }
                catch (Exception ex)
                {
                _Default.LogError("treeCorrection", "", ex);
                throw;
                }
            }

            #region -----------find section inside para----------------------------------------
            // check para for above sections
            public void checkInParaForSection(string replacedData, Dictionary<int, List<string>> combineSectionParaCopy, int regexValue, string regexVal, List<string> getTheParaToCheck, List<string> getAllSectionInRange, out Dictionary<string, List<string>> paraList)
            {
                try
                {

                    Dictionary<string, List<string>> saveNewSection = new Dictionary<string, List<string>>();
                    paraList = new Dictionary<string, List<string>>();
                    var finalSectionFound = "";
                    var finalSectionFoundIndex = 0;
                    var lastStringSet = new List<string>();
                    lastStringSet = combineSectionParaCopy.Values.Last();

                    Dictionary<int, string> newRegex = new Dictionary<int, string>();
                    newRegex.Add(1, @"(?!0)([0-9]{1,2})");
                    newRegex.Add(2, @"(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}");
                    newRegex.Add(3, @"(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}");
                    newRegex.Add(4, @"([a-z])");
                    newRegex.Add(5, @"([A-Z])");

                    Dictionary<string, string> replacedDataConditions = new Dictionary<string, string>();
                    replacedDataConditions.Add(".", ".|,");
                    replacedDataConditions.Add(")", ">|\\]|\\)");
                    replacedDataConditions.Add("]", "\\)|>|\\]");
                    replacedDataConditions.Add(":", ";|:");
                    replacedDataConditions.Add("[|]", "<|\\(|\\[#>|\\)|\\]");
                    replacedDataConditions.Add("(|)", "C|<|\\[|\\(#>|\\]|\\)");

                    Dictionary<string, string> ExceptionVal = new Dictionary<string, string>();
                    ExceptionVal.Add("O", "0");

                    var stringlistToCheck = getTheParaToCheck;
                    var checkNextLine = true;
                    foreach (var item in getAllSectionInRange)
                    {
                        for (int i = stringlistToCheck.Count() - 1; i >= 0; i--)
                        {
                            if (checkNextLine == false)
                                break;
                            var sentence = stringlistToCheck.ElementAt(i);
                            Regex regex = new Regex(regexVal.Replace("^", ""));
                            var checkNextRegex = true;

                            foreach (Match match in regex.Matches(sentence))
                            {
                                var foundInSentence = match.Value;
                                var sectionToSearch = item;
                                foreach (var itemSpeChar in _CheckWord.specialChar) // get the section no only
                                {
                                    Regex regexSpeChar = new Regex("(?i)" + itemSpeChar);
                                    var matchSpeChar = regexSpeChar.Match(foundInSentence); // check if match found
                                    if (matchSpeChar.Success)
                                    {
                                        sectionToSearch = sectionToSearch.Replace(matchSpeChar.Value, "");
                                        foundInSentence = foundInSentence.Replace(matchSpeChar.Value, "");
                                    }
                                }
                                if (foundInSentence == sectionToSearch)
                                {
                                    finalSectionFound = sectionToSearch;
                                    finalSectionFoundIndex = match.Index;
                                    checkNextRegex = false;
                                    break;
                                }
                            }
                            if (checkNextRegex == true)
                            {
                                var getSecondRegex = newRegex[regexValue];
                                var splitData = replacedDataConditions[replacedData].Split('#');

                                if (splitData.Count() == 2)
                                    getSecondRegex = "^[\\s]?[" + splitData[0] + "]?[\\s]{0,1}" + getSecondRegex + "[\\s]{0,1}[" + splitData[1] + "]?(?!\\S)";
                                else
                                    getSecondRegex = "^[\\s]?" + getSecondRegex + "[\\s]{0,1}[" + splitData[0] + "]?(?!\\S)";

                                Regex regexSecond = new Regex(getSecondRegex);
                                var sectionIndex = 0;
                                foreach (Match matchVal in regexSecond.Matches(sentence))
                                {
                                    var matchValCopy = matchVal.Value.Trim();
                                    matchValCopy = Regex.Replace(matchValCopy, "[(|<|\\[]", "");
                                    matchValCopy = Regex.Replace(matchValCopy, "[)|\\]|>|:|;|.|,]", "");

                                    if (matchValCopy == item & i != 0)
                                    {
                                        if (stringlistToCheck.ElementAt(i - 1).EndsWith(";") | stringlistToCheck.ElementAt(i - 1).EndsWith("."))
                                        {
                                            sectionIndex = sentence.IndexOf(matchValCopy);
                                            finalSectionFound = matchValCopy;
                                            checkNextLine = false;
                                            checkNextRegex = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (checkNextRegex == false)
                            {
                                List<string> firstSection = new List<string>();
                                List<string> secondSection = new List<string>();
                                var checkNextPara = false;
                                for (int z = 0; z < lastStringSet.Count(); z++)
                                {
                                    var para = lastStringSet.ElementAt(z);
                                    if (checkNextPara == false)
                                    {
                                        if (para.Trim() == sentence.Trim())
                                        {
                                            var suString = para.Substring(0, finalSectionFoundIndex);
                                            if (suString != "")
                                            {
                                                firstSection.Add(suString);
                                            }
                                            if (saveNewSection.Count() == 0)
                                            {
                                                saveNewSection.Add("0", firstSection);
                                                paraList.Add("", firstSection);
                                            }
                                            checkNextPara = true;
                                        }
                                        else
                                        {
                                            firstSection.Add(para);
                                        }

                                    }
                                    if (checkNextPara == true)
                                    {
                                        if (secondSection.Count() == 0 | lastStringSet.Count() == 1)
                                        {
                                            var suString = para.Substring(finalSectionFoundIndex);
                                            secondSection.Add(suString);
                                        }
                                        else
                                        {
                                            secondSection.Add(para);
                                        }

                                    }
                                }
                                saveNewSection.Add(finalSectionFound, secondSection);
                                paraList.Add(finalSectionFound, secondSection);
                                stringlistToCheck = secondSection;
                                checkNextLine = false;
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                _Default.LogError("checkInParaForSection", "", ex);
                throw;
                }
            }

            #endregion

            #region ---------------------------------tree view to json------------------------------------
            public JObject CreateJson(Node node)
            {
                try
                {


                    var jo = new JObject();
                    var ja = new JArray();
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        var child = node.Children[i];
                        var childJo = CreateJson(child);
                        ja.Add(childJo);
                    }

                    var jaPara = new JArray();
                    if (node.Para != null)
                    {
                        foreach (var item in node.Para)
                        {
                            jaPara.Add(item);
                        }
                    }

                    jo["section"] = node.Value;
                    jo["SectionName"] = node.SectionName;
                    jo["Level"] = node.Level;
                    jo["regex"] = node.Regex;
                    jo["para"] = jaPara;
                    jo["children"] = ja;
                    jo["parentCheck"] = node.ParentCheck;
                    return jo;
                }
                catch (Exception ex)
                {
                _Default.LogError("CreateJson", "", ex);
                throw;
                }
            }
            public JObject CreateSectionJson(Node node)
            {
                try
                {

                    var jo = new JObject();
                    var ja = new JArray();
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        var child = node.Children[i];
                        var childJo = CreateSectionJson(child);
                        ja.Add(childJo);
                    }

                    var jaPara = new JArray();
                    if (node.Para != null)
                    {
                        foreach (var item in node.Para)
                        {
                            jaPara.Add(item);
                        }
                    }

                    jo["section"] = node.Value;
                    jo["SectionName"] = node.SectionName;
                    jo["Level"] = node.Level;
                    jo["para"] = jaPara;
                    jo["regex"] = node.Regex;
                    jo["children"] = ja;
                    jo["parentCheck"] = node.ParentCheck;
                    return jo;

                }
                catch (Exception ex)
                {
                _Default.LogError("CreateSectionJson", "", ex);
                throw;
                }
            }
            #endregion

            //------------------------------------------------------------------------------------------------------------------------------------------

            #region --------------------------------------------------------FINANCIAL--------------------------------------------------

            //-------------------------------------complete date------------------------------------------------------
            // get complete date
            public List<string> getDate(string html)
            {
                try
                {

                    html = "26th January 2018";
                    var objDate = "";
                    Regex regex = new Regex(@"(?:\d+[a-z]*\s*(?:of\s+)?)?(?:jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[a-z]*\s*,?\s*(?:(?:\d+[a-z]*\s*)?\s*,?\s*)?\d{4}\b", RegexOptions.IgnoreCase);

                    List<string> formattedString = new List<string>();
                    foreach (Match m in regex.Matches(html))
                    {
                        string datestring = m.Value;


                        if (!string.IsNullOrEmpty(datestring))
                        {
                            int indexOfFirstLetter = Regex.Match(datestring, "(?<=[0-9])(?:st|nd|rd|th)", RegexOptions.IgnoreCase).Index;
                            if (indexOfFirstLetter != 0)
                            {
                                datestring = datestring.Remove(indexOfFirstLetter, 2);
                            }
                        }
                        else
                        {
                            return null;
                        }

                        DateTime value;
                        if (DateTime.TryParse(datestring, out value))
                        {
                            objDate = value.ToString("MM/dd/yyyy");
                        }
                        if (objDate == "")
                        {
                            return null;
                        }
                        else

                            formattedString.Add(objDate);
                    }
                    return formattedString;

                }
                catch (Exception ex)
                {
                _Default.LogError("getDate", "", ex);
                throw;
                }
            }

            //get moth year or year
            public List<string> getYear(string html)
            {
                try
                {


                    var objDate = "";
                    Regex regex = new Regex(@"((?:jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[a-z]*\s*)?,?\s*(?:(?:\d+[a-z]*\s*)?\s*,?\s*)?\d{4}\b", RegexOptions.IgnoreCase);

                    List<string> formattedString = new List<string>();
                    foreach (Match m in regex.Matches(html))
                    {
                        string datestring = m.Value;


                        if (!string.IsNullOrEmpty(datestring))
                        {
                            int indexOfFirstLetter = Regex.Match(datestring, "(?<=[0-9])(?:st|nd|rd|th)", RegexOptions.IgnoreCase).Index;
                            if (indexOfFirstLetter != 0)
                            {
                                datestring = datestring.Remove(indexOfFirstLetter, 2);
                            }
                        }
                        else
                        {
                            return null;
                        }

                        DateTime value;
                        Regex regexYear = new Regex(@"\d{4}", RegexOptions.IgnoreCase);
                        var match = regexYear.Match(datestring); // check if match found
                        if (match.Success)
                        {
                            if (datestring.Trim().IndexOf(match.Value.Trim()) == 0)
                            {
                                objDate = match.Value.Trim();
                            }
                            else
                            {
                                DateTime.TryParse(datestring, out value);
                                objDate = value.ToString("MM/yyyy");
                            }
                        }
                        if (objDate == "")
                        {
                            return null;
                        }
                        else

                            formattedString.Add(objDate);
                    }
                    return formattedString;
                }
                catch (Exception ex)
                {
                _Default.LogError("getYear", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //---------------------------------------amount----------------------------------------------------
            // get amount
            public List<string> getCurrencyAmount(string html)
            {
                try
                {

                    string Pattern = @"(?<SYMBOL>[$â‚¬Â£]){1}[\s]*([\d{1,3}]+(\.\d(?:\d+\.?)*)?)";
                    List<string> formattedString = new List<string>();
                    foreach (Match m in Regex.Matches(html, Pattern))
                    {
                        formattedString.Add(m.Value);
                    }
                    if (formattedString.Count() == 0)
                        getMultipleTextualDollar(html, out formattedString);
                    return formattedString;

                }
                catch (Exception ex)
                {
                _Default.LogError("getCurrencyAmount", "", ex);
                throw;
                }

            }

            // word amount
            public void getMultipleTextualDollar(string numberString, out List<string> formattedString)
            {
                try
                {

                    Dictionary<string, long> numberTable = new Dictionary<string, long>
        { {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
        {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
        {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
        {"fourteen",14},{"fifteen",15},{"sixteen",16},
        {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
        {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
        {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
        {"thousand",1000},{"million",1000000},{"billion",1000000000},
        {"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}};
                    formattedString = new List<string>();
                    var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                                .Select(m => m.Value.ToLowerInvariant())
                                .Where(v => numberTable.ContainsKey(v))
                                .Select(v => numberTable[v]);

                    List<int> dollarIndex = new List<int>();
                    var startdollar = 0;
                    string[] outputData = numberString.Split(new[] { "dollar" }, StringSplitOptions.None);
                    var dollarCount = outputData.Length - 1;
                    for (int k = 0; k < dollarCount; k++)
                    {
                        var indexVal = numberString.Replace(" ", "").ToLower().IndexOf("dollar", startdollar);
                        startdollar = indexVal + 6;
                        dollarIndex.Add(indexVal);
                    }

                    List<string> numberVal = new List<string>();
                    List<long> sample = new List<long>();
                    var dollar = "dollar";
                    long acc = 0, total = 0L;
                    int prevIndex = 0, currIndex = 0;
                    string currKey = "", prevKey = "";
                    int i = 0;
                    var startFromVal = 0;
                    if (numberString.ToLower().IndexOf("dollar") != -1)
                    {
                        foreach (var n in numbers)
                        {
                            numberString = numberString.Replace(" ", "");
                            currKey = numberTable.FirstOrDefault(x => x.Value.ToString().ToLower() == n.ToString().ToLower()).Key;
                            currIndex = numberString.ToLower().IndexOf(currKey.ToLower(), startFromVal);
                            startFromVal = currIndex + currKey.Length - 1;
                            if (!(prevIndex == 0 || currIndex - (prevIndex + prevKey.Length - 1) == 1))
                            {
                                var getAllWord = "";
                                foreach (var item in numberVal)
                                {
                                    getAllWord = getAllWord + item;
                                }
                                var indexOfNumber = numberString.IndexOf(getAllWord);

                                foreach (var item in dollarIndex)
                                {
                                    if (indexOfNumber - dollar.Length == item)
                                    {
                                        sample.Add(acc);
                                        break;
                                    }
                                }
                                numberVal = new List<string>();
                                i++;
                                prevIndex = 0;
                                currIndex = 0;
                                prevKey = "";
                                acc = 0;
                                total = 0L;

                            }
                            numberVal.Add(currKey);
                            if (n >= 1000)
                            {
                                total += (acc * n);
                                acc = 0;
                            }
                            else if (n >= 100)
                            {
                                acc *= n;
                            }
                            else acc += n;

                            prevIndex = currIndex;
                            prevKey = currKey;
                        }
                        if (acc != 0)
                        {
                            var getAllWord = "";
                            foreach (var item in numberVal)
                            {
                                getAllWord = getAllWord + item;
                            }
                            var indexOfNumber = numberString.IndexOf(getAllWord);

                            foreach (var item in dollarIndex)
                            {
                                if (indexOfNumber - dollar.Length == item)
                                {
                                    sample.Add(acc);
                                    break;
                                }
                            }
                        }


                        foreach (var ss in sample)
                        {
                            formattedString.Add(ss + "%");
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("getMultipleTextualDollar", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //---------------------------------------percentage----------------------------------------------------
            // get percent
            public List<string> extractPercentage(string html)
            {
                try
                {

                    List<string> formattedString = new List<string>();
                    foreach (Match m in Regex.Matches(html, @"(\d{1,3})?(\.(\d(?:\d+\.?)*)?)?[\s]*(\%)"))
                    {
                        formattedString.Add(m.Value);
                    }
                    if (formattedString.Count() == 0)
                        getMultipleTextualPercent(html, out formattedString);
                    for (var i = 0; i < formattedString.Count(); i++)
                    {
                        if (formattedString[i].IndexOf('.') == 0)
                        {
                            formattedString[i] = "0" + formattedString[i];
                        }
                    }
                    return formattedString;

                }
                catch (Exception ex)
                {
                _Default.LogError("extractPercentage", "", ex);
                throw;
                }
            }

            // word percent 
            public void getMultipleTextualPercent(string numberString, out List<string> formattedString)
            {
                try
                {

                    Dictionary<string, long> numberTable = new Dictionary<string, long>
        { {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
        {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
        {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
        {"fourteen",14},{"fifteen",15},{"sixteen",16},
        {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
        {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
        {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
        {"thousand",1000},{"million",1000000},{"billion",1000000000},
        {"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}};
                    formattedString = new List<string>();
                    var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                                .Select(m => m.Value.ToLowerInvariant())
                                .Where(v => numberTable.ContainsKey(v))
                                .Select(v => numberTable[v]);

                    List<long> sample = new List<long>();
                    long acc = 0, total = 0L;
                    int prevIndex = 0, currIndex = 0;
                    string currKey = "", prevKey = "";
                    int i = 0;
                    var startFromVal = 0;
                    if (numberString.ToLower().IndexOf("percent") != -1)
                    {
                        foreach (var n in numbers)
                        {
                            numberString = numberString.Replace(" ", "");
                            currKey = numberTable.FirstOrDefault(x => x.Value.ToString().ToLower() == n.ToString().ToLower()).Key;
                            currIndex = numberString.ToLower().IndexOf(currKey.ToLower(), startFromVal);
                            startFromVal = startFromVal + currKey.Length;
                            if (!(prevIndex == 0 || currIndex - (prevIndex + prevKey.Length - 1) == 1))
                            {
                                if (numberString.ToLower().IndexOf("percent") == prevIndex + prevKey.Length)
                                    sample.Add(acc);
                                i++;
                                prevIndex = 0;
                                currIndex = 0;
                                prevKey = "";
                                currKey = "";
                                acc = 0;
                                total = 0L;
                            }

                            if (n >= 1000)
                            {
                                total += (acc * n);
                                acc = 0;
                            }
                            else if (n >= 100)
                            {
                                acc *= n;
                            }
                            else acc += n;

                            prevIndex = currIndex;
                            prevKey = currKey;
                        }
                        if (acc != 0)
                            sample.Add(acc);

                        foreach (var ss in sample)
                        {
                            formattedString.Add(ss.ToString() + "%");
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("getMultipleTextualPercent", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //---------------------------------------days----------------------------------------------------
            // only days
            public List<string> getDays(string html)
            {
                try
                {

                    html = Regex.Replace(html, @"[^0-9a-zA-Z]+", " ");
                    List<string> formattedString = new List<string>();
                    foreach (Match m in Regex.Matches(html, @"\d{2}[\s]*days"))
                    {
                        formattedString.Add(m.Value);
                    }
                    if (formattedString.Count() == 0)
                        getTextualDays(html, out formattedString);
                    if (formattedString.Count() == 0)
                        getMonths(html, out formattedString);
                    return formattedString;

                }
                catch (Exception ex)
                {
                _Default.LogError("getDays", "", ex);
                throw;
                }
            }

            //only days word
            public void getTextualDays(string numberString, out List<string> formattedString)
            {
                try
                {


                    numberString = Regex.Replace(numberString, @"[\d-]", " ");
                    Dictionary<string, long> numberTable = new Dictionary<string, long>
            { {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
            {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
            {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
            {"fourteen",14},{"fifteen",15},{"sixteen",16},
            {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
            {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
            {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
            {"thousand",1000},{"million",1000000},{"billion",1000000000},
            {"trillion",1000000000000},{"quadrillion",1000000000000000},
            {"quintillion",1000000000000000000}};
                    formattedString = new List<string>();
                    var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                                .Select(m => m.Value.ToLowerInvariant())
                                .Where(v => numberTable.ContainsKey(v))
                                .Select(v => numberTable[v]);

                    List<long> sample = new List<long>();
                    long acc = 0, total = 0L;
                    int prevIndex = 0, currIndex = 0;
                    string currKey = "", prevKey = "";
                    int i = 0;
                    var startFromVal = 0;
                    if (numberString.ToLower().IndexOf("days") != -1)
                    {
                        foreach (var n in numbers)
                        {
                            numberString = numberString.Replace(" ", "");
                            currKey = numberTable.FirstOrDefault(x => x.Value.ToString().ToLower() == n.ToString().ToLower()).Key;
                            currIndex = numberString.ToLower().IndexOf(currKey.ToLower(), startFromVal);
                            startFromVal = startFromVal + currKey.Length;
                            if (!(prevIndex == 0 || currIndex - (prevIndex + prevKey.Length - 1) == 1))
                            {
                                if (numberString.ToLower().IndexOf("days") == prevIndex + prevKey.Length)
                                    sample.Add(acc);
                                i++;
                                prevIndex = 0;
                                currIndex = 0;
                                prevKey = "";
                                currKey = "";
                                acc = 0;
                                total = 0L;
                            }

                            if (n >= 1000)
                            {
                                total += (acc * n);
                                acc = 0;
                            }
                            else if (n >= 100)
                            {
                                acc *= n;
                            }
                            else acc += n;

                            prevIndex = currIndex;
                            prevKey = currKey;
                        }
                        if (acc != 0)
                            sample.Add(acc);

                        foreach (var ss in sample)
                        {
                            formattedString.Add(ss.ToString() + " days");
                        }
                    }
                }
                catch (Exception ex)
                {
                _Default.LogError("getTextualDays", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //-------------------------------------- months-----------------------------------------------------
            // only months
            public void getMonths(string html, out List<string> formattedString)
            {
                try
                {

                    html = Regex.Replace(html, @"[^0-9a-zA-Z]+", " ");
                    formattedString = new List<string>();
                    foreach (Match m in Regex.Matches(html, @"\d{2}[\s]*months"))
                    {
                        formattedString.Add(m.Value);
                    }
                    if (formattedString.Count() == 0)
                        getTextualMonths(html, out formattedString);
                }
                catch (Exception ex)
                {
                _Default.LogError("getMonths", "", ex);
                throw;
                }
            }

            // only month words
            public void getTextualMonths(string numberString, out List<string> formattedString)
            {
                try
                {

                    numberString = Regex.Replace(numberString, @"[\d-]", " ");
                    Dictionary<string, long> numberTable = new Dictionary<string, long>
            { {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
            {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
            {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
            {"fourteen",14},{"fifteen",15},{"sixteen",16},
            {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
            {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
            {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
            {"thousand",1000},{"million",1000000},{"billion",1000000000},
            {"trillion",1000000000000},{"quadrillion",1000000000000000},
            {"quintillion",1000000000000000000}};
                    formattedString = new List<string>();
                    var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                                .Select(m => m.Value.ToLowerInvariant())
                                .Where(v => numberTable.ContainsKey(v))
                                .Select(v => numberTable[v]);

                    List<long> sample = new List<long>();
                    long acc = 0, total = 0L;
                    int prevIndex = 0, currIndex = 0;
                    string currKey = "", prevKey = "";
                    int i = 0;
                    var startFromVal = 0;
                    if (numberString.ToLower().IndexOf("months") != -1)
                    {
                        foreach (var n in numbers)
                        {
                            numberString = numberString.Replace(" ", "");
                            currKey = numberTable.FirstOrDefault(x => x.Value.ToString().ToLower() == n.ToString().ToLower()).Key;
                            currIndex = numberString.ToLower().IndexOf(currKey.ToLower(), startFromVal);
                            startFromVal = startFromVal + currKey.Length;
                            if (!(prevIndex == 0 || currIndex - (prevIndex + prevKey.Length - 1) == 1))
                            {
                                if (numberString.ToLower().IndexOf("months") == prevIndex + prevKey.Length)
                                    sample.Add(acc);
                                i++;
                                prevIndex = 0;
                                currIndex = 0;
                                prevKey = "";
                                currKey = "";
                                acc = 0;
                                total = 0L;
                            }

                            if (n >= 1000)
                            {
                                total += (acc * n);
                                acc = 0;
                            }
                            else if (n >= 100)
                            {
                                acc *= n;
                            }
                            else acc += n;

                            prevIndex = currIndex;
                            prevKey = currKey;
                        }
                        if (acc != 0)
                            sample.Add(acc);

                        foreach (var ss in sample)
                        {
                            formattedString.Add(ss.ToString() + " months");
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("getTextualMonths", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //----------------------------------------- only days and year--------------------------------------------------
            // only year count
            public List<string> getYearCount(string html)
            {
                try
                {

                    html = Regex.Replace(html, @"[^0-9a-zA-Z]+", " ");
                    List<string> formattedString = new List<string>();
                    foreach (Match m in Regex.Matches(html, @"(year[s]?)?[\s]*\d{4}[\s]*(year[s]?)?"))
                    {
                        formattedString.Add(m.Value);
                    }
                    if (formattedString.Count() == 0)
                        getTextualYear(html, out formattedString);
                    return formattedString;

                }
                catch (Exception ex)
                {
                _Default.LogError("getYearCount", "", ex);
                throw;
                }
            }

            //only days word
            public void getTextualYear(string numberString, out List<string> formattedString)
            {
                try
                {


                    Dictionary<string, long> numberTable = new Dictionary<string, long>
        { {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
        {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
        {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
        {"fourteen",14},{"fifteen",15},{"sixteen",16},
        {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
        {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
        {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
        {"thousand",1000},{"million",1000000},{"billion",1000000000},
        {"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}};
                    formattedString = new List<string>();
                    var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                                .Select(m => m.Value.ToLowerInvariant())
                                .Where(v => numberTable.ContainsKey(v))
                                .Select(v => numberTable[v]);

                    List<long> sample = new List<long>();
                    long acc = 0, total = 0L;
                    int prevIndex = 0, currIndex = 0;
                    string currKey = "", prevKey = "";
                    int i = 0;
                    var startFromVal = 0;
                    if (numberString.ToLower().IndexOf("years") != -1)
                    {
                        foreach (var n in numbers)
                        {
                            numberString = numberString.Replace(" ", "");
                            currKey = numberTable.FirstOrDefault(x => x.Value.ToString().ToLower() == n.ToString().ToLower()).Key;
                            currIndex = numberString.ToLower().IndexOf(currKey.ToLower(), startFromVal);
                            startFromVal = startFromVal + currKey.Length;
                            if (!(prevIndex == 0 || currIndex - (prevIndex + prevKey.Length - 1) == 1))
                            {
                                if (numberString.ToLower().IndexOf("years") == prevIndex + prevKey.Length)
                                    sample.Add(acc);
                                i++;
                                prevIndex = 0;
                                currIndex = 0;
                                prevKey = "";
                                currKey = "";
                                acc = 0;
                                total = 0L;
                            }

                            if (n >= 1000)
                            {
                                total += (acc * n);
                                acc = 0;
                            }
                            else if (n >= 100)
                            {
                                acc *= n;
                            }
                            else acc += n;

                            prevIndex = currIndex;
                            prevKey = currKey;
                        }
                        if (acc != 0)
                            sample.Add(acc);

                        foreach (var ss in sample)
                        {
                            formattedString.Add(ss.ToString() + " years");
                        }
                    }
                }
                catch (Exception ex)
                {
                _Default.LogError("getTextualYear", "", ex);
                throw;
                }
            }
            //-------------------------------------------------------------------------------------------

            //----------------------------------------- get only amount value--------------------------------------------------

            public void getCurrencyAmountFinancial(List<string> amountList, out List<string> amountListCopy)
            {
                try
                {

                    amountListCopy = new List<string>();
                    foreach (var item in amountList)
                    {
                        string Pattern = @"(?<SYMBOL>[$â‚¬Â£]){1}[\s]*([\d{1,3}]+(\.\d(?:\d+\.?)*)?)";
                        foreach (Match m in Regex.Matches(item, Pattern))
                        {
                            amountListCopy.Add(m.Value);
                        }
                    }

                }
                catch (Exception ex)
                {
                _Default.LogError("getCurrencyAmountFinancial", "", ex);
                throw;
                }
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------------

            #endregion

            #region --------------------------------------------------EXCEL----------------------------------------------------
            //--
            public void ReplaceTextInExcelFile(string path, string projName, List<string> saveDatapointName, JArray saveLeaseDataForExcel)
            {
                try
                {

                    Microsoft.Office.Interop.Excel.Workbook wb = default(Workbook);
                    Microsoft.Office.Interop.Excel.Application app = default(Application);

                    var ExcelPathConfig = WebConfigurationManager.AppSettings["Excel path"];
                    var DirectoryConfig = WebConfigurationManager.AppSettings["Directory path"];
                    var getDirectoryName = path.Split(new string[] { "\\" }, StringSplitOptions.None); // get the directory name eg: D:
                    var LeaseName = getDirectoryName.Last();
                    var getExcelFile = Directory.GetFiles(DirectoryConfig + "\\" + ExcelPathConfig + "\\"); // get the excel file from folder
                    var mainFile = getExcelFile.Where(x => x.IndexOf("$") == -1).FirstOrDefault();
                    var fileSplitName = mainFile.Split('.'); // split for extension
                    var extension = fileSplitName[fileSplitName.Length - 1]; // get the extension

                    var date = DateTime.Now.ToString("d-M-yyyy");
                    var time = DateTime.Now.ToString("HH-mm-ss");

                    //-------------------------------------------get data to save --------------------------------------------------------

                    Dictionary<string, string> fileName = new Dictionary<string, string>(); // only file name 
                    Dictionary<string, string> pageNo = new Dictionary<string, string>(); // only page no
                    Dictionary<string, string> dataPointName = new Dictionary<string, string>(); // only datapoint name
                    Dictionary<string, string> completeOutput = new Dictionary<string, string>(); // sectio No and summarization
                    Dictionary<string, string> sectionNo = new Dictionary<string, string>(); // only section no
                    Dictionary<string, string> summarization = new Dictionary<string, string>(); // only summarization
                    Dictionary<string, string> output = new Dictionary<string, string>(); // paragraphs show


                    foreach (var item in saveLeaseDataForExcel) // save the data in dictionary
                    {
                        if (item["fileName"].ToString() == "")
                            fileName.Add(item["dataPointName"].ToString(), item["leaseName"].ToString());
                        else
                            fileName.Add(item["dataPointName"].ToString(), item["leaseName"].ToString() + " (" + item["fileName"].ToString() + ")");
                        pageNo.Add(item["dataPointName"].ToString(), item["pageNo"].ToString());
                        dataPointName.Add(item["dataPointName"].ToString(), item["dataPointName"].ToString());
                        completeOutput.Add(item["dataPointName"].ToString(), item["correctString"].ToString());
                        sectionNo.Add(item["dataPointName"].ToString(), item["onlySectionNo"].ToString());
                        output.Add(item["dataPointName"].ToString(), item["output"].ToString());
                        if (item["onlySummarization"].ToString() == "")
                            summarization.Add(item["dataPointName"].ToString(), "Lease is silent");
                        else
                            summarization.Add(item["dataPointName"].ToString(), item["onlySummarization"].ToString());
                    }

                    // save data in excel
                    try
                    {
                        var excelTempPath = DirectoryConfig + "\\" + ExcelPathConfig + "\\" + projName + "." + extension; // get the excel
                        var excelCopyPth = path.Remove(path.LastIndexOf('\\')) + "\\" + projName + "_(" + date + ")-(" + time + ")" + "." + extension;
                        var excelPath = path + "\\" + LeaseName + "_(" + date + ") - (" + time + ")" + "." + extension;
                        System.IO.File.Copy(excelTempPath, excelCopyPth, true); // make a copy of file where changes need to be made and save it to new path
                        File.Move(excelCopyPth, excelPath);


                        // -----------------------------getting excel files ----------------------------------------------------
                        object m = Type.Missing;
                        app = new Microsoft.Office.Interop.Excel.Application();
                        app.DisplayAlerts = false; // diable the aleart message
                        wb = app.Workbooks.Open(excelPath, m, false, m, m, m, m, m, m, m, m, m, m, m, m);

                        //----------------------------------------------------------------------------------------------------------

                        foreach (Worksheet ws in wb.Sheets)
                        {
                            Microsoft.Office.Interop.Excel.Range r = (Microsoft.Office.Interop.Excel.Range)ws.UsedRange;

                            foreach (var item in output)
                            {
                                var isSuccess = false;
                                string replace = "{{" + item.Key + "_output" + "}}"; // replace text
                                var stringLength = 250 - replace.Length; // char count pass  at once length
                                string replacement = (item.Value).ToString(); // replacement string
                                var replacementLength = replacement.Length; // length of replacement string
                                for (int i = 0; i < replacementLength; i += stringLength)
                                {
                                    var strlength = i + stringLength;
                                    if (strlength < replacementLength) // checks weather replacement string length is more than string to be passed at once 
                                    {
                                        var currentString = replacement.Substring(i, stringLength) + replace; // string to be passed with the replace string attached to it
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                    else
                                    {
                                        var currentString = replacement.Substring(i, replacementLength - i);
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                }
                                if (isSuccess == false)
                                {
                                    bool success = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                    wb.Save();
                                }
                            }
                            foreach (var item in completeOutput)
                            {
                                var isSuccess = false;
                                string replace = "{{" + item.Key + "_complete output" + "}}"; // replace text
                                var stringLength = 250 - replace.Length; // char count pass  at once length
                                string replacement = item.Value.ToString(); // replacement string
                                var replacementLength = replacement.Length; // length of replacement string
                                for (int i = 0; i < replacementLength; i += stringLength)
                                {
                                    var strlength = i + stringLength;
                                    if (strlength < replacementLength) // checks weather replacement string length is more than string to be passed at once 
                                    {
                                        var currentString = replacement.Substring(i, stringLength) + replace; // string to be passed with the replace string attached to it
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                    else
                                    {
                                        var currentString = replacement.Substring(i, replacementLength - i);
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                }
                                if (isSuccess == false)
                                {
                                    bool success = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                    wb.Save();
                                }
                            }
                            foreach (var item in summarization)
                            {
                                var isSuccess = false;
                                string replace = "{{" + item.Key + "_summarization" + "}}"; // replace text
                                var stringLength = 250 - replace.Length; // char count pass  at once length
                                string replacement = item.Value.ToString(); // replacement string
                                var replacementLength = replacement.Length; // length of replacement string
                                for (int i = 0; i < replacementLength; i += stringLength)
                                {
                                    var strlength = i + stringLength;
                                    if (strlength < replacementLength) // checks weather replacement string length is more than string to be passed at once 
                                    {
                                        var currentString = replacement.Substring(i, stringLength) + replace; // string to be passed with the replace string attached to it
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                    else
                                    {
                                        var currentString = replacement.Substring(i, replacementLength - i);
                                        bool success = (bool)r.Replace(
                                        replace,
                                        currentString,
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                        wb.Save();
                                        if (success == true)
                                            isSuccess = true;
                                    }
                                }
                                if (isSuccess == false)
                                {
                                    bool success = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                    wb.Save();
                                }
                            }
                            foreach (var item in fileName) // datapoint not been accepted loop
                            {
                                string replace = "{{" + item.Key + "_file name" + "}}";
                                var currentString = item.Value.ToString();
                                bool success = (bool)r.Replace(
                                replace,
                                currentString,
                                XlLookAt.xlPart,
                                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                true, m, m, m);
                                if (success == false)
                                {
                                    bool success1 = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                }
                                wb.Save();
                            }
                            foreach (var item in dataPointName) // datapoint not been accepted loop
                            {
                                string replace = "{{" + item.Key + "_datapoint name" + "}}";
                                var currentString = item.Value.ToString();
                                bool success = (bool)r.Replace(
                                replace,
                                currentString,
                                XlLookAt.xlPart,
                                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                true, m, m, m);
                                if (success == false)
                                {
                                    bool success1 = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                }
                                wb.Save();
                            }
                            foreach (var item in pageNo) // datapoint not been accepted loop
                            {
                                string replace = "{{" + item.Key + "_page no" + "}}";
                                var currentString = item.Value.ToString();
                                bool success = (bool)r.Replace(
                                replace,
                                currentString,
                                XlLookAt.xlPart,
                                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                true, m, m, m);
                                if (success == false)
                                {
                                    bool success1 = (bool)r.Replace(
                                        replace,
                                        "",
                                        XlLookAt.xlPart,
                                        Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                        true, m, m, m);
                                }
                                wb.Save();
                            }
                            foreach (var item in sectionNo)
                            {
                                var addSectionNo = true;
                                if (addSectionNo == true)
                                {
                                    string replace = "{{" + item.Key + "_section no" + "}}";
                                    var currentString = item.Value.ToString();
                                    bool success = (bool)r.Replace(
                                    replace,
                                    currentString,
                                    XlLookAt.xlPart,
                                    Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                    true, m, m, m);
                                    if (success == false)
                                    {
                                        bool success1 = (bool)r.Replace(
                                            replace,
                                            "",
                                            XlLookAt.xlPart,
                                            Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                            true, m, m, m);
                                    }
                                    wb.Save();
                                }
                            }
                        }
                        wb.Save();
                        wb.Close();
                        wb = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        app.Quit();
                        app = null;
                    }
                    catch (Exception ex)
                    {
                        var testPath = path + "\\test" + "_(" + date + ")-(" + time + ")" + ".txt";
                        var myFile = File.Create(testPath);
                        myFile.Close();
                        var finalError = ex.Message.ToString() + "#################" + ex.Source.ToString();
                        System.IO.File.WriteAllText(testPath, finalError);
                        app.Quit();
                        wb.Close();
                        throw ex;
                    }


                }
                catch (Exception ex)
                {
                _Default.LogError("ReplaceTextInExcelFile", "", ex);
                throw;
                }
            }

        #endregion

      
    }

        // for section tree construction
        public class Node
        {
            public string Value { get; set; }
            public int Level { get; set; }
            public List<Node> Children { get; set; }
            public List<string> Para { get; set; }
            public string Regex { get; set; }
            public string SectionName { get; set; }
            public int ParentCheck { get; set; }

            public Node(string val, int level, List<string> para, string regexVal, string sectionName, int parentCheck)
            {
                Value = val;
                Level = level;
                Para = para;
                Regex = regexVal;
                SectionName = sectionName;
                ParentCheck = parentCheck;
                Children = new List<Node>();
            }

            public Node(string val, int level)
            {
                Value = val;
                Level = level;
                Children = new List<Node>();
            }

        }
    }