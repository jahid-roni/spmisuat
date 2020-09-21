<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="CommissionClaim.aspx.cs" Inherits="SBM_WebUI.mp.CommissionClaim" %>

<%@ Register Src="~/UI/UC/UCSearchClaim.ascx" TagName="Claim" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).dateEntry({ dateFormat: 'dmy/' }); });
        });
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
                sErrorList += "<li>No data found in Sales Reference List grid. Please check.</li>";
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
            return OpenErrorPanel(sErrorList, 'Save');
            // end of show error divErroList
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Mandatory Field List</legend>
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
        <legend>Commission Claim</legend>
        <asp:UpdatePanel runat="server" ID="upGv">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Sp Type
                        </td>
                        <td>
                            <asp:DropDownList Width="160px" ID="ddlSpType" SkinID="ddlLarge" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right" width="20%">
                            Reference No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox Width="160px" ID="txtReferenceNo" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSearch" Text="Search" OnClientClick="return ClaimSearchReturnTrue(),DisbleControls()" /></div>
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
                            <asp:TextBox Width="140px" ID="txtStatementDate" CssClass="textInput" runat="server" Enabled="false"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Date From
                        </td>
                        <td>
                            <asp:TextBox Width="130px" ID="txtDateFrom" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Date To
                        </td>
                        <td>
                            <asp:DropDownList Width="160px" SkinID="ddlSmall" ID="ddlDateTo" runat="server" OnSelectedIndexChanged="ddlDateTo_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Duration
                        </td>
                        <td colspan="3">
                            <asp:DropDownList runat="server" ID="ddlDuration" SkinID="ddlSmall" OnSelectedIndexChanged="ddlDuration_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <%-- Hidden Field --%>
                <asp:HiddenField ID="hdnClaimTransNo" runat="server" />
                <%-- Error --%>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Sales Reference List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table cellspacing="0" cellpadding="5" border="0" style="color: Black;
                    background-color: #E9F0D5; border-color: #C1D586; border-width: 1px; border-style: Solid;
                    width: 98%; border-collapse: collapse;">
                    <tr style="color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal;
                        white-space: nowrap;">
                        <th style="width:36px" scope="col">
                            Select
                        </th>
                        <th style="width:161px" scope="col">
                            Reference No.
                        </th>
                        <th style="width:90px" scope="col">
                            Date
                        </th>
                        <th style="width:93px" scope="col">
                            Total<br />Face Value
                        </th>
                        <th style="width:115px" scope="col">
                            Non-Org.<br />Face Value
                        </th>
                        <th style="width:124px" scope="col">
                            Non-Org.<br />Commission
                        </th>
                        <th style="width:89px" scope="col">
                            Org.<br />Face Value
                        </th>
                        <th style="width:120px" scope="col">
                            Org. Commission
                        </th>
                    </tr>
                </table>
                <div style="height: 200px; width: 100%; overflow: auto;">
                    <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" SkinID="SBMLGridGreen"
                        ShowHeader="false" Width="98%">
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <ItemStyle HorizontalAlign="Left" Width="36px" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="chkSelected" />
                                    <asp:HiddenField ID="hdnSalesStatementTranNo" Value='<%# Eval("SaleStatementTransNo")%>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SaleStatementReferenceNo" HeaderText="Reference No." HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="161px" />
                            <asp:BoundField DataField="StatementDate" HeaderText="Date" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0: dd-MMM-yyyy}" ItemStyle-Width="90px" />
                            <asp:BoundField DataField="TotalFaceValue" HeaderText="Total Face Value" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="93px" />
                            <asp:BoundField DataField="NonOrgFaceValue" HeaderText="Non-Org. Face Value" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="115px" />
                            <asp:BoundField DataField="TotalNonOrgCommission" HeaderText="Non-Org. Commission"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="124px" />
                            <asp:BoundField DataField="OrgFaceValue" HeaderText="Org. Face Value" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="89px" />
                            <asp:BoundField DataField="TotalOrgCommission" HeaderText="Org. Commission" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px" />
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <AlternatingRowStyle CssClass="odd" />
                    </asp:GridView>
                </div>
                <div align="right" style="padding-top: 6px">
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSelectAll" name="Select" Text="Select All"
                        OnClick="btnSelectAll_Click" />
                    &nbsp;
                    <asp:Button runat="server" CssClass="ButtonAsh" ID="btnDeselectAll" name="DeSelect"
                        Text="Deselect All" OnClick="btnDeselectAll_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Calculate" ID="btnCalculate"
                        OnClick="btnCalculate_Click" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Sales Reference Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td valign="top" align="center">
                            <asp:CheckBoxList runat="server" ID="chkListForBB" RepeatDirection="Horizontal">
                                <asp:ListItem Text="With Cover Letter" Value="With Cover Letter"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td valign="top">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="View Journal" ID="Button4" />
                        </td>
                        <td align="right" valign="top">
                            Total Face Value
                        </td>
                        <td valign="top" width="20%">
                            <asp:TextBox Width="120px" ID="txtTotalFaceValue" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            Total Org. Commission
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtTotalOrgCommission1" CssClass="textInputDisabled"
                                runat="server" Enabled="false" onfocus="highlightActiveInputWithObj(this)" onblur="blurActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                        <td align="right">
                            Total Non-Org. Commission
                        </td>
                        <td width="20%">
                            <asp:TextBox Width="120px" ID="txtTotalNonOrgCommission1" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Conversion Currency
                        </td>
                        <td>
                            <asp:DropDownList Width="120px" SkinID="ddlSmall" ID="ddlConversionCurrency" CssClass="textInput"
                                runat="server" OnSelectedIndexChanged="ddlConversionCurrency_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Conversion Rate
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtConversionRate" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total Org. Commission
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtTotalOrgCommission2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Non-Org. Commission
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtTotalNonOrgCommission2" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="3">
                            Total Commission Claim
                        </td>
                        <td>
                            <asp:TextBox Width="120px" ID="txtTotalCommissionClaim" Enabled="false" CssClass="textInputDisabled"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
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
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center" valign="top">
                    Export Type
                    &nbsp;
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
                        OnClick="btnSaveAndPreview_Click" OnClientClick="return SaveValidation()" />
                &nbsp;<asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc1:Claim ID="Claim" runat="server" Type="COMMISSION_CLAIM" PageCaption="Commission Claim Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
