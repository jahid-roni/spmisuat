<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchReinvest.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchReinvest" %>
<div id="MDReinvest" class="MDClass" runat="server">
    <table width="650" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Reinvestment Issue Search</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_RIS_MDReinvest');" />
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
                                <table width="600px" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="right">
                                            Reference No
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox ID="txtReferenceNo" Width="140px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                MaxLength="20"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Reinvestment Date
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtReinvestmentDate" Width="140px" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            New Registration No
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox MaxLength="15" runat="server" ID="txtNewRegistrationNo" Width="140px" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Old Registration No
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox MaxLength="15" runat="server" ID="txtOldRegistrationNo" Width="140px" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            SP Type
                                        </td>
                                        <td colspan="3">
                                            &nbsp;&nbsp;<asp:DropDownList ID="ddlSPType" SkinID="ddlMedium" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Approval Status
                                        </td>
                                        <td colspan="3">
                                            <asp:RadioButtonList runat="server" ID="rdolApproval" RepeatDirection="Horizontal" onClick="CheckIssueStatus(this)" >
                                                <asp:ListItem Text="Ready For Approve&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Approved" Value="2"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Issue Status
                                        </td>
                                        <td colspan="3">
                                            <asp:CheckBoxList runat="server" ID="chklIssueStatus" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Issued" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Partially Encahsed" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Full Encahsed" Value="3"></asp:ListItem>
                                            </asp:CheckBoxList>
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
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_RIS_lblProgress')" OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Reinvestment Issue Search List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                        OnRowDataBound="gvSearchList_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_RIS_MDReinvest')" Text="Select" />
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

    function ReinvestmentPopupHide() {
        hideModal('ctl00_cphDet_RIS_MDReinvest');
        return false;
    }
    function ReinvestmentPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_RIS_MDReinvest');
        return true;
    }
    function ReinvestmentPopupReturnTrue() {
        $("#<%=gvSearchList.ClientID %>").remove();
        showModal("ctl00_cphDet_RIS_MDReinvest");
        return false;
    }
</script>

