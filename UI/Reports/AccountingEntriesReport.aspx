<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="AccountingEntriesReport.aspx.cs" Inherits="SBM_WebUI.mp.AccountingEntriesReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });


        function selectAllChekBox(flag) {

            $("#<%=chkLJournalType.ClientID %> input:checkbox").each(function() {
                this.checked = flag;
            });
        }


        function PrintValidation() {
            var sErrorList = "";
            var isChecked = false;

            $("#<%=chkLJournalType.ClientID %> input:checkbox").each(function() {

                if ($(this).is(":checked")) {
                    isChecked = true;
                    return false;
                }
            });

            if (!isChecked) {
                sErrorList += "<li>Please select Journal Type</li>";
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


        function checkedUploadDataRange() {
            var ochkDataRange = document.getElementById('<%=chkDataRange.ClientID %>');
            var oTxtFromDate = document.getElementById('<%=txtFromDate.ClientID %>');
            var oTxtToDate = document.getElementById('<%=txtToDate.ClientID %>');
            oTxtFromDate.disabled = ochkDataRange.checked == true ? false : true;
            oTxtToDate.disabled = ochkDataRange.checked == true ? false : true;
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
        <legend>Accounting Entries Report</legend>
        <table width="85%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right" valign="top">
                    Upload Status
                </td>
                <td valign="top">
                    <div class="fieldLeft">
                        <asp:DropDownList runat="server" ID="ddlUploadStatus" SkinID="ddlMedium">
                            <asp:ListItem Value="0" Text="All"></asp:ListItem>
<%--                            <asp:ListItem Value="1" Text="ReadyForUpload"></asp:ListItem>
                            <asp:ListItem Value="2" Text="SelectedToBeUploaded"></asp:ListItem>
                            <asp:ListItem Value="3" Text="UploadFileCreated"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Uploaded"></asp:ListItem>
                            <asp:ListItem Value="5" Text="Canceled"></asp:ListItem>--%>
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right" valign="top" rowspan="5">
                    Journal Type
                </td>
                <td rowspan="5">
                            <div style="height: 190px; width:205px; overflow: auto;">
                                <asp:CheckBoxList CssClass="checkboxlist_nowrap" SkinID="SBMCheckListGreen" runat="server" ID="chkLJournalType" RepeatDirection="Vertical">
                                </asp:CheckBoxList>
                            </div>
                            <table cellpadding="0">
                                <tr>
                                    <td style="padding-top:10px"><input type="button" class="ButtonAsh" id="btnSelectAll" name="Select" value="Select All"
                                        onclick="selectAllChekBox(true)" />&nbsp;&nbsp;</td>
                                    <td style="padding-top:10px"><input type="button" class="ButtonAsh" id="btnDeselectAll" name="DeSelect" value="Deselect All"
                                        onclick="selectAllChekBox(false)" /></td>
                                </tr>
                            </table>
                        </td>
            </tr>
            <tr>
                <td align="right"></td>
                <td>
                    <asp:CheckBox runat="server" Checked="true" ID="chkDataRange" Text="Date Range" onClick="checkedUploadDataRange()" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    From Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    To Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td >
                    <asp:CheckBox runat="server" Checked="true" ID="chkAllUser" Text="All User"/>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview"  OnClick="btnPrintPreview_Click"
                        OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
