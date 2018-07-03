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
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            string backEndVal = backEndData.Text; // get the value from front end
            if (backEndVal == "") // check if it has value else return
                return;
            // -------------get the common data----------------
            var backendObject = JObject.Parse(backEndVal.ToString()); // complete json 
            var folder = backendObject["folder"].ToString();// folder path
            var resultSection = (int)backendObject["result"][0]["section"];// page or sentance or paragraph
            var resultFormat = backendObject["result"][0]["format"].ToString();// eg: {{filename}}; {{result}}: {{pagenumber}}
            var libraryVal = ""; // get all the library value
            if ((LibVal.Text) != "")
                libraryVal = LibVal.Text;
            string[] LibArr = libraryVal.Split('|');
            var configuration = backendObject["Configuration"]; // get all the configuration
            var configurationOrder = new Dictionary<int, int>();
            for (var i = 0; i < configuration.Count(); i++)
            {
                configurationOrder.Add(i + 1, (int)configuration[i]["Index"]);
            }
            var list = configurationOrder.Values.ToList();
            list.Sort();
            //--------------------------------------------------

            //-----------------final output data----------------
            var accptedValThere = 0; // check if lease is silent or not
            var finalOutput = new JArray();
            //--------------------------------------------------

            // -----------------get all lease------------------------
            var folderPath = "";
            string drivePath = WebConfigurationManager.AppSettings["DrivePath"]; // get the access to d 
            var userDefinePath = drivePath + folder;
            string[] subdirectoryEntries = Directory.GetDirectories(userDefinePath);
            //-------------------------------------------------------

            // -----------------loop through all lease----------------
            string notAbstractedLease = "";
            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++)
            {
                var ja3 = new JArray(); // get the final output of one configuration
                var jaLibCheck = new JArray(); // get the final output of one configuration    
                                                // get only the pdf files from each lease
                folderPath = subdirectoryEntries[folderval]; // get the folder from where to get pdf
                var fornotAbstractedLease = folderPath.Split('\\');
                var fornotAbstractedLeaseName=fornotAbstractedLease[fornotAbstractedLease.Length - 1];
                string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf").Select(Path.GetFileName).ToArray(); // get all the pdf file in that folder
                var LeaseName = "";
                Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                Dictionary<int, Dictionary<int, string>> savePageLib = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                Dictionary<int, string> OutputMatch = new Dictionary<int, string>();
                string getCorrectSentances = "";
                    for (var configurationVal = 0; configurationVal < configuration.Count(); configurationVal++)
                    {
                        var gotValueForConfiguration = false;
                        var myKey = configurationOrder.FirstOrDefault(x => x.Value == list[configurationVal]).Key;
                        var configurationIndexSelect = configurationOrder[myKey];
                        var connectorVal = (int)configuration[myKey - 1]["Connector"];// get the connector output to check Mandatory or library check
                        var datapoint = configuration[myKey - 1]["Datapoint"].ToString(); // Datapoint 
                        var exclusionCount = 0;
                        if (configuration[myKey - 1]["ExclusionCount"].ToString() != "")
                            exclusionCount = (int)configuration[myKey - 1]["ExclusionCount"]; // all searchFor and there respected withIn
                        var logic = configuration[myKey - 1]["logic"]; // all searchFor and there respected withIn
                        JArray getExclusion = null;
                        foreach (var getTheExclusion in logic)
                        {
                            getExclusion = (JArray)getTheExclusion["exclusion"];
                        }
                        bool executeConfiguration = true;
                        List<int> pdfLibPageno = new List<int>();
                        List<int> pdfLibParaNo = new List<int>();
                        List<string> pdfLibPara = new List<string>();
                        List<string> pdfLibValFound = new List<string>();
                        var outputFound = false;
                        if (connectorVal == 2)
                        { // check if the last output contain the library value
                            if (LibArr[0] != "")
                            {
                                checklibrary(jaLibCheck, LibArr, out outputFound); // check if output of first logic has library value
                                if (outputFound == false)
                                { // check the whole pdf for library 
                                    if (!jaLibCheck.HasValues)
                                        break;
                                    pdfRead(jaLibCheck[0]["completeFilePath"].ToString(), out savePage); // read pdf
                                    searchLibInPDF(getExclusion, exclusionCount, savePage, LibArr, datapoint, out pdfLibPara, out pdfLibPageno, out pdfLibParaNo, out pdfLibValFound);
                                    savePageLib = savePage;
                                    executeConfiguration = pdfLibPara.Count() != 0 ? executeConfiguration = true : executeConfiguration = false; // if found get the second para else skip
                                }
                            }
                            else
                                executeConfiguration = false;
                        }

                        if (executeConfiguration == true)
                        { // check if to execute configurationDatapoint
                            var sort = (int)configuration[myKey - 1]["FileOrder"]["sort"]; // asc or desc
                            var type = (int)configuration[myKey - 1]["FileOrder"]["type"]; // Single File Search or All File Search
                            var datapointID = (int)configuration[myKey - 1]["DpId"]; // Datapoint ID
                            var datapointName = configuration[myKey - 1]["Datapoint"].ToString(); // Datapoint ID
                            var multipleRead = (int)configuration[myKey - 1]["MultipleRead"];  // to get score of multiple occurance
                            var SearchWithin = (int)configuration[myKey - 1]["SearchWithin"];// 1=page, 2=section, 3=paragraph
                            var mainLeaseRead = (int)configuration[myKey - 1]["MainLeaseRead"];// skip main lease read


                            Dictionary<string, int> searchFieldScore = new Dictionary<string, int>(); // saves the foundtext
                            Dictionary<string, int> withInScore = new Dictionary<string, int>(); // saves the foundtext
                            var totalScoreDenominatorVal = 0; // get the total score of all the search field
                            var ja1 = new JArray();
                            var ja2 = new JArray();

                            // -----------get the file order--------------
                            ArrayList fileDetails = new ArrayList();
                            fileToRead(mainLeaseRead, folderPath, sort, type, pdfFiles, out fileDetails);
                            var FilePathPlusLeaseName = folderPath.Split('\\');
                            //---------------------------------------------

                            LeaseName = FilePathPlusLeaseName[FilePathPlusLeaseName.Length - 1]; // get the lease name
                            var fileName = "";
                            float finalScore = 0;
                            var resultFound = 1;
                            float TopfinalScore = 0;
                            var readNextFile = 1;

                            //bool condition1 = true;
                            foreach (string fullFilePath in fileDetails)
                            {
                                if (readNextFile == 1)
                                {
                                    var ja = new JArray();
                                    var getTheIndividualFileName = fullFilePath.Split('\\');
                                    fileName = getTheIndividualFileName[getTheIndividualFileName.Length - 1]; // get the file name
                                    int index = fileName.LastIndexOf(".");
                                    fileName = fileName.Substring(0, index);
                                    //read the file

                                    pdfRead(fullFilePath, out savePage); // read pdf

                                    getAllFoundText(exclusionCount, fullFilePath, SearchWithin, resultSection, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text

                                    //--------------------scoring and final output ---------------------------------------------------------------------
                                    scoring(OutputMatch, LeaseName, savePage, resultFormat, totalScoreDenominatorVal, searchFieldScore, ja, multipleRead, out ja1, out accptedValThere, out finalScore);

                                    for (int i = 0; i < ja1.Count; i++)// get only one highest score value
                                    {
                                        if (type == 1)
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

                        // get the correct sentance
                        var singleCorrectSentence = "";
                        collectCorrectSentance(withInScore, ja2, out singleCorrectSentence);
                        if (singleCorrectSentence != "")
                            getCorrectSentances += " <b> (" + (configurationVal + 1) + ")[</b>  " + singleCorrectSentence + " <b>]</b>";


                        if (ja2.Count == 0)// check if any result found
                            resultFound = 0;
                        if (!ja3.HasValues && ja2.HasValues)
                        {
                            ja3.Add(ja2[0]);
                            jaLibCheck.RemoveAll();
                            jaLibCheck.Add(ja3[0]);
                            gotValueForConfiguration = true;
                            saveDataToFolder(ja3, folderPath);
                            if (!OutputMatch.ContainsKey(connectorVal))
                                OutputMatch.Add(connectorVal, ja2[0]["Pageoutput"].ToString());
                        }

                        else
                        {
                            if (ja3.HasValues && ja2.HasValues)
                            {
                                ja3[0]["output"] = ja3[0]["output"].ToString() + System.Environment.NewLine + ja2[0]["output"].ToString();
                                ja3[0]["pageNo"] = ja3[0]["pageNo"].ToString() + " , " + ja2[0]["pageNo"].ToString();
                                ja3[0]["fileName"] = ja3[0]["fileName"].ToString() + " , " + ja2[0]["fileName"].ToString();
                                ja3[0]["score"] = ja3[0]["score"].ToString() + " ||| " + ja2[0]["score"].ToString();
                                ja3[0]["foundWithIn"] = ja3[0]["foundWithIn"].ToString() + " ||| " + ja2[0]["foundWithIn"].ToString();
                                ja3[0]["AllSearchFieldKeyword"] = ja3[0]["AllSearchFieldKeyword"].ToString() + " ||| " + ja2[0]["AllSearchFieldKeyword"].ToString();

                                if (ja3[0]["Pageoutput"].ToString().EndsWith("."))
                                    ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + ja2[0]["Pageoutput"].ToString();
                                else
                                    ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + ". " + ja2[0]["Pageoutput"].ToString();

                                jaLibCheck.RemoveAll();
                                jaLibCheck.Add(ja3[0]);
                                gotValueForConfiguration = true;
                                saveDataToFolder(ja3, folderPath);
                                if (!OutputMatch.ContainsKey(connectorVal))
                                    OutputMatch.Add(connectorVal, ja2[0]["Pageoutput"].ToString());
                            }
                        }
                    }
                    if (connectorVal == 2 && outputFound == false && LibArr[0] != "")
                    {
                        if (pdfLibPara.Count() == 1 && gotValueForConfiguration == false)
                        {
                            var outputSame = false;
                            foreach (var getOutputToCheck in OutputMatch)
                            {
                                if (getOutputToCheck.Value == pdfLibPara[0])
                                {
                                    outputSame = true;
                                    break;
                                }
                            }
                            if (!OutputMatch.ContainsKey(connectorVal) && outputSame == false)
                            {

                                OutputMatch.Add(connectorVal, pdfLibPara[0]);
                                var getTheSectionValue = processing.SectionVal(savePageLib, pdfLibPageno[0], pdfLibParaNo[0]); // get the section value
                                if (getTheSectionValue == "false")
                                    getTheSectionValue = "?";
                                var output = resultFormat.Replace("{{Document Name}}", "").Replace("{{Paragraph Number}}", "<b>" + getTheSectionValue + "</b>").Replace("{{result}}", pdfLibPara[0]).Replace("{{found text}}", "....");
                                ja3[0]["output"] = ja3[0]["output"].ToString() + System.Environment.NewLine + output;
                                ja3[0]["foundWithIn"] = ja3[0]["foundWithIn"].ToString() + " ||| Library";
                                ja3[0]["AllSearchFieldKeyword"] = ja3[0]["AllSearchFieldKeyword"].ToString() + " ||| Library";
                                ja3[0]["pageNo"] = ja3[0]["pageNo"].ToString() + " , " + pdfLibPageno[0];
                                ja3[0]["score"] = ja3[0]["score"].ToString() + " ||| " + "Library";
                                if (ja3[0]["Pageoutput"].ToString().EndsWith("."))
                                    ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + pdfLibPara[0];
                                else
                                    ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + ". " + pdfLibPara[0];
                                saveDataToFolder(ja3, folderPath);

                                if (getCorrectSentances == "")
                                    getCorrectSentances += " <b> (" + (configurationVal + 1) + ")</b>  " + pdfLibPara[0];
                            }
                        }
                    }
                }

                    if (ja3.Count != 0)
                        ja3[0]["correctString"] = getCorrectSentances;

                    if (ja3.Count == 0)
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
                        jo4["foundWithIn"] = "";
                        jo4["correctString"] = "";
                        ja4.Add(jo4);
                        ja3.Add(ja4[0]);
                        saveDataToFolder(ja4, folderPath);
                    }

                    finalOutput.Add(ja3[0]);
                    //---------------------------------------------------------------
                
               
                
                //--------------------loop through all configuration------------
                

            }

            
            frontEndData.Text = finalOutput.ToString(); // set the data in front end
            watch.Stop();
            displayTime.Text = (watch.ElapsedMilliseconds / 60000).ToString() + " mins";
        }

        // get the score and the field to search
        public void getTotalScore(JToken withIn, JToken searchFor, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string,int> withInScore)
        {
            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            withInScore = new Dictionary<string, int>();
            // searchFor
            for (var k = 0; k < searchFor.Count(); k++) // loop throuch all searchFor
            {
                var AllSearchFieldKeyword = (searchFor[k]["keyword"]).ToString(); // get the search field
                var AllSearchFieldScore = (int)(searchFor[k]["score"]); // get the search field score
                if (!searchFieldScore.ContainsKey(AllSearchFieldKeyword))
                {
                    searchFieldScore.Add(AllSearchFieldKeyword, AllSearchFieldScore);
                    totalScoreDenominatorVal += AllSearchFieldScore;
                }
            }

            // withIn
            for (var h = 0; h < withIn.Count(); h++)
            {
                var withInKeyword = (withIn[h]["keyword"]).ToString();
                var withInscore = (int)(withIn[h]["score"]);
                if (!withInScore.ContainsKey(withInKeyword)) // for getting correct sentence
                    withInScore.Add(withInKeyword, withInscore);
                if (!searchFieldScore.ContainsKey(withInKeyword))
                {
                    
                    searchFieldScore.Add(withInKeyword, withInscore);// add the value for score
                    totalScoreDenominatorVal += withInscore;
                }
            }
        }
        
        // read the file and get lines 
        public void fileToRead(int mainLeaseRead, string folderPath, int sort, int type, string[] pdfFiles, out ArrayList fileDetails)
        {
            fileDetails = new ArrayList();
            if (pdfFiles.Length != 0)
            {
                for (var j = 0; j < pdfFiles.Length; j++)
                {
                    var index = sort == 2 ? pdfFiles.Length - (j + 1) : j; // get the file by order
                    var fileNameVal = pdfFiles[index];
                    var lowerFileNameVal = fileNameVal.ToLower();
                    if (mainLeaseRead == 0 && lowerFileNameVal.IndexOf("lease") == 0)
                        continue;

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

        // check library step2
        public void checklibrary(JArray ja2, string[] librarySet, out bool outputFound)
        {
            outputFound = false;
            //var getTheLibrary = librarySet[datapoint.ToLower()]; //  get all library for datapoint
            for (var i = 0; i < ja2.Count(); i++)
            {
                if (!ja2[i].HasValues)
                    break;
                var outputConfig1 = ja2[i]["Pageoutput"].ToString(); // get  the para

                for (var singleSF = 0; singleSF < librarySet.Count(); singleSF++)
                {
                    var matchData = Regex.Matches(outputConfig1, @"\b\s?" + librarySet[singleSF] + "(\\s|\\b)"); // search if library in para
                    if (librarySet[singleSF].IndexOf("\"") == 0)
                    {
                        var searchVal = (librarySet[singleSF]).Replace("\"", "");
                        matchData = Regex.Matches(outputConfig1, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                    }
                    if (matchData.Count > 0)
                    {
                        outputFound = true; // library found 
                        break;
                    }
                }
            }
        }
        
        // check sub case
        public void subCaseSearch(string lineToCheck, string checkForVal, JToken getSubCase, out bool checkFurther)
        {
            checkFurther = false;
            for (var i = 0; i < getSubCase.Count(); i++)
            {
                if (checkForVal == (getSubCase[i]["checkFor"]).ToString())
                {
                    var keyword = (getSubCase[i]["keyword"]).ToString();
                    var matchDataWithInIt = Regex.Matches(lineToCheck, @"\b\s?" + keyword + "(\\s|\\b)"); // find match
                    if (keyword.IndexOf("\"") == 0)
                    {
                        var searchVal = (keyword).Replace("\"", "");
                        matchDataWithInIt = Regex.Matches(lineToCheck, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                    }
                    if (matchDataWithInIt.Count > 0) // if match there
                    {
                        checkFurther = true;
                        break;
                    }
                }
            }
        }
        
        // -----------------get the sentence------------------------
        public void foundTextSentence(string AllSearchFieldKeyword, string getLineText, out string foundTextFinal)
        {
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
                foundtextBefore = getLineText.Substring(getTheFullStopBeforeWord, startIndex - getTheFullStopBeforeWord);
            var getTheTextAfterWord = getLineText.Substring(startIndex, textLength - startIndex); // get the string before word to find the full stop
            var getTheFullStopAfterWord = getTheTextAfterWord.IndexOf('.'); // get the index of the full stop
            if (getTheFullStopAfterWord == -1)
                foundtextAfter = getTheTextAfterWord;
            else
                foundtextAfter = getLineText.Substring(startIndex, getTheFullStopAfterWord + 1);

            foundTextFinal = foundtextBefore + foundtextAfter; // final foundtext
        }
        
        //-----------------------------------------FOUND TEXT-----------------------------------------------------------------------------------
        // get all the searched data 
        public void getAllFoundText(int exclusionCount, string fullFilePath, int SearchWithin, int resultSection, Dictionary<int, Dictionary<int, string>> savePage, string fileName, JToken logic, out JArray ja, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
        {
            withInScore = new Dictionary<string, int>();
            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            ja = new JArray();
            var gotResult = 0; // not got
            for (var allLogic = 0; allLogic < logic.Count(); allLogic++)
            {
                var getSearchFor = logic[allLogic]["searchFor"];
                var getWithIn = logic[allLogic]["withIn"];
                var getSubCase = logic[allLogic]["subCase"];
                var getExclusion = logic[allLogic]["exclusion"];

                if (gotResult == 0)// all condition under 'or' 
                {
                    getTotalScore(getWithIn, getSearchFor, out totalScoreDenominatorVal, out searchFieldScore,out withInScore);

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
                                var matchData = Regex.Matches(getLineText, @"\b\s?" + AllSearchFieldKeyword + "(\\s|\\b)"); // find match
                                if ((AllSearchFieldKeyword).IndexOf("\"") == 0)
                                {
                                    var searchVal = (AllSearchFieldKeyword).Replace("\"", "");
                                    matchData = Regex.Matches(getLineText, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                                }
                                if (matchData.Count > 0) // if match there
                                {

                                    // check for cases
                                    if (AllSearchFieldCaseCheck == "yes")
                                        subCaseSearch(getLineText, AllSearchFieldKeyword, getSubCase, out checkAfterSubCaseSearchFor);
                                    else
                                        checkAfterSubCaseSearchFor = true;

                                    if (checkAfterSubCaseSearchFor == true)
                                    {

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
                                            foundTextSentence(AllSearchFieldKeyword, getLineText, out foundTextFinal); // get the accepted result

                                        if (resultSection == 2) // paragraph
                                            foundTextFinal = getLineText;

                                        if (resultSection == 3) // section (current same as paragraph)
                                            foundTextFinal = getLineText;
                                        
                                        // check if withIn values are there 
                                        if (getWithIn.Count() > 0)
                                        {
                                            var acceptParaWithIn = "";
                                            var checkExclusion = true;
                                            var countWithInInAPara = 0;
                                            for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                            {
                                                bool checkAfterSubCaseWithIn = true;
                                                bool checkAfterSubCaseWithInExclusion = true;
                                                var withInIt = (getWithIn[g]["keyword"]).ToString();
                                                var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                                                var matchDataWithInIt = Regex.Matches(SearchWithinText, @"\b\s?" + withInIt + "(\\s|\\b)");
                                                if ((withInIt).IndexOf("\"") == 0)
                                                {
                                                    var searchVal = (withInIt).Replace("\"", "");
                                                    matchDataWithInIt = Regex.Matches(SearchWithinText, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                                                }
                                                if (withInIt == "$")
                                                    matchDataWithInIt = Regex.Matches(SearchWithinText, @"([$]+)"); // find match    
                                                if (withInIt == "%")
                                                    matchDataWithInIt = Regex.Matches(SearchWithinText, @"(%)"); // find match

                                                if (matchDataWithInIt.Count > 0) // if match there
                                                {
                                                    if (withInCaseCheck == "yes")// check for cases
                                                        subCaseSearch(SearchWithinText, withInIt, getSubCase, out checkAfterSubCaseWithIn);
                                                    if (checkAfterSubCaseWithIn == true) {

                                                        if (getExclusion.Count() > 0 && checkExclusion == true)
                                                            exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out checkAfterSubCaseWithInExclusion);

                                                        checkExclusion = false;
                                                        if (checkAfterSubCaseWithInExclusion == true)
                                                        {
                                                            countWithInInAPara += 1;
                                                            gotResult = 1;
                                                            acceptParaWithIn += (acceptParaWithIn == "") ? withInIt : "|" + withInIt;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                }
                                            }
                                            if (acceptParaWithIn != "")
                                            {

                                                var jo = new JObject();
                                                jo["foundText"] = foundTextFinal;
                                                jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
                                                jo["fileName"] = fileName.Split('.')[0];
                                                jo["pageNo"] = pageCount;
                                                jo["pageContent"] = SearchWithinText;
                                                jo["foundWithIn"] = acceptParaWithIn;
                                                jo["paraNumber"] = paraNumber; 
                                                jo["completeFilePath"] = fullFilePath;
                                                ja.Add(jo);
                                            }
                                        }

                                        else // if not withIn to search
                                        {
                                            if (getExclusion.Count() != 0) {
                                                bool acceptFoundText = true;
                                                exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out acceptFoundText);
                                                
                                                if(acceptFoundText== true)
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
                                                    jo["completeFilePath"] = fullFilePath;
                                                    ja.Add(jo);
                                                }
                                            }
                                            else {
                                                gotResult = 1;
                                                var jo = new JObject();
                                                jo["foundText"] = foundTextFinal;
                                                jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
                                                jo["fileName"] = fileName.Split('.')[0];
                                                jo["pageNo"] = pageCount;
                                                jo["pageContent"] = SearchWithinText;
                                                jo["foundWithIn"] = "";
                                                jo["paraNumber"] = paraNumber;
                                                jo["completeFilePath"] = fullFilePath;
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
        }
        
        //--------------------------------------------------------------------------------------------------------------------------------------
        
        public void scoring(Dictionary<int, string> OutputMatch, string LeaseName, Dictionary<int, Dictionary<int, string>> savePage, string resultFormat, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja, int multipleRead, out JArray ja1, out int accptedValThere, out float finalScore)
        {
            var onlyTopResult = true;
            ja1 = new JArray();
            finalScore = 0;
            accptedValThere = 0;
            var getAllAcceptedText = JArray.Parse(ja.ToString()); // get all the accepted result
            Dictionary<int, float> scoreVal = new Dictionary<int, float>();
            Dictionary<int, string> saveScoringKeyword = new Dictionary<int, string>();
            for (var l = 0; l < getAllAcceptedText.Count(); l++)
            {
                var pageContent = getAllAcceptedText[l]["pageContent"].ToString(); // get the page value to search all search fileds
                var scorePerSearch = 0;
                double finalScorePerSearch = 0;

                var setScoringKeyword = "";
                foreach (KeyValuePair<string, int> singleSearchFieldScore in searchFieldScore)
                { //  loop through all the search field
                    
                    var matchDataWithInIt = Regex.Matches(pageContent, @"\b\s?" + singleSearchFieldScore.Key + "(\\s|\\b)"); // find match
                    if ((singleSearchFieldScore.Key).IndexOf("\"") == 0)
                    {
                        var searchVal = (singleSearchFieldScore.Key).Replace("\"", "");
                        matchDataWithInIt = Regex.Matches(pageContent, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                    }
                    if (singleSearchFieldScore.Key == "$")
                        matchDataWithInIt = Regex.Matches(pageContent, @"([$]+)"); // find match    
                    if (singleSearchFieldScore.Key == "%")
                        matchDataWithInIt = Regex.Matches(pageContent, @"(%)"); // find match
                    
                    

                    if (matchDataWithInIt.Count > 0) // if found the match
                    {
                        setScoringKeyword = setScoringKeyword + singleSearchFieldScore.Key + " | " ;

                        if (multipleRead == 0)
                            scorePerSearch += singleSearchFieldScore.Value; // increment the score
                        else
                            scorePerSearch += (singleSearchFieldScore.Value * matchDataWithInIt.Count); // increment the score
                    }
                }
                setScoringKeyword = "( "+ setScoringKeyword + " )";
                finalScorePerSearch = ((double)scorePerSearch / (double)totalScoreDenominatorVal) * 100; // get the percentage
                scoreVal.Add(l, (float)finalScorePerSearch); // save that in deictionary
                saveScoringKeyword.Add(l, setScoringKeyword);
            }
            if (scoreVal.Count() != 0)
            { // if dictionary has val
                var accurateVal = scoreVal.OrderByDescending(x => x.Value); // sort the dictionary to get the highest val
                foreach (KeyValuePair<int, float> entry in accurateVal)
                { // loop to get all the highest value
                    var outputSame = false;
                    var pageContent = getAllAcceptedText[entry.Key]["pageContent"].ToString();
                    foreach (var getOutputToCheck in OutputMatch) // check for duplicate... if the same sentance is already an output
                    {
                        if (getOutputToCheck.Value == pageContent)
                            outputSame = true; // if true dont take that as output  and select the next output
                    }
                    // save the output for the file
                    if (onlyTopResult == true && outputSame == false)
                    { // get all the highest score value
                        var getAllScoringKeyword = saveScoringKeyword[entry.Key];
                        var foundText = getAllAcceptedText[entry.Key]["foundText"].ToString();
                        var AllSearchFieldKeyword1 = getAllAcceptedText[entry.Key]["AllSearchFieldKeyword"];
                        var paraNumber = (int)getAllAcceptedText[entry.Key]["paraNumber"];
                        var pageNo = (int)getAllAcceptedText[entry.Key]["pageNo"];
                        var fileNameVal = getAllAcceptedText[entry.Key]["fileName"];
                        var completeFilePathVal = getAllAcceptedText[entry.Key]["completeFilePath"];
                        var withInValFound = getAllAcceptedText[entry.Key]["foundWithIn"];
                        finalScore = entry.Value;
                        Dictionary<int, string>.ValueCollection entry1 = savePage[pageNo].Values;
                        var getTheSectionValue = processing.SectionVal(savePage, pageNo, paraNumber); // get the section value
                        if (getTheSectionValue == "false")
                            getTheSectionValue = "?";

                        var output = resultFormat.Replace("{{Document Name}}", "<b>" + fileNameVal.ToString() + "</b>").Replace("{{Paragraph Number}}", "<b>" + getTheSectionValue + "</b>").Replace("{{result}}", pageContent).Replace("{{found text}}", AllSearchFieldKeyword1.ToString());
                        var jo1 = new JObject();
                        jo1["output"] = output;
                        jo1["Pageoutput"] = foundText;
                        jo1["AllSearchFieldKeyword"] = AllSearchFieldKeyword1;
                        jo1["fileName"] = fileNameVal;
                        jo1["pageNo"] = pageNo;
                        jo1["score"] = finalScore +"%"+" - " + getAllScoringKeyword;
                        jo1["foundWithIn"] = withInValFound;
                        jo1["sectionVal"] = getTheSectionValue;
                        jo1["leaseName"] = LeaseName;
                        jo1["completeFilePath"] = completeFilePathVal;
                        ja1.Add(jo1);
                        accptedValThere = 1;
                        onlyTopResult = false;
                    }
                }
            }
        }


        // save all data found in 
        public void saveDataToFolder(JArray jArray, string folderPath)
        {

            var saveDataFolder = folderPath + "\\output";
            var outputFilePath = saveDataFolder + "\\result.txt";

            if (!Directory.Exists(saveDataFolder))//if output folder not exist
                Directory.CreateDirectory(saveDataFolder);//create folder
            else if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            var pathString = System.IO.Path.Combine(saveDataFolder, "result.txt"); // create the input file

            var dataMain = new JArray();
            for (var i = 0; i < jArray.Count; i++)
            {
                var data = new JObject();
                data["fileName"] = jArray[i]["fileName"].ToString();
                data["searchField"] = jArray[i]["AllSearchFieldKeyword"].ToString();
                data["pageNoVal"] = jArray[i]["pageNo"].ToString();
                data["sectionVal"] = jArray[i]["sectionVal"].ToString();
                dataMain.Add(data);
            }
            File.WriteAllText(pathString, dataMain.ToString());

        }

        // check the library in whole pdf
        public void searchLibInPDF(JArray getExclusion, int exclusionCount, Dictionary<int, Dictionary<int, string>> savePage, string[] librarySet, string datapoint, out List<string> pdfLibPara, out List<int> pdfLibPageno, out List<int> pdfLibParaNo, out List<string> pdfLibValFound)
        {
            pdfLibPara = new List<string>();
            pdfLibParaNo = new List<int>();
            pdfLibPageno = new List<int>();
            pdfLibValFound = new List<string>();
            var pageCount = 0;
            var checkAfterSubCaseWithInExclusion = true;
            try
            {
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
                            var matchData = Regex.Matches(para, @"\b\s?" + singleLibVAl + "(\\s|\\b)"); // search if library in para
                            if ((singleLibVAl).IndexOf("\"") == 0)
                            {
                                var searchVal = (singleLibVAl).Replace("\"", "");
                                matchData = Regex.Matches(para, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                            }
                            if (matchData.Count > 0)
                            {
                                exclusionProcess(exclusionCount, getExclusion, para, out checkAfterSubCaseWithInExclusion); // exclusion 
                                if (checkAfterSubCaseWithInExclusion == true) { // save all the data of the para
                                    pdfLibPara.Add(para);
                                    pdfLibParaNo.Add(paraNo);
                                    pdfLibValFound.Add(singleLibVAl);
                                    pdfLibPageno.Add(pageCount);
                                    break;
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // remove para on exclusion
        public void exclusionProcess(int exclusionCount, JToken getExclusion, string SearchWithinText, out bool checkAfterSubCaseWithInExclusion)
        {

            checkAfterSubCaseWithInExclusion = true;
            var count = 0;
            for (var i = 0; i < getExclusion.Count(); i++) // loop through all the exclusion
            {
                var matchExclusion = Regex.Matches(SearchWithinText, @"\b\s?" + getExclusion[i]["keyword"].ToString() + "(\\s|\\b)"); // find match
                if ((getExclusion[i]["keyword"].ToString()).IndexOf("\"") == 0)
                {
                    var searchVal = (getExclusion[i]["keyword"].ToString()).Replace("\"", "");
                    matchExclusion = Regex.Matches(SearchWithinText, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                }
                if (getExclusion[i]["keyword"].ToString() == "$")
                    matchExclusion = Regex.Matches(SearchWithinText, @"([$]+)"); // find match    
                if (getExclusion[i]["keyword"].ToString() == "%")
                    matchExclusion = Regex.Matches(SearchWithinText, @"(%)"); // find match
                if (matchExclusion.Count > 0)
                    count += 1;
            }
            if (count >= exclusionCount) // remove if the count matches
                checkAfterSubCaseWithInExclusion = false;
        }

        // get all the correct sentance from all the para 
        public void collectCorrectSentance(Dictionary<string, int> withInScore, JArray ja2, out string getCorrectSentance)
        {
            getCorrectSentance = "";
            if (ja2.HasValues)
            {
                var pageContent = ja2[0]["Pageoutput"].ToString(); // get the para
                //string[] getSentance = pageContent.Split('hello'); // split on "."
                //string[] getSentance = Regex.Split(pageContent, ". ");
                string[] getSentance = pageContent.Split(new string[] { ". " }, StringSplitOptions.None);
                var foundWithIn = ja2[0]["foundWithIn"].ToString(); 
                string[] allWithIn = foundWithIn.Split('|'); // get all the within 
                Dictionary<string, int> getFinalSentence = new Dictionary<string, int>();
                HashSet<string> evenNumbers = new HashSet<string>();
                foreach (var sentanceVal in getSentance) // loop through all the sentance
                {
                    foreach (var withIn in allWithIn) // loop through all the within
                    {
                        //var withInScoreVal = withInScore[withIn];
                        var matchData = Regex.Matches(sentanceVal, @"\b\s?" + withIn + "\\b"); // find match
                        if ((withIn).IndexOf("\"") == 0)
                        {
                            var searchVal = (withIn).Replace("\"", "");
                            matchData = Regex.Matches(sentanceVal, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
                        }
                        if (withIn == "$")
                            matchData = Regex.Matches(sentanceVal, @"([$]+)"); // find match    
                        if (withIn == "%")
                            matchData = Regex.Matches(sentanceVal, @"(\d+%)"); // find match

                        if (matchData.Count > 0) // if found add the score of that within
                        {
                            evenNumbers.Add(sentanceVal);
                        }
                    }
                }
                var count = 0;
                foreach (var item in evenNumbers)
                {
                    count++;
                    getCorrectSentance = getCorrectSentance + "<b>(" + count + ")</b>" + item;
                }
            }

        }

    }
}