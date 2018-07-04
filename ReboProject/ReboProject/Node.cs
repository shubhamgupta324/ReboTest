using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReboProject
{
    class Program
    {
        public static Dictionary<Dictionary<string, int>, int> SectionVal123(int startPageVal,int endPageVal, Dictionary<int, Dictionary<int, string>> savePage)
        {
            Dictionary<Dictionary<string, int>, int> getCompleteSection = new Dictionary<Dictionary<string, int>, int>();
            Dictionary<string,int> getAllParaOfSection = new Dictionary<string, int>();
            var regexDictionary = new Dictionary<int, string>();

            regexDictionary.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            regexDictionary.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            regexDictionary.Add(3, @"^[\s]*((section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            regexDictionary.Add(4, @"^[\s]*((sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
            regexDictionary.Add(5, @"^[\s]*(\d{1,3}[:])\s"); //   1:
            regexDictionary.Add(6, @"^[\s]*(\d{1,3}[)])\s"); //   1)
            regexDictionary.Add(7, @"^[\s]*(\d{1,3}[]])\s"); //   1]
            regexDictionary.Add(8,@"^[\s]*([[]+\d+[]])\s"); //   [1]
            regexDictionary.Add(9,@"^[\s]*([[(]+\d+[)])\s"); //   (1)
            regexDictionary.Add(10, @"^[\s]*((Section)\s\d+\.(:\d+\.?)*)\s"); //    Section 1.
            regexDictionary.Add(11, @"^[\s]*((Section)\s\d+\.\d(?:\d+\.?)*)"); //    Section 1.1 
            regexDictionary.Add(12, @"^[\s]*((SECTION)\s\d+\.\d(?:\d+\.?)*)"); //    SECTION 1.1 

            // ROMAN
            // upper
            regexDictionary.Add(13, @"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    (xvii)
            regexDictionary.Add(14, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s"); //    xvii
            regexDictionary.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s"); //    xvii)
            regexDictionary.Add(16, @"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    [xvii]
            regexDictionary.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s"); //    xvii]
            regexDictionary.Add(18, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s"); //    xvii:
            regexDictionary.Add(19, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s"); //    xvii.
            regexDictionary.Add(20, @"^[\s]*(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    section xvii
            regexDictionary.Add(21, @"^[\s]*(Section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    Section xvii
            regexDictionary.Add(22, @"^[\s]*(SECTION)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    SECTION xvii

            // lower
            regexDictionary.Add(23, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            regexDictionary.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            regexDictionary.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            regexDictionary.Add(26, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            regexDictionary.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            regexDictionary.Add(28, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            regexDictionary.Add(29, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            regexDictionary.Add(30, @"^[\s]*(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
            regexDictionary.Add(31, @"^[\s]*(Section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    Section XVII
            regexDictionary.Add(32, @"^[\s]*(SECTION)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    SECTION XVII

            // ALPHABET
            regexDictionary.Add(33, @"^[\s]*([a-z][.])\s");  //  a.
            regexDictionary.Add(34, @"^[\s]*([a-z][:])\s");  //  a:
            regexDictionary.Add(35, @"^[\s]*([(][a-z][)])\s");  //    (a)
            regexDictionary.Add(36, @"^[\s]*([a-z][)])\s");  // a)
            regexDictionary.Add(37, @"^[\s]*([[][a-z][]])\s");  //   a]
            regexDictionary.Add(38, @"^[\s]*([a-z][]])\s");  //     [a]
            regexDictionary.Add(39, @"^[\s]*((section)[\s]*[a-z])");  //      section a
            regexDictionary.Add(40, @"^[\s]*((Section)[\s]*[a-z])");  //    Section a
            regexDictionary.Add(41, @"^[\s]*((SECTION)[\s]*[a-z])");  //    SECTION a
            regexDictionary.Add(42, @"^[\s]*([A-Z][.])\s");  // A.
            regexDictionary.Add(43, @"^[\s]*([A-Z][:])\s");  // A:
            regexDictionary.Add(44, @"^[\s]*([(][A-Z][)])\s");  // (A)
            regexDictionary.Add(45, @"^[\s]*([A-Z][)])\s");  // A)
            regexDictionary.Add(46, @"^[\s]*([[][A-Z][]])\s");  // A]
            regexDictionary.Add(47, @"^[\s]*([A-Z][]])\s");  // [A]
            regexDictionary.Add(48, @"^[\s]*((section)[\s]*[A-Z])\s");  // section A
            regexDictionary.Add(49, @"^[\s]*((Section)[\s]*[A-Z])\s");  // section A
            regexDictionary.Add(50, @"^[\s]*((SECTION)[\s]*[A-Z])\s");  // SECTION A


            List<string>  getAllSectionValue = new List<string>();
            var pageno = 0;
            var regexFound = false;
            var regexToFind = "";
            int startPage = 0;
            foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
            {
                pageno++;
                if (pageno < startPageVal)
                    continue;
                if (pageno > endPageVal)
                    break;
                if (startPage == 0)
                    startPage = pageno;
                var paraNoVal = 0;
                foreach (var checkPage in entry.Value) // each page value
                {
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
                                    startPage = pageno;
                                    getAllParaOfSection = new Dictionary<string, int>();
                                    getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                                    regexToFind = item.Value;
                                    regexFound = true;
                                    break;
                                }
                                else {
                                    if(!getAllParaOfSection.ContainsKey(checkPage.Value))
                                        getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                                }  
                            }
                        }
                        if(regexFound == false){
                            if (!getAllParaOfSection.ContainsKey(checkPage.Value))
                                getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                        }
                    }
                    else
                    {
                        var matchData = Regex.Matches(checkPage.Value, regexToFind); // find match
                        if (matchData.Count > 0)
                        {
                            getCompleteSection.Add( getAllParaOfSection, startPage);
                            startPage = pageno;
                            getAllParaOfSection = new Dictionary<string, int>();
                            getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                        }
                        else
                        {
                            if (!getAllParaOfSection.ContainsKey(checkPage.Value))
                                getAllParaOfSection.Add(checkPage.Value, paraNoVal);
                        }
                    }
                }
            }
            return getCompleteSection;
        }
    }
}
