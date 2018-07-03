using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace ReboProject
{
    public class processing
    {
        // get the complete section once
        public static List<string> getSectionForPara(string para, string lastLine ,bool nextPara)
        {
            List<string> sectiongot = new List<string>(); // save the section number and regex...
            Dictionary<int, string> checkWordBefore = new Dictionary<int, string>(); // words to check before section number

            checkWordBefore.Add(1, "defined in");
            checkWordBefore.Add(2, "provided in");
            checkWordBefore.Add(3, "pursuant to this");
            checkWordBefore.Add(4, "contained in");
            checkWordBefore.Add(5, "under this");
            checkWordBefore.Add(6, "in this");
            checkWordBefore.Add(7, "provisions of");
            checkWordBefore.Add(8, "stated in");
            checkWordBefore.Add(9, "provided for in");
            checkWordBefore.Add(10, "pursuant to");
            checkWordBefore.Add(11, "provisions of this");
            checkWordBefore.Add(12, "Provisions");
            checkWordBefore.Add(13, "of");
            checkWordBefore.Add(14, "reflected on");
            checkWordBefore.Add(15, "reference as");
            checkWordBefore.Add(16, "year");
            checkWordBefore.Add(17, "years");
            checkWordBefore.Add(18, "and");
            checkWordBefore.Add(19, "or");
            checkWordBefore.Add(20, ",");

            Dictionary<int, string> afterCheckWord = new Dictionary<int, string>(); // check the words after section number

            afterCheckWord.Add(1, "days");
            afterCheckWord.Add(2, "months");
            afterCheckWord.Add(3, "years");
            afterCheckWord.Add(4, "days");
            afterCheckWord.Add(5, "months");
            afterCheckWord.Add(6, "years");

            

            if (String.IsNullOrEmpty(lastLine) || nextPara == true) // if last line not there......(first page of pdf) or its a different para
                sectiongot = regexLoop(para);
            else // if last line there
            {
                sectiongot = regexLoop(para);
                if (sectiongot[0] != null) // if the 
                {
                    var paraCopy = para; // copy of para
                    var sectionNumber = sectiongot[0]; // get the section number
                    var sectionLength = sectionNumber.Length; // length of section number
                    var sentenceWithoutSection = paraCopy.Remove(0, sectionLength).Trim(); // remove section number from 
                    if (sentenceWithoutSection != "")
                    {
                        var firsTChar = sentenceWithoutSection[0];
                        if (firsTChar.ToString() == char.ToUpper(firsTChar).ToString()) // check first word starts with uppercase 
                        {
                            if (lastLine.EndsWith(".") | lastLine.EndsWith(";") | lastLine.EndsWith(",")) // check if last sentence ends with full stop or not
                            {
                                foreach (var item in afterCheckWord) // ckeck words after section nuber
                                {
                                    if (sentenceWithoutSection.IndexOf(item.Value) == 0)
                                    {
                                        sectiongot = new List<string>();
                                        sectiongot.Add(null);
                                        sectiongot.Add(null);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var checkBeforeWords = true;
                                foreach (var item in afterCheckWord)// ckeck words after section nuber
                                {
                                    if (sentenceWithoutSection.IndexOf(item.Value) == 0)
                                    {
                                        sectiongot = new List<string>();
                                        sectiongot.Add(null);
                                        sectiongot.Add(null);
                                        checkBeforeWords = false;
                                        break;
                                    }
                                }
                                if (checkBeforeWords == true)
                                {
                                    foreach (var item in checkWordBefore) // ckeck words before section nuber
                                    {
                                        if (lastLine.ToLower().Trim().EndsWith(item.Value))
                                        {
                                            sectiongot = new List<string>();
                                            sectiongot.Add(null);
                                            sectiongot.Add(null);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else // if not upper case then section not found
                        {
                            sectiongot = new List<string>();
                            sectiongot.Add(null);
                            sectiongot.Add(null);
                        }
                    }
                    else if ((sentenceWithoutSection == ""))
                    {
                        if (!lastLine.EndsWith(".") | !lastLine.EndsWith(";") | !lastLine.EndsWith(","))
                        {
                            var sectionFound = true;
                        }
                    }
                    else // if not upper case then section not found
                    {
                        sectiongot = new List<string>();
                        sectiongot.Add(null);
                        sectiongot.Add(null);
                    }
                }
            }
            return sectiongot; // return section number and regex
        }

        // loop through all the regex to get the 
        public static List<string> regexLoop(string para)
        {
            List<string> sectiongot = new List<string>();
            Dictionary<int, string> regexDictionary = new Dictionary<int, string>(); // check the regex 

            regexDictionary.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.])?(?!\S)"); //    1./ 1. a)
            regexDictionary.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)"); //    1.1  / 1.1 a)
            regexDictionary.Add(3, @"^[\s]*((?i)(section)\s\d*)(?!\S)"); //    section 1
            regexDictionary.Add(4, @"^[\s]*((?i)(section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
            regexDictionary.Add(5, @"^[\s]*((?i)(article)\s\d*)(?!\S)"); //   article 1
            regexDictionary.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
            regexDictionary.Add(7, @"^[\s]*([1-9]{1,3}[:])(?!\S)"); //   1:
            regexDictionary.Add(8, @"^[\s]*([[(][\s]*[1-9]{1,3}[\s]*[)])(?!\S)"); //   (1)
            regexDictionary.Add(9, @"^[\s]*([1-9]{1,3}[]])(?!\S)"); //   1]
            regexDictionary.Add(10, @"^[\s]*([[[\s]*[1-9]{1,3}[\s]*[]])(?!\S)"); //   [1]
            regexDictionary.Add(11, @"^[\s]*([1-9]{1,3}[)])(?!\S)"); //   1)

            regexDictionary.Add(12, @"^[\s]*[(][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[)](?!\S)"); //    (xvii)
            regexDictionary.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    xvii)
            regexDictionary.Add(14, @"^[\s]*[[][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[]](?!\S)"); //    [xvii]
            regexDictionary.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    xvii]
            regexDictionary.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)"); //    xvii:
            regexDictionary.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)"); //    xvii.
            regexDictionary.Add(18, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    section xvii
            regexDictionary.Add(19, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    article xvii
                                      
            regexDictionary.Add(20, @"^[\s]*[(][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[)](?!\S)"); //    (XVII)
            regexDictionary.Add(21, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    XVII)
            regexDictionary.Add(22, @"^[\s]*[[][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[]](?!\S)"); //    [XVII]
            regexDictionary.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    XVII]
            regexDictionary.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)"); //    XVII:
            regexDictionary.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)"); //    XVII.
            regexDictionary.Add(26, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    section XVII
            regexDictionary.Add(27, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    article XVII
                                      
            regexDictionary.Add(28, @"^[\s]*([a-z][.])(?!\S)");  //  a.
            regexDictionary.Add(29, @"^[\s]*([a-z][:])(?!\S)");  //  a:
            regexDictionary.Add(30, @"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)");  //    (a)
            regexDictionary.Add(31, @"^[\s]*([a-z][)])(?!\S)");  // a)
            regexDictionary.Add(32, @"^[\s]*([[][\s]*[a-z][\s]*[]])(?!\S)");  //   a]
            regexDictionary.Add(33, @"^[\s]*([a-z][]])(?!\S)");  //     [a]
            regexDictionary.Add(34, @"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)");  //      section a
            regexDictionary.Add(35, @"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)");  //      article a
                                      
            regexDictionary.Add(36, @"^[\s]*([A-Z][.])(?!\S)");  // A.
            regexDictionary.Add(37, @"^[\s]*([A-Z][:])(?!\S)");  // A:
            regexDictionary.Add(38, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)");  // (A)
            regexDictionary.Add(39, @"^[\s]*([A-Z][)])(?!\S)");  // A)
            regexDictionary.Add(40, @"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)");  // A]
            regexDictionary.Add(41, @"^[\s]*([A-Z][]])(?!\S)");  // [A]
            regexDictionary.Add(42, @"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)");  // section A
            regexDictionary.Add(43, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)");  // ARTICLE A
            
            foreach (var item in regexDictionary) // loop through all the regexs
            {
                Regex regex = new Regex(item.Value);
                var foundSection = false;
                foreach (Match match in regex.Matches(para))
                {
                    sectiongot.Add(match.Value.Trim());
                    sectiongot.Add(item.Value);
                    foundSection = true;
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

        public static void checkPara(bool readSection, int totalParaCount,int sentenceMergeCount, JToken subChild,List<string> sectionList,string[] sectionNoreadAllPara, out List<string> sectionListCopy, out int paraCountSave, out bool sectionCheckParent)
        {
            paraCountSave = 0;
            sectionCheckParent = true;
            sectionListCopy = new List<string>();
            foreach (var item in subChild)
            {
                sectionListCopy = new List<string>();
                paraCountSave = 0;
                sectionList = new List<string>();
                var child = item["children"];
                var sectioNo = item["section"].ToString();
                var paraData = item["para"];
                var parentCheck = (int)item["parentCheck"];

                foreach (var sectionNoreadAllParaData in sectionNoreadAllPara)
                {
                    var exitAllParaLoop = false;
                    for (int j = 0; j < paraData.Count(); j++)
                    {
                        var paraVal = paraData[j].ToString().Trim();
                        if ((paraVal.IndexOf(sectionNoreadAllParaData.Trim()) != -1 | sectionNoreadAllParaData.Trim().IndexOf(paraVal) != -1 ) & paraVal.Length > 50)
                        {
                            if (!sectionListCopy.Contains(sectioNo) & readSection == true)
                            {
                                sectionListCopy.Add(sectioNo);
                                if (parentCheck == 1)
                                {
                                    readSection = false;
                                    sectionCheckParent = false;
                                }
                            }
                                
                            totalParaCount++;
                            paraCountSave = totalParaCount;
                            if (totalParaCount == sentenceMergeCount)
                            {
                                exitAllParaLoop = true;
                                break;
                            }
                        }
                        if (exitAllParaLoop == true)
                            break;
                    }
                }

                if (child.Count()>0 & readSection == true)
                    checkPara(readSection, totalParaCount, sentenceMergeCount,child, sectionList, sectionNoreadAllPara, out sectionListCopy, out paraCountSave, out sectionCheckParent);
                if (sectionListCopy.Count() > 0)
                {
                    if (!sectionListCopy.Contains(sectioNo) & readSection == true)
                    {
                        sectionListCopy.Add(sectioNo);
                        if (parentCheck == 1)
                        {
                            readSection = false;
                            sectionCheckParent = false;
                        }
                    }
                    break;
                }
            }

        } 

        // get the complete section 
        public static string getCompleteParaSection(string SectionNoCount, JArray ja2, Dictionary<int, Dictionary<int, string>> saveSectionNoAllFiles, Dictionary<Dictionary<int, string>, int> saveAllSection, string outputPara, string DefaultSectionName, JToken SectionName, string singleFileSectionTree)
        {
            var jarrayVal = ja2;
            var pageNo = (int)ja2[0]["pageNo"]; // get the pageno of output
            var paraNo = (int)ja2[0]["paraNo"]; //  get the para number
            var readNextPara = (int)ja2[0]["readNextPara"]; // get para
            var sectionNo = ja2[0]["sectionVal"].ToString();
            var sentenceMergeCount = (int)ja2[0]["sentenceMergeCount"];
            var sectionNoreadPara = ja2[0]["sectionNoreadPara"].ToString();
            var sectionNoreadAllPara = sectionNoreadPara.Split('|');


            var foundPara = "";
            if (outputPara == "")
                foundPara = ja2[0]["output"].ToString();
            else
                foundPara = outputPara;
            var finalSectionOutput = "";
            var getFirstLine = "";

            var completeTree = JObject.Parse(singleFileSectionTree.ToString())["children"];

            List<string> sectionList = new List<string>();
            List<string> sectionListCopy = new List<string>();
            var foundSection = false;

            //----------get section data---------------
            
            var toFInd = foundPara.Trim();
            var totalParaCount = 0;
            for (int i = 0; i < completeTree.Count(); i++)
            {
                var child = completeTree[i]["children"];
                var childSection = completeTree[i]["section"].ToString();
                var SectionNameData = "";
                if (completeTree[i]["SectionName"].ToString() != "")
                    SectionNameData = completeTree[i]["SectionName"].ToString();
                else
                    SectionNameData = null;
                var childCount = child.Count();
                foreach (var item in child)
                {
                    var sectionCheckParent = true;
                    var readSection = true;
                    var paraCountSave = 0;
                    var subChild = item["children"];
                    var sectioNo = item["section"].ToString();
                    var subChildCount = subChild.Count();
                    var paraData = item["para"];
                    var parentCheck = (int)item["parentCheck"];
                    var exitAllParaLoop = false;
                    foreach (var sectionNoreadAllParaData in sectionNoreadAllPara)
                    {
                        for (int j = 0; j < paraData.Count(); j++)
                        {
                            var paraVal = paraData[j].ToString().Trim();
                            if ((paraVal.IndexOf(sectionNoreadAllParaData.Trim()) != -1 | sectionNoreadAllParaData.Trim().IndexOf(paraVal) != -1) & paraVal.Length >50)
                            //if (paraVal == sectionNoreadAllParaData.Trim())
                            {
                                if (!sectionListCopy.Contains(sectioNo) & readSection == true & !sectionList.Contains(sectioNo))
                                {
                                    sectionList.Add(sectioNo);
                                    sectionListCopy.Add(sectioNo);
                                    if (parentCheck == 1)
                                        readSection = false;
                                }
                                totalParaCount++;
                                paraCountSave = totalParaCount;
                                if (totalParaCount == sentenceMergeCount)
                                {
                                    foundSection = true;
                                    exitAllParaLoop = true;
                                    break;
                                }
                            }
                            if (exitAllParaLoop == true)
                                break;
                        }
                    }
                    if (subChildCount > 0)
                        checkPara(readSection, totalParaCount, sentenceMergeCount, subChild, sectionList, sectionNoreadAllPara, out sectionListCopy, out paraCountSave, out sectionCheckParent);
                    if (sectionListCopy.Count() > 0 & paraCountSave >= sentenceMergeCount)
                    {
                        if (sectionCheckParent == true & !sectionListCopy.Contains(sectioNo))
                        {
                            sectionListCopy.Add(sectioNo);
                        }
                        foundSection = true;
                        break;
                    }
                }
                if (sectionList.Count() > 0 & foundSection == true)
                    sectionList.Add(SectionNameData);
                if (sectionListCopy.Count() > 0 & foundSection == true)
                {
                    sectionListCopy.Add(SectionNameData);
                    break;
                }
            }

            var finalSectionVal = new List<string>();
            if (sectionListCopy.Count() > 0)
                finalSectionVal= sectionListCopy;
            else if(sectionList.Count() > 0)
                finalSectionVal = sectionList;
            if (finalSectionVal.Count >0 )
            {
                if (finalSectionVal[finalSectionVal.Count - 1] != null | finalSectionVal[finalSectionVal.Count - 1] != "")
                    getFirstLine = finalSectionVal[finalSectionVal.Count - 1];
                else
                    getFirstLine = null;

                finalSectionVal.RemoveAt(finalSectionVal.Count - 1);
                for (int i = finalSectionVal.Count - 1; i >= 0; i--)
                {
                    var sectionNoVal = finalSectionVal[i].Trim();
                    if (finalSectionOutput == "")
                        finalSectionOutput = sectionNoVal;
                    else
                        finalSectionOutput = finalSectionOutput + " " + sectionNoVal;
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

        // loop through all the section regex to get complete section number
        //public static string SectionValParagraph(string SectionNoCount, List<string> allPara, string ouputPara)
        //{
        //    List<string> sectionList = new List<string>();
        //    Dictionary<string, string> getTopVal = new Dictionary<string, string>();
        //    getTopVal.Add("lowCaseAlpha", "a");
        //    getTopVal.Add("upCaseAlpha", "A");
        //    getTopVal.Add("lowCaseNumeric", "i");
        //    getTopVal.Add("upCaseNumeric", "I");
        //    getTopVal.Add("numberVal", "1");

        //    Dictionary<int, string> notations = new Dictionary<int, string>();
        //    notations.Add(1, "(,)");
        //    notations.Add(2, "[,]");
        //    notations.Add(3, "]");
        //    notations.Add(4, ")");
        //    notations.Add(5, ".");
        //    notations.Add(6, ":");
        //    notations.Add(7, "section");
        //    notations.Add(8, "article");

        //    Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
        //    // NUMBERS
        //    matchRegexNumeric.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.])?(?!\S)"); //    1./ 1. a)
        //    matchRegexNumeric.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)"); //    1.1  / 1.1 a)
        //    matchRegexNumeric.Add(3, @"^[\s]*((?i)(section)\s\d*)(?!\S)"); //    section 1
        //    matchRegexNumeric.Add(4, @"^[\s]*((?i)(section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
        //    matchRegexNumeric.Add(5, @"^[\s]*((?i)(article)\s\d*)(?!\S)"); //   article 1
        //    matchRegexNumeric.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
        //    matchRegexNumeric.Add(7, @"^[\s]*([1-9]{1,3}[:])(?!\S)"); //   1:
        //    matchRegexNumeric.Add(8, @"^[\s]*([1-9]{1,3}[)])(?!\S)"); //   1)
        //    matchRegexNumeric.Add(9, @"^[\s]*([1-9]{1,3}[]])(?!\S)"); //   1]
        //    matchRegexNumeric.Add(10, @"^[\s]*([[][\s]*[1-9][\s]*[]])(?!\S)"); //   [1]
        //    matchRegexNumeric.Add(11, @"^[\s]*([[(][\s]*[1-9][\s]*[)])(?!\S)"); //   (1)

        //    matchRegexNumeric.Add(12, @"^[\s]*[(][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[)](?!\S)"); //    (xvii)
        //    matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    xvii)
        //    matchRegexNumeric.Add(14, @"^[\s]*[[][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[]](?!\S)"); //    [xvii]
        //    matchRegexNumeric.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    xvii]
        //    matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)"); //    xvii:
        //    matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)"); //    xvii.
        //    matchRegexNumeric.Add(18, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    section xvii
        //    matchRegexNumeric.Add(19, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    article xvii

        //    matchRegexNumeric.Add(20, @"^[\s]*[(][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[)](?!\S)"); //    (XVII)
        //    matchRegexNumeric.Add(21, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    XVII)
        //    matchRegexNumeric.Add(22, @"^[\s]*[[][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[]](?!\S)"); //    [XVII]
        //    matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    XVII]
        //    matchRegexNumeric.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)"); //    XVII:
        //    matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)"); //    XVII.
        //    matchRegexNumeric.Add(26, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    section XVII
        //    matchRegexNumeric.Add(27, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    article XVII

        //    matchRegexNumeric.Add(28, @"^[\s]*([a-z][.])(?!\S)");  //  a.
        //    matchRegexNumeric.Add(29, @"^[\s]*([a-z][:])(?!\S)");  //  a:
        //    matchRegexNumeric.Add(30, @"^[\s]*([(][\s]*[a-z][)])[\s]*(?!\S)");  //    (a)
        //    matchRegexNumeric.Add(31, @"^[\s]*([a-z][)])(?!\S)");  // a)
        //    matchRegexNumeric.Add(32, @"^[\s]*([[][\s]*[a-z][\s]*[]])(?!\S)");  //   a]
        //    matchRegexNumeric.Add(33, @"^[\s]*([a-z][]])(?!\S)");  //     [a]
        //    matchRegexNumeric.Add(34, @"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)");  //      section a
        //    matchRegexNumeric.Add(35, @"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)");  //      article a

        //    matchRegexNumeric.Add(36, @"^[\s]*([A-Z][.])(?!\S)");  // A.
        //    matchRegexNumeric.Add(37, @"^[\s]*([A-Z][:])(?!\S)");  // A:
        //    matchRegexNumeric.Add(38, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)");  // (A)
        //    matchRegexNumeric.Add(39, @"^[\s]*([A-Z][)])(?!\S)");  // A)
        //    matchRegexNumeric.Add(40, @"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)");  // A]
        //    matchRegexNumeric.Add(41, @"^[\s]*([A-Z][]])(?!\S)");  // [A]
        //    matchRegexNumeric.Add(42, @"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)");  // section A
        //    matchRegexNumeric.Add(43, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)");  // ARTICLE A


        //    Dictionary<int, string> connections = new Dictionary<int, string>();

        //    connections.Add(1, "numberVal"); //    1.
        //    connections.Add(2, ""); //    1.1 
        //    connections.Add(3, ""); //    section 1.
        //    connections.Add(4, ""); //    section 1.1 
        //    connections.Add(5, ""); //   article 1,
        //    connections.Add(6, ""); //  article 1.1 
        //    connections.Add(7, "numberVal"); //   1:
        //    connections.Add(8, "numberVal"); //   1)
        //    connections.Add(9, "numberVal"); //   1]
        //    connections.Add(10, "numberVal"); //   [1]
        //    connections.Add(11, "numberVal"); //   (1)

        //    connections.Add(12, "lowCaseNumeric"); //    (xvii)
        //    connections.Add(13, "lowCaseNumeric"); //    xvii)
        //    connections.Add(14, "lowCaseNumeric"); //    [xvii]
        //    connections.Add(15, "lowCaseNumeric"); //    xvii]
        //    connections.Add(16, "lowCaseNumeric"); //    xvii:
        //    connections.Add(17, "lowCaseNumeric"); //    xvii.
        //    connections.Add(18, ""); //    section xvii
        //    connections.Add(19, ""); //    article xvii

        //    connections.Add(20, "upCaseNumeric"); //    (XVII)
        //    connections.Add(21, "upCaseNumeric"); //    XVII)
        //    connections.Add(22, "upCaseNumeric"); //    [XVII]
        //    connections.Add(23, "upCaseNumeric"); //    XVII]
        //    connections.Add(24, "upCaseNumeric"); //    XVII:
        //    connections.Add(25, "upCaseNumeric"); //    XVII.
        //    connections.Add(26, ""); //    section XVII
        //    connections.Add(27, ""); //    article XVII

        //    connections.Add(28, "lowCaseAlpha");  //  a.
        //    connections.Add(29, "lowCaseAlpha");  //  a:
        //    connections.Add(30, "lowCaseAlpha");  //    (a)
        //    connections.Add(31, "lowCaseAlpha");  // a)
        //    connections.Add(32, "lowCaseAlpha");  //   a]
        //    connections.Add(33, "lowCaseAlpha");  //     [a]
        //    connections.Add(34, "");  //      section a
        //    connections.Add(35, "");  //      article a

        //    connections.Add(36, "upCaseAlpha");  // A.
        //    connections.Add(37, "upCaseAlpha");  // A:
        //    connections.Add(38, "upCaseAlpha");  // (A)
        //    connections.Add(39, "upCaseAlpha");  // A)
        //    connections.Add(40, "upCaseAlpha");  // A]
        //    connections.Add(41, "upCaseAlpha");  // [A]
        //    connections.Add(42, "");  // section A
        //    connections.Add(43, "");  // ARTICLE A  


        //    List<string> allSectionVal = new List<string>();


        //    var notationFoundIn = "";
        //    var matchNextRegex = true;
        //    var nextToSearchVal = "";
        //    var regexToSearchNext ="";
        //    var lastRegexVal = "";
        //    var endSearch = false;
        //    var checkNextSectionInPara = true;
        //    var nextNotationtionToSearchVal = "";
        //    List<string> newSetOfParas = new List<string>();
        //    for (var j = 0; j < 5; j++)
        //    { // loop till 5th gen
        //        if (endSearch == true) // end the section process
        //            break;
        //        if (nextToSearchVal != "" && nextNotationtionToSearchVal != "" && regexToSearchNext != "") // check if the next srction val , notation val and next regex given or not
        //        {
        //            matchNextRegex = true;
        //            for (int q = allPara.Count-1; q >=0; q--) // loop through all the para
        //            {
        //                if (matchNextRegex == false) // to end the loop
        //                    break;
        //                var para = allPara[q];
        //                 // get the para val
        //                Regex regex = new Regex(regexToSearchNext);
        //                var match = regex.Match(para); // check if match found
        //                if (match.Success ) { // if the match found

        //                    var sectionGotInPara1 = "";
        //                    var notationGotInPara1 = "";
        //                    getNotationType(notations, match.Value.Trim(), out sectionGotInPara1, out notationGotInPara1); // get the setion val and the notation of match found
        //                    if (sectionGotInPara1 == nextToSearchVal && notationGotInPara1 == nextNotationtionToSearchVal) {// if the found section and the prev section value are of same type then move ahead
        //                        allPara.RemoveRange(q, allPara.Count - (q)); // remove the other para
        //                        matchNextRegex = true;
        //                        for (int k = allPara.Count-1; k >=0 ; k--) // again loop through all the para to get the parent section of the prev section
        //                        {
        //                            if (matchNextRegex == false)// to end the loop
        //                                break;
        //                            var para1 = allPara[k]; // get the para value
        //                            for (int l = 0; l < matchRegexNumeric.Count; l++) // match the regex
        //                            {
        //                                var regexValForNextSection = matchRegexNumeric.Values.ElementAt(l).ToString(); // get the regex
        //                                if (regexToSearchNext == regexValForNextSection || lastRegexVal == regexValForNextSection) // if the current regex and the prev regex same then continue else move ahead
        //                                    continue;
        //                                Regex regexForNextSection = new Regex(regexValForNextSection);
        //                                var matchForNextSection = regexForNextSection.Match(para1); // check if match found
        //                                if (matchForNextSection.Success) // if found
        //                                {
        //                                    lastRegexVal = regexValForNextSection;
        //                                    var saveSectionVal = "";
        //                                    if (matchForNextSection.Value.EndsWith("."))
        //                                        saveSectionVal = matchForNextSection.Value.TrimEnd('.');
        //                                    else
        //                                        saveSectionVal = matchForNextSection.Value;
        //                                    sectionList.Add(saveSectionVal.Trim());
        //                                    var sectionGotInParaVal = "";
        //                                    var notationGotInParaVal = "";
        //                                    getNotationType(notations, matchForNextSection.Value.Trim(), out sectionGotInParaVal, out notationGotInParaVal); 
        //                                    var keyToFindConnectionInPara = matchRegexNumeric.Keys.ElementAt(l);
        //                                    var getValueFromConnectionInPara = connections[keyToFindConnectionInPara];
        //                                    if (getValueFromConnectionInPara != "")
        //                                    {
        //                                        if (getTopVal[getValueFromConnectionInPara].ToString() == sectionGotInParaVal.Trim())
        //                                        {
        //                                            checkNextSectionInPara = true;
        //                                            regexToSearchNext = "";
        //                                            nextToSearchVal = "";
        //                                            nextNotationtionToSearchVal = "";
        //                                            allPara.RemoveRange(k, allPara.Count - (k));
        //                                            newSetOfParas = allPara;
        //                                            matchNextRegex = false;
        //                                            break;
        //                                        }
        //                                        else
        //                                        {
        //                                            checkNextSectionInPara = true;
        //                                            regexToSearchNext = regexValForNextSection;
        //                                            nextToSearchVal = getTopVal[getValueFromConnectionInPara].ToString();
        //                                            nextNotationtionToSearchVal = notationGotInParaVal;
        //                                            allPara.RemoveRange(k, allPara.Count - (k));
        //                                            newSetOfParas = allPara;
        //                                            matchNextRegex = false;
        //                                            break;
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        checkNextSectionInPara = false;
        //                                        nextToSearchVal = "";
        //                                        regexToSearchNext = "";
        //                                        nextNotationtionToSearchVal = "";
        //                                        newSetOfParas = null;
        //                                        endSearch = true;
        //                                        matchNextRegex = false;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else // if the sectionval and the notation is not same continue
        //                        continue;
        //                }
        //            }

        //        }
        //        else if(checkNextSectionInPara == true) {
        //            var foundFirstMatch = false;
        //            for (var h = allPara.Count - 1; h >=0; h--) {
        //                if (foundFirstMatch == true)
        //                    break;
        //                var para = allPara[h];
        //                matchNextRegex = true;
        //                for (int k = 0; k < matchRegexNumeric.Count; k++)
        //                {
        //                    if (matchNextRegex == false)
        //                        break;
        //                    var regexVal = matchRegexNumeric.Values.ElementAt(k).ToString(); // get the regex
        //                    if (lastRegexVal == regexVal)
        //                        continue;
        //                    Regex regex = new Regex(regexVal);
        //                    var match = regex.Match(para); // check if match found

        //                    if (match.Success) // if found
        //                    {
        //                        lastRegexVal = regexVal;
        //                        foundFirstMatch = true;
        //                        var saveSectionVal = "";
        //                        if (match.Value.EndsWith("."))
        //                            saveSectionVal = match.Value.TrimEnd('.');
        //                        else
        //                            saveSectionVal = match.Value;
        //                        sectionList.Add(saveSectionVal.Trim());

        //                        var sectionGot = "";
        //                        var sectionFound = match.Value.Trim();
        //                        getNotationType(notations, match.Value.Trim(), out sectionGot, out notationFoundIn);
        //                        var keyToFindConnection = matchRegexNumeric.Keys.ElementAt(k);
        //                        var getValueFromConnectionFirst = connections[keyToFindConnection];
        //                        allPara.RemoveRange(h, allPara.Count - h);
        //                        if (getValueFromConnectionFirst == "") {
        //                            nextToSearchVal = "";
        //                            regexToSearchNext = "";
        //                            nextNotationtionToSearchVal = "";
        //                            newSetOfParas = null;
        //                            endSearch = true;
        //                            matchNextRegex = false;
        //                            break;
        //                        }
        //                        else {
        //                            if (sectionGot != getTopVal[getValueFromConnectionFirst].ToString())
        //                            {
        //                                regexToSearchNext = regexVal;
        //                                nextToSearchVal = getTopVal[getValueFromConnectionFirst].ToString();
        //                                nextNotationtionToSearchVal = notationFoundIn;
        //                                allPara.RemoveRange(h, allPara.Count - (h));
        //                                matchNextRegex = false;
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                regexToSearchNext = "";
        //                                nextToSearchVal = "";
        //                                nextNotationtionToSearchVal = "";
        //                                matchNextRegex = false;
        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // combining section........
        //    var sectionListCount = 0;
        //    List<int> dataToRemove = new List<int>();
        //    foreach (var item in sectionList)
        //    {
        //        Regex regex = new Regex("^(?i)article$");
        //        var match = regex.Match(item); // check if match found
        //        if (match.Success)
        //            dataToRemove.Add(sectionListCount);
        //        else{
        //            Regex regex1 = new Regex("^(?i)section$");
        //            var match1 = regex1.Match(item); // check if match found
        //            if (match1.Success)
        //                dataToRemove.Add(sectionListCount);
        //        }
        //        sectionListCount++;
        //    }
        //    foreach (var item in dataToRemove)
        //    {
        //        sectionList.RemoveAt(item);
        //    }
        //    var finalSectionOutput = "";
        //    var sectionCount = 0;
        //    if (Int32.Parse(SectionNoCount) == 0)
        //        sectionCount = sectionList.Count;
        //    else if (Int32.Parse(SectionNoCount) <= sectionList.Count)
        //        sectionCount = Int32.Parse(SectionNoCount);
        //    else
        //        sectionCount = sectionList.Count;

        //    var sectionCountVal = 0;
        //    for (int i = sectionList.Count - 1; i >= 0; i--)
        //    {
        //        sectionCountVal++;

        //        if (finalSectionOutput != "")
        //            finalSectionOutput = finalSectionOutput + "," + sectionList[i];
        //        else
        //            finalSectionOutput = sectionList[i];
        //        if (sectionCountVal == sectionCount)
        //            break;
        //    }

        //    return finalSectionOutput;
        //}

        //// get all the notation found in the section 
        //public static void getNotationType(Dictionary<int, string> notations, string value, out string sectionGot, out string notationFoundIn)
        //{

        //    sectionGot = "";
        //    notationFoundIn = "";
        //    foreach (var item in notations) // remove the notations
        //    {
        //        string[] search = item.Value.Split(',');
        //        var found = false;
        //        var duplicateSectionGot = value;
        //        if (search.Count() > 1)
        //        {
        //            var foundBothNotation = 0;
        //            foreach (var val in search)
        //            {
        //                if ((duplicateSectionGot).IndexOf(val) == 0)
        //                {
        //                    duplicateSectionGot = duplicateSectionGot.Replace(val, "");
        //                    foundBothNotation++;
        //                }
        //                if ((duplicateSectionGot).LastIndexOf(val) == duplicateSectionGot.Length - 1)
        //                {
        //                    duplicateSectionGot = duplicateSectionGot.Replace(val, "");
        //                    foundBothNotation++;
        //                }
        //                if (foundBothNotation == search.Count())
        //                {
        //                    notationFoundIn = item.Value;
        //                    sectionGot = duplicateSectionGot.Trim();
        //                    found = true;
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var foundNotation = 0;
        //            foreach (var val in search)
        //            {
        //                if (val == "section" || val == "article")
        //                    duplicateSectionGot = duplicateSectionGot.ToLower();
        //                if ((duplicateSectionGot).IndexOf(val) == 0)
        //                {
        //                    duplicateSectionGot = duplicateSectionGot.Replace(val, "");
        //                    foundNotation++;
        //                }
        //                if ((duplicateSectionGot).LastIndexOf(val) == duplicateSectionGot.Length - 1)
        //                {
        //                    duplicateSectionGot = duplicateSectionGot.Replace(val, "");
        //                    foundNotation++;
        //                }
        //                if (foundNotation == 1)
        //                {
        //                    notationFoundIn = item.Value;
        //                    sectionGot = duplicateSectionGot.Trim();
        //                    found = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (found == true)
        //            break;
        //    }
        //}

        //// get the complete section number for SECTION......
        public static string sectionValSection(string section)
        {
            var sectionVal = "";
            Dictionary<int, string> regexDictionary = new Dictionary<int, string>();

            regexDictionary.Add(1, @"^[\s]*((?i)(section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            regexDictionary.Add(2, @"^[\s]*((?i)(sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            regexDictionary.Add(3, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
            regexDictionary.Add(4, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
            regexDictionary.Add(5, @"^[\s]*((?i)(section)[\s]*[A-Z])\s");  // section A
            regexDictionary.Add(6, @"^[\s]*((?i)(section)[\s]*[a-z])");  //      section a
            regexDictionary.Add(7, @"^[\s]*((?i)(article)[\s]*[a-z])");  //      article a
            regexDictionary.Add(8, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])");  // ARTICLE A  
            regexDictionary.Add(9, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII
            regexDictionary.Add(10, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii
            regexDictionary.Add(11, @"^[\s]*((?i)(article)\s\d)"); //   article 1,
            regexDictionary.Add(12, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1
            regexDictionary.Add(13, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            regexDictionary.Add(14, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            regexDictionary.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            regexDictionary.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            regexDictionary.Add(17, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            regexDictionary.Add(18, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            regexDictionary.Add(19, @"^[\s]*([A-Z][.])\s");  // A.

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

        // ------------------------------------------------------------------FINANCIAL--------------------------------------------------------------------------------------------

        //-------------------------------------complete date------------------------------------------------------
        // get complete date
        public static List<string> getDate(string html)
        {
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

        //get moth year or year
        public static List<string> getYear(string html)
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
                if (match.Success) {
                    if (datestring.Trim().IndexOf(match.Value.Trim()) == 0) {
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
        //-------------------------------------------------------------------------------------------

        //---------------------------------------amount----------------------------------------------------
        // get amount
        public static List<string> getCurrencyAmount(string html)
        {

            string Pattern = @"(?<SYMBOL>[$â‚¬Â£]){1}[\s]*(?<AMOUNT>[\d{1,3}(\.(\d(?:\d+\.?)*)?)?]+)";
            List<string> formattedString = new List<string>();
            foreach (Match m in Regex.Matches(html, Pattern))
            {
                formattedString.Add(m.Value);
            }
            if (formattedString.Count() == 0)
                getMultipleTextualDollar(html, out formattedString);
            return formattedString;

        }

        // word amount
        public static void getMultipleTextualDollar(string numberString, out List<string> formattedString)
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
                var indexVal = numberString.Replace(" ", "").ToLower().IndexOf("dollar" , startdollar);
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
                    startFromVal = currIndex + currKey.Length -1;
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
                            if (indexOfNumber - dollar.Length == item) {
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
                if (acc != 0) {
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
        //-------------------------------------------------------------------------------------------

        //---------------------------------------percentage----------------------------------------------------
        // get percent
        public static List<string> extractPercentage(string html)
        {
            List<string> formattedString = new List<string>();
            foreach (Match m in Regex.Matches(html, @"(\d{1,3})?(\.(\d(?:\d+\.?)*)?)?[\s]*(\%)"))
            {
                formattedString.Add(m.Value);
            }
            if (formattedString.Count() == 0)
                getMultipleTextualPercent(html, out formattedString);
            for (var i=0; i< formattedString.Count(); i++) {
                if (formattedString[i].IndexOf('.') == 0)
                {
                    formattedString[i] = "0" + formattedString[i];
                }
            }
            return formattedString;
        }
        
        // word percent 
        public static void getMultipleTextualPercent(string numberString, out List<string> formattedString)

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
                    if(numberString.ToLower().IndexOf("percent") == prevIndex + prevKey.Length)
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
        //-------------------------------------------------------------------------------------------

        //---------------------------------------days----------------------------------------------------
        // only days
        public static List<string> getDays(string html)
        {
            html = Regex.Replace(html, @"[^0-9a-zA-Z]+", " ");
            List<string> formattedString = new List<string>();
            foreach (Match m in Regex.Matches(html, @"\d{2}[\s]*days"))
            {
                formattedString.Add(m.Value);
            }
            if (formattedString.Count() == 0)
                getTextualDays(html, out formattedString);
            if(formattedString.Count() == 0)
                getMonths(html, out formattedString);
            return formattedString;
        }

        //only days word
        public static void getTextualDays(string numberString, out List<string> formattedString)
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
        //-------------------------------------------------------------------------------------------

        //-------------------------------------- months-----------------------------------------------------
        // only months
        public static void getMonths(string html, out List<string> formattedString)
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

        // only month words
        public static void getTextualMonths(string numberString, out List<string> formattedString)
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
        //-------------------------------------------------------------------------------------------

        //----------------------------------------- only days and year--------------------------------------------------
        // only year count
        public static List<string> getYearCount(string html)
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

        //only days word
        public static void getTextualYear(string numberString, out List<string> formattedString)

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
                    formattedString.Add(ss.ToString()+" years");
                }
            }
        }
        //-------------------------------------------------------------------------------------------



        // tree view of section
        public static void createTree(Dictionary<Dictionary<int, string>, int> saveSection, Dictionary<Dictionary<int, string>, int> saveSectionWithsectionNo, Dictionary<Dictionary<int, string>, int> saveSectionWithRegex, List<string> sectionNameFound, out string finalJson)
        {
             Dictionary<int, string> regexDictionary = new Dictionary<int, string>(); // check the regex 
            
            regexDictionary.Add(1, @"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)"); //    1.1  / 1.1 a)
            regexDictionary.Add(2, @"^[\s]*((?i)(section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
            regexDictionary.Add(3, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 

            Node rootTop = new Node("ROOT", -1);
            for ( int i = 0; i < saveSection.Count(); i++)
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

                var sectionPara = saveSection.ElementAt(i).Key;
                var sectionSectioNo = saveSectionWithsectionNo.ElementAt(i).Key;
                var SectionRegex = saveSectionWithRegex.ElementAt(i).Key;
                Dictionary<int, List<string>> combineSectionPara = new Dictionary<int, List<string>>();
                Dictionary<int, string> combineSectionSectioNo = new Dictionary<int, string>();
                Dictionary<int, string> combineSectionRegex = new Dictionary<int, string>();
                List<int> parentSectionNo = new List<int>();

                combinePara(sectionPara, sectionSectioNo, SectionRegex, out combineSectionPara, out combineSectionSectioNo, out combineSectionRegex, out parentSectionNo);

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

                    if (sectioNo == "" & combineSectionPara.Count() == 1)
                    {
                        root.Value = sectioNo;
                        root.Para = para;
                        root.SectionName = sectionNameFound.ElementAt(i);
                    }
                    else
                    {
                        if (j==0 & sectioNo == "")
                        {
                            root.Value = sectioNo;
                            root.Para = para;
                            root.SectionName = sectionNameFound.ElementAt(i);
                        }
                        else
                        {
                            var nodeValue = sectioNo;

                            if (stack.Count == 0)
                            {
                                var child = new Node(sectioNo, root.Level + 1, para, regex,"", parent);
                                root.Children.Add(child);
                                lastNode = child;
                                stack.Push(new KeyValuePair<string, Node>(regex, root));
                            }
                            else
                            {
                                bool differentSection = false;
                                if (matchedKeys.Contains(regex) & differentSection== false)
                                {
                                    var breakFlag = false;
                                    while (breakFlag == false)
                                    {
                                        if (stack.Count == 0)
                                            break;
                                        var top = stack.Peek();
                                        if (top.Key == regex)
                                        {
                                            var child = new Node(sectioNo, top.Value.Level + 1, para, regex,"", parent);
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
                                        var child = new Node(sectioNo, lastNode.Level + 1, para, regex,"", parent);
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
            finalJson = CreateJson(rootTop).ToString();
            //testc(rootTop);
       }

        // combine all para within section 
        public static void combinePara(Dictionary<int,string> sectionPara, Dictionary<int, string> sectionSectioNo, Dictionary<int, string> sectionRegex,out Dictionary<int, List<string>> combineSectionPara, out Dictionary<int, string> combineSectionSectioNo, out Dictionary<int, string> combineSectionRegex, out List<int> parentSectionNo)
        {
            List<string> doubleSectionRegex = new List<string>();
            doubleSectionRegex.Add(@"^(\d{1,3}\.(:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.])?(?!\S)");
            doubleSectionRegex.Add(@"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)");

            combineSectionPara = new Dictionary<int, List<string>>();
            combineSectionSectioNo =new  Dictionary<int, string>();
            combineSectionRegex = new  Dictionary<int, string>();
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
                parentCheck(regex, out parent);

                var countSectionRegexCount = 0;
                foreach (var item in doubleSectionRegex)
                {
                    countSectionRegexCount++;
                    if (item == regex)
                    {
                        bool doubleSection = false;
                        regexSelect(item, sectioNo, out List<string> sectionList, out List<string> regexList,out doubleSection);
                        if (doubleSection == true) {
                            List<string> savepara1 = new List<string>();
                            combineSectionPara.Add(count, savepara1);
                            combineSectionSectioNo.Add(count, sectionList.ElementAt(0));
                            combineSectionRegex.Add(count, regexList.ElementAt(0));
                            if(countSectionRegexCount == 2)
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

        // case handle eg:- 1.1(a) OR 1. VII)
        public static void regexSelect(string regex,string sectionNo , out List<string> sectionList, out List<string> regexList, out bool doubleSection)
        {
            doubleSection = false;
            Dictionary<int, string> regexDictionary = new Dictionary<int, string>(); // check the regex 
            regexDictionary.Add(1, @"^[\s]*([1-9]{1,3}[:])(?!\S)"); //   1:
            regexDictionary.Add(2, @"^[\s]*([1-9]{1,3}[)])(?!\S)"); //   1)
            regexDictionary.Add(3, @"^[\s]*([1-9]{1,3}[]])(?!\S)"); //   1]
            regexDictionary.Add(4, @"^[\s]*([[][1-9][]])(?!\S)"); //   [1]
            regexDictionary.Add(5, @"^[\s]*([[(][1-9][)|\]])(?!\S)"); //   (1)

            regexDictionary.Add(6, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    (xvii)
            regexDictionary.Add(7, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    xvii)
            regexDictionary.Add(8, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    [xvii]
            regexDictionary.Add(9, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    xvii]
            regexDictionary.Add(10, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)"); //    xvii:
            regexDictionary.Add(11, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)"); //    xvii.

            regexDictionary.Add(12, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)|\]](?!\S)"); //    (XVII)
            regexDictionary.Add(13, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    XVII)
            regexDictionary.Add(14, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    [XVII]
            regexDictionary.Add(15, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    XVII]
            regexDictionary.Add(16, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)"); //    XVII:
            regexDictionary.Add(17, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)"); //    XVII.

            regexDictionary.Add(18, @"^[\s]*([a-z][.])(?!\S)");  //  a.
            regexDictionary.Add(19, @"^[\s]*([a-z][:])(?!\S)");  //  a:
            regexDictionary.Add(20, @"^[\s]*([(][a-z][)|\]])(?!\S)");  //    (a)
            regexDictionary.Add(21, @"^[\s]*([a-z][)])(?!\S)");  // a)
            regexDictionary.Add(22, @"^[\s]*([[][a-z][]])(?!\S)");  //   a]
            regexDictionary.Add(23, @"^[\s]*([a-z][]])(?!\S)");  //     [a]

            regexDictionary.Add(24, @"^[\s]*([A-Z][.])(?!\S)");  // A.
            regexDictionary.Add(25, @"^[\s]*([A-Z][:])(?!\S)");  // A:
            regexDictionary.Add(26, @"^[\s]*([(][A-Z][)|\]])(?!\S)");  // (A)
            regexDictionary.Add(27, @"^[\s]*([A-Z][)])(?!\S)");  // A)
            regexDictionary.Add(28, @"^[\s]*([[][A-Z][]])(?!\S)");  // A]
            regexDictionary.Add(29, @"^[\s]*([A-Z][]])(?!\S)");  // [A]

            sectionList = new List<string>();
            regexList = new List<string>();
            
            Regex regexCheckVal = new Regex("(\\d{1,3}\\.(\\d(?:\\d+\\.?)*)?)"); // check if it has section number
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

        public static void parentCheck(string regex, out int parent)
        {
            parent = 0;
            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
            // NUMBERS
            matchRegexNumeric.Add(1, @"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)"); //    1.1  / 1.1 a)
            matchRegexNumeric.Add(2, @"^[\s]*((?i)(section)\s\d*)(?!\S)"); //    section 1
            matchRegexNumeric.Add(3, @"^[\s]*((?i)(section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
            matchRegexNumeric.Add(4, @"^[\s]*((?i)(article)\s\d*)(?!\S)"); //   article 1
            matchRegexNumeric.Add(5, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
            matchRegexNumeric.Add(6, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    section xvii
            matchRegexNumeric.Add(7, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    article xvii
            matchRegexNumeric.Add(8, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    section XVII
            matchRegexNumeric.Add(9, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    article XVII
            matchRegexNumeric.Add(10, @"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)");  //      section a
            matchRegexNumeric.Add(11, @"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)");  //      article a
            matchRegexNumeric.Add(12, @"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)");  // section A
            matchRegexNumeric.Add(13, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)");  // ARTICLE A

            foreach (var item in matchRegexNumeric)
            {
                if (regex == item.Value)
                {
                    parent = 1;
                    break;
                }
            }
        }

        // tree view to json
        public static JObject CreateJson(Node node)
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
            jo["para"] = jaPara;
            jo["children"] = ja;
            jo["parentCheck"] = node.ParentCheck;
            return jo;
        }

        // check before and after section number
        ////public static void checkSibling( string sectioNo, string regex, List<string> allSection, List<string> allregex, out bool differentSection)
        ////{
        ////    differentSection = false;
        ////    List<string> uppercaseAlpha = new List<string>();
        ////    uppercaseAlpha.Add("A");
        ////    uppercaseAlpha.Add("B");
        ////    uppercaseAlpha.Add("C");
        ////    uppercaseAlpha.Add("D");
        ////    uppercaseAlpha.Add("E");
        ////    uppercaseAlpha.Add("F");
        ////    uppercaseAlpha.Add("G");
        ////    uppercaseAlpha.Add("H");
        ////    uppercaseAlpha.Add("I");
        ////    uppercaseAlpha.Add("J");
        ////    uppercaseAlpha.Add("K");
        ////    uppercaseAlpha.Add("L");
        ////    uppercaseAlpha.Add("M");
        ////    uppercaseAlpha.Add("N");
        ////    uppercaseAlpha.Add("O");
        ////    uppercaseAlpha.Add("P");
        ////    uppercaseAlpha.Add("Q");
        ////    uppercaseAlpha.Add("R");
        ////    uppercaseAlpha.Add("S");
        ////    uppercaseAlpha.Add("T");
        ////    uppercaseAlpha.Add("U");
        ////    uppercaseAlpha.Add("V");
        ////    uppercaseAlpha.Add("W");
        ////    uppercaseAlpha.Add("X");
        ////    uppercaseAlpha.Add("Y");
        ////    uppercaseAlpha.Add("Z");

        ////    List<string> lowercaseAlpha = new List<string>();
        ////    lowercaseAlpha.Add("a");
        ////    lowercaseAlpha.Add("b");
        ////    lowercaseAlpha.Add("c");
        ////    lowercaseAlpha.Add("d");
        ////    lowercaseAlpha.Add("e");
        ////    lowercaseAlpha.Add("f");
        ////    lowercaseAlpha.Add("g");
        ////    lowercaseAlpha.Add("h");
        ////    lowercaseAlpha.Add("i");
        ////    lowercaseAlpha.Add("j");
        ////    lowercaseAlpha.Add("k");
        ////    lowercaseAlpha.Add("l");
        ////    lowercaseAlpha.Add("m");
        ////    lowercaseAlpha.Add("n");
        ////    lowercaseAlpha.Add("o");
        ////    lowercaseAlpha.Add("p");
        ////    lowercaseAlpha.Add("q");
        ////    lowercaseAlpha.Add("r");
        ////    lowercaseAlpha.Add("s");
        ////    lowercaseAlpha.Add("t");
        ////    lowercaseAlpha.Add("u");
        ////    lowercaseAlpha.Add("v");
        ////    lowercaseAlpha.Add("w");
        ////    lowercaseAlpha.Add("x");
        ////    lowercaseAlpha.Add("y");
        ////    lowercaseAlpha.Add("z");

        ////    List<string> number = new List<string>();
        ////    number.Add("1");
        ////    number.Add("2");
        ////    number.Add("3");
        ////    number.Add("4");
        ////    number.Add("5");
        ////    number.Add("6");
        ////    number.Add("7");
        ////    number.Add("8");
        ////    number.Add("9");
        ////    number.Add("10");
        ////    number.Add("11");
        ////    number.Add("12");
        ////    number.Add("13");
        ////    number.Add("14");
        ////    number.Add("15");
        ////    number.Add("16");
        ////    number.Add("17");
        ////    number.Add("18");
        ////    number.Add("19");
        ////    number.Add("20");

        ////    List<string> upperCaseRoman = new List<string>();
        ////    upperCaseRoman.Add("I");
        ////    upperCaseRoman.Add("II");
        ////    upperCaseRoman.Add("III");
        ////    upperCaseRoman.Add("IV");
        ////    upperCaseRoman.Add("V");
        ////    upperCaseRoman.Add("VI");
        ////    upperCaseRoman.Add("VII");
        ////    upperCaseRoman.Add("VIII");
        ////    upperCaseRoman.Add("IX");
        ////    upperCaseRoman.Add("X");
        ////    upperCaseRoman.Add("XI");
        ////    upperCaseRoman.Add("XII");
        ////    upperCaseRoman.Add("XIII");
        ////    upperCaseRoman.Add("XIV");
        ////    upperCaseRoman.Add("XV");
        ////    upperCaseRoman.Add("XVI");
        ////    upperCaseRoman.Add("XVII");
        ////    upperCaseRoman.Add("XVIII");
        ////    upperCaseRoman.Add("XIX");
        ////    upperCaseRoman.Add("XX");

        ////    List<string> lowerCaseRoman = new List<string>();
        ////    lowerCaseRoman.Add("i");
        ////    lowerCaseRoman.Add("ii");
        ////    lowerCaseRoman.Add("iii");
        ////    lowerCaseRoman.Add("iv");
        ////    lowerCaseRoman.Add("v");
        ////    lowerCaseRoman.Add("vi");
        ////    lowerCaseRoman.Add("vii");
        ////    lowerCaseRoman.Add("viii");
        ////    lowerCaseRoman.Add("ix");
        ////    lowerCaseRoman.Add("x");
        ////    lowerCaseRoman.Add("xi");
        ////    lowerCaseRoman.Add("xii");
        ////    lowerCaseRoman.Add("xiii");
        ////    lowerCaseRoman.Add("xiv");
        ////    lowerCaseRoman.Add("xv");
        ////    lowerCaseRoman.Add("xvi");
        ////    lowerCaseRoman.Add("xvii");
        ////    lowerCaseRoman.Add("xviii");
        ////    lowerCaseRoman.Add("xix");
        ////    lowerCaseRoman.Add("xx");


        ////    Dictionary<string, int> regexCheck = new Dictionary<string, int>();
        ////    regexCheck.Add(@"^[\s]*((?i)(section)\s\d*)(?!\S)", 1); //    section 1
        ////    regexCheck.Add(@"^[\s]*((?i)(article)\s\d*)(?!\S)", 1); //   article 1
        ////    regexCheck.Add(@"^[\s]*([0-9]{1,3}[:])(?!\S)", 1); //   1:
        ////    regexCheck.Add(@"^[\s]*([0-9]{1,3}[)])(?!\S)", 1); //   1)
        ////    regexCheck.Add(@"^[\s]*([0-9]{1,3}[]])(?!\S)", 1); //   1]
        ////    regexCheck.Add( @"^[\s]*([[]+[0-9]+[]])(?!\S)", 1); //   [1]
        ////    regexCheck.Add( @"^[\s]*([[(]+[0-9]+[)])(?!\S)", 1); //   (1)

        ////    regexCheck.Add(@"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)", 2); //    (xvii)
        ////    regexCheck.Add(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)", 2); //    xvii)
        ////    regexCheck.Add(@"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)", 2); //    [xvii]
        ////    regexCheck.Add(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)", 2); //    xvii]
        ////    regexCheck.Add(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)", 2); //    xvii:
        ////    regexCheck.Add(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)", 2); //    xvii.
        ////    regexCheck.Add(@"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)", 2); //    section xvii
        ////    regexCheck.Add(@"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)", 2); //    article xvii

        ////    regexCheck.Add(@"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)", 3); //    (XVII)
        ////    regexCheck.Add(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)", 3); //    XVII)
        ////    regexCheck.Add(@"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)", 3); //    [XVII]
        ////    regexCheck.Add(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)", 3); //    XVII]
        ////    regexCheck.Add(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)", 3); //    XVII:
        ////    regexCheck.Add(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)", 3); //    XVII.
        ////    regexCheck.Add(@"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)", 3); //    section XVII
        ////    regexCheck.Add(@"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)", 3); //    article XVII

        ////    regexCheck.Add(@"^[\s]*([a-z][.])(?!\S)", 4);  //  a.
        ////    regexCheck.Add(@"^[\s]*([a-z][:])(?!\S)", 4);  //  a:
        ////    regexCheck.Add(@"^[\s]*([(][a-z][)])(?!\S)", 4);  //    (a)
        ////    regexCheck.Add(@"^[\s]*([a-z][)])(?!\S)", 4);  // a)
        ////    regexCheck.Add(@"^[\s]*([[][a-z][]])(?!\S)", 4);  //   a]
        ////    regexCheck.Add(@"^[\s]*([a-z][]])(?!\S)", 4);  //     [a]
        ////    regexCheck.Add(@"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)", 4);  //      section a
        ////    regexCheck.Add(@"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)", 4);  //      article a

        ////    regexCheck.Add(@"^[\s]*([A-Z][.])(?!\S)", 5);  // A.
        ////    regexCheck.Add(@"^[\s]*([A-Z][:])(?!\S)", 5);  // A:
        ////    regexCheck.Add(@"^[\s]*([(][A-Z][)])(?!\S)", 5);  // (A)
        ////    regexCheck.Add(@"^[\s]*([A-Z][)])(?!\S)", 5);  // A)
        ////    regexCheck.Add(@"^[\s]*([[][A-Z][]])(?!\S)", 5);  // A]
        ////    regexCheck.Add(@"^[\s]*([A-Z][]])(?!\S)", 5);  // [A]
        ////    regexCheck.Add(@"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)", 5);  // section A
        ////    regexCheck.Add(@"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)", 5);  // ARTICLE A

        ////    Dictionary<int, List<string>> regexMatch = new Dictionary<int, List<string>>();
        ////    regexMatch.Add(1, number);  // A.
        ////    regexMatch.Add(2, lowerCaseRoman);  // A:
        ////    regexMatch.Add(3, upperCaseRoman);  // (A)
        ////    regexMatch.Add(4, lowercaseAlpha);  // A)
        ////    regexMatch.Add(5, uppercaseAlpha);  // A]

        ////    Dictionary<int, string> getValueRegex = new Dictionary<int, string>();
        ////    getValueRegex.Add(1, @"[0-9]{1,3}");  // A.
        ////    getValueRegex.Add(2, @"(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}");  // A:
        ////    getValueRegex.Add(3, @"(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}");  // (A)
        ////    getValueRegex.Add(4, @"[a-zA-Z]");  // A)
        ////    getValueRegex.Add(5, @"[a-zA-Z]");  // A]

        ////    var sectionNoCheck = ""; // get only the section number
        ////    foreach (var item in regexCheck) // loop through all the regex 
        ////    {
        ////        if (item.Key == regex) // regex match found
        ////        {
        ////            var getRegexVal = item.Value; // get the value for regex to get the data set
        ////            var dataSet = regexMatch.FirstOrDefault(x => x.Key == getRegexVal).Value; // get the data set
        ////            onlySectionVal(getRegexVal, getValueRegex, sectioNo, out sectionNoCheck);
        ////            var indexCurrentSection = dataSet.IndexOf(sectionNoCheck);// get index of it from data set;
        ////            var result = Enumerable.Range(0, allregex.Count).Where(i => allregex[i] == regex).ToList();// get all the index with same regex
        ////            if (result.Count >= 2 & indexCurrentSection !=0)
        ////            {
        ////                var lastSectionNo = result[result.Count - 2];// get the last index
        ////                var getLastIndexVal = allSection.ElementAt(lastSectionNo); //get the last index value
        ////                var getOnlySectionVal = "";
        ////                onlySectionVal(getRegexVal, getValueRegex, getLastIndexVal, out getOnlySectionVal); // get the last section only value
        ////                var getSectionNoIndex = dataSet.IndexOf(getOnlySectionVal);// get index of it from data set
        ////                if (dataSet.ElementAt(indexCurrentSection - 1) == getOnlySectionVal)
        ////                    break;

        ////                var topNlist = dataSet.Take(getSectionNoIndex + 1).ToList();// get all value above it
        ////                var sectionInDataSet = topNlist.FirstOrDefault(stringToCheck => stringToCheck.Contains(sectionNoCheck)); // get the value if its there in list
        ////                if (sectionInDataSet != null)
        ////                {
        ////                    differentSection = true;
        ////                }
        ////            }
        ////            else if (result.Count == 1)
        ////            {
        ////                var lastSectionNo = result[result.Count - 1];// get the last index
        ////                var getLastIndexVal = allSection.ElementAt(lastSectionNo); //get the last index value
        ////                var getOnlySectionVal = "";
        ////                onlySectionVal(getRegexVal, getValueRegex, getLastIndexVal, out getOnlySectionVal); // get the last section only value
        ////                if(getOnlySectionVal == sectionNoCheck)
        ////                    differentSection = true;
        ////            }
        ////            break; 
        ////        }

        ////    }
        ////}

        ////// remove section/article/special character from section value
        ////public static void onlySectionVal(int getRegexVal,Dictionary<int,string> getValueRegex,string sectioNo, out string sectionNoCheck)
        ////{
        ////    sectionNoCheck = "";
        ////    Regex regexCheckVal = new Regex("(?i)(section|article)"); // check if it has section number
        ////    var match = regexCheckVal.Match(sectioNo); // check if match found
        ////    if (match.Success) // if found then replace it
        ////    {
        ////        sectionNoCheck = sectioNo.Replace(match.Value, "").Replace(match.Value, "").Trim();
        ////    }
        ////    else // replace the special character in it
        ////    {
        ////        var getRegexForValueSearch = getValueRegex.FirstOrDefault(x => x.Key == getRegexVal).Value;
        ////        Regex regexCheckValue = new Regex(getRegexForValueSearch);
        ////        var matchFound = regexCheckValue.Match(sectioNo); // check if match found
        ////        if (matchFound.Success)
        ////            sectionNoCheck = matchFound.Value;
        ////    }
        ////}
    }
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



    //Dictionary<string, string> checkPrevSection = new Dictionary<string, string>(); // check the words after section number
    //checkPrevSection.Add("i", "j");
    //        checkPrevSection.Add("v", "u");
    //        checkPrevSection.Add("x", "v");
    //        checkPrevSection.Add("I", "J");
    //        checkPrevSection.Add("V", "U");
    //        checkPrevSection.Add("X", "V");

    //        Dictionary<string, string> checkPrevSectionRegex = new Dictionary<string, string>(); // check the words after section number
    //checkPrevSection.Add("i", "j");
    //        checkPrevSection.Add("v", "u");
    //        checkPrevSection.Add("x", "v");
    //        checkPrevSection.Add("I", "J");
    //        checkPrevSection.Add("V", "U");
    //        checkPrevSection.Add("X", "V");
}