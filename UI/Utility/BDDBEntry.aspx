<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="BDDBEntry.aspx.cs" EnableEventValidation="false" Inherits="SBM_WebUI.mp.BDDBEntry" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchSPReceive.ascx" TagName="SPReceiveSearch" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCViewGridData.ascx" TagName="VGData" TagPrefix="uc4" %>
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
    <br />
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="hdDataType" />
            <fieldset>
                <legend>Account Details</legend>
                <table width="100%" align="center" cellpadding="3" cellspacing="4" border="0">
                    <tr>
                        <td align="right">
                            Full Account No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" MaxLength="12" ID="txtAccNo" CssClass="textInput" Width="138px"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" AutoPostBack="True" OnTextChanged="txtAccNo_TextChanged"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            A/C Holder Name
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtAccHolderName" CssClass="textInput" Width="220px"
                                    MaxLength="77" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Master No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtMasterNo" runat="server" CssClass="textInput" MaxLength="9" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="138px"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtBranchCode" runat="server" CssClass="textInput" MaxLength="2"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    Width="42px"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Currency Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList SkinID="ddlMedium" ID="ddlPDCurrencyCode" Width="180px" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Address 1
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtAddress1" MaxLength="35" CssClass="textInput"
                                    Width="220px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Address 2
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtAddress2" MaxLength="35" CssClass="textInput"
                                    Width="220px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Address 3
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtAddress3" MaxLength="35" CssClass="textInput"
                                    Width="220px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            Address 4
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtAddress4" MaxLength="35" CssClass="textInput"
                                    Width="220px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
        </Triggers>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClick="btnSave_Click"
                        OnClientClick="return SaveValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="CloseErrorPanel()"
                        OnClick="btnReset_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
    <uc4:VGData ID="VGD" runat="server" />

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {

            $('input[id*=txtAccNo]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtMasterNo]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtBranchCode]').keypress(function(e) { return intNumber(e); });
        }

        function SaveValidation() {
            window.scroll(0, 0);
            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }

            var sErrorList = "";

            sErrorList += RequiredData('<%=txtAccNo.ClientID %>', 'TextBox', "Full Account No. cannot be empty!");
            sErrorList += RequiredData('<%=txtMasterNo.ClientID %>', 'TextBox', "Master No. cannot be empty!");
            sErrorList += RequiredData('<%=txtAccHolderName.ClientID %>', 'TextBox', "Account Holder Name cannot be empty!");
            sErrorList += RequiredData('<%=txtAddress1.ClientID %>', 'TextBox', "Address 1 cannot be empty!");
            sErrorList += RequiredData('<%=txtBranchCode.ClientID %>', 'TextBox', "Branch Code cannot be empty!");
            sErrorList += RequiredData('<%=ddlPDCurrencyCode.ClientID %>', 'DropDownList', "Please select Currency Code!");

            return OpenErrorPanel(sErrorList, 'Save');

        }        
    
    </script>

</asp:Content>
