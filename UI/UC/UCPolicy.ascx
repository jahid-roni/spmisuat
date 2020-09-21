<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPolicy.ascx.cs" Inherits="SBM_WebUI.UI.UC.UCPolicy" %>
<div id="MDPolicy" class="MDClass" runat="server">
    <table width="900" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Policy Detail</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_PD_MDPolicy');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 450px; overflow: auto;">
                    <%--<fieldset>
                        <legend>Sanchaya Patra Policy Details </legend>--%>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel8">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                                border="0">
                                <tr>
                                    <td align="right">
                                        SP Type
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSPType" SkinID="ddlLarge" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right">
                                        Effective Date
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEffectiveDate" Width="120px" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
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
                                                                        AutoPostBack="true">
                                                                        <asp:ListItem Text="Month" Value="1"></asp:ListItem>
                                                                        <asp:ListItem Text="Year" Value="0"></asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                                <td align="right">
                                                                    Duration
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textInput" Width="60px" runat="Server" ID="txtDuration" onblur="blurActiveInputWithObj(this)"
                                                                        onfocus="highlightActiveInputWithObj(this)" AutoPostBack="true"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" align="right">
                                                                    No of Coupon \ Installments
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textInput" Width="60px" runat="server" ID="txtNoOfCoupon"
                                                                        AutoPostBack="true" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox CssClass="textInput" Width="80px" runat="Server" ID="txtReinNumber"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                        <table width="100%" border="0">
                                            <tr>
                                                <td>
                                                    <div class="PageCaptionRight">
                                                        Activity Currency</div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="height: 200px; width: 100%; overflow: auto;">
                                            <asp:GridView ID="gvActiCurrency" runat="server" AutoGenerateColumns="false" SkinID="SBMLGridGreen"
                                                Width="98%">
                                                <Columns>
                                                    <asp:BoundField DataField="bfActivityType" HeaderText="Activity Type" />
                                                    <asp:BoundField DataField="bfCurrency" HeaderText="Currency" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
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
                                                    <asp:TextBox runat="server" Width="70px" CssClass="textInput" ID="txtEarlyEncashCouponNo"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                                <td align="right" valign="top">
                                                    Common Int. Rate
                                                </td>
                                                <td valign="top">
                                                    <asp:TextBox runat="server" Width="70px" CssClass="textInput" ID="txtCommonIntRate"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                            </tr>
                                        </table>
                                    </ContentTemplate>
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
                                                    <asp:TextBox ID="txtGeneralIntCouponNo" Width="70px" CssClass="textInput" runat="server"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                                <td align="right">
                                                    Claim Rate
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtGIClaimRate" Width="60px" CssClass="textInput" runat="server"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                                <td align="right">
                                                    Nonclaim Int. Rate
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtNonclaimIntRate" Width="60px" CssClass="textInput" runat="server"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                        <table width="100%" border="0">
                                            <tr>
                                                <td>
                                                    <div class="PageCaptionRight">
                                                        Interest Rate</div>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetNonOrgComm"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetOrgCommission"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetIntRemuneration"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetRemuneration"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox CssClass="textInput" Width="60px" runat="server" ID="txtComSetLevi"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtComSetIncomeTax"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                                                    <asp:TextBox Width="70px" CssClass="textInput" runat="server" ID="txtComSetIncomeTaxAbove"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
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
                                                                    <asp:TextBox Width="60px" CssClass="textInput" runat="server" ID="txtSocialSecurityAmount"
                                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
                                        <table width="100%" border="0">
                                            <tr>
                                                <td>
                                                    <div class="PageCaptionRight">
                                                        Payment Policy</div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="height: 200px; width: 100%; overflow: auto;">
                                            <asp:GridView Width="98%" ID="gvEliPaymentPolicy" runat="server" AutoGenerateColumns="False"
                                                SkinID="SBMLGridGreen" ShowHeader="true">
                                                <Columns>
                                                    <asp:BoundField DataField="bfCustActiType" HeaderText="Customer Activity Type" />
                                                    <asp:BoundField DataField="bfPayModeType" HeaderText="Payment Mode" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No record found
                                                </EmptyDataTemplate>
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
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
                                                    <asp:TextBox runat="server" Width="80px" CssClass="textInput" ID="txtMinimumAge"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                                <td rowspan="2" valign="top">
                                                    <div style="height: 80px; width: 280px; overflow: auto;">
                                                        <asp:GridView Style="width: 94%" ID="gvCustomerType" runat="server" AutoGenerateColumns="true"
                                                            SkinID="SBMLGridGreen" ShowHeader="false">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemStyle Width="15px" />
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox runat="server" ID="chkCusomerType" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No record found
                                                            </EmptyDataTemplate>
                                                            <AlternatingRowStyle CssClass="odd" />
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                                <td rowspan="2" valign="top">
                                                    <asp:DropDownList runat="server" ID="ddlApplicableSex" SkinID="ddlSmall">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top" align="right">
                                                    Maximum age
                                                </td>
                                                <td valign="top">
                                                    <asp:TextBox Width="80px" CssClass="textInput" runat="server" ID="txtMaximumAge"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </div>
                        <%-- End of Eligibility Tab --%>
                    </div>
                    <%-- </fieldset>--%>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function PolicyDetailPopupHide() {
        hideModal('ctl00_cphDet_PD_MDPolicy');
        return false;
    }
    function PolicyDetailPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_PD_MDPolicy');
        return true;
    }
    function PolicyDetailPopup() {
        showModal("ctl00_cphDet_PD_MDPolicy");
        RestCustomerDetail();
        return false;
    }
    function PolicyDetailPopupReturnTrue() {
        showModal("ctl00_cphDet_PD_MDPolicy");
        return true;
    }

    function RestCustomerDetail() {
        //        $(ele).find(':input').each(function() {
        //            switch (this.type) {
        //                case 'select-multiple':
        //                case 'select-one':
        //                case 'text':
        //                case 'textarea':
        //                    $(this).val('');
        //                    break;
        //                case 'checkbox':
        //                    this.checked = false;
        //            }
        //        });
    }
    
</script>

