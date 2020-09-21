﻿<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="SPWiseAccountMapping" CodeBehind="SPWiseAccountMapping.aspx.cs" %>

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
        <legend>Unapproved Currency List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdSPTypeID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowDataBound="gvList_RowDataBound" OnRowCommand="gvList_RowCommand"
                                ID="gvList" runat="server" AutoGenerateColumns="true" SkinID="SBMLGridGreen"
                                ShowHeader="true" AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging">
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
                <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            SP Type
                        </td>
                        <td valign="top" colspan="3">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSPTypeID" SkinID="ddlLarge" runat="server" Width="150px">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            </div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Stock in Hand
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtStoInHandAcc" Width="195px" MaxLength="13" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Stock in Hand Name
                        </td>
                        <td>
                        <asp:TextBox ID="txtStoInHandName" Width="195px" MaxLength="70" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Liability on Stock
                        </td>
                        <td>
                            <asp:TextBox ID="txtLibOnStoAcc" Width="195px" MaxLength="15" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Liability on Stock Name
                        </td>
                        <td>
                        <asp:TextBox ID="txtLibOnStoName" Width="195px" MaxLength="70" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Holding Account
                        </td>
                        <td>
                            <asp:TextBox ID="txtHolAcc" Width="195px" MaxLength="15" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Holding Account Name
                        </td>
                        <td>
                        <asp:TextBox ID="txtHolName" Width="195px" MaxLength="70" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Accrued Commission
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAccComAcc" Width="195px" MaxLength="15" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Accrued Commission Name
                        </td>
                        <td>
                        <asp:TextBox ID="txtComName" Width="195px" MaxLength="70" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Advance Against Interest
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAdvAgiIntAcc" Width="195px" MaxLength="15" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right"> Advance Against Interest Name</td>
                        <td>
                            <asp:TextBox ID="txtAdvAgiIntName" Width="195px" MaxLength="70" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Advance Against Principal
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAdvAgiPriAcc" Width="195px" MaxLength="15" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right"> Advance Against Principal Name</td>
                        <td>
                            <asp:TextBox ID="txtAdvAgiPriName" Width="195px" MaxLength="70" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" pagecaption="SP Wise Account Mapping Search"
        caption="SP Type" type="SPWiseAccountMappingSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=Acc]').each(function() { $(this).mask("99-9999999-99-99"); });
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPTypeID.ClientID %>', 'DropDownList', "SP Type cannot  be empty!");
            sErrorList += RequiredData('<%=txtStoInHandAcc.ClientID %>', 'TextBox', "Stock in Hand cannot be empty!");
            sErrorList += RequiredData('<%=txtAccComAcc.ClientID %>', 'TextBox', "Accrued Commission Stock cannot be empty!");
            sErrorList += RequiredData('<%=txtAdvAgiIntAcc.ClientID %>', 'TextBox', "Advance Against Interest cannot be empty!");
            sErrorList += RequiredData('<%=txtAdvAgiPriAcc.ClientID %>', 'TextBox', "Advance Against Principal cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function Reset() {
            ResetData('<%=txtStoInHandAcc.ClientID %>');
            ResetData('<%=txtStoInHandName.ClientID %>');
            ResetData('<%=txtLibOnStoAcc.ClientID %>');
            ResetData('<%=txtLibOnStoName.ClientID %>');
            ResetData('<%=txtHolAcc.ClientID %>');
            ResetData('<%=txtHolName.ClientID %>');
            ResetData('<%=txtAccComAcc.ClientID %>');
            ResetData('<%=txtComName.ClientID %>');
            ResetData('<%=txtAdvAgiIntAcc.ClientID %>');
            ResetData('<%=txtAdvAgiIntName.ClientID %>');
            ResetData('<%=txtAdvAgiPriAcc.ClientID %>');
            ResetData('<%=txtAdvAgiPriName.ClientID %>');
            ResetData('<%=ddlSPTypeID.ClientID %>');
            var obj = document.getElementById('<%=ddlSPTypeID.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetData('<%=hdSPTypeID.ClientID %>');

            ResetUserDetails();
            CloseErrorPanel();

            return false;
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdSPTypeID = document.getElementById('<%=hdSPTypeID.ClientID %>');
            if (hdSPTypeID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Sanchaya Patra wise Account Mapping ')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sanchaya Patra wise Account Mapping has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPTypeID.ClientID %>', 'DropDownList', "SP Type cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }
    </script>

</asp:Content>
