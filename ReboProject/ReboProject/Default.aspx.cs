using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using WebSupergoo.ABCpdf10;
using WebSupergoo.ABCpdf10.Operations;
using System.Collections;
using System.Text.RegularExpressions;
using ReboProject;

namespace ReboProject
{
    public partial class _Default : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void TestClick(object sender, EventArgs e)
        {
            
            string backEndVal = backEndData.Text;
            if (backEndVal == "")
                return;
            var backendObject = JObject.Parse(backEndVal.ToString());
            var folder = backendObject["folder"].ToString();// folder path
            var sort = (int)backendObject["FileOrder"]["sort"]; // asc or desc
            var type = (int)backendObject["FileOrder"]["type"]; // Single File Search or All File Search
            var SearchWithin = (int)backendObject["SearchWithin"];// 1=page, 2=section, 3=paragraph
            var resultSection = (int)backendObject["result"][0]["section"];// page or sentance or paragraph
            var resultFormat = backendObject["result"][0]["format"].ToString();// eg: {{filename}}; {{result}}: {{pagenumber}}
            var logic = backendObject["logic"]; // all searchFor and there respected withIn

            var accptedValThere = 0; // check if lease is silent or not
            var ja3 = new JArray(); // display on front end

            // ------------------get the total score and the field to search-------------------------------
            Dictionary<string, int> searchFieldScore = new Dictionary<string, int>(); // saves the foundtext
            var totalScoreDenominatorVal = 0; // get the total score of all the search field
            
            
            //------------------------------------------------loop through all lease--------------------------------------------------
            var folderPath = "";
            string drivePath = WebConfigurationManager.AppSettings["DrivePath"];  
            var userDefinePath = drivePath + folder;
            string[] subdirectoryEntries = Directory.GetDirectories(userDefinePath);
            string[] leasePath = new string[subdirectoryEntries.Count()];
            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++)
            {
                // to save the final result
                var ja1 = new JArray();
                var ja2 = new JArray();

                // get the files to read
                folderPath = subdirectoryEntries[folderval]; // get the folder to get the 
                string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf").Select(Path.GetFileName).ToArray(); // get all the file name in that folder
                ArrayList fileDetails = new ArrayList();

                // get all the files to read
                fileToRead(folderPath, sort, type, pdfFiles, out fileDetails);

                var FilePathPlusLeaseName = folderPath.Split('\\');
                var LeaseName = FilePathPlusLeaseName[FilePathPlusLeaseName.Length - 1]; // get the lease name
                var fileName = "";
                float finalScore = 0;
                float TopfinalScore = 0;
                var resultFound = 1;
                // -------------------------------------loop through all files----------------------------------------------------
                foreach (string fullFilePath in fileDetails)
                {
                    var ja = new JArray();
                    var getTheIndividualFileName = fullFilePath.Split('\\');
                    fileName = getTheIndividualFileName[getTheIndividualFileName.Length - 1]; // get the file name
                    int index = fileName.LastIndexOf(".");
                    fileName = fileName.Substring(0, index);
                    //read the file
                    Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it

                    pdfRead(fullFilePath, out savePage); // read pdf
                    //pdfReadSection(fullFilePath, out savePage);

                    getAllFoundText(SearchWithin, resultSection, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore); //  get the found text
                    
                    //--------------------scoring and final output ---------------------------------------------------------------------
                    scoring(LeaseName, savePage, resultFormat, totalScoreDenominatorVal, searchFieldScore, ja, out ja1, out accptedValThere, out finalScore);

                    
                    for (int i = 0; i < ja1.Count; i++)// get only one highest score value
                    {
                        if (TopfinalScore < finalScore) 
                        {
                            TopfinalScore = finalScore;
                            ja2.RemoveAll();
                            ja2.Add(ja1[i]);
                        }
                    }   
                }
                //---------------------save the result in folder----------------------------------------------
                if (ja2.Count == 0)
                { // check if any result found
                    resultFound = 0;
                }
                saveDataToFolder(resultFound, ja2, folderPath, fileName);
                for (int i = 0; i < ja2.Count; i++) // get all the values to display on front end
                    ja3.Add(ja2[i]);

                if (accptedValThere == 0) // if no result found "lease is silent"
                    Text1.Value = "Lease is silent";

            }
            frontEndData.Text = ja3.ToString(); // set the data in front end
        }
        
        // get the score and the field to search
        public void getTotalScore(JToken withIn , JToken searchFor , out int totalScoreDenominatorVal , out Dictionary<string,int> searchFieldScore) {

            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            // searchFor
            for (var k = 0; k < searchFor.Count(); k++) // loop throuch all searchFor
            {
                var AllSearchFieldKeyword = (searchFor[k]["keyword"]).ToString(); // get the search field
                 var AllSearchFieldScore = (int)(searchFor[k]["score"]); // get the search field score
                if (!searchFieldScore.ContainsKey(AllSearchFieldKeyword)) {
                    searchFieldScore.Add(AllSearchFieldKeyword, AllSearchFieldScore);
                    totalScoreDenominatorVal += AllSearchFieldScore;
                }
                
            }

            // withIn
            for (var h = 0; h < withIn.Count(); h++)
            {
                var withInKeyword = (withIn[h]["keyword"]).ToString();
                var withInscore = (int)(withIn[h]["score"]);
                if (!searchFieldScore.ContainsKey(withInKeyword))
                {
                    searchFieldScore.Add(withInKeyword, withInscore);// add the value for score
                    totalScoreDenominatorVal += withInscore;
                }
                    
            }
        }


        // read the file and get lines 
        public void fileToRead(string folderPath, int sort, int type, string[] pdfFiles, out ArrayList fileDetails)
        {

            fileDetails = new ArrayList();
            if (pdfFiles.Length != 0)
            {
                for (var j = 0; j < pdfFiles.Length; j++)
                {
                    var index = sort == 2 ? pdfFiles.Length - (j + 1) : j; // get the file by order
                    var fileNameVal = pdfFiles[index];
                    var filepath = folderPath + "\\" + fileNameVal; // full path of file
                    if (type == 1) // read single file 
                    {
                        fileDetails.Add(filepath);
                        break;
                    }
                    else // read all files
                        fileDetails.Add(filepath);
                }
            }
        }
        

        // get the paragraph lines

        public void pdfRead(string filepath, out Dictionary<int, Dictionary<int, string>> savePage)
        {
            savePage = new Dictionary<int, Dictionary<int, string>>();
            using (Doc doc = new Doc())
            {
                doc.Read(filepath);
                short PageIndex = 1;
                while (PageIndex <= doc.PageCount) // save the value in dictionary 
                {
                    TextOperation op = new TextOperation(doc);
                    op.PageContents.AddPages(PageIndex);
                    string theText = op.GetText();

                    IList<TextGroup> theGroups = op.Group(op.Select(0, theText.Length));
                    List<TextGroup> ordereddGroups = theGroups.OrderByDescending(o => o.Rect.Top).ToList();

                    StringBuilder sb1 = new StringBuilder();
                    XRect prevRect = new XRect("0 0 0 0");

                    Dictionary<int, string> saveLines = new Dictionary<int, string>();
                    var i = 1;
                    foreach (TextGroup lineGroup in ordereddGroups)
                    {
                        if ((prevRect.Bottom - lineGroup.Rect.Top) > 9)
                        { // current line is > 9 points lower than previous
                            saveLines.Add(i, sb1.ToString());
                            i++;
                            sb1.Clear();
                        }
                        sb1.Append(lineGroup.Text);
                        prevRect.String = lineGroup.Rect.String;
                    }
                    saveLines.Add(i, sb1.ToString());
                    savePage.Add(PageIndex, saveLines);
                    PageIndex++;
                }

            }
        }

        public void pdfReadSection(string filepath, out Dictionary<int, Dictionary<int, string>> savePage)
        {
            savePage = new Dictionary<int, Dictionary<int, string>>();

            // get the data by section
            Dictionary<int, string> matchRegex = new Dictionary<int, string>();
            matchRegex.Add(1, @"^(\d+\.(?:\d+\.?)*)"); //    (1.), (1.1) and (1.1.1)
            matchRegex.Add(2, @"^((section)\s\d+\.(?:\d+\.?)*)"); //   (section 1.), (section 1.1) and (section 1.1.1)
            matchRegex.Add(3, @"^((section)\s\d+(.\d+)+(\:\d+))"); //    (section 1.1:1)
            matchRegex.Add(4, @"^(\d+(.\d+)+(\:\d+))"); //   (1.1:1)
            matchRegex.Add(5, @"^((section)\s\d+(\:\d+))"); //   (section 1:1)
            matchRegex.Add(6, @"^(\d+(\:\d+))"); //  (1:1)

            using (Doc doc = new Doc())
            {
                doc.Read(filepath);
                short PageIndex = 1;
                var i = 1;
                var saveSection = 0;
                while (PageIndex <= doc.PageCount) // save the value in dictionary 
                {
                    var firstPara = 1;
                    TextOperation op = new TextOperation(doc);
                    op.PageContents.AddPages(PageIndex);
                    string theText = op.GetText();

                    IList<TextGroup> theGroups = op.Group(op.Select(0, theText.Length));
                    List<TextGroup> ordereddGroups = theGroups.OrderByDescending(o => o.Rect.Top).ToList();

                    Dictionary<int, string> saveLines = new Dictionary<int, string>();
                    StringBuilder sb1 = new StringBuilder();
                    XRect prevRect = new XRect("0 0 0 0");
                    var totalRows = 0;
                    foreach (TextGroup lineGroup in ordereddGroups)
                    {
                        totalRows++;
                        if ((prevRect.Bottom - lineGroup.Rect.Top) > 9 && totalRows == ordereddGroups.Count)// current line is > 9 points lower than previous
                        {
                            foreach (var check in matchRegex)
                            {
                                String AllowedChars = check.Value;
                                Regex regex = new Regex(AllowedChars);
                                var match = regex.Match(sb1.ToString());
                                if (match.Success) {
                                    saveSection = 1;
                                    break;
                                }   
                            }
                            if (saveSection == 0 && i != 1) {
                                if (firstPara == 1)
                                {
                                    var lastValues = savePage.Values.Last();
                                    var lastKeys = savePage.Keys.Last();

                                    var updateSectionVal = lastValues.Values.Last() + " " + sb1.ToString();
                                    (savePage[lastKeys])[lastValues.Count] = updateSectionVal;
                                }
                                else {
                                    var lastValueSaveLines = saveLines.Values.Last();
                                    var lastKeySaveLines = saveLines.Keys.Last();

                                    var updatesaveLines = lastValueSaveLines + " " + sb1.ToString();
                                    saveLines[lastKeySaveLines] = updatesaveLines;
                                }
                                sb1.Clear();
                            }

                            if (saveSection == 1) {
                                saveSection = 0;
                                firstPara = 0;
                                saveLines.Add(i, sb1.ToString());
                                i++;
                                sb1.Clear();
                            }
                        }
                        sb1.Append(lineGroup.Text);
                        prevRect.String = lineGroup.Rect.String;
                    }
                    saveLines.Add(i, sb1.ToString());
                    i++;
                    savePage.Add(PageIndex, saveLines);
                    PageIndex++;
                }

            }
        }
        

        //-----------------------------------------FOUND TEXT-----------------------------------------------------------------------------------
        // get all the searched data 
        public void getAllFoundText(int SearchWithin, int resultSection, Dictionary<int, Dictionary<int, string>> savePage, string fileName, JToken logic , out JArray ja , out int  totalScoreDenominatorVal, out Dictionary<string,int> searchFieldScore) {

            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            ja = new JArray();
            var gotResult = 0; // not got
            for (var allLogic =0; allLogic< logic.Count(); allLogic++)
            {
                var getSearchFor = logic[allLogic]["searchFor"];
                var getWithIn = logic[allLogic]["withIn"];
                var getSubCase = logic[allLogic]["subCase"];
                //var getBegin = logic[allLogic]["begin"];

                if (gotResult == 0)// all condition under 'or' 
                {
                    getTotalScore(getWithIn, getSearchFor, out totalScoreDenominatorVal, out searchFieldScore);

                    for (var k = 0; k < getSearchFor.Count(); k++) // loop throuch all searchFor
                    {
                        var AllSearchFieldKeyword = (getSearchFor[k]["keyword"]).ToString(); // get the search field
                        //var AllSearchFieldOp = (getSearchFor[k]["op"]).ToString().ToLower(); // get the search field op
                        var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field op
                        //var AllSearchFieldBegin = (getSearchFor[k]["beginWith"]).ToString().ToLower(); // get the search field op

                        bool checkAfterSubCaseSearchFor = true;
                        var pageCount = 0;
                        foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
                        {
                            pageCount += 1;
                            var searchFieldFound = new List<int>();
                            // saves the lineText
                            StringBuilder sb3 = new StringBuilder();
                            var paraNumber = 0;
                            foreach (var checkPage in entry.Value) // each page value
                            {
                                paraNumber += 1;
                                var getLineText = checkPage.Value; // get the  line text
                                var matchData = Regex.Matches(getLineText, @"\b\s?" + AllSearchFieldKeyword + "\\w*\\b"); // find match
                                if (matchData.Count > 0) // if match there
                                {

                                    // check for cases
                                    if (AllSearchFieldCaseCheck == "yes") 
                                        subCaseSearch(getLineText, AllSearchFieldKeyword, getSubCase, out checkAfterSubCaseSearchFor);
                                    else
                                        checkAfterSubCaseSearchFor = true;

                                    if (checkAfterSubCaseSearchFor == true) {

                                        //if (AllSearchFieldBegin == "yes")
                                        //    beginSearch(getLineText, AllSearchFieldKeyword, getSubCase, out checkAfterSubCaseSearchFor);
                                        //else
                                        //    checkAfterSubCaseSearchFor = true;


                                        string foundTextFinal = "";
                                        var SearchWithinText = "";

                                        if (SearchWithin == 1) // page
                                        {
                                            foreach (var getAllPara in entry.Value)
                                            {
                                                sb3.Append(getAllPara.Value); // get all lines in sb3 of single page
                                                sb3.AppendLine();
                                            }
                                            SearchWithinText = sb3.ToString();
                                        }

                                        if (SearchWithin == 2) // section  (current same as paragraph)
                                            SearchWithinText = getLineText;

                                        if (SearchWithin == 3) // paragraph
                                            SearchWithinText = getLineText;


                                        if (resultSection == 1) // sentence
                                            foundTextSection(AllSearchFieldKeyword, getLineText, out foundTextFinal); // get the accepted result

                                        if (resultSection == 2) // paragraph
                                            foundTextFinal = getLineText;

                                        if (resultSection == 3) // section (current same as paragraph)
                                            foundTextFinal = getLineText;


                                        // check if withIn values are there 
                                        if (getWithIn.Count() > 0)
                                        {
                                            for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                            {
                                                bool checkAfterSubCaseWithIn = true;
                                                var withInIt = (getWithIn[g]["keyword"]).ToString();
                                                //var withInItOp = (getWithIn[g]["op"]).ToString().ToLower();
                                                var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                                                var  matchDataWithInIt = Regex.Matches(SearchWithinText, @"\b\s?" + withInIt + "\\w*\\b");
                                                if (withInIt == "$")
                                                    matchDataWithInIt = Regex.Matches(SearchWithinText, @"([$]+)"); // find match    
                                                if (withInIt == "%")
                                                    matchDataWithInIt = Regex.Matches(SearchWithinText, @"(\d+%)"); // find match

                                                if (matchDataWithInIt.Count > 0) // if match there
                                                {
                                                    if (withInCaseCheck == "yes")// check for cases
                                                        subCaseSearch(SearchWithinText, withInIt, getSubCase, out checkAfterSubCaseWithIn);
                                                    
                                                    if (checkAfterSubCaseWithIn == true)
                                                    {
                                                        gotResult = 1;
                                                        var jo = new JObject();
                                                        jo["foundText"] = foundTextFinal;
                                                        jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
                                                        jo["fileName"] = fileName.Split('.')[0];
                                                        jo["pageNo"] = pageCount;
                                                        jo["pageContent"] = SearchWithinText;
                                                        jo["paraNumber"] = paraNumber;
                                                        ja.Add(jo);
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        else // if not withIn to search
                                        {
                                            gotResult = 1;
                                            var jo = new JObject();
                                            jo["foundText"] = foundTextFinal;
                                            jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
                                            jo["fileName"] = fileName.Split('.')[0];
                                            jo["pageNo"] = pageCount;
                                            jo["pageContent"] = SearchWithinText;
                                            jo["paraNumber"] = paraNumber;
                                            ja.Add(jo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // condition for and 
                //if (gotResult == 1 && condition == "and") {
                //    if (allLogic + 1 > allLogic)
                //        break;
                //    var getNextSearchFor = logic[allLogic+1]["searchFor"];
                //    var getNextWithIn = logic[allLogic+1]["withIn"];
                //    var ja3 = new JArray();
                //    //for (int i = 0; i < ja.Count; i++) // get all the values to display on front end
                //    //    ja3.Add(ja[i]);
                //    var checkForAnd = JArray.Parse(ja.ToString()); // get all the accepted result
                //    for (var l = 0; l < checkForAnd.Count(); l++)
                //    {
                //        for (var k = 0; k < getNextSearchFor.Count(); k++) // loop throuch all searchFor
                //        {
                //            var AllSearchFieldKeyword = (getNextSearchFor[k]["keyword"]).ToString(); // get the search field
                //            var AllSearchFieldOp = (getNextSearchFor[k]["op"]).ToString().ToLower(); // get the search field op
                //            var pageContent = checkForAnd[l]["pageContent"].ToString();
                //            var matchDataWithInIt = Regex.Matches(pageContent, @"\b\s?" + AllSearchFieldKeyword + "\\w*\\b"); // find match
                //            if (matchDataWithInIt.Count > 0) // if match there
                //            {
                //                for (var g = 0; g < getNextWithIn.Count(); g++) {
                //                    var withInIt = (getNextWithIn[g]["keyword"]).ToString();
                //                    var matchDataWithInItAndCondition = Regex.Matches(pageContent, @"\b\s?" + withInIt + "\\w*\\b"); // find match
                //                    if (matchDataWithInItAndCondition.Count > 0) // if match there
                //                    {
                //                        gotResult = 0;
                //                        var jo = new JObject();
                //                        jo["foundText"] = checkForAnd[l]["foundText"].ToString();
                //                        jo["AllSearchFieldKeyword"] = checkForAnd[l]["AllSearchFieldKeyword"].ToString();
                //                        jo["fileName"] = checkForAnd[l]["fileName"].ToString();
                //                        jo["pageNo"] = checkForAnd[l]["pageNo"].ToString();
                //                        jo["pageContent"] = pageContent;
                //                        jo["paraNumber"] = checkForAnd[l]["paraNumber"].ToString();
                //                        ja3.Add(jo);
                //                        break;
                //                    }
                //                }   
                //            }
                //        }
                //    }
                //    if (ja3.Count() > 0) {
                //        ja.Clear();
                //        for (int i = 0; i < ja3.Count; i++) // get all the values to display on front end
                //            ja.Add(ja3[i]);
                //    }
                //}
            }
            
        }


        // get the section
        public void foundTextSection(string AllSearchFieldKeyword, string getLineText, out string foundTextFinal) {
            foundTextFinal = "";
            var textLength = getLineText.Length;
            var AllSearchFieldKeywordLength = AllSearchFieldKeyword.Length;
            StringBuilder sb = new StringBuilder();
            var foundtextBefore = "";
            var foundtextAfter = "";
            var startIndex = getLineText.IndexOf(AllSearchFieldKeyword); // get the index of search field
            var getTheTextBeforeWord = getLineText.Substring(0, startIndex); // get the string before word to find the full stop
            var getTheFullStopBeforeWord = getTheTextBeforeWord.LastIndexOf('.'); // get the index of the full stop
            if (getTheFullStopBeforeWord == -1)
                foundtextBefore = getTheTextBeforeWord;
            else
                foundtextBefore = getLineText.Substring(getTheFullStopBeforeWord, startIndex- getTheFullStopBeforeWord);
            var getTheTextAfterWord = getLineText.Substring(startIndex, textLength - startIndex); // get the string before word to find the full stop
            var getTheFullStopAfterWord = getTheTextAfterWord.IndexOf('.'); // get the index of the full stop
            if (getTheFullStopAfterWord == -1)
                foundtextAfter = getTheTextAfterWord;
            else
                foundtextAfter = getLineText.Substring(startIndex, getTheFullStopAfterWord + 1);

            foundTextFinal = foundtextBefore + foundtextAfter; // final foundtext
        }


        // check sub case
        public void subCaseSearch(string lineToCheck, string checkForVal, JToken getSubCase, out bool checkFurther) {

            checkFurther = false;
            for (var i=0;i< getSubCase.Count(); i++) {
                if (checkForVal == (getSubCase[i]["checkFor"]).ToString()) {
                    var keyword = (getSubCase[i]["keyword"]).ToString();
                    var matchDataWithInIt = Regex.Matches(lineToCheck, @"\b\s?" + keyword + "\\w*\\b"); // find match
                    if (matchDataWithInIt.Count > 0) // if match there
                    {
                        checkFurther = true;
                        break;
                    }
                        
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        // scoring and output
        public void scoring(string LeaseName, Dictionary<int, Dictionary<int, string>> savePage,  string resultFormat, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja, out JArray ja1 ,out int accptedValThere , out float finalScore) {

            ja1 = new JArray();
            finalScore = 0;
            accptedValThere = 0;
            var getAllAcceptedText = JArray.Parse(ja.ToString()); // get all the accepted result
            Dictionary<int, float> scoreVal = new Dictionary<int, float>();
            for (var l = 0; l < getAllAcceptedText.Count(); l++)
            {
                var pageContent = getAllAcceptedText[l]["pageContent"].ToString(); // get the page value to search all search fileds
                var scorePerSearch = 0;
                double finalScorePerSearch = 0;

                foreach (KeyValuePair<string, int> singleSearchFieldScore in searchFieldScore)
                { //  loop through all the search field
                    var matchDataWithInIt = Regex.Matches(pageContent, @"\b\s?" + singleSearchFieldScore.Key + "\\w*\\b"); // find match
                    if (matchDataWithInIt.Count > 0)
                    {

                        scorePerSearch += (singleSearchFieldScore.Value * matchDataWithInIt.Count); // increment the score
                    }
                }
                finalScorePerSearch = ((double)scorePerSearch / (double)totalScoreDenominatorVal) * 100; // get the percentage
                scoreVal.Add(l, (float)finalScorePerSearch); // save that in deictionary
            }
            if (scoreVal.Count() != 0)
            { // if dictionary has val
                var accurateVal = scoreVal.OrderByDescending(x => x.Value); // sort the dictionary to get the highest val
                var accurateValKey = accurateVal.First().Key; // key of highest value
                var accurateValValue = accurateVal.First().Value; // value of highest value
                foreach (KeyValuePair<int, float> entry in accurateVal)
                { // loop to get all the highest value
                    // save the output for the file
                    if (entry.Value == accurateValValue) { // get all the highest score value
                        var foundText = getAllAcceptedText[entry.Key]["foundText"].ToString();
                        var AllSearchFieldKeyword1 = getAllAcceptedText[entry.Key]["AllSearchFieldKeyword"];
                        var paraNumber = (int)getAllAcceptedText[entry.Key]["paraNumber"];
                        var pageNo = (int)getAllAcceptedText[entry.Key]["pageNo"];
                        var fileNameVal = getAllAcceptedText[entry.Key]["fileName"];
                        finalScore = entry.Value;
                        Dictionary<int, string>.ValueCollection entry1 = savePage[pageNo].Values;
                        var getTheSectionValue = processing.SectionVal(savePage, AllSearchFieldKeyword1.ToString(), pageNo, paraNumber); // get the section value
                        if (getTheSectionValue == "false")
                            getTheSectionValue = "?";

                            var output = resultFormat.Replace("{{Document Name}}", fileNameVal.ToString()).Replace("{{Paragraph Number}}", getTheSectionValue).Replace("{{result}}", foundText).Replace("{{found text}}", AllSearchFieldKeyword1.ToString());
                            var jo1 = new JObject();
                            jo1["output"] = output;
                            jo1["AllSearchFieldKeyword"] = AllSearchFieldKeyword1;
                            jo1["fileName"] = fileNameVal;
                            jo1["pageNo"] = pageNo;
                            jo1["score"] = finalScore;
                            jo1["sectionVal"] = getTheSectionValue;
                            jo1["leaseName"] = LeaseName;
                            ja1.Add(jo1);
                            accptedValThere = 1;
                        
                    }
                }
            }
        }


        // save all data found in 
        public void saveDataToFolder(int resultFound, JArray ja1, string folderPath, string fileName) {

            var saveDataFolder = folderPath + "\\output";
            var outputFilePath = saveDataFolder + "\\result.txt";

            if (!Directory.Exists(saveDataFolder))//if output folder not exist
                Directory.CreateDirectory(saveDataFolder);//create folder
            else if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            var pathString = System.IO.Path.Combine(saveDataFolder, "result.txt"); // create the input file

            if (resultFound == 1) { // result found
                var dataMain = new JArray();
                for (var i = 0; i < ja1.Count; i++)
                {
                    var data = new JObject();
                    data["fileName"] = ja1[i]["fileName"].ToString();
                    data["searchField"] = ja1[i]["AllSearchFieldKeyword"].ToString();
                    data["pageNoVal"] = ja1[i]["pageNo"].ToString();
                    data["sectionVal"] = ja1[i]["sectionVal"].ToString();
                    dataMain.Add(data);
                }
                File.WriteAllText(pathString, dataMain.ToString());
            }
            else // if lease is silent
                File.WriteAllText(pathString, "Lease is Silent");

        }
    }
}