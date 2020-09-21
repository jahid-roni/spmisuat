<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="LMRemove.aspx.cs" Inherits="SBM_WebUI.mp.LMRemove" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchLienMark.ascx" TagName="LienMark" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtCDUpTo]').keypress(function(e) { return intNumber(e); });
        }

        function SaveValidation() {
            var sErrorList = "";

            var rowsGvgvCertInfo = $("#<%=gvCertInfo.ClientID %> tr").length;

            sErrorList += RequiredData('<%=hdIssueTransNo.ClientID %>', 'TextBox', "Need to load first!");
            sErrorList += RequiredData('<%=txtOurRefNo.ClientID %>', 'TextBox', "Our Reference cannot be empty!");
            sErrorList += RequiredData('<%=txtTheirRefNo.ClientID %>', 'TextBox', "Their Reference cannot be empty!");

            if (rowsGvgvCertInfo == 1 || rowsGvgvCertInfo == 0) {
                sErrorList += "<li>Certificate Detail cannot be empty!</li>";
            }

            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";

                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                MsgPopupReturnTrue('Save');
                return true;
            }
            // end of show error divErroList
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

                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                MsgPopupReturnTrue('Reject');
                return true;
            }
            // end of show error divErroList
        }

        function DeleteLienMarkRemove() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIssueTransNo = document.getElementById('<%=hdIssueTransNo.ClientID %>');
            if (hdIssueTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Lien Mark Remove')) {
                    // Reset();
                    CloseErrorPanel();
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Lien Mark Remove has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
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
    <fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved Lien Mark Remove Mark List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdLienRemoveTransNo" />
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                OnRowDataBound="gvData_RowDataBound" ID="gvData" runat="server" SkinID="SBMLGridGreen"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />--%>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Unlien Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="15%">
                            Unlien Trans No
                        </td>
                        <td width="40%">
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtUnlienTransNo" ReadOnly="true" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnUnLienSearch" Text="Unlien Search"
                                    OnClientClick="return StopPaymentSearchPopupReturnTrue('4.2') , CloseErrorPanel() " /></div>
                        </td>
                        <td align="right" width="15%">
                            Unlien Date
                        </td>
                        <td width="30%">
                            <asp:TextBox Width="160px" ID="txtUnlienDate" CssClass="textInput" runat="server"
                                Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="1" cellspacing="1"
                    border="0">
                    <tr>
                        <td align="right">
                            Lien Trans No
                        </td>
                        <td>
                            <asp:TextBox ID="txtLienTransNo" Enabled="false" Width="157px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" Enabled="false" SkinID="ddlMedium" runat="server"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Lien Amount
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtLienAmount" Enabled="false" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="157px" ID="txtRegNo" Enabled="false" CssClass="textInput" runat="server"
                                    MaxLength="25" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" OnClientClick="return LienMarkSearchPopupReturnTrue('1.2')"
                                    ID="btnRegSearch" Text="Search" /></div>
                        </td>
                        <td align="right">
                            Sp Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" Enabled="false" runat="server"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtIssueDate" Enabled="false" CssClass="textInput"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCustomerType" Enabled="false" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtIssueName" Width="180px" Enabled="false" CssClass="textInput"
                                runat="server" TextMode="MultiLine" Height="35px"></asp:TextBox>
                        </td>
                        <td align="right">
                            Lien Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtLienDate" Width="120px" Enabled="false" CssClass="textInput"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <!-- start to 2Tab panel  -->
    <fieldset>
        <legend>Customer(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                <ContentTemplate>
                    <div style="height: 100%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustomerDetail" runat="server"
                            AutoGenerateColumns="true" SkinID="SBMLGridGreen">
                            <Columns>
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Customer(s) Details Tab  --%>
        <%-- Nominee(s) Details Tab  --%>
        <br />
    <fieldset>
        <legend>Nominee(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div style="height: 100%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDetail" runat="server" AutoGenerateColumns="true"
                            SkinID="SBMLGridGreen">
                            <Columns>
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Lien Detail</legend>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="1" cellspacing="1"
                    border="0">
                    <tr>
                        <td valign="top" align="right">
                            Our Ref. No
                        </td>
                        <td width="27%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtOurRefNo" Width="200px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="25"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td valign="top" align="right">
                            Lien Bank
                        </td>
                        <td>
                            <asp:TextBox ID="txtLienBank" Width="220px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" MaxLength="35"></asp:TextBox>
                        </td>
                        <td align="right">
                            Their Ref. No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtTheirRefNo" Width="100px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    MaxLength="25"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Remarks
                        </td>
                        <td>
                            <asp:TextBox ID="txtRemarks" Height="70px" TextMode="MultiLine" Width="220px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                MaxLength="255"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Address
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtBankAddress" Height="70px" TextMode="MultiLine" Width="220px"
                                CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                MaxLength="255"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Certificate Detail</legend>
        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="0"
                    border="0">
                    <tr>
                        <td align="right">
                            Denomination
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList SkinID="ddlMedium" ID="ddlCDDenom" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlCDDenom_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td>
                            Up to
                        </td>
                        <td width="5%">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Mark All" ID="btnMarkAll" OnClick="btnMarkAll_OnClick" />
                        </td>
                        <td width="5%">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Remove All" ID="btnRemoveAll"
                                OnClientClick="return CheckForDelete('all Lien Remove Certificate(s) !')" OnClick="btnRemoveAll_OnClick" />
                        </td>
                        <td align="right">
                            Total Amount
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Certificate No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCDCertif" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td>
                            <asp:TextBox Width="80px" ID="txtCDUpTo" MaxLength="2" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Mark&nbsp;&nbsp;&nbsp;"
                                ID="btnMark" OnClick="btnMark_OnClick" />
                        </td>
                        <td>
                        </td>
                        <td align="right">
                            <asp:TextBox Width="120px" ID="txtCDRemoveAmount" Enabled="false" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div style="height: 100px; width: 100%; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvCertInfo" runat="server" AutoGenerateColumns="true"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowDeleting="gvCertInfo_RowDeleting"
                                    OnRowCommand="gvCertInfo_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" CommandName="Remove" runat="server" ID="btnRemove"
                                                    OnClientClick="return CheckForDelete('this Lien Remove Certificate !')" Text="Remove" />
                                                <asp:HiddenField runat="server" ID="hdOldStatus" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" OnClientClick="return NotDoneYet()" runat="server"
                        Text="View Journals" ID="btnViewJournals" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteLienMarkRemove()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
    <uc2:StopPay ID="StopPay" runat="server" type="18" title="Lien Remove Search" />
    <uc4:LienMark ID="LienMark" runat="server" type="17" title="Lien Search" />
</asp:Content>
