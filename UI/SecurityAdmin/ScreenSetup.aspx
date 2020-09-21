<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="ScreenSetup.aspx.cs" Inherits="SBM_WebUI.mp.ScreenSetup" %>

<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script language="javascript" type="text/javascript">
  function SaveValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtScreenName.ClientID %>', 'TextBox', "Screen Name cannot be empty!");
            sErrorList += RequiredData('<%=txtScreenID.ClientID %>', 'TextBox', "Screen ID cannot be empty!");
            sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");
            
            var rowsGvDenom = $("#<%=gvScreen.ClientID %> tr").length;
            if (rowsGvDenom == 1 || rowsGvDenom == 0) {
                sErrorList += "<li>Screen Object List cannot be null</li>";
            }

            return OpenErrorPanel(sErrorList, 'Save');
        }
        function DeleteScreen() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdScrID = document.getElementById('<%=hdScrID.ClientID %>');
            if (hdScrID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Screen')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Screen has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }

        }
        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }
        function SaveScreenObjectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtObjectName.ClientID %>', 'TextBox', "Object Name cannot be empty!");
            sErrorList += RequiredData('<%=txtObjectCaption.ClientID %>', 'TextBox', "Object Caption Level be empty!");
            sErrorList += RequiredData('<%=ddlOperationTypeId.ClientID %>', 'DropDownList', "select one Operation Type Id!");
//            sErrorList += RequiredData('<%=chkIsActive.ClientID %>', 'CheckBox', "No. Of Digits In Series be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }
        function ScreenObjectReset() {
            ResetData('<%=txtObjectName.ClientID %>');
            ResetData('<%=txtObjectCaption.ClientID %>');
            ResetData('<%=ddlOperationTypeId.ClientID %>');
            ResetData('<%=chkIsActive.ClientID %>');
            ResetData('<%=hdScrObjID.ClientID %>');

            ResetUserDetails();
            return false;
        }
        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtScreenID.ClientID %>', 'TextBox', "Screen ID cannot be empty!");
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
        <legend runat="server" id="lgText">Unapproved Screen List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdScrID" />
                <asp:HiddenField runat="server" ID="hdScrObjID" />
                
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
               
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
<fieldset>
        <legend>Screen Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            Screen ID
                        </td>
                        <td valign="top">
                                <asp:TextBox Width="140px" ID="txtScreenID" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click"  />
                        </td>
                        <td align="right" valign="top">
                            Screen Name
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" ID="txtScreenName" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        
                    </tr>
                    <tr>
                    <td align="right" valign="top">
                           Description
                        </td>
                        <td valign="top" colspan="3">
                            <div class="fieldLeft">
                                <asp:TextBox Width="140px" ID="txtDescription" CssClass="textInput" runat="server"
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
    <fieldset runat="server" id="fsDenom">
        <legend>Screen Object List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        
            <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdSLNo" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                        
                            <asp:GridView Width="98%" OnRowCommand="gvScreen_RowCommand" ID="gvScreen" runat="server"
                                AutoGenerateColumns="false" SkinID="SBMLGridGreen" ShowHeader="true" OnRowDeleting="gvScreen_RowDeleting">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" />
                                            <asp:HiddenField ID="bfSLNo"  runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnDelete"
                                            Text="Delete" OnClientClick="return CheckForDelete('this screen object')" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                    <asp:BoundField DataField="bfObjectName" HeaderText="Object Name" />
                                    <asp:BoundField DataField="bfObjectCaption" HeaderText="Object Caption" />
                                    
                                    
                                    <asp:BoundField DataField="bfOperationTypeID" HeaderText="Operation Type ID" />
                                    <asp:BoundField DataField="bfIsActive" HeaderText="Is Active" />
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
                <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSaveDenom" />--%>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Screen Object</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td width="20%" align="right">
                            Object Name
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtObjectName" MaxLength="7" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td width="20%" align="right">
                            Operation Type ID
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                            <asp:DropDownList ID="ddlOperationTypeId" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                                <%--<asp:TextBox ID="txtReOrderLevel" MaxLength="4" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>--%>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Object Caption
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtObjectCaption" MaxLength="12" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Is Active
                        </td>
                        <td>
                            <div class="fieldLeft">
                            
                                <%--<asp:TextBox ID="txtNoOfDigitInSeries" MaxLength="2" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>--%>
                              <asp:CheckBox runat="server" ID="chkIsActive"  />
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
        <div align="right" style="padding-top: 6px">
            <asp:Button CssClass="ButtonAsh" runat="server" Text="ScreenObject Add" ID="btnSaveScreenObject"
                OnClick="btnSaveScreenObject_Click" OnClientClick="return SaveScreenObjectValidation()" />
            &nbsp;
            <asp:Button CssClass="ButtonAsh" runat="server" Text="ScreenObject Reset" ID="btnScreenObjectReset"
                OnClientClick="return ScreenObjectReset()" />
        </div>
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
                                
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                    OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteScreen()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />

</asp:Content>
