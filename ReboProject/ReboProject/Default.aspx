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
}
.setData tr,td {
    border: 1px solid black;
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
                    </div> 
                    JSON 1
                    <asp:TextBox id="backEndData" TextMode="multiline" Columns="150" Rows="10" runat="server" />
                    <br /><br />
                    LIBRARY
                    <asp:TextBox id="LibVal" TextMode="multiline" Columns="150" Rows="10" runat="server" />
                    <br /><br />
					<asp:Button ID="Button1" runat="server" OnClick="TestClick" Text="RESULT" />
                    <asp:TextBox ID="displayTime" runat="server"></asp:TextBox>
                    <asp:TextBox id="frontEndData" TextMode="multiline" Columns="150" Rows="10" style="display:none;" runat="server" />
                    
                    <div class="setData">

                        <table >
                            <td style="max-width:100px">
                                FILENAME
                            </td>
                            <td style="width:100px">
                                pageno
                            </td>
                            <td style="width:100px">
                                search
                            </td>
                            <td style="width:100px">
                                score
                            </td>
                            <td style="width:700px">
                                OUTPUT
                            </td>
                        </table >

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
        debugger;

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
                var leaseNameVal =data[i].leaseName;
                htmlBuilderProject.push("<tr><td style='max-width:305px'>"+"("+leaseNameVal +")  "+ fileName + "</td ><td style='width:50px'>" + pageNumber + "</td > <td style='width:100px'>" + foundText + "</td><td style='width:115px'>" + scoreVal +"%"+ "</td><td style='width:600px'>" + outputVal + "</td></tr>");
            }
            $("#allVal").html(htmlBuilderProject.join(""));
        }
        else
            $(".LeaseSilent").css("display", "block");
    });

</script>
