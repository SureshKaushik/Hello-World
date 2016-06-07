<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="IncomeTaxCalc.Web.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        Attach the ITR XML file:
        <asp:FileUpload ID="FileUploadITR" runat="server" />
&nbsp;<asp:Button ID="ButtonUploadXml" runat="server" OnClick="ButtonUploadXml_Click" Text="Submit" />
        <br />
        <br />
        PNR Number:
        <asp:TextBox ID="TextBoxPANno" runat="server"></asp:TextBox>
        <br />
        Assessment Year:
        <asp:TextBox ID="TextBoxAssessmentYear" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonGetIncomeDeduction" runat="server" Text="Get Income and Deduction" OnClick="ButtonGetIncomeDeduction_Click" />
        <br />
        <br />
        Total Income:&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="LabelTotalIncome" runat="server"></asp:Label>
        <br />
        <br />
        Total Deduction:&nbsp;
        <asp:Label ID="LabelDeduction" runat="server"></asp:Label>
        <br />
        <br />
        Taxable Income:
        <asp:Label ID="LabelTaxableIncome" runat="server"></asp:Label>
        <br />
        <br />
        Income Tax:
        <asp:Label ID="LabelIncomeTax" runat="server"></asp:Label>
        <br />
        <br />
        Education Cess (2%):
        <asp:Label ID="LabelEducationCess" runat="server"></asp:Label>
        <br />
        <br />
        Total Tax Amount: <asp:Label ID="LabelTotalTaxPayment" runat="server"></asp:Label>
        <br />
        <br />
        
    </div>
    </form>
</body>
</html>
