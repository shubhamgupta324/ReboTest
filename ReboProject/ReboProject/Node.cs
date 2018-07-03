using Newtonsoft.Json.Linq;
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


        public static void checkPara(Dictionary<string, string> sectionList, string sectionNoreadAllParaData, JToken subChild, out List<string> sectionListCopy, out bool foundSection, out bool readSection)
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
                    if ((paraVal.IndexOf(sectionNoreadAllParaData.Trim()) != -1 | sectionNoreadAllParaData.Trim().IndexOf(paraVal) != -1) & paraVal.Length > 10)
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
                    checkPara(sectionList, sectionNoreadAllParaData, child, out sectionListCopy, out foundSection, out readSection);
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

        // get the complete section 
        public static string completeSectionPara(string singleFileSectionTree, string sectionNoreadPara)
        {

            var finalSectionOutput = "";

            var sectionNoreadAllPara = sectionNoreadPara.Split('|');

            var sectionNoreadAllParaData = "";
            var list = new List<string>(sectionNoreadAllPara);
            if (sectionNoreadAllPara.Count() > 1)
                sectionNoreadAllParaData = list.ElementAt(1);
            else
                sectionNoreadAllParaData = list.ElementAt(0);

            var completeTree = JObject.Parse(singleFileSectionTree)["children"];
            //----------get section data---------------

            Dictionary<string, string> sectionList = new Dictionary<string, string>();
            List<string> sectionListCopy = new List<string>();

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
                        if ((paraVal.IndexOf(sectionNoreadAllParaData.Trim()) != -1 | sectionNoreadAllParaData.Trim().IndexOf(paraVal) != -1) & paraVal.Length > 20)
                        {
                            if (!sectionListCopy.Contains(sectioNo) & readSection == true & !sectionList.ContainsKey(sectioNo))
                            {
                                sectionList.Add(sectioNo, "sectionNo");
                                if (parentCheck == 1)
                                {
                                    CheckNextChild = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (subChildCount > 0 & CheckNextChild == true)
                    {
                        var sectionNoReturn = new List<string>();
                        var foundSection = false;
                        checkPara(sectionList, sectionNoreadAllParaData, subChild, out sectionListCopy, out foundSection, out readSection);
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
                            sectionList.Add(sectionData, "SectionNo");
                        }
                        if (SectionNameData != null)
                            sectionList.Add(SectionNameData, "sectionParent");
                        break;
                    }
                    else if (CheckNextChild == false & sectionList.Count() > 0)
                    {
                        if (SectionNameData != null)
                            sectionList.Add(SectionNameData, "sectionParent");
                        break;
                    }
                }
                if (CheckNextChild == false)
                    break;
            }


            return finalSectionOutput;
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
            matchRegexNumeric.Add(7, @"^[\s]*([1-9]{1,2}[:])(?!\S)"); //   1:
            matchRegexNumeric.Add(8, @"^[\s]*([[(][\s]*[1-9]{1,2}[\s]*[)])(?!\S)"); //   (1)
            matchRegexNumeric.Add(9, @"^[\s]*([1-9]{1,2}[]])(?!\S)"); //   1]
            matchRegexNumeric.Add(10, @"^[\s]*([[[\s]*[1-9]{1,2}[\s]*[]])(?!\S)"); //   [1]
            matchRegexNumeric.Add(11, @"^[\s]*([1-9]{1,2}[)])(?!\S)"); //   1)
            
            matchRegexNumeric.Add(12, @"^[\s]*[(][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[)](?!\S)"); //    (xvii)
            matchRegexNumeric.Add(13, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)](?!\S)"); //    xvii)
            matchRegexNumeric.Add(14, @"^[\s]*[[][\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[\s]*[]](?!\S)"); //    [xvii]
            matchRegexNumeric.Add(15, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]](?!\S)"); //    xvii]
            matchRegexNumeric.Add(16, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:](?!\S)"); //    xvii:
            matchRegexNumeric.Add(17, @"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.](?!\S)"); //    xvii.
            matchRegexNumeric.Add(18, @"^[\s]*(?i)(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    section xvii
            matchRegexNumeric.Add(19, @"^[\s]*(?i)(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]?(?!\S)"); //    article xvii
            
            matchRegexNumeric.Add(20, @"^[\s]*[(][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[)](?!\S)"); //    (XVII)
            matchRegexNumeric.Add(21, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)](?!\S)"); //    XVII)
            matchRegexNumeric.Add(22, @"^[\s]*[[][\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[\s]*[]](?!\S)"); //    [XVII]
            matchRegexNumeric.Add(23, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]](?!\S)"); //    XVII]
            matchRegexNumeric.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:](?!\S)"); //    XVII:
            matchRegexNumeric.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.](?!\S)"); //    XVII.
            matchRegexNumeric.Add(26, @"^[\s]*(?i)(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    section XVII
            matchRegexNumeric.Add(27, @"^[\s]*(?i)(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]?(?!\S)"); //    article XVII
            
            matchRegexNumeric.Add(28, @"^[\s]*([a-z][.])(?!\S)");  //  a.
            matchRegexNumeric.Add(29, @"^[\s]*([a-z][:])(?!\S)");  //  a:
            matchRegexNumeric.Add(30, @"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)");  //    (a)
            matchRegexNumeric.Add(31, @"^[\s]*([a-z][)])(?!\S)");  // a)
            matchRegexNumeric.Add(32, @"^[\s]*([[][\s]*[a-z][\s]*[]])(?!\S)");  //   a]
            matchRegexNumeric.Add(33, @"^[\s]*([a-z][]])(?!\S)");  //     [a]
            matchRegexNumeric.Add(34, @"^[\s]*((?i)(section)[\s]*[a-z])[.]?(?!\S)");  //      section a
            matchRegexNumeric.Add(35, @"^[\s]*((?i)(article)[\s]*[a-z])[.]?(?!\S)");  //      article a
            
            matchRegexNumeric.Add(36, @"^[\s]*([A-Z][.])(?!\S)");  // A.
            matchRegexNumeric.Add(37, @"^[\s]*([A-Z][:])(?!\S)");  // A:
            matchRegexNumeric.Add(38, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)");  // (A)
            matchRegexNumeric.Add(39, @"^[\s]*([A-Z][)])(?!\S)");  // A)
            matchRegexNumeric.Add(40, @"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)");  // A]
            matchRegexNumeric.Add(41, @"^[\s]*([A-Z][]])(?!\S)");  // [A]
            matchRegexNumeric.Add(42, @"^[\s]*((?i)(section)[\s]*[A-Z])[.]?(?!\S)");  // section A
            matchRegexNumeric.Add(43, @"^[\s]*((?i)(ARTICLE)[\s]*[A-Z])[.]?(?!\S)");  // ARTICLE A

            foreach (var item in matchRegexNumeric) // loop through all the regex to check the section no
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
