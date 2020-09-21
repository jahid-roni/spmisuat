<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCViewGridData.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCViewGridData" %>
<div id="MDViewGridData" class="MDClass" runat="server">
    <asp:UpdatePanel runat="server" ID="upSuccess">
        <ContentTemplate>
            <table width="900" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <div class="MDHeader" style="width: 100%">
                            <div class="MDTitle">
                                <asp:Label runat="server" ID="lblPageCaption"></asp:Label></div>
                            <div class="MDClose">
                                <a href="javascript:void(0);">
                                    <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_VGD_MDViewGridData');" />
                                </a>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="MDBody" style="height: 350px; overflow: auto;">
                            <table width="850" align="center" class="tableBody" border="0">
                                <tr>
                                    <td>
                                        <div style="height: 100%; width: 100%; overflow: auto;">
                                            <asp:GridView AutoGenerateColumns="true" Width="98%" ID="gvData" runat="server" SkinID="SBMLGridGreen"
                                                OnRowDataBound="gvData_RowDataBound" AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging">
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding-bottom:10px">
                        <asp:Button ID="btnClose" runat="server" CssClass="ButtonAsh" Text="&nbsp;&nbsp;&nbsp;&nbsp;Close&nbsp;&nbsp;&nbsp;&nbsp;"
                            OnClientClick="return VGDataPopupHide()" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<script language="javascript" type="text/javascript">
    function VGDataPopupHide() {
        hideModal('ctl00_cphDet_VGD_MDViewGridData');
        return true;
    }
    function VGDataShow() {
        showModal('ctl00_cphDet_VGD_MDViewGridData');
        return true;
    }
</script>

