<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="SBM_WebUI.mp.ChangePassword" %>

<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function ResetPw() {
            ResetData('<%=txtOldPassword.ClientID %>');
            ResetData('<%=txtNewPassword.ClientID %>');
            ResetData('<%=txtConfirmPassword.ClientID %>');
            return false;
        }

        function SaveValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtOldPassword.ClientID %>', 'TextBox', "Old Password cannot be empty!");
            sErrorList += RequiredData('<%=txtNewPassword.ClientID %>', 'TextBox', "New Password cannot be empty!");
            sErrorList += RequiredData('<%=txtConfirmPassword.ClientID %>', 'TextBox', "Confirm Password cannot be empty!");

            var vPass = document.getElementById('<%=txtNewPassword.ClientID %>');
            var vConPass = document.getElementById('<%=txtConfirmPassword.ClientID %>');

            if (vPass != null && vConPass != null) {
                if (vPass.value != vConPass.value) {
                    sErrorList += "<li>New Password and Confirm Password do not match! Please check..</li>";
                }
            }
            
            return OpenErrorPanel(sErrorList, 'Save');
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset>
        <legend>Change Password</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Login User ID
                        </td>
                        <td width="30%">
                            <asp:TextBox ID="txtLoginUserID" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td align="right" width="20%">
                            Old Password
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtOldPassword" CssClass="textInput" TextMode="Password" runat="server" MaxLength="20" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            New Password
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNewPassword" CssClass="textInput" TextMode="Password" MaxLength="20" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Confirm Password
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtConfirmPassword" CssClass="textInput" TextMode="Password" MaxLength="20" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" OnClientClick="return SaveValidation()"
                        ID="btnSave" OnClick="btnSave_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return ResetPw()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
