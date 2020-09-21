<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="BBDataExportReports.aspx.cs" Inherits="SBM_WebUI.mp.BBDataExportReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });



        function PrintValidation() {
            return;
            // end of show error divErroList
        }
        
        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <fieldset>
        <legend>Bangladesh Bank Excel Export</legend>
        <table width="85%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right" valign="top">
                    SP Type
                </td>
                <td align="left"  valign="top" >
                    <asp:DropDownList ID="ddlSpType" SkinID="ddlLarge" runat="server" AutoPostBack="True" TabIndex="1">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right" valign="top">
                    Report Type
                </td>
                <td align="left"  valign="top" >
                    <asp:RadioButtonList runat="server" ID="rdlStatus" RepeatDirection="Vertical">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td width="13%" align="right">
                    From Date
                </td>
                <td width="28%">
                    <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    To Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    &nbsp;</td>
                <td>
                    <asp:Button CssClass="ButtonAsh" Width="130px" runat="server" Text="Export Date" ID="btnPrintPreview"
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    &nbsp;</td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
