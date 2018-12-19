<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogPage.aspx.cs" Inherits="ReboProject.LogPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnerror" runat="server" Text="Error Log" OnClick="btnerror_Click" /><asp:Button ID="Btnprojecterror" runat="server" Text="Failed Project" OnClick="Btnprojecterror_Click" /><asp:Button ID="btnpassproject" runat="server" Text="Passed Project" OnClick="btnpassproject_Click" />
            <asp:Button ID="Button1" PostBackUrl="~/Default.aspx" runat="server" Text="Back" />
        </div>
        <div>
            <asp:TextBox ID="TextBox1" Width="100%" Height="500" runat="server" TextMode="MultiLine"></asp:TextBox>
        </div>
    </form>
</body>
</html>
