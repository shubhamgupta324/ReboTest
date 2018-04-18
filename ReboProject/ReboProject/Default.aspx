<%@ Page Title="Home Page" Language="C#"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ReboProject._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Accepted Result</title>
	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
</head>
<body>
	<form id="form1" runat="server">
	    <table class="table table-bordered">
        <thead>
            <tr class="success">
                <th>
                    Accepted Result<br /><br />
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                     <asp:DropDownList ID="DropDownList1" runat="server">
                        <asp:ListItem Text="Admin management fee" Value="0" />
                        <asp:ListItem Text="Security deposit" Value="1" />
                        <asp:ListItem Text="Alteration" Value="2" />
                        <asp:ListItem Text="Assign sublet" Value="3" />
                        <asp:ListItem Text="Holdover" Value="4" />
                        <asp:ListItem Text="Tenant audit" Value="5" />
                        <asp:ListItem Text="Tenent sublet" Value="6" />
                        <asp:ListItem Text="Late fee" Value="7" />
                        <asp:ListItem Text="Permitted use" Value="8" />
                        <asp:ListItem Text="Restoration surrender" Value="9" />
                    </asp:DropDownList><br /><br />
                    <input type="text"  class="input-text" placeholder="Enter text" style="width:500px;" runat="server" id="myTextBox" /><br /><br />
					<asp:Button ID="Button1" runat="server" OnClick="TestClick" Text="Button" />
                     <br /><br />
                    <input type="text"  class="input-text" placeholder="Output" style="width:500px;" runat="server" id="Text1" /><br /><br />
                
                </td>
            </tr>
        </tbody>
    </table>
        
    </form>
</body>
</html>
<script>

    
    
</script>
