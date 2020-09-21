<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="DataUploadReport.aspx.cs" Inherits="SBM_WebUI.mp.DataUploadReport" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });
  
  
  

        function PrintValidation() {
            var sErrorList = "";
            var isAtlestOneSelect = false;
            
            
            //chkAccEntryOper
            var chkAccEntryOper = document.getElementById('<%=chkAccEntryOper.ClientID %>');
            if (chkAccEntryOper.checked == true) {
                sErrorList += RequiredData('<%=ddlOperator.ClientID %>', 'DropDownList', "Accounting Entry Operation cannot be empty!");
                isAtlestOneSelect = true;
            }
            
            //chkJournalType
            var chkJournalType = document.getElementById('<%=chkJournalType.ClientID %>');
            if (chkJournalType.checked == true) {
                sErrorList += RequiredData('<%=ddlJournalType.ClientID %>', 'DropDownList', "Journal Type cannot be empty!");
            }
            
            // chkUploadDataRange
            var chkUploadDataRange = document.getElementById('<%=chkUploadDataRange.ClientID %>');
            if (chkUploadDataRange.checked == true) {
                sErrorList += RequiredData('<%=txtFromDate.ClientID %>', 'TextBox', "From Date cannot be empty!");
                sErrorList += RequiredData('<%=txtToDate.ClientID %>', 'TextBox', "To Date cannot be empty!");
                isAtlestOneSelect = true;
            }

            if (sErrorList.length == 0 && isAtlestOneSelect == false) {
                sErrorList += "You must select at least one option from ‘Upload Date Range’ and ‘Accounting Entry Operation’ ";
            }
            

            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";
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
    
    <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
        <tr>
            <td width="50%" valign="top">
                <fieldset>
                    <legend>
                        <asp:CheckBox runat="server" ID="chkAccEntryOper" Text="Accounting Entry Operation" onClick="checkedEntryOper()" /></legend>
                    <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
                        <tr>
                            <td align="right" valign="top">Accounting Entry Operator</td>
                            <td align="right" valign="top">
                                <div class="fieldLeft"><asp:DropDownList runat="server" ID="ddlOperator" SkinID="ddlMedium">                                    
                                </asp:DropDownList></div>
                    <div class="errorIcon">
                        *</div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td width="50%" valign="top">
                <fieldset>
                    <legend>
                        <asp:CheckBox runat="server" ID="chkJournalType" Text="Journal Type" onClick="checkedJournalType()" /></legend>
                    <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
                        <tr>
                            <td align="right" valign="top">Journal Type</td>
                            <td align="right" valign="top">
                                <div class="fieldLeft"><asp:DropDownList runat="server" ID="ddlJournalType" Width="240px">
                                </asp:DropDownList></div>
                    <div class="errorIcon">
                        *</div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <fieldset>
                    <legend>
                        <asp:CheckBox runat="server" ID="chkUploadDataRange" Text="Upload Date Range" onClick="checkedUploadDataRange()" /></legend>
                    <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
                        <tr>
                            <td align="right">
                                From Date
                            </td>
                            <td>
                                <div class="fieldLeft"><asp:TextBox runat="server" CssClass="textInput" Width="150px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                    <div class="errorIcon">
                        *</div>
                                
                            </td>
                            <td align="right">
                                To Date
                            </td>
                            <td>
                                <div class="fieldLeft"><asp:TextBox runat="server" CssClass="textInput" Width="150px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                    <div class="errorIcon">
                        *</div>
                                
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview" OnClick="btnPrintPreview_Click"
                        OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>



<script language="javascript" type="text/javascript">
    
    function checkedEntryOper()
    {
        var oChkAccEntryOper = document.getElementById('<%=chkAccEntryOper.ClientID %>');
        var oDdlOperator = document.getElementById('<%=ddlOperator.ClientID %>');

        oDdlOperator.selectedIndex = 0;
        oDdlOperator.disabled = oChkAccEntryOper.checked == true ? false : true;
    }
    
    function checkedJournalType()
    {
        var oChkJournalType = document.getElementById('<%=chkJournalType.ClientID %>');
        var oDdlJournalType = document.getElementById('<%=ddlJournalType.ClientID %>');

        oDdlJournalType.selectedIndex = 0;
        oDdlJournalType.disabled = oChkJournalType.checked == true ? false : true;
    }
    
    function checkedUploadDataRange()
    {
        var oChkUploadDataRange = document.getElementById('<%=chkUploadDataRange.ClientID %>');
        var oTxtFromDate = document.getElementById('<%=txtFromDate.ClientID %>');
        var oTxtToDate = document.getElementById('<%=txtToDate.ClientID %>');

        oTxtFromDate.disabled = oChkUploadDataRange.checked == true ? false : true;
        oTxtToDate.disabled = oChkUploadDataRange.checked == true ? false : true;
    }
    
    function initDataLoad(bStatus) {
    
        var oChkAccEntryOper = document.getElementById('<%=chkAccEntryOper.ClientID %>');
        var oDdlOperator = document.getElementById('<%=ddlOperator.ClientID %>');

        oChkAccEntryOper.disabled = bStatus == true ? false : true;
        oDdlOperator.disabled = bStatus;

        var oChkJournalType = document.getElementById('<%=chkJournalType.ClientID %>');
        var oDdlJournalType = document.getElementById('<%=ddlJournalType.ClientID %>');

        oChkJournalType.disabled = bStatus == true ? false : true;
        oDdlJournalType.disabled = bStatus;

    
        var oChkUploadDataRange = document.getElementById('<%=chkUploadDataRange.ClientID %>');
        var oTxtFromDate = document.getElementById('<%=txtFromDate.ClientID %>');
        var oTxtToDate = document.getElementById('<%=txtToDate.ClientID %>');
    
        oChkUploadDataRange.disabled = bStatus == true ? false : true;
        oTxtFromDate.disabled = bStatus;
        oTxtToDate.disabled = bStatus;
    }
    
    
    //initDataLoad(true)
    
</script>
</asp:Content>

