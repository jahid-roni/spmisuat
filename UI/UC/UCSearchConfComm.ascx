<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchConfComm.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchConfComm" %>
<div id="MDSearchConfComm" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label runat="server" ID="lblPageCaption"></asp:Label></div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_ucSearchConfComm_MDSearchConfComm');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 450px; overflow: auto;">
                    <fieldset>
                        <legend>Search Criteria</legend>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                            <ContentTemplate>
                                <table width="550" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="right">
                                            <asp:Label runat="server" ID="lblCap1"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" Width="100px"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            <asp:Label runat="server" ID="lblCap2"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" Width="220px" MaxLength="150"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                    <br />
                    <fieldset>
                        <table width="550" align="center" class="tableBody" border="0">
                            <tr>
                                <td align="center">
                                    <asp:Label runat="server" ID="lblProgress"></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick=" ShowProgressStatus('ctl00_cphDet_ucSearchConfComm_lblProgress') "
                                        OnClick="btnSearch_Click" />
                                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Clear" ID="btnClear" OnClick="btnClear_Click" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <br />
                    <fieldset runat="server" id="fsList">
                        <legend>List of data</legend>
                        <table width="100%" align="center" class="tableBody" border="0">
                            <tr>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="upGv">
                                        <ContentTemplate>
                                            <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                AllowPaging="True" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                OnClientClick="hideModal('ctl00_cphDet_ucSearchConfComm_MDSearchConfComm')" Text="Select" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnClear" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function ConfCommPopupHide() {
        hideModal('ctl00_cphDet_ucSearchConfComm_MDSearchConfComm');
        return false;
    }
    function ConfCommPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_ucSearchConfComm_MDSearchConfComm');
        return true;
    }
    function ConfCommPopupReturnTrue() {
        showModal("ctl00_cphDet_ucSearchConfComm_MDSearchConfComm");
        return true;
    }
</script>

