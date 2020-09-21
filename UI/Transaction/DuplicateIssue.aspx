<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="DuplicateIssue.aspx.cs" Inherits="SBM_WebUI.mp.DuplicateIssueScript" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {

            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtIssueAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtDDQuantity]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtTotalStopAmount]').keypress(function(e) { return floatNumber(e); });
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SaveDuplicateValidation() {
            var hdDataType = document.getElementById('<%=hdDataType.ClientID %>');


            if (hdDataType != null) {
                if (hdDataType.value == "2") {
                    return true;
                }
            }

            var sErrorList = "";
            var rowsGvgvStopPayment = $("#<%=gvStopPayment.ClientID %> tr").length;
            var rowsGvgvDenomDetai = $("#<%=gvDenomDetail.ClientID %> tr").length;

            sErrorList += RequiredData('<%=hdIssueTransNo.ClientID %>', 'TextBox', "Need to load first!");
            if (rowsGvgvStopPayment == 1 || rowsGvgvStopPayment == 0) {
                sErrorList += "<li>Stop Certificate Details cannot be null</li>";
            }

            if (rowsGvgvDenomDetai == 1 || rowsGvgvDenomDetai == 0) {
                sErrorList += "<li>Replace Denomination cannot be null</li>";
            }

            sErrorList += RequiredData('<%=txtTotalStopAmount.ClientID %>', 'TextBox', "Total Amount of Stop Certificate Details cannot be empty!");
            sErrorList += RequiredData('<%=txtTotalAmount.ClientID %>', 'TextBox', "Total Amoun of Replace Denomination cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RemoveTransNoSearch() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtRemoveTransNo.ClientID %>', 'TextBox', "Remove Transaction No cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function SaveDenomValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlDDDenom.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtDDQuantity.ClientID %>', 'TextBox', "Quantity cannot be empty!");
            sErrorList += RequiredData('<%=txtDuplicateMark.ClientID %>', 'TextBox', "Duplicate Mark cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function DeleteDuplicate() {

            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }

            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdEncashTransNo = document.getElementById('<%=hdDuplicateIssueTransNo.ClientID %>');

            if (hdEncashTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Duplicate Issue')) {
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
                errorList.innerHTML = "<ul><li>Duplicate Issue not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function ButtonType(vType) {
            var hdButtonType = document.getElementById('<%=hdButtonType.ClientID %>');
            hdButtonType.value = vType;
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
        <legend runat="server" id="lgText">Unapproved Duplicate Issue List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdDuplicateIssueTransNo" />
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Duplicate Issue Detail</legend>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right" width="20%">
                            Trans No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtRemoveTransNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnRemoveSearch" Text="Search"
                                    OnClientClick="return StopPaymentSearchPopupReturnTrue('3.1'), ButtonType('D')" /></div>
                        </td>
                        <td align="right">
                            Duplicate Issue Date
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtDuplicateIssueDate" CssClass="textInput" Enabled="false"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Detail</legend>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                    border="0">
                    <tr>
                        <td align="right">
                            Stop Trans No
                        </td>
                        <td>
                            <asp:TextBox Width="158px" ID="txtStopTransNo" CssClass="textInput" runat="server"
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
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlCommon" Width="123px" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="158px" MaxLength="25" ID="txtRegNo" CssClass="textInput" runat="server"
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
                            <asp:TextBox Width="120px" ID="txtStopAmount" CssClass="textInput" runat="server"
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
                            Issue Name
                        </td>
                        <td colspan="3">
                            <asp:TextBox Width="406px" ID="txtIssueName" CssClass="textInput" runat="server"
                                TextMode="MultiLine" Height="30px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Sp Type
                        </td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlCommon" Width="400px" runat="server"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Master No
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtMasterID" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
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
        </div>
        <%-- End of Nominee(s) Details Tab  --%>
    </fieldset>
    <!-- End of 2 Tab panel  -->
    <br />
    <fieldset>
        <legend>Remarks</legend>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            Stop Remarks
                        </td>
                        <td width="35%">
                            <asp:TextBox ID="txtStopRemarks" Height="70px" TextMode="MultiLine" Width="270px"
                                CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Issue Remarks
                        </td>
                        <td width="35%">
                            <asp:TextBox ID="txtIssueRemarks" Height="70px" TextMode="MultiLine" Width="270px"
                                CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="0"
        border="0">
        <tr>
            <td width="35%" valign="top">
                <fieldset>
                    <legend>Stop Certificate(s) Details</legend>
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td colspan="2">
                                        <div style="height: 130px; width: 100%; overflow: auto;">
                                            <asp:GridView Style="width: 98%" ID="gvStopPayment" runat="server" AutoGenerateColumns="true"
                                                SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvStopPayment_RowDataBound">
                                                <Columns>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Total Stop Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="120px" ID="txtTotalStopAmount" ReadOnly="true" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
            </td>
            <td valign="top" width="65%">
                <fieldset>
                    <legend>Duplicate Denomination(s) details</legend>
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="0"
                                border="0">
                                <tr>
                                    <td width="60%" valign="top">
                                        <div style="height: 168px; width: 98%; overflow: auto; border: solid 1px white">
                                            <asp:GridView Style="width: 94%" ID="gvDenomDetail" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvDenomDetail_RowCommand">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select">
                                                        <ItemStyle Width="15px" />
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" OnClientClick="return CheckForDelete('this Denomination!')"
                                                                ID="btnDenomRemove" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Denomination" HeaderText="Denomination" />
                                                    <asp:BoundField DataField="DuplicateMark" HeaderText="Duplicate Mark" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No Record Found.
                                                </EmptyDataTemplate>
                                                <HeaderStyle CssClass="ssHeader" />
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td width="35%" valign="top">
                                        <table width="100%" align="center" class="tableBody" border="0">
                                            <tr>
                                                <td align="right">
                                                    Denomination
                                                </td>
                                                <td>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlDDDenom" SkinID="ddlCommon" Width="100px" runat="server">
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="errorIcon">
                                                        *</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    Quantity
                                                </td>
                                                <td>
                                                    <asp:TextBox MaxLength="4" Width="100px" ID="txtDDQuantity" CssClass="textInput"
                                                        runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">
                                                    Duplicate Mark
                                                </td>
                                                <td>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox TextMode="MultiLine" Width="100px" Height="50" ID="txtDuplicateMark"
                                                            CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                    </div>
                                                    <div class="errorIcon">
                                                        *</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    <asp:Button CssClass="ButtonAsh" Width="80px" runat="server" Text="Add" ID="btnAddDenomination"
                                                        OnClick="btnAddDenomination_Click" OnClientClick="return SaveDenomValidation()" />
                                                    <asp:Button CssClass="ButtonAsh" Width="80px" runat="server" Text="Delete All" ID="btnDeleteAllDenomination"
                                                        OnClick="btnDeleteAllDenomination_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    Total Amount
                                                </td>
                                                <td>
                                                    <asp:TextBox Width="100px" ID="txtTotalAmount" CssClass="textInput" runat="server"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                        MaxLength="9" ReadOnly="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
                <br />
            </td>
        </tr>
    </table>
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
                        Text="View Journals" ID="btnViewJournals" Visible="false" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveDuplicateValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteDuplicate()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:StopPay ID="StopPay" runat="server" Type="16" Title="Duplicate Issue Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
