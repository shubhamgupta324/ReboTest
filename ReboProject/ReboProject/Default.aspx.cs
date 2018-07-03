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
            var watch = new System.Diagnostics.Stopwatch(); // get the time
                watch.Start();

            string backEndVal = backEndData.Text; // get the value from front end
            if (backEndVal == "") // check if it has value else return
                return;
            var backendObject = JObject.Parse(backEndVal.ToString()); // complete  json
            var multipleDatapointJson = backendObject["Datapoint"]; // get json for all datapoints

            //---------------------------------- read all files----------------------------------
            var folder = backendObject["folder"].ToString();// folder path
            var folderPath = "";
            string drivePath = WebConfigurationManager.AppSettings["DrivePath"]; // get the access to d 
            var userDefinePath = drivePath + folder;
            string[] subdirectoryEntries = Directory.GetDirectories(userDefinePath);
            string[] pdfFiles = { };
            var filepath = ""; // full file path
            List<string> fileFullPath = new List<string>();
            Dictionary<string, string[]> folder_fileName = new Dictionary<string, string[]>();
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> savePageAllFiles = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save pagenumber and the lines in it
            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++) {
                folderPath = subdirectoryEntries[folderval]; // get the folder from where to get pdf
                var fornotAbstractedLease = folderPath.Split('\\');
                var fornotAbstractedLeaseName = fornotAbstractedLease[fornotAbstractedLease.Length - 1];
                pdfFiles = Directory.GetFiles(folderPath, "*.pdf").Select(Path.GetFileName).ToArray(); // get all the pdf file in that folder\
                folder_fileName.Add(folderPath, pdfFiles);// get the folder name and the files in it
                foreach (var fileNameVal in pdfFiles)
                {
                    Dictionary<int, Dictionary<int, string>> savePageSingleFile = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    filepath = folderPath + "\\" + fileNameVal; // full path of file
                    fileFullPath.Add(filepath);
                    pdfRead(filepath, out savePageSingleFile); // read pdf
                    savePageAllFiles.Add(filepath, savePageSingleFile);
                }
            }
            //-------------------------------------------------------------------------------------
            
            var finalOutput = new JArray();// save all output in it for frontend display
            foreach (var singleDp in multipleDatapointJson) // loop through all the datapoints
            {
                var resultSearch = singleDp["result"][0]["search"];// eg: {{filename}}; {{result}}: {{pagenumber}}
                var SentenceResultOutputFormat = singleDp["result"][0]["SentenceoutputFormat"].ToString();// set all the sentences from all output configuration
                var resultOutputFormat = singleDp["result"][0]["outputFormat"].ToString();// eg: final output format
                var resultAllKeyword = singleDp["result"][0]["allKeyword"];// list of all keywords used
                var libraryVal = ""; // get all the library value
                if ((LibVal.Text) != "")
                    libraryVal = LibVal.Text;
                string[] LibArr = libraryVal.Split('|');
                var configuration = singleDp["Configuration"]; // get all the configuration
                var configurationOrder = new Dictionary<int, int>();
                for (var i = 0; i < configuration.Count(); i++) // loop through all the configuration to get the index
                {
                    configurationOrder.Add(i + 1, (int)configuration[i]["Index"]);
                }
                var list = configurationOrder.Values.ToList();
                list.Sort(); // sort to get the configuration order
                
                for (var folderval=0; folderval< subdirectoryEntries.Length; folderval++)
                {
                    var ja3 = new JArray(); // get the final output of one configuration from configuration
                    var jaLibCheck = new JArray(); // get the final output of one configuration from library
                    var LeaseName = ""; // get lease name
                    
                    Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    Dictionary<int, Dictionary<int, string>> savePageLib = new Dictionary<int, Dictionary<int, string>>();  // duplicate of savePage
                    Dictionary<int, string> OutputMatch = new Dictionary<int, string>();
                    string getCorrectSentances = "";
                    JArray jaCompleteSentence = new JArray();
                    Dictionary<string, int> withInForSentence = new Dictionary<string, int>();
                    List<string> getParaForSentence = new List<string>();
                    Dictionary<string, string> getSectionAndFileName = new Dictionary<string, string>();
                    var getSectionAndFileNameAndSearchJA = new JArray();
                    var nextVal = 0;
                    
                    var copyResultOutputFormat = SentenceResultOutputFormat;
                    var nextDuplicateCheck = 0;
                    for (var configurationVal = 0; configurationVal < configuration.Count(); configurationVal++)
                    {
                        // ------------------the data of the configuration to be read---------------------
                        var getSectionAndFileNameAndSearchJO = new JObject();
                        var getTheWithinDictionary = true;
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
                        //---------------------------------------------------------------------------------

                        // ---------------------if connectorVal ==2----------------------------------------
                        bool executeConfiguration = true;
                        List<int> pdfLibPageno = new List<int>(); // get page no of the para
                        List<int> pdfLibParaNo = new List<int>(); // get the para no of para
                        List<string> pdfLibPara = new List<string>(); // get the para
                        List<string> pdfLibValFound = new List<string>(); // the lib value found in para
                        var outputFound = false;
                        if (connectorVal == 2)
                        { // check if the last output contain the library value
                            if (LibArr[0] != "" && jaLibCheck.Count != 0)
                            {
                                checklibrary(jaLibCheck, LibArr, out outputFound); // check if output of first logic has library value
                                if (outputFound == false)
                                { // check the whole pdf for library 
                                    if (!jaLibCheck.HasValues)
                                        break;
                                    savePage = savePage = savePageAllFiles[jaLibCheck[0]["completeFilePath"].ToString()];
                                    searchLibInPDF(getExclusion, exclusionCount, savePage, LibArr, datapoint, out pdfLibPara, out pdfLibPageno, out pdfLibParaNo, out pdfLibValFound);
                                    savePageLib = savePage;
                                    executeConfiguration = pdfLibPara.Count() != 0 ? executeConfiguration = true : executeConfiguration = false; // if found get the second para else skip
                                }
                            }
                            else
                                executeConfiguration = false;
                        }
                        //--------------------------------------------------------------------------------
                        if (executeConfiguration == true)
                        { // check if to execute configurationDatapoint
                            var sort = (int)configuration[myKey - 1]["FileOrder"]["sort"]; // asc or desc
                            var type = (int)configuration[myKey - 1]["FileOrder"]["type"]; // Single File Search or All File Search
                            var datapointID = (int)configuration[myKey - 1]["DpId"]; // Datapoint ID
                            var datapointName = configuration[myKey - 1]["Datapoint"].ToString(); // Datapoint ID
                            var multipleRead = (int)configuration[myKey - 1]["MultipleRead"];  // to get score of multiple occurance
                            var SearchWithin = (int)configuration[myKey - 1]["SearchWithin"];// 1=page, 2=section, 3=paragraph
                            var mainLeaseRead = (int)configuration[myKey - 1]["MainLeaseRead"];// skip main lease read
                            var pageNoRange = configuration[myKey - 1]["PageNoRange"];// skip main lease read
                            var startPage = pageNoRange[0]["startRange"].ToString();
                            var endPage = pageNoRange[0]["endRange"].ToString();
                            var startPageVal = 0;
                            var endPageVal = 0;

                            Dictionary<string, int> searchFieldScore = new Dictionary<string, int>(); // saves the foundtext
                            Dictionary<string, int> withInScore = new Dictionary<string, int>(); // saves the foundtext
                            var totalScoreDenominatorVal = 0; // get the total score of all the search field
                            var ja1 = new JArray();
                            var ja2 = new JArray();

                            // -----------get the file order--------------
                            ArrayList fileDetails = new ArrayList();
                            
                            var fileFullPathToRead = subdirectoryEntries[folderval];
                            fileToRead(folder_fileName,mainLeaseRead, fileFullPathToRead, sort, type, out fileDetails);
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
                                    
                                    savePage = savePageAllFiles[fullFilePath]; // all page data 

                                    getPageNo(savePage.Count(), startPage, endPage, out startPageVal, out endPageVal);
                                    Dictionary<Dictionary<string, int>, int> getNode = new Dictionary<Dictionary<string, int>, int>();
                                    if (SearchWithin == 2)
                                    {
                                        getNode = Program.SectionVal123(startPageVal, endPageVal, savePage);
                                        getAllFoundText(startPageVal, endPageVal, getNode, exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text
                                    }
                                    else
                                        getAllFoundText(startPageVal, endPageVal, getNode, exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text

                                    if (getTheWithinDictionary == true)
                                    { // get the within in Dictionary for sentence
                                        foreach (var item in withInScore)
                                        {
                                            if (!withInForSentence.ContainsKey(item.Key)) // check if dictionary has the same key
                                                withInForSentence.Add(item.Key, item.Value);
                                        }
                                    }
                                    
                                    //--------------------scoring and final output ---------------------------------------------------------------------
                                    scoring(OutputMatch, LeaseName, savePage, totalScoreDenominatorVal, searchFieldScore, ja, multipleRead, out ja1, out finalScore);

                                    for (int i = 0; i < ja1.Count; i++)// get only one highest score value
                                    {
                                        if (type == 1) // check in all pdf or stop if found in one
                                            readNextFile = 0;
                                        if (TopfinalScore < finalScore) // check the score to get the highest
                                        {
                                            TopfinalScore = finalScore;
                                            ja2.RemoveAll();
                                            ja2.Add(ja1[i]);
                                        }
                                    }
                                }
                            }

                            if (ja2.Count > 0)// check if any result found
                            {
                                var getTheSectionValue = "";
                                if (SearchWithin == 3)
                                {
                                    savePage = savePage = savePageAllFiles[ja2[0]["completeFilePath"].ToString()];

                                    getTheSectionValue = processing.SectionValParagraph(savePage, (int)ja2[0]["pageNo"], (int)ja2[0]["paraNo"]); // get the section value
                                    ja2[0]["sectionVal"] = getTheSectionValue;
                                }
                                if (SearchWithin == 2)
                                {
                                    getTheSectionValue = processing.SectionValSection(ja2[0]["pageContent"].ToString(), ja2[0]["Pageoutput"].ToString()); // get the section value
                                    ja2[0]["sectionVal"] = getTheSectionValue;
                                }
                                if (!getParaForSentence.Contains(ja2[0]["Pageoutput"].ToString()))
                                {
                                    getSectionAndFileNameAndSearchJO["Pageoutput"] = ja2[0]["Pageoutput"].ToString();
                                    getSectionAndFileNameAndSearchJO["fileName"] = ja2[0]["fileName"].ToString();
                                    getSectionAndFileNameAndSearchJO["sectionNo"] = getTheSectionValue;
                                    getSectionAndFileNameAndSearchJO["searchFor"] = ja2[0]["AllSearchFieldKeyword"].ToString();
                                    getSectionAndFileNameAndSearchJA.Add(getSectionAndFileNameAndSearchJO);
                                }
                            }


                            if (ja2.Count == 0)// check if any result found
                                resultFound = 0;

                            if (!ja3.HasValues && ja2.HasValues)
                            {
                                ja3.Add(ja2[0]);
                                jaLibCheck.RemoveAll();
                                jaLibCheck.Add(ja3[0]);
                                gotValueForConfiguration = true;
                                saveDataToFolder(ja3, folderPath);
                                if (!OutputMatch.ContainsValue(ja2[0]["Pageoutput"].ToString()))
                                {
                                    OutputMatch.Add(nextDuplicateCheck, ja2[0]["Pageoutput"].ToString());
                                    nextDuplicateCheck++;
                                }
                            }

                            else
                            {
                                if (ja3.HasValues && ja2.HasValues)
                                {
                                    ja3[0]["output"] = ja3[0]["output"].ToString() + System.Environment.NewLine + "###" + ja2[0]["output"].ToString();
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
                                    if (!OutputMatch.ContainsValue(ja2[0]["Pageoutput"].ToString()))
                                    {
                                        OutputMatch.Add(nextDuplicateCheck, ja2[0]["Pageoutput"].ToString());
                                        nextDuplicateCheck++;
                                    }
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
                                    OutputMatch.Add(nextDuplicateCheck, pdfLibPara[0]);
                                    nextDuplicateCheck++;
                                    var getTheSectionValue = processing.SectionValParagraph(savePageLib, pdfLibPageno[0], pdfLibParaNo[0]); // get the section value
                                    if (getTheSectionValue == "false")
                                        getTheSectionValue = "?";
                                    //var output = resultset.Replace("{{Document Name}}", "").Replace("{{Paragraph Number}}", "<b>" + getTheSectionValue + "</b>").Replace("{{result}}", pdfLibPara[0]).Replace("{{found text}}", "....");
                                    var output = pdfLibPara[0];
                                    ja3[0]["output"] = ja3[0]["output"].ToString() + System.Environment.NewLine + "###" + output;
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
                                        getCorrectSentances = getCorrectSentances + " <b> (" + (configurationVal + 1) + ")</b>  " + pdfLibPara[0];
                                }
                            }
                        }
                        nextVal++;
                    }

                    // get the final output 
                    if (getSectionAndFileNameAndSearchJA.HasValues)
                    {
                        var finalOutputData = "";
                        JArray collectCorrectSentanceOutput = new JArray();
                        collectCorrectSentance(getSectionAndFileNameAndSearchJA, withInForSentence, resultAllKeyword, resultSearch, copyResultOutputFormat, out finalOutputData, out collectCorrectSentanceOutput);
                        var format = "";
                        buildFormat(collectCorrectSentanceOutput, finalOutputData, resultOutputFormat, out format);
                        ja3[0]["correctString"] = format;
                    }

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
                }
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
        
        // get the file order
        //public void fileToRead(Dictionary<string,string[]> folder_fileName, int mainLeaseRead, string folderPath, int sort, int type, out ArrayList fileDetails)
        //{
        //    fileDetails = new ArrayList();
        //    var pdfFiles = folder_fileName[folderPath];
        //    if (pdfFiles.Length > 0) // check if file is there in folder
        //    {
        //        for (var j = 0; j < pdfFiles.Length; j++) // loop through all the files
        //        {
        //            var index = sort == 2 ? pdfFiles.Length - (j + 1) : j; // get the file by order
        //            var fileNameVal = pdfFiles[index]; // get the index of the file to read
        //            var lowerFileNameVal = fileNameVal.ToLower();
        //            if (mainLeaseRead == 0 && lowerFileNameVal.IndexOf("lease") == 0) // check if only one file there and name starts with lease
        //                continue;
        //            var filepath = folderPath + "\\" + fileNameVal; // full path of file save 
        //            fileDetails.Add(filepath); // save in arraylist
        //        }
        //    }
        //}


        public void fileToRead(Dictionary<string, string[]> folder_fileName, int mainLeaseRead, string folderPath, int sort, int type, out ArrayList fileDetails)
        {
            fileDetails = new ArrayList();
            if (true)
            {
                var pdfFiles = folder_fileName[folderPath];
                Dictionary<string, DateTime> fileSort = new Dictionary<string, DateTime>();
                for (var j = 0; j < pdfFiles.Length; j++)
                {
                    var fileNameVal = pdfFiles[j];
                    var getDate = fileNameVal.Split('-')[0];
                    var mm = getDate[0].ToString() + getDate[1].ToString();
                    var dd = getDate[2].ToString() + getDate[3].ToString();
                    var yyyy = getDate[4].ToString() + getDate[5].ToString() + getDate[6].ToString() + getDate[7].ToString();
                    DateTime dateVal = new DateTime(Convert.ToInt32(yyyy), Convert.ToInt32(mm), Convert.ToInt32(dd), 0, 0, 0);
                    //var uniDate = dateVal.ToUniversalTime();
                    fileSort.Add(fileNameVal, dateVal);
                }
                var desc = sort == 2 ? 0 : 1; // get the file by order
                Dictionary<string, DateTime> sortedFile = new Dictionary<string, DateTime>();
                if (desc == 0)
                    sortedFile = fileSort.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                else
                    sortedFile = fileSort.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


                foreach (var item in sortedFile)
                {
                    var fileNameVal = item.Key;
                    var lowerFileNameVal = fileNameVal.ToLower().Split('-')[1];
                    if (mainLeaseRead == 0 && lowerFileNameVal.IndexOf(" lease") == 0)
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
                        if ((prevRect.Bottom - lineGroup.Rect.Top) > 8.5)
                        { // current line is > 9 points lower than previous
                            saveLines.Add(i, sb1.ToString());
                            i++;
                            sb1.Clear();
                        }
                        sb1.Append(lineGroup.Text + " ");
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
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            //var getTheLibrary = librarySet[datapoint.ToLower()]; //  get all library for datapoint
            for (var i = 0; i < ja2.Count(); i++)
            {
                if (!ja2[i].HasValues)
                    break;
                var outputConfig1 = ja2[i]["Pageoutput"].ToString(); // get  the para

                for (var singleSF = 0; singleSF < librarySet.Count(); singleSF++)
                {
                    MatchCollection matchData;
                    regexMatch(rgx, outputConfig1, librarySet[singleSF], out matchData); // function to match 

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
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            for (var i = 0; i < getSubCase.Count(); i++)
            {
                if (checkForVal == (getSubCase[i]["checkFor"]).ToString())
                {
                    var keyword = (getSubCase[i]["keyword"]).ToString();
                    var matchDataWithInIt = Regex.Matches(lineToCheck, @"\b\s?" + rgx.Replace(keyword, "\\$1") + "(\\s|\\b)"); // find match
                    if (keyword.IndexOf("\"") == 0)
                    {
                        var searchVal = (rgx.Replace(keyword, "\\$1")).Replace("\"", "");
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
        public void getAllFoundText(int startPageVal, int endPageVal,Dictionary<Dictionary<string, int>, int> getNode, int exclusionCount, string fullFilePath, int SearchWithin, Dictionary<int, Dictionary<int, string>> savePage, string fileName, JToken logic, out JArray ja, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
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
                    getTotalScore(getWithIn, getSearchFor, out totalScoreDenominatorVal, out searchFieldScore, out withInScore);
                    Regex rgx = new Regex("(['^$.|?*+()\\\\])");
                    if (SearchWithin == 3)
                    {
                        for (var k = 0; k < getSearchFor.Count(); k++) // loop throuch all searchFor
                        {
                            var AllSearchFieldKeyword = (getSearchFor[k]["keyword"]).ToString(); // get the search field

                            var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field op

                            bool checkAfterSubCaseSearchFor = true;
                            var pageCount = 0;
                            if (SearchWithin == 3)
                            {
                                foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
                                {
                                    pageCount += 1;
                                    if (pageCount < startPageVal)
                                        continue;
                                    if (pageCount > endPageVal)
                                        break;
                                    // saves the lineText
                                    JArray ja1Val = new JArray();
                                    StringBuilder sb3 = new StringBuilder();
                                    var paraNumber = 0;
                                    foreach (var checkPage in entry.Value) // each page value
                                    {
                                        paraNumber = paraNumber + 1;
                                        processSearchForAndwithInParagraph(paraNumber, checkPage, AllSearchFieldKeyword, rgx, AllSearchFieldCaseCheck, getSubCase, checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName, pageCount, fullFilePath, out ja1Val);
                                        if (ja1Val.HasValues)
                                            ja.Add(ja1Val[0]);
                                    }
                                }
                            }
                        }
                    }

                    if (SearchWithin == 2)
                    {
                        foreach (var entry in getNode) // get the section
                        {
                            bool checkAfterSubCaseSearchFor = true;
                            JArray ja1Val = new JArray();
                            processSearchForAndwithInSection(startPageVal, endPageVal, getSearchFor, entry, rgx, getSubCase, checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName, fullFilePath, out ja1Val);
                            if (ja1Val.HasValues)
                                ja.Add(ja1Val[0]);
                        }

                    }

                }
            }
        }

        public void processSearchForAndwithInParagraph(int paraNumber, KeyValuePair<int, string> checkPage, string AllSearchFieldKeyword, Regex rgx, string AllSearchFieldCaseCheck, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, int pageCount, string fullFilePath, out JArray ja)
        {

            ja = new JArray();
            //paraNumber += 1;
            var completeSectionText = "";
            var getLineText = checkPage.Value; // get the  line text
            MatchCollection matchData;
            regexMatch(rgx, getLineText, AllSearchFieldKeyword, out matchData);

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
                    SearchWithinText = getLineText;
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
                            MatchCollection matchDataWithInIt;
                            // match function
                            regexMatch(rgx, SearchWithinText, withInIt, out matchDataWithInIt);

                            if (matchDataWithInIt.Count > 0) // if match there
                            {
                                if (withInCaseCheck == "yes")// check for cases
                                    subCaseSearch(SearchWithinText, withInIt, getSubCase, out checkAfterSubCaseWithIn);
                                if (checkAfterSubCaseWithIn == true)
                                {
                                    if (getExclusion.Count() > 0 && checkExclusion == true)
                                        exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out checkAfterSubCaseWithInExclusion);

                                    checkExclusion = false;
                                    if (checkAfterSubCaseWithInExclusion == true)
                                    {
                                        countWithInInAPara += 1;
                                        acceptParaWithIn += (acceptParaWithIn == "") ? withInIt : "|" + withInIt;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        if (acceptParaWithIn != "")
                        {
                            gotResult = 1;
                            jarrayEnter(completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                        }
                    }

                    else // if not withIn to search
                    {
                        var acceptParaWithIn = "";
                        if (getExclusion.Count() != 0)
                        {
                            bool acceptFoundText = true;
                            exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out acceptFoundText);

                            if (acceptFoundText == true)
                            {
                                gotResult = 1;
                                jarrayEnter(completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                            }
                        }
                        else
                        {
                            gotResult = 1;
                            jarrayEnter(completeSectionText, AllSearchFieldKeyword, fileName.Split('.')[0], pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                        }
                    }
                }
            }
        }

        public void processSearchForAndwithInSection(int startPageVal, int endPageVal,JToken getSearchFor, KeyValuePair<Dictionary<string, int>, int> entry, Regex rgx, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, string fullFilePath, out JArray ja)
        {
            ja = new JArray();
            var pageCount = entry.Value;
            var AllSearchFieldKeyword = "";
            var acceptParaWithIn = "";
            List<string> saveAllPara = new List<string>();
            List<string> saveAllwithin = new List<string>();
            List<string> saveAllSearchFor = new List<string>();
            var getLineText = "";
            var paraNumber = 0;
            var completeSectionText = "";
            foreach (var item in entry.Key)
            {
                completeSectionText = completeSectionText + item.Key + "|||";
                if (paraNumber == 0)
                    paraNumber = item.Value;

                getLineText = item.Key; // get the  line text
                for (var k = 0; k < getSearchFor.Count(); k++)
                {
                    AllSearchFieldKeyword = (getSearchFor[k]["keyword"]).ToString(); // get the search field
                    var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field op
                    MatchCollection matchData;
                    regexMatch(rgx, getLineText, AllSearchFieldKeyword, out matchData);
                    if (matchData.Count > 0)
                    {
                        if (AllSearchFieldCaseCheck == "yes")
                            subCaseSearch(getLineText, AllSearchFieldKeyword, getSubCase, out checkAfterSubCaseSearchFor);
                        else
                            checkAfterSubCaseSearchFor = true;

                        if (checkAfterSubCaseSearchFor == true)
                        {
                            var SearchWithinText = "";
                            SearchWithinText = getLineText;
                            if (getWithIn.Count() > 0)
                            {
                                var checkExclusion = true;
                                for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                {
                                    bool checkAfterSubCaseWithIn = true;
                                    bool checkAfterSubCaseWithInExclusion = true;
                                    var withInIt = (getWithIn[g]["keyword"]).ToString();
                                    var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                                    MatchCollection matchDataWithInIt;
                                    // match function
                                    regexMatch(rgx, SearchWithinText, withInIt, out matchDataWithInIt);

                                    if (matchDataWithInIt.Count > 0) // if match there
                                    {
                                        if (withInCaseCheck == "yes")// check for cases
                                            subCaseSearch(SearchWithinText, withInIt, getSubCase, out checkAfterSubCaseWithIn);
                                        if (checkAfterSubCaseWithIn == true)
                                        {
                                            if (getExclusion.Count() > 0 && checkExclusion == true)
                                                exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out checkAfterSubCaseWithInExclusion);

                                            checkExclusion = false;
                                            if (checkAfterSubCaseWithInExclusion == true)
                                            {
                                                if (!saveAllPara.Contains(SearchWithinText))
                                                    saveAllPara.Add(SearchWithinText);
                                                if (!saveAllwithin.Contains(withInIt))
                                                    acceptParaWithIn += (acceptParaWithIn == "") ? withInIt : "|" + withInIt;
                                                if (!saveAllSearchFor.Contains(AllSearchFieldKeyword))
                                                    saveAllSearchFor.Add(AllSearchFieldKeyword);
                                            }
                                            else
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (acceptParaWithIn != "")
            {
                var finalPara = "";
                for (int i = 0; i < saveAllPara.Count(); i++)
                {
                    finalPara = finalPara + saveAllPara[i] + "|||. ";
                }
                getLineText = finalPara;
                gotResult = 1;
                var AllSearchFieldKeywordVal = "";
                foreach (var item in saveAllSearchFor)
                {
                    AllSearchFieldKeywordVal = AllSearchFieldKeywordVal + item + "|";
                }
                JArray ja1 = new JArray();
                jarrayEnter(completeSectionText, AllSearchFieldKeywordVal, fileName, pageCount, getLineText, acceptParaWithIn, paraNumber, fullFilePath, out ja1);
                ja.Add(ja1[0]);
            }
        }

        // save data in jarray
        public void jarrayEnter(string completeSectionText, string AllSearchFieldKeyword, string fileName, int pageCount, string getLineText, string acceptParaWithIn, int paraNumber, string fullFilePath, out JArray ja)
        {

            ja = new JArray();
            var jo = new JObject();
            jo["foundText"] = getLineText;
            jo["AllSearchFieldKeyword"] = AllSearchFieldKeyword;
            jo["fileName"] = fileName;
            jo["pageNo"] = pageCount;
            jo["pageContent"] = completeSectionText;
            jo["foundWithIn"] = acceptParaWithIn;
            jo["paraNumber"] = paraNumber;
            jo["completeFilePath"] = fullFilePath;
            ja.Add(jo);
        }

        // to match regex condition for searchfor and within
        public void regexMatch(Regex rgx, string sentence, string toSearch, out MatchCollection matchData)
        {

            matchData = null;
            matchData = Regex.Matches(sentence, @"\b\s?" + rgx.Replace(toSearch, "\\$1") + "(\\s|\\b)");
            if ((toSearch).IndexOf("\"") == 0)
            {
                var searchVal = (rgx.Replace(toSearch, "\\$1")).Replace("\"", "");
                matchData = Regex.Matches(sentence, "[\"]" + searchVal + "[\"][^a-zA-Z0-9_]"); // find match
            }
            if (toSearch == "$")
                matchData = Regex.Matches(sentence, @"([$]+)"); // find match    
            if (toSearch == "%")
                matchData = Regex.Matches(sentence, @"(%)"); // find match
        }
        //--------------------------------------------------------------------------------------------------------------------------------------

        public void scoring(Dictionary<int, string> OutputMatch, string LeaseName, Dictionary<int, Dictionary<int, string>> savePage, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja, int multipleRead, out JArray ja1, out float finalScore)
        {
            var onlyTopResult = true;
            ja1 = new JArray();
            finalScore = 0;
            var getAllAcceptedText = JArray.Parse(ja.ToString()); // get all the accepted result
            Dictionary<int, float> scoreVal = new Dictionary<int, float>();
            Dictionary<int, string> saveScoringKeyword = new Dictionary<int, string>();
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            for (var l = 0; l < getAllAcceptedText.Count(); l++)
            {
                var pageContent = getAllAcceptedText[l]["foundText"].ToString(); // get the page value to search all search fileds
                var scorePerSearch = 0;
                double finalScorePerSearch = 0;

                var setScoringKeyword = "";
                foreach (KeyValuePair<string, int> singleSearchFieldScore in searchFieldScore)
                { //  loop through all the search field

                    MatchCollection matchDataWithInIt;
                    regexMatch(rgx, pageContent, singleSearchFieldScore.Key, out matchDataWithInIt); // function to match

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
                    var pageContent = getAllAcceptedText[entry.Key]["foundText"].ToString();
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
                        var pageCompleteContent = getAllAcceptedText[entry.Key]["pageContent"].ToString();
                        var AllSearchFieldKeyword1 = getAllAcceptedText[entry.Key]["AllSearchFieldKeyword"];
                        var paraNumber = (int)getAllAcceptedText[entry.Key]["paraNumber"];
                        var pageNo = (int)getAllAcceptedText[entry.Key]["pageNo"];
                        var fileNameVal = getAllAcceptedText[entry.Key]["fileName"];
                        var completeFilePathVal = getAllAcceptedText[entry.Key]["completeFilePath"];
                        var withInValFound = getAllAcceptedText[entry.Key]["foundWithIn"];
                        finalScore = entry.Value;
                        //var output = resultFormat.Replace("{{Document Name}}", "<b>" + fileNameVal.ToString() + "</b>").Replace("{{result}}", foundText).Replace("{{found text}}", AllSearchFieldKeyword1.ToString());

                        var jo1 = new JObject();
                        jo1["output"] = foundText;
                        jo1["Pageoutput"] = foundText;
                        jo1["AllSearchFieldKeyword"] = AllSearchFieldKeyword1;
                        jo1["fileName"] = fileNameVal;
                        jo1["pageNo"] = pageNo;
                        jo1["paraNo"] = paraNumber;
                        jo1["score"] = finalScore +"%"+" - " + getAllScoringKeyword;
                        jo1["foundWithIn"] = withInValFound;
                        jo1["pageContent"] = pageCompleteContent;
                        jo1["sectionVal"] = "";
                        jo1["leaseName"] = LeaseName;
                        jo1["completeFilePath"] = completeFilePathVal;
                        ja1.Add(jo1);
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
                    Regex rgx = new Regex("(['^$.|?*+()\\\\])");
                    foreach (var checkPage in entry.Value)
                    {
                        var para = checkPage.Value;
                        var paraNo = checkPage.Key;
                        //var librarySetVal = librarySet[datapoint.ToLower()];
                        for (var i = 0; i < librarySet.Count(); i++)
                        {
                            
                            var singleLibVAl = librarySet[i];
                            MatchCollection matchData;
                            regexMatch(rgx, para, singleLibVAl, out matchData); // function to match 

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
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            checkAfterSubCaseWithInExclusion = true;
            var count = 0;
            for (var i = 0; i < getExclusion.Count(); i++) // loop through all the exclusion
            {
                MatchCollection matchExclusion;
                regexMatch(rgx, SearchWithinText, getExclusion[i]["keyword"].ToString(), out matchExclusion); // function to match
                
                if (matchExclusion.Count > 0)
                    count += 1;
            }
            if (count >= exclusionCount) // remove if the count matches
                checkAfterSubCaseWithInExclusion = false;
        }

        public void getPageNo(int totalPages, string startRange, string endRange, out int startPageVal, out int endPageVal)
        {
            startPageVal = 0;
            endPageVal = 0;
            // get start value
            if (startRange == "")
                startPageVal = 1;
            else if (startRange.Contains("<"))
                startPageVal = 1;
            else if (startRange.Contains(">"))
            {
                var startRangeNumber = startRange.Replace(">", "");
                var pageNo = Int32.Parse(startRangeNumber);
                if (pageNo <= totalPages)
                    startPageVal = pageNo;
                else
                    startPageVal = 1;
            }
            else if (!startRange.Contains(">") && !startRange.Contains("<"))
            {
                if (Int32.Parse(startRange) <= totalPages)
                    startPageVal = Int32.Parse(startRange);
                else
                    startPageVal = 1;
            }
            // get end value
            if (endRange == "")
                endPageVal = totalPages;
            else if (endRange.Contains("<"))
            {
                var endRangeNumber = endRange.Replace("<", "");
                var pageNo = Int32.Parse(endRangeNumber);
                if ((totalPages - pageNo) > 0 && startPageVal <= (totalPages - pageNo))
                    endPageVal = totalPages - pageNo;
                else
                    endPageVal = totalPages;
            }
            else if (endRange.Contains(">"))
                endPageVal = totalPages;
            else if (!endRange.Contains(">") && !endRange.Contains("<"))
            {
                if (Int32.Parse(endRange) >= startPageVal && Int32.Parse(endRange) <= totalPages)
                    endPageVal = Int32.Parse(endRange);
                else
                    endPageVal = Int32.Parse(endRange);
            }
        }

        // get all the correct sentance from all the para 
        public void collectCorrectSentance(JArray getSectionAndFileNameAndSearchJA, Dictionary<string,int> withInForSentence,JToken resultAllKeyword,JToken resultSearch,string copyResultOutputFormat, out string finalOutputData, out JArray collectCorrectSentanceOutput)
        {
            finalOutputData = "";
            collectCorrectSentanceOutput = new JArray();
            Dictionary<string,string> getAllTheSentence = new Dictionary<string, string>();
            List<string> sentenceSection = new List<string>();
            List<string> sentenceFileName = new List<string>();
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            foreach (var item in getSectionAndFileNameAndSearchJA)
            {
                
                string[] getSentanceColon = item["Pageoutput"].ToString().Split(new string[] { "; " }, StringSplitOptions.None);
                string[] getSentanceFullStop = item["Pageoutput"].ToString().Split(new string[] { ". " }, StringSplitOptions.None);
                if (getSentanceColon.Count() < getSentanceFullStop.Count())
                    getSentanceFullStop = getSentanceFullStop.Take(getSentanceFullStop.Count() - 1).ToArray();
                string[] getSentance = getSentanceColon.Count() > getSentanceFullStop.Count() ? getSentanceColon : getSentanceFullStop;
                foreach (var sentenceVal in getSentance)
                {
                    foreach (var withIn in withInForSentence) // loop through all the within
                    {
                        MatchCollection matchData = null;
                        regexMatch(rgx, sentenceVal, withIn.Key, out matchData); // function to match

                        if (matchData.Count > 0) // if found add the score of that within
                        {
                            if (!getAllTheSentence.ContainsKey(sentenceVal))
                            {
                                getAllTheSentence.Add(sentenceVal, item["searchFor"].ToString());
                                sentenceSection.Add(item["sectionNo"].ToString());
                                sentenceFileName.Add(item["fileName"].ToString());
                            }
                        }
                    }
                }
            }

            Dictionary<string, string> allSetKeyword = new Dictionary<string, string>();
            foreach (var item in resultAllKeyword)
            {
                allSetKeyword.Add(item["id"].ToString(), item["keyword"].ToString());
            }
            foreach (var searchVal in resultSearch)
            {
                var searchId = (int)searchVal["id"];
                var searchAndCondition = searchVal["andCondition"];
                var searchFormat = searchVal["format"].ToString();
                var searchExclusion = searchVal["exclusion"];
                Dictionary<string, string> getSearchExclusion = new Dictionary<string, string>();
                foreach (var item in searchExclusion)
                {
                    getSearchExclusion.Add(item["keyword"].ToString(), item["Check"].ToString());
                }
                foreach (var andConditionVal in searchAndCondition)
                {
                    var andConditionId = (int)andConditionVal["id"];
                    var andConditionOrCondition = andConditionVal["orCondition"];
                    var checkNextOrCondition = true;
                    foreach (var orConditionVal in andConditionOrCondition)
                    {
                        if (checkNextOrCondition == false)
                            break;
                        List<string> getKeyword = new List<string>();
                        var orConditionKeyword = orConditionVal["keyword"].ToString();
                        var orConditionCondition = (int)orConditionVal["condition"];
                        var orConditionSentence = (int)orConditionVal["sentence"];
                        var sentenceAsOutput = "";
                        var orConditionFormat = orConditionVal["format"].ToString();
                        string[] splitkeywordId = orConditionKeyword.Split('|');
                        foreach (var item in splitkeywordId)
                        {
                            getKeyword.Add(allSetKeyword[item]);
                        }
                        var checkNextSentence = true;
                        var next = 0;
                        var checkNextSentenceExclusion = true;
                        foreach (var singleSentence in getAllTheSentence)
                        {
                            if (checkNextSentence == false)
                                break;
                            foreach (var item in getSearchExclusion)
                            {
                                MatchCollection matchDataKey;
                                regexMatch(rgx, singleSentence.Key, item.Key, out matchDataKey);
                                MatchCollection matchDataValue;
                                regexMatch(rgx, singleSentence.Key, item.Value, out matchDataValue);
                                if (matchDataKey.Count > 0 && matchDataValue.Count > 0)
                                {
                                    checkNextSentenceExclusion = false;
                                    break;
                                }
                            }
                            if (checkNextSentenceExclusion == false)
                                break;
                            var count = 0;
                            foreach (var toFIndVal in getKeyword)
                            {
                                var regexToFInd = "";
                                var resultKeywordCopy = toFIndVal;
                                if (toFIndVal.Contains("##d##"))
                                {
                                    var getFromString = resultKeywordCopy.Replace("##d##", "").Trim();
                                    if (toFIndVal.IndexOf("##d##") == 0)
                                        regexToFInd = @"([0-9\.]+[\s]*[" + getFromString + "])";
                                    else
                                        regexToFInd = @"([" + getFromString + "][\\s]*[0-9\\.]+)";
                                }
                                else
                                {
                                    regexToFInd = @"\b\s?" + rgx.Replace(toFIndVal, "\\$1") + "(\\s|\\b)";
                                }
                                Regex regex = new Regex(regexToFInd);
                                var match = regex.Match(singleSentence.Key); // check if match found
                                if (match.Success)
                                {
                                    JObject collectCorrectSentanceOutputJO = new JObject();
                                    count++;
                                    if (orConditionCondition == 1 && count == getKeyword.Count())
                                    {
                                        sentenceAsOutput = singleSentence.Key;
                                        collectCorrectSentanceOutputJO["sentence"]= singleSentence.Key;
                                        collectCorrectSentanceOutputJO["sectionNo"] = sentenceSection.ElementAt(next);
                                        collectCorrectSentanceOutputJO["fileName"] = sentenceFileName.ElementAt(next);
                                        collectCorrectSentanceOutputJO["search"] = singleSentence.Value;
                                        collectCorrectSentanceOutput.Add(collectCorrectSentanceOutputJO);
                                        checkNextOrCondition = false;
                                        checkNextSentence = false;
                                        break;
                                    }
                                    if (orConditionCondition == 0)
                                    {
                                        sentenceAsOutput = singleSentence.Key;
                                        collectCorrectSentanceOutputJO["sentence"] = singleSentence.Key;
                                        collectCorrectSentanceOutputJO["sectionNo"] = sentenceSection.ElementAt(next);
                                        collectCorrectSentanceOutputJO["fileName"] = sentenceFileName.ElementAt(next);
                                        collectCorrectSentanceOutputJO["search"] = singleSentence.Value;
                                        collectCorrectSentanceOutput.Add(collectCorrectSentanceOutputJO);
                                        checkNextOrCondition = false;
                                        checkNextSentence = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (orConditionSentence == 1)
                        {
                            if (sentenceAsOutput == "")
                                searchFormat = searchFormat.Replace("{{" + andConditionId + "}}", "");
                            else
                                searchFormat = searchFormat.Replace("{{" + andConditionId + "}}", sentenceAsOutput);
                        }
                        if (orConditionSentence == 0)
                        {
                            if (sentenceAsOutput == "")
                                searchFormat = searchFormat.Replace("{{" + andConditionId + "}}", "");
                            else
                                searchFormat = searchFormat.Replace("{{" + andConditionId + "}}", orConditionFormat);
                        }
                    }
                }
                copyResultOutputFormat = copyResultOutputFormat.Replace("{{" + searchId + "}}", searchFormat);
                finalOutputData = copyResultOutputFormat;
            }
        }

        public void buildFormat(JArray jaCompleteData, string finalOutputData, string resultOutputFormat, out string format)
        {
            format = "";
            var DocumentName = "";
            var SectionNumber = "";
            var SearchFor = "";
            var FoundText = "";
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                if (!DocumentName.Contains(jaCompleteData[i]["fileName"].ToString()))
                {
                    if (DocumentName == "")
                        DocumentName = DocumentName + jaCompleteData[i]["fileName"].ToString();
                    else
                        DocumentName = DocumentName + ", " + jaCompleteData[i]["fileName"].ToString();
                }
            }
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                if (!SectionNumber.Contains(jaCompleteData[i]["sectionNo"].ToString()))
                {
                    if (SectionNumber == "")
                        SectionNumber = SectionNumber + jaCompleteData[i]["sectionNo"].ToString();
                    else
                        SectionNumber = SectionNumber + "& " + jaCompleteData[i]["sectionNo"].ToString();
                }
            }
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                if (!SearchFor.Contains(jaCompleteData[i]["search"].ToString()))
                {
                    if (SearchFor == "")
                        SearchFor = SearchFor + jaCompleteData[i]["search"].ToString();
                    else
                        SearchFor = SearchFor + " and " + jaCompleteData[i]["search"].ToString();
                }
            }
            FoundText = finalOutputData;

            format = resultOutputFormat.Replace("{{DocumentName}}", DocumentName).Replace("{{searchFor}}", SearchFor).Replace("{{found text}}", FoundText).Replace("{{Paragraph Number}}", SectionNumber);
        }
    }
}