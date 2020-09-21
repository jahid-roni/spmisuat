<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="StopPayMark.aspx.cs" Inherits="SBM_WebUI.mp.StopPayMark" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtCDUpTo]').keypress(function(e) { return intNumber(e); });
        }

        function DeleteStopPay() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIntTransNo = document.getElementById('<%=hdStopPayTransNo.ClientID %>');
            if (hdIntTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Stop Payment Mark')) {
                    //Reset();
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
                errorList.innerHTML = "<ul><li>Stop Payment Mark has not loaded yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
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

        function SaveValidation() {
            var sErrorList = "";

            var rowsGvgvCertInfo = $("#<%=gvCertInfo.ClientID %> tr").length;

            sErrorList += RequiredData('<%=txtRemarks.ClientID %>', 'TextBox', "Remarks cannot be null!");
            sErrorList += RequiredData('<%=hdIssueTransNo.ClientID %>', 'TextBox', "Need to load first!");

            if (rowsGvgvCertInfo == 1 || rowsGvgvCertInfo == 0) {
                sErrorList += "<li>Certificate Detail cannot be null</li>";
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

        function isGridAvailable() {
            var rowsGvgvCertInfo = $("#<%=gvCertInfo.ClientID %> tr").length;
            if (rowsGvgvCertInfo == 1 || rowsGvgvCertInfo == 0) {
                alert("No record found to remove");
                return false;
            }
            else {
                return CheckForDelete('all Stop Payment Certificate !');
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
        <legend runat="server" id="lgText">Unapproved Stop Payment Mark List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdStopPayTransNo" />
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
                                                Text="Select" OnClientClick="CloseErrorPanel()" />
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
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Stop Payment Mark Transaction No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtStopPayTransNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnStopPaySearch" Text="Search"
                                    OnClientClick="return StopPaymentSearchPopupReturnTrue('1.1')" /></div>
                        </td>
                        <td align="right">
                            Stop Payment Date
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtStopPaymentDate" CssClass="textInput" Enabled="false" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                    border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Year
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlYear" SkinID="ddlCommon" Width="105px" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlCommon" Width="120px" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Registration No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="25" ID="txtRegNo" Width="160px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" OnClick="btnRegSearch_Click" runat="server" Text="Search"
                                    ID="btnRegSearch" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegNo')" /></div>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalAmount" Width="116px" CssClass="textInput" runat="server"
                                MaxLength="19" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Name
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtIssueName" Width="300px" CssClass="textInput" runat="server"
                                TextMode="MultiLine" Height="30px" MaxLength="19" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Stop Payment Comment</legend>
        <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right" valign="top" width="15%">
                    Remarks
                </td>
                <td width="85%">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRemarks" Height="35px" TextMode="MultiLine" Width="620px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
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
                                OnClientClick="return isGridAvailable()" OnClick="btnRemoveAll_OnClick" />
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
                            <asp:TextBox Width="120px" ID="txtCDTotalAmount" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div style="height: 160px; width: 100%; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvCertInfo" runat="server" AutoGenerateColumns="true"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowDeleting="gvCertInfo_RowDeleting"
                                    OnRowCommand="gvCertInfo_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" CommandName="Remove" runat="server" ID="btnRemove"
                                                    OnClientClick="return CheckForDelete('this Stop Payment Certificate !')" Text="Remove" />
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
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteStopPay()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:StopPay ID="StopPay" runat="server" Type="14" Title="Stop Payment Search" />
    <uc4:SIssue ID="SIssue" runat="server" Type="5" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
