<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="StopPayRemoveMark.aspx.cs" Inherits="SBM_WebUI.mp.StopPayRemoveMark" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtCDUpTo]').keypress(function(e) { return intNumber(e); });
        }

        function DeleteStopPayRemove() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdIssueTransNo = document.getElementById('<%=hdIssueTransNo.ClientID %>');
            if (hdIssueTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Stop Payment Remove Mark')) {
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
                errorList.innerHTML = "<ul><li>Stop Payment Remove Mark has not loaded yet</li></ul>";
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

            sErrorList += RequiredData('<%=hdIssueTransNo.ClientID %>', 'TextBox', "Need to load first!");
            sErrorList += RequiredData('<%=txtRemoveRemarks.ClientID %>', 'TextBox', "Remarks cannot be empty!");

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

        function ButtonType(vType) {
            var hdButtonType = document.getElementById('<%=hdButtonType.ClientID %>');
            hdButtonType.value = vType;
        }

        function isGridAvailable() {
            var rowsGvgvCertInfo = $("#<%=gvCertInfo.ClientID %> tr").length;
            if (rowsGvgvCertInfo == 1 || rowsGvgvCertInfo == 0) {
                alert("No record found to remove");
                return false;
            }
            else {
                return CheckForDelete('all Stop Payment Remove Mark Certificate !');
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
        <legend runat="server" id="lgText">Unapproved Stop Payment Remove Mark List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdStopPayRemoveTransNo" />
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <asp:HiddenField runat="server" ID="hdButtonType" />
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
                                                OnClientClick="CloseErrorPanel(), ButtonType('D')" Text="Select" />
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
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Stop Payment Remove Transaction No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtStopPayRemoveTransNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnRemoveSearch" Text="Search"
                                    OnClientClick="return StopPaymentSearchPopupReturnTrue('2.1'), ButtonType('D')" /></div>
                        </td>
                        <td align="right" width="15%">
                            Remove Date
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtRemoveDate" CssClass="textInput" runat="server"
                                Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                    border="0">
                    <tr>
                        <td align="right">
                            Stop Trans No
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtStopTransNo" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Stop Date
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtStopDate" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="135px" MaxLength="25" ID="txtRegNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" Enabled="false" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnRegSearch" Text="Search" OnClientClick="return StopPaymentSearchPopupReturnTrue('3.2'), ButtonType('S')" /></div>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtIssueDate" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Stop Amount
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtStopAmount" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                            Sp Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Name
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtIssueName" CssClass="textInput" runat="server"
                                TextMode="MultiLine" Height="30px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
    <!-- End of 2 Tab panel  -->
    <br />
    <fieldset>
        <legend>Remarks Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top" width="15%">
                            Stop Remarks
                        </td>
                        <td width="35%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtStopRemarks" Height="45px" TextMode="MultiLine" Width="270px"
                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top" width="15%">
                            Remove Remarks
                        </td>
                        <td width="35%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRemoveRemarks" Height="45px" TextMode="MultiLine" Width="270px"
                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Certificate Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
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
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Mark All" ID="btnMarkAll" OnClick="btnMarkAll_Click" />
                        </td>
                        <td width="5%">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Remove All" ID="btnRemoveAll"
                                OnClick="btnRemoveAll_Click" OnClientClick="return isGridAvailable()" />
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
                                ID="btnMark" OnClick="btnMark_Click" />
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
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvCertInfo_RowDataBound"
                                    OnRowDeleting="gvCertInfo_RowDeleting" OnRowCommand="gvCertInfo_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" CommandName="Remove" runat="server" ID="btnRemove"
                                                    OnClientClick="return CheckForDelete('this Stop Payment Remove Mark Certificate !')"
                                                    Text="Remove" />
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
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteStopPayRemove()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:StopPay ID="StopPay" runat="server" Type="15" Title="Stop Payment Remove Search" />
    <uc4:SIssue ID="SIssue" runat="server" Type="5" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
