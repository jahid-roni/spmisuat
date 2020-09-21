<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="LienLetter.aspx.cs" Inherits="SBM_WebUI.mp.LienLetter" %>

<%@ Register Src="~/UI/UC/UCSearchLienMark.ascx" TagName="LienMark" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCSearchStopPayment.ascx" TagName="StopPay" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function PrintValidation() {
            var sErrorList = "";

            var rowsGvData = $("#<%=gvData.ClientID %> tr").length;

            if (rowsGvData == 1 || rowsGvData == 0) {
                sErrorList += "<li>Confirmation Data cannot be empty</li>";
            }
            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                return true;
            }
            // end of show error divErroList
        }

        function OpenSearch() {

            var lineOp = document.getElementById('ctl00_cphDet_rdoLetterType_0');
            var lineRemoveOp = document.getElementById('ctl00_cphDet_rdoLetterType_1');
            if (lineOp.checked == true) {
                LienMarkSearchPopupReturnTrue('1.2');
            } else if (lineRemoveOp.checked == true) {
                StopPaymentSearchPopupReturnTrue('4.2')
            }
            return false;
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
    <fieldset>
        <legend>Lien Letter</legend>
        <table width="95%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right" valign="top">
                    Confirmation Letter type
                </td>
                <td valign="top">
                    <div class="fieldLeft">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <asp:RadioButtonList runat="server" ID="rdoLetterType" AutoPostBack="true" OnSelectedIndexChanged="rdoLetterType_OnSelectedIndexChanged">
                                    <asp:ListItem Text="Lien" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Lien Remove Mark"></asp:ListItem>
                                </asp:RadioButtonList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="errorIcon">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="return OpenSearch()" /></div>
                </td>
                <td align="right" valign="top">
                    Lien Bank
                </td>
                <td valign="top">
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <asp:TextBox runat="server" TextMode="MultiLine" Height="50px" Enabled="false" CssClass="textInputLeftDisabled"
                                Width="200px" ID="txtLienBank" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Lien Trans No
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                        <ContentTemplate>
                            <asp:TextBox runat="server" Enabled="false" CssClass="textInputLeftDisabled" Width="130px"
                                ID="txtLienTransNo" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="right">
                    Our Ref. No
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <asp:TextBox runat="server" Enabled="false" CssClass="textInputLeftDisabled" Width="130px"
                                ID="txtOurRefNo" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Their Ref. No
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:TextBox runat="server" Enabled="false" CssClass="textInputLeftDisabled" Width="130px"
                                ID="txtThierRefNo" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="right">
                    Lien Date
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:TextBox runat="server" Enabled="false" CssClass="textInputLeftDisabled" Width="130px"
                                ID="txtLienDate" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Confirmation Data</legend>
        <div style="height: 200px; width: 100%; overflow: auto;">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                <ContentTemplate>
                    <asp:GridView Style="width: 98%" ID="gvData" runat="server" AutoGenerateColumns="true"
                        SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvData_RowDataBound">
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </fieldset>
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="right" valign="top">
                    Export Type &nbsp;
                    <asp:DropDownList ID="ddlExportType" runat="server" SkinID="ddlSmall">
                        <asp:ListItem Selected="True" Value="PDF">Portable Document</asp:ListItem>
                        <asp:ListItem Value="WRD">Word Document</asp:ListItem>
                        <asp:ListItem Value="XLS">Excel Document</asp:ListItem>
                        <asp:ListItem Value="XLR">Data Only</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview"
                        OnClientClick="return PrintValidation()" OnClick="btnPrintPreview_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:StopPay ID="StopPay" runat="server" type="18" title="Lien Remove Search" />
    <uc4:LienMark ID="LienMark" runat="server" type="17" title="Lien Search" />
</asp:Content>
