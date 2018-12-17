//using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WebSupergoo.ABCpdf10;
using WebSupergoo.ABCpdf10.Operations;

namespace ReboProject
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e) 
        {
        }
        processing processing = new processing();
        #region  -----------------------------------------financial list -------------------------------------------------

        static List<string> year = new List<string> { "per year", "Per Annum", "Annual Base Rent" };

        static List<string> month = new List<string> { "per month", "Monthly Base Rent", "Monthly Rent" };

        static List<string> squareFoot = new List<string> { "Sq. Ft.", "per RSF", "psf", "Square Foot" };

        static List<string> perSquareFoot = new List<string> { "Sq. Ft.", "per RSF", "psf", "Square Foot" };

        static List<string> duration = new List<string> { "Months", "Lease Months", "Lease Period", "Commencement Date", "lease year" };

        #endregion

        protected void TestClick(object sender, EventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch(); // start time of process
            watch.Start();
            
             string backEndVal = backEndData.Text; // get the data from front end (JSON)
            string abbreviationVal = LibVal.Text.ToString(); // get the json from front end abbreviation
            if (backEndVal == "") // check if it has value else return
                return;
             var backendObject = JObject.Parse(backEndVal.ToString()); // parse json
            var abbreviationObject = JObject.Parse(abbreviationVal); // parse abbreviation

            var multipleDatapointJson = backendObject["Datapoint"]; // all datapoints json 
            //section Libary is used to divided the lease into section so that we can get the correct section number for the para
            var CompleteSectionLib = backendObject["sectionLib"];
            /* this is used to break the sentence ...
             if the 1st sentence ends with and the 2nd sentence starts with endkeyword and startkeyword resp. 
            then consider both the sentence as one sentence */
            var abbreviationCheckData = backendObject["abbreviationCheck"];

            #region ------------get all the section library and save --------------------------

            var collectSectionLib = string.Join("|",CompleteSectionLib);
            
            //foreach (var item in CompleteSectionLib) // loop through all the section library and save in collectSectionLib
            //{
            //    if (String.IsNullOrEmpty(collectSectionLib))
            //        collectSectionLib = collectSectionLib + item;
            //    else
            //        collectSectionLib = collectSectionLib + "|" + item;
            //}
            #endregion

            #region---------------save all the abbreviation in dictionary-----------------

            Dictionary<string, string> AbbreviationData = new Dictionary<string, string>();
            //Dictionary<string, string> AbbreviationData = abbreviationObject["Abbreviation"].ToDictionary(s=>new Dictionary<string, string> { {s["keyword"].ToString(),s["replace"].ToString() } });

            //var abbreviationlist = abbreviationObject["Abbreviation"].Select(s=>new Dictionary<string, string> { { s["keyword"].ToString(), s["Abbreviation"].ToString() } } ).Distinct();
            for (var i = 0; i < abbreviationObject["Abbreviation"].Count(); i++) // save the abbreviation in dictionary 
            {
                if (!AbbreviationData.ContainsKey(abbreviationObject["Abbreviation"][i]["keyword"].ToString())) // dont save duplicate
                    AbbreviationData.Add(abbreviationObject["Abbreviation"][i]["keyword"].ToString(), abbreviationObject["Abbreviation"][i]["replace"].ToString());
            }
            #endregion

            #region ---------------------------------- read all files and save the data----------------------------------

            #region ----------get the folder details -----------------------

            var folder = backendObject["folder"].ToString();// parent folder path 
            var projName = backendObject["ProjectName"].ToString(); // project Name (will be used for getting the excel from excel template folder)
            var folderPath = ""; // used to save the folder of each lease
            string drivePath = WebConfigurationManager.AppSettings["DrivePath"]; // get the drive path from  "webConfig"
            var userDefinePath = drivePath + folder;// get the project path
            string[] subdirectoryEntries = Directory.GetDirectories(userDefinePath);// get all the lease folder
            var listVal = new List<string>(subdirectoryEntries);// save it in list
            subdirectoryEntries = listVal.ToArray(); // save that list in string array
            string[] pdfFiles = { }; // get all the pdf of the lease from the folder
            var filepath = ""; // full file path
            #endregion

            #region ----------------------PDF READ process --------------------------------

            List<string> fileFullPath = new List<string>();
            Dictionary<string, string[]> folder_fileName = new Dictionary<string, string[]>();
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> savePageAllFiles = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save pagenumber and the lines in it
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> saveSectionNoAllFiles = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save section number for each para each page
            Dictionary<string, Dictionary<Dictionary<int, string>, int>> saveAllSection = new Dictionary<string, Dictionary<Dictionary<int, string>, int>>();  // save section 
            Dictionary<string, Dictionary<int, Dictionary<int, string>>> saveAllSectionRegex = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();  // save regex for each section for each para each page
            Dictionary<string, string> sectionTree = new Dictionary<string, string>();  // save section for each page
            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++) // loop through all the lease
            {
                folderPath = subdirectoryEntries[folderval]; // get the folder of the lease
                var fornotAbstractedLease = folderPath.Split('\\');
                var fornotAbstractedLeaseName = fornotAbstractedLease[fornotAbstractedLease.Length - 1];
                pdfFiles = Directory.GetFiles(folderPath, "*.pdf").Select(Path.GetFileName).ToArray(); //get the pdf from folder
                folder_fileName.Add(folderPath, pdfFiles);// get the folder name and the files in it
                foreach (var fileNameVal in pdfFiles) // loop through all the pdfs
                {
                    Dictionary<int, Dictionary<int, string>> savePageSingleFile = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the para
                    Dictionary<int, Dictionary<int, string>> saveSectionNo = new Dictionary<int, Dictionary<int, string>>();  // save section number for each para 
                    Dictionary<Dictionary<int, string>, int> saveSection = new Dictionary<Dictionary<int, string>, int>();  // save section 
                    Dictionary<int, Dictionary<int, string>> savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>(); // save section number regex
                    var finalJson = "";
                    filepath = folderPath + "\\" + fileNameVal; // full path of file
                    fileFullPath.Add(filepath); // save the path of pdf read
                    pdfRead(fileNameVal, collectSectionLib, filepath, out savePageSingleFile, out saveSectionNo, out saveSection, out savePageSectionRegex, out finalJson); // read pdf
                    saveAllSection.Add(filepath, saveSection); // entering section
                    sectionTree.Add(filepath, finalJson);
                    saveAllSectionRegex.Add(filepath, savePageSectionRegex); //entering regex
                    saveSectionNoAllFiles.Add(filepath, saveSectionNo); // entering section number
                    savePageAllFiles.Add(filepath, savePageSingleFile); // entering para
                }
            }
            #endregion

            #endregion

            var finalOutput = new JArray();// save all output in it for frontend display

            List<string> saveDatapointName = new List<string>();

            #region -----------------------Loop through all the leases in the folder------------------------------

            for (var folderval = 0; folderval < subdirectoryEntries.Length; folderval++) // loop through all the leases for the correct paragraph
            {
                var saveLeaseDataForExcel = new JArray();// save all output in it for frontend display

                #region ------------------get all financial data globally defined--------------------------------

                    Dictionary<string[], string> saveFinancial = new Dictionary<string[], string>();
                    var saveFinancialCount = 0;
                    Dictionary<string, List<string>> financialLibrary = new Dictionary<string, List<string>>();
                    financialLibrary.Add("psf", perSquareFoot);
                    financialLibrary.Add("sf", squareFoot);
                    financialLibrary.Add("year", year);
                    financialLibrary.Add("month", month);
                    //financialLibrary.Add("duration", duration);

                    Dictionary<string, string[]> saveFinancialCopy = new Dictionary<string, string[]>();

                #endregion

                #region -----------------------run all the datapoints for the lease---------------------------
                foreach (var singleDp in multipleDatapointJson) // loop through all the datapoints
                {
                    var resultSearch = singleDp["result"][0]["search"];// get all andCondition for output
                    var sentenceStart = singleDp["result"][0]["startData"];// list of start word for output
                    var sentenceEnd = singleDp["result"][0]["endData"];// list of end word for output
                    var checkNextLine = singleDp["result"][0]["checkNextLine"].ToString();// check duplicate line or not for output
                    var SentenceResultOutputFormat = singleDp["result"][0]["SentenceoutputFormat"].ToString();// output format for combining all andCOndition
                    var SentenceResultOutputFormatCondition = singleDp["result"][0]["FinalformatCondition"];// formay condition for output format
                    var andConditionCheck = singleDp["result"][0]["andConditionCheck"];// condition for andCondition 
                    var resultOutputFormat = singleDp["result"][0]["outputFormat"].ToString();// final output format
                    var outputNotFoundMessage = singleDp["result"][0]["outputNotFoundMessage"].ToString();// output message if no output found
                    var resultAllKeyword = singleDp["result"][0]["allKeyword"];// list of all keywords used
                    var financialSelect = singleDp["result"][0]["financialSelect"];// list of all financial
                    var SectionNoCount = singleDp["SectionNoCount"].ToString();// section number count
                    //var FinancialData = (int)singleDp["FinancialData"];// financial datapoint 
                    var paraBreakCondition = singleDp["paraBreakCondition"].ToString();// set of all para break condition for output
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

                    var ja3 = new JArray(); // get the final output of one configuration from configuration
                    var jaLibCheck = new JArray(); // get the final output of one configuration from library
                    var LeaseName = ""; // get lease name

                    Dictionary<int, Dictionary<int, string>> savePage = new Dictionary<int, Dictionary<int, string>>();  // save pagenumber and the lines in it
                    Dictionary<int, Dictionary<int, string>> savePageSection = new Dictionary<int, Dictionary<int, string>>();  // save section if connectior value is 2 (library)
                    Dictionary<int, Dictionary<int, string>> savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>();  // save regex for line
                    Dictionary<Dictionary<int, string>, int> getSaveSection = new Dictionary<Dictionary<int, string>, int>();  // save section
                    Dictionary<int, Dictionary<int, string>> savePageLib = new Dictionary<int, Dictionary<int, string>>();  // duplicate of savePage
                    var singleFileSectionTree = "";
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
                    var datapointName = singleDp["DatapointName"].ToString(); // Datapoint 
                    if (!saveDatapointName.Contains(datapointName))
                        saveDatapointName.Add(datapointName);
                    


                    for (var configurationVal = 0; configurationVal < configuration.Count(); configurationVal++)
                    {
                        // ------------------the data of the configuration to be read---------------------
                        var getSectionAndFileNameAndSearchJO = new JObject();
                        var getTheWithinDictionary = true;
                        var gotValueForConfiguration = false;
                        var myKey = configurationOrder.FirstOrDefault(x => x.Value == list[configurationVal]).Key;
                        var configurationIndexSelect = configurationOrder[myKey];
                        var connectorVal = (int)configuration[myKey - 1]["Connector"];// get the connector output to check Mandatory or library check
                        
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
                            var SectionName = configuration[myKey - 1]["SectionName"]; // Section name to display
                            var DefaultSectionName = configuration[myKey - 1]["DefaultSectionName"].ToString(); // default section name
                            var sort = (int)configuration[myKey - 1]["FileOrder"]["sort"]; // asc or desc
                            var type = (int)configuration[myKey - 1]["FileOrder"]["type"]; // Single File Search or All File Search
                            var multipleRead = (int)configuration[myKey - 1]["MultipleRead"];  // to get score of multiple occurance                            
                            var mainLeaseRead = (int)configuration[myKey - 1]["MainLeaseRead"];// skip main lease read
                            var pageNoRange = configuration[myKey - 1]["PageNoRange"];// page number range
                            var startPage = pageNoRange[0]["startRange"].ToString();// start range
                            var readDuplicate = "";
                            var endPage = pageNoRange[0]["endRange"].ToString(); // end range
                            var startPageVal = 0;
                            var endPageVal = 0;

                            Dictionary<string, int> searchFieldScore = new Dictionary<string, int>(); // search filed score
                            Dictionary<string, int> withInScore = new Dictionary<string, int>(); // withinScore
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
                            
                            // ------------------------------------input process-----------------------------------------------------
                            foreach (string fullFilePath in fileDetails) // loop through all the files
                            {
                                if (readNextFile == 1) // check to read next file 
                                {
                                    var ja = new JArray();
                                    fileName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);
                                    //var getTheIndividualFileName = fullFilePath.Split('\\');
                                    //fileName = getTheIndividualFileName[getTheIndividualFileName.Length - 1]; // get the file name
                                    //int index = fileName.LastIndexOf(".");
                                    //fileName = fileName.Substring(0, index);
                                    //read the file

                                    // get all the data for that pdf
                                    savePage = savePageAllFiles[fullFilePath]; // all page data 
                                    savePageSection = saveSectionNoAllFiles[fullFilePath];
                                    getSaveSection = saveAllSection[fullFilePath];
                                    savePageSectionRegex = saveAllSectionRegex[fullFilePath];
                                    singleFileSectionTree = sectionTree[fullFilePath];

                                    getPageNo(savePage.Count(), startPage, endPage, out startPageVal, out endPageVal);
                                    List<Dictionary<Dictionary<string, int>, int>> dataSet = new List<Dictionary<Dictionary<string, int>, int>>();
                                    Dictionary<Dictionary<string, int>, int> getNode = new Dictionary<Dictionary<string, int>, int>();
                                    Dictionary<Dictionary<string, int>, int> getNodeRegex = new Dictionary<Dictionary<string, int>, int>();
                                    //---------------------get paragraph process --------------------------------
                                    if (SearchWithin == 2)
                                    {
                                        getAllFoundText(singleFileSectionTree,savePageSection, startPageVal, endPageVal, exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text
                                    }
                                    else
                                        getAllFoundText(singleFileSectionTree, savePageSection, startPageVal, endPageVal,exclusionCount, fullFilePath, SearchWithin, savePage, fileName, logic, out ja, out totalScoreDenominatorVal, out searchFieldScore, out withInScore); //  get the found text
                                    // ---------------------------------------------------------------------------
                                    if (getTheWithinDictionary == true)// get the within in Dictionary for sentence
                                    { 
                                        foreach (var item in withInScore)
                                        {
                                            if (!withInForSentence.ContainsKey(item.Key)) // check if dictionary has the same key
                                                withInForSentence.Add(item.Key, item.Value);
                                        }
                                    }
                                    //--------------------scoring and final input para ---------------------------------------------------------------------

                                    scoring(readDuplicate, PageNoMatch, datapointName, OutputMatch, LeaseName, savePage, totalScoreDenominatorVal, searchFieldScore, ja, multipleRead, out ja1, out finalScore);
                                    //-------------------------------------------------------------------------------------------------------------------

                                    // -----------------------financial CHeck-----------------------------------------------------
                                    var financialInTable = 1;
                                    //if financial datapoint
                                    //if(FinancialData == 1)
                                    //{
                                        var maxColumn = 0;
                                        string[][] financial = new string[50][];
                                        getTheFinancialValFromPage(ja1, savePage, fullFilePath, out financial, out maxColumn); // get the financial column

                                        if (financialInTable == 1)
                                        {
                                            for (int i = 0; i < maxColumn; i++) // 
                                            {
                                                List<string> coulmnData = new List<string>();
                                                foreach (var item in financial)
                                                {
                                                    coulmnData.Add(item[i]);
                                                }
                                                saveFinancialCount++;
                                                saveFinancial.Add(coulmnData.ToArray(), saveFinancialCount.ToString());
                                            }
                                            findColumnHead(saveFinancial, financialLibrary, out saveFinancialCopy); // check the heading of the data 
                                        }
                                    //}
                                    //------------------------------------------------------------------------------------------------


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
                            //------------------------------------------------------------------------------------------------------


                            //----------------------data concate-----------------------------------------------------------------
                            if (ja2.Count > 0)// check if any result found
                            {
                                var getTheCompleteSectionValue = "";
                                savePageSection = saveSectionNoAllFiles[ja2[0]["completeFilePath"].ToString()];
                                getSaveSection = saveAllSection[ja2[0]["completeFilePath"].ToString()];
                                singleFileSectionTree = sectionTree[ja2[0]["completeFilePath"].ToString()];
                                var outputPara = "";
                                if (SearchWithin == 3) // para
                                {
                                    getTheCompleteSectionValue = processing.getCompleteParaSection(SectionNoCount, ja2, savePageSection, getSaveSection, outputPara, DefaultSectionName, SectionName, singleFileSectionTree); // get the section value
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
                                    getSectionAndFileNameAndSearchJO["withIn"] = ja2[0]["foundWithIn"].ToString();
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
                            //--------------------------------------------------------------------------------------------
                        }
                        //---------------------------------------------------------------------------------------------
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
                                    getSectionAndFileNameAndSearchJO["withIn"] = "";
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

                        //---------------------------------------------------------------------------------------------
                        nextVal++;
                    }

                    #region --------------------------------get complete output from multiple para (SUMMARIZATION)---------------------------------------------------

                    if (getSectionAndFileNameAndSearchJA.HasValues)
                    {
                        var finalOutputData = "";
                        JArray collectCorrectSentanceOutput = new JArray();
                        // get the filename, sectionNo and data....
                        collectCorrectSentance(abbreviationCheckData, andConditionCheck, paraBreakCondition, SearchWithinVal, checkNextLine, financialSelect, sentenceStart, sentenceEnd, SentenceResultOutputFormatCondition, getSectionAndFileNameAndSearchJA, withInForSentence, resultAllKeyword, resultSearch, copyResultOutputFormat, out finalOutputData, out collectCorrectSentanceOutput);
                        var format = "";
                        string FoundText = "";
                        string finalFormat = "";
                        // set the complete format
                        buildFormat(outputNotFoundMessage, collectCorrectSentanceOutput, finalOutputData, resultOutputFormat, out format, out FoundText, out finalFormat);
                        // save the format in json
                        string finalFormatData = "";
                        abbreviationReplace(AbbreviationData, format, out finalFormatData);
                        ja3[0]["correctString"] = finalFormatData;
                        var onlysummarization = finalFormatData.Remove(0, finalFormatData.IndexOf(':') + 1);
                        ja3[0]["onlySummarization"] = onlysummarization;
                        var onlySectionNo = finalFormatData.Replace(onlysummarization, "").Trim();
                        if (onlySectionNo.EndsWith(":"))
                            onlySectionNo = onlySectionNo.TrimEnd(':');
                        if (onlySectionNo.StartsWith(","))
                            onlySectionNo = onlySectionNo.TrimStart(',');
                        ja3[0]["onlySectionNo"] = onlySectionNo.Trim();
                    }
                    #endregion

                    #region ---------- if data found then display "lease is silent"------------------------------------

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
                        jo4["correctString"] = "Lease is silent";
                        jo4["onlySectionNo"] = "";
                        jo4["dataPointName"] = datapointName;
                        jo4["onlySummarization"] = "Lease is silent";
                        jo4["sectionNoreadPara"] = "Lease is silent";
                        jo4["onlySummarization"] = "Lease is silent";
                        ja4.Add(jo4);
                        ja3.Add(ja4[0]);
                    }
                    #endregion

                    // check if financial datapoint
                    //if (FinancialData == 1)
                    //{
                        List<string> finalFinancialValues = new List<string>();
                    if (saveFinancialCopy.Count() > 0)
                    {
                        processing.getCurrencyAmountFinancial(saveFinancialCopy.ElementAt(0).Value.ToList(), out finalFinancialValues);
                        ja3[0]["financialData"] = true;
                        ja3[0]["financialValue"] = saveFinancialCopy.ElementAt(0).Key;
                        var financialContent = JsonConvert.SerializeObject(finalFinancialValues);
                        ja3[0]["financialContent"] = financialContent;
                    }
                    //}
                    //else
                    //{
                    //    ja3[0]["financialData"] = false;
                    //    ja3[0]["financialValue"] = "";
                    //    ja3[0]["financialContent"] = "";
                    //}

                    finalOutput.Add(ja3[0]); // saves output of all the datapoint-lease (is used to save data for all the leases and send to front end)
                    saveLeaseDataForExcel.Add(ja3[0]); // save data for single lease (lease has multiple pdfs) (used to save data in excel)

                    #region ----------------------------Highlight process (PENDING)----------------------------------
                    //if (ja3[0]["output"].ToString() != "Lease is silent" && ja3[0]["output"].ToString() != "lease is silent")
                    //    highlightPdf(ja3[0]["output"].ToString(), ja3[0]["pageNo"].ToString(), ja3[0]["completeFilePath"].ToString(), ja3[0]["dataPointName"].ToString());
                    #endregion

                }
                #endregion

                processing.ReplaceTextInExcelFile(subdirectoryEntries[folderval], projName, saveDatapointName, saveLeaseDataForExcel);
               
            }

            #endregion

            #region-------------------------front end display----------------------
            frontEndData.Text = finalOutput.ToString(); // set the data in front end
            watch.Stop(); // total time
            displayTime.Text = (watch.ElapsedMilliseconds / 60000).ToString() + " mins";
            #endregion  
        }

            #region --------------------financial process-------------------------------------
        public void findColumnHead(Dictionary<string[], string> saveFinancial,Dictionary<string, List<string>> financialLibrary, out Dictionary<string, string[]> saveFinancialCopy)
        {
            var columnCount = 0;
            saveFinancialCopy = new Dictionary<string, string[]>();
            foreach (var completeFinancial in saveFinancial)
            {
                var key = completeFinancial.Key;
                var firstRow = key[0];
                var foundDataType = false;
                foreach (var completeFinancialLib in financialLibrary)
                {
                    var financialLibKey = completeFinancialLib.Key;
                    var financialLibVal = completeFinancialLib.Value;
                    foreach (var singleFinancialLib in financialLibVal)
                    {
                        string Pattern = @"(?i)" + singleFinancialLib;
                        foreach (Match m in Regex.Matches(firstRow, Pattern))
                        {
                            //saveFinancial[key] = financialLibKey;
                            saveFinancialCopy.Add(financialLibKey, completeFinancial.Key);
                            foundDataType = true;
                            break;
                        }
                        if (foundDataType == false)
                        {
                            Pattern = @"(?i)" + singleFinancialLib;
                            foreach (Match m in Regex.Matches(key[1], Pattern))
                            {
                                saveFinancialCopy.Add(financialLibKey, completeFinancial.Key);
                                foundDataType = true;
                                break;
                            }
                        }
                        if (foundDataType == true)
                            break;
                    }
                    if (foundDataType == true)
                        break;
                }
                if (foundDataType == true)
                    break;
                columnCount++;
            }
        }

        public void readPdfForFinancial(string fullFilePath, int pageNoToProcess, Doc doc , out List<Data> readedValues)
        {
            TextOperation op = new TextOperation(doc);
            
            op.PageContents.AddPages(pageNoToProcess);
            string theText = op.GetText();

            IList<TextGroup> theGroups = op.Group(op.Select(0, theText.Length));
            List<TextGroup> ordereddGroups = theGroups.OrderByDescending(o => o.Rect.Top).ToList();
            StringBuilder sb1 = new StringBuilder();

            int pos = fullFilePath.LastIndexOf(".") + 1;
            var extencion = fullFilePath.Substring(pos, fullFilePath.Length - pos);

            File.WriteAllText(fullFilePath.Replace("." + extencion, $"_svg_p{pageNoToProcess}.svg"), doc.GetText("SVG"));

            string[] getSentance = fullFilePath.Split(new string[] { "\\" }, StringSplitOptions.None);
            var fileNameVal = getSentance[getSentance.Count() - 1];
            string searchStr = "." + extencion;
            int lastIndex = fileNameVal.LastIndexOf(searchStr);
            var saveNewFile = fileNameVal.Substring(0, lastIndex) + fileNameVal.Substring(lastIndex + searchStr.Length) + "_svg_p" + pageNoToProcess + ".svg";
            XDocument document = XDocument.Load(fullFilePath.Replace(fileNameVal, saveNewFile));
            XNamespace ns = "http://www.w3.org/2000/svg";

            List<double> yAxis = new List<double>();
            List<double> xAxis = new List<double>();
            List<double> textLength = new List<double>();
            List<string> word = new List<string>();
            foreach (var item in document.Root.Descendants())
            {
                
                var nodeName = item.Name.LocalName;
                if (nodeName == "text")
                {
                   
                     xAxis.Add((int)(double.Parse(item.Attribute("x").Value)));
                     yAxis.Add((int)(double.Parse(item.Attribute("y").Value)));
                  
                     word.Add(item.Value);
                    if (item.Attribute("textLength") == null)
                        textLength.Add(0);
                    else
                        textLength.Add(double.Parse(item.Attribute("textLength").Value));
                }
                else if (nodeName == "tspan")
                {
                    if (item.Attribute("dx") == null)
                    {
                        word[word.Count() - 1] = item.Value;
                        textLength[textLength.Count() - 1] = double.Parse(item.Attribute("textLength").Value);
                    }
                    else if (item.Attribute("dx") != null)
                    {
                        var dxAttrVal = item.Attribute("dx").Value;
                        if ((int)(double.Parse(dxAttrVal)) < 0)
                            xAxis.Add(xAxis.Last() + +textLength.Last() + 0);
                        else
                            xAxis.Add(xAxis.Last() + textLength.Last() + (int)(double.Parse(dxAttrVal)));
                        yAxis.Add(yAxis.Last());
                        word.Add(item.Value);
                        textLength.Add(double.Parse(item.Attribute("textLength").Value));
                    }
                }
            }

            readedValues = new List<Data>();
            for (int j = 0; j < xAxis.Count(); j++)
            {
                readedValues.Add(new Data(word.ElementAt(j).Trim(), yAxis.ElementAt(j), xAxis.ElementAt(j), textLength.ElementAt(j)));

                readedValues = readedValues.OrderBy(x => x.YAxis).ThenBy(x => x.XAxis).ToList();
            }
        }
        
        public void getTheFinancialValFromPage(JToken ja , Dictionary<int, Dictionary<int,string>> savePage, string fullFilePath, out string[][] financial, out int maxColumn)
        {
            financial = new string[50][];
            maxColumn = 0;
            //var toFInd = "year";
            //List<string> toFindList = new List<string>();
            //if (toFInd == "year")
            //    toFindList = year;
            //if (toFInd == "month")
            //    toFindList = month;
            //if (toFInd == "squareFoot")
            //    toFindList = squareFoot;
            //if (toFInd == "duration")
            //    toFindList = duration;
            if (ja.Count()==0)
                return;
            using (Doc doc = new Doc())
            {
                doc.Read(fullFilePath);
                var firstPageFound = (int)ja[0]["pageNo"];
                var paraFound = ja[0]["sectionNoreadPara"].ToString();
                var sentenceFind = 0;
                
                // find in table
                if (sentenceFind == 0)
                {
                    readedValues = new List<Data>();
                    readPdfForFinancial(fullFilePath, firstPageFound, doc, out readedValues);

                    string[][] jaggedArray = new string[50][];
                    var XCorrdinateValue = 0;
                        
                    List<double> dummyXAxis = new List<double>();
                    List<double> dummyYAxis = new List<double>();
                    List<double> dummyTextLength = new List<double>();
                    List<string> dummyWord = new List<string>();

                    List<string> saveColumnData = new List<string>();
                    List<int> saveColumnIndex = new List<int>();
                    List<int> saveColumnTextLength = new List<int>();
                    List<Rebo.Model.Table.Table> tableList = new List<Rebo.Model.Table.Table>();
                    var readedValuesCopy = new List<Data>();
                    var dd = readedValues.Where(s => s.YAxis >= 500 && s.YAxis <= 585).OrderBy(x=>x.XAxis).ToList();
                    readedValuesCopy.AddRange(dd);


                    //foreach (var item in readedValues) // get data to process
                    //{
                    //    if (item.YAxis >= 500 & item.YAxis <= 585)
                    //    {
                    //        readedValuesCopy.Add(new Data {YAxis=item.YAxis,XAxis=item.XAxis,Word=item.Word,TextLength=item.TextLength });

                    //        readedValuesCopy = readedValuesCopy.OrderBy(x => x.XAxis).ThenBy(x => x.YAxis).ToList();
                    //    }
                           
                    //}

                    // sort the data on x axis and y axis

                 
                    //for (int j = 0; j < dummyXAxis.Count(); j++)
                    //{
                    //    readedValuesCopy.Add(new Data(dummyWord.ElementAt(j).Trim(), dummyYAxis.ElementAt(j), dummyXAxis.ElementAt(j), dummyTextLength.ElementAt(j)));

                    //    readedValuesCopy = readedValuesCopy.OrderBy(x => x.XAxis).ThenBy(x =>x.YAxis).ToList();
                    //}

                    int[][] jaggedArrayXAxis = new int[50][];
                    int[][] jaggedArrayTextLength = new int[50][];
                    Dictionary<double, int[]> saveCoordinate = new Dictionary<double,int[]>();
                        
                    var rowCountCOpy = 0;

                    foreach (var item in readedValuesCopy)
                    {
                        if (!saveCoordinate.ContainsKey(item.YAxis))
                        {
                            saveCoordinate.Add(item.YAxis, null);
                            string[] row = new string[10];
                            int[] rowXVal = new int[10];
                            int[] textLengthVal = new int[10];
                            row[0] = item.Word;
                            rowXVal[0] = (int)item.XAxis;
                            textLengthVal[0] = (int)item.TextLength;
                            jaggedArray[rowCountCOpy] = row;
                            jaggedArrayXAxis[rowCountCOpy] = rowXVal;
                            jaggedArrayTextLength[rowCountCOpy] = textLengthVal;
                            rowCountCOpy++;
                        }
                        else
                        {
                            var index2 = 0;
                            int index = saveCoordinate.Keys.ToList().IndexOf(item.YAxis);
                            int[] array = jaggedArrayXAxis[index];
                            for (int j = 0; j < array.Length; j++)
                                if (array[j].Equals(0))
                                {
                                    index2 = j;
                                    break;
                                }

                            if (index2 != 0)
                            {
                                jaggedArrayXAxis[index][index2] = (int)item.XAxis;
                                jaggedArray[index][index2] = item.Word;
                                jaggedArrayTextLength[index][index2] = (int)item.TextLength;
                            }
                                
                        }
                    }

                    var dictionaryAddCount = 0;
                    jaggedArrayXAxis = jaggedArrayXAxis.Where(c => c != null).ToArray();
                    jaggedArray = jaggedArray.Where(c => c != null).ToArray();
                    jaggedArrayTextLength = jaggedArrayTextLength.Where(c => c != null).ToArray();

                    for (int u = 0; u < jaggedArray.Count(); u++)
                    {
                        var jaggedArrayRow = jaggedArrayXAxis[u];
                        jaggedArrayRow = jaggedArrayRow.Where(val => val != 0).ToArray();
                        jaggedArrayXAxis[u] = jaggedArrayRow;
                        var jaggedArrayRowTextLength = jaggedArrayTextLength[u];
                        jaggedArrayRowTextLength = jaggedArrayRowTextLength.Where(val => val != 0).ToArray();
                        jaggedArrayTextLength[u] = jaggedArrayRowTextLength;
                    }

                    for (int h = 0; h < jaggedArrayXAxis.Count(); h++) // loop through all the rows
                    {
                        var jaggedArrayRowXAxis = jaggedArrayXAxis[h];
                        var jaggedArrayRowTextLength = jaggedArrayTextLength[h];
                        var jaggedArrayRowText = jaggedArray[h];
                        for (int d = 0; d < jaggedArrayRowXAxis.Count(); d++) // loop through all the content of the row
                        {
                            var xVAl = jaggedArrayRowXAxis[d];
                            var wordVal = jaggedArrayRowText[d];
                            var textLengthVal = jaggedArrayRowTextLength[d];
                            if (XCorrdinateValue == 0 &  saveColumnData.Count() == 0)
                            {
                                saveColumnData.Add(wordVal);
                                XCorrdinateValue = xVAl + textLengthVal;
                                saveColumnIndex.Add(xVAl);
                                saveColumnTextLength.Add(textLengthVal);
                            }
                            else
                            {
                                if ((xVAl < XCorrdinateValue + 15)) // same word
                                {
                                    Regex regex = new Regex("(?<SYMBOL>[$â‚¬Â£]){1}[\\s]*(?<AMOUNT>[\\d{1,3}(\\s){0,1}(\\.(\\d(?:\\+\\.?)*)?)?]+)");
                                    var amountCount = regex.Matches(wordVal).Count;
                                    if (amountCount > 0)
                                    {
                                        foreach (Match match in regex.Matches(wordVal))
                                        {
                                            saveColumnData.Add(match.ToString());
                                            saveColumnIndex.Add(xVAl);
                                            saveColumnTextLength.Add(textLengthVal);
                                            XCorrdinateValue = xVAl + textLengthVal;
                                        }
                                    }
                                    else
                                    {
                                        saveColumnData[saveColumnData.Count() - 1] = saveColumnData.Last() + " " + wordVal;
                                        saveColumnTextLength[saveColumnTextLength.Count() - 1] = xVAl + textLengthVal - (saveColumnTextLength.Last() + saveColumnIndex.Last());
                                        XCorrdinateValue = xVAl + textLengthVal;
                                    }
                                }
                                else if ((xVAl >= XCorrdinateValue + 15)) // next column
                                {
                                    saveColumnData.Add(wordVal);
                                    saveColumnIndex.Add(xVAl);
                                    saveColumnTextLength.Add(textLengthVal);
                                    XCorrdinateValue = xVAl + textLengthVal;
                                }
                            }
                        }
                        jaggedArray[h] = saveColumnData.ToArray();
                        jaggedArrayXAxis[h] = saveColumnIndex.ToArray();
                        jaggedArrayTextLength[h] =saveColumnTextLength.ToArray();
                        saveColumnData = new List<string>();
                        saveColumnIndex = new List<int>();
                        saveColumnTextLength = new List<int>();
                        XCorrdinateValue = 0;
                    }

                    //saveCoordinateData
                    Dictionary<double, string[]> saveCoordinateData = new Dictionary<double, string[]>();
                    Dictionary<double, int[]> saveCoordinateDataTextLength = new Dictionary<double, int[]>();
                    // add data in dictionary
                    for (var item=0; item< jaggedArrayXAxis.Count(); item ++)
                    {
                        saveCoordinate[saveCoordinate.ElementAt(dictionaryAddCount).Key] = jaggedArrayXAxis[item];
                        saveCoordinateData.Add(saveCoordinate.ElementAt(dictionaryAddCount).Key, jaggedArray[item]);
                        saveCoordinateDataTextLength.Add(saveCoordinate.ElementAt(dictionaryAddCount).Key, jaggedArrayTextLength[item]);
                        dictionaryAddCount++;
                    }

                    saveCoordinate = saveCoordinate.OrderBy(key => key.Key).ToDictionary(x => x.Key, y => y.Value);
                    saveCoordinateData = saveCoordinateData.OrderBy(key => key.Key).ToDictionary(x => x.Key, y => y.Value);
                    saveCoordinateDataTextLength = saveCoordinateDataTextLength.OrderBy(key => key.Key).ToDictionary(x => x.Key, y => y.Value);

                    Dictionary<double, int[]> sortedDictionaryCoordinate = new Dictionary<double, int[]>();
                    Dictionary<double, string[]> sortedDictionaryData = new Dictionary<double, string[]>();

                    List<string> commonRowInfo = new List<string>();
                    var rowGapValue = 12;
                    List<double> gapVal = new List<double>();
                    List<double> gapDifference = new List<double>();
                    for (int w = 0; w < saveCoordinate.Count(); w++)
                    {
                        if (w != 0)
                        {
                            var lastVal = gapVal.Last();
                            var currentVal = saveCoordinate.ElementAt(w).Key;
                            gapDifference.Add(currentVal - lastVal);
                            gapVal.Add(saveCoordinate.ElementAt(w).Key);
                        }
                        else
                        {
                            gapVal.Add(saveCoordinate.ElementAt(w).Key);
                        }
                    }
                        
                    var readLowerGap = true;
                    foreach (var item in gapDifference)
                    {
                        if (item > 12)
                        {
                            readLowerGap = false;
                            break;
                        }
                    }

                    for (int w = 0; w < saveCoordinate.Count(); w++)
                    {
                        if (w != 0 & saveCoordinate.Count() > 1 & readLowerGap == false)
                        {
                            if ((int)saveCoordinate.ElementAt(w).Key - (int)saveCoordinate.ElementAt(w - 1).Key <= rowGapValue)
                                commonRowInfo[commonRowInfo.Count() - 1] = commonRowInfo.ElementAt(commonRowInfo.Count() - 1) + "-" + w.ToString();
                            else
                                commonRowInfo.Add(w.ToString());
                        }
                        else if (w != 0 & saveCoordinate.Count() > 1 & readLowerGap == true)
                        {
                            if ((int)saveCoordinate.ElementAt(w).Key - (int)saveCoordinate.ElementAt(w - 1).Key < rowGapValue)
                                commonRowInfo[commonRowInfo.Count() - 1] = commonRowInfo.ElementAt(commonRowInfo.Count() - 1) + "-" + w.ToString();
                            else
                                commonRowInfo.Add(w.ToString());
                        }
                        else
                            commonRowInfo.Add(w.ToString());
                    }

                    string[][] finalTable = new string[commonRowInfo.Count()][];
                    int[][] finalTableCoordinates = new int[commonRowInfo.Count()][];
                    int[][] finalTableTextLength = new int[commonRowInfo.Count()][];
                    var rowCount = 0;
                    foreach (var item in commonRowInfo)
                    {
                        var commonRowCombineData = item.Split('-');
                        if (commonRowCombineData.Count() == 1)
                        {
                            var rowNo = Int32.Parse(commonRowCombineData[0]);
                            var firstRowDataTextLength = saveCoordinateData[saveCoordinate.ElementAt(rowNo).Key];
                            finalTable[rowCount] = firstRowDataTextLength;
                            finalTableCoordinates[rowCount] = saveCoordinate[saveCoordinate.ElementAt(rowNo).Key];
                            finalTableTextLength[rowCount] = saveCoordinateDataTextLength[saveCoordinate.ElementAt(rowNo).Key];
                            rowCount++;
                        }
                        else
                        {
                            Dictionary<string, int> combineRowData = new Dictionary<string, int>();
                            Dictionary<string, int> combineRowDataTextLength = new Dictionary<string, int>();
                            foreach (var data in commonRowCombineData)
                            {
                                var rowNo = Int32.Parse(data);
                                var firstRowCoordinate = saveCoordinate[saveCoordinate.ElementAt(rowNo).Key];
                                var firstRowData = saveCoordinateData[saveCoordinate.ElementAt(rowNo).Key];
                                var firstRowDataTextLength = saveCoordinateDataTextLength[saveCoordinate.ElementAt(rowNo).Key];
                                for (int e = 0; e < firstRowCoordinate.Count(); e++)
                                {
                                    combineRowData.Add(firstRowData[e] + "_" + e, firstRowCoordinate[e]);
                                    combineRowDataTextLength.Add(firstRowData[e] + "_" + e, firstRowDataTextLength[e]);
                                }
                            }
                            combineRowData = combineRowData.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                            Dictionary<string, int> combineRowDataCopy = new Dictionary<string, int>();
                            Dictionary<string, int> combineRowDataCopyCoordinate = new Dictionary<string, int>();
                            var readDataOfRow = false;
                            for (int t = 0; t < combineRowData.Count(); t++)
                            {
                                if (readDataOfRow == true)
                                {
                                    var firstXCoordinate = combineRowData.ElementAt(t);
                                    var nextXCoordinate = combineRowData.ElementAt(t - 1);
                                    var firstBorder = firstXCoordinate.Value + combineRowDataTextLength[firstXCoordinate.Key];
                                    var nextBorder = nextXCoordinate.Value + combineRowDataTextLength[nextXCoordinate.Key];

                                    if ((firstXCoordinate.Value <= nextXCoordinate.Value & firstBorder >= nextXCoordinate.Value) | (nextXCoordinate.Value <= firstXCoordinate.Value & nextBorder >= firstXCoordinate.Value))
                                    {
                                        var lastText = combineRowDataCopy.Last().Key;
                                        var lastVal = combineRowDataCopy.Last().Value;
                                        var lastTextLength = combineRowDataCopyCoordinate.Last().Value;
                                        Regex regexCheck = new Regex("[_]\\d$");
                                        var matchVal = regexCheck.Match(lastText); // check if match found
                                        if (matchVal.Success)
                                        {
                                            lastText = lastText.Replace(matchVal.Value, "").Trim();
                                            var completeWord = lastText + " " + firstXCoordinate.Key;
                                            combineRowDataCopy.Remove(combineRowDataCopy.Last().Key);
                                            if (combineRowDataCopy.Count > 0)
                                            {
                                                combineRowDataCopyCoordinate.Remove(combineRowDataCopy.Last().Key);
                                            }
                                            combineRowDataCopyCoordinate.Add(completeWord, lastTextLength + combineRowDataTextLength.ElementAt(t).Value);
                                            combineRowDataCopy.Add(completeWord, lastVal);
                                        }
                                    }
                                    else
                                    {
                                        combineRowDataCopy.Add(combineRowData.ElementAt(t).Key, combineRowData.ElementAt(t).Value);
                                        combineRowDataCopyCoordinate.Add(combineRowData.ElementAt(t).Key, combineRowDataTextLength.ElementAt(t).Value);
                                    }
                                }
                                else
                                {
                                    combineRowDataCopy.Add(combineRowData.ElementAt(t).Key, combineRowData.ElementAt(t).Value);
                                    combineRowDataCopyCoordinate.Add(combineRowData.ElementAt(t).Key, combineRowDataTextLength.ElementAt(t).Value);
                                    readDataOfRow = true;
                                }
                            }
                            var dictonaryKey = combineRowDataCopy.Keys.ToList();
                            var dictionaryValue = combineRowDataCopy.Values.ToList();
                            var dictonaryValue = combineRowDataCopy.Values.ToList();
                            for (int g = 0; g < dictonaryKey.Count(); g++)
                            {
                                var textValue = dictonaryKey.ElementAt(g);
                                Regex regexCheck = new Regex("[_]\\d$");
                                var matchVal = regexCheck.Match(textValue); // check if match found
                                if (matchVal.Success)
                                {
                                    dictonaryKey[g] = textValue.Replace(matchVal.Value,""); 
                                }
                            }
                            finalTable[rowCount] = dictonaryKey.ToArray();
                            finalTableCoordinates[rowCount] = dictionaryValue.ToArray();
                            finalTableTextLength[rowCount] = combineRowDataCopyCoordinate.Values.ToArray();
                            rowCount++;
                        }
                    }

                    string[][] createNewArray = new string[commonRowInfo.Count()][];
                    var createNewArrayCount = 0;

                    foreach (var item in finalTable)
                    {
                        if (maxColumn < item.Count())
                            maxColumn = item.Count();
                    }

                    financial=finalTable;

                    var rowCountVal = 0;
                    for (int i = 0; i < finalTableCoordinates.Count(); i++)
                    {
                        var rowTextLength = finalTableTextLength[i];
                        if (i != 0)
                        {
                            rowCountVal = finalTableCoordinates[i - 1].Count();
                            string[] dummyArray = new string[finalTableCoordinates[i].Count()];
                            var dummyArrayCount = 0;
                            if (rowCountVal < finalTableCoordinates[i].Count())
                            {
                                int[] newArrayCoordinate = new int[finalTableCoordinates[i - 1].Count()];
                                string[] newArrayData = new string[finalTableCoordinates[i - 1].Count()];
                                newArrayCoordinate = finalTableCoordinates[i - 1];
                                newArrayData = finalTable[i - 1];
                                for (int j = 0; j < newArrayCoordinate.Count(); j++)
                                {
                                    var smallArrayCoordinate = newArrayCoordinate[j];
                                    var smallArrayData = newArrayData[j];
                                    for (int k = 0; k < finalTableCoordinates[i].Count(); k++)
                                    {
                                        var largeArray = finalTableCoordinates[i][k];
                                        var currentTextLength = rowTextLength[k];
                                        if (smallArrayCoordinate <= largeArray) // done
                                        {
                                            dummyArray[k] = smallArrayData;
                                            dummyArrayCount++;
                                            break;
                                        }
                                        else if (smallArrayCoordinate > largeArray)
                                        {
                                            if (currentTextLength + largeArray > smallArrayCoordinate)
                                            {
                                                dummyArray[k] = smallArrayData;
                                                dummyArrayCount++;
                                                break;
                                            }
                                        }
                                        else if (k == finalTableCoordinates[i].Count() - 1 & j == newArrayCoordinate.Count() - 1) // done
                                        {
                                            dummyArray[finalTableCoordinates[i].Count() - 1] = smallArrayData;
                                            dummyArrayCount++;
                                            break;
                                        }
                                    }
                                }
                                createNewArray[createNewArrayCount] = dummyArray;
                                createNewArrayCount++;
                            }
                            else if (finalTableCoordinates[i].Count() < rowCountVal)
                            {
                                createNewArray[createNewArrayCount] = finalTable[i - 1];
                                createNewArrayCount++;


                            }
                            else
                            {
                                createNewArray[createNewArrayCount] = finalTable[i - 1];
                                createNewArrayCount++;
                            }
                        }
                    }





                    // var averageColumnVal = totalCount / finalTable.Count();

                    // get the rows

                    var finalTableArray = finalTable;

                    var test2 = "hello";

                }
                // find in sentence
                else
                {
                    //Dictionary<int, Dictionary<double, string>> saveAllPageDetails = new Dictionary<int, Dictionary<double, string>>();

                    //List<int> pageNos = new List<int>();
                    //pageNos.Add(firstPageFound);
                    //pageNos.Add(firstPageFound + 1);
                    //pageNos.Add(firstPageFound + 2);

                    //var pageCount = 0;
                    //foreach (var item in pageNos)
                    //{
                    //    pageCount++;
                    //    Dictionary<double, string> completePageDetail = new Dictionary<double, string>();
                    //    readedValues = new List<Data>();
                    //    readPdfForFinancial(fullFilePath, item, doc, out readedValues);

                    //    foreach (var pageData in readedValues)
                    //    {
                    //        if (pageData.Word != "&#65533;")
                    //        {
                    //            var enterVal = true;
                    //            var keyVal = pageData.YAxis;
                    //            if (completePageDetail.ContainsKey(keyVal))
                    //            {
                    //                enterVal = false;
                    //                completePageDetail[keyVal] = completePageDetail[keyVal].Trim() + " " + pageData.Word;
                    //            }
                    //            else if (completePageDetail.Count() > 0)
                    //            {
                    //                if (keyVal - completePageDetail.Last().Key < 5)
                    //                {
                    //                    enterVal = false;
                    //                    completePageDetail[completePageDetail.Last().Key] = completePageDetail[completePageDetail.Last().Key].Trim() + " " + pageData.Word;
                    //                }
                    //            }
                    //            if (enterVal == true)
                    //                completePageDetail.Add(keyVal, pageData.Word);
                    //        }
                    //    }
                    //    saveAllPageDetails.Add(pageCount, completePageDetail);
                    //}

                    //var startIndex = 0.0;
                    //var firstPage = saveAllPageDetails.ElementAt(0).Value;
                    //for (int i = 0; i < firstPage.Count(); i++)
                    //{
                    //    var pageLines = firstPage.ElementAt(i).Value.Trim();
                    //        if (paraFound.IndexOf(pageLines) != -1)
                    //        {
                    //            startIndex = firstPage.ElementAt(i).Key;
                    //            break;
                    //        }
                    //}
                    
                    //firstPage = firstPage.Where(kvp => kvp.Key >= startIndex).ToDictionary(x=> x.Key,y =>y.Value);
                    //saveAllPageDetails[1] = firstPage;

                    //List<string> saveAllLine = new List<string>();
                    //foreach (var item in saveAllPageDetails)
                    //{
                    //    var page = item.Value;
                    //    foreach (var pageLines in page)
                    //    {
                    //        saveAllLine.Add(pageLines.Value);
                    //    }
                    //}

                    //List<string> saveData = new List<string>();
                    //var foundFirstVal = false;
                    //var valNotFoundCount = 0;
                    //for (int i = 0; i < saveAllLine.Count(); i++)
                    //{
                    //    var dataToCheck = saveAllLine.ElementAt(i);
                    //    foreach (var item in toFindList)
                    //    {
                    //        Regex regex = new Regex("()"+ item);
                    //        var match = regex.Match(dataToCheck); // check if match found
                    //        if (match.Success)
                    //        {
                    //            var index = match.Index;
                    //            var completeLine ="";
                    //            var getStringBeforeIt = dataToCheck.Substring(0, index);
                    //            //if ()
                    //            //{ }
                    //            //else
                    //            //{ }
                    //            //string[] getSentanceFullStop = pageOutput.Split(new string[] { ". " }, StringSplitOptions.None);
                    //            var foundData = processing.getCurrencyAmount(getStringBeforeIt);
                    //            if (foundData.Count() > 0)
                    //            {
                    //                valNotFoundCount = 0;
                    //                foundFirstVal = true;
                    //                saveData.Add(foundData.ElementAt(foundData.Count() -1));
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    if (foundFirstVal == true)
                    //    {
                    //        valNotFoundCount++;
                    //        if (valNotFoundCount > 10)
                    //            break;
                    //    }

                    //}


                    //var test = true;
                }
                
            }
        }
        #endregion


        List<Data> readedValues = new List<Data>();

        #region ---------score (within and searchFor) and the field to search --------------------
        // get the score (within and searchFor) and the field to search 
        public void getTotalScore(JToken withIn, JToken searchFor, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
        {
            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            withInScore = new Dictionary<string, int>();
            // searchFor

            //Directory<string, int> result = searchFor.Select(s => new Directory<string, int> {s["keyword"].ToString(),int.Parse(s["score"]) } )
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
        #endregion  

        #region ----------------------------------------get all files to read----------------------------------------------
        // this process checks the format of each pdf of the lease
        // and on the basis of the fileOrder attribute in json.......it sorts the file and cheks which files to read for the process
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
        #endregion

        #region ----------------------complete process for pdf read and tree construction----------------------------
        // get the paragraph lines
        public void pdfRead(string fileNameVal, string collectSectionLib, string filepath, out Dictionary<int, Dictionary<int, string>> savePage, out Dictionary<int, Dictionary<int, string>> savePageSection, out Dictionary<Dictionary<int, string>, int> saveSection, out Dictionary<int, Dictionary<int, string>> savePageSectionRegex, out string finalJson)
        {
            finalJson = "";
            savePage = new Dictionary<int, Dictionary<int, string>>();
            savePageSection = new Dictionary<int, Dictionary<int, string>>();
            savePageSectionRegex = new Dictionary<int, Dictionary<int, string>>();
            saveSection = new Dictionary<Dictionary<int, string>, int>();
            Dictionary<Dictionary<int, string>, int> saveSectionWithsectionNo = new Dictionary<Dictionary<int, string>, int>();
            Dictionary<Dictionary<int, string>, int> saveSectionWithRegex = new Dictionary<Dictionary<int, string>, int>();
            List<string> sectionNameFound = new List<string>();
            var nextSection = 0;
            List<string> saveSectionSectionVal = new List<string>();
            List<string> saveSectionSectionRegex = new List<string>();
            using (Doc doc = new Doc())
            {
                doc.Read(filepath);// read the document
                short PageIndex = 1; // start with page no 1
                var lastSectionPageNo = 0;
                Dictionary<int, string> saveSectionPara = new Dictionary<int, string>();
                Dictionary<int, string> saveSectionSectionNo = new Dictionary<int, string>();
                Dictionary<int, string> saveSectionRegex = new Dictionary<int, string>();
                var lastLine = "";
                var lastSection = "";
                var lastRegex = "";
                List<string> saveSectionName = new List<string>(); // to check if the new line has section / article
                saveSectionName.Add("section");
                saveSectionName.Add("article");
                StringBuilder sbSection = new StringBuilder();
                
                while (PageIndex <= doc.PageCount) // loop through all the pages
                {
                   // pdf reading process
                    TextOperation op = new TextOperation(doc);
                    op.PageContents.AddPages(PageIndex);
                    string theText = op.GetText();

                    IList<TextGroup> theGroups = op.Group(op.Select(0, theText.Length));
                    List<TextGroup> ordereddGroups = theGroups.OrderByDescending(o => o.Rect.Top).ToList();
                    StringBuilder sb1 = new StringBuilder();
                    
                    XRect prevRect = new XRect("0 0 0 0");

                    //----------------------------------------------------------------------------------------------------
                    int pos = filepath.LastIndexOf(".") + 1;
                    var extencion =filepath.Substring(pos, filepath.Length - pos);

                    File.WriteAllText(filepath.Replace("."+ extencion, $"_svg_p{doc.PageNumber}.svg"), doc.GetText("SVG"));

                    string searchStr = "."+extencion;
                    int lastIndex = fileNameVal.LastIndexOf(searchStr);
                    var saveNewFile = fileNameVal.Substring(0, lastIndex) + fileNameVal.Substring(lastIndex + searchStr.Length) + "_svg_p" + PageIndex + ".svg";
                    XDocument document = XDocument.Load(filepath.Replace(fileNameVal, saveNewFile));
                    XNamespace ns = "http://www.w3.org/2000/svg";

                    List<double> yAxis = new List<double>();
                    List<double> xAxis = new List<double>();
                    List<double> textLength = new List<double>();
                    List<string> word = new List<string>();
                    
                    // get the coordinate of each word in the page
                    foreach (var item in document.Root.Descendants())
                    {
                        var nodeName = item.Name.LocalName;
                        if (nodeName == "text")
                        {
                            xAxis.Add((int)(double.Parse(item.Attribute("x").Value)));
                            yAxis.Add((int)(double.Parse(item.Attribute("y").Value)));
                            word.Add(item.Value);
                            if (item.Attribute("textLength") == null)
                                textLength.Add(0);
                            else
                                textLength.Add(double.Parse(item.Attribute("textLength").Value));
                        }
                        else if (nodeName == "tspan")
                        {
                            if (item.Attribute("dx") == null)
                            {
                                word[word.Count() - 1] = item.Value;
                                textLength[textLength.Count() - 1] = double.Parse(item.Attribute("textLength").Value);
                            }
                            else if (item.Attribute("dx") != null)
                            {
                                var dxAttrVal = item.Attribute("dx").Value;
                                if ((int)(double.Parse(dxAttrVal)) < 0)
                                    xAxis.Add(xAxis.Last() + +textLength.Last() + 0);
                                else
                                    xAxis.Add(xAxis.Last() + textLength.Last() + (int)(double.Parse(dxAttrVal)));
                                yAxis.Add(yAxis.Last());
                                word.Add(item.Value);
                                textLength.Add(double.Parse(item.Attribute("textLength").Value));
                            }
                        }
                    }

                    // sort the word on the basis of x and y axis
                    readedValues = new List<Data>();
                    for (int l = 0; l < xAxis.Count(); l++)
                    {
                        readedValues.Add(new Data(word.ElementAt(l).Trim(), yAxis.ElementAt(l), xAxis.ElementAt(l), textLength.ElementAt(l)));

                        readedValues = readedValues.OrderBy(x => x.YAxis).ThenBy(x => x.XAxis).ToList();
                    }
                   
                    // join words to make sentence
                    Dictionary<double, string> completePageDetail = new Dictionary<double, string>();
                    foreach (var item in readedValues)
                    {
                        if (item.Word !="&#65533;")
                        {
                            var enterVal = true;
                            var keyVal = item.YAxis;
                            if (completePageDetail.ContainsKey(keyVal))
                            {
                                enterVal = false;
                                completePageDetail[keyVal] = completePageDetail[keyVal].Trim() + " " + item.Word;
                            }
                            else if (completePageDetail.Count() > 0)
                            {
                                if (keyVal - completePageDetail.Last().Key < 5)
                                {
                                    enterVal = false;
                                    completePageDetail[completePageDetail.Last().Key] = completePageDetail[completePageDetail.Last().Key].Trim() + " " + item.Word;
                                }
                            }
                            if (enterVal == true)
                                 completePageDetail.Add(keyVal, item.Word);
                        }
                    }

                    // remove the sentence with only one char count
                    Dictionary<double, string> completePageDetailCopy = new Dictionary<double, string>(completePageDetail);
                    foreach (var item in completePageDetailCopy)
                    {
                        if(item.Value.Trim().Length ==1)
                            completePageDetail.Remove(item.Key);
                    }
                    
                    //-----------------------------------------------------------------------------------------------------
                    Dictionary<int, string> saveLines = new Dictionary<int, string>();
                    Dictionary<int, string> saveSectionNo = new Dictionary<int, string>();
                    Dictionary<int, string> saveSectionNoRegex = new Dictionary<int, string>();
                    var i = 1;
                    
                    List<string> sectionNoCheck = new List<string>();
                    var lineCount = 0;
                    var nextPara = false;
                    var firstParaNoSection = false;
                    var pdfLine = "";
                    var pageCount = 1;
                    var limit = completePageDetail.Count() - 2;
                    // loop through all the lines of the page
                    //form section , identify the section  and save the section(combinition of multiple section)
                    for (var t =0; t< completePageDetail.Count();t++)
                    {
                        pdfLine = completePageDetail.ElementAt(t).Value.Trim();// get the line
                        nextPara = false;
                        lineCount++; // line count value
                        if (lineCount != 1) // if not first line
                        {
                            if ((completePageDetail.ElementAt(t).Key - completePageDetail.ElementAt(t-1).Key) >= 18) // to check if its a new para
                                nextPara = true;
                        }
                        bool section = false;
                        var headingLength = Int32.Parse(WebConfigurationManager.AppSettings["headingLength"]); // heading character count from webconfig
                        if(pdfLine.Length <= headingLength & t <= completePageDetail.Count() -10) // if its top 10 lines then only check for section libaray for section
                            checkSection(lastLine, lineCount, collectSectionLib, pdfLine, out section);

                        if (section == true) // if its a section then check if it starts with saveSectionName value
                        {
                            foreach (var item in saveSectionName) 
                            {
                                Regex regex = new Regex(@"^(?i)("+ item + ")");
                                var match = regex.Match(pdfLine); // check if match found
                                if (match.Success) // if it matches the saveSectionName then find the regex and the section number for it
                                {
                                    sectionNoCheck = processing.getSectionForPara(t, pdfLine, lastLine, nextPara); // get the section value
                                    break;
                                }
                            }
                        }
                        else 
                            sectionNoCheck = processing.getSectionForPara(t, pdfLine, lastLine, nextPara); // get the section value


                        if (sectionNoCheck.Count() == 0) // if no section found ....assign null value
                        {
                            sectionNoCheck.Add(null);
                            sectionNoCheck.Add(null);
                        }

                        // new para found OR new section number found OR new Section found
                        if (nextPara == true || sectionNoCheck.ElementAt(0) != null || section == true)
                        {
                            if (sb1.ToString().Trim() != "")// if section number there
                            {
                                saveLines.Add(i, sb1.ToString().Trim());
                                if (lastSection != "")
                                {
                                    saveSectionNo.Add(i, lastSection);
                                    saveSectionNoRegex.Add(i, lastRegex);
                                }
                                else
                                {
                                    saveSectionNo.Add(i, null);
                                    saveSectionNoRegex.Add(i, null);
                                }
                                lastSection = sectionNoCheck[0];
                                lastRegex = sectionNoCheck[1];
                                i++;
                            }
                            if (lineCount == 1 & sectionNoCheck.ElementAt(0) != null)
                            {
                                lastSection = sectionNoCheck[0];
                                lastRegex = sectionNoCheck[1];
                            }

                            // save section if found
                            if (section == true) 
                            {
                                if (sbSection.ToString().Trim() != "")
                                {
                                    var regexVal ="";
                                    var sectionNoVal ="";
                                    if (saveSectionNo.Count() != 0)
                                    {
                                        regexVal = saveSectionNoRegex.Values.Last();
                                        sectionNoVal = saveSectionNo.Values.Last();
                                    }
                                    else
                                    {
                                        regexVal = null;
                                        sectionNoVal = null;
                                    }
                                    nextSection++;
                                    saveSectionSectionNo.Add(nextSection, sectionNoVal);
                                    saveSectionRegex.Add(nextSection, regexVal);
                                    saveSectionPara.Add(nextSection, sbSection.ToString());
                                }
                                if (saveSectionPara.Count > 0)
                                {
                                    if (saveSection.Count() == 0)
                                    {
                                        sectionNameFound.Add("");
                                        sectionNameFound.Add(pdfLine);
                                    }
                                    else
                                        sectionNameFound.Add(pdfLine);
                                    saveSection.Add(saveSectionPara, PageIndex);
                                    saveSectionWithsectionNo.Add(saveSectionSectionNo, PageIndex);
                                    saveSectionWithRegex.Add(saveSectionRegex, PageIndex);
                                    saveSectionPara = new Dictionary<int, string>();
                                    saveSectionSectionNo = new Dictionary<int, string>();
                                    saveSectionRegex = new Dictionary<int, string>();
                                    lastSectionPageNo = PageIndex;
                                    nextSection = 0;
                                    sbSection.Clear();
                                }
                            }
                            else
                            {
                                
                                if (sbSection.ToString().Trim() == "")
                                {
                                    nextSection++;
                                    saveSectionSectionNo.Add(nextSection,"");
                                    saveSectionRegex.Add(nextSection,"");
                                    saveSectionPara.Add(nextSection, pdfLine);
                                }
                                else if (!saveSectionPara.ContainsValue(sbSection.ToString()))
                                {
                                    var regexVal = "";
                                    var sectionNoVal = "";
                                    if (saveSectionNo.Count() != 0)
                                    {
                                        regexVal = saveSectionNoRegex.Values.Last();
                                        sectionNoVal = saveSectionNo.Values.Last();
                                    }
                                    else
                                    {
                                        regexVal = null;
                                        sectionNoVal = null;
                                    }
                                    nextSection++;
                                    saveSectionSectionNo.Add(nextSection, sectionNoVal);
                                    saveSectionRegex.Add(nextSection, regexVal);
                                    saveSectionPara.Add(nextSection, sbSection.ToString());
                                }
                            }
                            sb1.Clear();
                            sbSection.Clear();

                            if (!pdfLine.EndsWith(" "))
                            {
                                sbSection.Append(pdfLine + " ");
                                sb1.Append(pdfLine + " ");
                            } 
                            else
                            {
                                sbSection.Append(pdfLine);
                                sb1.Append(pdfLine);
                            }
                        }

                        // if its just a line....add all lines to make a para 
                        else
                        {
                            if (!pdfLine.EndsWith(" "))
                            {
                                sbSection.Append(pdfLine + " ");
                                sb1.Append(pdfLine + " ");
                            }
                            else
                            {
                                sbSection.Append(pdfLine);
                                sb1.Append(pdfLine);
                            } 
                        }

                        //save last line for section process .......(PENDING)
                        if (ordereddGroups.Count == lineCount)
                        {
                            var getStringLength = Int32.Parse(WebConfigurationManager.AppSettings["StringLength"]); 
                            if (pdfLine.Length > getStringLength)
                                lastLine = pdfLine;
                            else
                                lastLine = pdfLine;
                        }
                        else
                            lastLine = pdfLine;

                        //prevRect.String = lineGroup.Rect.String;

                        if (section == true) // if the section found then save the section in the dictionary
                        {
                            saveSectionSectionVal.Add(lastSection);
                            saveSectionSectionRegex.Add(lastRegex);
                            lastSection = "";
                            lastRegex="";
                        }
                            
                        lastSectionPageNo = PageIndex;// save the last page no
                        pageCount++;
                    }
                    
                    saveLines.Add(i, sb1.ToString());
                    
                    // ------------------------------------save details for the last page-----------------------------------------

                    // get section for last line
                    sectionNoCheck = processing.getSectionForPara(completePageDetail.Count(), sb1.ToString(), lastLine, nextPara); // get the section value

                    if (PageIndex == doc.PageCount)
                    {
                        nextSection++;
                        saveSectionPara.Add(nextSection, sb1.ToString());
                        saveSectionSectionNo.Add(nextSection, sectionNoCheck[0]);
                        saveSectionRegex.Add(nextSection, sectionNoCheck[1]);
                    }
                    

                    if (firstParaNoSection == false)
                    {
                        saveSectionNo.Add(i, sectionNoCheck[0]);
                        saveSectionNoRegex.Add(i, sectionNoCheck[1]);
                    }
                    else if(saveLines.Count() > saveSectionNo.Count())
                    {
                        saveSectionNo.Add(i + 1, sectionNoCheck[0]);
                        saveSectionNoRegex.Add(i + 1, sectionNoCheck[1]);
                    }
                    firstParaNoSection = false;
                    savePage.Add(PageIndex, saveLines);
                    savePageSection.Add(PageIndex, saveSectionNo);
                    savePageSectionRegex.Add(PageIndex, saveSectionNoRegex);
                    var fileToDelete = filepath.Replace(fileNameVal, saveNewFile);
                    File.Delete(fileToDelete);
                    PageIndex++;
                }
                if (saveSectionPara.Count > 0)
                {
                    saveSection.Add(saveSectionPara, lastSectionPageNo);
                    saveSectionWithsectionNo.Add(saveSectionSectionNo, lastSectionPageNo);
                    saveSectionWithRegex.Add(saveSectionRegex, lastSectionPageNo);
                }
            }
            var countSection = 0;
            foreach (var item in saveSectionSectionVal)
            {
                if (item == null | item == "")
                {
                    countSection++;
                    continue;
                }

                else
                {
                    saveSectionWithsectionNo.ElementAt(countSection).Key[1] = item;
                    saveSectionWithRegex.ElementAt(countSection).Key[1] = saveSectionSectionRegex.ElementAt(countSection);
                    countSection++;
                }

            }
            // create tree for the pdf

            processing.createTree(saveSection, saveSectionWithsectionNo, saveSectionWithRegex, sectionNameFound, out finalJson); // get the section value
        }
        #endregion
        
        #region ----------------------Check section from Section Library ---------------------------------------
        // check the section dictionary
        public void checkSection(string lastLine, int lineCount, string collectSectionLib, string para, out bool section)
        {
            List<string> lastLineCheck = new List<string>(); // this is to check if the name of the section is not a part of the para
            lastLineCheck.Add("this");
            section = false;
            var checkForSection = true;
            if (lastLine != "" | lastLine != null)
            {
                foreach (var item in lastLineCheck)
                {
                    if (lastLine.Trim().EndsWith(item))
                        checkForSection = false;
                }
            }
            if (checkForSection == true)
            {
                Regex rgx = new Regex("(['^$.|?*+()\\\\])");
                string[] getSingleLib = collectSectionLib.Split('|'); // split the lib
                foreach (var item in getSingleLib) // loop through all library
                {
                    var sectionLibVal = JObject.Parse(item)["keyword"].ToString();
                    MatchCollection matchData = null;
                    MatchCollection matchExhibit = Regex.Matches("Exhibit", @"(?i)" + rgx.Replace(sectionLibVal, "\\$1"));
                    var matchCount = 0;
                    if (matchExhibit.Count > 0)
                    {
                        if (lineCount < 4)
                        {
                            matchData = Regex.Matches(para.Trim(), @"^\b\s?(?i)" + rgx.Replace(sectionLibVal, "\\$1") + "(\\s|\\b)[\"]?(([a-zA-Z]{1,2}|\\d{1,3}|(\\W?)))?[\"]?$");
                            if (matchData.Count == 0)
                            { // match found then is a section
                                matchData = Regex.Matches(para.Trim(), @"^\b\s?E(?i)xh((i|b|t|h|f|1|I|H|B|T|6|l|L)*){4,5}([a-zA-Z]{1}|\\d{1})?(\s|\b)([a-zA-Z]{1}|\d{0,3})(\W?)([a-zA-Z]{1}[\\s]?|\d{0,2}[\\s]?)$");
                            }
                            matchCount = matchData.Count;
                        }
                        else
                            matchCount = 0;
                    }
                    else if ((para).IndexOf("\"") == 0)// to check if the section library is in double quotes
                    {
                        var searchVal = (rgx.Replace(sectionLibVal, "\\$1")).Replace("\"", "");
                        matchData = Regex.Matches(para, "^(?i)[\"]" + searchVal + "[\"]([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]|\\d{0,2}[\\s])$"); // find match
                        matchCount = matchData.Count;
                    }
                    else if (para.Trim().IndexOf("\"") != -1)// different regex format
                    {
                        matchData = Regex.Matches(para.Trim(), @"^\b\s?(?i)" + rgx.Replace(sectionLibVal, "\\$1") + "(\\s|\\b)[\"]?(([a-zA-Z]{1,2}|\\d{1,3}|(\\W?)))?[\"]?$");
                        matchCount = matchData.Count;
                    }
                    else // different section format
                    {
                        matchData = Regex.Matches(para, @"^\b\s?(?i)" + rgx.Replace(sectionLibVal, "\\$1") + "[\\s]*(([a-zA-Z]{1}|\\d{1})?(\\s|\\b)([a-zA-Z]{1}|\\d{0,3})(\\W?)([a-zA-Z]{1}[\\s]?|\\d{0,2}[\\s]?)|[(|\\[]?([a-zA-Z]{1}|\\d{0,3}|x{0,2}){1,2}(ix|iv|v?i{0,3})|(X{0,2}){1,2}(IX|IV|V?I{0,3})[\\]|)|:|.|•|-])$");
                        matchCount = matchData.Count;
                    }
                    if (matchCount > 0)
                    { // match found then is a section
                        section = true;
                        break;
                    }
                }
            }
        }
        #endregion

        #region----------------------check library step2 (Not using currently) (PENDING)-------------------------------
        public void checklibrary(JArray ja2, string[] librarySet, out bool outputFound)
        {
            outputFound = false;
            Regex rgx = new Regex("(['^$.|?*+()\\\\])");
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
        #endregion

        #region -----------------------------subCase check-------------------------------------
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

        #endregion

        #region ----------------------get para after searchfor and within find--------------------------------
        public void getAllFoundText(string singleFileSectionTree, Dictionary<int, Dictionary<int, string>> savePageSection, int startPageVal, int endPageVal,int exclusionCount, string fullFilePath, int SearchWithin, Dictionary<int, Dictionary<int, string>> savePage, string fileName, JToken logic, out JArray ja, out int totalScoreDenominatorVal, out Dictionary<string, int> searchFieldScore, out Dictionary<string, int> withInScore)
        {
            withInScore = new Dictionary<string, int>();
            totalScoreDenominatorVal = 0;
            searchFieldScore = new Dictionary<string, int>();
            ja = new JArray();

            var gotResult = 0; // not got
            for (var allLogic = 0; allLogic < logic.Count(); allLogic++)
            {
                var getSearchFor = logic[allLogic]["searchFor"]; // get searchfor form the respected configuration
                var getWithIn = logic[allLogic]["withIn"];// get within form the respected configuration
                var getSubCase = logic[allLogic]["subCase"];// get subcase form the respected configuration
                var getExclusion = logic[allLogic]["exclusion"];// get exclusion form the respected configuration

                if (gotResult == 0)// all condition under 'or' 
                {
                    // get the total score of searchFor and withIn for scoring
                    getTotalScore(getWithIn, getSearchFor, out totalScoreDenominatorVal, out searchFieldScore, out withInScore);

                    Regex rgx = new Regex("(['^$.|?*+()\\\\])");
                    for (var k = 0; k < getSearchFor.Count(); k++) // loop throuch all searchFor
                    {
                        var AllSearchFieldKeyword = (getSearchFor[k]["keyword"]).ToString(); // get the search field
                        var AllSearchFieldscore = (getSearchFor[k]["score"]).ToString().ToLower(); // get the search field score
                        var AllSearchFieldCaseCheck = (getSearchFor[k]["caseCheck"]).ToString().ToLower(); // get the search field caseCheck status

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
                                if (SearchWithin == 3)
                                    processSearchForAndwithInParagraph(AllSearchFieldscore, savePage, entry, nextParaSection,paraNumber, checkPage, AllSearchFieldKeyword, rgx, AllSearchFieldCaseCheck, getSubCase,checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName,pageCount, fullFilePath, out ja1Val);
                                if (SearchWithin == 2)
                                    processSearchForAndwithInSection(singleFileSectionTree, AllSearchFieldscore, savePage, entry, nextParaSection, paraNumber, checkPage, AllSearchFieldKeyword, rgx, AllSearchFieldCaseCheck, getSubCase, checkAfterSubCaseSearchFor, SearchWithin, getWithIn, getExclusion, exclusionCount, gotResult, fileName, pageCount, fullFilePath, out ja1Val);

                                if (ja1Val.HasValues)
                                    ja.Add(ja1Val[0]);
                                nextPara++;
                            }
                            nextPage++;
                        }
                    }
                }
            }
        }
        #endregion

        #region --------------search searchfor and within in paragraph -----------------------------------
        public void processSearchForAndwithInParagraph(string AllSearchFieldscore, Dictionary<int, Dictionary<int, string>> savePage,KeyValuePair<int, Dictionary<int, string>> entry, KeyValuePair<int, string> nextParaSection, int paraNumber,KeyValuePair<int, string> checkPage, string AllSearchFieldKeyword, Regex rgx, string AllSearchFieldCaseCheck, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, int pageCount, string fullFilePath, out JArray ja)
        { 
            ja = new JArray();
            var readNextPara = 0;
            var sentenceMergeCount = 0;
            //paraNumber += 1;
            var sectionNoreadPara = "";
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
                    sectionNoreadPara = SearchWithinText;
                    foundTextFinal = getLineText;
                    sentenceMergeCount++;
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
                                                    if (sectionNoreadPara.IndexOf("|" + entry.Value[paraNumber + 1]) == -1)
                                                    {
                                                        sectionNoreadPara = sectionNoreadPara + "|" + entry.Value[paraNumber + 1];
                                                        sentenceMergeCount++;
                                                    }
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
                                                        if (sectionNoreadPara.IndexOf("|" + savePage[pageCount + 1][1]) == -1)
                                                        {
                                                            sectionNoreadPara = sectionNoreadPara + "|" + savePage[pageCount + 1][1];
                                                            sentenceMergeCount++;
                                                        }
                                                        
                                                        checkNextWithIn = false;
                                                        readNextPara = 1;
                                                        foundWithIn = true;
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
                                        if (sectionNoreadPara.IndexOf("|" + savePage[pageCount + 1][1]) == -1)
                                        {
                                            sectionNoreadPara = sectionNoreadPara + "|" + savePage[pageCount + 1][1];
                                            sentenceMergeCount++;
                                        }
                                        
                                        sectionPageNos = pageCount + "|" + (pageCount + 1);
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
                                        if (sectionNoreadPara.IndexOf(firstSentence) == -1)
                                        {
                                            sectionNoreadPara = firstSentence + "|" + sectionNoreadPara;
                                            sentenceMergeCount++;
                                        }
                                        
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
                                        if (sectionNoreadPara.IndexOf(firstSentence) == -1)
                                        {
                                            sectionNoreadPara = firstSentence + "|" + sectionNoreadPara;
                                            sentenceMergeCount++;
                                        }
                                        
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
                            jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara,sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
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
                                jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                            }
                        }
                        else
                        {
                            gotResult = 1;
                            jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName.Split('.')[0], pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                        }
                    }
                }
            }
        }
        #endregion

        #region --------------search searchFor and Within in section  (PENDING)------------------------
        public void processSearchForAndwithInSection(string singleFileSectionTree, string AllSearchFieldscore, Dictionary<int, Dictionary<int, string>> savePage, KeyValuePair<int, Dictionary<int, string>> entry, KeyValuePair<int, string> nextParaSection, int paraNumber, KeyValuePair<int, string> checkPage, string AllSearchFieldKeyword, Regex rgx, string AllSearchFieldCaseCheck, JToken getSubCase, bool checkAfterSubCaseSearchFor, int SearchWithin, JToken getWithIn, JToken getExclusion, int exclusionCount, int gotResult, string fileName, int pageCount, string fullFilePath, out JArray ja)
        {
            ja = new JArray();
            var readNextPara = 0;
            var sentenceMergeCount = 0;
            //paraNumber += 1;
            var sectionNoreadPara = "";
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
                    sectionNoreadPara = SearchWithinText;
                    foundTextFinal = getLineText;
                    sentenceMergeCount++;
                    // check if withIn values are there 
                    if (getWithIn.Count() > 0)
                    {
                        var acceptParaWithIn = "";
                        var checkExclusion = true;
                        var countWithInInAPara = 0;
                        var getTheCompleteSectionData = false;
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
                                        getTheCompleteSectionData = true;
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
                                        getNextParaToCheck = SearchWithinText.Trim() + " " + entry.Value[paraNumber + 1];
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
                                                    SearchWithinText = getNextParaToCheck;
                                                    sectionNoreadPara = sectionNoreadPara + "|" + entry.Value[paraNumber + 1];
                                                    sentenceMergeCount++;
                                                    sectionPageNos = pageCount.ToString();
                                                    checkNextWithIn = false;
                                                    readNextPara = 1;
                                                    addNextParacheck = true;
                                                    getTheCompleteSectionData = true;
                                                }
                                                else
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else if (entry.Value.Count == paraNumber + 1 && savePage.Count() > pageCount && getInSamePara == false)
                                {
                                    var hasSectionNo = Program.checkHasSectionNo(savePage[pageCount + 1][1]);
                                    if (hasSectionNo == false)
                                    {
                                        var getNextParaToCheck = SearchWithinText + savePage[pageCount + 1][1];
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
                                                        SearchWithinText = getNextParaToCheck;
                                                        sectionNoreadPara = sectionNoreadPara + "|" + savePage[pageCount + 1][1];
                                                        sentenceMergeCount++;
                                                        checkNextWithIn = false;
                                                        readNextPara = 1;
                                                        foundWithIn = true;
                                                        getTheCompleteSectionData = true;
                                                    }
                                                    else
                                                        break;
                                                }
                                            }
                                            if (foundWithIn == true)
                                            {
                                                sectionPageNos = pageCount + "|" + pageCount + 1;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        if (getTheCompleteSectionData == true)
                        {
                            var sectionNoData = Program.completeSectionPara(singleFileSectionTree, sectionNoreadPara);
                            
                            sectionNoreadPara = "";
                        }
                        if (acceptParaWithIn != "")
                        {
                            gotResult = 1;
                            jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
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
                                jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName, pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                            }
                        }
                        else
                        {
                            gotResult = 1;
                            jarrayEnter(sectionNoreadPara, sentenceMergeCount, readNextPara, sectionPageNos, getCurrentParaSearchFor, getCurrentParaScore, getSectionForPara, completeSectionText, AllSearchFieldKeyword, fileName.Split('.')[0], pageCount, SearchWithinText, acceptParaWithIn, paraNumber, fullFilePath, out ja);
                        }
                    }
                }
            }
        }
        #endregion
        
        #region ------------------------------ save data in jarray-------------------------------------
        public void jarrayEnter(string sectionNoreadPara, int sentenceMergeCount, int readNextPara, string sectionPageNos, string getCurrentParaSearchFor, int getCurrentParaScore, string getSectionForPara, string completeSectionText, string AllSearchFieldKeyword, string fileName, int pageCount, string getLineText, string acceptParaWithIn, int paraNumber, string fullFilePath, out JArray ja)
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
            jo["sentenceMergeCount"] = sentenceMergeCount; 
            jo["sectionNoreadPara"] = sectionNoreadPara; 
            ja.Add(jo);
        }
        #endregion

        #region ------------------to match regex condition for searchfor and within---------------------------------
       
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
        #endregion

        #region --------------------------SCORING---------------------------------
        // scoring para
        public void scoring(string readDuplicate, Dictionary<int, string>  PageNoMatch, string datapointName, Dictionary<int, string> OutputMatch, string LeaseName, Dictionary<int, Dictionary<int, string>> savePage, int totalScoreDenominatorVal, Dictionary<string, int> searchFieldScore, JArray ja, int multipleRead, out JArray ja1, out float finalScore)
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
                            duplicateByPage = true;
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
                        var sentenceMergeCount = getAllAcceptedText[entry.Key]["sentenceMergeCount"];
                        var sectionNoreadPara = getAllAcceptedText[entry.Key]["sectionNoreadPara"];
                        
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
                        jo1["sentenceMergeCount"] = sentenceMergeCount;
                        jo1["sectionNoreadPara"] = sectionNoreadPara;
                        
                        ja1.Add(jo1);
                        onlyTopResult = false;
                    }
                }
            }
        }
        #endregion

        #region --------check the library in whole pdf (when Connector =2) (PENDING) -------------------------------
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

        #endregion

        #region -----------remove paragraph with exclusion------------------------
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
        #endregion

        #region -------------------------start and end page of lease--------------------------------

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
        #endregion

        #region -------------------------SUMMARIZATION PROCESS--------------------------------------------
        // get all the correct sentance from all the para 
        public void collectCorrectSentance(JToken abbreviationCheckData, JToken andConditionCheck, string paraBreakCondition, int searchWithInVal,string checkNextLineVal, JToken financialSelect, JToken sentenceStart, JToken sentenceEnd, JToken SentenceResultOutputFormatCondition, JArray getSectionAndFileNameAndSearchJA, Dictionary<string, int> withInForSentence, JToken resultAllKeyword, JToken resultSearch, string copyResultOutputFormat, out string finalOutputData, out JArray collectCorrectSentanceOutput)
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
            if(searchWithInVal == 2) // section
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
            if (searchWithInVal == 3) // para
            {
                foreach (var item in getSectionAndFileNameAndSearchJA)
                {
                    var pageOutput = item["Pageoutput"].ToString();
                    getAllTheParagraph.Add(pageOutput, item["searchFor"].ToString());
                    paragraphSection.Add(item["sectionNo"].ToString());
                    paragraphFileName.Add(item["fileName"].ToString());
                    var withInVal = item["withIn"].ToString();
                    var singleWithIn = withInVal.Split('|')[0];
                    
                    var getAllBreakPoints = paraBreakCondition.Trim().Split('|');
                    Dictionary<string, int> saveSplitIndex = new Dictionary<string, int>();
                    foreach (var singleBreak in getAllBreakPoints)
                    {
                        string[] breakCount = item["Pageoutput"].ToString().Split(new string[] { singleBreak + " " }, StringSplitOptions.None);
                        saveSplitIndex.Add(singleBreak, breakCount.Count());
                    }
                    
                    var topVal = "";
                    if (saveSplitIndex.Count() > 0)
                    {
                        var top = saveSplitIndex.OrderByDescending(pair => pair.Value);
                        topVal = top.First().Key;
                    }
                    else
                        topVal = ".";
                    
                    string[] finalBreak = { };
                    
                    if (topVal == "." | paraBreakCondition.Trim() == "")
                    {
                        string[] getSentanceFullStop = pageOutput.Split(new string[] { ". " }, StringSplitOptions.None);
                        List<string> getSentanceFullStopCopy = getSentanceFullStop.ToList<string>();
                        List<string> correctSentenceBreak = new List<string>();
                        var sentenceCount = 0;
                        var textFound = false;
                        foreach (var sentence in getSentanceFullStopCopy)
                        {
                            textFound = false;
                            if (sentenceCount > 0)
                                foreach (var data in abbreviationCheckData)
                                {
                                    var startData = data["startkeyword"].ToString();
                                    var endData = data["endkeyword"].ToString();
                                    if (startData != "" & endData != "")
                                    {
                                        if (sentence.StartsWith(startData) | getSentanceFullStopCopy.ElementAt(sentenceCount - 1).EndsWith(endData))
                                        {
                                            correctSentenceBreak[correctSentenceBreak.Count() -1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                            textFound = true;
                                        }
                                    }
                                    else if (sentence.StartsWith(startData) & startData != "")
                                    {
                                        correctSentenceBreak[correctSentenceBreak.Count() - 1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                        textFound = true;
                                    }
                                    else if (getSentanceFullStopCopy.ElementAt(sentenceCount - 1).EndsWith(endData) & endData != "")
                                    {
                                        correctSentenceBreak[correctSentenceBreak.Count() - 1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                        textFound = true;
                                    }
                                }
                            else
                            {
                                correctSentenceBreak.Add(sentence);
                                textFound = true;
                            }
                            if (textFound == false)
                            {
                                correctSentenceBreak.Add(sentence);
                            }
                            sentenceCount++;
                        }
                        getSentanceFullStop = correctSentenceBreak.ToArray();

                        
                        if (getSentanceFullStop[getSentanceFullStop.Count() - 1] == "" | getSentanceFullStop[getSentanceFullStop.Count() - 1] == " ")
                            getSentanceFullStop = getSentanceFullStop.Take(getSentanceFullStop.Count() - 1).ToArray();
                        List<string> wrongStrings = new List<string>();
                        List<string> y = getSentanceFullStop.ToList<string>();
                        y.RemoveAll(p => string.IsNullOrEmpty(p));
                        getSentanceFullStop = y.ToArray();
                        for (int i = 1; i < getSentanceFullStop.Count(); i++) // checks the first character of the sentence ....
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
                    
                    else 
                    {
                        string[] getSentanceColon = pageOutput.Split(new string[] { "" + topVal + " " }, StringSplitOptions.None);
                        List<string> finalSentences = new List<string>();
                        for (int i = 0; i < getSentanceColon.Count(); i++)
                        {
                            string[] splitString = getSentanceColon[i].Split(new string[] { ". " }, StringSplitOptions.None);
                            List<string> getSentanceFullStopCopy = splitString.ToList<string>();
                            List<string> correctSentenceBreak = new List<string>();
                            var sentenceCount = 0;
                            var textFound = false;
                            foreach (var sentence in getSentanceFullStopCopy)
                            {
                                if (sentenceCount > 0)
                                    foreach (var data in abbreviationCheckData)
                                    {
                                        var startData = data["startkeyword"].ToString();
                                        var endData = data["endkeyword"].ToString();
                                        if (startData != "" & endData != "")
                                        {
                                            if (sentence.StartsWith(startData) | getSentanceFullStopCopy.ElementAt(sentenceCount - 1).EndsWith(endData))
                                            {
                                                correctSentenceBreak[sentenceCount - 1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                                textFound = true;
                                            }
                                        }
                                        else if (sentence.StartsWith(startData) & startData != "")
                                        {
                                            correctSentenceBreak[sentenceCount - 1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                            textFound = true;
                                        }
                                        else if (getSentanceFullStopCopy.ElementAt(sentenceCount - 1).EndsWith(endData) & endData != "")
                                        {
                                            correctSentenceBreak[sentenceCount - 1] = getSentanceFullStopCopy.ElementAt(sentenceCount - 1) + ". " + sentence;
                                            textFound = true;
                                        }
                                    }
                                else
                                {
                                    correctSentenceBreak.Add(sentence);
                                    textFound = true;
                                }
                                if (textFound == false)
                                {
                                    correctSentenceBreak.Add(sentence);
                                }
                                sentenceCount++;
                            }
                            splitString = correctSentenceBreak.ToArray();
                            
                            if (splitString.Count() == 1)
                                finalSentences.Add(splitString[0]);
                            else {
                                List<string> splitOnFullStop = splitString.ToList<string>();
                                List<string> wrongStringsFullStop = new List<string>();
                                splitOnFullStop.RemoveAll(p => string.IsNullOrEmpty(p));
                                for (var u = 0; u < splitOnFullStop.Count(); u++)
                                {
                                    var trimString = splitOnFullStop[u].Trim();
                                    if (splitOnFullStop.Count() > 1 & u == 0)
                                    {
                                        finalSentences.Add(trimString);
                                    }
                                    else
                                    {
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
                                    break;
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

                    for (var i = 0; i < andConditionCheck.Count(); i++)
                    {
                        var id = (int)andConditionCheck[i]["id"]; // get the id
                        var condition = andConditionCheck[i]["condition"].ToString().Split('|'); // get the condition
                        var display = (int)andConditionCheck[i]["display"]; // get the display
                        var displayCondition = andConditionCheck[i]["displayCondition"].ToString(); // get the display Condition

                        var containsData = 0;
                        foreach (var item in condition)
                        {
                            var getId = item.Replace("{{", "").Replace("}}", "").Trim();
                            if (allFormatSave.ContainsKey(Int32.Parse(getId)))
                                containsData++;

                        }
                        if (displayCondition.ToLower() == "and")
                        {
                            if (containsData == condition.Count()) // if the count of condition and the count of contain data are equal
                            {
                                if (display == 0)
                                    allFormatSave.Remove(id);
                            }
                            else if (containsData < 2) // but if the count is less then 2 i.e the count is less ten countData
                            {
                                if (display == 1) // as the condition is and so if both the values are not there then then its not required to display that id
                                    allFormatSave.Remove(id);
                            }
                        }
                        else if (displayCondition.ToLower() == "or")
                        {
                            if (containsData >= 1)
                            {
                                if(display == 0)
                                    allFormatSave.Remove(id);
                            }
                            else if (containsData == 0)
                            {
                                if (display == 1)
                                    allFormatSave.Remove(id);
                            }
                        }
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
            var sentenceFileNameCopy1 = sentenceFileName;
            var sentenceSectionCopy1 = sentenceSection;
            foreach (var orConditionVal in andConditionOrCondition)
            {
                var sentenceFileNameCopy = new List<string>();
                var sentenceSectionCopy = new List<string>();
                for (int i = 0; i < sentenceFileName.Count; i++)
                {
                    sentenceFileNameCopy.Add(sentenceFileName.ElementAt(i));
                    sentenceSectionCopy.Add(sentenceSection.ElementAt(i));
                }
                
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
                var indexValue = new List<int>();
                var indexValueCount = 0;
                foreach (var singleSentenceVal in getAllTheSentence) // check for exclusion
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
                                indexValue.Add(indexValueCount);
                                exclusionPresent = true;
                                break;
                            }
                        }
                        if(exclusionPresent == false)
                            getAllTheSentenceWithExclusion.Add(singleSentenceVal.Key, singleSentenceVal.Value);
                    }
                    else
                        getAllTheSentenceWithExclusion = getAllTheSentence;
                    indexValueCount++;
                }

                for (int i = 0; i < indexValue.Count(); i++) // remove the sentence from the list having section number
                {
                    var count = indexValue.ElementAt(i);
                    count = count - i;
                    sentenceFileNameCopy.RemoveAt(count);
                    sentenceSectionCopy.RemoveAt(count);
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
                        if (toFIndVal.Contains("##d##")) // ##d## is a number to find in sentence along with word
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
                                        if (singleSentence.Key.Trim() == item.Trim() && checkNextLineVal == "1")
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
                                var getFileName = sentenceFileNameCopy.ElementAt(next).Split('-')[0];
                                var fileNameVal = sentenceFileNameCopy.ElementAt(next).Replace(getFileName + "-", "").Trim();
                                if (orConditionCondition == 1 && count == getKeyword.Count())
                                {
                                    sentenceAsOutput = singleSentence.Key;
                                    collectCorrectSentanceOutputJO["sentence"] = singleSentence.Key;
                                    saveSentenceForDuplicate.Add(singleSentence.Key);
                                    collectCorrectSentanceOutputJO["sectionNo"] = sentenceSectionCopy.ElementAt(next);
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
                                    collectCorrectSentanceOutputJO["sectionNo"] = sentenceSectionCopy.ElementAt(next);
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
        public void buildFormat(string outputNotFoundMessage, JArray jaCompleteData, string finalOutputData, string resultOutputFormat, out string format, out string FoundText, out string finalFormat)
        {
            format = "";
            var DocumentName = "";
            var SearchFor = "";
            FoundText = "";
            finalFormat = "";
            List<string> fileNme = new List<string>();
            List<string> fileNmeCopy = new List<string>();
            List<string> sectionNumberList = new List<string>();
            for (int i = 0; i < jaCompleteData.Count; i++)
            {
                fileNme.Add(jaCompleteData[i]["fileName"].ToString());
                fileNmeCopy.Add(jaCompleteData[i]["fileName"].ToString());
            }
            
            List<string> finalFormatList = new List<string>();
            
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
                Regex regexArticle = new Regex("(?i)(article|art1c1e|art1cle|artic1e)");
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
                            if(allSectionNameCopy.Count() > 1)
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

        #endregion

        #region ------cut the sentence between start word and end word (Summarization process)---------
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
            if (output == "") // if the start word and end word dont match ....save the output as same sentence
                output = tocheck;
        }
        #endregion

        #region ------------------------Highlight Process  (PENDING) ------------------------------------------
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
        #endregion

        #region-----------------------special character before highlight---------------------------------

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
        #endregion


        #region -----------------------Abbreviation Process--------------------------------------
        // check abbreviation and replace
        public void abbreviationReplace(Dictionary<string,string> AbbreviationData,string format, out string finalFormat)
        {
            finalFormat = "";
            foreach (var item in AbbreviationData) // loop through all the abbreviation 
            {
                format = Regex.Replace(format, "(?i)" + item.Key + "(?!\\S)", item.Value, RegexOptions.IgnoreCase); // replace all the occurance of the abbreviation if found in output
            }
            finalFormat = format;
        }

        #endregion

    }
}