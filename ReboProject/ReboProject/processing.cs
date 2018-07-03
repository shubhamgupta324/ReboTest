using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ReboProject
{
    public class processing
    {
        //public static string SectionVal(Dictionary<int, Dictionary<int, string>> savePage, int pageNo, int paraNumber)
        //{
        //    //---------------------------REGEX--------------------------------------------------------
        //    Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
        //    matchRegexNumeric.Add(1, @"^(\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
        //    matchRegexNumeric.Add(2, @"^(\d+(.\d+)+(\:\d+))"); //   (1.1:1)
        //    matchRegexNumeric.Add(3, @"^(\d+(\:\d+))"); //  (1:1)
        //    matchRegexNumeric.Add(4, @"^((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
        //    matchRegexNumeric.Add(5, @"^((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
        //    matchRegexNumeric.Add(6, @"^((section)\s\d+(\:\d+))"); //   (section 1:1)
        //    matchRegexNumeric.Add(7, @"^(\(+\d+\))\s");  // (1)

        //    matchRegexNumeric.Add(8, @"^[ \t](\d+\.(?:\d+\.?)*)"); //    ( 1.), ( 1.1) and ( 1.1.1)
        //    matchRegexNumeric.Add(9, @"^[ \t](\d+(.\d+)+(\:\d+))"); //   ( 1.1:1)
        //    matchRegexNumeric.Add(10, @"^[ \t](\d+(\:\d+))"); //  ( 1:1)
        //    matchRegexNumeric.Add(11, @"^[ \t]((section)\s\d+\.(?:\d+\.?)*)"); //   ( section 1.), ( section 1.1) and ( section 1.1.1)
        //    matchRegexNumeric.Add(12, @"^[ \t]((section)\s\d+(.\d+)+(\:\d+))"); //    ( section 1.1:1)
        //    matchRegexNumeric.Add(13, @"^[ \t]((section)\s\d+(\:\d+))"); //   ( section 1:1)
        //    matchRegexNumeric.Add(14, @"^[ \t](\(+\d+\))\s");  // ( 1)

        //    Dictionary<int, string> matchRegexAlphabet = new Dictionary<int, string>();
        //    matchRegexAlphabet.Add(1, @"^[ \t]([a-zA-Z]+\.)\s");  // a.  
        //    matchRegexAlphabet.Add(2, @"^[ \t](\(+[a-zA-Z]+\))\s");  // (a)
        //    matchRegexAlphabet.Add(3, @"^([a-zA-Z]+\.)\s");  // a.  
        //    matchRegexAlphabet.Add(4, @"^(\(+[a-zA-Z]+\))\s");  // (a)

        //    //-------------------------------------------------------------------------------------------
        //    var count = pageNo - 3;
        //    var endPageCheck = 0;
        //    if (count > 0)
        //        endPageCheck = pageNo - 2;

        //    for (int i = pageNo; i > endPageCheck; i--) // loop through current and last 2 pages
        //    {
        //        var entry1 = savePage[i].Values; // get the content of current page
        //        var paraCount = paraNumber-1; // paraNumber // get the para number of result in current page
        //        if (i != pageNo) // if previous page 
        //            paraCount = entry1.Count()-1; // take all para in a page to check
        //        for (var j= paraCount;j >= 0; j--) { // loop th

        //            var entry2 = entry1.ElementAt(j);
        //            var textLower = @entry2.ToLower().ToString();
        //            foreach (var check in matchRegexNumeric)
        //            {
        //                String AllowedChars = check.Value;
        //                Regex regex = new Regex(AllowedChars);
        //                var match = regex.Match(textLower);
        //                if (match.Success)
        //                {
        //                    var sectionVal = (match.Value).Replace("section", "");
        //                    return sectionVal;
        //                }             
        //            }
        //            var text = @entry2.ToLower().ToString();
        //            foreach (var  check in matchRegexAlphabet) {
        //                String AllowedChars = check.Value;
        //                Regex regex = new Regex(AllowedChars);
        //                var match = regex.Match(text);
        //                if (match.Success)
        //                {
        //                    var sectionVal = (match.Value).Replace("section", "");
        //                    return sectionVal;
                            
        //                }
        //            }
        //        }
        //    }
        //    return "false";
        //}
        public static string SectionValParagraph(Dictionary<int, Dictionary<int, string>> savePage, int pageNo, int paraNumber)
        {

            Dictionary<string, string> getTopVal = new Dictionary<string, string>();
            getTopVal.Add("lowCaseAlpha","a");
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



            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();

            // NUMBERS

            matchRegexNumeric.Add(1, @"^[\s]*(\d+\.(:\d+\.?)*)\s"); //    1.
            matchRegexNumeric.Add(2, @"^[\s]*(\d+\.(?:\d+\.?)*)"); //    1., 1.1 
            matchRegexNumeric.Add(3, @"^[\s]*((section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            matchRegexNumeric.Add(4, @"^[\s]*((section)\s\d+\.(?:\d+\.?)*)"); //   section 1., section 1.1 
            matchRegexNumeric.Add(5, @"^[\s]*((article)\s\d)"); //   article 1,
            matchRegexNumeric.Add(6, @"^[\s]*((article)\s\d+\.(?:\d+\.?)*)"); //   article 1., article 1.1 
            matchRegexNumeric.Add(7, @"^[\s]*(\d+[:])\s"); //   1:
            matchRegexNumeric.Add(8, @"^[\s]*(\d+[)])\s"); //   1)
            matchRegexNumeric.Add(9, @"^[\s]*(\d+[]])\s"); //   1]
            matchRegexNumeric.Add(10, @"^[\s]*([[]+\d+[]])\s"); //   [1]
            matchRegexNumeric.Add(11, @"^[\s]*([[(]+\d+[)])\s"); //   (1)

            matchRegexNumeric.Add(54, @"^[\s]*((Section)\s\d+\.(:\d+\.?)*)\s"); //    Section 1.
            matchRegexNumeric.Add(55, @"^[\s]*((Article)\s\d)"); //   Article 1,
            matchRegexNumeric.Add(56, @"^[\s]*((Section)\s\d+\.(?:\d+\.?)*)"); //   Section 1., Section 1.1 
            matchRegexNumeric.Add(57, @"^[\s]*((Article)\s\d)"); //   Article 1., Article 1.1 
            matchRegexNumeric.Add(58, @"^[\s]*((SECTION)\s\d+\.(?:\d+\.?)*)"); //   Section 1., Section 1.1 
            matchRegexNumeric.Add(59, @"^[\s]*((ARTICLE)\s\d)"); //   Article 1., Article 1.1 

            // ROMAN
            // upper
            matchRegexNumeric.Add(12, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            matchRegexNumeric.Add(14, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            matchRegexNumeric.Add(15, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            matchRegexNumeric.Add(18, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            matchRegexNumeric.Add(19, @"^[\s]*(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    section xvii
            matchRegexNumeric.Add(20, @"^[\s]*(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii

            matchRegexNumeric.Add(52, @"^[\s]*(Section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    Section xvii
            matchRegexNumeric.Add(53, @"^[\s]*(Article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    Article xvii
            matchRegexNumeric.Add(60, @"^[\s]*(SECTION)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    SECTION xvii
            matchRegexNumeric.Add(61, @"^[\s]*(ARTICLE)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    ARTICLE xvii

            // lower
            matchRegexNumeric.Add(21, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            matchRegexNumeric.Add(22, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            matchRegexNumeric.Add(24, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            matchRegexNumeric.Add(26, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            matchRegexNumeric.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            matchRegexNumeric.Add(28, @"^[\s]*(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    section XVII
            matchRegexNumeric.Add(29, @"^[\s]*(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII

            matchRegexNumeric.Add(50, @"^[\s]*(Section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    Section XVII
            matchRegexNumeric.Add(51, @"^[\s]*(Article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    Article XVII
            matchRegexNumeric.Add(62, @"^[\s]*(SECTION)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    SECTION XVII
            matchRegexNumeric.Add(63, @"^[\s]*(ARTICLE)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    ARTICLE XVII

            // ALPHABET

            matchRegexNumeric.Add(30, @"^[\s]*([a-z][.])\s");  //  a.
            matchRegexNumeric.Add(31, @"^[\s]*([a-z][:])\s");  //  a: 
            matchRegexNumeric.Add(32, @"^[\s]*([(][a-z][)])\s");  //    (a) 
            matchRegexNumeric.Add(33, @"^[\s]*([a-z][)])\s");  // a) 
            matchRegexNumeric.Add(34, @"^[\s]*([[][a-z][]])\s");  //   a] 
            matchRegexNumeric.Add(35, @"^[\s]*([a-z][]])\s");  //     [a]
            matchRegexNumeric.Add(36, @"^[\s]*((section)[\s]*[a-z])\s");  //      section a 
            matchRegexNumeric.Add(37, @"^[\s]*((article)[\s]*[a-z])");  //      article a

            matchRegexNumeric.Add(46, @"^[\s]*((Section)[\s]*[a-z])\s");  //    Section a
            matchRegexNumeric.Add(47, @"^[\s]*((Article)[\s]*[a-z])");  //    Article a
            matchRegexNumeric.Add(64, @"^[\s]*((SECTION)[\s]*[a-z])\s");  //    SECTION a
            matchRegexNumeric.Add(65, @"^[\s]*((ARTICLE)[\s]*[a-z])");  //    ARTICLE a


            matchRegexNumeric.Add(38, @"^[\s]*([A-Z][.])\s");  // A. 
            matchRegexNumeric.Add(39, @"^[\s]*([A-Z][:])\s");  // A: 
            matchRegexNumeric.Add(40, @"^[\s]*([(][A-Z][)])\s");  // (A) 
            matchRegexNumeric.Add(41, @"^[\s]*([A-Z][)])\s");  // A) 
            matchRegexNumeric.Add(42, @"^[\s]*([[][A-Z][]])\s");  // A]  
            matchRegexNumeric.Add(43, @"^[\s]*([A-Z][]])\s");  // [A]    
            matchRegexNumeric.Add(44, @"^[\s]*((section)[\s]*[A-Z])\s");  // section A    
            matchRegexNumeric.Add(45, @"^[\s]*((article)[\s]*[A-Z])");  // article A    

            matchRegexNumeric.Add(48, @"^[\s]*((Section)[\s]*[A-Z])\s");  // section A   
            matchRegexNumeric.Add(49, @"^[\s]*((Article)[\s]*[A-Z])");  // Article A  
            matchRegexNumeric.Add(66, @"^[\s]*((SECTION)[\s]*[A-Z])\s");  // SECTION A   
            matchRegexNumeric.Add(67, @"^[\s]*((ARTICLE)[\s]*[A-Z])");  // ARTICLE A   

            //next 68

            Dictionary<int, string> connections = new Dictionary<int, string>();
            connections.Add(1,"numberVal"); //    1.
            connections.Add(2, ""); //    1., 1.1 
            connections.Add(3, "numberVal"); //    section 1.
            connections.Add(4, ""); //   section 1., section 1.1 
            connections.Add(5, "numberVal"); //   article 1.,
            connections.Add(6, ""); //   article 1., article 1.1 
            connections.Add(7, "numberVal"); //   1:
            connections.Add(8, "numberVal"); //   1)
            connections.Add(9, "numberVal"); //   1]
            connections.Add(10,"numberVal"); //   [1]
            connections.Add(11,"numberVal"); //   (1)
            connections.Add(54, "numberVal"); //   [1]
            connections.Add(55, "numberVal"); //   (1)
            connections.Add(56, ""); //   Section 1., Section 1.1 
            connections.Add(57, ""); //   Article 1., Article 1.1 
            connections.Add(58, ""); //   SECTION 1., SECTION 1.1 
            connections.Add(59, ""); //   ARTICLE 1., ARTICLE 1.1 

            connections.Add(12, "lowCaseNumeric"); //    (xvii)
            connections.Add(13, "lowCaseNumeric"); //    xvii
            connections.Add(14, "lowCaseNumeric"); //    xvii)
            connections.Add(15, "lowCaseNumeric"); //    [xvii]
            connections.Add(16, "lowCaseNumeric"); //    xvii]
            connections.Add(17, "lowCaseNumeric"); //    xvii:
            connections.Add(18, "lowCaseNumeric"); //    xvii.
            connections.Add(19, "lowCaseNumeric"); //    section xvii
            connections.Add(20, "lowCaseNumeric"); //    article xvii
            connections.Add(52, "lowCaseNumeric"); //    section xvii
            connections.Add(53, "lowCaseNumeric"); //    article xvii
            connections.Add(60, "lowCaseNumeric"); //    SECTION xvii
            connections.Add(61, "lowCaseNumeric"); //    ARTICLE xvii

            connections.Add(21, "upCaseNumeric"); //    (XVII)
            connections.Add(22, "upCaseNumeric"); //    XVII
            connections.Add(23, "upCaseNumeric"); //    XVII)
            connections.Add(24, "upCaseNumeric"); //    [XVII]
            connections.Add(25, "upCaseNumeric"); //    XVII]
            connections.Add(26, "upCaseNumeric"); //    XVII:
            connections.Add(27, "upCaseNumeric"); //    XVII.
            connections.Add(28, "upCaseNumeric"); //    section XVII
            connections.Add(29, "upCaseNumeric"); //    article XVII
            connections.Add(50, "upCaseNumeric"); //    section XVII
            connections.Add(51, "upCaseNumeric"); //    article XVII
            connections.Add(62, "upCaseNumeric"); //    SECTION XVII
            connections.Add(63, "upCaseNumeric"); //    ARTICLE XVII

            connections.Add(30, "lowCaseAlpha");  //   A.
            connections.Add(31, "lowCaseAlpha");  //  A:
            connections.Add(32, "lowCaseAlpha");  //   (A)
            connections.Add(33, "lowCaseAlpha");  //  A)
            connections.Add(34, "lowCaseAlpha");  //   A]
            connections.Add(35, "lowCaseAlpha");  //      [A]
            connections.Add(36, "lowCaseAlpha");  //    section A
            connections.Add(37, "lowCaseAlpha");  //     article A
            connections.Add(48, "lowCaseAlpha");  //     Section A
            connections.Add(49, "lowCaseAlpha");  //     Article A 
            connections.Add(66, "lowCaseAlpha");  //     SECTION A
            connections.Add(67, "lowCaseAlpha");  //     ARTICLE A 


            connections.Add(38, "upCaseAlpha");  // a. 
            connections.Add(39, "upCaseAlpha");  // a: 
            connections.Add(40, "upCaseAlpha");  // (a)  
            connections.Add(41, "upCaseAlpha");  // a)
            connections.Add(42, "upCaseAlpha");  // a]  
            connections.Add(43, "upCaseAlpha");  // [a]     
            connections.Add(44, "upCaseAlpha");  // section a    
            connections.Add(45, "upCaseAlpha");  // article a   
            connections.Add(46, "upCaseAlpha");  // section a    
            connections.Add(47, "upCaseAlpha");  // article a    
            connections.Add(64, "upCaseAlpha");  // SECTION a    
            connections.Add(65, "upCaseAlpha");  // ARTICLE a  



            List<string> allSectionVal = new List<string>();

            var searchNow = "";
            var regexNow = "";
            var notationFoundIn = "";
            var pageNoVal = pageNo;
            var paraNoVal = paraNumber;
            var completeSectionFound = false;
            bool checkNextGenSection = true;
            var lastSection = "";
            var checkNextSection = true;
            for (var j = 0; j < 5; j++) { // loop till 5th gen
                if (completeSectionFound == false && checkNextGenSection == true) { // check if final gen found
                    paraNoVal = paraNoVal - 1;
                    if (searchNow !="" && regexNow !="" && lastSection != searchNow)
                    {
                        var NextsectionVal = 0;
                        int nextPageNo = 0;
                        int nextParaNo = 0;
                        string nextRegex = "";
                        string nextNotation = "";
                        string nextSectionFound = "";
                        saveDataToFolder(getTopVal,connections, searchNow, regexNow, notationFoundIn, pageNoVal, paraNoVal, matchRegexNumeric, savePage, notations, out NextsectionVal, out checkNextGenSection, out nextPageNo, out nextParaNo, out nextRegex, out nextNotation,out nextSectionFound, out lastSection, out checkNextSection);
                        if (checkNextGenSection == true) {
                            var getConnectionName = connections[NextsectionVal];
                            if (getConnectionName != "") {
                                var getFirstVal = getTopVal[getConnectionName];
                                searchNow = getFirstVal;
                            }
                            else
                                searchNow = "";
                            regexNow = nextRegex;
                            notationFoundIn = nextNotation;
                            pageNoVal = nextPageNo;
                            paraNoVal = nextParaNo;
                            allSectionVal.Add(nextSectionFound);
                        }
                    }
                    else if(checkNextSection == true)
                    {
                        var checkNextPage = true;
                        for (int i = pageNoVal; i > 0; i--) // loop through all the pages
                        {
                            if (checkNextPage == false) // read next page or not
                                break;
                            var entry1 = savePage[i].Values;// get the content of current page
                            var paraCount = paraNoVal; // paraNumber // get the para number of result in current page

                            if (i != pageNoVal) // if previous page 
                                paraCount = entry1.Count() - 1; // take all para in a page to check

                            var checkNextPara = true;
                            for (var k = paraCount; k >= 0; k--) // start from the para till top
                            {
                                if (checkNextPara == false) // read next para or not
                                    break;
                                var entry2 = entry1.ElementAt(k); // get the para\

                                var matchNext = true;
                                foreach (var check in matchRegexNumeric) // loop through all the regex
                                {
                                    if (matchNext == false) // need to check next regex
                                        break;
                                    String AllowedChars = check.Value;
                                    Regex regex = new Regex(AllowedChars);
                                    var match = regex.Match(entry2); // check if match found

                                    if (match.Success) // if found
                                    {
                                        var keyToFindConnection = check.Key;
                                        //var sectionGot = match.Value;
                                        var sectionGot = "";
                                        if (connections[check.Key] != "")
                                            getNotationType(notations, match.Value.Trim(), out sectionGot, out notationFoundIn);
                                        

                                        var getValueFromConnection = connections[keyToFindConnection];
                                        if (sectionGot != "")
                                        {
                                            if (sectionGot != getTopVal[getValueFromConnection])
                                                searchNow = getTopVal[getValueFromConnection];
                                            allSectionVal.Add(match.Value.Trim());
                                            paraNoVal = k;
                                            if(paraNoVal ==0)
                                                pageNoVal = i-1;
                                            else
                                                pageNoVal = i;
                                            lastSection = sectionGot;
                                            regexNow = AllowedChars;
                                            matchNext = false;
                                            checkNextPage = false;
                                            checkNextPara = false;
                                            break;
                                        }
                                        else if (getValueFromConnection == "")
                                        {
                                            checkNextSection = false;
                                            searchNow = "";
                                            allSectionVal.Add(match.Value.Trim());
                                            paraNoVal = k;
                                            if (paraNoVal == 0)
                                                pageNoVal = i - 1;
                                            else
                                                pageNoVal = i;
                                            lastSection = sectionGot;
                                            regexNow = AllowedChars;
                                            matchNext = false;
                                            checkNextPage = false;
                                            checkNextPara = false;
                                            break;
                                        }
                                        else
                                            completeSectionFound = true;
                                    }
                                }
                            }
                        }
                        if (allSectionVal.Count() == j)
                            break;
                    } 
                }
            }
            var finalSectionOutput = "";
            for (int i = allSectionVal.Count; i >0; i--)
            {
                finalSectionOutput = finalSectionOutput +" "+allSectionVal[i-1];
            }
            return finalSectionOutput;
        }

        public static void saveDataToFolder(Dictionary<string,string> getTopVal, Dictionary<int,string> connections, string searchNow,string regexNow,string notationFoundIn,int pageNoVal,int paraNoVal, Dictionary<int,string> matchRegexNumeric, Dictionary<int, Dictionary<int, string>> savePage, Dictionary<int, string> notations, out int NextsectionVal ,out bool foundVal, out int nextPageNo, out int nextParaNo, out string nextRegex, out string nextNotation ,out string nextSectionFound, out string lastSection, out bool checkNextSection)
        {
            var currentPage = pageNoVal;
            var executeNext1 = true;
            // out parameters
            NextsectionVal = 0;
            nextSectionFound = "";
            foundVal = false;
            nextPageNo = 0;
            nextParaNo = 0;
            nextRegex = "";
            nextNotation = "";
            lastSection = "";
            checkNextSection = true;
            for (int i = currentPage; i > 0; i--) // loop through all the pages
            {
                if (executeNext1 == false)
                    break;
                var entry1 = savePage[i].Values;// get the content of current page
                var paraCount = paraNoVal; // paraNumber // get the para number of result in current page

                if (i != pageNoVal) // if previous page 
                    paraCount = entry1.Count() - 1; // take all para in a page to check

                var executeNext2 = true;
                for (var k = paraCount; k >= 0; k--) // start from the para till top
                {
                    if (executeNext2 == false) // read next para or not
                        break;
                    var entry2 = entry1.ElementAt(k); // get the para
                    //var textLower = @entry2.ToLower().ToString(); // lower the case
                    Regex regex = new Regex(regexNow);
                    var match = regex.Match(@entry2); // check if match found
                    //var sectionGot = match.Value;
                    var sectionGot = "";
                    if (match.Success)
                        getNotationType(notations, match.Value.Trim(), out sectionGot, out notationFoundIn);
                    if (match.Success && sectionGot == searchNow) // if found
                    {
                        var paraCountnextGen = k - 1;
                        var executeNext3 = true;
                        for (int l = i; l > 0; l--) // loop through all the pages
                        {
                            if (executeNext3 == false) // read next para or not
                                break;
                            var pageData = savePage[l].Values;// get the content of current page
                            var paraNoCount = paraNoVal + 1; // paraNumber // get the para number of result in current page

                            if (l != i) // if previous page 
                                paraCountnextGen = pageData.Count() - 1; // take all para in a page to check
                            if (paraCount < 0)
                                continue;
                            var executeNext4 = true;
                            for (var p = paraCountnextGen; p >= 0; p--) // start from the para till top
                            {
                                if (executeNext4 == false) // read next para or not
                                    break;
                                paraNoVal = p + 1;
                                var pageData2 = pageData.ElementAt(p); // get the para
                                                                       //var textLowerData = @pageData2.ToLower().ToString(); // lower the case
                                var matchNext = true;

                                foreach (var check in matchRegexNumeric) // loop through all the regex
                                {
                                    if (matchNext == true) // need to check next regex
                                    {
                                        if (check.Value == regexNow)
                                            continue;
                                        String AllowedChars = check.Value;
                                        Regex regexVal = new Regex(AllowedChars);
                                        var matchVal = regexVal.Match(pageData2); // check if match found
                                        if (matchVal.Success)
                                        {
                                            sectionGot = "";
                                            notationFoundIn = "";
                                            if (connections[check.Key] != "")
                                                getNotationType(notations, matchVal.Value.Trim(), out sectionGot, out notationFoundIn);

                                            if (sectionGot != "") {
                                                NextsectionVal = check.Key;
                                                lastSection = sectionGot;
                                                nextSectionFound = matchVal.Value;
                                                foundVal = true;
                                                nextPageNo = l;
                                                nextParaNo = p;
                                                nextRegex = AllowedChars;
                                                nextNotation = notationFoundIn;
                                                executeNext1 = false;
                                                executeNext2 = false;
                                                executeNext3 = false;
                                                executeNext4 = false;
                                                break;
                                            }
                                            else if (connections[check.Key] =="") {
                                                NextsectionVal = check.Key;
                                                lastSection = sectionGot;
                                                nextSectionFound = matchVal.Value;
                                                foundVal = true;
                                                nextPageNo = l;
                                                nextParaNo = p;
                                                nextRegex = AllowedChars;
                                                nextNotation = notationFoundIn;
                                                checkNextSection = false;
                                                executeNext1 = false;
                                                executeNext2 = false;
                                                executeNext3 = false;
                                                executeNext4 = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void getNotationType(Dictionary<int, string> notations,string value, out string sectionGot, out string notationFoundIn) {

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
                        if (foundBothNotation == search.Count()) {
                            notationFoundIn = item.Value;
                            sectionGot = duplicateSectionGot.Trim();
                            found = true;
                            break;
                        }
                    }
                }
                else {
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
                        if ((duplicateSectionGot).LastIndexOf(val) == duplicateSectionGot.Length-1)
                        {
                            duplicateSectionGot = duplicateSectionGot.Replace(val, "");
                            foundNotation ++;
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
            }
        }

        public static string SectionValSection(string pageContent, string output) {


            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();

            // NUMBERS

            matchRegexNumeric.Add(1, @"^[\s]*(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            matchRegexNumeric.Add(2, @"^[\s]*(\d{1,3}\.(?:\d+\.?)*)"); //    1., 1.1 
            matchRegexNumeric.Add(3, @"^[\s]*((section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            matchRegexNumeric.Add(4, @"^[\s]*((section)\s\d+\.(?:\d+\.?)*)"); //   section 1., section 1.1 
            matchRegexNumeric.Add(5, @"^[\s]*((article)\s\d)"); //   article 1,
            matchRegexNumeric.Add(6, @"^[\s]*((article)\s\d+\.(?:\d+\.?)*)"); //   article 1., article 1.1 
            matchRegexNumeric.Add(7, @"^[\s]*(\d{1,3}[:])\s"); //   1:
            matchRegexNumeric.Add(8, @"^[\s]*(\d{1,3}[)])\s"); //   1)
            matchRegexNumeric.Add(9, @"^[\s]*(\d{1,3}[]])\s"); //   1]
            matchRegexNumeric.Add(10, @"^[\s]*([[]+\d+[]])\s"); //   [1]
            matchRegexNumeric.Add(11, @"^[\s]*([[(]+\d+[)])\s"); //   (1)

            matchRegexNumeric.Add(54, @"^[\s]*((Section)\s\d+\.(:\d+\.?)*)\s"); //    Section 1.
            matchRegexNumeric.Add(55, @"^[\s]*((Article)\s\d)"); //   Article 1,
            matchRegexNumeric.Add(56, @"^[\s]*((Section)\s\d+\.(?:\d+\.?)*)"); //   Section 1., Section 1.1 
            matchRegexNumeric.Add(57, @"^[\s]*((Article)\s\d)"); //   Article 1., Article 1.1 
            matchRegexNumeric.Add(58, @"^[\s]*((SECTION)\s\d+\.(?:\d+\.?)*)"); //   Section 1., Section 1.1 
            matchRegexNumeric.Add(59, @"^[\s]*((ARTICLE)\s\d)"); //   Article 1., Article 1.1 

            // ROMAN
            // upper
            matchRegexNumeric.Add(12, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            matchRegexNumeric.Add(14, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            matchRegexNumeric.Add(15, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            matchRegexNumeric.Add(18, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            matchRegexNumeric.Add(19, @"^[\s]*(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    section xvii
            matchRegexNumeric.Add(20, @"^[\s]*(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii

            matchRegexNumeric.Add(52, @"^[\s]*(Section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    Section xvii
            matchRegexNumeric.Add(53, @"^[\s]*(Article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    Article xvii
            matchRegexNumeric.Add(60, @"^[\s]*(SECTION)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    SECTION xvii
            matchRegexNumeric.Add(61, @"^[\s]*(ARTICLE)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    ARTICLE xvii

            // lower
            matchRegexNumeric.Add(21, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            matchRegexNumeric.Add(22, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            matchRegexNumeric.Add(24, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            matchRegexNumeric.Add(26, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            matchRegexNumeric.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            matchRegexNumeric.Add(28, @"^[\s]*(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    section XVII
            matchRegexNumeric.Add(29, @"^[\s]*(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII

            matchRegexNumeric.Add(50, @"^[\s]*(Section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    Section XVII
            matchRegexNumeric.Add(51, @"^[\s]*(Article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    Article XVII
            matchRegexNumeric.Add(62, @"^[\s]*(SECTION)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    SECTION XVII
            matchRegexNumeric.Add(63, @"^[\s]*(ARTICLE)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    ARTICLE XVII

            // ALPHABET

            matchRegexNumeric.Add(30, @"^[\s]*([a-z][.])\s");  //  a.
            matchRegexNumeric.Add(31, @"^[\s]*([a-z][:])\s");  //  a: 
            matchRegexNumeric.Add(32, @"^[\s]*([(][a-z][)])\s");  //    (a) 
            matchRegexNumeric.Add(33, @"^[\s]*([a-z][)])\s");  // a) 
            matchRegexNumeric.Add(34, @"^[\s]*([[][a-z][]])\s");  //   a] 
            matchRegexNumeric.Add(35, @"^[\s]*([a-z][]])\s");  //     [a]
            matchRegexNumeric.Add(36, @"^[\s]*((section)[\s]*[a-z])\s");  //      section a 
            matchRegexNumeric.Add(37, @"^[\s]*((article)[\s]*[a-z])");  //      article a

            matchRegexNumeric.Add(46, @"^[\s]*((Section)[\s]*[a-z])\s");  //    Section a
            matchRegexNumeric.Add(47, @"^[\s]*((Article)[\s]*[a-z])");  //    Article a
            matchRegexNumeric.Add(64, @"^[\s]*((SECTION)[\s]*[a-z])\s");  //    SECTION a
            matchRegexNumeric.Add(65, @"^[\s]*((ARTICLE)[\s]*[a-z])");  //    ARTICLE a


            matchRegexNumeric.Add(38, @"^[\s]*([A-Z][.])\s");  // A. 
            matchRegexNumeric.Add(39, @"^[\s]*([A-Z][:])\s");  // A: 
            matchRegexNumeric.Add(40, @"^[\s]*([(][A-Z][)])\s");  // (A) 
            matchRegexNumeric.Add(41, @"^[\s]*([A-Z][)])\s");  // A) 
            matchRegexNumeric.Add(42, @"^[\s]*([[][A-Z][]])\s");  // A]  
            matchRegexNumeric.Add(43, @"^[\s]*([A-Z][]])\s");  // [A]    
            matchRegexNumeric.Add(44, @"^[\s]*((section)[\s]*[A-Z])\s");  // section A    
            matchRegexNumeric.Add(45, @"^[\s]*((article)[\s]*[A-Z])");  // article A    

            matchRegexNumeric.Add(48, @"^[\s]*((Section)[\s]*[A-Z])\s");  // section A   
            matchRegexNumeric.Add(49, @"^[\s]*((Article)[\s]*[A-Z])");  // Article A  
            matchRegexNumeric.Add(66, @"^[\s]*((SECTION)[\s]*[A-Z])\s");  // SECTION A   
            matchRegexNumeric.Add(67, @"^[\s]*((ARTICLE)[\s]*[A-Z])");  // ARTICLE A   

            
            string[] pageContentData = pageContent.Split(new[] { "|||" }, StringSplitOptions.None);
            pageContentData = pageContentData.Take(pageContentData.Count() - 1).ToArray();
            string[] outputData = output.Split(new[] { "|||. " }, StringSplitOptions.None);
            outputData = outputData.Take(outputData.Count() - 1).ToArray();
            var finalSection = "";

            var getHeadSection = "";
            foreach (var item in matchRegexNumeric)
            {
                Regex regex = new Regex(item.Value);
                var matchHeadSection = regex.Match(pageContentData[0]); // check if match found
                if (matchHeadSection.Success)
                {
                    getHeadSection = matchHeadSection.Value.Trim();
                    break;
                }
            }
            List<string> sectionVal = new List<string>();
            for (int i = 0; i < outputData.Length; i++)
            {
                var sectionSinglePara = "";
                var start =Array.IndexOf<string>(pageContentData, outputData[i]);
                List<string> startRegex = new List<string>();
                foreach (var item in matchRegexNumeric)
                {
                    Regex regex = new Regex(item.Value);
                    var match = regex.Match(outputData[i]); // check if match found
                    if (match.Success) {
                        sectionSinglePara = sectionSinglePara + match.Value.Trim();
                        startRegex.Add(item.Value);
                        break;
                    }
                }

                var lastAdded = "";
                for (int j = start; j >=0; j--)
                {
                    var checkThisRegex = true;
                    foreach (var item in matchRegexNumeric)
                    {
                        foreach (var val in startRegex)
                        {
                            if (val == item.Value)
                            {
                                checkThisRegex = false;
                                break;
                            }
                        }
                        if (checkThisRegex == false)
                            continue;
                        Regex regex = new Regex(item.Value);
                        var match = regex.Match(pageContentData[j]); // check if match found

                        if (match.Success && !sectionSinglePara.Contains(match.Value.Trim())) {
                            lastAdded = match.Value.Trim();
                            if (sectionSinglePara == "")
                                startRegex.Add(item.Value);
                            sectionSinglePara = match.Value.Trim() + " " + sectionSinglePara;
                        }
                    }
                }
                sectionSinglePara = sectionSinglePara.Replace(lastAdded,"");
                sectionVal.Add(sectionSinglePara);
               // finalSection = finalSection + " " + sectionSinglePara;
            }
            for (var i=0;i< sectionVal.Count();i++) {
                finalSection = finalSection + " " + sectionVal[i];
            }
            finalSection = getHeadSection+" ("+finalSection+")";
            
            return finalSection;
        }

    }
}