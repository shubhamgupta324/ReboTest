using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rebo.Model.Regex
{
    public  class Regex
    {
        /// <summary>
        ///  regex to find the section in para
        /// </summary>
        public Dictionary<string, int> treeCorrectionRegex
        {
            get
            {
                return new Dictionary<string, int>{
            {@"^[\s]*(((?i)article|art1c1e|art1cle|artic1e)\s\d+\.(?:\d+\.?)*)(?!\S)",(int)Rebo.Enum.Processing.ParentRegex.article}, //  article 1.1 
            {@"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)", (int)Rebo.Enum.Processing.ParentRegex.article}, //    1.1  / 1.1 a)
            {@"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)", (int)Rebo.Enum.Processing.ParentRegex.numeric}, //    section 1.1 
            {@"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)", (int)Rebo.Enum.Processing.ParentRegex.numeric}, //    1./ 1. a)
            {@"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.ParentRegex.section}, //    section 1
            {@"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.ParentRegex.article}, //   article 1
            {@"^[\s]*(?!0)([0-9]{1,2}[:])(?!\S)", (int)Rebo.Enum.Processing.Processing.numeric}, //   1:
            {@"^[\s]*(?!0)([0-9]{1,2}[.])(?!\S)",(int)Rebo.Enum.Processing.Processing.numeric}, //   1.
            {@"^[\s]*([[(][\s]*(?!0)([0-9]{1,2})[\s]*[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.numeric}, //   (1)
            {@"^[\s]*(?!0)([0-9]{1,2}[]])(?!\S)", (int)Rebo.Enum.Processing.Processing.numeric}, //   1]
            {@"^[\s]*([[\\[][\s]*(?!0)([0-9]{1,2})[\s]*[]])(?!\S)", (int)Rebo.Enum.Processing.Processing.numeric}, //   [1]
            {@"^[\s]*(?!0)([0-9]{1,2}[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.numeric}, //   1)
            {@"^[\s]*[(][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[)](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    (xvii)
            {@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[)](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    xvii)
            {@"^[\s]*[[][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[]](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    [xvii]
            {@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[]](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    xvii]
            {@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[:](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    xvii:
            {@"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.](?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    xvii.
            {@"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    section xvii
            {@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.lowerCaseRoman}, //    article xvii 
            {@"^[\s]*[(][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[)](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    (XVII)
            {@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[)](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    XVII)
            {@"^[\s]*[[][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[]](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    [XVII]
            {@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[]](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    XVII]
            {@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[:](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    XVII:
            {@"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.](?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    XVII.
            {@"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    section XVII
            {@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.upperCaseRoman}, //    article XVII
            {@"^[\s]*([a-z][\s]{0,1}[.])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //  a.
            {@"^[\s]*([a-z][\s]{0,1}[:])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //  a:
            {@"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //    (a)
            {@"^[\s]*([a-z][\s]{0,1}[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  // a)
            {@"^[\s]*([[][\s]*[a-z][\s]{0,1}[]])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //   a]
            {@"^[\s]*([a-z][]])(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //     [a]
            {@"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //      section a
            {@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.lowercaseAlpha},  //      article a
            {@"^[\s]*([A-Z][\s]{0,1}[.])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // A.
            {@"^[\s]*([A-Z][\s]{0,1}[:])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // A:
            {@"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // (A)
            {@"^[\s]*([A-Z][\s]{0,1}[)])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // A)
            {@"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // A]
            {@"^[\s]*([A-Z][]])(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // [A]
            {@"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha},  // section A
            {@"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)", (int)Rebo.Enum.Processing.Processing.uppercaseAlpha}  // ARTICLE A
            };

            }
        }
        /// <summary>
        /// regex to ignore while checking tree
        /// </summary>
        public string[] regexNotToCheckFn { get {
            return new string[] { 
           @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)",  // 1.1  / 1.1 a)
           @"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)", //  article 1.1 
           @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))\s\d+\.(?:\d+\.?)*)(?!\S)", //  article 1.1 
           @"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)",//    section 1.1 
           @"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", //    section 1
           @"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)", //   article 1
           @"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", //    section xvii
           @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", //    article xvii 
           @"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)", //    section XVII
           @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)", //    article XVII
           @"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)", //      section a
           @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)",  //      article a
           @"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)",  // section A
           @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)"  // ARTICLE A
            };
        } }
        /// <summary>
        /// correction of regex while matching the regex of a section for a paragraph
        /// </summary>
        public Dictionary<string, string> regexCorrectionRegex { get {
            return new Dictionary<string, string> {            
            {@"^((?i)(section|article))?[\s]*((\d{1,2}|(I|l)(\d{1,1})|(\d{1,1})(I|l|O))[\s]{0,1}\.[\s]{0,1}(\d{1,2}|(I|l|O))([\s]{0,1}(I|l|O)|[\s]{0,1}[(?:\d+\.?)]*))[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.|•|-])?(?!\S)", @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)"},
            {@"^((?i)(section|article))?[\s]*(((?!0)\d{1,2}|(I|T|l)(\d{1,1})|(\d{1,1})(I|T|l))[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)", @"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"}
            };
        } }
        /// <summary>
        /// regex for matching section in a paragraph
        /// </summary>
        public Dictionary<int, string> regexDictionary { get {
            return new Dictionary<int, string> { 
            {44, @"^((?i)(section|article))?[\s]*((\d{1,2}|(I|l)(\d{1,1})|(\d{1,1})(I|l|O))[\s]{0,1}\.[\s]{0,1}(\d{1,2}|(I|l|O))([\s]{0,1}(I|l|O)|[\s]{0,1}[(?:\d+\.?)]*))[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|(?=[xvi])M*D?C{0,4}L?x{0,4}v?i{0,4})[\]|)|:|.|•|-])?(?!\S)"}, //    1.1  / 1.1 a)
            {1, @"^((?i)(section|article))?[\s]*(\d{1,2}\.\d[(?:\d+\.?)]*)[\s]?([(|\[]?([a-zA-Z]{1}|\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\]|)|:|.|•|-])?(?!\S)"}, //    1.1  / 1.1 a)
            {45, @"^((?i)(section|article))?[\s]*(((?!0)\d{1,2}|(I|T|l)(\d{1,1})|(\d{1,1})(I|T|l))[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"}, //    1./ 1. a)
            {2, @"^((?i)(section|article))?[\s]*((?!0)\d{1,2}[\s]{0,1}\.)[\s]?([(|\[]?(([a-zA-Z]{1})|(\d{1,2})|((x{0,2}){1,2}(ix|iv|v?i{0,3})|((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\]|)|:|.|•|-])(?!\S)"}, //    1./ 1. a)
            {3, @"^[\s]*(((?i)section)[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"}, //    section 1
            {4, @"^[\s]*(((?i)section)\s\d+\.(?:\d+\.?)*)(?!\S)"}, //    section 1.1 
            {5, @"^[\s]*((((?i)article|art1c1e|art1cle|artic1e))[\s]*\d*)[\s]{0,1}(.|:|•|-)?(?!\S)"}, //   article 1
            {6, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))\s\d+\.(?:\d+\.?)*)(?!\S)"}, //  article 1.1 
            {7, @"^[\s]*(?!0)([0-9]{1,2}[:])(?!\S)"}, //   1:
            {46, @"^[\s]*(?!0)([0-9]{1,2}[.])(?!\S)"}, //   1.
            {8, @"^[\s]*([[(][\s]*(?!0)([0-9]{1,2})[\s]*[)])(?!\S)"}, //   (1)
            {9, @"^[\s]*(?!0)([0-9]{1,2}[]])(?!\S)"}, //   1]
            {10, @"^[\s]*([[\\[][\s]*(?!0)([0-9]{1,2})[\s]*[]])(?!\S)"}, //   [1]
            {11, @"^[\s]*(?!0)([0-9]{1,2}[)])(?!\S)"}, //   1)
            {12, @"^[\s]*[(][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[)](?!\S)"}, //    (xvii)
            {13, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[)](?!\S)"}, //    xvii)
            {14, @"^[\s]*[[][\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]*[]](?!\S)"},//    [xvii]
            {15, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[]](?!\S)"},//    xvii]
            {16, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[:](?!\S)"},//    xvii:
            {17, @"^[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}[.](?!\S)"}, //    xvii.
            {18, @"^[\s]*((?i)section)[\s]*\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"}, //    section xvii
            {19, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((x{0,2}){1,2}(ix|iv|v?i{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"}, //    article xvii
            {20, @"^[\s]*[(][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[)](?!\S)"}, //    (XVII)
            {21, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[)](?!\S)"},//    XVII)
            {22, @"^[\s]*[[][\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]*[]](?!\S)"}, //    [XVII]
            {23, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[]](?!\S)"}, //    XVII]
            {24, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[:](?!\S)"}, //    XVII:
            {25, @"^[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}[.](?!\S)"}, //    XVII.
            {26, @"^[\s]*((?i)section)[\s]*\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))[\s]{0,1}(.|:|•|-)?(?!\S)"}, //    section XVII
            {27, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*(\b((X{0,2}){1,2}(IX|IV|V?I{0,3}))))[\s]{0,1}(.|:|•|-)?(?!\S)"}, //    article XVII
            {28, @"^[\s]*([a-z][\s]{0,1}[.])(?!\S)"},  //  a.
            {29, @"^[\s]*([a-z][\s]{0,1}[:])(?!\S)"},  //  a:
            {30, @"^[\s]*([(][\s]*[a-z][\s]*[)])(?!\S)"},  //    (a)
            {31, @"^[\s]*([a-z][\s]{0,1}[)])(?!\S)"},  // a)
            {32, @"^[\s]*([[][\s]*[a-z][\s]*[]])(?!\S)"},  //   a]
            {33, @"^[\s]*([a-z][\s]{0,1}[]])(?!\S)"},  //     [a]
            {34, @"^[\s]*((?i)(section)[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)"},  //      section a
            {35, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[a-z])[\s]{0,1}(.|:|•|-)?(?!\S)"},  //      article a
            {36, @"^[\s]*([A-Z][\s]{0,1}[.])(?!\S)"},  // A.
            {37, @"^[\s]*([A-Z][\s]{0,1}[:])(?!\S)"},  // A:
            {38, @"^[\s]*([(][\s]*[A-Z][\s]*[)])(?!\S)"},  // (A)
            {39, @"^[\s]*([A-Z][\s]{0,1}[)])(?!\S)"},  // A)
            {40, @"^[\s]*([[][\s]*[A-Z][\s]*[]])(?!\S)"},  // A]
            {41, @"^[\s]*([A-Z][\s]{0,1}[]])(?!\S)"},  // [A]
            {42, @"^[\s]*((?i)(section)[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)"},  // section A
            {43, @"^[\s]*(((?i)(article|art1c1e|art1cle|artic1e))[\s]*[A-Z])[\s]{0,1}(.|:|•|-)?(?!\S)"} // ARTICLE A
            };
        } }
    }


}
