<%@ Page Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="EncashClaimRec.aspx.cs"
    Inherits="SBM_WebUI.mp.EncashClaimRec" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%--<td>
                            <asp:TextBox ID="txtIncomeTax" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>--%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%--<td>
                            <asp:TextBox ID="txtIncomeTax" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>--%>
    <fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved Encashment Reconciliation List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
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
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" Width="150px" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Year
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlYear" Width="70px" runat="server">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            BB Reference No
                        </td>
                        <td>
                            <asp:TextBox ID="txtBBReferenceNo" Width="160px" CssClass="textInput" runat="server"
                                MaxLength="30" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Recon Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtReconciliationDate" Width="90px" CssClass="textInput" runat="server"
                                Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Recon Currency ID
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlReconCurrency" Width="150px" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlReconCurrency_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <%--Conv. Rate--%>
                        </td>
                        <td>
                            <asp:TextBox ID="txtReimConvRate" Width="80px" CssClass="textInput" runat="server" Visible="false"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            <%--Conv. Rate to BC--%>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCovRateToBC" Width="80px" CssClass="textInput" runat="server" Visible="false"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                            <td align="right">
                            BB Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtBBAmount" Width="90px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" 
                                onfocus="highlightActiveInputWithObj(this)" AutoPostBack="True" 
                                ontextchanged="txtBBAmount_TextChanged"></asp:TextBox>
                        </td>                        
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField ID="hdnEncashmentReimburseTransNo" runat="server" />
                <asp:HiddenField ID="hdnEncashmentClaimTransNo" runat="server" />
                <asp:HiddenField ID="hdnEncashmentTransNo" runat="server" />
                <asp:HiddenField ID="hdnClaimRefNo" runat="server" />
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="15%">
                            Reference No
                        </td>
                        <td width="16%">
                            <asp:TextBox Width="240px" ID="txtClaimRefNo" CssClass="textInput" runat="server"
                                MaxLength="40" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                OnTextChanged="txtClaimRefNo_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </td>
                        <td align="right" width="18%">
                            Claim Date
                        </td>
                        <td width="15%">
                            <asp:TextBox Width="100px" ID="txtClaimDate" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right" width="16%">
                            Date From
                        </td>
                        <td width="15%">
                            <asp:TextBox Width="100px" ID="txtClaimDateFrom" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Date To
                        </td>
                        <td>
                            <asp:TextBox ID="txtClaimDateTo" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Claim Currency
                        </td>
                        <td>
                            <asp:TextBox ID="txtClaimCurrncy" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Conv. Rate
                        </td>
                        <td>
                            <asp:TextBox ID="txtConvRate" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Claim Encashment
                        </td>
                        <td>
                            <asp:TextBox ID="txtEncashment" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Remuneration
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtRemuneration" CssClass="textInputDisabled" Width="100px" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                        </td>
                        <%--<td>
                            <asp:TextBox ID="txtIncomeTax" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>--%>
                        <td align="right" valign="top">
                            <asp:Button runat="server" ID="btnRestClaim" Width="50px" Text="Reset" CssClass="ButtonAsh"
                                OnClick="btnRestClaim_Click" />
                            &nbsp;<asp:Button runat="server" ID="btnAdd" Width="50px" Text="Add" CssClass="ButtonAsh"
                                OnClientClick="return AddEncashmentCalimDetail()" OnClick="btnAdd_Click" />
                        </td>
                        <%--<td>
                            <asp:TextBox ID="txtReconIncomeTax" CssClass="textInput" Width="100px" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>--%>
                    </tr>
                    <%--                    <tr>
                        <td align="right" valign="top">
                            Recon Encashment
                        </td>
                        <td>
                            <asp:TextBox ID="txtReconEncashment" CssClass="textInput" Width="100px" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Recon Remuneration
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtReconRemuneration" CssClass="textInput" Width="100px" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Comment
                        </td>
                        <td valign="top" colspan="4">
                            <asp:TextBox ID="txtComment" Width="415px" TextMode="MultiLine" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
