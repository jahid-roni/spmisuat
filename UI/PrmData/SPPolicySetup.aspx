<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    Inherits="SPPolicySetup" CodeBehind="SPPolicySetup.aspx.cs" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchPolicy.ascx" TagName="SearchPolicy" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

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
        <legend>Unapproved Policy List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdPolicyID" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <asp:HiddenField runat="server" ID="hdPolEffDate" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView Width="98%" OnRowCommand="gvList_RowCommand" ID="gvList" runat="server"
                                AutoGenerateColumns="true" OnRowDataBound="gvList_RowDataBound" SkinID="SBMLGridGreen"
                                ShowHeader="true" AllowPaging="True" OnPageIndexChanging="gvList_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" OnClientClick="CloseErrorPanel() " />
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
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnAddNewSPPolicy" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Sanchaya Patra Policy Details </legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel8">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSPType" SkinID="ddlLarge" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Effective Date
                        </td>
                        <td width='18%'>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtEffectiveDate" Width="120px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td width='1%'>
                            <asp:Button OnClientClick="return SearchPolicyPopupReturnTrue(),CloseErrorPanel()"
                                CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="tabber">
            <%-- General Tab  --%>
            <div class="tabbertab">
                <h2>
                    General</h2>
                <fieldset>
                    <legend>General Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                        <ContentTemplate>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="0"
                                border="0">
                                <tr>
                                    <td width="50%" valign="top">
                                        <fieldset>
                                            <legend>SP Duration</legend>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td>
                                                        <asp:RadioButtonList ID="rblIsSPDurationInMonth" runat="server" RepeatDirection="Horizontal"
                                                            AutoPostBack="true" OnSelectedIndexChanged="rblIsSPDurationInMonth_SelectedIndexChanged">
                                                            <asp:ListItem Text="Month" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Year" Value="0"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td align="right">
                                                        Duration
                                                    </td>
                                                    <td>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox CssClass="textInput" Width="60px" runat="Server" ID="txtDuration" onblur="return IsSPTypeValidated(),blurActiveInputWithObj(this)"
                                                                onfocus="highlightActiveInputWithObj(this)" OnTextChanged="txtDuration_TextChanged"
                                                                AutoPostBack="true"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="right">
                                                        No of Coupon \ Installments
                                                    </td>
                                                    <td>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox CssClass="textInput" Width="60px" runat="server" ID="txtNoOfCoupon"
                                                                AutoPostBack="true" OnTextChanged="txtNoOfCoupon_TextChanged" onblur="return IsSPTypeValidated(),blurActiveInputWithObj(this)"
                                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                        <br />
                                        <fieldset>
                                            <legend>Interest Type</legend>
                                            <fieldset>
                                                <legend>Interest Type</legend>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="left" width="80%">
                                                            Interest Type
                                                        </td>
                                                        <td align="right" width="20%">
                                                            <asp:DropDownList runat="Server" ID="ddlIntrType" SkinID="ddlSmall">
                                                                <asp:ListItem Text="" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Simple" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Compound " Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            Simple After Interest Claim?
                                                        </td>
                                                        <td align="right">
                                                            <fieldset>
                                                                <asp:RadioButtonList ID="rblInterestTypeAfterIntPayment" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </fieldset>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                            <br />
                                            <fieldset>
                                                <legend>Pre- Maturity </legend>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="left" width="80%">
                                                            Interest Type
                                                        </td>
                                                        <td align="right" width="20%">
                                                            <asp:DropDownList runat="Server" ID="ddlPreMatIntrType" SkinID="ddlSmall">
                                                                <asp:ListItem Text="" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Simple" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Compound " Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            Simple After Interest Claim?
                                                        </td>
                                                        <td align="right">
                                                            <fieldset>
                                                                <asp:RadioButtonList ID="rblGSPreMatIntrClaim" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </fieldset>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </fieldset>
                                    </td>
                                    <td width="50%" valign="top">
                                        <fieldset>
                                            <legend>Different Requirements</legend>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td align="left" width="70%">
                                                        Is Bond Holder Required?
                                                    </td>
                                                    <td align="right" width="30%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblIsBondHolderRequired" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Is Nominee Per Scrip Required?
                                                    </td>
                                                    <td align="right">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblIsNomineePerScripRequired" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Is Foreign Address Required?
                                                    </td>
                                                    <td align="right">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblIsFoeignAddressRequired" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                        <br />
                                        <fieldset>
                                            <legend>Reinvestment & Partial Encashment</legend>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td align="left" width="70%">
                                                        Bond can be Reinvested ?
                                                    </td>
                                                    <td align="right" width="30%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblReinvestmentSuported" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Interest can be Reinvested?
                                                    </td>
                                                    <td align="right">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblInterestReinvestable" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Maximum number of reinvestment
                                                    </td>
                                                    <td align="right">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox CssClass="textInput" Width="80px" runat="Server" ID="txtReinNumber"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Encashment can be Partial
                                                    </td>
                                                    <td align="right">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblPartiallyEncashable" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Partially Encashed Bond can be Reinvested
                                                    </td>
                                                    <td align="right">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblPartiallyEncashedReinvestable" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of General Tab  --%>
            <%-- Currency Tab start --%>
            <div class="tabbertab">
                <h2>
                    Currency</h2>
                <fieldset>
                    <legend>Currency Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                        <ContentTemplate>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td width="25%" align="right">
                                        Activity Type
                                    </td>
                                    <td width="25%">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlCurrencyActiveType" runat="server" SkinID="ddlMedium">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td width="25%" align="right">
                                        Currency
                                    </td>
                                    <td width="25%">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlCurrency" runat="server" SkinID="ddlMedium">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                            </table>
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <div class="PageCaptionRight">
                                            Activity Currency</div>
                                    </td>
                                    <td>
                                        <div align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnAddNewActiCurrency_Click"
                                                Text="Add New" OnClientClick="return SaveCurrencyValidation()" ID="btnAddNewActiCurrency" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView ID="gvActiCurrency" runat="server" AutoGenerateColumns="false" SkinID="SBMLGridGreen"
                                    Width="98%" OnRowCommand="gvActiCurrency_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="bfActivityType" HeaderText="Activity Type" />
                                        <asp:BoundField DataField="bfCurrency" HeaderText="Currency" />
                                        <asp:TemplateField HeaderText="Sl.No" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemoveActiCurrency" Text="Remove" runat="server" CssClass="ButtonAsh"
                                                    CommandName="Remove" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdCurrencyID")%>' ID="hdCurrencyID" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdCustActiID")%>' ID="hdCustActiID" />
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
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of Currency Tab --%>
            <%-- Early Encashment Tab start --%>
            <div class="tabbertab">
                <h2>
                    Early Encashment</h2>
                <fieldset>
                    <legend>Early Encashment Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                        <ContentTemplate>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td align="right" valign="top">
                                        Coupon \ Installments
                                    </td>
                                    <td valign="top">
                                        <div class="fieldLeft">
                                            <asp:TextBox runat="server" Width="70px" CssClass="textInput" ID="txtEarlyEncashCouponNo"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td align="right" valign="top">
                                        Common Int. Rate
                                    </td>
                                    <td valign="top">
                                        <div class="fieldLeft">
                                            <asp:TextBox runat="server" Width="70px" CssClass="textInput" ID="txtCommonIntRate"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td valign="top" width="35%">
                                        <asp:CheckBox runat="server" Text="All Matured Coupon \ Installments" ID="chkMaturedCoupon" />
                                    </td>
                                </tr>
                            </table>
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <div class="PageCaptionRight">
                                            Encashment Interest Rate</div>
                                    </td>
                                    <td>
                                        <div align="right">
                                            <asp:Button OnClientClick="return SaveEncashmentInterestValidation()" CssClass="ButtonAsh"
                                                runat="server" OnClick="btnSaveEarlyEncashment_Click" Text="Save" ID="btnSaveEarlyEncashment" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="RowCommand" ControlID="gvEncashmentIntRate" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel10">
                        <ContentTemplate>
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView ID="gvEncashmentIntRate" runat="server" AutoGenerateColumns="false"
                                    SkinID="SBMLGridGreen" Width="98%">
                                    <Columns>
                                        <asp:BoundField DataField="bfCouponInstallmentNo" HeaderText="Coupon \ Installment No" />
                                        <asp:BoundField DataField="bfMonthFrom" HeaderText="Month From" />
                                        <asp:BoundField DataField="bfMonthTo" HeaderText="Month To" />
                                        <asp:TemplateField HeaderText="Interest Rate">
                                            <ItemTemplate>
                                                <asp:TextBox ID="bfInterestRate" Text='<%# Eval("bfInterestRate")%>' runat="server"
                                                    Width="80px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="No Of Slabs Int Payable">
                                            <ItemTemplate>
                                                <asp:TextBox ID="bfNoOfSlabsIntPayable" Text='<%# Eval("bfNoOfSlabsIntPayable")%>'
                                                    runat="server" Width="80px" CssClass="textInput" onblur="blurActiveInputWithObj(this), EarlyCouponIntPayableOnChange(this)"
                                                    onfocus="highlightActiveInputWithObj(this), EarlyCouponIntPayableOnClick(this)"></asp:TextBox>
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdPolicyID")%>' ID="hdEliPayModeID" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdSlabNoID")%>' ID="hdEliCustActiID" />
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
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSaveEarlyEncashment" />
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of Early Encashment Tab --%>
            <%-- General Interest Tab start --%>
            <div class="tabbertab">
                <h2>
                    General Interest</h2>
                <fieldset>
                    <legend>General Interest Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel5">
                        <ContentTemplate>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td width="18%" align="right">
                                        Coupon \ Installments
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtGeneralIntCouponNo" Width="70px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td align="right">
                                        Claim Rate
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtGIClaimRate" Width="60px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td align="right">
                                        Nonclaim Int. Rate
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtNonclaimIntRate" Width="60px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                            </table>
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <div class="PageCaptionRight">
                                            Interest Rate</div>
                                    </td>
                                    <td>
                                        <div align="right">
                                            <asp:Button OnClientClick="return SaveGeneralValidation()" OnClick="btnGeneralSaveInterest_Click"
                                                CssClass="ButtonAsh" runat="server" Text="Save" ID="btnGeneralSaveInterest" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView Width="98%" ID="gvGeneralInt" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true">
                                    <Columns>
                                        <asp:BoundField DataField="bfCouponInstallmentNo" HeaderText="Coupon \ Installment No" />
                                        <asp:BoundField DataField="bfMonthFrom" HeaderText="Month From" />
                                        <asp:BoundField DataField="bfMonthTo" HeaderText="Month To" />
                                        <asp:TemplateField HeaderText="Claim Rate." HeaderStyle-Width="15%">
                                            <ItemTemplate>
                                                <asp:TextBox ID="bfClaimRate" runat="server" Text='<%# Eval("bfClaimRate")%>' Width="80px"
                                                    CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nonclaim Int. Rate">
                                            <ItemTemplate>
                                                <asp:TextBox ID="bfNonclaimIntRate" runat="server" Text='<%# Eval("bfNonclaimIntRate")%>'
                                                    Width="80px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdPolicyID")%>' ID="hdEliPayModeID" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdSlabNoID")%>' ID="hdEliCustActiID" />
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
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of General Interest Tab --%>
            <%-- Commission Tax Tab start --%>
            <div class="tabbertab">
                <h2>
                    Commission Tax</h2>
                <fieldset>
                    <legend>Commission Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel6">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                                border="0">
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Non-org Commission
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetNonOrgComm"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Charge on per
                                                    </td>
                                                    <td width="18%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetNonOrgChargeOnPer" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Script" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Reg." Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Calculate Int
                                                    </td>
                                                    <td width="27%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetNonOrgCalculateInt" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Org. Commission
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetOrgCommission"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Charge on per
                                                    </td>
                                                    <td width="18%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetOrgChargeOnPer" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Script" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Reg." Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Calculate Int
                                                    </td>
                                                    <td width="27%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetOrgCalculateInt" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Int. Remuneration
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetIntRemuneration"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Charge on per
                                                    </td>
                                                    <td width="18%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetIntRemuChargeOnPer" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Script" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Reg." Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Calculate Int
                                                    </td>
                                                    <td width="27%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetIntRemuCalculateInt" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Remuneration
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetRemuneration"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Charge on per
                                                    </td>
                                                    <td width="18%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetRemuChargeOnPer" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Script" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Reg." Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Calculate Int
                                                    </td>
                                                    <td width="27%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetRemuCalculateInt" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Levi
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox CssClass="textInput" Width="60px" runat="server" ID="txtComSetLevi"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td width="33%" align="right">
                                                    </td>
                                                    <td width="15%" align="right">
                                                        Calculate Int
                                                    </td>
                                                    <td width="27%">
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetLevi" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <fieldset>
                                            <table width="100%">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Income Tax
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetIncomeTax"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td>
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblComSetIncomeTax" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td align="right">
                                                        Above
                                                    </td>
                                                    <td>
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="70px" CssClass="textInput" runat="server" ID="txtComSetIncomeTaxAbove"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td align="right">
                                                        Amount
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox runat="server" ID="chkYearly" Text="Yearly" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <fieldset>
                                            <table width="100%">
                                                <tr>
                                                    <td width="15%" align="right">
                                                        Social Security Amount
                                                    </td>
                                                    <td width="10%" align="left">
                                                        <div class="fieldLeft">
                                                            <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtSocialSecurityAmount"
                                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                        </div>
                                                        <div class="errorIcon">
                                                            *</div>
                                                    </td>
                                                    <td>
                                                        <fieldset>
                                                            <asp:RadioButtonList ID="rblSocialSecurityAmount" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Percentage(%)" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Amount" Value="1"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </fieldset>
                                                    </td>
                                                    <td width="40%">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of Commission Tax Tab --%>
            <%-- Eligibility Tab start --%>
            <div class="tabbertab">
                <h2>
                    Eligibility</h2>
                <fieldset>
                    <legend>Eligibility Setup</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel9">
                        <ContentTemplate>
                            <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td align="right" width="25%">
                                        Customer Activity Type
                                    </td>
                                    <td width="25%">
                                        <div class="fieldLeft">
                                            <asp:GridView ID="gvCustomerType" runat="server" AutoGenerateColumns="true" 
                                                ShowHeader="false" SkinID="SBMLGridGreen" Style="width: 94%">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemStyle Width="15px" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkCusomerType" runat="server" />
                                                            <asp:HiddenField ID="hdCustomerTypeID" runat="server" 
                                                                Value='<%# Eval("Value")%>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:DropDownList ID="ddlEligibilityCustomerActivityType" runat="server" SkinID="ddlMedium">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td align="right" width="25%">
                                        Payment Mode
                                    </td>
                                    <td width="25%">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlPaymentMode" runat="server" SkinID="ddlMedium">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <div class="PageCaptionRight">
                                            Payment Policy</div>
                                    </td>
                                    <td>
                                        <div align="right">
                                            <asp:Button OnClientClick="return SavePaymentPolicyValidation()" CssClass="ButtonAsh"
                                                runat="server" Text="Add New" OnClick="btnAddNewEligibility_Click" ID="btnAddNewEligibility" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView Width="98%" ID="gvEliPaymentPolicy" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvEliPaymentPolicy_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="bfCustActiType" HeaderText="Customer Activity Type" />
                                        <asp:BoundField DataField="bfPayModeType" HeaderText="Payment Mode" />
                                        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemoveEligibility" Text="Remove" runat="server" CssClass="ButtonAsh"
                                                    CommandName="Remove" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdEliPayModeID")%>' ID="hdEliPayModeID" />
                                                <asp:HiddenField runat="server" Value='<%# Eval("hdEliCustActiID")%>' ID="hdEliCustActiID" />
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
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
                <br />
                <fieldset>
                    <legend>Eligibility Detail</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel7">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td colspan="2">
                                        &nbsp;
                                    </td>
                                    <td valign="top">
                                        Application Customer Type
                                    </td>
                                    <td valign="top">
                                        Applicable Sex
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="right">
                                        Minimum age
                                    </td>
                                    <td valign="top">
                                        <div class="fieldLeft">
                                            <asp:TextBox runat="server" Width="80px" CssClass="textInput" ID="txtMinimumAge"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                    <td rowspan="2" valign="top">
                                        <div style="height: 80px; width: 280px; overflow: auto;">
                                        </div>
                                    </td>
                                    <td rowspan="2" valign="top">
                                        <div class="fieldLeft">
                                            <asp:DropDownList runat="server" ID="ddlApplicableSex" SkinID="ddlSmall">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="right">
                                        Maximum age
                                    </td>
                                    <td valign="top">
                                        <div class="fieldLeft">
                                            <asp:TextBox Width="80px" CssClass="textInput" runat="server" ID="txtMaximumAge"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </fieldset>
            </div>
            <%-- End of Eligibility Tab --%>
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Add New Policy" ID="btnAddNewSPPolicy" OnClientClick="return SaveValidation()"
                        OnClick="btnAddNewSPPolicy_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:SearchPolicy ID="ucSearchPolicy" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });

            $('input[id*=txtDuration]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtNoOfCoupon]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtReinNumber]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetNonOrgComm]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetOrgCommission]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetIntRemuneration]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetRemuneration]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetLevi]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetIncomeTax]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtComSetIncomeTaxAbove]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtMinimumAge]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtMaximumAge]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtSocialSecurityAmount]').keypress(function(e) { return floatNumber(e); });
            // });
        }

        // for checking coupon payable number..
        var oldValuePaybleNumber = "0";
        function EarlyCouponIntPayableOnClick(obj) {
            oldValuePaybleNumber = obj.value;
        }
        function EarlyCouponIntPayableOnChange(obj) {
            var TargetBaseControl = document.getElementById('<%= this.gvEncashmentIntRate.ClientID %>');
            var Inputs = TargetBaseControl.getElementsByTagName("input");
            for (var n = 0; n < Inputs.length; ++n) {
                if (Inputs[n].id == obj.id) {
                    var CoupNo = Inputs[n].parentNode.parentNode.childNodes[0].nextSibling.innerHTML;
                    if (CoupNo < Inputs[n].value) {
                        alert("You cannot change, please change your value..");
                        Inputs[n].value = oldValuePaybleNumber;
                    }
                    break;
                }
            }
        }
        // end of ... for checking coupon payable number.. 


        function SaveValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=txtEffectiveDate.ClientID %>', 'TextBox', "Effective Date cannot be empty!");

            // General Setup
            sErrorList += RequiredData('<%=txtDuration.ClientID %>', 'TextBox', "Duration cannot be empty!");
            sErrorList += RequiredData('<%=txtNoOfCoupon.ClientID %>', 'TextBox', "No of Coupon\Installments cannot be empty!");
            sErrorList += RequiredData('<%=txtReinNumber.ClientID %>', 'TextBox', "Maximum number of reinvestment cannot be empty!");

            //Commission Setup
            sErrorList += RequiredData('<%=txtComSetNonOrgComm.ClientID %>', 'TextBox', "Non-org Commission cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetOrgCommission.ClientID %>', 'TextBox', "Org. Commission cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetIntRemuneration.ClientID %>', 'TextBox', "Int. Remuneration cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetRemuneration.ClientID %>', 'TextBox', "Remuneration cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetLevi.ClientID %>', 'TextBox', "Levi cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetIncomeTax.ClientID %>', 'TextBox', "Income Tax cannot be empty!");
            sErrorList += RequiredData('<%=txtComSetIncomeTaxAbove.ClientID %>', 'TextBox', "Above cannot be empty!");

            //Eligibility Setup
            sErrorList += RequiredData('<%=txtMinimumAge.ClientID %>', 'TextBox', "Minimum age cannot be empty!");
            sErrorList += RequiredData('<%=txtMaximumAge.ClientID %>', 'TextBox', "Maximum age cannot be empty!");
            sErrorList += RequiredData('<%=ddlApplicableSex.ClientID %>', 'DropDownList', "Applicable Sex cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function SaveEncashmentInterestValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtCommonIntRate.ClientID %>', 'TextBox', "Common Int. Rate cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }


        function SaveGeneralValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtGIClaimRate.ClientID %>', 'TextBox', "Claim Rate cannot be empty!");
            sErrorList += RequiredData('<%=txtNonclaimIntRate.ClientID %>', 'TextBox', "Nonclaim Int. Rate cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function SaveCurrencyValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlCurrencyActiveType.ClientID %>', 'DropDownList', "Activity Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlCurrency.ClientID %>', 'DropDownList', "Currency cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function SavePaymentPolicyValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlEligibilityCustomerActivityType.ClientID %>', 'DropDownList', "Customer Activity Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlPaymentMode.ClientID %>', 'DropDownList', "Payment Mode cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Reject');
        }


        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdPolicyID = document.getElementById('<%=hdPolicyID.ClientID %>');
            if (hdPolicyID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Sanchaya Patra Policy')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Sanchaya Patra Policy has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function IsSPTypeValidated() {

            var spTypeID = $("#<%=ddlSPType.ClientID %> option:selected").val();
            if (spTypeID == "") {
                MsgPopupReturnTrue('Save');
                return false;
            }
            else {
                return true;
            }
        }
        
    </script>

</asp:Content>
