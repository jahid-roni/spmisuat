<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="IssueUpdate.aspx.cs" Inherits="SBM_WebUI.mp.IssueUpdate" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function resetNomineeDetatil() {

            $("#<%=txtNDName.ClientID %>").val('');
            $("#<%=txtNDRelation.ClientID %>").val('');
            $("#<%=txtNDAddress.ClientID %>").val('');
            $("#<%=txtNDShare.ClientID %>").val('');
            $("#<%=txtNDAmount.ClientID %>").val('');
            $("#<%=hdNomSlno.ClientID %>").val('');
        }

        function CalculateAmount(vType) {
            var oAmount = document.getElementById('<%=txtTotalAmount.ClientID %>');
            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');

            var vTotalAmount = "";
            if (oAmount.value != "") {
                vTotalAmount = oAmount.value.replace(/,/g, '');
                CloseErrorPanel();
            }
            else {
                OpenErrorPanel("<li>Total Amount cannot be empty!</li>", "");
                oNDShare.value = "";
                oNDAmount.value = "";
                return false;
            }
            vTotalAmount = parseInt(vTotalAmount, 10);


            // this is by total amount
            if (vType == "Amount") {
                if (oNDAmount.value != "") {
                    if (parseInt(oNDAmount.value, 10) > parseInt(oAmount.value, 10)) {
                        OpenErrorPanel("<li>Total Nominee amount cannot be exceeded from Total Amount</li>", "");
                        oNDShare.value = "";
                        return false;
                    }
                    var vNDAmount = parseInt(oNDAmount.value, 10);
                    var vFinalVal = (parseFloat(vNDAmount) * 100) / parseFloat(vTotalAmount);
                    vFinalVal = vFinalVal.toFixed(0);
                    oNDShare.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDShare.value = "";
                    return false;
                }
            }

            // this is by share amount
            else if (vType == "Share") {
                if (oNDShare.value != "") {
                    if (parseInt(oNDShare.value, 10) > 100) {
                        OpenErrorPanel("<li>Total amount of share cannot be exceeded 100!</li>", "");
                        oNDAmount.value = "";
                        return false;
                    }
                    var vShare = parseInt(oNDShare.value, 10);
                    var vFinalVal = (parseFloat(vShare) * parseFloat(vTotalAmount)) / 100;
                    vFinalVal = vFinalVal.toFixed(2);

                    oNDAmount.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDAmount.value = "";
                    return false;
                }
            }
        }

        function SaveNomineeValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtNDName.ClientID %>', 'TextBox', "Nominee Name cannot be empty!");
            sErrorList += RequiredData('<%=txtNDShare.ClientID %>', 'TextBox', "Share cannot be empty!");
            sErrorList += RequiredData('<%=txtNDAmount.ClientID %>', 'TextBox', "Amount cannot be empty!");


            // show error divErrorList
            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');
            if (oNDShare.value != "") {
                if (parseInt(oNDShare.value, 10) > 100) {
                    sErrorList += "<li>Total amount of share cannot be exceeded 100!</li>";
                    oNDAmount.value = "";
                }
            }
            return OpenErrorPanel(sErrorList, '');
            // end of show error divErroList
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            // show error divErrorList
            return OpenErrorPanel(sErrorList, 'Reject');
            // end of show error divErroList
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdUpdateIssueTransNo = document.getElementById('<%=hdUpdateIssueTransNo.ClientID %>');
            if (hdUpdateIssueTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Issue Update Data')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Issue Update transaction has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtRegistrationNo.ClientID %>', 'TextBox', "Registration No cannot be empty!");

            // show error divErrorList
            return OpenErrorPanel(sErrorList, 'Save');
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
    <fieldset runat="server" id="Fieldset1">
        <legend runat="server" id="lgText">Unapproved Issue List</legend>
        <asp:UpdatePanel runat="server" ID="fsList">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdUpdateIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="910px"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" OnClientClick="CloseErrorPanel()"
                                                runat="server" ID="btnSelect" Text="Select" />
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
    <%-- Error --%>
    <fieldset>
        <legend>Issue Search</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td valign="top" align="right">
                            Transaction No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="12" ID="txtTransNo" Width="100px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    Enabled="False"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnTransSearch"
                                    OnClientClick="return StopPaymentSearchPopupReturnTrue('5.2')" /></div>
                        </td>
                        <td valign="top" align="right">
                            Change Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtChangeDate" Width="100px" CssClass="textInput" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                    border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" AutoPostBack="True"
                                Enabled="False">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Year
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlYear" Width="100px" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" runat="server" Enabled="False">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Registration No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="25" ID="txtRegistrationNo" Width="160px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnRegSearch" OnClick="btnRegSearch_Click"
                                    OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" /></div>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Enabled="False"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalAmount" Width="100px" CssClass="textInput" runat="server"
                                MaxLength="19" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer Type
                        </td>
                        <td colspan="5">
                            <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server" Enabled="False">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
        <br />
    <!-- start to 2 Tab panel  -->
    <fieldset>
        <legend>Customer(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="height: 100px; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustomerDetail" runat="server"
                            AutoGenerateColumns="False" SkinID="SBMLGridGreen" OnRowDataBound="gvCustomerDetail_RowDataBound"
                            Enabled="False">
                            <Columns>
                                <asp:BoundField DataField="bfCustomerID" HeaderText="Customer ID" />
                                <asp:BoundField DataField="bfCustomerName" HeaderText="Name" />
                                <asp:BoundField DataField="bfDateOfBirth" HeaderText="Date Of Birth 1" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="bfAddress" HeaderText="Address" />
                                <asp:BoundField DataField="bfPhone" HeaderText="Phone" />
                                <asp:BoundField DataField="bfDateOfBirth2" HeaderText="Date Of Birth 2" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="bfNationality" HeaderText="Nationality" />
                                <asp:BoundField DataField="bfPassportNo" HeaderText="Passport No" />
                                <asp:BoundField DataField="bfForeignAddress" HeaderText="Foreign Address" />
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
        <!--  End of Customer(s) Details -->
        <!--  Start to Nominee(s) Details -->
    <br />
    <fieldset>
        <legend>Nominee(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                <ContentTemplate>
                    <h2>
                        Nominee(s) Details</h2>
                    <div style="height: 40%; width: 100%; overflow: auto;">
                        <table>
                            <tr>
                                <td>
                                    Name
                                </td>
                                <td>
                                    Relation
                                </td>
                                <td>
                                    Address
                                </td>
                                <td>
                                    Share
                                </td>
                                <td>
                                    Amount
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtNDName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="250px" MaxLength="100"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDRelation" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px" MaxLength="20"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDAddress" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="300px" MaxLength="150"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDShare" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this),CalculateAmount('Share')"
                                        onfocus="highlightActiveInputWithObj(this)" Width="60px" MaxLength="3"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDAmount" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Amount')"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px" MaxLength="9"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="5">
                                    <asp:Button ID="btnNDAdd" runat="server" CssClass="ButtonAsh" Text="Add" Width="80px"
                                        OnClick="btnNDAdd_Click" OnClientClick="return SaveNomineeValidation()" />
                                    &nbsp
                                    <asp:Button ID="btnNDReset" runat="server" CssClass="ButtonAsh" Text="Reset" Width="80px"
                                        OnClientClick="resetNomineeDetatil()" />
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdNomSlno" Value="" runat="server" />
                    </div>
                    <div style="height: 60%; width: 100%; overflow: auto;" id="divOldNominee" runat="server">
                        <fieldset>
                            <legend>Old Nominee List</legend>
                            <asp:GridView Style="width: 98%; height: 100%" ID="gvOldNomineeList" runat="server"
                                AutoGenerateColumns="False" SkinID="SBMLGridGreen">
                                <Columns>
                                    <asp:BoundField DataField="NomineeName" HeaderText="Nominee Name" />
                                    <asp:BoundField DataField="Relation" HeaderText="Relation" />
                                    <asp:BoundField DataField="Address" HeaderText="Address" />
                                    <asp:BoundField DataField="NomineeShare" HeaderText="Share" />
                                    <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </fieldset>
                    </div>
                    <br>
                        <br></br>
                        <div style="height: 60%; width: 100%; overflow: auto;">
                            <fieldset>
                                <legend>Nominee List</legend>
                                <asp:GridView ID="gvNomDetail" runat="server" AutoGenerateColumns="False" OnRowCommand="gvNomDetail_RowCommand"
                                    OnRowDataBound="gvNomDetail_RowDataBound" OnRowDeleting="gvNomDetail_RowDeleting"
                                    SkinID="SBMLGridGreen" Style="width: 98%; height: 100%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemTemplate>
                                                <asp:Button ID="btnNDSelect" runat="server" CommandName="Select" CssClass="ButtonAsh"
                                                    Text="Select" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemTemplate>
                                                <asp:Button ID="btnNDDelete" runat="server" CommandName="Delete" CssClass="ButtonAsh"
                                                    OnClientClick="return CheckForDelete('this Nominee')" Text="Delete" />
                                                <asp:HiddenField ID="hdNomineeSlno" runat="server" Value='<%# Eval("SlNo") %>' />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="NomineeName" HeaderText="Nominee Name" />
                                        <asp:BoundField DataField="Relation" HeaderText="Relation" />
                                        <asp:BoundField DataField="Address" HeaderText="Address" />
                                        <asp:BoundField DataField="NomineeShare" HeaderText="Share" />
                                        <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </fieldset>
                        </div>
                    </br>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnNDAdd" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </fieldset>
        <!--  End of Nominee(s) Details -->
    <br />
    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
        <ContentTemplate>
            <table width="100%" align="center" class="tableBody" border="0">
                <tr>
                    <td valign="top" align="right">
                        Remarks
                    </td>
                    <td>
                        <asp:TextBox ID="txtRemarks" Width="700px" TextMode="MultiLine" Height="50px" CssClass="textInput"
                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <legend>Change Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0" cellpadding="2" cellspacing="2">
                    <tr>
                        <td align="right">
                            Old Issue Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtOldIssueDate" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Enabled="False"></asp:TextBox>
                        </td>
                        <td align="right">
                            New Issue Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtNewIssueDate" Width="100px" CssClass="textInput" runat="server"
                                AccessKey="A" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Old Issue Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtOldIssueName" TextMode="MultiLine" Height="40px" Width="200px"
                                MaxLength="100" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            New Issue Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtNewIssueName" TextMode="MultiLine" Height="40px" Width="200px"
                                MaxLength="100" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_OnClick" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick="MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_OnClick" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click"
                        OnClientClick="return Delete()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:StopPay ID="StopPay" runat="server" type="13" title="Issue Update Search" />
    <uc4:SIssue ID="SIssue" runat="server" type="4" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
