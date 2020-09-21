<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SecurityPolicySetup.aspx.cs" Inherits="SBM_WebUI.mp.SecurityPolicySetup" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=txtOldPwprohibited]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtMinimumPwLg]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtPwExpDays]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtDayEarly]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtWrongLoginTrial]').keypress(function(e) { return intNumber(e); });
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtOldPwprohibited.ClientID %>', 'TextBox', "How many most recent old password should be prohibited as new password cannot be empty!");
            sErrorList += RequiredData('<%=txtMinimumPwLg.ClientID %>', 'TextBox', "Minimum password length cannot be empty!");
            sErrorList += RequiredData('<%=txtPwExpDays.ClientID %>', 'TextBox', "Password expires after days cannot be empty!");
            sErrorList += RequiredData('<%=txtDayEarly.ClientID %>', 'TextBox', "How many days earlier a user be informed of password expiration cannot be empty!");
            sErrorList += RequiredData('<%=txtWrongLoginTrial.ClientID %>', 'TextBox', "Maximum wrong login trial cannot be empty!");
            
            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }
        
        function DeleteSP() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdSPID = document.getElementById('<%=hdSecurityPolicyID.ClientID %>');
            if (hdSPID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Security Policy')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Security Policy has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }
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
    
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Security Policy Data List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                        <ContentTemplate>
                            <asp:GridView OnRowCommand="gvList_RowCommand" AutoGenerateColumns="true"
                                Width="98%" OnRowDataBound="gvList_RowDataBound" ID="gvList" runat="server" SkinID="SBMLGridGreen">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select"  OnClientClick="CloseErrorPanel()" />
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
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    
    <fieldset>
        <legend>Security Policy Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdSecurityPolicyID" />
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="40%" valign="top">
                            1. Is user required to change password on first log on?
                        </td>
                        <td width="10%">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlRequiredToChangePw" runat="server" SkinID="ddlCommon">
                                    <asp:ListItem Value="true" Text="Yes"></asp:ListItem>
                                    <asp:ListItem Value="false" Text="No"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" width="40%" valign="top">
                            2. How many most recent old password should be prohibited as new password?
                        </td>
                        <td width="10%">
                            <div class="fieldLeft">
                                <asp:TextBox Width="50px" MaxLength="2" ID="txtOldPwprohibited" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            3. Minimum password length
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="50px" MaxLength="2" ID="txtMinimumPwLg" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            4. Force alpha numeric password?
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlAlphaNumPw" runat="server" SkinID="ddlCommon">
                                    <asp:ListItem Value="true" Text="Yes"></asp:ListItem>
                                    <asp:ListItem Value="false" Text="No"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            5. Password expires after days
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="50px" MaxLength="2" ID="txtPwExpDays" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            6. How many days earlier a user be informed of password expiration?
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="50px" MaxLength="3" ID="txtDayEarly" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            7. Maximum wrong login trial
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="50px" MaxLength="2" ID="txtWrongLoginTrial" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="center" colspan="2" >
                            <asp:Button CssClass="ButtonAsh" ID="btnLoad" runat="server" Text="Load Approved Data"
                                OnClick="btnLoad_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteSP()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
