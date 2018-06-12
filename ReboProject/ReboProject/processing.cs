using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ReboProject
{
    public class processing
    {
        public static string SectionVal(Dictionary<int, Dictionary<int, string>> savePage, int pageNo, int paraNumber)
        {
            //---------------------------REGEX--------------------------------------------------------
            Dictionary<int, string> matchRegexNumeric = new Dictionary<int, string>();
            matchRegexNumeric.Add(1, @"^(\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
            matchRegexNumeric.Add(2, @"^(\d+(.\d+)+(\:\d+))"); //   (1.1:1)
            matchRegexNumeric.Add(3, @"^(\d+(\:\d+))"); //  (1:1)
            matchRegexNumeric.Add(4, @"^((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
            matchRegexNumeric.Add(5, @"^((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
            matchRegexNumeric.Add(6, @"^((section)\s\d+(\:\d+))"); //   (section 1:1)
            matchRegexNumeric.Add(7, @"^(\(+\d+\))\s");  // (1)

            matchRegexNumeric.Add(8, @"^[ \t](\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
            matchRegexNumeric.Add(9, @"^[ \t](\d+(.\d+)+(\:\d+))"); //   (1.1:1)
            matchRegexNumeric.Add(10, @"^[ \t](\d+(\:\d+))"); //  (1:1)
            matchRegexNumeric.Add(11, @"^[ \t]((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
            matchRegexNumeric.Add(12, @"^[ \t]((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
            matchRegexNumeric.Add(13, @"^[ \t]((section)\s\d+(\:\d+))"); //   (section 1:1)
            matchRegexNumeric.Add(14, @"^[ \t](\(+\d+\))\s");  // (1)

            Dictionary<int, string> matchRegexAlphabet = new Dictionary<int, string>();
            matchRegexAlphabet.Add(1, @"^[ \t]([a-zA-Z]+\.)\s");  // a.  
            matchRegexAlphabet.Add(2, @"^[ \t](\(+[a-zA-Z]+\))\s");  // (a)
            matchRegexAlphabet.Add(3, @"^([a-zA-Z]+\.)\s");  // a.  
            matchRegexAlphabet.Add(4, @"^(\(+[a-zA-Z]+\))\s");  // (a)

            //-------------------------------------------------------------------------------------------
            var count = pageNo - 3;
            var endPageCheck = 0;
            if (count > 0)
                endPageCheck = pageNo - 2;

            for (int i = pageNo; i > endPageCheck; i--) // loop through current and last 2 pages
            {
                var entry1 = savePage[i].Values; // get the content of current page
                var paraCount = paraNumber-1; // paraNumber // get the para number of result in current page
                if (i != pageNo) // if previous page 
                    paraCount = entry1.Count()-1; // take all para in a page to check
                for (var j= paraCount;j >= 0; j--) { // loop th

                    var entry2 = entry1.ElementAt(j);
                    var textLower = @entry2.ToLower().ToString();
                    foreach (var check in matchRegexNumeric)
                    {
                        String AllowedChars = check.Value;
                        Regex regex = new Regex(AllowedChars);
                        var match = regex.Match(textLower);
                        if (match.Success)
                        {
                            var sectionVal = (match.Value).Replace("section", "");
                            return sectionVal;
                        }             
                    }
                    var text = @entry2.ToLower().ToString();
                    foreach (var  check in matchRegexAlphabet) {
                        String AllowedChars = check.Value;
                        Regex regex = new Regex(AllowedChars);
                        var match = regex.Match(text);
                        if (match.Success)
                        {
                            var sectionVal = (match.Value);
                            var pageCountForSection = i - 5;
                            if (pageCountForSection <= 0)
                                pageCountForSection = 0;
                                for (var t=i;t> pageCountForSection; t++ ) {

                                    var entryCurrentPage1 = savePage[i].Values;
                                    var paraCountTotal = j-1; // paraNumber
                                    if (i != t)
                                        paraCountTotal = entryCurrentPage1.Count() - 1;
                                    for (var k = paraCountTotal; k >= 0; k--) {
                                        var entryCurrentPage2 = entryCurrentPage1.ElementAt(k);
                                        foreach (var checkAlphabet in matchRegexNumeric)
                                        {
                                            String AllowedCharsAlphabet = checkAlphabet.Value;
                                            Regex regexAlphabet = new Regex(AllowedCharsAlphabet);
                                            var matchAlphabet = regexAlphabet.Match(entryCurrentPage2);
                                            if (matchAlphabet.Success)
                                            {
                                                var sectionVaAlphabetl = matchAlphabet.Value +" "+ sectionVal;
                                                return sectionVaAlphabetl;
                                            }
                                        }
                                    }
                                }
                        }
                    }
                }
            }
            return "false";
        }
    }
}