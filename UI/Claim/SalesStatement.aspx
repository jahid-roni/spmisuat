<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SalesStatement.aspx.cs" Inherits="SBM_WebUI.mp.SalesStatement" %>

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
            CloseErrorPanel();
            var control = document.getElementById("<%=btnSaveAndPreview.ClientID%>");
            control.disabled = false;
        }
        function selectDeselectAllChekBox(flag) {
            $('#<%=gvData.ClientID %>').find("input:checkbox").each(function() {
                this.checked = flag;
            });
            calculateTotalFaceValue(5);
        }

        function calculateTotalFaceValue(colIndex) {
            var total = 0.0;
            $("tr:has(:checkbox:checked) td:nth-child(" + colIndex + ")").each(function() {
                total += parseFloat($(this).text());
            });
            $("#<%=txtTotalFaceValue.ClientID %>").val(total.toFixed(2));
        }

        function SaveValidation() {

            var sErrorList = "";

            var rowsGvData = $("#<%=gvData.ClientID %> tr").length;
            if (rowsGvData == 1 || rowsGvData == 0) {
                sErrorList += "<li>No data found in Registration Details gird. Please check.</li>";
            }

            return OpenErrorPanel(sErrorList, 'Save');

        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <%-- Error --%>
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
    <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
    <fieldset>
        <legend>Sales Statement </legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="20%">
                            Sp Type
                        </td>
                        <td>
                            <asp:DropDownList Width="160px" ID="ddlSpType" SkinID="ddlLarge" runat="server" OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged"
                                AutoPostBack="True">
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
                                <asp:Button runat="server" CssClass="ButtonAsh" ID="btnSearch" Text="Search" OnClientClick="return ClaimSearchReturnTrue(),DisbleControls()" />
                                <asp:HiddenField ID="hdnClaimTransNo" runat="server" />
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
                            Statement Date
                        </td>
                        <td>
                            <asp:TextBox Width="140px" ID="txtStatementDate" CssClass="textInput" Enabled="false"
                                runat="server"></asp:TextBox>
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
                            <asp:DropDownList Width="160px" SkinID="ddlSmall" ID="ddlDateTo" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Duration
                        </td>
                        <td colspan="1">
                            <asp:DropDownList runat="server" ID="ddlDuration" SkinID="ddlSmall" OnSelectedIndexChanged="ddlDuration_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="btnLoad" CssClass="ButtonAsh" Text="Load Data" runat="server" OnClientClick="return CloseErrorPanel()"
                                OnClick="btnLoad_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Registration Details</legend>
        <asp:UpdatePanel ID="upGvData" runat="server">
            <ContentTemplate>
                <div style="height: 200px; width: 100%; overflow: auto;">
                    <asp:GridView ID="gvData" OnRowDataBound="gvData_RowDataBound" runat="server" AutoGenerateColumns="false"
                        Width="98%" SkinID="SBMLGridGreen">
                        <Columns>
                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="chkSelected" />
                                    <asp:HiddenField ID="hdnIssueTransNo" Value='<%# Eval("IssueTransNo") %>' runat="server" />
                                    <asp:HiddenField ID="hdnOrgAmount" Value='<%# Eval("OrgAmount") %>' runat="server" />
                                    <asp:HiddenField ID="hdnNonOrgAmount" Value='<%# Eval("NonOrgAmount") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RegNo" HeaderStyle-Width="10%" HeaderText="Reg. No." HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="IssueDate" HeaderStyle-Width="10%" HeaderText="Issue Date"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataFormatString="{0: dd-MMM-yyyy}" />
                            <asp:BoundField DataField="CustomerName" HeaderStyle-Width="13%" HeaderText="Customer Name"
                                HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="NoOfCertificates" HeaderStyle-Width="15%" HeaderText="No Of Denomination(s)"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="IssueAmount" HeaderStyle-Width="19%" HeaderText="Issue Amount"
                                HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
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
                <asp:AsyncPostBackTrigger ControlID="btnLoad" EventName="Click" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td width="20%" valign="top">
                            <asp:CheckBoxList runat="server" ID="chkListForBB" RepeatDirection="Vertical">
                                <asp:ListItem Text="With Cover Letter" Value="With Cover Letter"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td width="50%" valign="top">
                        </td>
                        <td align="right" width="15%" valign="top">
                            Total Face Value
                        </td>
                        <td width="15%" valign="top">
                            <asp:TextBox Width="120px" ID="txtTotalFaceValue" CssClass="textInputDisabled" Enabled="false"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center" valign="top">
                    Export Type&nbsp;
                    <asp:DropDownList ID="ddlExportType" runat="server" SkinID="ddlSmall">
                        <asp:ListItem Selected="True" Value="PDF">Portable Document</asp:ListItem>
                        <asp:ListItem Value="WRD">Word Document</asp:ListItem>
                        <asp:ListItem Value="XLS">Excel Document</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="EnableControls()"
                        OnClick="btnReset_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnPreview" OnClick="btnPreview_Click"
                        OnClientClick="return SaveValidation()" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save & Preview" ID="btnSaveAndPreview"
                        OnClick="btnSaveAndPreview_Click" OnClientClick="return SaveValidation()" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" 
                        onclick="btnDelete_Click" />

                </td>
            </tr>
        </table>
        <%--<asp:AsyncPostBackTrigger ControlID="btnSaveAndPreview" EventName="Click" />--%>
    </fieldset>
    <uc1:Claim ID="Claim" runat="server" Type="SALESSTATEMENT_CLAIM" PageCaption="Sales Statement Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
