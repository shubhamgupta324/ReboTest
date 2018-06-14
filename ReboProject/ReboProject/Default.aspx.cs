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
            var datapoint = backendObject["Datapoint"].ToString(); // Datapoint 
            var multipleRead = (int)backendObject["MultipleRead"];  // to get score of multiple occurance
            var SearchWithin = (int)backendObject["SearchWithin"];// 1=page, 2=section, 3=paragraph
            var resultSection = (int)backendObject["result"][0]["section"];// page or sentance or paragraph
            var resultFormat = backendObject["result"][0]["format"].ToString();// eg: {{filename}}; {{result}}: {{pagenumber}}
            var logic = backendObject["logic"]; // all searchFor and there respected withIn
            var set2 = backendObject["Set2"]; // all searchFor and there respected withIn

            var libraryVal = ""; // get all the library value
            if ((LibVal.Text) != "")
                libraryVal = LibVal.Text;
                
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
                var readNextFile = 1;
                Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                bool condition1 = true;
                // -------------------------------------loop through all files----------------------------------------------------
                foreach (string fullFilePath in fileDetails)
                {
                    if (readNextFile == 1) {
                        var ja = new JArray();
                        var getTheIndividualFileName = fullFilePath.Split('\\');
                        fileName = getTheIndividualFileName[getTheIndividualFileName.Length - 1]; // get the file name
                        int index = fileName.LastIndexOf(".");
                        fileName = fileName.Substring(0, index);
                        //read the file
                        

                        pdfRead(fullFilePath, out savePage); // read pdf
                                                             //pdfReadSection(fullFilePath, out savePage);
                                                             //pdfReadSectionCase2(fullFilePath, out savePage);

                        getAllFoundText(SearchWithin, resultSection, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore); //  get the found text

                        

                        //--------------------scoring and final output ---------------------------------------------------------------------
                        scoring(LeaseName, savePage, resultFormat, totalScoreDenominatorVal, searchFieldScore, ja, condition1, multipleRead, out ja1, out accptedValThere, out finalScore);


                        for (int i = 0; i < ja1.Count; i++)// get only one highest score value
                        {
                            if(type == 1)
                                readNextFile = 0;
                            if (TopfinalScore < finalScore)
                            {
                                TopfinalScore = finalScore;
                                ja2.RemoveAll();
                                ja2.Add(ja1[i]); 
                            }
                        }
                    }
                   
                }
                
                // logic 2
                if (set2.HasValues)
                    checkLogic(libraryVal, ja1, resultFormat, condition1, set2, datapoint, ja2, SearchWithin, resultSection, savePage, fileName, LeaseName, out ja2);
                    

                //---------------------save the result in folder----------------------------------------------
                if (ja2.Count == 0)// check if any result found
                    resultFound = 0;
                saveDataToFolder(resultFound, ja2, folderPath, fileName);
                for (int i = 0; i < ja2.Count; i++) // get all the values to display on front end
                    ja3.Add(ja2[i]);
                if (ja2.Count == 0)
                {
                    var ja4 = new JArray();
                    var jo4 = new JObject();
                    jo4["output"] = "Lease is silent";
                    jo4["AllSearchFieldKeyword"] = "";
                    jo4["fileName"] = "";
                    jo4["pageNo"] = 0;
                    jo4["score"] = 0;
                    jo4["sectionVal"] = "";
                    jo4["leaseName"] = LeaseName;
                    ja4.Add(jo4);
                    ja3.Add(ja4[0]);
                }
                

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
                            foreach (var check in matchRegex) // get para section number wise..
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

                if (gotResult == 0)// all condition under 'or' 
                {
                    getTotalScore(getWithIn, getSearchFor, out totalScoreDenominatorVal, out searchFieldScore);

                    for (var k = 0; k < getSearchFor.Count(); k++) // loop throuch all searchFor
                    {
                        var AllSearchFieldKeyword = (getSearchFor[k]["keyword"]).ToString(); // get the search field
                        var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field op

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
                                            var foundWithIn = "";
                                            for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                            {
                                                bool checkAfterSubCaseWithIn = true;
                                                var withInIt = (getWithIn[g]["keyword"]).ToString();
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
                                                        foundWithIn = (foundWithIn == "") ? withInIt : ","+ withInIt;
                                                    }
                                                }
                                            }
                                            if (foundWithIn != "") {
                                                var jo = new JObject();
                                                jo["foundText"] = foundTextFinal;
                                                jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
                                                jo["fileName"] = fileName.Split('.')[0];
                                                jo["pageNo"] = pageCount;
                                                jo["pageContent"] = SearchWithinText;
                                                jo["foundWithIn"] = foundWithIn;
                                                jo["paraNumber"] = paraNumber;
                                                ja.Add(jo);
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
                                            jo["foundWithIn"] = "";
                                            jo["paraNumber"] = paraNumber;
                                            ja.Add(jo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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
        public void scoring(string LeaseName, Dictionary<int, Dictionary<int, string>> savePage,  string resultFormat, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja,bool condition1,int multipleRead, out JArray ja1 ,out int accptedValThere , out float finalScore) {

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
                        if (multipleRead == 0)
                            scorePerSearch += singleSearchFieldScore.Value; // increment the score
                        else
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
                        var getTheSectionValue = processing.SectionVal(savePage, pageNo, paraNumber); // get the section value
                        if (getTheSectionValue == "false")
                            getTheSectionValue = "?";

                            var output = resultFormat.Replace("{{Document Name}}", fileNameVal.ToString()).Replace("{{Paragraph Number}}", getTheSectionValue).Replace("{{result}}", foundText).Replace("{{found text}}", AllSearchFieldKeyword1.ToString());
                            var jo1 = new JObject();
                            jo1["output"] = output;
                            jo1["Pageoutput"] = foundText;
                            jo1["AllSearchFieldKeyword"] = AllSearchFieldKeyword1;
                            jo1["fileName"] = fileNameVal;
                            jo1["pageNo"] = pageNo;
                            jo1["score"] = finalScore;
                            jo1["sectionVal"] = getTheSectionValue;
                            jo1["leaseName"] = LeaseName;
                            ja1.Add(jo1);
                            accptedValThere = 1;

                        if (condition1 == false)
                            return;
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


        // check library step2
        public void checklibrary(JArray ja2, string[] librarySet, string datapoint, out bool outputFound) {
            
            outputFound = false;
            //var getTheLibrary = librarySet[datapoint.ToLower()]; //  get all library for datapoint
            for (var i=0; i<ja2.Count();i++) {
                var outputConfig1 =ja2[i]["Pageoutput"].ToString(); // get  the para

                for (var singleSF = 0; singleSF < librarySet.Count(); singleSF++) {
                    var matchData = Regex.Matches(outputConfig1, @"\b\s?" + librarySet[singleSF] + "\\w*\\b"); // search if library in para
                    if (matchData.Count > 0) {
                        outputFound = true; // library found 
                        break;
                    }
                }
            }
        }

        // check the library in whole pdf
        public void searchLibInPDF(Dictionary<int, Dictionary<int, string>> savePage, string[] librarySet , string datapoint , out List<string>  pdfLibPara, out List<int> pdfLibPageno, out List<int> pdfLibParaNo)
        {
            pdfLibPara = new List<string>();
            pdfLibParaNo = new List<int>();
            pdfLibPageno = new List<int>();
            var pageCount = 0;
            try {
                foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage)
                {
                    pageCount += 1;
                    foreach (var checkPage in entry.Value)
                    {
                        var para = checkPage.Value;
                        var paraNo = checkPage.Key;
                        //var librarySetVal = librarySet[datapoint.ToLower()];
                        for (var i = 0; i < librarySet.Count(); i++)
                        {
                            var singleLibVAl = librarySet[i];
                            var matchData = Regex.Matches(para, @"\b\s?" + singleLibVAl + "\\w*\\b"); // search if library in para
                            if (matchData.Count > 0)
                            {
                                pdfLibPara.Add(para);
                                pdfLibParaNo.Add(paraNo);
                                pdfLibPageno.Add(pageCount);
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            

        }

        public void checkLogic(string libraryVal, JArray ja1, string resultFormat, bool condition1, JToken set2,string datapoint,JArray ja2, int SearchWithin, int resultSection, Dictionary<int, Dictionary<int, string>> savePage, string fileName,string LeaseName, out JArray ja5)
        {
            ja5 = new JArray();
            var SearchWithinlogic2 = (int)set2[0]["SearchWithin"];
            var sortlogic2 = (int)set2[0]["FileOrder"]["sort"]; // asc or desc
            var typelogic2 = (int)set2[0]["FileOrder"]["type"]; // Single File Search or All File Search
            var multipleReadlogic2 = (int)set2[0]["MultipleRead"];
            var logic2 = set2[0]["logic"];

            if (logic2.HasValues)
            {
                string[] LibArr = null;
                // check for second condition 
                var jaOutput = new JArray();
                var outputFound = false;
                List<string> pdfLibPara = new List<string>();
                LibArr = libraryVal.Split('|');
                var totalScoreDenominatorValLogic2 = 0;
                Dictionary<string, int> searchFieldScoreLogic2 = new Dictionary<string, int>();
                checklibrary(ja2, LibArr, datapoint, out outputFound); // check if output of first logic has library value
                condition1 = false;
                var accptedValThere = 0;
                float finalScore = 0;
                if (ja2.HasValues)
                {
                    if (outputFound == true)// check if output of first logic has library value
                    { // run the second configuration 
                        getAllFoundText(SearchWithin, resultSection, savePage, fileName, logic2, out jaOutput, out totalScoreDenominatorValLogic2, out searchFieldScoreLogic2); //  get the found text
                        scoring(LeaseName, savePage, resultFormat, totalScoreDenominatorValLogic2, searchFieldScoreLogic2, jaOutput, condition1, multipleReadlogic2, out ja1, out accptedValThere, out finalScore);
                        if (ja1.HasValues)
                        {
                            ja2[0]["output"] = ja2[0]["output"].ToString() + ja1[0]["output"].ToString();
                            ja2[0]["pageNo"] = ja2[0]["pageNo"].ToString() + "," + ja1[0]["pageNo"].ToString();
                        }
                    }
                    else
                    { // search the library in pdf and run the second condition
                        List<int> pdfLibPageno = new List<int>();
                        List<int> pdfLibParaNo = new List<int>();
                        searchLibInPDF(savePage, LibArr, datapoint, out pdfLibPara, out pdfLibPageno, out pdfLibParaNo);
                        if (pdfLibPara.Count() != 0)
                        {
                            getAllFoundText(SearchWithin, resultSection, savePage, fileName, logic2, out jaOutput, out totalScoreDenominatorValLogic2, out searchFieldScoreLogic2); //  get the found text
                            scoring(LeaseName, savePage, resultFormat, totalScoreDenominatorValLogic2, searchFieldScoreLogic2, jaOutput, condition1, multipleReadlogic2, out ja1, out accptedValThere, out finalScore);
                            if (ja1.Count != 0)
                            {
                                ja2[0]["output"] = ja2[0]["output"].ToString() + System.Environment.NewLine + ja1[0]["output"].ToString();
                                ja2[0]["pageNo"] = ja2[0]["pageNo"].ToString() + "," + ja1[0]["pageNo"].ToString();
                                if (ja2[0]["pageNo"].ToString().EndsWith("."))
                                    ja2[0]["Pageoutput"] = ja2[0]["Pageoutput"].ToString() + ja1[0]["Pageoutput"].ToString();
                                else
                                    ja2[0]["Pageoutput"] = ja2[0]["Pageoutput"].ToString() + "." + ja1[0]["Pageoutput"].ToString();

                            }
                            else
                            {
                                for (var i = 0; i < pdfLibPara.Count(); i++)
                                {
                                    var getTheSectionValue = processing.SectionVal(savePage, pdfLibPageno[i], pdfLibParaNo[i]); // get the section value
                                    if (getTheSectionValue == "false")
                                        getTheSectionValue = "?";
                                    var output = resultFormat.Replace("{{Document Name}}", "").Replace("{{Paragraph Number}}", getTheSectionValue).Replace("{{result}}", pdfLibPara[i]).Replace("{{found text}}", "....");
                                    ja2[0]["output"] = ja2[0]["output"].ToString() + System.Environment.NewLine + output;
                                    ja2[0]["pageNo"] = ja2[0]["pageNo"].ToString() + "," + pdfLibPageno[i];
                                }
                            }
                        }
                    }
                    for (int i = 0; i < ja2.Count; i++) // get all the values to display on front end
                        ja5.Add(ja2[i]);
                }
            }
        }
    }
}