--%>
                    <tr>
                        <td colspan="5">
                            Encashment Claim Details<br />
                            <div style="height: 200px; width: 700px; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvClaim" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvClaim_RowCommand" OnRowDeleting="gvClaim_RowDeleting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnSelect" Text="Select" CssClass="ButtonAsh" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Remove" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemove" Text="Remove" CssClass="ButtonAsh" runat="server" OnClientClick="return CheckForDelete('this Encashment Claim detail?')" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ClaimRefNo" HeaderText="Reference No" />
                                        <asp:BoundField DataField="ClaimDate" HeaderText="Claim Date" />
                                        <asp:BoundField DataField="ClaimDateFrom" HeaderText="From Date" Visible="false"/>
                                        <asp:BoundField DataField="ClaimDateTo" HeaderText="To Date" Visible="false"/>
                                        <asp:BoundField DataField="ClaimAmount" HeaderText="ClaimAmount" />
                                        <asp:BoundField DataField="ReconAmount" HeaderText="ReconAmount" />
                                        <%--<asp:BoundField DataField="ReconEncashment" HeaderText="Recon Encashment" />--%>
                                        <%--<asp:BoundField DataField="IncomeTax" HeaderText="Income Tax" />--%>
                                        <asp:BoundField DataField="Remuneration" HeaderText="Remuneration" />
                                        <%--<asp:BoundField DataField="ReconIncomeTax" HeaderText="Recon Income Tax" />--%>
                                        <%--<asp:BoundField DataField="ReconRemuneration" HeaderText="Recon Remuneration" />--%>
                                        <%--<asp:BoundField DataField="Comment" HeaderText="Comment" />--%>
                                        <asp:BoundField DataField="EncashmentClaimTransNo" HeaderText="ClaimTransNo" Visible="false" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                        <td width="10%" valign="top">
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Encashment
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtTotEncashment" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Claim Remuneration
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotRemuneration" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Recon Encashment
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotRecInt" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Recon Remuneration
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotRecRem" Width="100px" CssClass="textInput" ReadOnly="false"
                                runat="server" ontextchanged="txtTotRecRem_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Variation
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotVariation" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Recon Variation
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotReconVariation" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            BB Variation
                        </td>
                        <td>
                            <asp:TextBox ID="txtBBVariation" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>                        
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="12.5%">
                            Reg No
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtRegNo" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="12.5%">
                            Payment Date
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtPaymentDate" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="12.5%">
                            Claim Amount
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtClaimAmount" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td width="12.5%">
                            Recon Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtReconAmount" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" align="right">
                            <asp:TextBox Width="100px" ID="txtIntClaimTransNo" CssClass="textInputDisabled" Visible="false" runat="server"></asp:TextBox>
                            <asp:TextBox Width="100px" ID="txtEncTransNo" CssClass="textInputDisabled" Visible="false" runat="server"></asp:TextBox>
                            <asp:TextBox Width="100px" ID="txtCRefNo" CssClass="textInputDisabled" Visible="false" runat="server"></asp:TextBox>
                            <asp:Button runat="server" ID="btnUpdate" Text="Update" CssClass="ButtonAsh" Width="50px"
                                OnClick="btnUpdate_Click" />
                            &nbsp;<asp:Button runat="server" ID="btnResetDetails" Text="Reset" 
                                CssClass="ButtonAsh" Width="50px"
                                OnClientClick="return AddEncashmentDetail()" 
                                OnClick="btnResetDetails_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="7">
                            Encashment Payment Details<br />
                            <div style="height: 200px; width: 700px; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvIntClaimDetail" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" OnRowCommand="gvIntClaimDetail_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnSelect_D" Text="Select" CssClass="ButtonAsh" runat="server" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Remove" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemove_D" Text="Remove" CssClass="ButtonAsh" runat="server" OnClientClick="return CheckForDelete('this Encashment Claim detail?')" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="5%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ClaimRefNo" HeaderText="Claim Ref No" />
                                        <asp:BoundField DataField="RegNo" HeaderText="Reg. No." />
                                        <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" />
                                        <asp:BoundField DataField="ClaimAmount" HeaderText="Claim Amount" />
                                        <asp:BoundField DataField="ReconAmount" HeaderText="Recon Amount" />
                                        <asp:BoundField DataField="EncashmentTransNo" HeaderText="EncashmentTransNo" Visible="false" />
                                        <asp:BoundField DataField="EncashmentClaimTransNo" HeaderText="EncashmentClaimTransNo"
                                            Visible="false" />
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
                            No. of Encashment Payment
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtNoOfEncashmentPay" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Claim Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalClaimAmount" Width="100px" CssClass="textInputDisabled"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Recon Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalReconAmount" Width="100px" CssClass="textInputDisabled"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" align="right">
                            Total Variation
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalVariation" Width="100px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" Visible="true"  OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button ID="btnReset" Text="Reset" CssClass="ButtonAsh" runat="server" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button ID="btnSave" Text="Save" CssClass="ButtonAsh" runat="server" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                        
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />

    <script type="text/javascript" language="javascript">

        function pageLoad(sender, args) {
            $('input[id*=txtReconciliationDate]').each(function() { $(this).mask("99/99/9999"); });
//            $('input[id*=txtReimConvRate]').keypress(function(e) { return floatNumber(e); });
//            $('input[id*=txtCovRateToBC]').keypress(function(e) { return floatNumber(e); });
            //$('input[id*=txtReconEncashment]').keypress(function(e) { return floatNumber(e); });
            //$('input[id*=txtReconIncomeTax]').keypress(function(e) { return floatNumber(e); });
            //$('input[id*=txtReconRemuneration]').keypress(function(e) { return floatNumber(e); });
        }

        function AddEncashmentCalimDetail() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtClaimRefNo.ClientID %>', 'TextBox', "Claim Reference No. cannot be empty!");

            return OpenErrorPanelWithoutPopUp(sErrorList);
        }
        function AddEncashmentDetail() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtRegNo.ClientID %>', 'TextBox', "Reg No. cannot be empty!");
            sErrorList += RequiredData('<%=txtReconAmount.ClientID %>', 'TextBox', "Recon Amount. cannot be empty!");

            return OpenErrorPanelWithoutPopUp(sErrorList);
        }        

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "Please select SP Type");
            sErrorList += RequiredData('<%=txtBBReferenceNo.ClientID %>', 'TextBox', "BB Reference No cannot be empty!");
            sErrorList += RequiredData('<%=ddlReconCurrency.ClientID %>', 'DropDownList', "Please select Reconciliation Currency ID");
//            sErrorList += RequiredData('<%=txtReimConvRate.ClientID %>', 'TextBox', "Conversion Rate cannot be empty!");
//            sErrorList += RequiredData('<%=txtCovRateToBC.ClientID %>', 'TextBox', "Coversin Rate To BC cannot be empty!");
            sErrorList += RequiredData('<%=txtBBAmount.ClientID %>', 'TextBox', "Coversin Rate To BC cannot be empty!");

            var rowsGvClaim = $("#<%=gvClaim.ClientID %> tr").length;
            if (rowsGvClaim == 1 || rowsGvClaim == 0) {
                sErrorList += "<li>Encashment Claim Details grid cannot be empty</li>";
            }

            return OpenErrorPanel(sErrorList, 'Save');
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

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIntReimTransNo = document.getElementById('<%=hdnEncashmentReimburseTransNo.ClientID %>');
            if (hdIntReimTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Encashment Claim Reconciliation Data?')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Encashment Claim Reconciliation has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
    </script>

</asp:Content>
