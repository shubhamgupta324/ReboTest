using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReboProject
{
    class Program
    {
        // for Only section
        public static List<Dictionary<Dictionary<string, int>, int>> getSectionVal(int startPageVal,int endPageVal, Dictionary<int, Dictionary<int, string>> savePage, Dictionary<int, Dictionary<int, string>> savePageSectionRegex)
        {
            List<Dictionary<Dictionary<string, int>, int>> dataSet = new List<Dictionary<Dictionary<string, int>, int>>();
            Dictionary<Dictionary<string, int>, int> getCompleteSection = new Dictionary<Dictionary<string, int>, int>();
            Dictionary<Dictionary<string, int>, int> getCompleteSectionRegex = new Dictionary<Dictionary<string, int>, int>();
            Dictionary<string,int> getAllParaOfSection = new Dictionary<string, int>();
            Dictionary<string, int> regexSave = new Dictionary<string, int>();
            var regexDictionary = new Dictionary<int, string>();

            regexDictionary.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            regexDictionary.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            regexDictionary.Add(3, @"^[\s]*((?i)(section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            regexDictionary.Add(4, @"^[\s]*((?i)(sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            regexDictionary.Add(5, @"^[\s]*((?i)(article)\s\d)"); //   article 1,
            regexDictionary.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)"); //  article 1.1 

            regexDictionary.Add(7, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            regexDictionary.Add(8, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            regexDictionary.Add(9, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            regexDictionary.Add(10, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            regexDictionary.Add(11, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            regexDictionary.Add(12, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            regexDictionary.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            regexDictionary.Add(14, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
            regexDictionary.Add(15, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii

            regexDictionary.Add(16, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            regexDictionary.Add(17, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            regexDictionary.Add(18, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            regexDictionary.Add(19, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            regexDictionary.Add(20, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            regexDictionary.Add(21, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            regexDictionary.Add(22, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            regexDictionary.Add(23, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
            regexDictionary.Add(24, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII

            
            regexDictionary.Add(25, @"^[\s]*((?i)(section)[\s]*[a-z])");  //      section a
            regexDictionary.Add(26, @"^[\s]*((?i)(article)[\s]*[a-z])");  //      article a
            regexDictionary.Add(27, @"^[\s]*([A-Z][.])\s");  // A.
            regexDictionary.Add(28, @"^[\s]*((?i)(section)[\s]*[A-Z])\s");  // section A
            regexDictionary.Add(29, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])");  // ARTICLE A  


            List<string>  getAllSectionValue = new List<string>();
            var pageno = 0;
            var regexFound = false;
            var regexToFind = "";
            int startPage = 0;
            var nextPageRegex = 0;
            var regexCount = 0;
            foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
            {
                nextPageRegex++;
                pageno++;
                if (pageno < startPageVal)
                    continue;
                if (pageno > endPageVal)
                    break;
                if (startPage == 0)
                    startPage = pageno;
                var paraNoVal = 0;
                
                var nextRegex = 0;
                var nextPageRegexVal = savePageSectionRegex[nextPageRegex];
                foreach (var checkPage in entry.Value) // each page value
                {
                    nextRegex++;
                    var regexVal = nextPageRegexVal[nextRegex];
                    if (regexVal == null)
                        regexVal = regexCount + "-";
                    else
                        regexVal = regexCount + "-" + regexVal;
                    paraNoVal++;
                    if (regexFound == false)
                    {
                        foreach (var item in regexDictionary)
                        {
                            var matchData = Regex.Matches(checkPage.Value, item.Value); // find match
                            if (matchData.Count > 0)
                            {
                                if (getAllParaOfSection.Count() > 0)
                                {
                                    getCompleteSection.Add( getAllParaOfSection , startPage);
                                    getCompleteSectionRegex.Add(regexSave, startPage);
                                    startPage = pageno;
                                    getAllParaOfSection = new Dictionary<string, int>();
                                    regexSave = new Dictionary<string, int>();
                                    getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                                    regexSave.Add(regexVal, paraNoVal);
                                    regexToFind = item.Value;
                                    regexFound = true;
                                    break;
                                }
                                else {
                                    if (!getAllParaOfSection.ContainsKey(checkPage.Value)) {
                                        regexSave.Add(regexVal, paraNoVal);
                                        getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                                    }
                                }  
                            }
                        }
                        if(regexFound == false){
                            if (!getAllParaOfSection.ContainsKey(checkPage.Value)) {
                                regexSave.Add(regexVal, paraNoVal);
                                getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                            }
                        }
                    }
                    else
                    {
                        var matchData = Regex.Matches(checkPage.Value, regexToFind); // find match
                        if (matchData.Count > 0)
                        {
                            getCompleteSection.Add( getAllParaOfSection, startPage);
                            getCompleteSectionRegex.Add(regexSave, startPage);
                            startPage = pageno;
                            getAllParaOfSection = new Dictionary<string, int>();
                            regexSave = new Dictionary<string, int>();
                            getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                            regexSave.Add(regexVal, paraNoVal);
                        }
                        else
                        {
                            if (!getAllParaOfSection.ContainsKey(checkPage.Value)) {
                                regexSave.Add(regexVal, paraNoVal);
                                getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                            }
                        }
                    }
                    regexCount++;
                }
            }
            dataSet.Add(getCompleteSection);
            dataSet.Add(getCompleteSectionRegex);
            return dataSet;
        }
        
        // get the section page number
        public static List<string> completeSection(int sectionPageNo, Dictionary<int, Dictionary<int, string>> savePage, Dictionary<string, int> sectionRegex, Dictionary<string, int> entry, string getLineText, string myRegex)
        {
            List<string> sectionData = new List<string>();
            var completeSection = "";
            List<int> pageNos = new List<int>();
            var SectionPageNo = "";
            var startSectionParaNo = "";
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

            int index = entry.Keys.ToList().IndexOf(getLineText);
            var sectionRegexVal = sectionRegex.ElementAt(index).Key;
            var indexVal = sectionRegexVal.IndexOf('-');
            var finalRegex = sectionRegexVal.Substring(indexVal + 1, sectionRegexVal.Length - (indexVal + 1));
            var firstRegex = "";
            if (finalRegex == "")
            {
                for (int i = index - 1; i > 0; i--)
                {
                    var sectionRegexVal1 = sectionRegex.ElementAt(i).Key;
                    var indexVal1 = sectionRegexVal1.IndexOf('-');
                    var finalRegex1 = sectionRegexVal1.Substring(indexVal1 + 1, sectionRegexVal1.Length - (indexVal1 + 1));
                    if (finalRegex1 != "")
                    {
                        getLineText = entry.ElementAt(i).Key;
                        index = entry.Keys.ToList().IndexOf(getLineText);
                        firstRegex = finalRegex1;
                        break;
                    }
                }
            }
            else
                firstRegex = finalRegex;

            var checkParent = true;
            for (int i = 0; i < regexDictionary.Count(); i++)
            {
                if (firstRegex == regexDictionary[i + 1].ToString())
                {
                    checkParent = false;
                    break;
                }
            }
            var pageNoVal = 0;
            int paraNoVal = 0;
            if (checkParent == true)
            {
               
                var parentSentence = "";
                var parentRegex = "";
                var parentIndex = 0;
                var parentFound = false;
                for (int i = index - 1; i >= 0; i--)
                {
                    parentSentence = entry.ElementAt(i).Key;
                    parentRegex = sectionRegex.ElementAt(i).Key;
                    parentIndex = entry.Keys.ToList().IndexOf(parentSentence);
                    var indexVal1 = parentRegex.IndexOf('-');
                    parentRegex = parentRegex.Substring(indexVal1 + 1, parentRegex.Length - (indexVal1 + 1));
                    if (parentRegex != firstRegex)
                    {
                        for (int j = 0; j < regexDictionary.Count(); j++)
                        {
                            if (parentRegex == regexDictionary[j + 1].ToString())
                            {
                                parentFound = true;
                                break;
                            }
                        }

                        if (parentFound == true)
                            break;
                    }
                }
                getNotationType(parentSentence, sectionPageNo, savePage, out pageNoVal,out paraNoVal);
                if (!pageNos.Contains(pageNoVal)) {
                    pageNos.Add(pageNoVal);
                    startSectionParaNo = paraNoVal.ToString();
                }
                completeSection = completeSection + parentSentence;
                if (parentFound == true)
                {
                    for (int i = parentIndex + 1; i < entry.Count() - 1; i++)
                    {
                        var nextRegex = sectionRegex.ElementAt(i).Key;
                        var nextSentence = entry.ElementAt(i).Key;
                        var indexVal1 = nextRegex.IndexOf('-');
                        nextRegex = nextRegex.Substring(indexVal1 + 1, nextRegex.Length - (indexVal1 + 1));
                        if (nextRegex == parentRegex)
                        {
                            break;
                        }
                        else
                        {
                            getNotationType(nextSentence, sectionPageNo, savePage, out pageNoVal, out paraNoVal);
                            if (!pageNos.Contains(pageNoVal))
                                pageNos.Add(pageNoVal);
                            completeSection = completeSection + "|||" +nextSentence;
                        }
                    }
                }
            }
            else {
                getNotationType(getLineText, sectionPageNo, savePage, out pageNoVal, out paraNoVal);
                if (!pageNos.Contains(pageNoVal))
                {
                    pageNos.Add(pageNoVal);
                    startSectionParaNo = paraNoVal.ToString();
                }
                completeSection = completeSection + getLineText;
                for (int i = index + 1; i < entry.Count() - 1; i++)
                {
                    var nextRegex = sectionRegex.ElementAt(i).Key;
                    var nextSentence = entry.ElementAt(i).Key;
                    var indexVal1 = nextRegex.IndexOf('-');
                    nextRegex = nextRegex.Substring(indexVal1 + 1, nextRegex.Length - (indexVal1 + 1));
                    if (nextRegex == firstRegex)
                    {
                        break;
                    }
                    else
                    {
                        getNotationType(nextSentence, sectionPageNo, savePage, out pageNoVal, out paraNoVal);
                        if (!pageNos.Contains(pageNoVal))
                            pageNos.Add(pageNoVal);
                        completeSection = completeSection + "|||" + nextSentence;
                    }
                }
            }
            for (int i = 0; i < pageNos.Count(); i++)
            {
                if (SectionPageNo == "")
                    SectionPageNo = SectionPageNo + pageNos[i];
                else
                    SectionPageNo = SectionPageNo + "|" + pageNos[i];
            }
            sectionData.Add(completeSection);
            sectionData.Add(SectionPageNo); 
            sectionData.Add(startSectionParaNo); 
            return sectionData;
        }

        // get section para
        public static void getNotationType(string Sentence, int sectionPageNo, Dictionary<int, Dictionary<int, string>> savePage, out int pageNoVal, out int paraNoVal)
        {
            pageNoVal = 0;
            paraNoVal = 0;
            var pageNoFound = false;
            for (int i = sectionPageNo; i < savePage.Count(); i++)
            {
                var pageContent = savePage[i];
                for (int j = 0; j < pageContent.Count(); j++)
                {
                    var pagePara = pageContent[j+1];
                    if(pagePara == Sentence) {
                        pageNoVal = i;
                        paraNoVal = j + 1;
                        break;
                    }
                }
                if (pageNoFound == true)
                    break;
            }

        }

        // check if the para has section number or not for paragraph
        public static bool checkHasSectionNo(string sentence) {
            var hasSectionNo = false;

            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
            // NUMBERS
            matchRegexNumeric.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.])?(?!\S)"); //    1./ 1. a)
            matchRegexNumeric.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[]|)|:|.])?(?!\S)"); //    1.1  / 1.1 a)
            matchRegexNumeric.Add(3, @"^[\s]*((?i)(section)\s\d*)(?!\S)"); //    section 1
            matchRegexNumeric.Add(4, @"^[\s]*((?i)(section)\s\d+\.(?:\d+\.?)*)(?!\S)"); //    section 1.1 
            matchRegexNumeric.Add(5, @"^[\s]*((?i)(article)\s\d*)(?!\S)"); //   article 1
            matchRegexNumeric.Add(6, @"^[\s]*((?i)(article)\s\d+\.(?:\d+\.?)*)(?!\S)"); //  article 1.1 
            matchRegexNumeric.Add(7, @"^[\s]*([1-9]{1,3}[:])(?!\S)"); //   1:
            matchRegexNumeric.Add(8, @"^[\s]*([1-9]{1,3}[)])(?!\S)"); //   1)
            matchRegexNumeric.Add(9, @"^[\s]*([1-9]{1,3}[]])(?!\S)"); //   1]
            matchRegexNumeric.Add(10, @"^[\s]*([[]+[1-9]+[]])(?!\S)"); //   [1]
            matchRegexNumeric.Add(11, @"^[\s]*([[(]+[1-9]+[)])(?!\S)"); //   (1)
            
            matchRegexNumeric.Add(12, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    (xvii)
            matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    xvii)
            matchRegexNumeric.Add(14, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    [xvii]
            matchRegexNumeric.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    xvii]
            matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)"); //    xvii:
            matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)"); //    xvii.
            matchRegexNumeric.Add(18, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    section xvii
            matchRegexNumeric.Add(19, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    article xvii
            
            matchRegexNumeric.Add(20, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    (XVII)
            matchRegexNumeric.Add(21, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    XVII)
            matchRegexNumeric.Add(22, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    [XVII]
            matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    XVII]
            matchRegexNumeric.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)"); //    XVII:
            matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)"); //    XVII.
            matchRegexNumeric.Add(26, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    section XVII
            matchRegexNumeric.Add(27, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    article XVII
            
            matchRegexNumeric.Add(28, @"^[\s]*([a-z][.])(?!\S)");  //  a.
            matchRegexNumeric.Add(29, @"^[\s]*([a-z][:])(?!\S)");  //  a:
            matchRegexNumeric.Add(30, @"^[\s]*([(][a-z][)])(?!\S)");  //    (a)
            matchRegexNumeric.Add(31, @"^[\s]*([a-z][)])(?!\S)");  // a)
            matchRegexNumeric.Add(32, @"^[\s]*([[][a-z][]])(?!\S)");  //   a]
            matchRegexNumeric.Add(33, @"^[\s]*([a-z][]])(?!\S)");  //     [a]
            matchRegexNumeric.Add(34, @"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)");  //      section a
            matchRegexNumeric.Add(35, @"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)");  //      article a
            
            matchRegexNumeric.Add(36, @"^[\s]*([A-Z][.])(?!\S)");  // A.
            matchRegexNumeric.Add(37, @"^[\s]*([A-Z][:])(?!\S)");  // A:
            matchRegexNumeric.Add(38, @"^[\s]*([(][A-Z][)])(?!\S)");  // (A)
            matchRegexNumeric.Add(39, @"^[\s]*([A-Z][)])(?!\S)");  // A)
            matchRegexNumeric.Add(40, @"^[\s]*([[][A-Z][]])(?!\S)");  // A]
            matchRegexNumeric.Add(41, @"^[\s]*([A-Z][]])(?!\S)");  // [A]
            matchRegexNumeric.Add(42, @"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)");  // section A
            matchRegexNumeric.Add(43, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)");  // ARTICLE A

            foreach (var item in matchRegexNumeric)
            {
                Regex regex = new Regex(item.Value);
                var match = regex.Match(sentence); // check if match found
                if (match.Success)
                {
                    hasSectionNo = true;
                    break;
                }
            }
            return hasSectionNo;
        }
    }
}
