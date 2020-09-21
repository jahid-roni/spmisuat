<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="CurrencySetup" CodeBehind="CurrencySetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchConfComm.ascx" TagName="ConfComm" TagPrefix="uc2" %>
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
        <legend>Unapproved Currency List</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td>
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdCurrencyID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="False" SkinID="SBMLGridGreen" ShowHeader="true"  AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging" >
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CurrencyID" HeaderText="Currency ID" />
                                    <asp:BoundField DataField="CurrencyCode" HeaderText="Currency Code" />
                                    <asp:BoundField DataField="CurrencySymbol" HeaderText="Currency Symbol" />
                                    <asp:BoundField DataField="CurrencyDesc" HeaderText="Currency Description" />
                                    <asp:BoundField DataField="MakeDate" HeaderText="Make Date" DataFormatString="{0:dd-MMM-yyyy}" />
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
        <legend>Currency Details</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Currency ID
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCurrencyID" CssClass="textInput" MaxLength="2" Width="50px" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td align="right" width="20%">
                            Currency Code
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCurrencyCode" MaxLength="3" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Currency Symbol
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtCurrencySymbol" MaxLength="3" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Description
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtDescription" MaxLength="35" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClientClick="  MsgPopupReturnTrue('Approve') "
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
    
    <uc2:ConfComm ID="ucSearchConfComm" runat="server" PageCaption="Currency Search"  Caption_1="Currency ID" Caption_2="Currency Code" Type="CurrencySearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function Reset() {
            ResetData('<%=txtCurrencyID.ClientID %>');
            var obj = document.getElementById('<%=txtCurrencyID.ClientID %>');
            if (obj != null) {
                obj.readOnly = false;
            }
            ResetData('<%=txtCurrencyCode.ClientID %>');
            ResetData('<%=txtCurrencySymbol.ClientID %>');
            ResetData('<%=txtDescription.ClientID %>');
            ResetData('<%=hdCurrencyID.ClientID %>');

            ResetUserDetails();

            return false;
        }
        
        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtCurrencyID.ClientID %>', 'TextBox', "Currency ID cannot be empty!");
            sErrorList += RequiredData('<%=txtCurrencyCode.ClientID %>', 'TextBox', "Currency Code cannot be empty!");
            sErrorList += RequiredData('<%=txtCurrencySymbol.ClientID %>', 'TextBox', "Currency Symbol cannot be empty!");
            sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdCurrencyID = document.getElementById('<%=hdCurrencyID.ClientID %>');
            if (hdCurrencyID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Currency')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Currency has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtCurrencyID.ClientID %>', 'TextBox', "Currency ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }
    </script>

</asp:Content>
