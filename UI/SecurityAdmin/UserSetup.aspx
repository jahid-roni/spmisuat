<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="UserSetup.aspx.cs" Inherits="SBM_WebUI.mp.UserSetup" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtLoginUserID.ClientID %>', 'TextBox', "Login User ID cannot be empty!");
            sErrorList += RequiredData('<%=ddlUserClass.ClientID %>', 'DropDownList', "User Class cannot be empty!");
            sErrorList += RequiredData('<%=txtUserFirstName.ClientID %>', 'TextBox', "First Name cannot be empty!");
            sErrorList += RequiredData('<%=txtUserLastName.ClientID %>', 'TextBox', "Last Name cannot be empty!");
            sErrorList += RequiredData('<%=txtNewPassword.ClientID %>', 'TextBox', "User Password cannot be empty!");
            sErrorList += RequiredData('<%=txtConfirmPassword.ClientID %>', 'TextBox', "Confirm Password cannot be empty!");
            sErrorList += RequiredData('<%=ddlDesignation.ClientID %>', 'DropDownList', "Please select Designation");
            sErrorList += RequiredData('<%=ddlDivision.ClientID %>', 'DropDownList', "Please select Division");

            var vPass = document.getElementById('<%=txtNewPassword.ClientID %>');
            var vConPass = document.getElementById('<%=txtConfirmPassword.ClientID %>');

            if (vPass != null && vConPass != null) {
                if (vPass.value != vConPass.value) {
                    sErrorList += "<li>User Password and Confirm Password do not match! Please check..</li>";
                }
            }
            return OpenErrorPanel(sErrorList, 'Save');
        }

        function DeleteUser() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdUserID = document.getElementById('<%=hdUserID.ClientID %>');
            if (hdUserID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this user')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>User has not been selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtLoginUserID.ClientID %>', 'TextBox', "Login User ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function DataMoveValidation() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdUserID = document.getElementById('<%=hdUserID.ClientID %>');
            if (hdUserID.value == "") {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>User has not been selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
            } else {
                return true;
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
        <legend runat="server" id="lgText">Unapproved User List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdUserID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                OnRowDataBound="gvData_RowDataBound" ID="gvData" runat="server" SkinID="SBMLGridGreen"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
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
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnFirst" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnPrevious" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnNext" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLast" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>User Setup Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Login User ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtLoginUserID" runat="server" MaxLength="20" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                        </td>
                        <td align="right">
                            Group
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlUserClass" runat="server" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            User First Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtUserFirstName" CssClass="textInput" runat="server"
                                    MaxLength="100" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            User Last Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtUserLastName" CssClass="textInput" runat="server"
                                    MaxLength="50" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            User Password
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtNewPassword" TextMode="Password" CssClass="textInput"
                                    MaxLength="20" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Confirm Password
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtConfirmPassword" TextMode="Password" CssClass="textInput"
                                    MaxLength="20" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Designation
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlDesignation" runat="server" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Division
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlDivision" runat="server" SkinID="ddlMedium">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="left">
                            <asp:CheckBox ID="chkStatus" runat="server" Text="Status"></asp:CheckBox>
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
        <table width="100%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;First&nbsp;&nbsp;"
                        ID="btnFirst" OnClick="btnFirst_Click" OnClientClick="return DataMoveValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Previous" ID="btnPrevious"
                        OnClick="btnPrevious_Click" OnClientClick="return DataMoveValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Next&nbsp;&nbsp;"
                        ID="btnNext" OnClick="btnNext_Click" OnClientClick="return DataMoveValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Last&nbsp;&nbsp;"
                        ID="btnLast" OnClick="btnLast_Click" OnClientClick="return DataMoveValidation()" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="CloseErrorPanel()"
                        OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteUser()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
