<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ACERegistration.aspx.cs" Inherits="SBM_WebUI.UI.Transaction.ACERegistration" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCCustomerDetail_HSB.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function CheckNextData() {
            var reg = document.getElementById("<%=hdRegNo.ClientID %>");
            if (reg != null) {
                if (reg.value == "") {
                    MsgPopupReturnTrue('Info');
                }
            }
            return true;
        }

        function ShowPolicyDetail() {
            var reg = document.getElementById("<%=hdRegNo.ClientID %>");
            if (reg != null) {
                if (reg.value == "") {
                    MsgPopupReturnTrue('Info');
                } else {
                    PolicyDetailPopupReturnTrue();
                }
            }
            return true
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdRegNo = document.getElementById('<%=hdRegNo.ClientID %>');

            if (hdRegNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Data')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>ACER transaction has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
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
    <fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved ACE List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel9">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="910px"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" OnClientClick="CloseErrorPanel()"
                                                runat="server" ID="btnSelect" Text="Select" />
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
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel8">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" Enabled="false" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td align="left">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="140px" CssClass="textInput" runat="server"
                                    MaxLength="25" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnRegSearch_Click" Text="Search"
                                    ID="bntRegSeach" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" /></div>
                        </td>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Applied Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtAppliedAmount" Width="100px" CssClass="textInput" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Issue Name
                        </td>
                        <td valign="top">
                                <asp:TextBox ID="txtIssueName" Width="300px" TextMode="MultiLine" Height="37" CssClass="textInput"
                                    runat="server" Enabled="False"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Master No
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtMasterNo" Width="100px" MaxLength="7" CssClass="textInput" 
                                runat="server" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLeftShift" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnRightShift" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="bntRegSeach" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <table width="100%" align="center" class="tableBody" border="0">
        <tr>
            <td valign="top" width="60%">
                <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                    <ContentTemplate>
                        <fieldset>
                            <legend>Certificate(s) Details</legend>
                            <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td valign="top">
                                        <div style="height: 170px; width: 98%; overflow: auto;">
                                            <asp:GridView Style="width: 98%" ID="gvDenomDetail" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true">
                                                <Columns>
                                                    <asp:BoundField DataField="bfDenomination" HeaderText="Denomination" />
                                                    <asp:BoundField DataField="bfSeries" HeaderText="Series" />
                                                    <asp:BoundField DataField="bfSerialNo" HeaderText="Serial No" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
            </td>
            <td valign="top" width="40%">
                <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                    <ContentTemplate>
                        <fieldset>
                            <legend>ACE Registration</legend>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td align="right">
                                        Status
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlStatus" SkinID="ddlMedium" Width="150px" runat="server"
                                            Enabled="False">
                                            <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                            <asp:ListItem Text="Deactive" Value="Deactive"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Date
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStatusDate" Width="150px" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Account/Draft No
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:TextBox Width="120px" ID="txtPDAccDraftNo" Enabled="false" CssClass="textInput"
                                                runat="server" MaxLength="12" AutoPostBack="True" OnTextChanged="txtPDAccDraftNo_TextChanged"></asp:TextBox></div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Account Name
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAccountName" Width="180px" CssClass="textInputDisabledForTxt"
                                            runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Inventory Location
                                    </td>
                                    <td>
                                        <asp:TextBox Width="180px" ID="txtInvLocation" MaxLength="50" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Remarks
                                    </td>
                                    <td>
                                        <asp:TextBox Width="220px" Height="30px" ID="txtRemarks" CssClass="textInput" TextMode="MultiLine"
                                            runat="server" onblur="blurActiveInputWithObj(this)" MaxLength="255" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="98%" align="center" class="tableBody" border="0">
            <tr>
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick="MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClick="btnSave_Click"
                        OnClientClick="MsgPopupReturnTrue('Save')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                    <asp:Button CssClass="ButtonAsh" Width="75px" runat="server" Text="<<<" ID="btnLeftShift"
                        OnClick="btnLeftShift_Click" OnClientClick="CheckNextData()" />
                    <asp:Button CssClass="ButtonAsh" Width="75px" runat="server" Text=">>>" ID="btnRightShift"
                        OnClick="btnRightShift_Click" OnClientClick="CheckNextData()" />
                </td>
                <td align="right">
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:CustomerDetails ID="CustomerDetail" runat="server" />
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" Type="2" />
    <uc5:Error ID="ucMessage" runat="server" />
</asp:Content>
