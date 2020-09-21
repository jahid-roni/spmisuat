<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="InterestClaim.aspx.cs" Inherits="SBM_WebUI.mp.InterestClaim" %>

<%@ Register Src="~/UI/UC/UCSearchClaim.ascx" TagName="Claim" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).dateEntry({ dateFormat: 'dmy/' }); });
        }
        function DisbleControls() {
            CloseErrorPanel();
            var control = document.getElementById("<%=btnSaveAndPreview.ClientID%>");
            control.disabled = true;
        }
        function EnableControls() {
            //alert("test");
            CloseErrorPanel();
            var control = document.getElementById("<%=btnSaveAndPreview.ClientID%>");
            control.disabled = false;
        }

        function SaveValidation() {
            var sErrorList = "";

            var rowsGvData = $("#<%=gvData.ClientID %> tr").length;
            var colGvData = $("#<%=gvData.ClientID %>").find('tr')[0].cells.length;

            if (rowsGvData == 0 || colGvData <= 1) {
                sErrorList += "<li>No data found in Interest Payment List grid. Please check.</li>";
            }

            sErrorList += RequiredData('<%=ddlConversionCurrency.ClientID %>', 'DropDownList', "Conversion Currency cannot be empty.");
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

        function selectDeselectAllChekBox(flag) {
            $('#<%=gvData.ClientID %>').find("input:checkbox").each(function() {
                this.checked = flag;
            });
            //calculate();
        }

        function calculate() {
            var vTotIncomeTax = 0.0;
            var vTotInterest = 0.0;

            vTotIncomeTax = calculateTotalValue(7);
            $("#<%=txtTotalIncomeTax1.ClientID %>").val(vTotIncomeTax.toFixed(2));
            var vRemuration = $("#<%=txtRemuneration.ClientID %>").val();
            $("#<%=txtTotalIncomeTax2.ClientID %>").val((vTotIncomeTax + parseFloat(vRemuration)).toFixed(2));

            vTotInterest = calculateTotalValue(5);
            $("#<%=txtTotalInterest1.ClientID %>").val(vTotInterest.toFixed(2));
            var vConvRate = $("#<%=txtConversionRate.ClientID %>").val();
            $("#<%=txtTotalInterest2.ClientID %>").val((vTotInterest * parseFloat(vConvRate)).toFixed(2));
        }

        function calculateTotalValue(colIndex) {
            var total = 0.0;
            var vCheckedCount = 0;
            $("tr:has(:checkbox:checked) td:nth-child(" + colIndex + ")").each(function() {
                total += parseFloat($(this).text());
                vCheckedCount++;
            });
            var vTotalData = $("#<%=hdnGridTotal.ClientID %>").val();
            $("#<%=txtSelectCount.ClientID %>").val("TOTAL SELECTED " + vCheckedCount + " OF " + vTotalData);
            return total;
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
    <asp:UpdatePanel runat="server" ID="upSuccess">
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
        <legend>Interest Claim</legend>
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
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSearch" Text="Search" 
                                    OnClientClick="return ClaimSearchReturnTrue(),DisbleControls()"/></div>
                            <asp:HiddenField ID="hdnClaimTransNo" runat="server" />
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
                            <asp:TextBox Width="160px" ID="txtClaimDate" CssClass="textInput" runat="server" Enabled="false"
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
                                    OnClientClick="CloseErrorPanel()" OnClick="btnShowData_Click" />
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
        <legend>Interest Payment List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
                    border="0">
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" style="color: Black; background-color: #E9F0D5;
                                border-color: #C1D586; border-width: 1px; border-style: Solid; width: 98%; border-collapse: collapse;">
                                <tr style="color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal;
                                    white-space: nowrap;">
                                    <th style="width: 35px" align="left" scope="col">
                                        Select
                                    </th>
                                    <th style="width: 150px" align="left" scope="col">
                                        Reg No.
                                    </th>
                                    <th style="width: 150px" align="left" scope="col">
                                        Issue Name
                                    </th>
                                    <th style="width: 100px" align="left" scope="col">
                                        Payment Date
                                    </th>
                                    <th style="width: 100px" align="left" scope="col">
                                        Interest Payable
                                    </th>
                                    <th style="width: 100px" align="left" scope="col">
                                        SS Premium
                                    </th>
                                    <th style="width: 100px" align="left" scope="col">
                                        Income Tax
                                    </th>
                                    <th style="width: 100px" align="left" scope="col">
                                        Payment
                                    </th>
                                </tr>
                            </table>
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView ID="gvData" runat="server" Width="98%" BorderWidth="1px" AutoGenerateColumns="false"
                                    SkinID="SBMLGridV2" ShowHeader="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemStyle HorizontalAlign="Left" Width="35px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkSelected" />
                                                <asp:HiddenField ID="hdnIntPayTranNo" Value='<%# Eval("IntPaymentTransNo")%>' runat="server" />
                                                <asp:HiddenField ID="hdnNoOfCoupon" Value='<%# Eval("NoOfCoupon")%>' runat="server" />
                                                <asp:HiddenField ID="hdnLevi" Value='<%# Eval("LeviA")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="RegNo" HeaderStyle-Width="150px" HeaderText="Reg No" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="IssueName" ItemStyle-Width="150px" HeaderText="Issue Name" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="PaymentDate" HeaderStyle-Width="100px" HeaderText="Payment Date"
                                            ItemStyle-HorizontalAlign="Center" DataFormatString="{0: dd-MMM-yyyy}" />
                                        <asp:BoundField DataField="PaidInterest" HeaderStyle-Width="100px" HeaderText="Interest Payable"
                                            ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="SocialSecurityAmount" HeaderStyle-Width="100px" HeaderText="SS Premium"
                                            ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="IncomeTax" HeaderStyle-Width="100px" HeaderText="Income Tax"
                                            ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="PaymentAmount" HeaderStyle-Width="100px" HeaderText="Amount"
                                            ItemStyle-HorizontalAlign="Center" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
                <div align="right" style="padding-top: 6px">
                    <asp:TextBox Width="250px" ID="txtSelectCount" CssClass="textInputDisabled" ReadOnly="true"
                        runat="server" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Calculate" ID="btnCalculate"
                        OnClick="btnCalculate_Click" />
                    &nbsp;
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSelectAll" name="Select" Text="Select All"
                        OnClick="btnSelectAll_Click" />
                    &nbsp;
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnDeselectAll" name="DeSelect"
                        Text="Deselect All" OnClick="btnDeselectAll_Click" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Interest Payment Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField ID="hdnGridTotal" runat="server" />
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td colspan="2">
                            <asp:CheckBoxList runat="server" ID="chkListForBB" RepeatDirection="Horizontal">
                                <asp:ListItem Text="With Cover Letter" Value="With Cover Letter"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" width="15%">
                            Total Income Tax
                        </td>
                        <td width="16%">
                            <asp:TextBox Width="135px" ID="txtTotalIncomeTax1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="18%">
                            Total Levi
                        </td>
                        <td width="15%">
                            <asp:TextBox Width="135px" ID="txtTotalLevi1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="16%">
                            Total Interest
                        </td>
                        <td width="15%">
                            <asp:TextBox Width="135px" ID="txtTotalInterest1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Remuneration
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtRemuneration" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Conversion Currency
                        </td>
                        <td>
                            <asp:DropDownList Width="100px" ID="ddlConversionCurrency" runat="server" SkinID="ddlSmall"
                                OnSelectedIndexChanged="ddlConversionCurrency_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Conversion Rate
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtConversionRate" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                OnTextChanged="txtConversionRate_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Income Tax
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtTotalIncomeTax2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Levi
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtTotalLevi2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Interest
                        </td>
                        <td>
                            <asp:TextBox Width="135px" ID="txtTotalInterest2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            &nbsp;
                        </td>
                        <td>
                        </td>
                        <td align="right">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="right">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                <asp:AsyncPostBackTrigger EventName="SelectedIndexChanged" ControlID="ddlConversionCurrency" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center" valign="top">
                    &nbsp;Export Type
                    <asp:DropDownList ID="ddlExportType" runat="server" SkinID="ddlSmall">
                        <asp:ListItem Selected="True" Value="PDF">Portable Document</asp:ListItem>
                        <asp:ListItem Value="WRD">Word Document</asp:ListItem>
                        <asp:ListItem Value="XLS">Excel Document</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="EnableControls()"
                        OnClick="btnReset_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnPreview" OnClientClick="return SaveValidation()"
                        OnClick="btnPreview_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save & Preview" ID="btnSaveAndPreview"
                        OnClientClick="return SaveValidation()" OnClick="btnSaveAndPreview_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" 
                        onclick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc1:Claim ID="Claim" runat="server" Type="INTEREST_CLAIM" PageCaption="Interest Claim Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
