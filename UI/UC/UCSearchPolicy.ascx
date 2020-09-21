<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchPolicy.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchPolicy" %>
<div id="MDPolicySearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Sanchaya Patra Policy Search</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_ucSearchPolicy_MDPolicySearch');" />
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
                                        <td align="right" width="25%">
                                            SP Type ID
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlSearchSPType" SkinID="ddlLarge" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            From Date
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSearchFromDate" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" Width="120px" MaxLength="9"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            To Date
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox Width="120px" ID="txtSearchToDate" CssClass="textInput" runat="server"
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
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick=" ShowProgressStatus('ctl00_cphDet_ucSearchPolicy_lblProgress') "
                                                OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Clear" ID="btnClear" OnClick="btnClear_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Polic Type List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" OnRowDataBound="gvSearchList_RowDataBound" AutoGenerateColumns="true"
                                                        ID="gvSearchList" runat="server" OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen"
                                                        ShowHeader="true" AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_ucSearchPolicy_MDPolicySearch')" Text="Select" />
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
    function SearchPolicyPopupHide() {
        hideModal('ctl00_cphDet_ucSearchPolicy_MDPolicySearch');
        return false;
    }
    function SearchPolicyPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_ucSearchPolicy_MDPolicySearch');
        return true;
    }
    function SearchPolicyPopupReturnTrue() {
        showModal("ctl00_cphDet_ucSearchPolicy_MDPolicySearch");
        return true;
    }
    
</script>

