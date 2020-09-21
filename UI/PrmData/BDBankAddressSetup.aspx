<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="True"
    Inherits="BDBankAddressSetup" CodeBehind="BDBankAddressSetup.aspx.cs" %>

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
        <legend>Unapproved Bangladesh Bank Address List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdSPTypeID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="true" OnRowDataBound="gvList_RowDataBound" SkinID="SBMLGridGreen"
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
        <legend>Address Setup</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            SP Type
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSPType" SkinID="ddlCommon" runat="server" Width="120px">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommSPPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td valign="top" align="right">
                            Sales Statement Address
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtSaleStatementAddress" TextMode="MultiLine" Height="50px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            Commission Claim Address
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCommClaimAddress" TextMode="MultiLine" Height="50px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td valign="top" align="right">
                            Inrest Claim Address
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtInrestClaimAddress" TextMode="MultiLine" Height="50px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right" width="20%">
                            Encashment Claim Address
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtEncashClaimAddress" TextMode="MultiLine" Height="50px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td valign="top" align="right" width="20%">
                            Reinvestment Address
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtReinAddress" TextMode="MultiLine" Height="50px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
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
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfCommSP ID="ucSearchConfCommSP" runat="server" pagecaption="Bangladesh Bank Address Search"
        caption="SP Type" type="BBAddressSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function Reset() {
            ResetData('<%=txtSaleStatementAddress.ClientID %>');
            ResetData('<%=txtCommClaimAddress.ClientID %>');
            ResetData('<%=txtInrestClaimAddress.ClientID %>');
            ResetData('<%=txtEncashClaimAddress.ClientID %>');
            ResetData('<%=txtReinAddress.ClientID %>');
            ResetData('<%=ddlSPType.ClientID %>');
            var obj = document.getElementById('<%=ddlSPType.ClientID %>');
            if (obj != null) {
                obj.disabled = false;
            }
            ResetUserDetails();
            CloseErrorPanel();
            return false;
        }
        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=txtSaleStatementAddress.ClientID %>', 'TextBox', "Sales Statement Address cannot be empty!");
            sErrorList += RequiredData('<%=txtCommClaimAddress.ClientID %>', 'TextBox', "Commission Claim Address cannot be empty!");
            sErrorList += RequiredData('<%=txtInrestClaimAddress.ClientID %>', 'TextBox', "Inrest Claim Address cannot be empty!");
            sErrorList += RequiredData('<%=txtEncashClaimAddress.ClientID %>', 'TextBox', "Encashment Claim Address cannot be empty!");
            sErrorList += RequiredData('<%=txtReinAddress.ClientID %>', 'TextBox', "Reinvestment Address cannot be empty!");

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
                if (CheckForDelete('this Bangladesh Bank Address')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Bangladesh Bank Address has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                return true;
            }
            // end of show error divErroList
        }
        
    </script>

</asp:Content>
