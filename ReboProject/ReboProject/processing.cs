using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web.Configuration;

namespace ReboProject
{
    public class processing
    {
        // get the complete section once
        public static List<string> getSectionForPara(string para)
        {
            List<string> sectiongot = new List<string>();

            Dictionary<int, string> regexDictionary = new Dictionary<int, string>();

            regexDictionary.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            regexDictionary.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            regexDictionary.Add(3, @"^[\s]*((?i)(section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            regexDictionary.Add(4, @"^[\s]*((?i)(sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            regexDictionary.Add(5, @"^[\s]*((?i)(article)\s\d)"); //   article 1,
            regexDictionary.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1 
            regexDictionary.Add(7, @"^[\s]*(\d{1,3}[:])\s"); //   1:
            regexDictionary.Add(8, @"^[\s]*(\d{1,3}[)])\s"); //   1)
            regexDictionary.Add(9, @"^[\s]*(\d{1,3}[]])\s"); //   1]
            regexDictionary.Add(10, @"^[\s]*([[]+\d+[]])\s"); //   [1]
            regexDictionary.Add(11, @"^[\s]*([[(]+\d+[)])\s"); //   (1)

            regexDictionary.Add(12, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            regexDictionary.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            regexDictionary.Add(14, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            regexDictionary.Add(15, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            regexDictionary.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            regexDictionary.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            regexDictionary.Add(18, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            regexDictionary.Add(19, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
            regexDictionary.Add(20, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii

            regexDictionary.Add(21, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            regexDictionary.Add(22, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            regexDictionary.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            regexDictionary.Add(24, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            regexDictionary.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            regexDictionary.Add(26, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            regexDictionary.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            regexDictionary.Add(28, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
            regexDictionary.Add(29, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII

            regexDictionary.Add(30, @"^[\s]*([a-z][.])\s");  //  a.
            regexDictionary.Add(31, @"^[\s]*([a-z][:])\s");  //  a:
            regexDictionary.Add(32, @"^[\s]*([(][a-z][)])\s");  //    (a)
            regexDictionary.Add(33, @"^[\s]*([a-z][)])\s");  // a)
            regexDictionary.Add(34, @"^[\s]*([[][a-z][]])\s");  //   a]
            regexDictionary.Add(35, @"^[\s]*([a-z][]])\s");  //     [a]
            regexDictionary.Add(36, @"^[\s]*((?i)(section)[\s]*[a-z])");  //      section a
            regexDictionary.Add(37, @"^[\s]*((?i)(article)[\s]*[a-z])");  //      article a
            regexDictionary.Add(38, @"^[\s]*([A-Z][.])\s");  // A.
            regexDictionary.Add(39, @"^[\s]*([A-Z][:])\s");  // A:
            regexDictionary.Add(40, @"^[\s]*([(][A-Z][)])\s");  // (A)
            regexDictionary.Add(41, @"^[\s]*([A-Z][)])\s");  // A)
            regexDictionary.Add(42, @"^[\s]*([[][A-Z][]])\s");  // A]
            regexDictionary.Add(43, @"^[\s]*([A-Z][]])\s");  // [A]
            regexDictionary.Add(44, @"^[\s]*((?i)(section)[\s]*[A-Z])\s");  // section A
            regexDictionary.Add(45, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])");  // ARTICLE A  

            foreach (var item in regexDictionary)
            {
                Regex regex = new Regex(item.Value);
                var match = regex.Match(para); // check if match found
                if (match.Success)
                {
                    sectiongot.Add(match.Value.Trim());
                    sectiongot.Add(item.Value);
                    break;
                }
            }
            if (sectiongot.Count == 0)// if section not found for para then section = null
            {
                sectiongot.Add(null);
                sectiongot.Add(null);
            }

            return sectiongot;
        }

        public static string getCompleteParaSection(string SectionNoCount, JArray ja2, Dictionary<int, Dictionary<int, string>> saveSectionNoAllFiles, Dictionary<Dictionary<int, string>, int> saveAllSection, string outputPara, string DefaultSectionName, JToken SectionName)
        {
            var jarrayVal = ja2;
            var pageNo = (int)ja2[0]["pageNo"]; // get the pageno of output
            var paraNo = (int)ja2[0]["paraNo"]; // 
            var readNextPara = (int)ja2[0]["readNextPara"]; // 
            var sectionNo = ja2[0]["sectionVal"].ToString();
            var foundPara = "";
            if (outputPara == "")
                foundPara = ja2[0]["output"].ToString();
            else
                foundPara = outputPara;
            var finalSectionOutput = "";
            var checkNextSection = true;
            var getFirstLine = "";
            for (int i = 0; i < saveAllSection.Count; i++)
            {
                List<string> allPara = new List<string>();
                if (checkNextSection == false)
                    break;
                var sectionDictionary = saveAllSection.Keys.ElementAt(i);
                for (int j = 0; j < sectionDictionary.Count; j++)
                {
                    //(sectionDictionary.Values.ElementAt(j).ToString().Trim() == foundPara.Trim())
                    if (readNextPara == 1)
                    {
                        if (foundPara.IndexOf(sectionDictionary.Values.ElementAt(j).ToString()) != -1)
                        {
                            if (sectionDictionary.Values.ElementAt(j+1).ToString().Trim().Length >= Int32.Parse(WebConfigurationManager.AppSettings["StringLength"]))
                            {
                                if ((sectionDictionary.Values.ElementAt(j).ToString() + sectionDictionary.Values.ElementAt(j+1).ToString()).Trim() == foundPara.Trim()) {
                                    getFirstLine = sectionDictionary.Values.ElementAt(0).ToString().Trim();
                                    allPara.Add(sectionDictionary.Values.ElementAt(j).ToString());
                                    finalSectionOutput = SectionValParagraph(SectionNoCount, allPara, sectionDictionary.Values.ElementAt(j).ToString());
                                    checkNextSection = false;
                                    break;
                                }
                            }
                            else {
                                if ((sectionDictionary.Values.ElementAt(j).ToString() + sectionDictionary.Values.ElementAt(j + 2).ToString()).Trim() == foundPara.Trim()) {
                                    getFirstLine = sectionDictionary.Values.ElementAt(0).ToString().Trim();
                                    allPara.Add(sectionDictionary.Values.ElementAt(j).ToString());
                                    finalSectionOutput = SectionValParagraph(SectionNoCount, allPara, sectionDictionary.Values.ElementAt(j).ToString());
                                    checkNextSection = false;
                                    break;
                                }
                            }
                        }
                        else
                            allPara.Add(sectionDictionary.Values.ElementAt(j).ToString());
                    }
                    else {
                        if (sectionDictionary.Values.ElementAt(j).ToString().Trim().Contains(foundPara.Trim()))
                        {
                            getFirstLine = sectionDictionary.Values.ElementAt(0).ToString().Trim();
                            allPara.Add(sectionDictionary.Values.ElementAt(j).ToString());
                            finalSectionOutput = SectionValParagraph(SectionNoCount, allPara, sectionDictionary.Values.ElementAt(j).ToString());
                            checkNextSection = false;
                            break;
                        }
                        else
                            allPara.Add(sectionDictionary.Values.ElementAt(j).ToString());
                    }
                }
            }

            var toSearch = "";
            Dictionary<int, string> regexDictionary = new Dictionary<int, string>();
            regexDictionary.Add(1, "[\\s]*([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]*|\\d{0,2}[\\s]*)$"); //    1.
            regexDictionary.Add(2, "[\\s]*([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}|\\d{0,2})$"); //    1.1 
            regexDictionary.Add(3, "[\\s]*[\"]([a-zA-Z]{1}[\\s]*|\\d{0,2}[\\s]*)[\"]$"); //    1.1 
            regexDictionary.Add(4, "[\\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\\s]?"); //    1.1 
            //if (!String.IsNullOrEmpty(finalSectionOutput)) {
                var foundSectionName = false;
                foreach (var item in SectionName)
                {
                    if (foundSectionName == false) {
                        foreach (var regexVal in regexDictionary)
                        {
                            toSearch = item["keyword"].ToString();
                            Regex regex1 = new Regex("^(?i)" + toSearch + regexVal.Value);
                            var match1 = regex1.Match(getFirstLine); // check if match found
                            if (match1.Success)
                            {
                                foundSectionName = true;
                                finalSectionOutput = match1.Value + " " + finalSectionOutput;
                                break;
                            }
                        }
                    }
                }
                if(foundSectionName == false)
                    finalSectionOutput = DefaultSectionName + " " + finalSectionOutput;
           // }
            return finalSectionOutput;
        }

        public static string SectionValParagraph(string SectionNoCount, List<string> allPara, string ouputPara)
        {
            List<string> sectionList = new List<string>();
            Dictionary<string, string> getTopVal = new Dictionary<string, string>();
            getTopVal.Add("lowCaseAlpha", "a");
            getTopVal.Add("upCaseAlpha", "A");
            getTopVal.Add("lowCaseNumeric", "i");
            getTopVal.Add("upCaseNumeric", "I");
            getTopVal.Add("numberVal", "1");

            Dictionary<int, string> notations = new Dictionary<int, string>();
            notations.Add(1, "(,)");
            notations.Add(2, "[,]");
            notations.Add(3, "]");
            notations.Add(4, ")");
            notations.Add(5, ".");
            notations.Add(6, ":");
            notations.Add(7, "section");
            notations.Add(8, "article");

            Dictionary<int, string> matchRegexNumericSection = new Dictionary<int, string>();
            matchRegexNumericSection.Add(4, @"^[\s]*((?i)(section)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            matchRegexNumericSection.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            matchRegexNumericSection.Add(3, @"^[\s]*((?i)(section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            matchRegexNumericSection.Add(5, @"^[\s]*((?i)(article)\s\d)"); //   article 1,
            matchRegexNumericSection.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1 

            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
            // NUMBERS
            matchRegexNumeric.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            matchRegexNumeric.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            matchRegexNumeric.Add(3, @"^[\s]*((?i)(section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            matchRegexNumeric.Add(4, @"^[\s]*((?i)(section)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            matchRegexNumeric.Add(5, @"^[\s]*((?i)(article)\s\d{0,3})"); //   article 1,
            matchRegexNumeric.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1 
            matchRegexNumeric.Add(7, @"^[\s]*(\d{1,3}[:])\s"); //   1:
            matchRegexNumeric.Add(8, @"^[\s]*(\d{1,3}[)])\s"); //   1)
            matchRegexNumeric.Add(9, @"^[\s]*(\d{1,3}[]])\s"); //   1]
            matchRegexNumeric.Add(10, @"^[\s]*([[]+\d+[]])\s"); //   [1]
            matchRegexNumeric.Add(11, @"^[\s]*([[(]+\d+[)])\s"); //   (1)
            
            matchRegexNumeric.Add(12, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            matchRegexNumeric.Add(14, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            matchRegexNumeric.Add(15, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            matchRegexNumeric.Add(18, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            matchRegexNumeric.Add(19, @"^[\s]*((?i)(section))[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
            matchRegexNumeric.Add(20, @"^[\s]*((?i)(article))[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii
            
            matchRegexNumeric.Add(21, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            matchRegexNumeric.Add(22, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); // XVII.
            matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            matchRegexNumeric.Add(24, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            matchRegexNumeric.Add(26, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            matchRegexNumeric.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            matchRegexNumeric.Add(28, @"^[\s]*((?i)(section))[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
            matchRegexNumeric.Add(29, @"^[\s]*((?i)(article))[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII
            
            matchRegexNumeric.Add(30, @"^[\s]*([a-z][.])\s");  //  a.
            matchRegexNumeric.Add(31, @"^[\s]*([a-z][:])\s");  //  a:
            matchRegexNumeric.Add(32, @"^[\s]*([(][a-z][)])\s");  //    (a)
            matchRegexNumeric.Add(33, @"^[\s]*([a-z][)])\s");  // a)
            matchRegexNumeric.Add(34, @"^[\s]*([[][a-z][]])\s");  //   a]
            matchRegexNumeric.Add(35, @"^[\s]*([a-z][]])\s");  //     [a]
            matchRegexNumeric.Add(36, @"^[\s]*(((?i)(section))[\s]*[a-z])");  //      section a
            matchRegexNumeric.Add(37, @"^[\s]*(((?i)(article))[\s]*[a-z])");  //      article a
            matchRegexNumeric.Add(38, @"^[\s]*([A-Z][.])\s");  // A.
            matchRegexNumeric.Add(39, @"^[\s]*([A-Z][:])\s");  // A:
            matchRegexNumeric.Add(40, @"^[\s]*([(][A-Z][)])\s");  // (A)
            matchRegexNumeric.Add(41, @"^[\s]*([A-Z][)])\s");  // A)
            matchRegexNumeric.Add(42, @"^[\s]*([[][A-Z][]])\s");  // A]
            matchRegexNumeric.Add(43, @"^[\s]*([A-Z][]])\s");  // [A]
            matchRegexNumeric.Add(44, @"^[\s]*(((?i)(section))[\s]*[A-Z])\s");  // section A
            matchRegexNumeric.Add(45, @"^[\s]*(((?i)(ARTICLE))[\s]*[A-Z])");  // ARTICLE A  

            
            Dictionary<int, string> connections = new Dictionary<int, string>();

            connections.Add(1, "numberVal"); //    1.
            connections.Add(2, ""); //    1.1 
            connections.Add(3, ""); //    section 1.
            connections.Add(4, ""); //    section 1.1 
            connections.Add(5, ""); //   article 1,
            connections.Add(6, ""); //  article 1.1 
            connections.Add(7, "numberVal"); //   1:
            connections.Add(8, "numberVal"); //   1)
            connections.Add(9, "numberVal"); //   1]
            connections.Add(10, "numberVal"); //   [1]
            connections.Add(11, "numberVal"); //   (1)
            
            connections.Add(12, "lowCaseNumeric"); //    (xvii)
            connections.Add(13, "lowCaseNumeric"); //    xvii
            connections.Add(14, "lowCaseNumeric"); //    xvii)
            connections.Add(15, "lowCaseNumeric"); //    [xvii]
            connections.Add(16, "lowCaseNumeric"); //    xvii]
            connections.Add(17, "lowCaseNumeric"); //    xvii:
            connections.Add(18, "lowCaseNumeric"); //    xvii.
            connections.Add(19, ""); //    section xvii
            connections.Add(20, ""); //    article xvii
            
            connections.Add(21, "upCaseNumeric"); //    (XVII)
            connections.Add(22, "upCaseNumeric"); //    XVII
            connections.Add(23, "upCaseNumeric"); //    XVII)
            connections.Add(24, "upCaseNumeric"); //    [XVII]
            connections.Add(25, "upCaseNumeric"); //    XVII]
            connections.Add(26, "upCaseNumeric"); //    XVII:
            connections.Add(27, "upCaseNumeric"); //    XVII.
            connections.Add(28, ""); //    section XVII
            connections.Add(29, ""); //    article XVII
            
            connections.Add(30, "lowCaseAlpha");  //  a.
            connections.Add(31, "lowCaseAlpha");  //  a:
            connections.Add(32, "lowCaseAlpha");  //    (a)
            connections.Add(33, "lowCaseAlpha");  // a)
            connections.Add(34, "lowCaseAlpha");  //   a]
            connections.Add(35, "lowCaseAlpha");  //     [a]
            connections.Add(36, "");  //      section a
            connections.Add(37, "");  //      article a
            
            connections.Add(38, "upCaseAlpha");  // A.
            connections.Add(39, "upCaseAlpha");  // A:
            connections.Add(40, "upCaseAlpha");  // (A)
            connections.Add(41, "upCaseAlpha");  // A)
            connections.Add(42, "upCaseAlpha");  // A]
            connections.Add(43, "upCaseAlpha");  // [A]
            connections.Add(44, "");  // section A
            connections.Add(45, "");  // ARTICLE A  

            
            List<string> allSectionVal = new List<string>();

            
            var notationFoundIn = "";
            var checkNextPara = true;
            var matchNextRegex = true;
            var nextToSearchVal = "";
            var regexToSearchNext ="";
            var lastRegexVal = "";
            var endSearch = false;
            var checkNextSectionInPara = true;
            var nextNotationtionToSearchVal = "";
            List<string> newSetOfParas = new List<string>();
            for (var j = 0; j < 5; j++)
            { // loop till 5th gen
                if (endSearch == true) // end the section process
                    break;
                if (nextToSearchVal != "" && nextNotationtionToSearchVal != "" && regexToSearchNext != "") // check if the next srction val , notation val and next regex given or not
                {
                    matchNextRegex = true;
                    for (int q = allPara.Count-1; q >=0; q--) // loop through all the para
                    {
                        if (matchNextRegex == false) // to end the loop
                            break;
                        var para = allPara[q];
                        var secondregexMatch = "";
                        var secondOutputFound = "";
                         // get the para val
                        Regex regex = new Regex(regexToSearchNext);
                        var match = regex.Match(para); // check if match found
                        if (match.Success ) { // if the match found

                            var sectionGotInPara1 = "";
                            var notationGotInPara1 = "";
                            getNotationType(notations, match.Value.Trim(), out sectionGotInPara1, out notationGotInPara1); // get the setion val and the notation of match found
                            if (sectionGotInPara1 == nextToSearchVal && notationGotInPara1 == nextNotationtionToSearchVal) {// if the found section and the prev section value are of same type then move ahead
                                allPara.RemoveRange(q, allPara.Count - (q)); // remove the other para
                                matchNextRegex = true;
                                for (int k = allPara.Count-1; k >=0 ; k--) // again loop through all the para to get the parent section of the prev section
                                {
                                    if (matchNextRegex == false)// to end the loop
                                        break;
                                    var para1 = allPara[k]; // get the para value
                                    for (int l = 0; l < matchRegexNumeric.Count; l++) // match the regex
                                    {
                                        var regexValForNextSection = matchRegexNumeric.Values.ElementAt(l).ToString(); // get the regex
                                        if (regexToSearchNext == regexValForNextSection || lastRegexVal == regexValForNextSection) // if the current regex and the prev regex same then continue else move ahead
                                            continue;
                                        Regex regexForNextSection = new Regex(regexValForNextSection);
                                        var matchForNextSection = regexForNextSection.Match(para1); // check if match found
                                        if (matchForNextSection.Success) // if found
                                        {
                                            lastRegexVal = regexValForNextSection;
                                            sectionList.Add(matchForNextSection.Value.Trim());
                                            var sectionGotInParaVal = "";
                                            var notationGotInParaVal = "";
                                            getNotationType(notations, matchForNextSection.Value.Trim(), out sectionGotInParaVal, out notationGotInParaVal); 
                                            var keyToFindConnectionInPara = matchRegexNumeric.Keys.ElementAt(l);
                                            var getValueFromConnectionInPara = connections[keyToFindConnectionInPara];
                                            if (getValueFromConnectionInPara != "")
                                            {
                                                if (getTopVal[getValueFromConnectionInPara].ToString() == sectionGotInParaVal.Trim())
                                                {
                                                    checkNextSectionInPara = true;
                                                    regexToSearchNext = "";
                                                    nextToSearchVal = "";
                                                    nextNotationtionToSearchVal = "";
                                                    allPara.RemoveRange(k, allPara.Count - (k));
                                                    newSetOfParas = allPara;
                                                    matchNextRegex = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    checkNextSectionInPara = true;
                                                    regexToSearchNext = regexValForNextSection;
                                                    nextToSearchVal = getTopVal[getValueFromConnectionInPara].ToString();
                                                    nextNotationtionToSearchVal = notationGotInParaVal;
                                                    allPara.RemoveRange(k, allPara.Count - (k));
                                                    newSetOfParas = allPara;
                                                    matchNextRegex = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                checkNextSectionInPara = false;
                                                nextToSearchVal = "";
                                                regexToSearchNext = "";
                                                nextNotationtionToSearchVal = "";
                                                newSetOfParas = null;
                                                endSearch = true;
                                                matchNextRegex = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else // if the sectionval and the notation is not same continue
                                continue;
                        }
                    }

                }
                else if(checkNextSectionInPara == true) {
                    var foundFirstMatch = false;
                    for (var h = allPara.Count - 1; h >=0; h--) {
                        if (foundFirstMatch == true)
                            break;
                        var para = allPara[h];
                        matchNextRegex = true;
                        for (int k = 0; k < matchRegexNumeric.Count; k++)
                        {
                            if (matchNextRegex == false)
                                break;
                            var regexVal = matchRegexNumeric.Values.ElementAt(k).ToString(); // get the regex
                            if (lastRegexVal == regexVal)
                                continue;
                            Regex regex = new Regex(regexVal);
                            var match = regex.Match(para); // check if match found
                            
                            if (match.Success) // if found
                            {
                                lastRegexVal = regexVal;
                                foundFirstMatch = true;
                                sectionList.Add(match.Value.Trim());
                                
                                var sectionGot = "";
                                var sectionFound = match.Value.Trim();
                                getNotationType(notations, match.Value.Trim(), out sectionGot, out notationFoundIn);
                                var keyToFindConnection = matchRegexNumeric.Keys.ElementAt(k);
                                var getValueFromConnectionFirst = connections[keyToFindConnection];
                                allPara.RemoveRange(h, allPara.Count - h);
                                if (getValueFromConnectionFirst == "") {
                                    nextToSearchVal = "";
                                    regexToSearchNext = "";
                                    nextNotationtionToSearchVal = "";
                                    newSetOfParas = null;
                                    endSearch = true;
                                    matchNextRegex = false;
                                    break;
                                }
                                else {
                                    if (sectionGot != getTopVal[getValueFromConnectionFirst].ToString())
                                    {
                                        regexToSearchNext = regexVal;
                                        nextToSearchVal = getTopVal[getValueFromConnectionFirst].ToString();
                                        nextNotationtionToSearchVal = notationFoundIn;
                                        allPara.RemoveRange(h, allPara.Count - (h));
                                        matchNextRegex = false;
                                        break;
                                    }
                                    else
                                    {
                                        regexToSearchNext = "";
                                        nextToSearchVal = "";
                                        nextNotationtionToSearchVal = "";
                                        matchNextRegex = false;
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }

            var sectionListCount = 0;
            List<int> dataToRemove = new List<int>();
            foreach (var item in sectionList)
            {
                Regex regex = new Regex("^(?i)article$");
                var match = regex.Match(item); // check if match found
                if (match.Success)
                    dataToRemove.Add(sectionListCount);
                else{
                    Regex regex1 = new Regex("^(?i)section$");
                    var match1 = regex1.Match(item); // check if match found
                    if (match1.Success)
                        dataToRemove.Add(sectionListCount);
                }
                sectionListCount++;
            }
            foreach (var item in dataToRemove)
            {
                sectionList.RemoveAt(item);
            }
            var finalSectionOutput = "";
            var sectionCount = 0;
            if (Int32.Parse(SectionNoCount) == 0)
                sectionCount = sectionList.Count;
            else if (Int32.Parse(SectionNoCount) <= sectionList.Count)
                sectionCount = Int32.Parse(SectionNoCount);
            else
                sectionCount = sectionList.Count;

            var sectionCountVal = 0;
            for (int i = sectionList.Count - 1; i >= 0; i--)
            {
                sectionCountVal++;
               
                if (finalSectionOutput != "")
                    finalSectionOutput = finalSectionOutput + "," + sectionList[i];
                else
                    finalSectionOutput = sectionList[i];
                if (sectionCountVal == sectionCount)
                    break;
            }

            return finalSectionOutput;
        }

        public static void getNotationType(Dictionary<int, string> notations, string value, out string sectionGot, out string notationFoundIn)
        {

            sectionGot = "";
            notationFoundIn = "";
            foreach (var item in notations) // remove the notations
            {
                string[] search = item.Value.Split(',');
                var found = false;
                var duplicateSectionGot = value;
                if (search.Count() > 1)
                {
                    var foundBothNotation = 0;
                    foreach (var val in search)
                    {
                        if ((duplicateSectionGot).IndexOf(val) == 0)
                        {
                            duplicateSectionGot = duplicateSectionGot.Replace(val, "");
                            foundBothNotation++;
                        }
                        if ((duplicateSectionGot).LastIndexOf(val) == duplicateSectionGot.Length - 1)
                        {
                            duplicateSectionGot = duplicateSectionGot.Replace(val, "");
                            foundBothNotation++;
                        }
                        if (foundBothNotation == search.Count())
                        {
                            notationFoundIn = item.Value;
                            sectionGot = duplicateSectionGot.Trim();
                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    var foundNotation = 0;
                    foreach (var val in search)
                    {
                        if (val == "section" || val == "article")
                            duplicateSectionGot = duplicateSectionGot.ToLower();
                        if ((duplicateSectionGot).IndexOf(val) == 0)
                        {
                            duplicateSectionGot = duplicateSectionGot.Replace(val, "");
                            foundNotation++;
                        }
                        if ((duplicateSectionGot).LastIndexOf(val) == duplicateSectionGot.Length - 1)
                        {
                            duplicateSectionGot = duplicateSectionGot.Replace(val, "");
                            foundNotation++;
                        }
                        if (foundNotation == 1)
                        {
                            notationFoundIn = item.Value;
                            sectionGot = duplicateSectionGot.Trim();
                            found = true;
                            break;
                        }
                    }
                }
                if (found == true)
                    break;
            }
        }

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
                Regex regexForNextSection = new Regex(regexDictionary[i+1]);
                var matchForNextSection = regexForNextSection.Match(section); // check if match found
                if (matchForNextSection.Success)
                {
                    sectionVal = matchForNextSection.Value;
                    break;
                }
            }

            return sectionVal;
        }

        // --------------------------------------------financial------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------
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
                    if (datestring.IndexOf(match.Value) == 1) {
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

        //-------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------
        // get percent
        public static List<string> extractPercentage(string html)
        {
            List<string> formattedString = new List<string>();
            foreach (Match m in Regex.Matches(html, @"(\d{1,3})(\.(\d(?:\d+\.?)*)?)?[\s]*(\%|\s\bpercent\b)"))
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

        //-------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------
        // only year count
        public static List<string> getYearCount(string html)
        {
            html = Regex.Replace(html, @"[^0-9a-zA-Z]+", " ");
            List<string> formattedString = new List<string>();
            foreach (Match m in Regex.Matches(html, @"\d{2}[\s]*years"))
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
    }
}