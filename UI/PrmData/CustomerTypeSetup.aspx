<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="CustomerTypeSetup" CodeBehind="CustomerTypeSetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchCustType.ascx" TagName="SearchCustType" TagPrefix="uc2" %>
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
        <legend>Unapproved Customer Type List</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td>
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdCustomerTypeID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView Width="98%" OnRowDataBound="gvList_RowDataBound" AutoGenerateColumns="true"
                                OnRowCommand="gvList_RowCommand" ID="gvList" runat="server" SkinID="SBMLGridGreen"
                                ShowHeader="true" AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging" >
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" OnClientClick="CloseErrorPanel()" CommandName="Select"
                                                runat="server" ID="btnSelect" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Customer Details</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right" width="35%">
                            Customer Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" MaxLength="2" ID="txtCustomerTypeID" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button CssClass="ButtonAsh" OnClientClick="return LoadValidation()" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            &nbsp;<asp:Button OnClientClick="return CustTypePopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                            &nbsp;<asp:CheckBox ID="chkIsOrganization" onClick="ChkChangeColor(this)" Text="Is Organization"
                                runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Description
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="320px" MaxLength="35" TextMode="MultiLine" Height="40px" ID="txtDescription"
                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            No. of Maximum Members
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="200px" MaxLength="4" ID="txtNoOfMaxMembers" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClientClick=" MsgPopupReturnTrue('Approve') "
                        OnClick="btnApprove_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:SearchCustType ID="ucSearchCustType" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtNoOfMaxMembers]').keypress(function(e) { return intNumber(e); });
        }


        function Reset() {
            ResetData('<%=txtCustomerTypeID.ClientID %>');
            var obj = document.getElementById('<%=txtCustomerTypeID.ClientID %>');
            if (obj != null) {
                obj.readOnly = false;
            }
            ResetData('<%=txtDescription.ClientID %>');
            ResetData('<%=txtNoOfMaxMembers.ClientID %>');
            ResetData('<%=chkIsOrganization.ClientID %>');
            ResetData('<%=hdCustomerTypeID.ClientID %>');

            ResetUserDetails() 
            CloseErrorPanel();
            return false;
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtCustomerTypeID.ClientID %>', 'TextBox', "Customer Type cannot be empty!");
            sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");
            sErrorList += RequiredData('<%=txtNoOfMaxMembers.ClientID %>', 'TextBox', "No. of Maximum Members cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_txtCustomerTypeID', 'TextBox', "Customer Type cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdCustomerTypeID = document.getElementById('<%=hdCustomerTypeID.ClientID %>');
            if (hdCustomerTypeID.value != "") {
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
                errorList.innerHTML = "<ul><li>Customer Type Setup has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
        
    </script>

</asp:Content>
