<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="SPCertificateMapping" CodeBehind="SPCertificateMapping.aspx.cs" %>

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
        <legend>Unapproved Certificate Mapping List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdSPTypeID" />
                <asp:HiddenField runat="server" ID="hdDenomination" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="true" OnRowDataBound="gvList_RowDataBound" SkinID="SBMLGridGreen"
                                ShowHeader="true" AllowPaging="True" PageSize="2" OnPageIndexChanging="gvList_PageIndexChanging">
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
    <fieldset>
        <legend>Certificate Mapping</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td width="20%" align="right">
                            SP Type
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSPType" runat="server" SkinID="ddlMedium" OnSelectedIndexChanged="ddlSPType_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td width="20%" align="right">
                            &nbsp;
                        </td>
                        <td width="30%">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Denomination
                        </td>
                        <td>
                            <asp:UpdatePanel ID="upDenomination" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlDenomination" runat="server" SkinID="ddlMedium">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="errorIcon">
                                        *</div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlSPType" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td align="right">
                            Script Format
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList runat="server" ID="ddlScriptFormat" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnCommissionPrv" />
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" PageCaption="Sanchaya Patra wise Certificate format Mapping Search"
        Caption="SP Type" Type="SPCertificateFormatMappingSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function Reset() {
            ResetData('<%=ddlSPType.ClientID %>');
            ResetData('<%=ddlDenomination.ClientID %>');
            ResetData('<%=ddlScriptFormat.ClientID %>');

            var obj = document.getElementById('<%=ddlSPType.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            var obj1 = document.getElementById('<%=ddlDenomination.ClientID %>');
            if (obj1 != null) {
                obj1.disabled = false;
            }
            var i;
            for (i = obj1.options.length - 1; i >= 0; i--) {
                obj1.remove(i);
            }

            ResetUserDetails();
            CloseErrorPanel();
            
            return false;
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Reject');
        }


        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type  cannot be empty!");
            sErrorList += RequiredData('<%=ddlDenomination.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=ddlScriptFormat.ClientID %>', 'DropDownList', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }


        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdSPTypeID = document.getElementById('<%=hdSPTypeID.ClientID %>');
            if (hdSPTypeID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Sanchaya Patra wise Certificate format Mapping')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sanchaya Patra wise Certificate format Mapping has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
    </script>

</asp:Content>
