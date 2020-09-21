<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="OperationCodeSetup.aspx.cs" Inherits="SBM_WebUI.mp.OperationCodeSetup" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script language="javascript" type="text/javascript">
    function SaveValidation() {
        var sErrorList = "";
        sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");

        return OpenErrorPanel(sErrorList, 'Save');
    }

    function DeleteDescription() {
        var divErrorPanel = document.getElementById('divErrorPanel');

        var hdOpID = document.getElementById('<%=hdOpID.ClientID %>');
        if (hdOpID.value != "") {
            if (CheckForDelete('this Description')) {
                MsgPopupReturnTrue('Delete');
                return true;
            } else {
                return false;
            }
        }
        else {
            // show error divErrorList
            var errorList = document.getElementById('divErrorList');
            errorList.innerHTML = "<ul><li>Description has not selected yet</li></ul>";
            divErrorPanel.style.display = "block";
            return false;
            // end of show error divErroList
        }
    }
    
 function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtOperationID.ClientID %>', 'TextBox', "Operation ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
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
        <legend runat="server" id="lgText">Operation Code List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdOpID" />
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
                
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                
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
        <legend>Operation Code Detail Setup</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" valign="top">
                            Operation ID
                        </td>
                        <td valign="top">
                                <asp:TextBox Width="140px" ID="txtOperationID" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                        </td>
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
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
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
                    
                    
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteDescription()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
