<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="RptAdminActivity.aspx.cs" Inherits="SBM_WebUI.mp.RptAdminActivity" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";

                setTimeout(RejectValidation, 800);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                return true;
            }
            // end of show error divErroList
        }


        function PrintValidation() {
            var sErrorList = "";
            var isChecked = false;

            $("#<%=chkLSpType.ClientID %> input:checkbox").each(function() {

                if ($(this).is(":checked")) {
                    isChecked = true;
                    return false;
                }
            });

            if (!isChecked) {
                sErrorList += "<li>Please select SP Type!</li>";
            }
            sErrorList += RequiredData('<%=ddlUserName.ClientID %>', 'DropDownList', "User Name cannot be empty!");
            sErrorList += RequiredData('<%=txtFromDate.ClientID %>', 'TextBox', "From Date cannot be empty!");
            sErrorList += RequiredData('<%=txtToDate.ClientID %>', 'TextBox', "To Date cannot be empty!");

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
    <%-- Error --%>
    <br />
    <fieldset>
        <legend>Admin Activity Report</legend>
        <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    Name of the Bank
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox Width="160px" ID="txtBankName" CssClass="textInput" ReadOnly="true"
                            runat="server"></asp:TextBox>
                    </div>
                </td>
                <td align="right">
                    Branch Name
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox Width="160px" ID="txtBranchName" CssClass="textInput" ReadOnly="true"
                            runat="server"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Activity Report on
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox Width="160px" ID="txtReporDate" CssClass="textInput" runat="server"
                            ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right">
                    User
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:DropDownList ID="ddlUserClass" runat="server" SkinID="ddlMedium">
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview"
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
