<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ReinvestmentStatement.aspx.cs" Inherits="SBM_WebUI.mp.ReinvestmentStatement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function selectDeselectAllChekBox(flag) {
            $('#<%=gvData.ClientID %>').find("input:checkbox").each(function() {
                this.checked = flag;
            });
        }

        function PrintValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "Sp Type cannot be empty!");
            sErrorList += RequiredData('<%=txtFromDate.ClientID %>', 'TextBox', "From Date cannot be empty!");
            sErrorList += RequiredData('<%=txtToDate.ClientID %>', 'TextBox', "To Date cannot be empty!");
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
    <%--<b>
         <div class="errorBox" id="div1">
                <ul>
                    <li>WDB -> from date: 09/09/2009, Data is loading by this parameter..</li>
                    <li>//rd = new rptReinvestmentSummary(); // new rptReInvestCoveringLetter(); parameter is not matching .. rd.OpenSubreport("ReinvestmentDetailOld"); cannot open..</li>
                </ul>
        </div>
    </b>--%>
    <fieldset>
        <legend>Reinvestment Statement</legend>
        <table width="100%" align="center" cellpadding="3" cellspacing="0" border="0">
            <tr>
                <td align="right">
                    SP Type
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:DropDownList runat="server" ID="ddlSpType" SkinID="ddlLarge">
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right">
                    From Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="110px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right">
                    To Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="110px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Show Data" ID="btnShowData"
                        OnClick="btnShowData_Click" OnClientClick="return PrintValidation()"></asp:Button>
                </td>
            </tr>
            <tr>
                <td colspan="7">
                    <div style="height: 200px; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%" ID="gvData" runat="server" AutoGenerateColumns="true"
                            SkinID="SBMLGridGreen" ShowHeader="true" OnRowDataBound="gvData_RowDataBound">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle Width="15px" />
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkData" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                    <div align="left" style="padding-top: 6px">
                        <input type="button" class="ButtonAsh" id="btnSelectAll" name="Select" value="Select All"
                            onclick="selectDeselectAllChekBox(true)" />
                        &nbsp;
                        <input type="button" class="ButtonAsh" id="btnDeselectAll" name="DeSelect" value="Deselect All"
                            onclick="selectDeselectAllChekBox(false)" />
                    </div>
                </td>
            </tr>
        </table>
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
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview"
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
