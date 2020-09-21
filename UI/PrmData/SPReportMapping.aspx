<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="SPReportMapping" CodeBehind="SPReportMapping.aspx.cs" %>

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
        <legend>Unapproved Report Mapping List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdSPTypeID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView OnRowCommand="gvList_RowCommand" AutoGenerateColumns="true" Width="98%"
                                ID="gvList" runat="server" OnRowDataBound="gvList_RowDataBound" SkinID="SBMLGridGreen"
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
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Report Mapping</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSPType" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button ID="btnLoad" OnClientClick="return LoadValidation()" OnClick="btnLoad_Click"
                                runat="server" CssClass="ButtonAsh" Text="Load" />
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td align="right">
                            Sale Statement
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSaleStatement" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnSalesPrv" 
                                OnClick="btnSalesPrv_Click" Visible="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Interest Claim
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlInterestClaim" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnInterestPrv"
                                OnClick="btnInterestPrv_Click" Visible="False" />
                        </td>
                        <td align="right">
                            Encashment Claim
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlEncashmentClaim" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnEncashmentPrv"
                                OnClick="btnEncashmentPrv_Click" Visible="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Commission Claim Format
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCommissionClaim" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnCommissionPrv"
                                OnClick="btnCommissionPrv_Click" Visible="False" />
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" pagecaption="Report Mapping Search"
        caption="SP Type" type="ReportMappingSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function Reset() {
            ResetData('<%=ddlSPType.ClientID %>');
            var obj = document.getElementById('<%=ddlSPType.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetData('<%=ddlSaleStatement.ClientID %>');
            ResetData('<%=ddlInterestClaim.ClientID %>');
            ResetData('<%=ddlEncashmentClaim.ClientID %>');
            ResetData('<%=ddlCommissionClaim.ClientID %>');
            ResetData('<%=hdSPTypeID.ClientID %>');
            ResetUserDetails();

            CloseErrorPanel();
            
            return false;
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'TextBox', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlSaleStatement.ClientID %>', 'TextBox', "Sale Statement cannot be empty!");
            sErrorList += RequiredData('<%=ddlInterestClaim.ClientID %>', 'TextBox', "Interest Claim cannot be empty!");
            sErrorList += RequiredData('<%=ddlEncashmentClaim.ClientID %>', 'TextBox', "Encashment Claim cannot be empty!");
            sErrorList += RequiredData('<%=ddlCommissionClaim.ClientID %>', 'TextBox', "Commission Claim cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
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
                if (CheckForDelete('this Sanchaya Patra wise Format Mapping')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sanchaya Patra wise Format Mapping has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }


        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }
        
    </script>

</asp:Content>
