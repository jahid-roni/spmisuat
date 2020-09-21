<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMessage.ascx.cs" Inherits="SBM_WebUI.UI.UC.UCMessage" %>
<style type="text/css">
    .auto-style1 {
        width: 65px;
    }
</style>
<div id="MDMessage" class="MDClass" runat="server">
    <table width="470" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label runat="server" ID="lblPageCaption"></asp:Label></div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <asp:ImageButton runat="server" ID="imgClose" ImageUrl="~/Images/popupclose.gif"
                                OnClientClick="return MsgPopupHide()" OnClick="btnClose_Click" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 155px; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdReturnType" />
                            <table width="98%" align="center">
                                <tr>
                                    <td class="auto-style1">
                                        <asp:Image runat="server" ID="imgID" ImageUrl="~/Images/progress.jpg" Height="55px" Width="54px" />
                                    </td>
                                    <td>
                                        <div>
                                            <asp:Label CssClass="lblError" runat="server" ID="lblMsg" Text="Operation progressing.."></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table width="30%" align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnClose" runat="server" CssClass="ButtonAsh" Text="&nbsp;&nbsp;&nbsp;&nbsp;Close&nbsp;&nbsp;&nbsp;&nbsp;"
                                            OnClientClick="return MsgPopupHide()" OnClick="btnClose_Click" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnClose" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="imgClose" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function MsgPopupHide() {
        hideModal('ctl00_cphDet_ucMessage_MDMessage');
        return true;
    }
    function MsgPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_ucMessage_MDMessage');
        return true;
    }
    function MsgPopupReturnTrue(vData) {
        var oReturnType = document.getElementById('<%=hdReturnType.ClientID %>');
        if (oReturnType != null) {
            oReturnType.value = vData;
        }
        window.scroll(0, 0);
        showModal("ctl00_cphDet_ucMessage_MDMessage");
        return true;
    }
</script>

