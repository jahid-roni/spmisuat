<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchCustType.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchCustType" %>
<div id="MDCustTypeSearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Customer Type Search</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_ucSearchCustType_MDCustTypeSearch');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 450px; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Search Criteria</legend>
                                <table width="550" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="right" width="40%">
                                            Customer Type
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCustomerType" MaxLength="9" runat="server" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                Width="120px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Description
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDescription" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" Width="120px" MaxLength="9"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">
                                            Number of Maximum Members
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox Width="120px" ID="txtNumberOfMaximumMember" CssClass="textInput" runat="server"
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
                                        <td align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_ucSearchCustType_lblProgress') " OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Clear" ID="btnClear" OnClick="btnClear_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Customer Type List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                        AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_ucSearchCustType_MDCustTypeSearch')" Text="Select" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
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
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnClear" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function CustTypePopupHide() {
        hideModal('ctl00_cphDet_ucSearchCustType_MDCustTypeSearch');
        return false;
    }
    function CustTypePopupHideReturnTrue() {
        hideModal('ctl00_cphDet_ucSearchCustType_MDCustTypeSearch');
        return true;
    }
    function CustTypePopupReturnTrue() {
        showModal("ctl00_cphDet_ucSearchCustType_MDCustTypeSearch");
        return true;
    }
</script>

