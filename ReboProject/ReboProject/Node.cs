using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReboProject
{
    class Program
    {
        static int defaultLevel = -1;
        static string regexString = "";
        static Node root = new Node("ROOT", defaultLevel);
        static Dictionary<int, Dictionary<string, int>> getCompleteSection = new Dictionary<int, Dictionary<string, int>>();
        static Stack<KeyValuePair<int, Node>> stack = new Stack<KeyValuePair<int, Node>>();
        static List<int> matchedKeys = new List<int>();
        static Node lastNode;
        public static Dictionary<Dictionary<string, int>, int> SectionVal123(Dictionary<int, Dictionary<int, string>> savePage)
        {
            //stack = new Stack<KeyValuePair<int, Node>>();
            //matchedKeys = new List<int>();
            //root = new Node("ROOT", defaultLevel);
            //regexString = "";
            Dictionary<Dictionary<string, int>, int> getCompleteSection = new Dictionary<Dictionary<string, int>, int>();
            Dictionary<string,int> getAllParaOfSection = new Dictionary<string, int>();
            var regexDictionary = new Dictionary<int, string>();

            regexDictionary.Add(1, @"^(\d{1,3}\.(:\d+\.?)*)\s"); //    1.
            regexDictionary.Add(2, @"^(\d{1,3}\.\d(?:\d+\.?)*)"); //    1.1 
            regexDictionary.Add(3, @"^[\s]*((section)\s\d+\.(:\d+\.?)*)\s"); //    section 1.
            regexDictionary.Add(4, @"^[\s]*((sectionss)\s\d+\.\d(?:\d+\.?)*)"); //    section 1.1 
           // regexDictionary.Add(5, @"^[\s]*((article)\s\d\s)"); //   article 1,
            regexDictionary.Add(5, @"^[\s]*(\d{1,3}[:])\s"); //   1:
            regexDictionary.Add(6, @"^[\s]*(\d{1,3}[)])\s"); //   1)
            regexDictionary.Add(7, @"^[\s]*(\d{1,3}[]])\s"); //   1]
            regexDictionary.Add(8,@"^[\s]*([[]+\d+[]])\s"); //   [1]
            regexDictionary.Add(9,@"^[\s]*([[(]+\d+[)])\s"); //   (1)

            regexDictionary.Add(10, @"^[\s]*((Section)\s\d+\.(:\d+\.?)*)\s"); //    Section 1.
           // regexDictionary.Add(55, @"^[\s]*((Article)\s\d\s)"); //   Article 1,
            regexDictionary.Add(11, @"^[\s]*((Section)\s\d+\.\d(?:\d+\.?)*)"); //    Section 1.1 
            //regexDictionary.Add(57, @"^[\s]*((Article)\s\d+\.\d(?:\d+\.?)*)"); //    Article 1.1 
            regexDictionary.Add(12, @"^[\s]*((SECTION)\s\d+\.\d(?:\d+\.?)*)"); //    SECTION 1.1 
           // regexDictionary.Add(59, @"^[\s]*((ARTICLE)\s\d+\.\d(?:\d+\.?)*)"); //    ARTICLE 1.1 
           // regexDictionary.Add(68, @"^[\s]*((ARTICLE)\s\d)"); //   ARTICLE 1,

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
            //regexDictionary.Add(20, @"^[\s]*(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    article xvii

            regexDictionary.Add(21, @"^[\s]*(Section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    Section xvii
            //regexDictionary.Add(53, @"^[\s]*(Article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    Article xvii
            regexDictionary.Add(22, @"^[\s]*(SECTION)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    SECTION xvii
            //regexDictionary.Add(61, @"^[\s]*(ARTICLE)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}"); //    ARTICLE xvii

            // lower
            regexDictionary.Add(23, @"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    (XVII)
            regexDictionary.Add(24, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s"); //    XVII
            regexDictionary.Add(25, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s"); //    XVII)
            regexDictionary.Add(26, @"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    [XVII]
            regexDictionary.Add(27, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s"); //    XVII]
            regexDictionary.Add(28, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s"); //    XVII:
            regexDictionary.Add(29, @"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s"); //    XVII.
            regexDictionary.Add(30, @"^[\s]*(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    section XVII
           // regexDictionary.Add(29, @"^[\s]*(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    article XVII

            regexDictionary.Add(31, @"^[\s]*(Section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    Section XVII
            //regexDictionary.Add(51, @"^[\s]*(Article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    Article XVII
            regexDictionary.Add(32, @"^[\s]*(SECTION)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    SECTION XVII
           // regexDictionary.Add(63, @"^[\s]*(ARTICLE)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}"); //    ARTICLE XVII

            // ALPHABET

            regexDictionary.Add(33, @"^[\s]*([a-z][.])\s");  //  a.
            regexDictionary.Add(34, @"^[\s]*([a-z][:])\s");  //  a: 
            regexDictionary.Add(35, @"^[\s]*([(][a-z][)])\s");  //    (a) 
            regexDictionary.Add(36, @"^[\s]*([a-z][)])\s");  // a) 
            regexDictionary.Add(37, @"^[\s]*([[][a-z][]])\s");  //   a] 
            regexDictionary.Add(38, @"^[\s]*([a-z][]])\s");  //     [a]
            regexDictionary.Add(39, @"^[\s]*((section)[\s]*[a-z])");  //      section a 
            //regexDictionary.Add(37, @"^[\s]*((article)[\s]*[a-z])");  //      article a

            regexDictionary.Add(40, @"^[\s]*((Section)[\s]*[a-z])");  //    Section a
            //regexDictionary.Add(47, @"^[\s]*((Article)[\s]*[a-z])");  //    Article a
            regexDictionary.Add(41, @"^[\s]*((SECTION)[\s]*[a-z])");  //    SECTION a
            //regexDictionary.Add(65, @"^[\s]*((ARTICLE)[\s]*[a-z])");  //    ARTICLE a

            regexDictionary.Add(42, @"^[\s]*([A-Z][.])\s");  // A. 
            regexDictionary.Add(43, @"^[\s]*([A-Z][:])\s");  // A: 
            regexDictionary.Add(44, @"^[\s]*([(][A-Z][)])\s");  // (A) 
            regexDictionary.Add(45, @"^[\s]*([A-Z][)])\s");  // A) 
            regexDictionary.Add(46, @"^[\s]*([[][A-Z][]])\s");  // A]  
            regexDictionary.Add(47, @"^[\s]*([A-Z][]])\s");  // [A]    
            regexDictionary.Add(48, @"^[\s]*((section)[\s]*[A-Z])\s");  // section A    
           // regexDictionary.Add(45, @"^[\s]*((article)[\s]*[A-Z])\s");  // article A    

            regexDictionary.Add(49, @"^[\s]*((Section)[\s]*[A-Z])\s");  // section A   
            //regexDictionary.Add(49, @"^[\s]*((Article)[\s]*[A-Z])\s");  // Article A  
            regexDictionary.Add(50, @"^[\s]*((SECTION)[\s]*[A-Z])\s");  // SECTION A   
            //regexDictionary.Add(67, @"^[\s]*((ARTICLE)[\s]*[A-Z])\s");  // ARTICLE A   


            List<string>  getAllSectionValue = new List<string>();
            var pageno = 0;
            var regexFound = false;
            var regexToFind = "";
            int startPage = 0;
            foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
            {
                pageno++;
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
                            //processElement(checkPage.Value , pageno);
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
            //for (int i = 0; i < root.Children.Count(); i++)
            //{
            //    getAllSectionValue.Add(root.Children[i].Value);
            //}
            return getCompleteSection;
        }
        static void processElement(string ele, int pagNo)
        {
            Dictionary<string, int> saveSection = new Dictionary<string, int>();
            var regexDictionary = new Dictionary<int, Regex>();
            regexDictionary.Add(1, new Regex(@"^(\d{1,3}\.(:\d+\.?)*)\s")); //    1.
            regexDictionary.Add(2, new Regex(@"^(\d{1,3}\.\d(?:\d+\.?)*)")); //    1.1 
            regexDictionary.Add(3, new Regex(@"^[\s]*((section)\s\d+\.(:\d+\.?)*)\s")); //    section 1.
            regexDictionary.Add(4, new Regex(@"^[\s]*((sectionss)\s\d+\.\d(?:\d+\.?)*)")); //    section 1.1 
            regexDictionary.Add(5, new Regex(@"^[\s]*((article)\s\d\s)")); //   article 1,
            regexDictionary.Add(7, new Regex(@"^[\s]*(\d{1,3}[:])\s")); //   1:
            regexDictionary.Add(8, new Regex(@"^[\s]*(\d{1,3}[)])\s")); //   1)
            regexDictionary.Add(9, new Regex(@"^[\s]*(\d{1,3}[]])\s")); //   1]
            regexDictionary.Add(10, new Regex(@"^[\s]*([[]+\d+[]])\s")); //   [1]
            regexDictionary.Add(11, new Regex(@"^[\s]*([[(]+\d+[)])\s")); //   (1)

            regexDictionary.Add(54, new Regex(@"^[\s]*((Section)\s\d+\.(:\d+\.?)*)\s")); //    Section 1.
            regexDictionary.Add(55, new Regex(@"^[\s]*((Article)\s\d\s)")); //   Article 1,
            regexDictionary.Add(56, new Regex(@"^[\s]*((Section)\s\d+\.\d(?:\d+\.?)*)")); //    Section 1.1 
            regexDictionary.Add(57, new Regex(@"^[\s]*((Article)\s\d+\.\d(?:\d+\.?)*)")); //    Article 1.1 
            regexDictionary.Add(58, new Regex(@"^[\s]*((SECTION)\s\d+\.\d(?:\d+\.?)*)")); //    SECTION 1.1 
            regexDictionary.Add(59, new Regex(@"^[\s]*((ARTICLE)\s\d+\.\d(?:\d+\.?)*)")); //    ARTICLE 1.1 
            regexDictionary.Add(68, new Regex(@"^[\s]*((ARTICLE)\s\d)")); //   ARTICLE 1,

            // ROMAN
            // upper
            regexDictionary.Add(12, new Regex(@"^[\s]*[(](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s")); //    (xvii)
            regexDictionary.Add(13, new Regex(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}\s")); //    xvii
            regexDictionary.Add(14, new Regex(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[)]\s")); //    xvii)
            regexDictionary.Add(15, new Regex(@"^[\s]*[[](?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s")); //    [xvii]
            regexDictionary.Add(16, new Regex(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[]]\s")); //    xvii]
            regexDictionary.Add(17, new Regex(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[:]\s")); //    xvii:
            regexDictionary.Add(18, new Regex(@"^[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}[.]\s")); //    xvii.
            regexDictionary.Add(19, new Regex(@"^[\s]*(section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    section xvii
            regexDictionary.Add(20, new Regex(@"^[\s]*(article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    article xvii

            regexDictionary.Add(52, new Regex(@"^[\s]*(Section)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    Section xvii
            regexDictionary.Add(53, new Regex(@"^[\s]*(Article)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    Article xvii
            regexDictionary.Add(60, new Regex(@"^[\s]*(SECTION)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    SECTION xvii
            regexDictionary.Add(61, new Regex(@"^[\s]*(ARTICLE)[\s]*(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4}")); //    ARTICLE xvii

            // lower
            regexDictionary.Add(21, new Regex(@"^[\s]*[(](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s")); //    (XVII)
            regexDictionary.Add(22, new Regex(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}\s")); //    XVII
            regexDictionary.Add(23, new Regex(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[)]\s")); //    XVII)
            regexDictionary.Add(24, new Regex(@"^[\s]*[[](?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s")); //    [XVII]
            regexDictionary.Add(25, new Regex(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[]]\s")); //    XVII]
            regexDictionary.Add(26, new Regex(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[:]\s")); //    XVII:
            regexDictionary.Add(27, new Regex(@"^[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}[.]\s")); //    XVII.
            regexDictionary.Add(28, new Regex(@"^[\s]*(section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    section XVII
            regexDictionary.Add(29, new Regex(@"^[\s]*(article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    article XVII

            regexDictionary.Add(50, new Regex(@"^[\s]*(Section)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    Section XVII
            regexDictionary.Add(51, new Regex(@"^[\s]*(Article)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    Article XVII
            regexDictionary.Add(62, new Regex(@"^[\s]*(SECTION)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    SECTION XVII
            regexDictionary.Add(63, new Regex(@"^[\s]*(ARTICLE)[\s]*(?=[XVI])M*D?C{0,4}L?X{0,4}V?I{0,4}")); //    ARTICLE XVII

            // ALPHABET

            regexDictionary.Add(30, new Regex(@"^[\s]*([a-z][.])\s"));  //  a.
            regexDictionary.Add(31, new Regex(@"^[\s]*([a-z][:])\s"));  //  a: 
            regexDictionary.Add(32, new Regex(@"^[\s]*([(][a-z][)])\s"));  //    (a) 
            regexDictionary.Add(33, new Regex(@"^[\s]*([a-z][)])\s"));  // a) 
            regexDictionary.Add(34, new Regex(@"^[\s]*([[][a-z][]])\s"));  //   a] 
            regexDictionary.Add(35, new Regex(@"^[\s]*([a-z][]])\s"));  //     [a]
            regexDictionary.Add(36, new Regex(@"^[\s]*((section)[\s]*[a-z])"));  //      section a 
            regexDictionary.Add(37, new Regex(@"^[\s]*((article)[\s]*[a-z])"));  //      article a

            regexDictionary.Add(46, new Regex(@"^[\s]*((Section)[\s]*[a-z])"));  //    Section a
            regexDictionary.Add(47, new Regex(@"^[\s]*((Article)[\s]*[a-z])"));  //    Article a
            regexDictionary.Add(64, new Regex(@"^[\s]*((SECTION)[\s]*[a-z])"));  //    SECTION a
            regexDictionary.Add(65, new Regex(@"^[\s]*((ARTICLE)[\s]*[a-z])"));  //    ARTICLE a

            regexDictionary.Add(38, new Regex(@"^[\s]*([A-Z][.])\s"));  // A. 
            regexDictionary.Add(39, new Regex(@"^[\s]*([A-Z][:])\s"));  // A: 
            regexDictionary.Add(40, new Regex(@"^[\s]*([(][A-Z][)])\s"));  // (A) 
            regexDictionary.Add(41, new Regex(@"^[\s]*([A-Z][)])\s"));  // A) 
            regexDictionary.Add(42, new Regex(@"^[\s]*([[][A-Z][]])\s"));  // A]  
            regexDictionary.Add(43, new Regex(@"^[\s]*([A-Z][]])\s"));  // [A]    
            regexDictionary.Add(44, new Regex(@"^[\s]*((section)[\s]*[A-Z])\s"));  // section A    
            regexDictionary.Add(45, new Regex(@"^[\s]*((article)[\s]*[A-Z])\s"));  // article A    

            regexDictionary.Add(48, new Regex(@"^[\s]*((Section)[\s]*[A-Z])\s"));  // section A   
            regexDictionary.Add(49, new Regex(@"^[\s]*((Article)[\s]*[A-Z])\s"));  // Article A  
            regexDictionary.Add(66, new Regex(@"^[\s]*((SECTION)[\s]*[A-Z])\s"));  // SECTION A   
            regexDictionary.Add(67, new Regex(@"^[\s]*((ARTICLE)[\s]*[A-Z])\s"));  // ARTICLE A   



            foreach (KeyValuePair<int, Regex> item in regexDictionary)
            {
                var test = item.Value.Matches(ele);
                foreach (Match match in test)
                {
                    for (int i = 0; i < match.Captures.Count; i++)
                    {
                        var nodeValue = match.Captures[i].Value;

                        if (stack.Count == 0)
                        {
                            regexString = item.Value.ToString();
                            var child = new Node(nodeValue, root.Level + 1);
                            root.Children.Add(child);

                            lastNode = child;

                            stack.Push(new KeyValuePair<int, Node>(item.Key, root));
                        }
                        else
                        {
                            if (matchedKeys.Contains(item.Key))
                            {
                                var breakFlag = false;
                                while (breakFlag == false)
                                {
                                    if (stack.Count == 0)
                                        break;

                                    var top = stack.Peek();
                                    if (top.Key == item.Key)
                                    {
                                        var child = new Node(nodeValue, top.Value.Level + 1);
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
                                var child = new Node(nodeValue, lastNode.Level + 1);
                                lastNode.Children.Add(child);

                                stack.Push(new KeyValuePair<int, Node>(item.Key, lastNode));
                            }
                        }

                    }

                    if (matchedKeys.Contains(item.Key) == false)
                        matchedKeys.Add(item.Key);
                }
            }
        }
    }


    class Node
    {
        public string Value { get; set; }
        public int Level { get; set; }
        public List<Node> Children { get; set; }

        public Node(string val, int level)
        {
            Value = val;
            Level = level;
            Children = new List<Node>();
        }
    }
}
