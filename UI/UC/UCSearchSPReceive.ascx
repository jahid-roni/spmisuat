<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchSPReceive.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchSPReceive" %>
<div id="MDSPReceiveSearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        SP Receive Search</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_ucSearchSPReceive_MDSPReceiveSearch');" />
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
                                <table width="550px" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="right">
                                            SP Type
                                        </td>
                                        <td colspan="3">
                                            &nbsp;&nbsp;<asp:DropDownList ID="ddlSPType" SkinID="ddlCommon" Width="340px" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            From Date
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtFromDate" CssClass="textInput" Width="120px"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            To Date
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtToDate" CssClass="textInput" Width="120px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            From Amount
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtFromAmount" MaxLength="15" CssClass="textInput"
                                                Width="120px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            To Amount
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtToAmount" MaxLength="15" CssClass="textInput"
                                                Width="120px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Approval Status
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Ready for Approve&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp"
                                                    Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Approved" Value="2"></asp:ListItem>
                                            </asp:RadioButtonList>
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
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_ucSearchSPReceive_lblProgress')" OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Receive List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        OnRowCommand="gvSearchList_RowCommand" OnRowDataBound="gvSearchList_RowDataBound"
                                                        AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging" SkinID="SBMLGridGreen"
                                                        ShowHeader="true">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_ucSearchSPReceive_MDSPReceiveSearch')"
                                                                        Text="Select" />
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
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function SearchSPReceivePopupHide() {
        hideModal('ctl00_cphDet_ucSearchSPReceive_MDSPReceiveSearch');
        return false;
    }
    function SearchSPReceivePopupHideReturnTrue() {
        hideModal('ctl00_cphDet_ucSearchSPReceive_MDSPReceiveSearch');
        return true;
    }
    function SearchSPReceivePopupReturnTrue() {
        showModal("ctl00_cphDet_ucSearchSPReceive_MDSPReceiveSearch");
        return true;
    }
    

</script>

