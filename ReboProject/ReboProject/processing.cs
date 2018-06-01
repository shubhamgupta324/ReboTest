using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ReboProject
{
    public class processing
    {
        public static string SectionVal(Dictionary<int, Dictionary<int, string>> savePage, string AllSearchFieldKeyword1, int pageNo, int paraNumber)
        {
            var foundTextSection = AllSearchFieldKeyword1.ToLower();
            //---------------------------REGEX--------------------------------------------------------
            Dictionary<int, string> matchRegex = new Dictionary<int, string>();
            matchRegex.Add(1, @"^(\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
            matchRegex.Add(2, @"^(\d+(.\d+)+(\:\d+))"); //   (1.1:1)
            matchRegex.Add(3, @"^(\d+(\:\d+))"); //  (1:1)
            matchRegex.Add(4, @"^((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
            matchRegex.Add(5, @"^((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
            matchRegex.Add(6, @"^((section)\s\d+(\:\d+))"); //   (section 1:1)
            matchRegex.Add(7, @"^([a-zA-Z]+\.)\s");  // a.  
            matchRegex.Add(8, @"^(\(+[a-zA-Z]+\))\s");  // (a)
            matchRegex.Add(9, @"^(\(+\d+\))\s");  // (1)

            matchRegex.Add(10, @"^[ \t](\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
            matchRegex.Add(11, @"^[ \t](\d+(.\d+)+(\:\d+))"); //   (1.1:1)
            matchRegex.Add(12, @"^[ \t](\d+(\:\d+))"); //  (1:1)
            matchRegex.Add(13, @"^[ \t]((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
            matchRegex.Add(14, @"^[ \t]((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
            matchRegex.Add(15, @"^[ \t]((section)\s\d+(\:\d+))"); //   (section 1:1)
            matchRegex.Add(16, @"^[ \t]([a-zA-Z]+\.)\s");  // a.  
            matchRegex.Add(17, @"^[ \t](\(+[a-zA-Z]+\))\s");  // (a)
            matchRegex.Add(18, @"^[ \t](\(+\d+\))\s");  // (1)

            //-------------------------------------------------------------------------------------------
            var count = pageNo - 3;
            var endPageCheck = 0;
            if (count > 0)
                endPageCheck = pageNo - 2;

            for (int i = pageNo; i > endPageCheck; i--)
            {
                var entry1 = savePage[i].Values;
                var paraCount = paraNumber-1; // paraNumber
                if (i != pageNo)
                    paraCount = entry1.Count()-1;
                for (var j= paraCount;j >= 0; j--) {

                    var entry2 = entry1.ElementAt(j);
                    var textLower = @entry2.ToLower().ToString();
                    foreach (var check in matchRegex)
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
                }
            }
            return "false";
        }
    }
}