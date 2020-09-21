<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="CurrencyWiseAccMapping" CodeBehind="CurrencyWiseAccMapping.aspx.cs" %>

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
        <br />
    </div>
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Currency wise Account Mapping List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdnCurrencyID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="true" SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvList_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" OnClientClick="CloseErrorPanel() " />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Account Mapping</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Currency Code
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCurrencyID" runat="server" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation(),CloseErrorPanel()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Suspense A/c
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtSusPenAcc" MaxLength="13" Width="180px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">Suspense A/c Name
                        </td>
                        <td><asp:TextBox ID="txtSusPenName" MaxLength="70" Width="180px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Foreign Exchange A/c
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtForExcchAcc" Width="180px" MaxLength="13" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">Foreign Exchange A/c Name
                        </td>
                        <td>
                        <asp:TextBox ID="txtForExcchName" Width="180px" MaxLength="70" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Branch Fx A/c
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBraFaxAcc"  Width="180px"  MaxLength="13" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">Branch Fx A/c Name
                        </td>
                        <td><asp:TextBox ID="txtBraFaxName"  Width="180px"  MaxLength="70" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Branch Exchange Fx A/c
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBraExcFaxAcc" Width="180px" MaxLength="13" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">Branch Exchange Fx A/c Name
                        </td>
                        <td><asp:TextBox ID="txtBraExcFaxName" Width="180px" MaxLength="70" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Bangladesh Bank A/c
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBngBankAcc"  Width="180px"  MaxLength="13" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">Bangladesh Bank A/c Name
                        </td>
                        <td><asp:TextBox ID="txtBngBankName"  Width="180px"  MaxLength="70" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset(this.form)" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" PageCaption="Currency wise Account Mapping Search"
        Caption="Currency Code" Type="CurrencywiseAccountMappingSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=Acc]').each(function() { $(this).mask("99-9999999-99-99"); });
        }


        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtSusPenAcc.ClientID %>', 'TextBox', "Suspense A/C cannot be empty!");
            sErrorList += RequiredData('<%=txtBngBankAcc.ClientID %>', 'TextBox', "Bangladesh Bank A/C cannot be empty!");
            sErrorList += RequiredData('<%=ddlCurrencyID.ClientID %>', 'DropDownList', "Currency cannot be empty!");
            sErrorList += RequiredData('<%=txtForExcchAcc.ClientID %>', 'TextBox', "For Excch  A/C cannot be empty!");
            sErrorList += RequiredData('<%=txtBraFaxAcc.ClientID %>', 'TextBox', "Bra Fax A/C cannot be empty!");
            sErrorList += RequiredData('<%=txtBraExcFaxAcc.ClientID %>', 'TextBox', "Bra Exc Fax A/C cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function Reset() {
            ResetData('<%=txtSusPenAcc.ClientID %>');
            ResetData('<%=txtForExcchAcc.ClientID %>');
            ResetData('<%=txtBraFaxAcc.ClientID %>');
            ResetData('<%=txtBraExcFaxAcc.ClientID %>');
            ResetData('<%=txtBngBankAcc.ClientID %>');
            ResetData('<%=ddlCurrencyID.ClientID %>');

            var obj = document.getElementById('<%=ddlCurrencyID.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetData('<%=hdnCurrencyID.ClientID %>');

            ResetUserDetails();
            CloseErrorPanel();

            return false;
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlCurrencyID.ClientID %>', 'DropDownList', "Currency cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdCurrencyID = document.getElementById('<%=hdnCurrencyID.ClientID %>');
            if (hdCurrencyID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this currency wise account mapping')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Currency wise account mapping has not selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
        
    </script>

</asp:Content>
