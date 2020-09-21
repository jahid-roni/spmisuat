<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="RptGroupList.aspx.cs" Inherits="SBM_WebUI.mp.RptGroupList" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script language="javascript" type="text/javascript">

    function PrintValidation() {
        var sErrorList = "";

        sErrorList += RequiredData('<%=txtBankName.ClientID %>', 'TextBox', "Name of the Bank cannot be empty!");
        sErrorList += RequiredData('<%=txtBranchName.ClientID %>', 'TextBox', "Branch Name cannot be empty!");

        // show error divErrorList
        var divErrorPanel = document.getElementById('divErrorPanel');
        if (sErrorList.length > 0) {
            var errorList = document.getElementById('divErrorList');
            errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
            divErrorPanel.style.display = "block";

            setTimeout(PrintValidation, 800);
            return false;
        }
        else {
            divErrorPanel.style.display = "none";
            return true;
        }
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
    <%-- Error --%>
    
    
    <fieldset>
        <legend>Group List</legend>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Name of the Bank
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtBankName" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Branch Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtBranchName" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
    </fieldset>
    <br />
    
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview" OnClick="btnPrintPreview_Click"
                        OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
    
</asp:Content>