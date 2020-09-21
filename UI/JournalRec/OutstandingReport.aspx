<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="OutstandingReport.aspx.cs" Inherits="SBM_WebUI.mp.OutstandingReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <fieldset>
        <legend>Outstanding Report</legend>
        <table width="98%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    Type
                </td>
                <td >
                    <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rdlType">
                        <asp:ListItem Text="Current Journal"></asp:ListItem>
                        <asp:ListItem Text="Archive"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td align="right">
                    Account Name
                </td>
                <td >
                    <asp:Label ID="lblAccName" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="right" width="20%">
                    Sp Type
                </td>
                <td width="30%">
                    <asp:DropDownList runat="server" ID="ddlSpType">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td width="20%" align="right">
                    Account No
                </td>
                <td width="30%">
                    <asp:DropDownList runat="server" ID="ddlAccountNo">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Payment From Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="154px" ID="txtPaymentFromDate"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    
                </td>
                <td align="right">
                    Payment To Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtPaymentToDate"
                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        
                    </div>
                    <div class="errorIcon">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Blank" ID="btnPaymentBlank" /></div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Recon From Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="154px" ID="txtReconFromDate"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    
                </td>
                <td align="right">
                    Recon To Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtReconToDate"
                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        
                    </div>
                    <div class="errorIcon">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Blank" ID="btnReconBlank" /></div>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="3">
                    <asp:CheckBox runat="server" ID="chkAscending" Text="Ascending" />
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Out Standing" 
                        ID="btnOutStanding" onclick="btnOutStanding_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reconciled" ID="btnReconciled" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Export" ID="btnExport" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
