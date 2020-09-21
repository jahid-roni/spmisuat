<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="DataUpload.aspx.cs" Inherits="SBM_WebUI.mp.DataUpload" %>

<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function SaveValidation() {

            var sErrorList = "";

            var rowsGvData = $("#<%=gvTransactionList.ClientID %> tr").length;
            if (rowsGvData == 1 || rowsGvData == 0) {
                sErrorList += "<li>Transaction List cannot be empty</li>";
            }

            return OpenErrorPanel(sErrorList);
        }
21
        function ConfirmDialog() {
            if (confirm('Upload file related to the selected Journal Type is already being used by other user. do you want to load that information?') == true) {
                __doPostBack("<%= this.hdnLinkButton.UniqueID %>", "");
            }
            else {
                alert("Selected upload file contains data that are not yet uploaded, hence cannot be used for further upload. Please check...");
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Hidden Field --%>
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Mandatory Field List</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset>
        <asp:LinkButton ID="hdnLinkButton" runat="server" Style="display: none" OnClick="hdnLinkButton_Click">LinkButton</asp:LinkButton>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="15%" valign="top">
                            User
                        </td>
                        <td width="20%" valign="top">
                            <div class="fieldLeft">
                                <asp:DropDownList runat="server" ID="ddlUserName" SkinID="ddlSmall" 
                                    OnSelectedIndexChanged="ddlUserName_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" width="10%" valign="top">
                            Journal Type
                        </td>
                        <td width="28%" valign="top">
                            <div class="fieldLeft">
                                <asp:DropDownList runat="server" ID="ddlJournalType" SkinID="ddlLarge" OnSelectedIndexChanged="ddlJournalType_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" width="12%" valign="top">
                            Transaction No
                        </td>
                        <td width="38%" valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox Width="110px" ID="txtTransacNo" CssClass="textInputDisabledForTxt" ReadOnly="true"
                                    runat="server"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Current Activity
                        </td>
                        <td valign="top" colspan="3">
                            <asp:RadioButtonList RepeatDirection="Horizontal" RepeatColumns="2" runat="server"
                                ID="rblCurrencyActivity" Enabled="false">
                                <asp:ListItem Text="None" Value="None" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Select To Make File" Value="Select To Make File"></asp:ListItem>
                                <asp:ListItem Text="Make Upload File" Value="Make Upload File"></asp:ListItem>
                                <asp:ListItem Text="Mark Uploaded" Value="Mark Uploaded"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td colspan="2">
                            <asp:Label ID="lblActivity" Font-Bold="true" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
                border="0">
                <tr>
                    <td>
                        Transaction List
                    </td>
                    <td align="right">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Select All" ID="btnSelectAll"
                            OnClick="btnSelectAll_Click" />
                        &nbsp;
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Deselect All" ID="btnDeSelectAll"
                            OnClick="btnDeSelectAll_Click" />
                    </td>
                </tr>
                <tr>
                    <td width="100%" colspan="2">
                        <div style="height: 200px; width: 100%; overflow: auto;">
                            <asp:GridView ID="gvTransactionList" runat="server" AutoGenerateColumns="false" Width="98%"
                                OnRowCommand="gvTransactionList_RowCommand" SkinID="SBMLGridGreen">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select">
                                        <ItemStyle HorizontalAlign="Left" Width="25px" />
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="chkSelect" OnCheckedChanged="chkSelect_OnCheckedChanged"
                                                AutoPostBack="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="AccTransactionNo" HeaderText="Trans No" />
                                    <asp:BoundField DataField="TransactionDate" HeaderText="Trans Date" DataFormatString="{0: dd/MM/yyyy}" />
                                    <asp:BoundField DataField="Narration" HeaderText="Narration" />
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
        <%--<Triggers>
            <asp:AsyncPostBackTrigger EventName="SelectedIndexChanged" ControlID="ddlJournalType" />
        </Triggers>--%>
    </asp:UpdatePanel>
    <br />
    <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
        border="0">
        <tr>
            <td>
                Transaction Details
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                    <ContentTemplate>
                        <div style="height: 200px; width: 100%; overflow: auto;">
                            <asp:GridView ID="gvTransactionDetail" runat="server" AutoGenerateColumns="false"
                                Width="98%" SkinID="SBMLGridGreen">
                                <Columns>
                                    <asp:BoundField DataField="AccountNo" HeaderText="Account No." />
                                    <asp:BoundField DataField="DrCr" HeaderText="Dr/Cr" />
                                    <asp:BoundField DataField="CurrencyCode" HeaderText="Currency" />
                                    <asp:BoundField DataField="amount" HeaderText="Amount" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <fieldset>
        <legend>Satistics</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right" style="width: 22%">
                            No. of Selected Transactions
                        </td>
                        <td style="width: 15%">
                            <asp:TextBox ID="txtStTransSelected" Width="80px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            File Sequence No
                        </td>
                        <td style="width: 15%">
                            <asp:TextBox ID="txtStFileSeqNo" Width="130px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            Originator ID
                        </td>
                        <td style="width: 15%">
                            <asp:TextBox ID="txtStOriginatorID" Width="150px" CssClass="textInputDisabledForTxt"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            No. of Total Credit Transactions
                        </td>
                        <td>
                            <asp:TextBox ID="txtStTtlCrdtTrns" Width="80px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Debit Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtStTtlDbAmnt" Width="130px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Credit Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtStTtlCrAmount" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Maker
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtStMaker" MaxLength="20" Width="120px" CssClass="textInputDisabledForTxt"
                                    ReadOnly="true" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            Transaction Date
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtStTransDate" Width="130px" CssClass="textInputDisabledForTxt"
                                    ReadOnly="true" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            Transaction Time
                        </td>
                        <td>
                            <asp:TextBox ID="txtStTransTime" MaxLength="8" Width="150px" CssClass="textInputDisabledForTxt"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Upload File Path
                        </td>
                        <td colspan="5">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtStUpldFilePth" Width="99%" CssClass="textInputDisabledForTxt"
                                    ReadOnly="true" runat="server"></asp:TextBox>
                            </div>
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
                <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="center">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset the Remainig" ID="btnResetRemain"
                                OnClick="btnResetRemain_Click" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Washed" ID="btnCancel" OnClick="btnCancel_Click" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Select" ID="btnSelect" OnClick="btnSelect_Click"
                                OnClientClick="return SaveValidation()" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
