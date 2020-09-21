﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCReconBalance.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCReconBalance" %>
<div id="MDReconBalance" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Data Download Reconciliation Balance</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_JBalance_MDReconBalance');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 500px; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Search Criteria</legend>
                                <table width="650px" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="right">
                                            Enter Reference No
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRefNo" Width="140px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" MaxLength="20"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Check" ID="btnCheck" OnClientClick="ShowProgressStatus('ctl00_cphDet_SIssue_lblProgress') "
                                                OnClick="btnCheck_Click" />
                                        </td>
                                        <td>
                                            <asp:CheckBox runat="server" ID="chkArchive" Text="Archive" />
                                        </td>
                                        <td align="right">
                                            Balance Date
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtBalanceDate" Width="80px" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <table width="550" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="center">
                                            <asp:Label runat="server" ID="lblProgress"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Journals</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        SkinID="SBMLGridGreen" ShowHeader="true">
                                                        <Columns>
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            No record found
                                                        </EmptyDataTemplate>
                                                        <AlternatingRowStyle CssClass="odd" />
                                                    </asp:GridView>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <legend runat="server" id="Legend1">Reconciliation Specification</legend>
                                <table border="0" width="98%" align="center" cellpadding="2" border="1">
                                    <tr>
                                        <td align="right" valign="top">
                                            Total Debit Amount
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalDebitAmount" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total Credit Amount
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalCreditAmount" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total Balance
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalBalance" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">
                                            Total No. of Debit Transaction
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalNoofDebitTransaction" Width="100px" CssClass="textInput"
                                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total No. of Credit Transaction
                                        </td>
                                        <td colspan="3" valign="top">
                                            <asp:TextBox ID="txtTotalNoofCreditTransaction" Width="100px" CssClass="textInput"
                                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <legend>Operation</legend>
                                <table width="98%" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="left">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Undo" ID="btnUndo" OnClick="btnUndo_Click" />
                                        </td>
                                        <td align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Close" ID="btnClose" OnClientClick="javascript:hideModal('ctl00_cphDet_JBalance_MDReconBalance');" />
                                        </td>
                                    </tr>
                                 </table
                            </fieldset>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnCheck" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnUndo" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function JBalancePopupHide() {
        hideModal('ctl00_cphDet_JBalance_MDReconBalance');
        return false;
    }
    function JBalancePopupHideReturnTrue() {
        hideModal('ctl00_cphDet_JBalance_MDReconBalance');
        return true;
    }
    function JBalancePopupReturnTrue() {
        $("#<%=gvSearchList.ClientID %>").remove();
        showModal("ctl00_cphDet_JBalance_MDReconBalance");
        return false;
    }
</script>

