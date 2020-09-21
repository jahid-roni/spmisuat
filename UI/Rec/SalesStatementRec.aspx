<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SalesStatementRec.aspx.cs" Inherits="SBM_WebUI.mp.SalesStatementRec" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        <legend runat="server" id="lgText">Unapproved Sales Statement Reconciliation List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
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
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            SP Type
                        </td>
                        <td width="30%">
                            <asp:DropDownList ID="ddlSpType" Width="160px" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right" width="20%">
                            Year
                        </td>
                        <td width="30%">
                            <asp:DropDownList ID="ddlYear" Width="160px" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Debit Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtDebitDate" Width="160px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" 
                                onfocus="highlightActiveInputWithObj(this)" Enabled="False"></asp:TextBox>
                        </td>
                        <td align="right">
                            BB Reference No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBBReferenceNo" Width="160px" CssClass="textInput" runat="server" MaxLength="20"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                <%--<asp:Button ID="btnSearch" CssClass="ButtonAsh" runat="server" Text="Search" /></div>--%>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend runat="server" id="Legend1">Reconciliation Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <asp:HiddenField ID="hdnSaleStatTrnsNo" runat="server" />
                <asp:HiddenField ID="hdnReconSaleStatTransNo" Value="" runat="server" />
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Reference No
                        </td>
                        <td width="30%">
                            <asp:TextBox ID="txtReferenceNo" Width="240px" CssClass="textInput" runat="server"
                                MaxLength="40" onclick="CloseErrorPanel()" OnTextChanged="txtReferenceNo_TextChanged"
                                AutoPostBack="true"></asp:TextBox>
                        </td>
                        <td align="right" width="20%">
                            Claim Date BB
                        </td>
                        <td width="30%">
                            <asp:TextBox ID="txtClaimBBDate" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Claim Date From
                        </td>
                        <td>
                            <asp:TextBox ID="txtClaimFromDate" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Claim Date To
                        </td>
                        <td>
                            <asp:TextBox ID="txtClaimToDate" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Claim Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtClaimAmount" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Debit Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtDebitAmount" Width="160px" CssClass="textInput" runat="server"
                                MaxLength="18" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Comment
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtClaimComment" Height="35px" TextMode="MultiLine" CssClass="textInput"
                                Width="603px" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td colspan="2">
                            Sales Claim Details
                        </td>
                        <td valign="top" align="right" colspan="4">
                            <asp:Button runat="server" ID="btnAdd" Text="Add" Width="80px" CssClass="ButtonAsh"
                                OnClientClick="return AddSalesCalimDetail()" OnClick="btnAdd_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div style="height: 200px; width: 900px; overflow: auto;">
                                <asp:GridView Style="width: 99%" ID="gvClaim" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvClaim_RowCommand" OnRowDeleting="gvClaim_RowDeleting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnSelect" Text="Select" CssClass="ButtonAsh" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Remove" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemove" Text="Remove" CssClass="ButtonAsh" runat="server" OnClientClick="return CheckForDelete('this sales claim detail?')" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="RefNo" HeaderText="Reference No" />
                                        <asp:BoundField DataField="ClaimDateBB" HeaderText="Claim Date BB" />
                                        <asp:BoundField DataField="ClaimDateFrom" HeaderText="Claim Date From" />
                                        <asp:BoundField DataField="ClaimDateTo" HeaderText="Claim Date To" />
                                        <asp:BoundField DataField="ClaimAmount" HeaderText="Claim Amount" />
                                        <asp:BoundField DataField="DebitAmount" HeaderText="Debit Amount" />
                                        <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                        <asp:BoundField DataField="SaleStatementTransNo" HeaderText="Sales Trans. No." Visible="false" />
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
                            No. of Claims
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtNoOfClaim" Width="80px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Claim Amount
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtTtlCalimAmnt" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Debit Amount
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtTtlDrAmnt" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
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
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td colspan="4">
                            Sales Statement Issue Details
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <div style="height: 200px; width: 900px; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvReg" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true">
                                    <Columns>
                                        <asp:BoundField DataField="Reg. No." HeaderText="Reg. No." HeaderStyle-HorizontalAlign="Center"
                                            ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="Issue Date" HeaderText="Issue Date" HeaderStyle-HorizontalAlign="Center"
                                            ItemStyle-HorizontalAlign="Center" DataFormatString="{0: dd-MMM-yyyy}" />
                                        <asp:BoundField DataField="No. of Certificate" HeaderText="No. of Certificate" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="Sales Amount" HeaderText="Sales Amount" HeaderStyle-HorizontalAlign="Right"
                                            ItemStyle-HorizontalAlign="Right" />
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
                            No. of Registration
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtNoOfReg" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Face Value
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtFaceValue" Width="160px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
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
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button ID="btnReset" Text="Reset" CssClass="ButtonAsh" runat="server" OnClick="btnReset_Click" />
                    <asp:Button ID="btnSave" Text="Save" CssClass="ButtonAsh" runat="server" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=txtDebitDate]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDebitAmount]').keypress(function(e) { return floatNumber(e); });
        }

        function AddSalesCalimDetail() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtReferenceNo.ClientID %>', 'TextBox', "Reference No. cannot be empty!");
            sErrorList += RequiredData('<%=txtDebitAmount.ClientID %>', 'TextBox', "Debit Amount cannot be empty!");

            var vDebitAmnt = $('#<%=txtDebitAmount.ClientID %>').val();
            var vClaimAmnt = $('#<%=txtClaimAmount.ClientID %>').val();
            if (vDebitAmnt != "" && vClaimAmnt != "") {
                if (parseFloat(vDebitAmnt) > parseFloat(vClaimAmnt)) {
                    sErrorList += "<li>Debit Amount cannot be greater than Claim Amount!</li>";
                }
            }

            return OpenErrorPanelWithoutPopUp(sErrorList);                       
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtDebitDate.ClientID %>', 'TextBox', "Debit Date cannot be empty!");
            sErrorList += RequiredData('<%=txtBBReferenceNo.ClientID %>', 'TextBox', "BB Reference No cannot be empty!");

            var rowsGvClaim = $("#<%=gvClaim.ClientID %> tr").length;
            if (rowsGvClaim == 1 || rowsGvClaim == 0) {
                sErrorList += "<li>Sales Claim Details grid cannot be empty!</li>";
            }

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdReconSaleStatTransNo = document.getElementById('<%=hdnReconSaleStatTransNo.ClientID %>');
            if (hdReconSaleStatTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Sales Claim Reconciliation Data?')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sales Claim Reconciliation has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
    </script>

</asp:Content>
