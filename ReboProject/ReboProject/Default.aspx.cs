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
            string abbreviationVal = LibVal.Text.ToString(); // get the value from front end
            if (backEndVal == "") // check if it has value else return
                return;
            var backendObject = JObject.Parse(backEndVal.ToString()); // complete  json
            var abbreviationObject = JObject.Parse(abbreviationVal);
            var multipleDatapointJson = backendObject["Datapoint"]; // get json for all datapoints
            var sectionLib = backendObject["sectionLib"]; // section library
            var collectSectionLib = "";
            foreach (var item in sectionLib)//get all the section
            {
                if (String.IsNullOrEmpty(collectSectionLib))
                    collectSectionLib = collectSectionLib + item;
                else
                    collectSectionLib = collectSectionLib + "|" + item;
            }
            Dictionary<string, string> AbbreviationData = new Dictionary<string, string>();
            for (var i=0; i< abbreviationObject["Abbreviation"].Count(); i++)
            {
                if (!AbbreviationData.ContainsKey(abbreviationObject["Abbreviation"][i]["keyword"].ToString()))
                    AbbreviationData.Add(abbreviationObject["Abbreviation"][i]["keyword"].ToString(), abbreviationObject["Abbreviation"][i]["replace"].ToString());
            }
            //---------------------------------- read all files----------------------------------
            var folder = backendObject["folder"].ToString();// folder path
            var folderPath = "";
            string drivePath = WebConfigurationManager.AppSettings["DrivePath"]; // get the drive path
            var userDefinePath = drivePath + folder;// get the project path
            string[] subdirectoryEntries = Directory.GetDirectories(userDefinePath);// get all the lease folder
            var listVal = new List<string>(subdirectoryEntries);
            subdirectoryEntries = listVal.ToArray();
            string[] pdfFiles = { };
            var filepath = ""; // full file path
            List<string> fileFullPath = new List<string>();
            Dictionary<string, string[]> folder_fileName = new Dictionary<string, string[]>();
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> savePageAllFiles = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save pagenumber and the lines in it
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> saveSectionNoAllFiles = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save section for each page
            Dictionary<string, Dictionary<Dictionary<int, string>, int>> saveAllSection = new Dictionary<string, Dictionary<Dictionary<int, string>, int>>();  // save section for each page
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> saveAllSectionRegex = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save section for each page
            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++)
            {
                folderPath = subdirectoryEntries[folderval]; // get the first folder
                var fornotAbstractedLease = folderPath.Split('\\');
                var fornotAbstractedLeaseName = fornotAbstractedLease[fornotAbstractedLease.Length - 1];
                pdfFiles = Directory.GetFiles(folderPath, "*.pdf").Select(Path.GetFileName).ToArray(); //get the pdf from folder
                folder_fileName.Add(folderPath, pdfFiles);// get the folder name and the files in it
                foreach (var fileNameVal in pdfFiles) // loop through all the pdfs
                {
                    Dictionary<int, Dictionary<int, string>> savePageSingleFile = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the para
                    Dictionary<int, Dictionary<int, string>> saveSectionNo = new Dictionary<int, Dictionary<int, string>>();  // save section for each para 
                    Dictionary<Dictionary<int, string>, int> saveSection = new Dictionary<Dictionary<int, string>, int>();  // save section for each para 
                    Dictionary<int, Dictionary<int, string>> savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>();
                    filepath = folderPath + "\\" + fileNameVal; // full path of file
                    fileFullPath.Add(filepath); // save the path of pdf read
                    pdfRead(collectSectionLib, filepath, out savePageSingleFile, out saveSectionNo, out saveSection, out savePageSectionRegex); // read pdf
                    saveAllSection.Add(filepath, saveSection);
                    saveAllSectionRegex.Add(filepath, savePageSectionRegex);
                    saveSectionNoAllFiles.Add(filepath, saveSectionNo); // entering section
                    savePageAllFiles.Add(filepath, savePageSingleFile); // entering para
                }
            }
            //-------------------------------------------------------------------------------------

            var finalOutput = new JArray();// save all output in it for frontend display

            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++)
            {
                foreach (var singleDp in multipleDatapointJson) // loop through all the datapoints
                {
                    
                    var resultSearch = singleDp["result"][0]["search"];// eg: {{filename}}; {{result}}: {{pagenumber}}
                    var sentenceStart = singleDp["result"][0]["startData"];// set all the sentences from all output configuration
                    var sentenceEnd = singleDp["result"][0]["endData"];// set all the sentences from all output configuration
                    var checkNextLine = singleDp["result"][0]["checkNextLine"].ToString();// set all the sentences from all output configuration
                    var SentenceResultOutputFormat = singleDp["result"][0]["SentenceoutputFormat"].ToString();// set all the sentences from all output configuration
                    var SentenceResultOutputFormatCondition = singleDp["result"][0]["FinalformatCondition"];// set all the sentences from all output configuration
                    var resultOutputFormat = singleDp["result"][0]["outputFormat"].ToString();// eg: final output format
                    var outputNotFoundMessage = singleDp["result"][0]["outputNotFoundMessage"].ToString();// eg: final output format
                    var resultAllKeyword = singleDp["result"][0]["allKeyword"];// list of all keywords used
                    var financialSelect = singleDp["result"][0]["financialSelect"];// list of all keywords used
                    var SectionNoCount = singleDp["SectionNoCount"].ToString();// list of all keywords used
                    var paraBreakCondition = singleDp["paraBreakCondition"].ToString();// list of all keywords used
                    var libraryVal = ""; // get all the library value
                    if ((singleDp["library"].ToString()) != "")
                        libraryVal = singleDp["library"].ToString();
                    string[] LibArr = libraryVal.Split('|');
                    var configuration = singleDp["Configuration"]; // get all the configuration
                    var configurationOrder = new Dictionary<int, int>();
                    for (var i = 0; i < configuration.Count(); i++) // loop through all the configuration to get the index
                    {
                        configurationOrder.Add(i + 1, (int)configuration[i]["Index"]);
                    }
                    var list = configurationOrder.Values.ToList();
                    list.Sort(); // sort to get the configuration order
                    var loopThroughNext = true;

                    var ja3 = new JArray(); // get the final output of one configuration from configuration
                    var jaLibCheck = new JArray(); // get the final output of one configuration from library
                    var LeaseName = ""; // get lease name

                    Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    Dictionary<int, Dictionary<int, string>> savePageSection = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    Dictionary<int, Dictionary<int, string>> savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    Dictionary<Dictionary<int, string>, int> getSaveSection = new Dictionary<Dictionary<int, string>, int>();  // save pagenumber and the lines in it
                    Dictionary<int, Dictionary<int, string>> savePageLib = new Dictionary<int, Dictionary<int, string>>();  // duplicate of savePage
                    Dictionary<int, string> OutputMatch = new Dictionary<int, string>();
                    Dictionary<int, string> PageNoMatch = new Dictionary<int, string>();
                    string getCorrectSentances = "";
                    JArray jaCompleteSentence = new JArray();
                    Dictionary<string, int> withInForSentence = new Dictionary<string, int>();
                    List<string> getParaForSentence = new List<string>();
                    Dictionary<string, string> getSectionAndFileName = new Dictionary<string, string>();
                    var getSectionAndFileNameAndSearchJA = new JArray();
                    var nextVal = 0;

                    var copyResultOutputFormat = SentenceResultOutputFormat;
                    var nextDuplicateCheck = 0;
                    var SearchWithinVal = 0;
                    for (var configurationVal = 0; configurationVal < configuration.Count(); configurationVal++)
                    {
                        // ------------------the data of the configuration to be read---------------------
                        var getSectionAndFileNameAndSearchJO = new JObject();
                        var getTheWithinDictionary = true;
                        var gotValueForConfiguration = false;
                        var myKey = configurationOrder.FirstOrDefault(x => x.Value == list[configurationVal]).Key;
                        var configurationIndexSelect = configurationOrder[myKey];
                        var connectorVal = (int)configuration[myKey - 1]["Connector"];// get the connector output to check Mandatory or library check
                        var datapointName = singleDp["DatapointName"].ToString(); // Datapoint 
                        var SearchWithin = (int)configuration[myKey - 1]["SearchWithin"];// 1=page, 2=section, 3=paragraph
                        SearchWithinVal = SearchWithin;
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
                        var fileNameForLib = "";
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
                                    fileNameForLib = jaLibCheck[0]["completeFilePath"].ToString();
                                    savePage = savePageAllFiles[fileNameForLib];
                                    savePageSection = saveSectionNoAllFiles[jaLibCheck[0]["completeFilePath"].ToString()];
                                    searchLibInPDF(getExclusion, exclusionCount, savePage, LibArr, datapointName, out pdfLibPara, out pdfLibPageno, out pdfLibParaNo, out pdfLibValFound);
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

                            var datapointID = (int)configuration[myKey - 1]["DpId"]; // Datapoint ID
                            var SectionName = configuration[myKey - 1]["SectionName"]; // Datapoint ID
                            var DefaultSectionName = configuration[myKey - 1]["DefaultSectionName"].ToString(); // Datapoint ID
                                                                                                                //var datapointName = configuration[myKey - 1]["Datapoint"].ToString(); // Datapoint ID
                            var sort = (int)configuration[myKey - 1]["FileOrder"]["sort"]; // asc or desc
                            var type = (int)configuration[myKey - 1]["FileOrder"]["type"]; // Single File Search or All File Search
                            var multipleRead = (int)configuration[myKey - 1]["MultipleRead"];  // to get score of multiple occurance
                            
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
                            fileToRead(folder_fileName, mainLeaseRead, fileFullPathToRead, sort, type, out fileDetails);
                            var FilePathPlusLeaseName = fileFullPathToRead.Split('\\');
                            //---------------------------------------------

                            LeaseName = FilePathPlusLeaseName[FilePathPlusLeaseName.Length - 1]; // get the lease name
                            var fileName = "";
                            float finalScore = 0;
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
                                    savePageSection = saveSectionNoAllFiles[fullFilePath];
                                    getSaveSection = saveAllSection[fullFilePath];
                                    savePageSectionRegex = saveAllSectionRegex[fullFilePath];

                                    getPageNo(savePage.Count(), startPage, endPage, out startPageVal, out endPageVal);
                                    List<Dictionary<Dictionary<string, int>, int>> dataSet = new List<Dictionary<Dictionary<string, int>, int>>();
                                    Dictionary<Dictionary<string, int>, int> getNode = new Dictionary<Dictionary<string, int>, int>();
                                    Dictionary<Dictionary<string, int>, int> getNodeRegex = new Dictionary<Dictionary<string, int>, int>();
                                    if (SearchWithin == 2)
                                    {
                                        dataSet = Program.getSectionVal(startPageVal, endPageVal, savePage, savePageSectionRegex);
                                        getNode = dataSet[0];
                                        getNodeRegex = dataSet[1];
                                        //var getVAl = Program.completeSection(getSaveSection, savePageSection, savePageSectionRegex);
                                        getAllFoundText(getNodeRegex, savePageSection, startPageVal, endPageVal, getNode, exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text
                                    }
                                    else
                                        getAllFoundText(getNodeRegex, savePageSection, startPageVal, endPageVal, getNode, exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text

                                    if (getTheWithinDictionary == true)
                                    { // get the within in Dictionary for sentence
                                        foreach (var item in withInScore)
                                        {
                                            if (!withInForSentence.ContainsKey(item.Key)) // check if dictionary has the same key
                                                withInForSentence.Add(item.Key, item.Value);
                                        }
                                    }

                                    //--------------------scoring and final output ---------------------------------------------------------------------
                                    scoring(PageNoMatch, datapointName, OutputMatch, LeaseName, savePage, totalScoreDenominatorVal, searchFieldScore, ja, multipleRead, out ja1, out finalScore);

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
                                var getTheCompleteSectionValue = "";
                                savePageSection = saveSectionNoAllFiles[ja2[0]["completeFilePath"].ToString()];
                                getSaveSection = saveAllSection[ja2[0]["completeFilePath"].ToString()];
                                var outputPara = "";
                                if (SearchWithin == 3) // para
                                {
                                    getTheCompleteSectionValue = processing.getCompleteParaSection(SectionNoCount, ja2, savePageSection, getSaveSection, outputPara, DefaultSectionName, SectionName); // get the section value
                                    ja2[0]["sectionVal"] = getTheCompleteSectionValue.Replace(",", " ");
                                }
                                if (SearchWithin == 2) // section
                                {
                                    var getCompleteSection = ja2[0]["output"].ToString();
                                    ja2[0]["sectionVal"] = DefaultSectionName + processing.sectionValSection(getCompleteSection);
                                }

                                if (!getParaForSentence.Contains(ja2[0]["Pageoutput"].ToString()))
                                {
                                    getSectionAndFileNameAndSearchJO["Pageoutput"] = ja2[0]["Pageoutput"].ToString();
                                    getSectionAndFileNameAndSearchJO["fileName"] = ja2[0]["fileName"].ToString();
                                    getSectionAndFileNameAndSearchJO["sectionNo"] = ja2[0]["sectionVal"].ToString();
                                    getSectionAndFileNameAndSearchJO["searchFor"] = ja2[0]["AllSearchFieldKeyword"].ToString();
                                    getSectionAndFileNameAndSearchJA.Add(getSectionAndFileNameAndSearchJO);
                                }
                            }

                            if (!ja3.HasValues && ja2.HasValues)
                            {
                                ja3.Add(ja2[0]);
                                jaLibCheck.RemoveAll();
                                jaLibCheck.Add(ja3[0]);
                                gotValueForConfiguration = true;
                                //saveDataToFolder(ja3, folderPath);
                                if (!OutputMatch.ContainsValue(ja2[0]["Pageoutput"].ToString()))
                                {
                                    OutputMatch.Add(nextDuplicateCheck, ja2[0]["Pageoutput"].ToString().Replace("|||. ", "").Replace("###", "").Trim());
                                    PageNoMatch.Add(nextDuplicateCheck, ja2[0]["sectionPageNos"].ToString().Trim());
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
                                    //saveDataToFolder(ja3, folderPath);
                                    if (!OutputMatch.ContainsValue(ja2[0]["Pageoutput"].ToString()))
                                    {
                                        OutputMatch.Add(nextDuplicateCheck, ja2[0]["Pageoutput"].ToString());
                                        PageNoMatch.Add(nextDuplicateCheck, ja2[0]["sectionPageNos"].ToString());
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
                                    PageNoMatch.Add(nextDuplicateCheck, pdfLibPageno[0].ToString());
                                    nextDuplicateCheck++;
                                    getSectionAndFileNameAndSearchJO["Pageoutput"] = pdfLibPara[0];
                                    getSectionAndFileNameAndSearchJO["fileName"] = fileNameForLib;
                                    getSectionAndFileNameAndSearchJO["sectionNo"] = "";
                                    getSectionAndFileNameAndSearchJO["searchFor"] = "";
                                    getSectionAndFileNameAndSearchJA.Add(getSectionAndFileNameAndSearchJO);
                                    ja3[0]["foundWithIn"] = ja3[0]["foundWithIn"].ToString() + " ||| Library";
                                    ja3[0]["AllSearchFieldKeyword"] = ja3[0]["AllSearchFieldKeyword"].ToString() + " ||| Library";
                                    ja3[0]["pageNo"] = ja3[0]["pageNo"].ToString() + " , " + pdfLibPageno[0];
                                    ja3[0]["score"] = ja3[0]["score"].ToString() + " ||| " + "Library";
                                    if (ja3[0]["Pageoutput"].ToString().EndsWith("."))
                                        ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + pdfLibPara[0];
                                    else
                                        ja3[0]["Pageoutput"] = ja3[0]["Pageoutput"].ToString() + ". " + pdfLibPara[0];
                                    //saveDataToFolder(ja3, folderPath);

                                    if (getCorrectSentances == "")
                                        getCorrectSentances = getCorrectSentances + " <b> (" + (configurationVal + 1) + ")</b>  " + pdfLibPara[0];
                                }
                            }
                        }
                        nextVal++;
                    }

                    // get complete output 
                    if (getSectionAndFileNameAndSearchJA.HasValues)
                    {
                        var finalOutputData = "";
                        JArray collectCorrectSentanceOutput = new JArray();
                        // get the filename, sectionNo and data....
                        collectCorrectSentance(paraBreakCondition, SearchWithinVal, checkNextLine, financialSelect, sentenceStart, sentenceEnd, SentenceResultOutputFormatCondition, getSectionAndFileNameAndSearchJA, withInForSentence, resultAllKeyword, resultSearch, copyResultOutputFormat, out finalOutputData, out collectCorrectSentanceOutput);
                        var format = "";
                        // set the complete format
                        buildFormat(outputNotFoundMessage, collectCorrectSentanceOutput, finalOutputData, resultOutputFormat, out format);
                        // save the format in json
                        string finalFormat = "";
                        abbreviationReplace(AbbreviationData, format, out finalFormat);
                        ja3[0]["correctString"] = finalFormat;
                    }

                    if (ja3.Count == 0) // if no output Found... display "Lease is silent"
                    {
                        var ja4 = new JArray();
                        var jo4 = new JObject();
                        jo4["output"] = outputNotFoundMessage;
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
                        //saveDataToFolder(ja4, folderPath);
                    }
                    finalOutput.Add(ja3[0]); // saves output of all the datapoint-lease
                    //if (ja3[0]["output"].ToString() != "Lease is silent" && ja3[0]["output"].ToString() != "lease is silent")
                    //    highlightPdf(ja3[0]["output"].ToString(), ja3[0]["pageNo"].ToString(), ja3[0]["completeFilePath"].ToString(), ja3[0]["dataPointName"].ToString());
                    
                    
                    
                    //---------------------------------------------------------------

                }
            }


            frontEndData.Text = finalOutput.ToString(); // set the data in front end
            watch.Stop(); // total time
            displayTime.Text = (watch.ElapsedMilliseconds / 60000).ToString() + " mins";
        }


        // get the score and the field to search
        public void getTotalScore(JToken withIn, JToken searchFor, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
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
                    if (mainLeaseRead == 0 && lowerFileNameVal.IndexOf("lease") == 0)
                        continue;

                    var filepath = folderPath + "\\" + fileNameVal; // full path of file
                    fileDetails.Add(filepath);
                }
            }
        }
        // get the paragraph lines
        public void pdfRead(string collectSectionLib, string filepath, out Dictionary<int, Dictionary<int, string>> savePage, out Dictionary<int, Dictionary<int, string>> savePageSection, out Dictionary<Dictionary<int, string>, int> saveSection, out Dictionary<int, Dictionary<int, string>> savePageSectionRegex)
        {
            savePage = new Dictionary<int, Dictionary<int, string>>();
            savePageSection = new Dictionary<int, Dictionary<int, string>>();
            savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>();
            saveSection = new Dictionary<Dictionary<int, string>, int>();
            var nextSection = 0;
            using (Doc doc = new Doc())
            {
                doc.Read(filepath);
                short PageIndex = 1;
                var lastSectionPageNo = 0;
                Dictionary<int, string> saveSectionPara = new Dictionary<int, string>();
                var lastLine = "";
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
                    Dictionary<int, string> saveSectionNo = new Dictionary<int, string>();
                    Dictionary<int, string> saveSectionNoRegex = new Dictionary<int, string>();
                    var i = 1;
                    
                    List<string> sectionNoCheck = new List<string>();
                    var lineCount = 0;
                    var searchFound = 0;
                    var nextPara = false;
                    var firstParaNoSection = false;
                    foreach (TextGroup lineGroup in ordereddGroups)
                    {
                        nextPara = false;
                        if ((prevRect.Bottom - lineGroup.Rect.Top) > 8.5)
                            nextPara = true;
                        lineCount++;
                        bool section = false;
                        checkSection(collectSectionLib, sb1.ToString(), out section);

                        if (section != true)
                            sectionNoCheck = processing.getSectionForPara(lineGroup.Text, lastLine, nextPara); // get the section value
                        else
                        {
                            sectionNoCheck.Add(null);
                            sectionNoCheck.Add(null);
                        }
                        if (nextPara == true || sectionNoCheck.ElementAt(0) != null || section == true)
                        { // current line is > 9 points lower than previous
                            searchFound++;
                            if (lastLine == "")
                                saveLines.Add(i, lineGroup.Text);
                            else
                                saveLines.Add(i, sb1.ToString());
                            // get section
                            if (searchFound == 1 & lineCount > 1)
                            {
                                saveSectionNo.Add(i, null);
                                saveSectionNoRegex.Add(i, null);
                                saveSectionNo.Add(i+1, sectionNoCheck[0]);
                                saveSectionNoRegex.Add(i+1, sectionNoCheck[1]);
                                firstParaNoSection = true;
                            }
                            else
                            {
                                if (firstParaNoSection == false)
                                {
                                    saveSectionNo.Add(i, sectionNoCheck[0]);
                                    saveSectionNoRegex.Add(i, sectionNoCheck[1]);
                                }
                                else
                                {
                                    saveSectionNo.Add(i+1, sectionNoCheck[0]);
                                    saveSectionNoRegex.Add(i+1, sectionNoCheck[1]);
                                }
                            }
                            
                            nextSection++;
                            if (section == true)
                            {
                                if (saveSectionPara.Count > 0) {
                                    
                                    saveSection.Add(saveSectionPara, PageIndex);
                                    saveSectionPara = new Dictionary<int, string>();
                                    lastSectionPageNo = PageIndex;
                                }
                                saveSectionPara.Add(nextSection, sb1.ToString());
                            }
                            else
                            {
                                saveSectionPara.Add(nextSection, sb1.ToString());
                            }
                            i++;
                            sb1.Clear();
                        }
                        if (ordereddGroups.Count == lineCount)
                        {
                            var getStringLength = Int32.Parse(WebConfigurationManager.AppSettings["StringLength"]);
                            if (lineGroup.Text.Length > getStringLength)
                                lastLine = lineGroup.Text;
                        }
                        else
                            lastLine = lineGroup.Text;

                        if (!lineGroup.Text.EndsWith(" "))
                            sb1.Append(lineGroup.Text + " ");
                        else
                            sb1.Append(lineGroup.Text);
                        prevRect.String = lineGroup.Rect.String;
                        lastSectionPageNo = PageIndex;
                    }
                    
                    saveLines.Add(i, sb1.ToString());
                    nextSection++;
                    saveSectionPara.Add(nextSection, sb1.ToString());
                    // get section
                    sectionNoCheck = processing.getSectionForPara(sb1.ToString(), lastLine, nextPara); // get the section value
                    if (firstParaNoSection == false)
                    {
                        saveSectionNo.Add(i, sectionNoCheck[0]);
                        saveSectionNoRegex.Add(i, sectionNoCheck[1]);
                    }
                    else
                    {
                        saveSectionNo.Add(i + 1, sectionNoCheck[0]);
                        saveSectionNoRegex.Add(i + 1, sectionNoCheck[1]);
                    }
                    firstParaNoSection = false;
                    savePage.Add(PageIndex, saveLines);
                    savePageSection.Add(PageIndex, saveSectionNo);
                    savePageSectionRegex.Add(PageIndex, saveSectionNoRegex);
                    PageIndex++;
                }
                if (saveSectionPara.Count > 0)
                    saveSection.Add(saveSectionPara, lastSectionPageNo);
            }
        }

        // check section value
        public void checkSection(string collectSectionLib, string para, out bool section)
        {
            section = false;
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            string[] getSingleLib = collectSectionLib.Split('|'); // split the lib
            foreach (var item in getSingleLib) // loop through all library
            {
                var sectionLibVal = JObject.Parse(item)["keyword"].ToString();
                MatchCollection matchData = null;
                if ((para).IndexOf("\"") == 0)
                {
                    var searchVal = (rgx.Replace(sectionLibVal, "\\$1")).Replace("\"", "");
                    matchData = Regex.Matches(para, "^(?i)[\"]" + searchVal + "[\"]([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]|\\d{0,2}[\\s])$"); // find match
                }
                else if (para.Trim().IndexOf("\"") != -1)
                    matchData = Regex.Matches(para.Trim(), @"^\b\s?(?i)" + rgx.Replace(sectionLibVal, "\\$1") + "(\\s|\\b)[\"]?(([a-zA-Z]{1}|\\d{0,3}|(\\W?))*)?[\"]?$");
                else
                    matchData = Regex.Matches(para, @"^\b\s?(?i)" + rgx.Replace(sectionLibVal, "\\$1") + "([a-zA-Z]{1}|\\d{1})?(\\s|\\b)([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]|\\d{0,2}[\\s])$");

                if (matchData.Count > 0)
                { // match found then is a section
                    section = true;
                    break;
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
        public void getAllFoundText(Dictionary<Dictionary<string, int>, int> getNodeRegex, Dictionary<int, Dictionary<int, string>> savePageSection, int startPageVal, int endPageVal, Dictionary<Dictionary<string, int>, int> getNode, int exclusionCount, string fullFilePath, int SearchWithin, Dictionary<int, Dictionary<int, string>> savePage, string fileName, JToken logic, out JArray ja, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
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
                            var AllSearchFieldscore = (getSearchFor[k]["score"]).ToString().ToLower(); // get the search field op
                            var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field op

                            bool checkAfterSubCaseSearchFor = true;
                            var pageCount = 0;
                            var nextPage = 0;
                            foreach (KeyValuePair<int, Dictionary<int, string>> entry in savePage) // get the page
                            {
                                var allParaSection = savePageSection.ElementAt(entry.Key - 1);
                                pageCount += 1;
                                if (pageCount < startPageVal)
                                    continue;
                                if (pageCount > endPageVal)
                                    break;
                                // saves the lineText
                                JArray ja1Val = new JArray();
                                StringBuilder sb3 = new StringBuilder();
                                var paraNumber = 0;
                                var nextPara = 0;
                                foreach (var checkPage in entry.Value) // each page value
                                {
                                    var nextParaSection = allParaSection.Value.ElementAt(nextPara);
                                    paraNumber = paraNumber + 1;
                                    processSearchForAndwithInParagraph(AllSearchFieldscore, savePage, entry, nextParaSection,paraNumber, checkPage, AllSearchFieldKeyword, rgx, AllSearchFieldCaseCheck, getSubCase,checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName,pageCount, fullFilePath, out ja1Val);
                                    if (ja1Val.HasValues)
                                        ja.Add(ja1Val[0]);
                                    nextPara++;
                                }
                                nextPage++;
                            }
                        }
                    }

                    if (SearchWithin == 2)
                    {
                        var count = 0;
                        foreach (var entry in getNode) // get the section
                        {
                            var sectionRegex = getNodeRegex.ElementAt(count).Key;
                            var sectionPageNo = entry.Value;
                            bool checkAfterSubCaseSearchFor = true;
                            JArray ja1Val = new JArray();
                            processSearchForAndwithInSection(sectionPageNo, savePage, sectionRegex, startPageVal, endPageVal, getSearchFor, entry, rgx, getSubCase, checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName, fullFilePath, out ja1Val);
                            if (ja1Val.HasValues)
                                ja.Add(ja1Val[0]);
                            count++;
                        }
                    }
                }
            }
        }

        public void processSearchForAndwithInParagraph(string AllSearchFieldscore, Dictionary<int, Dictionary<int, string>> savePage,KeyValuePair<int, Dictionary<int, string>> entry, KeyValuePair<int, string> nextParaSection, int paraNumber,KeyValuePair<int, string> checkPage, string AllSearchFieldKeyword, Regex rgx, string AllSearchFieldCaseCheck, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, int pageCount, string fullFilePath, out JArray ja)
        { 
            ja = new JArray();
            var readNextPara = 0;
            //paraNumber += 1;
            var getCurrentParaScore = 0;
            var sectionPageNos = "";
            var getCurrentParaSearchFor = "";
            var completeSectionText = "";
            var getLineText = checkPage.Value; // get the  line text
            var getSectionForPara = nextParaSection.Value;
            MatchCollection matchData;
            regexMatch(rgx, getLineText, AllSearchFieldKeyword, out matchData);
            var getStringLength = Int32.Parse(WebConfigurationManager.AppSettings["StringLength"]); // get the access to d ;
            var headingLength = Int32.Parse(WebConfigurationManager.AppSettings["headingLength"]);
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
                        var getInSamePara = false;
                        var checkNextWithIn = true;
                        var addNextPara = false;
                        var addNextParacheck = false;
                        for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                        {
                            if (checkNextWithIn == false)
                                break;
                            bool checkAfterSubCaseWithIn = true;
                            bool checkAfterSubCaseWithInExclusion = true;
                            var withInIt = (getWithIn[g]["keyword"]).ToString();
                            var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                            MatchCollection matchDataWithInIt;
                            // match function
                            regexMatch(rgx, SearchWithinText, withInIt, out matchDataWithInIt);

                            if (matchDataWithInIt.Count > 0) // if match there
                            {
                                getInSamePara = true;
                                if (withInCaseCheck == "yes")// check for cases
                                    subCaseSearch(SearchWithinText, withInIt, getSubCase, out checkAfterSubCaseWithIn);
                                if (checkAfterSubCaseWithIn == true)
                                {
                                    if (getExclusion.Count() > 0 && checkExclusion == true)
                                        exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out checkAfterSubCaseWithInExclusion);

                                    checkExclusion = false;
                                    if (checkAfterSubCaseWithInExclusion == true)
                                    {
                                        sectionPageNos = pageCount.ToString();
                                        countWithInInAPara += 1;
                                        addNextPara = true;
                                        acceptParaWithIn += (acceptParaWithIn == "") ? withInIt : "|" + withInIt;
                                    }
                                    else
                                        break;
                                }
                            }
                            else if (g + 1 == getWithIn.Count() && getInSamePara == false)
                            {
                                if (entry.Value.Count > paraNumber && entry.Value[paraNumber + 1].Length > getStringLength && SearchWithinText.Length <= headingLength)
                                {
                                    var getNextParaToCheck = "";
                                    if (SearchWithinText.Trim().EndsWith("."))
                                        getNextParaToCheck = SearchWithinText.Trim() + " " +entry.Value[paraNumber + 1];
                                    else
                                        getNextParaToCheck = SearchWithinText.Trim() + ". " + entry.Value[paraNumber + 1];
                                    //var getNextParaToCheck = SearchWithinText + entry.Value[paraNumber + 1];
                                    for (var h = 0; h < getWithIn.Count(); h++)
                                    {
                                        bool checkAfterSubCaseWithIn2 = true;
                                        bool checkAfterSubCaseWithInExclusion2 = true;
                                        var withInIt2 = (getWithIn[h]["keyword"]).ToString();
                                        var withInCaseCheck2 = (getWithIn[h]["caseCheck"]).ToString().ToLower();
                                        // match function
                                        regexMatch(rgx, getNextParaToCheck, withInIt2, out matchDataWithInIt);

                                        if (matchDataWithInIt.Count > 0) // if match there
                                        {
                                            if (withInCaseCheck2 == "yes")// check for cases
                                                subCaseSearch(getNextParaToCheck, withInIt2, getSubCase, out checkAfterSubCaseWithIn2);
                                            if (checkAfterSubCaseWithIn2 == true)
                                            {
                                                if (getExclusion.Count() > 0 && checkExclusion == true)
                                                    exclusionProcess(exclusionCount, getExclusion, getNextParaToCheck, out checkAfterSubCaseWithInExclusion2);

                                                checkExclusion = false;
                                                if (checkAfterSubCaseWithInExclusion2 == true)
                                                {
                                                    countWithInInAPara += 1;
                                                    acceptParaWithIn += (acceptParaWithIn == "") ? withInIt2 : "|" + withInIt2;
                                                    //paraNumber = paraNumber+1;
                                                    //getCurrentParaScore = Int32.Parse(AllSearchFieldscore);
                                                    SearchWithinText = getNextParaToCheck;
                                                    sectionPageNos = pageCount.ToString();
                                                    checkNextWithIn = false;
                                                    readNextPara = 1;
                                                    addNextParacheck = true;
                                                    //getCurrentParaSearchFor = AllSearchFieldKeyword;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else if(entry.Value.Count == paraNumber+1 && savePage.Count() > pageCount && getInSamePara == false)
                                {
                                    var hasSectionNo = Program.checkHasSectionNo(savePage[pageCount + 1][1]);
                                    if (hasSectionNo == false) {
                                        var getNextParaToCheck = SearchWithinText + savePage[pageCount + 1][1];
                                        // var getNextParaToCheck = savePage[pageCount + 1][1];
                                        for (var h = 0; h < getWithIn.Count(); h++)
                                        {
                                            bool checkAfterSubCaseWithIn2 = true;
                                            bool checkAfterSubCaseWithInExclusion2 = true;
                                            var withInIt2 = (getWithIn[h]["keyword"]).ToString();
                                            var withInCaseCheck2 = (getWithIn[h]["caseCheck"]).ToString().ToLower();
                                            // match function
                                            regexMatch(rgx, getNextParaToCheck, withInIt2, out matchDataWithInIt);
                                            var foundWithIn = false;
                                            if (matchDataWithInIt.Count > 0) // if match there
                                            {
                                                if (withInCaseCheck2 == "yes")// check for cases
                                                    subCaseSearch(getNextParaToCheck, withInIt2, getSubCase, out checkAfterSubCaseWithIn2);
                                                if (checkAfterSubCaseWithIn2 == true)
                                                {
                                                    if (getExclusion.Count() > 0 && checkExclusion == true)
                                                        exclusionProcess(exclusionCount, getExclusion, getNextParaToCheck, out checkAfterSubCaseWithInExclusion2);

                                                    checkExclusion = false;
                                                    if (checkAfterSubCaseWithInExclusion2 == true)
                                                    {
                                                        countWithInInAPara += 1;
                                                        acceptParaWithIn += (acceptParaWithIn == "") ? withInIt2 : "|" + withInIt2;
                                                        //paraNumber = 1;
                                                        SearchWithinText = getNextParaToCheck;
                                                        checkNextWithIn = false;
                                                        readNextPara = 1;
                                                        // getCurrentParaSearchFor = AllSearchFieldKeyword;
                                                    }
                                                    else
                                                        break;
                                                }
                                            }
                                            if (foundWithIn == true)
                                            {
                                                sectionPageNos= pageCount +"|"+ pageCount + 1;
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                        // take next sentence 
                        if (addNextPara == true | addNextParacheck == true)
                        {
                            var paraNoCopy = paraNumber;
                            if (addNextParacheck == true)
                                paraNoCopy = paraNumber + 1;
                            if (paraNoCopy + 1 <= entry.Value.Count())
                            {
                                if (paraNoCopy == entry.Value.Count - 1 & entry.Value[paraNoCopy + 1].Length < getStringLength & pageCount + 1 <= savePage.Count())
                                {
                                    var hasSectionNo = Program.checkHasSectionNo(savePage[pageCount + 1][1]);
                                    if (hasSectionNo == false)
                                    {
                                        SearchWithinText = SearchWithinText + savePage[pageCount + 1][1];
                                        sectionPageNos = pageCount + "|" + pageCount + 1;
                                        readNextPara = 1;
                                    }
                                }
                            }
                            else if (paraNoCopy == 1 && addNextParacheck == false) {
                                var hasSectionNo = Program.checkHasSectionNo(SearchWithinText);
                                if (hasSectionNo == false)
                                {
                                    if (savePage[pageCount - 1][savePage[pageCount - 1].Count()].Length > getStringLength)
                                    {
                                        var firstSentence = savePage[pageCount - 1][savePage[pageCount - 1].Count()];
                                        if (firstSentence.Length <= Int32.Parse(WebConfigurationManager.AppSettings["headingLength"]))
                                        {
                                            if (firstSentence.Trim().EndsWith("."))
                                                firstSentence = firstSentence.Trim() + " ";
                                            else
                                                firstSentence = firstSentence.Trim() + ". ";
                                        }
                                        SearchWithinText = firstSentence + SearchWithinText;
                                        sectionPageNos = pageCount-1 + "|" + pageCount;
                                        pageCount = pageCount - 1;
                                        paraNumber =savePage[pageCount].Count();
                                        readNextPara = 1;
                                    }
                                    else
                                    {
                                        var firstSentence = savePage[pageCount - 1][savePage[pageCount - 1].Count()-1];
                                        if (firstSentence.Length <= Int32.Parse(WebConfigurationManager.AppSettings["headingLength"]))
                                        {
                                            if (firstSentence.Trim().EndsWith("."))
                                                firstSentence = firstSentence.Trim() + " ";
                                            else
                                                firstSentence = firstSentence.Trim() + ". ";
                                        }
                                        SearchWithinText = firstSentence + SearchWithinText;
                                        sectionPageNos = pageCount-1 + "|" + pageCount;
                                        pageCount = pageCount -1;
                                        paraNumber =savePage[pageCount].Count() -1;
                                        readNextPara = 1;
                                    }
                                }
                            }
                        }
                        if (acceptParaWithIn != "")
                        {
                            gotResult = 1;
                            jarrayEnter(readNextPara,sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
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
                                jarrayEnter(readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                            }
                        }
                        else
                        {
                            gotResult = 1;
                            jarrayEnter(readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName.Split('.')[0], pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                        }
                    }
                }
            }
        }
        
        public void processSearchForAndwithInSection(int sectionPageNo, Dictionary<int, Dictionary<int, string>> savePage, Dictionary<string, int> sectionRegex, int startPageVal, int endPageVal, JToken getSearchFor, KeyValuePair<Dictionary<string, int>, int> entry, Regex rgx, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, string fullFilePath, out JArray ja)
        {
            ja = new JArray();
            var getCurrentParaScore = 0;
            var pageCount = entry.Value;
            var readNextPara = 0;
            var AllSearchFieldKeyword = "";
            var getCurrentParaSearchFor = "";
            var sectionPageNos = "";
            var acceptParaWithIn = "";
            List<string> saveAllPara = new List<string>();
            List<string> saveAllwithin = new List<string>();
            List<string> saveAllSearchFor = new List<string>();
            var getLineText = "";
            var paraNumber = 0;
            var completeSectionText = "";
            var keyCount = 0;
            foreach (var item in entry.Key)
            {
                
                //var myRegex = sectionRegex.FirstOrDefault(x => x.Value == keyCount).Key;
                var myRegex = sectionRegex.ElementAt(keyCount).Key;
                keyCount++;
                completeSectionText = completeSectionText + item.Key + "|||";
                if (paraNumber == 0)
                    paraNumber = item.Value;

                getLineText = item.Key; // get the  line text
                var checkNextWithIn = true;
                for (var k = 0; k < getSearchFor.Count(); k++)
                {
                    if (checkNextWithIn == false)
                        break;
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
                            var checkInNextPara = true;
                            if (getWithIn.Count() > 0)
                            {
                                var checkExclusion = true;
                                var checkCompleteSection = true;
                                var getWithInCount = 0;
                                for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                {
                                    if (checkNextWithIn == false)
                                        break;
                                    var withInIt = (getWithIn[g]["keyword"]).ToString();
                                    var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                                    MatchCollection matchDataWithInIt;
                                    // match function
                                    regexMatch(rgx, SearchWithinText, withInIt, out matchDataWithInIt);

                                    if (matchDataWithInIt.Count > 0) // if match there
                                    {
                                        getWithInCount++;
                                        checkNextWithIn = false;
                                        if (getWithInCount > 0)
                                        { 
                                            var getVAl = Program.completeSection(sectionPageNo, savePage, sectionRegex, entry.Key, getLineText, myRegex);
                                            getLineText = getVAl[0];
                                            sectionPageNos = getVAl[1];
                                            var firstPageNo = sectionPageNos.Split('|');
                                            pageCount = Int32.Parse(firstPageNo[0]);
                                            paraNumber = Int32.Parse(getVAl[2]);
                                        }
                                        

                                        SearchWithinText = getLineText;
                                        withInProcess(exclusionCount, AllSearchFieldKeyword, checkExclusion, getExclusion, getSubCase, SearchWithinText, rgx, getWithIn, out acceptParaWithIn,out saveAllPara,out saveAllSearchFor, out checkInNextPara, out checkCompleteSection);
                                        
                                    }
                                }
                                if (checkInNextPara == true)
                                {
                                    var getStringLength = Int32.Parse(WebConfigurationManager.AppSettings["StringLength"]);
                                    if (keyCount <= entry.Key.Count()-1) {
                                        SearchWithinText = entry.Key.ElementAt(keyCount).Key;
                                        if (getStringLength >= SearchWithinText.Length && keyCount+1 <= entry.Key.Count() - 1)
                                            SearchWithinText = entry.Key.ElementAt(keyCount + 1).Key;

                                        getWithInCount = 0;
                                        for (var g = 0; g < getWithIn.Count(); g++) // search for within fields
                                        {
                                            if (checkNextWithIn == false)
                                                break;
                                            var withInIt = (getWithIn[g]["keyword"]).ToString();
                                            var withInCaseCheck = (getWithIn[g]["caseCheck"]).ToString().ToLower();
                                            MatchCollection matchDataWithInIt;
                                            // match function
                                            regexMatch(rgx, SearchWithinText, withInIt, out matchDataWithInIt);

                                            if (matchDataWithInIt.Count > 0) // if match there
                                            {
                                                getWithInCount++;
                                                checkNextWithIn = false;
                                                if (getWithInCount >0)
                                                {
                                                    var getVAl = Program.completeSection(sectionPageNo, savePage, sectionRegex, entry.Key, getLineText, myRegex);
                                                    getLineText = getVAl[0];
                                                    sectionPageNos = getVAl[1];
                                                    var firstPageNo = sectionPageNos.Split('|');
                                                    pageCount = Int32.Parse(firstPageNo[0]);
                                                    paraNumber = Int32.Parse(getVAl[2]);
                                                }

                                                SearchWithinText = getLineText;
                                                withInProcess(exclusionCount, AllSearchFieldKeyword, checkExclusion, getExclusion, getSubCase, SearchWithinText, rgx, getWithIn, out acceptParaWithIn, out saveAllPara, out saveAllSearchFor, out checkInNextPara, out checkCompleteSection);
                                               
                                            }
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
                    if(finalPara.IndexOf(saveAllPara[i]) == -1)
                        finalPara = finalPara + saveAllPara[i];
                }

                getLineText = finalPara;
                gotResult = 1;
                var AllSearchFieldKeywordVal = "";
                foreach (var item in saveAllSearchFor)
                {
                    if(AllSearchFieldKeywordVal == "")
                        AllSearchFieldKeywordVal = AllSearchFieldKeywordVal + item;
                    else
                        AllSearchFieldKeywordVal = AllSearchFieldKeywordVal + "|" + item;
                }
                JArray ja1 = new JArray();
                string getSectionForPara = "";
                jarrayEnter(readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeywordVal, fileName, pageCount, getLineText, acceptParaWithIn, paraNumber, fullFilePath, out ja1);
                ja.Add(ja1[0]);
            }
        }

        public void withInProcess(int exclusionCount ,string AllSearchFieldKeyword, bool checkExclusion, JToken getExclusion,JToken getSubCase, string SearchWithinText,Regex rgx, JToken getWithIn, out string acceptParaWithIn,out List<string> saveAllPara,out List<string> saveAllSearchFor,out bool checkInNextPara, out bool checkCompleteSection)
        {
            acceptParaWithIn = "";
            checkCompleteSection = true;
            saveAllPara =new  List<string>();
            saveAllSearchFor = new List<string>();
            MatchCollection matchDataWithInIt;
            checkInNextPara = true;
            if (getWithIn.Count() > 0)
            {
                for (var h = 0; h < getWithIn.Count(); h++) // search for within fields
                {
                    bool checkAfterSubCaseWithIn1 = true;
                    bool checkAfterSubCaseWithInExclusion1 = true;
                    var withInIt1 = (getWithIn[h]["keyword"]).ToString();
                    var withInCaseCheck1 = (getWithIn[h]["caseCheck"]).ToString().ToLower();
                    // match function
                    regexMatch(rgx, SearchWithinText, withInIt1, out matchDataWithInIt);

                    if (matchDataWithInIt.Count > 0) // if match there
                    {
                        if (withInCaseCheck1 == "yes")// check for cases
                            subCaseSearch(SearchWithinText, withInIt1, getSubCase, out checkAfterSubCaseWithIn1);
                        if (checkAfterSubCaseWithIn1 == true)
                        {
                            if (getExclusion.Count() > 0 && checkExclusion == true)
                                exclusionProcess(exclusionCount, getExclusion, SearchWithinText, out checkAfterSubCaseWithInExclusion1);

                            checkExclusion = false;
                            if (checkAfterSubCaseWithInExclusion1 == true)
                            {
                                if(!saveAllPara.Contains(SearchWithinText))
                                    saveAllPara.Add(SearchWithinText);
                                acceptParaWithIn += (acceptParaWithIn == "") ? withInIt1 : "|" + withInIt1;
                                if (!saveAllSearchFor.Contains(AllSearchFieldKeyword))
                                    saveAllSearchFor.Add(AllSearchFieldKeyword);
                                checkInNextPara = false;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }

        // save data in jarray
        public void jarrayEnter(int readNextPara, string sectionPageNos, string getCurrentParaSearchFor, int getCurrentParaScore, string getSectionForPara, string completeSectionText, string AllSearchFieldKeyword, string fileName, int pageCount, string getLineText, string acceptParaWithIn, int paraNumber, string fullFilePath, out JArray ja)
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
            jo["getSectionForPara"] = getSectionForPara;
            jo["getCurrentParaScore"] = getCurrentParaScore;
            jo["getCurrentParaSearchFor"] = getCurrentParaSearchFor;
            jo["sectionPageNos"] = sectionPageNos;
            jo["readNextPara"] = readNextPara;
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

        public void scoring(Dictionary<int, string>  PageNoMatch, string datapointName, Dictionary<int, string> OutputMatch, string LeaseName, Dictionary<int, Dictionary<int, string>> savePage, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja, int multipleRead, out JArray ja1, out float finalScore)
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
                var getCurrentParaScore = getAllAcceptedText[l]["getCurrentParaScore"].ToString(); // get the page value to search all search fileds
                var getCurrentParaSearchFor = getAllAcceptedText[l]["getCurrentParaSearchFor"].ToString(); // get the page value to search all search fileds
                var scorePerSearch = 0;
                double finalScorePerSearch = 0;

                var setScoringKeyword = "";
                foreach (KeyValuePair<string, int> singleSearchFieldScore in searchFieldScore)
                { //  loop through all the search field

                    MatchCollection matchDataWithInIt;
                    regexMatch(rgx, pageContent, singleSearchFieldScore.Key, out matchDataWithInIt); // function to match

                    if (matchDataWithInIt.Count > 0) // if found the match
                    {
                        setScoringKeyword = setScoringKeyword + singleSearchFieldScore.Key + " | ";

                        if (multipleRead == 0)
                            scorePerSearch += singleSearchFieldScore.Value; // increment the score
                        else
                            scorePerSearch += (singleSearchFieldScore.Value * matchDataWithInIt.Count); // increment the score
                    }
                }
                if(getCurrentParaScore != "0" && pageContent.IndexOf(getCurrentParaSearchFor) == -1)
                {
                    setScoringKeyword = setScoringKeyword + " | " + getCurrentParaSearchFor;
                    scorePerSearch = scorePerSearch + Int32.Parse(getCurrentParaScore);
                }
                setScoringKeyword = "( " + setScoringKeyword + " )";
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
                    var sectionPageNos= getAllAcceptedText[entry.Key]["sectionPageNos"].ToString();
                    for (var i= 0;i < OutputMatch.Count(); i++) // check for duplicate... if the same sentance is already an output
                    {
                        var duplicateByPage = false;
                        if (sectionPageNos != "")
                        {
                            List<string> foundResultPage = new List<string>(PageNoMatch.ElementAt(i).Value.Trim().Split('|'));
                            List<string> currentResultPage = new List<string>(sectionPageNos.Trim().Split('|'));
                            List<string> duplicates = foundResultPage.Intersect(currentResultPage).ToList();
                            if (duplicates.Count > 0)
                            {
                                duplicateByPage = true;
                            }
                        }
                            
                        
                        if ((OutputMatch.ElementAt(i).Value.Trim() == pageContent.Trim() | OutputMatch.ElementAt(i).Value.Trim().IndexOf(pageContent.Trim()) != -1 | pageContent.Trim().IndexOf(OutputMatch.ElementAt(i).Value.Trim()) != -1) & duplicateByPage == true)
                            outputSame = true; // if true dont take that as output  and select the next output
                    }
                    // save the output for the file
                    if (onlyTopResult == true && outputSame == false)
                    { // get all the highest score value
                        var getAllScoringKeyword = saveScoringKeyword[entry.Key];
                        var foundText = getAllAcceptedText[entry.Key]["foundText"].ToString();
                        var sectionVal = getAllAcceptedText[entry.Key]["getSectionForPara"].ToString();
                        var pageCompleteContent = getAllAcceptedText[entry.Key]["pageContent"].ToString();
                        var AllSearchFieldKeyword1 = getAllAcceptedText[entry.Key]["AllSearchFieldKeyword"];
                        var paraNumber = (int)getAllAcceptedText[entry.Key]["paraNumber"];
                        var pageNo = (int)getAllAcceptedText[entry.Key]["pageNo"];
                        var fileNameVal = getAllAcceptedText[entry.Key]["fileName"];
                        var completeFilePathVal = getAllAcceptedText[entry.Key]["completeFilePath"];
                        var withInValFound = getAllAcceptedText[entry.Key]["foundWithIn"];
                        var readNextPara = getAllAcceptedText[entry.Key]["readNextPara"];
                        finalScore = entry.Value;

                        var jo1 = new JObject();
                        jo1["output"] = foundText;
                        jo1["Pageoutput"] = foundText;
                        jo1["AllSearchFieldKeyword"] = AllSearchFieldKeyword1;
                        jo1["fileName"] = fileNameVal;
                        jo1["pageNo"] = pageNo;
                        jo1["paraNo"] = paraNumber;
                        jo1["score"] = finalScore + "%" + " - " + getAllScoringKeyword;
                        jo1["foundWithIn"] = withInValFound;
                        jo1["pageContent"] = pageCompleteContent;
                        jo1["leaseName"] = LeaseName;
                        jo1["completeFilePath"] = completeFilePathVal;
                        jo1["sectionVal"] = sectionVal;
                        jo1["dataPointName"] = datapointName;
                        jo1["readNextPara"] = readNextPara;
                        jo1["sectionPageNos"] = sectionPageNos;
                        ja1.Add(jo1);
                        onlyTopResult = false;
                    }
                }
            }
        }

        // save all data found in 
        //public void saveDataToFolder(JArray jArray, string folderPath)
        //{

        //    var saveDataFolder = folderPath + "\\output";
        //    var outputFilePath = saveDataFolder + "\\result.txt";

        //    if (!Directory.Exists(saveDataFolder))//if output folder not exist
        //        Directory.CreateDirectory(saveDataFolder);//create folder
        //    else if (File.Exists(outputFilePath))
        //        File.Delete(outputFilePath);

        //    var pathString = System.IO.Path.Combine(saveDataFolder, "result.txt"); // create the input file

        //    var dataMain = new JArray();
        //    for (var i = 0; i < jArray.Count; i++)
        //    {
        //        var data = new JObject();
        //        data["fileName"] = jArray[i]["fileName"].ToString();
        //        data["searchField"] = jArray[i]["AllSearchFieldKeyword"].ToString();
        //        data["pageNoVal"] = jArray[i]["pageNo"].ToString();
        //        data["sectionVal"] = jArray[i]["sectionVal"].ToString();
        //        dataMain.Add(data);
        //    }
        //    File.WriteAllText(pathString, dataMain.ToString());

        //}

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
                                if (checkAfterSubCaseWithInExclusion == true)
                                { // save all the data of the para
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
        public void collectCorrectSentance(string paraBreakCondition, int searchWithInVal,string checkNextLineVal, JToken financialSelect, JToken sentenceStart, JToken sentenceEnd, JToken SentenceResultOutputFormatCondition, JArray getSectionAndFileNameAndSearchJA, Dictionary<string, int> withInForSentence, JToken resultAllKeyword, JToken resultSearch, string copyResultOutputFormat, out string finalOutputData, out JArray collectCorrectSentanceOutput)
        {
            finalOutputData = "";
            collectCorrectSentanceOutput = new JArray();
            Dictionary<string, string> getAllTheSentence = new Dictionary<string, string>();
            Dictionary<string, string> getAllTheParagraph = new Dictionary<string, string>();
            List<string> paragraphSection = new List<string>();
            List<string> paragraphFileName = new List<string>();
            List<string> sentenceSection = new List<string>();
            List<string> sentenceFileName = new List<string>();
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
            if(searchWithInVal == 2)
            {
                foreach (var item in getSectionAndFileNameAndSearchJA)
                {
                    string[] getSentance = item["Pageoutput"].ToString().Split(new string[] { "|||" }, StringSplitOptions.None);
                    for (int i = 0; i < getSentance.Count(); i++)
                    {
                        if (!getAllTheSentence.ContainsKey(getSentance[i])) {
                            getAllTheSentence.Add(getSentance[i], item["searchFor"].ToString());
                            sentenceSection.Add(item["sectionNo"].ToString());
                            sentenceFileName.Add(item["fileName"].ToString());
                        }
                    }
                }
            }
            if (searchWithInVal == 3) {
                foreach (var item in getSectionAndFileNameAndSearchJA)
                {
                    getAllTheParagraph.Add(item["Pageoutput"].ToString(), item["searchFor"].ToString());
                    paragraphSection.Add(item["sectionNo"].ToString());
                    paragraphFileName.Add(item["fileName"].ToString());

                    string[] finalBreak = { };
                    string[] getSentanceColon = item["Pageoutput"].ToString().Split(new string[] { ""+ paraBreakCondition .Trim()+ " " }, StringSplitOptions.None);
                    string[] getSentanceFullStop = item["Pageoutput"].ToString().Split(new string[] { ". " }, StringSplitOptions.None);
                    if (paraBreakCondition.Trim() == "")
                    {
                        if (getSentanceFullStop[getSentanceFullStop.Count() - 1] == "" | getSentanceFullStop[getSentanceFullStop.Count() - 1] == " ")
                            getSentanceFullStop = getSentanceFullStop.Take(getSentanceFullStop.Count() - 1).ToArray();
                        List<string> wrongStrings = new List<string>();
                        List<string> y = getSentanceFullStop.ToList<string>();
                        y.RemoveAll(p => string.IsNullOrEmpty(p));
                        getSentanceFullStop = y.ToArray();
                        for (int i = 1; i < getSentanceFullStop.Count(); i++)
                        {
                            var trimString = getSentanceFullStop[i].Trim();
                            var nextLine = false;
                            Regex regex = new Regex("^[\"|'|(]");
                            var match = regex.Match(trimString); // check if match found
                            if (match.Success)
                            {
                                nextLine = true;
                            }
                            if (!char.IsUpper(trimString[0]) & nextLine == false)
                            {
                                wrongStrings.Add(getSentanceFullStop[i]);
                            }
                        }
                        for (int k = 0; k < wrongStrings.Count(); k++)
                        {
                            if (Array.IndexOf(getSentanceFullStop, wrongStrings[k]) - 1 != -1)
                                getSentanceFullStop[Array.IndexOf(getSentanceFullStop, wrongStrings[k]) - 1] = getSentanceFullStop[Array.IndexOf(getSentanceFullStop, wrongStrings[k]) - 1] + ". " + getSentanceFullStop[Array.IndexOf(getSentanceFullStop, wrongStrings[k])];
                            var listVal = new List<string>(getSentanceFullStop);
                            listVal.Remove(getSentanceFullStop[Array.IndexOf(getSentanceFullStop, wrongStrings[k])]);
                            getSentanceFullStop = listVal.ToArray();
                        }
                        finalBreak = getSentanceFullStop;
                    }
                   

                    else if (paraBreakCondition.Trim() != "")
                    {
                        List<string> finalSentences = new List<string>();
                        for (int i = 0; i < getSentanceColon.Count(); i++)
                        {
                            string[] splitString = getSentanceColon[i].Split(new string[] { ". " }, StringSplitOptions.None);
                            List<string> splitOnFullStop = splitString.ToList<string>();
                            List<string> wrongStringsFullStop = new List<string>();
                            splitOnFullStop.RemoveAll(p => string.IsNullOrEmpty(p));
                            for (var u = 0; u < splitOnFullStop.Count(); u++)
                            {
                                var trimString = splitOnFullStop[u].Trim();
                                var nextLine = false;
                                Regex regex = new Regex("^[\"|'|(|\\d]");
                                var match = regex.Match(trimString); // check if match found
                                if (match.Success)
                                {
                                    nextLine = true;
                                }
                                if (char.IsUpper(trimString[0]) | nextLine == true)
                                {
                                    finalSentences.Add(trimString);
                                }
                                
                            }
                        }
                        getSentanceColon = finalSentences.ToArray();
                        finalBreak = getSentanceColon;
                    }

                    string[] getSentance = finalBreak;
                    foreach (var sentenceVal in getSentance)
                    {
                        var dupliSentenceVal = sentenceVal.Replace("|||. ", "");
                        foreach (var withIn in withInForSentence) // loop through all the within
                        {
                            MatchCollection matchData = null;
                            regexMatch(rgx, dupliSentenceVal, withIn.Key, out matchData); // function to match

                            if (matchData.Count > 0) // if found add the score of that within
                            {
                                if (!getAllTheSentence.ContainsKey(dupliSentenceVal))
                                {
                                    getAllTheSentence.Add(dupliSentenceVal, item["searchFor"].ToString());
                                    sentenceSection.Add(item["sectionNo"].ToString());
                                    sentenceFileName.Add(item["fileName"].ToString());
                                }
                            }
                        }
                    }
                }
            }
            
            List<string> saveSentenceForDuplicate = new List<string>();
            List<string> sentenceStartList = new List<string>();
            List<string> sentenceEndList = new List<string>();
            foreach (var item in sentenceStart)
            {
                sentenceStartList.Add(item["keyword"].ToString());
            }
            foreach (var item in sentenceEnd)
            {
                sentenceEndList.Add(item["keyword"].ToString());
            }

            Dictionary<string, string> allSetKeyword = new Dictionary<string, string>();
            foreach (var item in resultAllKeyword)
            {
                allSetKeyword.Add(item["id"].ToString(), item["keyword"].ToString());
            }
            var searchValCount = 0;
            Dictionary<int, string> allFormatSave = new Dictionary<int, string>();
            foreach (var searchVal in resultSearch)
            {
                searchValCount++;
                var searchId = (int)searchVal["id"];
                var searchAndCondition = searchVal["andCondition"];
                var formatCondition = searchVal["formatCondition"];
                var searchFormat = searchVal["format"].ToString();
                var searchFormatCOpy = searchFormat;
                var searchExclusion = searchVal["exclusion"];
                Dictionary<string, string> getSearchExclusion = new Dictionary<string, string>();
                foreach (var item in searchExclusion)
                {
                    getSearchExclusion.Add(item["keyword"].ToString(), item["Check"].ToString());
                }
                List<int> allContionToReplace = new List<int>();
                List<string> stringOutput = new List<string>();
                foreach (var andConditionVal in searchAndCondition)
                {
                    var andConditionId = (int)andConditionVal["id"];
                    var defaultFormat = andConditionVal["defaultFormat"].ToString();
                    var financialCheck = andConditionVal["financialCheck"].ToString();
                    if (!allContionToReplace.Contains(andConditionId))
                        allContionToReplace.Add(andConditionId);
                    var andConditionOrCondition = andConditionVal["orCondition"];
                    var outputGet = andConditionVal["outputGet"].ToString();
                    //var outputGet = "1";
                    var checkNextOrCondition = true;
                    var searchFormatCopy = searchFormat;
                    if(outputGet == "1")
                        readSentenceOrParagraph(defaultFormat, searchFormatCopy, sentenceEndList, financialCheck, financialSelect, sentenceStartList, collectCorrectSentanceOutput, andConditionId, allFormatSave, sentenceSection, sentenceFileName, checkNextLineVal, saveSentenceForDuplicate, rgx, getSearchExclusion, getAllTheSentence, allSetKeyword, checkNextOrCondition, andConditionOrCondition, out stringOutput, out searchFormat);
                    if (outputGet == "2")
                        readSentenceOrParagraph(defaultFormat,searchFormatCopy, sentenceEndList, financialCheck, financialSelect, sentenceStartList, collectCorrectSentanceOutput, andConditionId, allFormatSave, paragraphSection, paragraphFileName, checkNextLineVal, saveSentenceForDuplicate, rgx, getSearchExclusion, getAllTheParagraph, allSetKeyword, checkNextOrCondition, andConditionOrCondition, out stringOutput, out searchFormat);
                }

                var displayConstant = "";
                var formatContion = "";
                var gotVal = false;
                if (formatCondition != null)
                {
                    for (var i = 0; i < formatCondition.Count(); i++)
                    {
                        formatContion = formatCondition[i]["condition"].ToString();
                        var id = formatCondition[i]["id"].ToString();
                        string[] getCondition = formatContion.Split('|'); // break the contion if multiple
                        var count = 0;
                        for (int k = 0; k < getCondition.Count(); k++) // loop through multiple condition in one
                        {
                            if (searchFormatCOpy.IndexOf(getCondition[k]) != -1)
                            { // check if that condition is ther in format string 

                                if (Int32.Parse(getCondition[k].Replace("{", "").Replace("}", "").Trim()) - 1 > stringOutput.Count())
                                {
                                    displayConstant = formatCondition[i]["value"][0]["fail"].ToString();
                                    gotVal = true;
                                    break;
                                }
                                else if (stringOutput[Int32.Parse(getCondition[k].Replace("{", "").Replace("}", "").Trim()) - 1] != null) // to check if there and output for there
                                {
                                    var allCondition = formatCondition[i]["value"][0]["success"][0]["condition"];
                                    if (formatCondition[i]["value"][0]["success"][0]["condition"].HasValues)
                                    {
                                        for (var conditionVal = 0; conditionVal < allCondition.Count(); conditionVal++)
                                        {
                                            var getData = formatCondition[i]["value"][0]["success"][0]["condition"][conditionVal];
                                            if (stringOutput[i] != null && stringOutput[i] == getData["value"].ToString())
                                            {
                                                displayConstant = getData["display"].ToString();
                                                count++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        displayConstant = formatCondition[i]["value"][0]["success"][0]["display"].ToString();
                                        count++;
                                    }
                                }
                                else
                                {
                                    displayConstant = formatCondition[i]["value"][0]["fail"].ToString();
                                    gotVal = true;
                                    break;
                                }
                            }
                        }
                        if (gotVal == true || count == getCondition.Count())
                            searchFormat = searchFormat.Replace("##" + id + "##", displayConstant).Trim();
                    }
                }

                foreach (var contion in allContionToReplace)
                {
                    searchFormat = searchFormat.Replace("{{" + contion + "}}", "").Trim();
                }

                stringOutput = new List<string>();

                allFormatSave.Add(searchValCount, searchFormat);
                
                if (allFormatSave.Count() == resultSearch.Count())
                {
                    var searchFormatJoin = new Dictionary<int, string>();
                    for (var i = 0; i < SentenceResultOutputFormatCondition.Count(); i++) // get all the cobdition 
                    {
                        var id = SentenceResultOutputFormatCondition[i]["id"].ToString(); // get the id
                        var condition = SentenceResultOutputFormatCondition[i]["condition"].ToString(); // get the set on which condition is dependent
                        var success = SentenceResultOutputFormatCondition[i]["success"].ToString(); // success output
                        var fail = SentenceResultOutputFormatCondition[i]["fail"].ToString(); // fail output
                        string[] getCondition = condition.Split('|'); // break the contion if multiple
                        var count = 0;
                        for (var j = 0; j < getCondition.Count(); j++)
                        {
                            if (copyResultOutputFormat.IndexOf(getCondition[j]) != -1)
                            {
                                if (allFormatSave.ContainsKey(Int32.Parse(getCondition[j].Replace("{", "").Replace("}", "").Trim())))
                                {
                                    if (allFormatSave[Int32.Parse(getCondition[j].Replace("{", "").Replace("}", "").Trim())] != "")
                                        count++;
                                }
                            }
                        }
                        if (count == getCondition.Count()) // save the output
                            searchFormatJoin.Add(i, success);
                        else
                            searchFormatJoin.Add(i, fail);
                    }
                    foreach (var item in searchFormatJoin) // loop throucg all the output
                    {
                        copyResultOutputFormat = copyResultOutputFormat.Replace("##" + (item.Key + 1) + "##", item.Value);
                    }
                    foreach (var item in allFormatSave)
                    {
                        copyResultOutputFormat = copyResultOutputFormat.Replace("{{" + item.Key + "}}", item.Value);
                        finalOutputData = copyResultOutputFormat;
                    }
                }
            }
        }

        public void readSentenceOrParagraph(string defaultFormat, string searchFormatCopy, List<string> sentenceEndList,string financialCheck, JToken financialSelect,List<string> sentenceStartList,JArray collectCorrectSentanceOutput, int andConditionId,Dictionary<int,string> allFormatSave,List<string> sentenceSection, List<string> sentenceFileName,string checkNextLineVal, List<string> saveSentenceForDuplicate,Regex rgx,Dictionary<string,string> getSearchExclusion, Dictionary<string, string> getAllTheSentence, Dictionary<string, string> allSetKeyword, bool checkNextOrCondition,JToken andConditionOrCondition, out List<string> stringOutput ,out string searchFormat)
        {
            stringOutput = new List<string>();
            searchFormat = "";
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
                Dictionary<string, string> getAllTheSentenceWithExclusion = new Dictionary<string, string>();
                foreach (var singleSentenceVal in getAllTheSentence)
                {
                    if (getSearchExclusion.Count > 0)
                    {
                        var exclusionPresent = false;
                        foreach (var item in getSearchExclusion)
                        {
                            MatchCollection matchDataKey;
                            regexMatch(rgx, singleSentenceVal.Key, item.Key, out matchDataKey);
                            MatchCollection matchDataValue;
                            regexMatch(rgx, singleSentenceVal.Key, item.Value, out matchDataValue);
                            if (matchDataKey.Count > 0 && matchDataValue.Count > 0)
                            {
                                exclusionPresent = true;
                                break;
                            }
                        }
                        if(exclusionPresent == false)
                            getAllTheSentenceWithExclusion.Add(singleSentenceVal.Key, singleSentenceVal.Value);
                    }
                    else
                        getAllTheSentenceWithExclusion = getAllTheSentence;

                }
                var conditionCount = 0;
                var checkConditionCount = false;
                foreach (var singleSentence in getAllTheSentenceWithExclusion)
                {
                    if (checkNextSentence == false)
                        break;
                    var count = 0;
                    var CheckGetKeywordCount = 0;
                    foreach (var toFIndVal in getKeyword)
                    {
                        CheckGetKeywordCount++;
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
                        else if (toFIndVal == "%")
                        {
                            regexToFInd = @"(%)";
                        }
                        else if (toFIndVal == "$")
                        {
                            regexToFInd = @"([$]+)";
                        }
                        else
                        {
                            regexToFInd = @"\b\s?" + rgx.Replace(toFIndVal, "\\$1") + "(\\s|\\b)";
                        }
                        Regex regex = new Regex(regexToFInd);
                        var match = regex.Match(singleSentence.Key); // check if match found
                        var checkDuplicate = true;
                        if (match.Success)
                        {
                            if (orConditionFormat.IndexOf("##" + andConditionId + "##") != -1)
                            {
                                conditionCount++;
                                checkConditionCount = true;
                            }
                            var checkNextLIne = false;
                            if (checkDuplicate == true && saveSentenceForDuplicate.Count > 0)
                            {
                                if (CheckGetKeywordCount == getKeyword.Count() & orConditionCondition == 1) {
                                    foreach (var item in saveSentenceForDuplicate)
                                    {
                                        if (singleSentence.Key.Trim() == item && checkNextLineVal == "1")
                                        {
                                            checkNextLIne = true;
                                            break;
                                        }
                                    }
                                }
                                if (orConditionCondition == 0) {
                                    foreach (var item in saveSentenceForDuplicate)
                                    {
                                        if (singleSentence.Key.Trim() == item && checkNextLineVal == "1")
                                        {
                                            checkNextLIne = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (checkNextLIne == false)
                            {
                                JObject collectCorrectSentanceOutputJO = new JObject();
                                count++;
                                var getFileName = sentenceFileName.ElementAt(next).Split('-')[0];
                                var fileNameVal = sentenceFileName.ElementAt(next).Replace(getFileName + "-", "").Trim();
                                if (orConditionCondition == 1 && count == getKeyword.Count())
                                {
                                    sentenceAsOutput = singleSentence.Key;
                                    collectCorrectSentanceOutputJO["sentence"] = singleSentence.Key;
                                    saveSentenceForDuplicate.Add(singleSentence.Key);
                                    collectCorrectSentanceOutputJO["sectionNo"] = sentenceSection.ElementAt(next);
                                    //if (fileNameVal.ToLower().IndexOf("lease") == 0)
                                    //    collectCorrectSentanceOutputJO["fileName"] = "";
                                    //else
                                        collectCorrectSentanceOutputJO["fileName"] = fileNameVal;
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
                                    saveSentenceForDuplicate.Add(singleSentence.Key);
                                    collectCorrectSentanceOutputJO["sectionNo"] = sentenceSection.ElementAt(next);
                                    //if (fileNameVal.ToLower().IndexOf("lease") == 0)
                                    //    collectCorrectSentanceOutputJO["fileName"] = "";
                                    //else
                                        collectCorrectSentanceOutputJO["fileName"] = fileNameVal;
                                    collectCorrectSentanceOutputJO["search"] = singleSentence.Value;
                                    collectCorrectSentanceOutput.Add(collectCorrectSentanceOutputJO);
                                    checkNextOrCondition = false;
                                    checkNextSentence = false;
                                    break;
                                }
                            }
                        }
                    }
                    next++;
                }
                var finalOutputSentence = "";
                var duplicateFound = false;
                foreach (var item in allFormatSave)
                {
                    var outputStringVal = item.Value;
                    if (outputStringVal.IndexOf(sentenceAsOutput.Trim()) != -1 && sentenceAsOutput != "")
                    {
                        duplicateFound = true;
                        break;
                    }
                }
                if (searchFormatCopy.IndexOf(sentenceAsOutput) == -1 && duplicateFound == false)
                    startToEnd(sentenceStartList, sentenceEndList, sentenceAsOutput, out finalOutputSentence);
                List<string> financialVal = new List<string>();
                if (duplicateFound == false)
                {
                    if (financialCheck != "0")
                    {
                        foreach (var item in financialSelect)
                        {
                            var financialId = item["id"].ToString();
                            if (financialId == financialCheck)
                            {
                                var financialKeyword = item["keyword"].ToString();
                                if (financialKeyword == "getDate")
                                    financialVal = processing.getDate(finalOutputSentence);
                                if (financialKeyword == "getCurrencyAmount")
                                    financialVal = processing.getCurrencyAmount(finalOutputSentence);
                                if (financialKeyword == "getYear")
                                    financialVal = processing.getYear(finalOutputSentence);
                                if (financialKeyword == "extractPercentage")
                                    financialVal = processing.extractPercentage(finalOutputSentence);
                                if (financialKeyword == "getDays")
                                    financialVal = processing.getDays(finalOutputSentence);
                                if (financialKeyword == "getYearCount")
                                    financialVal = processing.getYearCount(finalOutputSentence);
                            }
                        }
                    }
                }
                if (orConditionCondition == 1 && conditionCount == getKeyword.Count() && checkConditionCount == true && financialVal.Count() == 0)
                {
                    orConditionFormat = defaultFormat;
                }
                else if (orConditionCondition == 0 && checkConditionCount == true && financialVal.Count() == 0)
                {
                    orConditionFormat = defaultFormat;
                }

                if (orConditionSentence == 1)
                {
                    if (sentenceAsOutput != "")
                    {
                        if (orConditionFormat != "")
                        {
                            if (orConditionFormat.IndexOf("{{" + andConditionId + "}}") != -1)
                            {
                                // financial
                                if (financialVal.Count() != 0)
                                    orConditionFormat = orConditionFormat.Replace("##" + andConditionId + "##", financialVal[0].ToString());
                                else
                                    orConditionFormat = orConditionFormat.Replace("##" + andConditionId + "##", "");

                                if (searchFormatCopy.IndexOf(sentenceAsOutput) == -1 && duplicateFound == false)
                                    orConditionFormat = orConditionFormat.Replace("{{" + andConditionId + "}}", finalOutputSentence);
                                else
                                    orConditionFormat = orConditionFormat.Replace("{{" + andConditionId + "}}", "");

                                stringOutput.Add(orConditionFormat);
                                searchFormatCopy = searchFormatCopy.Replace("{{" + andConditionId + "}}", orConditionFormat);
                            }
                            else
                            {
                                finalOutputSentence = finalOutputSentence + " " + orConditionFormat;
                                // financial
                                if (financialVal.Count() != 0)
                                    finalOutputSentence = finalOutputSentence.Replace("##" + andConditionId + "##", financialVal[0].ToString());
                                else
                                    finalOutputSentence = finalOutputSentence.Replace("##" + andConditionId + "##", "");

                                stringOutput.Add(finalOutputSentence);
                                searchFormatCopy = searchFormatCopy.Replace("{{" + andConditionId + "}}", finalOutputSentence);
                            }

                        }
                        else
                        {
                            if (searchFormatCopy.IndexOf(finalOutputSentence) == -1 && duplicateFound == false)
                            {
                                stringOutput.Add(finalOutputSentence);
                                searchFormatCopy = searchFormatCopy.Replace("{{" + andConditionId + "}}", finalOutputSentence);
                            }
                            else
                                searchFormatCopy = searchFormatCopy.Replace("{{" + andConditionId + "}}", "");
                        }
                    }
                }
                if (orConditionSentence == 0)
                {
                    if (sentenceAsOutput != "")
                    {
                        // financial
                        if (financialVal.Count() != 0)
                            orConditionFormat = orConditionFormat.Replace("##" + andConditionId + "##", financialVal[0].ToString());
                        else
                            orConditionFormat = orConditionFormat.Replace("##" + andConditionId + "##", "");

                        orConditionFormat = orConditionFormat.Replace("{{" + andConditionId + "}}", "").Trim();
                        stringOutput.Add(orConditionFormat);
                        searchFormatCopy = searchFormatCopy.Replace("{{" + andConditionId + "}}", orConditionFormat);
                    }

                }
                if (stringOutput.Count() <= andConditionId - 1)
                {
                    stringOutput.Add(null);
                }
                searchFormat = searchFormatCopy;
            }

        }
        
        //bulid the final format
        public void buildFormat(string outputNotFoundMessage, JArray jaCompleteData, string finalOutputData, string resultOutputFormat, out string format)
        {
            format = "";
            var DocumentName = "";
            var SearchFor = "";
            var FoundText = "";
            List<string> fileNme = new List<string>();
            List<string> fileNmeCopy = new List<string>();
            List<string> sectionNumberList = new List<string>();
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                fileNme.Add(jaCompleteData[i]["fileName"].ToString());
                fileNmeCopy.Add(jaCompleteData[i]["fileName"].ToString());
            }
            
            List<string> finalFormatList = new List<string>();
            var finalFormat = "";
            List<string> onlySection = new List<string>();
            List<string>  onlyFileName = new List<string>();
            formatDesign(fileNme, fileNmeCopy, jaCompleteData, out finalFormatList, out onlySection, out onlyFileName);
            if (resultOutputFormat.IndexOf("({{DocumentName}} {{Paragraph Number}})") != -1)
            {
                foreach (var item in finalFormatList)
                {
                    if (finalFormat == "")
                        finalFormat = finalFormat + item;
                    else
                        finalFormat = finalFormat + " & " + item;
                }
            }

            else if(resultOutputFormat.IndexOf("{{DocumentName}}") != -1 | resultOutputFormat.IndexOf("{{Paragraph Number}}") != -1)
            {
                foreach (var item in onlyFileName)
                {
                    if (item.ToLower().IndexOf("lease") != 0) {
                        if (DocumentName == "")
                            DocumentName = DocumentName + item;
                        else
                            DocumentName = DocumentName + "," + item;
                    }
                }
                foreach (var item in onlySection)
                {
                    if (finalFormat == "")
                        finalFormat = finalFormat + item;
                    else
                        finalFormat = finalFormat + "," + item;
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
            
            if (FoundText == "" & finalFormat == "")
                format = outputNotFoundMessage;
            else
                format = resultOutputFormat.Replace("{{searchFor}}", SearchFor).Replace("{{found text}}", FoundText).Replace("({{DocumentName}} {{Paragraph Number}})", finalFormat).Replace("{{Paragraph Number}}", finalFormat).Replace("{{DocumentName}}", DocumentName);
        }

        // complete format for ({{DocumentName}} {{Paragraph Number}})
        public void formatDesign(List<string>  fileNme, List<string> fileNmeCopy, JArray jaCompleteData, out List<string> finalFormatList, out List<string> onlySection, out List<string> onlyFileName)
        {
            finalFormatList = new List<string>();
            onlySection = new List<string>();
            onlyFileName = new List<string>();
            List<string> allSection = new List<string>();
            List<string> allSectionName = new List<string>();
            List<string> allSectionNameSplit = new List<string>();
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                var sectionVal = jaCompleteData[i]["sectionNo"].ToString();
                Regex regexArticle = new Regex("(?i)(article)");
                var matchArticle = regexArticle.Match(sectionVal); // check if match found
                if (matchArticle.Success)
                    sectionVal = sectionVal.Replace(matchArticle.Value, "").Trim();
                Regex regexSection = new Regex("(?i)(section)");
                var matchSection = regexSection.Match(sectionVal); // check if match found
                if (matchSection.Success)
                    sectionVal = sectionVal.Replace(matchSection.Value, "").Trim();
                var splitSectionVal = sectionVal.Split(' ');
                List<string> splitSectionValList = new List<string>(splitSectionVal);
                splitSectionValList = splitSectionValList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                splitSectionVal = splitSectionValList.Select(l => l.ToString()).ToArray();
                allSectionName.Add(sectionVal);
                allSectionNameSplit.Add(splitSectionVal[0]);
                sectionVal = "";
                for (var k = 1; k < splitSectionValList.Count; k++)
                {
                    sectionVal = sectionVal + " " + splitSectionValList.ElementAt(k);
                }
                allSection.Add(sectionVal.Trim());
            }
            fileNmeCopy = fileNmeCopy.Distinct().ToList();
            
            List<string> commonFile = new List<string>();
            List<string> commonSectionname = new List<string>();

            for (int p = 0; p < fileNmeCopy.Count(); p++)
            {
                Dictionary<string, string[]> saveSectionVal = new Dictionary<string, string[]>();
                Dictionary<string, string> saveArtAndSection = new Dictionary<string, string>();
                Dictionary<string, string> savesectionAndFile = new Dictionary<string, string>();
                List<string> finalSections = new List<string>();
                var singleFormat = "";
                var fileToCheck = fileNmeCopy.ElementAt(p).Trim();
                for (int f = 0; f < fileNme.Count(); f++)
                {
                    if (fileToCheck == fileNme.ElementAt(f).Trim())
                    {
                        if (!saveSectionVal.ContainsKey(allSection.ElementAt(f)))
                        {
                            var allSectionNameCopy = allSectionName.ElementAt(f).Trim().Split(' ');
                            allSectionNameCopy = allSectionNameCopy.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            allSectionNameCopy = allSectionNameCopy.Skip(1).ToArray();
                            if (allSectionNameCopy.Count() > 0)
                            {
                                saveSectionVal.Add(allSection.ElementAt(f), allSectionNameCopy);
                                saveArtAndSection.Add(allSection.ElementAt(f), allSectionNameSplit.ElementAt(f));
                                savesectionAndFile.Add(allSection.ElementAt(f), fileNme.ElementAt(f).Trim());
                            }
                        }
                    }
                }
                List<string> saveResult = new List<string>();
                var sectionCount = 0;
                var sectionName = "";
                Dictionary<string, string> secondCommonSet = new Dictionary<string, string>();
                if (saveSectionVal.Values.Count() > 0)
                {
                    var loopup = saveSectionVal.ToLookup(x => x.Value.ElementAt(0), x => x.Key).Where(x => x.Count() > 1);
                    foreach (var item in loopup)
                    {
                        if (item.Key != "")
                        {
                            var commonData = item.Key;
                            var KeyVal = item.Aggregate("", (s, v) => s + "," + v);
                            var splitSection = KeyVal.Split(',');
                            splitSection = splitSection.Skip(1).ToArray();

                            for (int i = 0; i < splitSection.Count(); i++)
                            {
                                var sectionString = splitSection[i];
                                if (saveSectionVal[sectionString].Count() > 2)
                                    secondCommonSet.Add(sectionString, (commonData + " " + saveSectionVal[sectionString].ElementAt(1)).Trim());
                            }
                            var secondloopup = secondCommonSet.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() > 1);
                            foreach (var seconditem in secondloopup)
                            {
                                if (seconditem.Key != "")
                                    commonData = seconditem.Key;
                            }
                            var addNextVal = new List<string>();
                            foreach (var checkNext in splitSection)
                            {
                                var check = checkNext.Replace(commonData, "");
                                if (check != "")
                                    addNextVal.Add(check);
                            }
                            if (addNextVal.Count != 0 & addNextVal.Count() != splitSection.Count())
                                break;
                            var sectionSaveFormat = "";
                            sectionName = "";
                            for (int i = 0; i < splitSection.Count(); i++)
                            {
                                if (splitSection.ElementAt(i) != "")
                                {
                                    saveResult.Add(splitSection.ElementAt(i));
                                    sectionCount++;
                                    var sectionReplace = splitSection.ElementAt(i).Replace(commonData, "");
                                    if (sectionSaveFormat == "")
                                        sectionSaveFormat = sectionSaveFormat + sectionReplace;
                                    else if (i != 0 && i < splitSection.Count() - 1)
                                        sectionSaveFormat = sectionSaveFormat + ", " + sectionReplace;
                                    else
                                        sectionSaveFormat = sectionSaveFormat + " & " + sectionReplace;
                                }
                                if (i == 0)
                                {
                                    sectionName = saveArtAndSection[splitSection.ElementAt(i)];
                                    commonSectionname.Add(sectionName);
                                }
                            }
                            finalSections.Add(sectionName + " " + commonData + " (" + sectionSaveFormat + ")");
                        }
                    }
                }
                if (sectionCount == 0)
                {
                    List<string> leaseNameThere = new List<string>();
                    var sortedDectionary = saveArtAndSection.OrderByDescending(pair => pair.Value);
                    foreach (var item in sortedDectionary)
                    {
                        if (item.Key != "")
                        {
                            var sectionNameVal = saveArtAndSection[item.Key];

                            if (!finalSections.Contains(item.Key))
                            {
                                if (leaseNameThere.Contains(sectionNameVal))
                                    finalSections.Add(item.Key);
                                else
                                {
                                    finalSections.Add(sectionNameVal + " " + item.Key);
                                    leaseNameThere.Add(sectionNameVal);
                                }

                            }
                        }
                    }
                }
                if (sectionCount > 0)
                {
                    foreach (var item in saveSectionVal)
                    {
                        if (!saveResult.Contains(item.Key) & item.Key != "")
                        {
                            var sectionNameVal = saveArtAndSection[item.Key];
                            var fileNameval = savesectionAndFile[item.Key];

                            if (!finalSections.Contains(item.Key))
                            {
                                if (commonSectionname.Contains(fileNameval))
                                    finalSections.Add(item.Key);
                                else
                                    finalSections.Add(sectionNameVal + " " + item.Key);
                            }
                        }
                    }
                }

                foreach (var item in finalSections)
                {
                    if (singleFormat == "")
                        singleFormat = singleFormat + item;
                    else
                        singleFormat = singleFormat + " & " + item;
                }

                var fileNameToDisplay = "";
                if (fileToCheck.ToLower().IndexOf("lease") == 0)
                {
                    fileNameToDisplay = "";
                    finalFormatList.Add((fileNameToDisplay + " " + singleFormat).Trim());
                }
                else
                {
                    fileNameToDisplay = fileToCheck;
                    finalFormatList.Add((fileNameToDisplay + " " + singleFormat).Trim());
                }
                onlySection.Add(singleFormat);
                onlyFileName.Add(fileNameToDisplay);
            }
        }

        // cut the sentence on start word and end word 
        public void startToEnd(List<string> sentenceStartList, List<string> sentenceEndList, string tocheck, out string output)
        {
            output = "";
            var tocheckLength = 0;
            var checkNextSentence = true;
            var checkEndCondition = "";
            for (int i = 0; i < sentenceStartList.Count(); i++) // loop through all start words
            {
                if (checkNextSentence == false)
                    break;
                Regex regexStart = new Regex("(?i)(" + sentenceStartList[i] + ")");
                var matchStart = regexStart.Match(tocheck); // check if match found
                if (matchStart.Success)
                {
                    var startIndex = tocheck.IndexOf(matchStart.Value);
                    tocheck = tocheck.Remove(0, startIndex);
                    tocheckLength = tocheck.Length;
                    checkNextSentence = false;
                }
                checkEndCondition = tocheck;
            }
            checkNextSentence = true;
            for (int j = 0; j < sentenceEndList.Count(); j++) // loop through all end words
            {
                if (checkNextSentence == false)
                    break;
                Regex regexEnd = new Regex("(?i)(" + sentenceEndList[j] + ")");
                var matchEnd = regexEnd.Match(checkEndCondition); // check if match found
                if (matchEnd.Success)
                {
                    var endLength = matchEnd.Value.Length;
                    var endIndex = checkEndCondition.IndexOf(matchEnd.Value);
                    tocheckLength = checkEndCondition.Length;
                    checkEndCondition = checkEndCondition.Remove(endIndex + endLength, tocheckLength - (endIndex + endLength));
                    checkNextSentence = false;
                }
                output = checkEndCondition;
            }
            if (output == "")
                output = tocheck;
        }

        //public void highlightPdf(string output, string pageNo, string filePath, string dataPointName)
        //{
        //    var fileName = "";
        //    var folderName = "";
        //    string[] getFolder = filePath.Split(new string[] { "\\" }, StringSplitOptions.None);
        //    fileName = getFolder[getFolder.Length - 1];
        //    folderName = getFolder[getFolder.Length - 2];
        //    getFolder = getFolder.Take(getFolder.Count() - 1).ToArray();
        //    getFolder = getFolder.Take(getFolder.Count() - 1).ToArray();

        //    List<string> y = getFolder.ToList<string>();
        //    y.RemoveAll(p => string.IsNullOrEmpty(p));
        //    getFolder = y.ToArray();
        //    var newPath = "";
        //    foreach (var item in getFolder)
        //    {
        //        //if (newPath != "")
        //        //    newPath = newPath + "\\" + item;
        //        //else
        //            newPath = newPath + item;
        //        break;
        //    }
        //    var hightlightFolder = newPath + "\\highlight";
        //    if (!Directory.Exists(hightlightFolder))//if output folder not exist
        //        Directory.CreateDirectory(hightlightFolder);//create folder
        //    if (!Directory.Exists(hightlightFolder + "\\" + folderName))//if output folder not exist
        //        Directory.CreateDirectory(hightlightFolder + "\\" + folderName);//create folder



        //    if (File.Exists(hightlightFolder + "\\" + folderName + "\\" + dataPointName + "-" + fileName))
        //    {
        //        File.Delete(hightlightFolder + "\\" + folderName + "\\" + dataPointName + "-" + fileName);
        //    }
        //    var newFilePath = hightlightFolder + "\\" + folderName + "\\" + dataPointName + "-" + fileName;

        //    string[] singlePageNo = pageNo.Split(',');
        //    string[] singlePara = output.Split(new string[] { "###" }, StringSplitOptions.None);
        //    using (Doc doc = new Doc())
        //    {
        //        doc.Read(filePath);
        //        for (int i = 0; i < singlePageNo.Count(); i++)
        //        {
        //            TextOperation op = new TextOperation(doc);
        //            XRect _xrect1 = new XRect();
        //            _xrect1.String = "0 0 612 1000";//default to A4 size
        //            XRect[] _xrect = new XRect[] { };
        //            op.PageContents.AddPages(Int32.Parse(singlePageNo[i].Trim()));
        //            string[] test = op.GetText(_xrect1, Int32.Parse(singlePageNo[i].Trim())).Split(new string[] { "\r\n" }, StringSplitOptions.None);
        //            var _text = replaceSplChar(op.GetText(_xrect1, Int32.Parse(singlePageNo[i].Trim())));
        //            for (int j = 0; j < test.Count(); j++)
        //            {
        //                int pos = 0;
        //                if (singlePara[i].IndexOf(test[j].Trim()) != -1)
        //                {
        //                    var searchText = replaceSplChar(test[j]);
        //                    var pattern = searchText.Trim().Replace(" ", "[\\s]{1,}"); //replace all spaces;
        //                    var matches = System.Text.RegularExpressions.Regex.Matches(_text, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));  // matches found text with the page
        //                    //if (pos != 0) {
        //                        pos = matches[0].Captures[0].Index; // get index of matched found text
        //                        IList<TextFragment> theSelection = op.Select(pos, test[j].Trim().Length);
        //                        IList<TextGroup> theGroups = op.Group(theSelection);
        //                        theSelection = op.Select(pos, searchText.Length + ((theGroups.Count - 1) * 2));//add the new lines in the length of the string
        //                        theGroups = op.Group(theSelection);//get the groups again...
        //                        foreach (TextGroup theGroup in theGroups)
        //                        {
        //                            if (string.IsNullOrEmpty(theGroup.Text.Trim()))
        //                                continue;
        //                            doc.Rect.String = theGroup.Rect.String;
        //                            doc.Color.String = "255 255 0";
        //                            doc.Color.Alpha = 100;// change shade for found text
        //                            doc.FillRect();
        //                        }
        //                    //}
        //                }
        //            }
        //        }
        //        doc.Save(newFilePath);
        //    }
        //}

        
        private string replaceSplChar(string text)// replace special character before highlight...
        {
            var specialChar = "(,),*,\",^,#,$,!,@,%,~,{,},[,],\\,_,`,:,;,+,=,-,>,/";// get specail chars from web config
            var specialCharArr = specialChar.Split(','); // split by ','
            for (var i = 0; i < specialCharArr.Length; i++) // loop through special char
            {
                text = text.Replace(specialCharArr[i].ToString(), " "); // replace each special char with space
            }
            return text;
        }

        // check abbreviation and replace
        public void abbreviationReplace(Dictionary<string,string> AbbreviationData,string format, out string finalFormat)
        {
            finalFormat = "";
            foreach (var item in AbbreviationData) // loop through all the abbreviation 
            {
                format = Regex.Replace(format, "(?i)" + item.Key + "(?!\\S)", item.Value, RegexOptions.IgnoreCase); // replace all the occurance of the abbreviation 
            }
            finalFormat = format;
        }

    }
}