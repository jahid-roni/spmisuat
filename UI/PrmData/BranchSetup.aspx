<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="True"
    Inherits="BranchSetup" CodeBehind="BranchSetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchConfComm.ascx" TagName="ConfComm" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtPhoneNumber]').keypress(function(e) { return textPhoneFax(e); });
            $('input[id*=txtFaxNumber]').keypress(function(e) { return textPhoneFax(e); });
            $('input[id*=txtZipCode]').keypress(function(e) { return intNumber(e); });
        });

        function Reset() {
            ResetData('<%=txtBranchID.ClientID %>');
            var obj = document.getElementById('<%=txtBranchID.ClientID %>');
            if (obj != null) {
                obj.readOnly = false;
            }
            ResetData('<%=txtBranchName.ClientID %>');
            ResetData('<%=txtBdBankCode.ClientID %>');
            ResetData('<%=txtZipCode.ClientID %>');
            ResetData('<%=txtPhoneNumber.ClientID %>');
            ResetData('<%=txtEmailID.ClientID %>');
            ResetData('<%=txtFaxNumber.ClientID %>');
            ResetData('<%=txtAddress.ClientID %>');
            ResetData('<%=hdBranchID.ClientID %>');

            var objCountry = document.getElementById('<%=ddlCountryName.ClientID %>');
            if (objCountry.options.length > 0) {
                objCountry.options[1].selected = "selected";
            }

            ResetUserDetails();
            CloseErrorPanel();
            return false;
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function LoadValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtBranchID.ClientID %>', 'TextBox', "Branch ID cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
        }

        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtBranchID.ClientID %>', 'TextBox', "Branch ID cannot be empty!");
            sErrorList += RequiredData('<%=txtBranchName.ClientID %>', 'TextBox', "Branch Name cannot be empty!");
            sErrorList += RequiredData('<%=ddlCountryName.ClientID %>', 'DropDownList', "Country cannot be empty!");


            return OpenErrorPanel(sErrorList, 'Save');
        }

        function DeleteBrach() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdBranchID = document.getElementById('<%=hdBranchID.ClientID %>');
            if (hdBranchID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Branch Setup')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {

                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Branch has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
        
    </script>

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
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Branch List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdBranchID" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView OnRowCommand="gvBranchList_RowCommand" AutoGenerateColumns="false"
                                Width="98%" ID="gvBranchList" runat="server" SkinID="SBMLGridGreen" AllowPaging="True"
                                OnPageIndexChanging="gvBranchList_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" OnClientClick="CloseErrorPanel()" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="BranchID" HeaderText="Branch ID" />
                                    <asp:BoundField DataField="BranchName" HeaderText="Branch Name" />
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
    <fieldset>
        <legend>Branch Details</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Branch ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBranchID" MaxLength="3" Width="45px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button OnClientClick="return LoadValidation()" CssClass="ButtonAsh" runat="server"
                                Text="Load" ID="btnLoad" OnClick="btnLoad_Click" />
                            <asp:Button OnClientClick="return ConfCommPopupReturnTrue(), CloseErrorPanel()" CssClass="ButtonAsh"
                                runat="server" Text="Search" ID="btnSearch" />
                        </td>
                        <td align="right">
                            Branch Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBranchName" Width="150px" MaxLength="35" CssClass="textInput"
                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                                </asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Bangladesh Bank Code
                        </td>
                        <td>
                            <asp:TextBox ID="txtBdBankCode" MaxLength="6" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Zip Code
                        </td>
                        <td>
                            <asp:TextBox ID="txtZipCode" MaxLength="20" Width="150px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                        <td align="right">
                            Country
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList Width="154px" ID="ddlCountryName" SkinID="ddlCommon" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Phone
                        </td>
                        <td>
                            <asp:TextBox ID="txtPhoneNumber" MaxLength="50" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Email ID
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtEmailID" MaxLength="50" Width="150px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Fax Number
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtFaxNumber" MaxLength="20" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Address
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddress" Width="200px" MaxLength="150" CssClass="textInput" TextMode="MultiLine"
                                Height="50px" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click"
                        OnClientClick="return DeleteBrach()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:ConfComm ID="ucSearchConfComm" runat="server" PageCaption="Branch Search" Caption_1="Branch ID"
        Caption_2="Branch Name" Type="BranchSearch" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
