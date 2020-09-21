<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchLienMark.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchLienMark" %>
<div id="MDLienMarkSearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                    </div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_LienMark_MDLienMarkSearch');" />
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
                                            Registration No
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox ID="txtRegNo" Width="140px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                MaxLength="25"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            SP Type
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSPType" SkinID="ddlMedium" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Date
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtIssueDate" Width="140px" CssClass="textInput"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Our Ref. No.
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox ID="txtOurRefNo" Width="140px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                MaxLength="20"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Their Ref. No.
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTheirRefNo" Width="140px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                MaxLength="20"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Approval Status
                                        </td>
                                        <td colspan="3">
                                            <asp:RadioButtonList runat="server" ID="rdolApproval" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Ready For Approve&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" Value="1" Selected="True"></asp:ListItem>
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
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick=" ShowProgressStatus('ctl00_cphDet_LienMark_lblProgress') "
                                                OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Stop Payment List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                        OnRowDataBound="gvSearchList_RowDataBound" AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_LienMark_MDLienMarkSearch')" Text="Select" />
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

    function LienMarkSearchPopupReturnTrue(sPageType) {
        $("#<%=gvSearchList.ClientID %>").remove();
        showModal("ctl00_cphDet_LienMark_MDLienMarkSearch");
        var rdolApproval = document.getElementById("ctl00_cphDet_LienMark_rdolApproval_0");
        var rdolApproval_1 = document.getElementById("ctl00_cphDet_LienMark_rdolApproval_1");
        if (sPageType == "1.1") {
            rdolApproval.disabled = false;
            rdolApproval.checked = true;
            rdolApproval_1.checked = false;
        }
        
        if (sPageType == "1.2") {
            rdolApproval.disabled = true;
            rdolApproval_1.checked = true;
        }
        CloseErrorPanel();
        return false;
    }
</script>

