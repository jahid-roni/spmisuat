<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="DepartmentSetup.aspx.cs" Inherits="SBM_WebUI.mp.DepartmentSetup" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDepartmentID]').keypress(function(e) { return intNumber(e); });
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SaveValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtDepartmentName.ClientID %>', 'TextBox', "Department Name cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function DeleteDepartment() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdDptID = document.getElementById('<%=hdDptID.ClientID %>');
            if (hdDptID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Department')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Department has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtDepartmentID.ClientID %>', 'TextBox', "Department ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
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
        <legend runat="server" id="lgText">Unapproved Department List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdDptID" />
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnFirst" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnPrevious" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnNext" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLast" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    
    <fieldset>
        <legend>Department Detail Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            Department ID
                        </td>
                        <td valign="top">
                                <asp:TextBox Width="140px" ID="txtDepartmentID" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                        </td>
                        <td align="right" valign="top">
                            Department Name
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" ID="txtDepartmentName" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteDepartment()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
