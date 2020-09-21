<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="EncashmentClaim.aspx.cs" Inherits="SBM_WebUI.mp.EncashmentClaim" %>

<%@ Register Src="~/UI/UC/UCSearchClaim.ascx" TagName="Claim" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).dateEntry({ dateFormat: 'dmy/' }); });
        }
        function DisbleControls() {
            hideErrorPanel();
            var control = document.getElementById("<%=btnSaveAndPreview.ClientID%>");
            control.disabled = true;
        }
        function EnableControls() {
            //alert("test");
            hideErrorPanel();
            var control = document.getElementById("<%=btnSaveAndPreview.ClientID%>");
            control.disabled = false;
        }
        function SaveValidation() {
            var sErrorList = "";

            var rowsGvData = $("#<%=gvData.ClientID %> tr").length;
            if (rowsGvData == 1 || rowsGvData == 0) {
                sErrorList += "<li>No data found in Principal Encashment List grid. Please check.</li>";
            }

            //ddlConversionCurrency
            sErrorList += RequiredData('<%=ddlConversionCurrency.ClientID %>', 'DropDownList', "Please select Conversion Currency");
            var oConversionRate = document.getElementById('<%=txtConversionRate.ClientID %>');
            if (oConversionRate != null) {
                if (oConversionRate.value <= 0) {
                    sErrorList += "<li>Conversion Rate cannot be ZERO.</li>"
                }
            }

            // show error divErrorList

            return OpenErrorPanelWithoutPopUp(sErrorList);

            // end of show error divErroList
        }


        function hideErrorPanel() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            divErrorPanel.style.display = "none";
            return true;
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
    <asp:UpdatePanel runat="server" ID="upSuccess" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlMsg" Visible="false">
                <fieldset class="SuccessFieldset" id="fsSuccessFieldset" runat="server">
                    <legend class="SuccessLegend" id="lSuccessLegend" runat="server">Operation Status</legend>
                    <div class="SuccessBox" runat="server" id="lblSuccessBox">
                        <asp:Label runat="server" ID="lblMsg"></asp:Label>
                    </div>
                </fieldset>
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- Error --%>
    <fieldset>
        <legend>Encashment Claim</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Sp Type
                        </td>
                        <td>
                            <asp:DropDownList Width="160px" ID="ddlSpType" runat="server" SkinID="ddlLarge" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Reference No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtReferenceNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSearch" Text="Search" OnClientClick="return ClaimSearchReturnTrue(),DisbleControls()" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Year
                        </td>
                        <td>
                            <asp:DropDownList Width="160px" SkinID="ddlSmall" ID="ddlYear" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Claim Date
                        </td>
                        <td>
                            <asp:TextBox Width="160px" ID="txtClaimDate" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Report Category
                        </td>
                        <td>
                            <asp:DropDownList AutoPostBack="true" Width="160px" SkinID="ddlSmall" ID="ddlReportCategory" runat="server">
                                <asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>
                                <asp:ListItem Value="NEW">BB New Format</asp:ListItem>
                                <asp:ListItem Value="OLD">BB Old Format</asp:ListItem>
                                <asp:ListItem Value="PRE">BB Pre-matured</asp:ListItem>
                                <asp:ListItem Value="RIV">BB Reinvestment</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            
                        </td>
                        <td>
                            
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Payment Date From
                        </td>
                        <td>
                            <asp:TextBox Width="132px" ID="txtPaymentDateFrom" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Payment Date To
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtPaymentDateTo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnShowData" Text="Show Data"
                                    OnClientClick="return hideErrorPanel()" OnClick="btnShowData_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Principal Encashment List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <div style="height: 200px; width: 100%; overflow: auto;">
                    <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" Width="98%"
                        SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" Checked='<%# Eval("IsSelect") %>' ID="IsSelect" />
                                    <asp:HiddenField ID="hdCurrencyID" Value='<%# Eval("CurrencyID") %>' runat="server" />
                                    <asp:HiddenField ID="InterestRate" Value='<%# Eval("InterestRate") %>' runat="server" />
                                    <asp:HiddenField ID="NoOfCouponsToBeEncashed" Value='<%# Eval("NoOfCouponsToBeEncashed") %>'
                                        runat="server" />
                                    <asp:HiddenField ID="AlreadyPaidInterest" Value='<%# Eval("AlreadyPaidInterest") %>'
                                        runat="server" />
                                    <asp:HiddenField ID="CalculatedInterest" Value='<%# Eval("CalculatedInterest") %>'
                                        runat="server" />
                                    <asp:HiddenField ID="EncashmentTransNo" Value='<%# Eval("EncashmentTransNo") %>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RegNo" HeaderStyle-Width="150px" HeaderText="Reg No" />
                            <asp:BoundField DataField="IssueName" HeaderText="Issue Name" />
                            <asp:BoundField DataField="EncashDate" HeaderText="Encashment Date" DataFormatString="{0: dd-MMM-yyyy}" />
                            <asp:BoundField DataField="ActualPrincipalAmount" HeaderText="Principal." />
                            <asp:BoundField DataField="InterestToBePaid" HeaderText="Interest." />
                            <asp:BoundField DataField="LeviToBePaid" HeaderText="Levi." />
                            <asp:BoundField DataField="IncomeTaxToBePaid" HeaderText="Income Tax." />
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <AlternatingRowStyle CssClass="odd" />
                    </asp:GridView>
                </div>
                <div align="right" style="padding-top: 6px">
                    <asp:TextBox Width="220px" Enabled="false" CssClass="textInputDisabled" runat="server"
                        ID="txtTotalSelectedRow"></asp:TextBox>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Calculate" ID="btnCalculate"
                        OnClick="btnCalculate_Click" />
                    &nbsp;
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSelectAll" name="Select" Text="Select All"
                        OnClick="btnSelectAll_Click" />
                    &nbsp;
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnDeselectAll" name="DeSelect"
                        Text="Deselect All" OnClick="btnDeselectAll_Click" />
                    &nbsp;
                    <asp:HiddenField ID="hdnClaimTransNo" runat="server" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Encashment Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Total Principal
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtTotalPrincipal1" CssClass="textInputDisabled" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Enabled="false"></asp:TextBox>
                        </td>
                        <td colspan="2" align="center">
                            <asp:CheckBox runat="server" ID="chkWithCoverLetter" Text="With Cover Letter" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" width="12%">
                            Total Income Tax
                        </td>
                        <td width="14%">
                            <asp:TextBox Width="100px" ID="txtTotalIncomeTax1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="18%">
                            Total Levi
                        </td>
                        <td width="13%">
                            <asp:TextBox Width="100px" ID="txtTotalLevi1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="17%">
                            Total Interest
                        </td>
                        <td width="13%">
                            <asp:TextBox Width="100px" ID="txtTotalInterest1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Remuneration
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtRemuneration" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Conversion Currency
                        </td>
                        <td>
                            <asp:DropDownList Width="100px" SkinID="ddlSmall" ID="ddlConversionCurrency" runat="server"
                                OnSelectedIndexChanged="ddlConversionCurrency_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Conversion Rate
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtConversionRate" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                OnTextChanged="txtConversionRate_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Income Tax
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtTotalIncomeTax2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Levi
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtTotalLevi2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Interest
                        </td>
                        <td>
                            <asp:TextBox Width="100px" ID="txtTotalInterest2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Principal
                        </td>
                        <td colspan="5">
                            <asp:TextBox Width="100px" Enabled="false" ID="txtTotalPrincipal2" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnCalculate" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="txtConversionRate" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center" valign="top">
                    Export Type &nbsp;
                    <asp:DropDownList ID="ddlExportType" runat="server" SkinID="ddlSmall">
                        <asp:ListItem Selected="True" Value="PDF">Portable Document</asp:ListItem>
                        <asp:ListItem Value="WRD">Word Document</asp:ListItem>
                        <asp:ListItem Value="XLS">Excel Document</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="EnableControls()" />&nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnPreview" OnClick="btnPreview_Click"
                        OnClientClick="return SaveValidation()" />&nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save & Preview" ID="btnSaveAndPreview"
                        OnClick="btnSaveAndPreview_Click" OnClientClick="return SaveValidation()" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc1:Claim ID="Claim" runat="server" Type="ENCASHMENT_CLAIM" PageCaption="Encashment Claim Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
