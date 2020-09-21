<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="True"
    Inherits="SPTypeSetup" CodeBehind="SPTypeSetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchSPType.ascx" TagName="SearchSPType" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src="../../Scripts/util.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDenomination]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtReOrderLevel]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtNoOfDigitInSeries]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtSeries]').keypress(function(e) { return textInput(e); });
        }

        function DenomReset() {
            ResetData('<%=txtDenomination.ClientID %>');
            ResetData('<%=txtReOrderLevel.ClientID %>');
            ResetData('<%=txtSeries.ClientID %>');
            ResetData('<%=txtNoOfDigitInSeries.ClientID %>');
            $('input[id*=txtDenomination]').attr("disabled", "true");
            $('input[id*=txtSeries]').attr("disabled", "true");            
            $('input[id*=txtDenomination]').removeAttr('disabled');
            $('input[id*=txtSeries]').removeAttr('disabled');

            ResetUserDetails();
            return false;
        }
        function Reset() {            
            ResetUserDetails();
            return true;
        }

        function SaveDenomValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtDenomination.ClientID %>', 'TextBox', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtReOrderLevel.ClientID %>', 'TextBox', "Re-Order Level be empty!");
            sErrorList += RequiredData('<%=txtSeries.ClientID %>', 'TextBox', "Series be empty!");
            sErrorList += RequiredData('<%=txtNoOfDigitInSeries.ClientID %>', 'TextBox', "No. Of Digits In Series be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtSpTypeId.ClientID %>', 'TextBox', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=txtDescription.ClientID %>', 'TextBox', "Description cannot be empty!");
            sErrorList += RequiredData('<%=ddlCurrency.ClientID %>', 'DropDownList', "Currency cannot be empty!");

            var rowsGvDenom = $("#<%=gvDenom.ClientID %> tr").length;
            if (rowsGvDenom == 1 || rowsGvDenom == 0) {
                sErrorList += "<li>Denominations List cannot be null</li>";
            }

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
                if (CheckForDelete('this Sanchaya Patra Type Setup')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sanchaya Patra has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function LoadValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtSpTypeId.ClientID %>', 'TextBox', "SP Type cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

    </script>

    <style type="text/css">
        /* new auto complete*/.ac_results .ac_results
        {
            font-size: 15px;
            border-right: black 1px solid;
            padding-right: 0px;
            border-top: black 1px solid;
            padding-left: 0px;
            z-index: 99999;
            padding-bottom: 0px;
            overflow: hidden;
            border-left: black 1px solid;
            padding-top: 0px;
            border-bottom: black 1px solid;
            background-color: white;
        }
        .ac_results UL
        {
            padding-right: 0px;
            padding-left: 0px;
            list-style-position: outside;
            padding-bottom: 0px;
            margin: 0px;
            width: 100%;
            padding-top: 0px;
            list-style-type: none;
            font-size: 15px;
        }
        .ac_results LI
        {
            padding-right: 5px;
            display: block;
            padding-left: 5px;
            font-size: 15px;
            padding-bottom: 2px;
            margin: 0px;
            font: menu;
            overflow: hidden;
            cursor: default;
            line-height: 16px;
            padding-top: 2px;
        }
        .ac_loading
        {
            background: url(indicator.gif) white no-repeat right center;
            font-size: 15px;
        }
        .ac_odd
        {
            background-color: #EAFBE1;
            font-size: 15px;
        }
        .ac_over
        {
            color: white;
            background-color: #3192C4;
            font-size: 15px;
        }
        /* end of auto complete*/</style>
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
        <legend>Unapproved SP Details List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdDenomID" />
                <asp:HiddenField runat="server" ID="hdSPTypeID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" OnRowDataBound="gvList_RowDataBound"
                                ID="gvList" runat="server" SkinID="SBMLGridGreen" ShowHeader="true" AutoGenerateColumns="false"
                                AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="bfSpType" HeaderText="Sp Type" />
                                    <asp:BoundField DataField="bfTypeDesc" HeaderText="Type Description" />
                                    <asp:BoundField DataField="bfCurrencyCode" HeaderText="Currency Code" />
                                    <asp:BoundField DataField="MakeDate" HeaderText="Make Date" DataFormatString="{0:dd-MMM-yyyy}" />
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
        <legend>SP Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            SP Type
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtSpTypeId" MaxLength="3" Width="100px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return SearchSPTypePopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td align="right" width="20%">
                            Description
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtDescription" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Currency
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCurrency" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
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
    </fieldset>
    <br />
    <fieldset runat="server" id="fsDenom">
        <legend>SP Denominations List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvDenom_RowCommand" ID="gvDenom" runat="server"
                                AutoGenerateColumns="false" SkinID="SBMLGridGreen" ShowHeader="true" OnRowDeleting="gvDenom_RowDeleting">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnDelete"
                                                Text="Delete" OnClientClick="return CheckForDelete('this SP Type')" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:BoundField DataField="bfDenomSPType" HeaderText="SP Type" />
                                    <asp:BoundField DataField="bfDenomination" HeaderText="Denomination" />
                                    <asp:BoundField DataField="bfSPSeries" HeaderText="SP Series" />
                                    <asp:BoundField DataField="bfDigitsInSlNo" HeaderText="Digits In SlNo" />
                                    <asp:BoundField DataField="bfReOrderLevel" HeaderText="Re-Order Level" />
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSaveDenom" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>SP Denominations</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td width="20%" align="right">
                            Denomination
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtDenomination" MaxLength="7" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td width="20%" align="right">
                            Re-Order Level
                        </td>
                        <td width="30%">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtReOrderLevel" MaxLength="4" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Series
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtSeries" MaxLength="12" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            No. Of Digits In Series
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNoOfDigitInSeries" MaxLength="2" CssClass="textInput" runat="server"
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
        <div align="right" style="padding-top: 6px">
            <asp:Button CssClass="ButtonAsh" runat="server" Text="Denominations Add" ID="btnSaveDenom"
                OnClick="btnSaveDenom_Click" OnClientClick="return SaveDenomValidation()" />
            &nbsp;
            <asp:Button CssClass="ButtonAsh" runat="server" Text="Denominations Reset" ID="btnDenomReset"
                OnClientClick="return DenomReset()" />
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
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClientClick=" MsgPopupReturnTrue('Approve') "
                        OnClick="btnApprove_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:SearchSPType ID="ucSearchSPType" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
