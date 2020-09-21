<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="CommonMappingSetup" CodeBehind="CommonMappingSetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchConfCommSP.ascx" TagName="ConfCommSP" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="Server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
    </div>
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Common Mapping List</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td>
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdCurrencyID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="true" SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvList_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" OnClientClick="CloseErrorPanel()" />
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
        <legend>Currency Details</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Currency ID
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCurrencyID" runat="server" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            P & L Account for Commission & Charges
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAccount" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Account Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccName" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                MaxLength="70" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
            </Triggers>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" pagecaption="Common Account Mapping Search"
        caption="Currency ID" type="CommonAccountMappingSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtAccount]').each(function() { $(this).mask("99-9999999-99-99"); });
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlCurrencyID.ClientID %>', 'DropDownList', "Currency ID cannot be empty!");
            sErrorList += RequiredData('<%=txtAccount.ClientID %>', 'TextBox', "P & L Account for Commission & Charges cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdCurrencyID = document.getElementById('<%=hdCurrencyID.ClientID %>');
            if (hdCurrencyID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Currency')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Currency has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }


        function Reset() {
            ResetData('<%=txtAccount.ClientID %>');
            ResetData('<%=txtAccName.ClientID %>');
            ResetData('<%=ddlCurrencyID.ClientID %>');
            var obj = document.getElementById('<%=ddlCurrencyID.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetData('<%=hdCurrencyID.ClientID %>');

            ResetUserDetails();
            CloseErrorPanel();

            return false;
        }
        
    </script>

</asp:Content>
