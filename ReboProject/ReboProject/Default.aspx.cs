using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace ReboProject
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void TestClick(object sender, EventArgs e)
        {
            string val = DropDownList1.SelectedValue; // get the datapoint 
            var accptedValThere = 0;
            var getVerbiage = myTextBox.Value; // Verbiage in which data to search
            var textLength =getVerbiage.Length; // get the length of the text
            var endIndex =0;
            Dictionary<string, string> searchField = new Dictionary<string, string>();
            if (val == "0")
            { // if Admin management fee

                // get all the found text in dictionary
                searchField.Add("administration fee", null);
                searchField.Add("management fee", null);
                searchField.Add("administrative cost", null);
                searchField.Add("management cost", null);
                searchField.Add("overhead charges", null);
                searchField.Add("administrative services", null);
                searchField.Add("management services", null);
                searchField.Add("administrative expenses", null);
                searchField.Add("management expenses", null);
                searchField.Add("cost of management", null);
                searchField.Add("cost of administration", null);
                searchField.Add("cost of supervision", null);

                foreach (KeyValuePair<string, string> entry in searchField)
                {
                    var searchFieldVal = entry.Key; // get the first search field
                    var matchData = Regex.Matches(getVerbiage, @"\b\s?" + searchFieldVal + "\\w*\\b"); // find match
                    if (matchData.Count > 0) // if match there
                    {
                        accptedValThere = 1; // result found
                        var startIndex = getVerbiage.IndexOf(searchFieldVal); // get the index of search field
                        var getTheTextBeforeWord =getVerbiage.Substring(0, startIndex); // get the string before word to find the full stop
                        var getTheFullStopBeforeWord =getTheTextBeforeWord.LastIndexOf('.'); // get the index of the full stop
                        if (getTheFullStopBeforeWord == -1) // full stop not present
                        {
                            getTheFullStopBeforeWord = 0;
                            endIndex = textLength;
                        }
                        else 
                        {
                            endIndex = textLength - getTheFullStopBeforeWord - 1; // get the char count after full stop
                            getTheFullStopBeforeWord += 1;// to avoid full stop in the accepted result
                        } 
                        var getTheTextAfterFullStopTillEnd = getVerbiage.Substring(getTheFullStopBeforeWord, endIndex); // get the result from full stop to end of Verbiage text
                        var getTheFullStopAfterWord =getTheTextAfterFullStopTillEnd.IndexOf('.');// get the full stop after found text
                        if (getTheFullStopAfterWord == -1) // full stop not present
                        {
                            getTheFullStopAfterWord = endIndex; 
                        }
                        else
                            getTheFullStopAfterWord += 1; // to get the last full stop in accepted string
                        string sub = getTheTextAfterFullStopTillEnd.Substring(0, getTheFullStopAfterWord); // get the final accepted result 
                        Text1.Value = sub; // set the value of accepted result
                        break;
                    }
                }
            }
            if (accptedValThere == 0)
            {
                Text1.Value = "Lease is silent";
            }
            
        }
    }
}