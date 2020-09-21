<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="GGPermissionSetup.aspx.cs" Inherits="SBM_WebUI.mp.GGPermissionSetup" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <%--<fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved Group Permission List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdGpID" />
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
    </fieldset>--%>
    <br />
    <fieldset>
        <legend>Group and Group Permission Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdGpID" />
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            Group ID
                        </td>
                        <td valign="top">
                            <asp:TextBox Width="140px" ID="txtGroupID" Enabled="false" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Group Name
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" ID="txtGroupName" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <%--<asp:Button CssClass="ButtonAsh" runat="server" Text="Search"  ID="btnSearch" OnClick="btnSearch_Click" />--%>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Search&nbsp;&nbsp;"
                                ID="btnSearch" OnClick="btnSearch_Click" OnClientClick="return SearchValidation()" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Description
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" ID="txtDescription" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top">
                            Department ID
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlDepartmentID" SkinID="ddlCommon" Width="145px" runat="server">
                                    <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                                </asp:DropDownList>
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
    <fieldset>
        <legend>Group Permission Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" cellspacing="0" cellpadding="5" border="1" style="color: Black;
                    background-color: #E9F0D5; border-color: #C1D586; border-width: 1px; border-style: Solid;
                    width: 98%; border-collapse: collapse;">
                    <tr style="color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal;
                        white-space: nowrap;">
                        <th style="width: 15px" scope="col">
                            Screen ID
                        </th>
                        <th style="width: 250px" scope="col">
                            Screen Name
                        </th>
                        <th style="width: 25px" scope="col">
                            <asp:CheckBox runat="server" Text="ADD" ID="ChkAdd" OnClick="SelectDeselectAll_tmp(this,'ADD')" />
                        </th>
                        <th style="width: 25px" scope="col">
                            <asp:CheckBox runat="server" Text="APPROVE" ID="ChkApprove" OnClick="SelectDeselectAll_tmp(this,'APPROVE')" />
                        </th>
                        <th style="width: 45px" scope="col">
                            <asp:CheckBox runat="server" Text="DELETE" ID="ChkDelete" OnClick="SelectDeselectAll_tmp(this,'DELETE')" />
                        </th>
                        <th style="width: 45px" scope="col">
                            <asp:CheckBox runat="server" Text="VIEW" ID="ChkView" OnClick="SelectDeselectAll_tmp(this,'VIEW')" />
                        </th>
                        <th style="width: 45px" scope="col">
                            <asp:CheckBox runat="server" Text="PRINT" ID="ChkPrint" OnClick="SelectDeselectAll_tmp(this,'PRINT')" />
                        </th>
                    </tr>
                </table>
                <div style="height: 200px; width: 100%; overflow: auto;">
                    <asp:GridView AutoGenerateColumns="false" ID="gvScreenList" runat="server" SkinID="SBMLGridGreen"
                        ShowHeader="false">
                        <Columns>
                            <asp:BoundField DataField="ScreenID" ItemStyle-Width="77px" HeaderText="Screen ID" />
                            <asp:BoundField DataField="ScreenName" ItemStyle-Width="330px" HeaderText="Screen Name" />
                            <asp:TemplateField HeaderText="ADD">
                                <ItemStyle HorizontalAlign="Left" Width="70px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("ADD") %>' ID="ADD" />
                                    <asp:HiddenField runat="server" Value='<%# Eval("hdGPID") %>' ID="hdGPID" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="APPROVE">
                                <ItemStyle HorizontalAlign="Left" Width="106px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("APPROVE") %>' ID="APPROVE" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DELETE">
                                <ItemStyle HorizontalAlign="Left" Width="87px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("DELETE") %>' ID="DELETE" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="VIEW">
                                <ItemStyle HorizontalAlign="Left" Width="79px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("VIEW") %>' ID="VIEW" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PRINT">
                                <ItemStyle HorizontalAlign="Left" Width="87px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("PRINT") %>' ID="PRINT" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <AlternatingRowStyle CssClass="odd" />
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
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
                        ID="btnFirst" OnClick="btnFirst_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Previous" ID="btnPrevious"
                        OnClick="btnPrevious_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Next&nbsp;&nbsp;"
                        ID="btnNext" OnClick="btnNext_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Last&nbsp;&nbsp;"
                        ID="btnLast" OnClick="btnLast_Click" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteGroup()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });


        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtGroupName.ClientID %>', 'TextBox', "Group Name cannot be empty!");
            sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");
            sErrorList += RequiredData('<%=ddlDepartmentID.ClientID %>', 'DropDownList', "Department cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtGroupName.ClientID %>', 'TextBox', "Group Name cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SearchValidation() {

            var sErrorList = RequiredData('<%=txtGroupName.ClientID %>', 'TextBox', "Group Name cannot be empty!");
            return OpenErrorPanel(sErrorList, '');

        }

        function Reset() {
            ResetData('<%=txtGroupName.ClientID %>');
            ResetData('<%=txtDescription.ClientID %>');
            ResetData('<%=ddlDepartmentID.ClientID %>');
            ResetData('<%=hdGpID.ClientID %>');
            ResetUserDetails();
            CloseErrorPanel();
            return false;
        }

        function DeleteGroup() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdGpID = document.getElementById('<%=hdGpID.ClientID %>');
            if (hdGpID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Group')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Group has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }


        function SelectDeselectAll_tmp(obj, type) {
            var TargetBaseControl = document.getElementById('<%= this.gvScreenList.ClientID %>');


            var Inputs = TargetBaseControl.getElementsByTagName("input");
            var oADD = new Array(Inputs.length);
            var oAPPROVE = new Array(Inputs.length);
            var oDELETE = new Array(Inputs.length);
            var oPRINT = new Array(Inputs.length);
            var oVIEW = new Array(Inputs.length);

            for (var n = 0; n < Inputs.length; n++) {
                if (Inputs[n].type == 'checkbox') {
                    var chkBoxID = Inputs[n].id;
                    var iIndex = chkBoxID.lastIndexOf("_")

                    if (type == "ADD") {
                        oADD[n] = document.getElementById(chkBoxID.substr(0, iIndex) + '_ADD');
                        oADD[n].checked = obj.checked;
                    }
                    else if (type == "APPROVE") {
                        oAPPROVE[n] = document.getElementById(chkBoxID.substr(0, iIndex) + '_APPROVE');
                        oAPPROVE[n].checked = obj.checked;
                    }
                    else if (type == "DELETE") {
                        oDELETE[n] = document.getElementById(chkBoxID.substr(0, iIndex) + '_DELETE');
                        oDELETE[n].checked = obj.checked;
                    }
                    else if (type == "PRINT") {
                        oPRINT[n] = document.getElementById(chkBoxID.substr(0, iIndex) + '_PRINT');
                        oPRINT[n].checked = obj.checked;
                    }
                    else if (type == "VIEW") {
                        oVIEW[n] = document.getElementById(chkBoxID.substr(0, iIndex) + '_VIEW');

                        oVIEW[n].checked = obj.checked;
                    }
                }
            }
        }
    </script>

</asp:Content>
