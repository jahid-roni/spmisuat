<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="BankSetup" CodeBehind="BankSetup.aspx.cs" %>

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
        <legend>Unapproved Bank List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdBankID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView OnRowCommand="gvBankList_RowCommand" AutoGenerateColumns="false" Width="98%"
                                ID="gvBankList" runat="server" SkinID="SBMLGridGreen" AllowPaging="True" OnPageIndexChanging="gvBankList_PageIndexChanging" >
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" OnClientClick="CloseErrorPanel()" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="BankID" HeaderText="Bank ID" />
                                    <asp:BoundField DataField="CompanyName" HeaderText="Bank Name" />
                                    <asp:BoundField DataField="BBCode" HeaderText="BB Code" />
                                    <asp:BoundField DataField="Address" HeaderText="Address" />
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
    <asp:UpdatePanel runat="server" ID="upData">
        <ContentTemplate>
            <fieldset>
                <legend>Bank Details</legend>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Bank ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="30px" MaxLength="3" ID="txtBankID" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommPopupReturnTrue(),CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td align="right">
                            Bangladesh Bank Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="150px" MaxLength="11" ID="txtBdBankCode" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Bank Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="150px" ID="txtDivisionName" MaxLength="35" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlBranchCode" Width="155px" SkinID="ddlCommon" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Country Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList Width="155px"  ID="ddlCountryName" SkinID="ddlCommon" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top" rowspan="2">
                            Address
                        </td>
                        <td valign="top" rowspan="2">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAddress" MaxLength="150" Width="150px" CssClass="textArea" TextMode="MultiLine"
                                    Height="50px" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            SWIFT BIC
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="150px" MaxLength="11" ID="txtSWIFTBIC" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                        </td>
                        <td align="right">
                            Phone Number
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="150px" MaxLength="50" ID="txtPhoneNumber" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Zip Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="150px" MaxLength="20" ID="txtZipCode" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                        </td>
                        <td valign="top" align="right">
                            Fax Number
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtFaxNumber" MaxLength="20" Width="150px" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"> 
                                </asp:TextBox></div>
                        </td>
                        <td align="right">
                            Email ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtEmailID" MaxLength="50" Width="150px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"> 
                                </asp:TextBox></div>
                        </td>
                    </tr>
                </table>
            </fieldset>
            <br />
            <fieldset>
                <legend>Fiscal Year</legend>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            From Month
                        </td>
                        <td width="30%">
                            <asp:DropDownList ID="ddlFromMonth" SkinID="ddlSmall" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right" width="20%">
                            To Month
                        </td>
                        <td width="30%">
                            <asp:DropDownList ID="ddlToMonth" SkinID="ddlSmall" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLoad" />
        </Triggers>
    </asp:UpdatePanel>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfComm ID="ucSearchConfComm" runat="server" PageCaption="Bank Search" Caption_1="Bank ID"
        Caption_2="Bank Name" Type="BankSearch" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtPhoneNumber]').keypress(function(e) { return textPhoneFax(e); });
            $('input[id*=txtFaxNumber]').keypress(function(e) { return textPhoneFax(e); });
            $('input[id*=txtZipCode]').keypress(function(e) { return intNumber(e); });
        });


        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtBankID.ClientID %>', 'TextBox', "Bank ID cannot be empty!");
            sErrorList += RequiredData('<%=txtBdBankCode.ClientID %>', 'TextBox', "Bangladesh Bank Code cannot be empty!");
            sErrorList += RequiredData('<%=txtDivisionName.ClientID %>', 'TextBox', "Area Name cannot be empty!");
            sErrorList += RequiredData('<%=ddlCountryName.ClientID %>', 'DropDownList', "Country Name cannot be empty!");
            sErrorList += RequiredData('<%=ddlBranchCode.ClientID %>', 'DropDownList', "Branch Code cannot be empty!");
            sErrorList += RequiredData('<%=txtAddress.ClientID %>', 'TextBox', "Address cannot be empty!");
           
            return OpenErrorPanel(sErrorList , 'Save');
        }

        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtBankID.ClientID %>', 'TextBox', "Bank ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function Reset() {
            ResetData('<%=txtBankID.ClientID %>');
            var obj = document.getElementById('<%=txtBankID.ClientID %>');
            if (obj != null) {
                obj.readOnly = false;
            }
            ResetData('<%=txtBdBankCode.ClientID %>');
            ResetData('<%=txtDivisionName.ClientID %>');
            ResetData('<%=ddlCountryName.ClientID %>');
            ResetData('<%=ddlFromMonth.ClientID %>');
            ResetData('<%=ddlToMonth.ClientID %>');
            ResetData('<%=txtZipCode.ClientID %>');
            ResetData('<%=txtSWIFTBIC.ClientID %>');
            ResetData('<%=txtPhoneNumber.ClientID %>');
            ResetData('<%=ddlBranchCode.ClientID %>');
            ResetData('<%=txtEmailID.ClientID %>');
            ResetData('<%=txtFaxNumber.ClientID %>');
            ResetData('<%=txtAddress.ClientID %>');
            ResetData('<%=hdBankID.ClientID %>');

            var objCountry = document.getElementById('<%=ddlCountryName.ClientID %>');
            if (objCountry.options.length > 0) {
                objCountry.options[1].selected = "selected";
            }            
            ResetUserDetails();
            CloseErrorPanel();

            return false;
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdBankID = document.getElementById('<%=hdBankID.ClientID %>');
            if (hdBankID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this division')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Area has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
    </script>

</asp:Content>
