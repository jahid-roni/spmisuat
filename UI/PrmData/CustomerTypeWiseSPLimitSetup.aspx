<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="CustomerTypeWiseSPLimitSetup" CodeBehind="CustomerTypeWiseSPLimitSetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchCustTypeWiseSPLimit.ascx" TagName="CustTypeWiseSPLimitSetup"
    TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="Server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input:</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend>Unapproved SP Type Wise Limit List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdCustomerID" />
                <asp:HiddenField runat="server" ID="hdSPTypeID" />
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
                                                Text="Select" OnClientClick="CloseErrorPanel()" />
                                            <asp:HiddenField runat="server" ID="hdCustTypeID" />
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
    <fieldset id="fsCommon">
        <legend>Customer Type wise SP Limit Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            SP Type
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:DropDownList runat="server" ID="ddlSPType" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *<asp:Button OnClientClick="return CustTypeWiseSPLimitPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                    runat="server" Text="Search" ID="btnSearch" /></div>
                        </td>
                        <td align="right">
                            Minimum Limit
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox CssClass="textInput" MaxLength="18" Width="100px" runat="server" ID="txtMinLim"
                                    onblur="blurActiveInputWithObj(this), CheckFromToData(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" width="20%">
                            Customer Type
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:DropDownList runat="server" ID="ddlCustType" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Maximum Limit
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox CssClass="textInput" MaxLength="18" Width="100px" runat="server" ID="txtMaxLim"
                                    onblur="blurActiveInputWithObj(this), CheckFromToData(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click"
                        OnClientClick="return Delete()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:CustTypeWiseSPLimitSetup ID="ucSearchCustTypeWiseSPLimit" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });

            $('input[id*=txtMinLim]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtMaxLim]').keypress(function(e) { return intNumber(e); });
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtMinLim.ClientID %>', 'TextBox', "Minimum Limit cannot be empty!");
            sErrorList += RequiredData('<%=txtMaxLim.ClientID %>', 'TextBox', "Maximum Limit cannot be empty!");

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlCustType.ClientID %>', 'DropDownList', "Customer Type Limit cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function Reset() {
            ResetData('<%=txtMinLim.ClientID %>');
            ResetData('<%=txtMaxLim.ClientID %>');
            ResetData('<%=ddlSPType.ClientID %>');
            ResetData('<%=ddlCustType.ClientID %>');
            var obj = document.getElementById('<%=ddlSPType.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            obj = document.getElementById('<%=ddlCustType.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetData('<%=hdSPTypeID.ClientID %>');
            ResetData('<%=hdCustomerID.ClientID %>');

            ResetUserDetails();
            CloseErrorPanel();

            return false;
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdSPTypeID = document.getElementById('<%=hdSPTypeID.ClientID %>');
            if (hdSPTypeID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Customer Type wise SP Limit Setup')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Customer Type has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function CheckFromToData(obj) {
            var oFrom = document.getElementById('<%=txtMinLim.ClientID %>');
            var vFrom
            if (oFrom != null) {
                vFrom = parseInt(oFrom.value);
            }
            var oTo = document.getElementById('<%=txtMaxLim.ClientID %>');
            var vTo
            {
                vTo = parseInt(oTo.value);
            }

            if (oFrom.value != "" && oTo.value != "") {
                if (vFrom >= vTo) {
                    alert("Data is not in correct format");
                    obj.value = "";
                    return false;
                } else {
                    return true;
                }
            }
        }
        
    </script>

</asp:Content>
