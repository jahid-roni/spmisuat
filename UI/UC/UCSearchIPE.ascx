<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchIPE.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchIPE" %>
<div id="MDSearchIPE" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label runat="server" ID="lblTitle"></asp:Label></div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_SearchIPE_MDSearchIPE');" />
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
                                            <asp:Label runat="server" ID="lblTransType"></asp:Label>
                                        </td>
                                        <td >
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtTransNo" MaxLength="15" CssClass="textInput"
                                                Width="120px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Registration No
                                        </td>
                                        <td><asp:TextBox runat="server" ID="txtRegNo" CssClass="textInput" Width="120px" MaxLength="25"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
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
                                            Payment From Date
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;<asp:TextBox runat="server" ID="txtFromDate" CssClass="textInput" Width="120px"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Payment To Date
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtToDate" CssClass="textInput" Width="120px" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_SearchIPE_lblProgress')" OnClick="btnSearch_Click" />
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
                                                        OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                        OnRowDataBound="gvSearchList_RowDataBound" AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                        OnClientClick="hideModal('ctl00_cphDet_SearchIPE_MDSearchIPE')" Text="Select" />
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

    function SearchIPEPopupHide() {
        hideModal('ctl00_cphDet_SearchIPE_MDSearchIPE');
        return false;
    }
    function SearchIPEPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_SearchIPE_MDSearchIPE');
        return true;
    }
    function SearchIPEPopupReturnTrue() {
        showModal("ctl00_cphDet_SearchIPE_MDSearchIPE");
        return true;
    }
    
</script>

