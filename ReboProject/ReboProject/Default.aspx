<%@ Page Title="Home Page" Language="C#"  AutoEventWireup="true" ValidateRequest = "false" CodeBehind="Default.aspx.cs" Inherits="ReboProject._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Accepted Result</title>
	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <link href="css/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.0/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <style>
.setData {
    border: 1px solid black;
    table-layout: fixed;
    width: 100%;
}
.setData tr,td {
    border: 1px solid black;
}
.setData td {
    width: 185px!important;
}

.setData td:nth-child(2), .setData td:nth-child(3), .setData td:nth-child(4) {
    width: 60px!important;
}

.setData td:nth-child(5) {
    width: 100px!important;
}

.setData td:nth-child(6), .setData td:nth-child(7) {
    width: 390px!important;
}
</style>
</head>
<body>

	<form id="form1" runat="server">
	    <table class="table table-bordered">
        <thead>
            <tr class="success">
                <th>
                    Accepted Result<br /><br />
                    <asp:Label runat="server" ID="lblStatus"></asp:Label>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td style="padding:10px;">
                  
                    <p>JSON 1</p>
                    <asp:TextBox id="backEndData" TextMode="multiline" Width="100%" Columns="150" Rows="10" runat="server" />
                  
                  <%--  <p>LIBRARY</p> 
                    <asp:TextBox id="LibVal" TextMode="multiline" Width="100%" Columns="150" Rows="10" runat="server" />
                    <br />--%>
					<asp:Button ID="Button1" runat="server" OnClick="TestClick" Text="RESULT" />
                    <%--<input type="button" onclick="tableToExcel('.allVal', 'name12')" value="Export to Excel">--%>
                    <asp:TextBox ID="displayTime" runat="server"></asp:TextBox>
                    <asp:Button ID="btnlog" runat="server" PostBackUrl="~/LogPage.aspx"  Text="View Log" OnClick="btnlog_Click" />
                    <asp:TextBox id="frontEndData" TextMode="multiline" Columns="150" Rows="10" style="display:none;" runat="server" />
                    
                    
                    <div class="setData">

                        <table style="width:100%" >
                            <td style="max-width:100px;">
                                FILENAME
                            </td>
                            <td style="width:100px;">
                                pageno
                            </td>
                            <td style="width:100px;">
                                search
                            </td>
                            <td style="width:100px;">
                                score
                            </td>
                            <td style="width:50px;">
                                datapoint
                            </td>
                            <td style="width:700px;">
                                OUTPUT
                            </td>
                            <td style="width:700px;">
                                Sentences
                            </td>
                        </table>

                        <table class="setData" id="allVal">
                            
                        </table>
                        <div class="LeaseSilent" style="display:none;">
                            <input type="text"  class="input-text" placeholder="Enter text" style="width:500px;" runat="server" id="Text1" /><br /><br />
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
        
    </form>
</body>
</html>
<script type="text/javascript">

    function generateJson() {DivFoundText = $(".fndTxtDetailContent");
        
        value = { 'folder': '', 'FileOrder': { 'sort': fileOrderSort, 'type': fileOrderType }, 'SearchWithin': '1', 'searchFor': jsonObjFoundText, 'result': { 'section': '1', 'format': resultFormat } };

        $("#backEndData").val(JSON.stringify(value));
        $("#Button1").click();
    }

    $(document).ready(function () {
    
        $(".LeaseSilent").css("display", "none");
        var jsonVal = $("#frontEndData").val();
        htmlBuilderProject = [];
        data = $.parseJSON(jsonVal)
        if (data.length != 0) {
            for (var i = 0; i < data.length; i++) {
                var fileName = data[i].fileName;
                var foundText = data[i].AllSearchFieldKeyword;
                var pageNumber = data[i].pageNo;
                var outputVal = data[i].output;
                var scoreVal = data[i].score;
                var foundWithInVal = data[i].foundWithIn;
                var leaseNameVal = data[i].leaseName;
                var dataPointName = data[i].dataPointName;
                var correctStringVal =data[i].correctString;
                htmlBuilderProject.push("<tr><td style='width:100px;word-break: break-all;'>"+"("+leaseNameVal +")  "+ fileName + "</td ><td style='width:50px;word-break: break-all;'>" + pageNumber + "</td > <td style='width:100px;word-break: break-all;'>" + foundText + "</td><td style='width:100px;word-break: break-all;'>" + scoreVal + "</td><td style='width:80px;word-break: break-all;'>" + dataPointName + "</td><td style='width:600px;word-break: break-all;'>" + outputVal + "</td><td style='width:600px;word-break: break-all;'>" + correctStringVal + "</td></tr>");

            }
            $("#allVal").html(htmlBuilderProject.join(""));
        }
        else
            $(".LeaseSilent").css("display", "block");
    });

//var tableToExcel = (function () {
//        var uri = 'data:application/vnd.ms-excel;base64,'
//        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>'
//        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
//        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
//        return function (table, name, filename) {
//            if (!table.nodeType) table = document.getElementById(table)
//            var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }

//            document.getElementById("dlink").href = uri + base64(format(template, ctx));
//            document.getElementById("dlink").download = filename;
//            document.getElementById("dlink").click();

//        }
//    })()

</script>
