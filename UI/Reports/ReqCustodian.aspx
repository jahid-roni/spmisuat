<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ReqCustodian.aspx.cs" Inherits="SBM_WebUI.mp.ReqCustodian" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        function selectAllChekBox(flag) {

            $("#<%=chkLSpType.ClientID %> input:checkbox").each(function() {
                this.checked = flag;
            });
        }


        function PrintValidation() {
            var sErrorList = "";
            var isChecked = false;

            $("#<%=chkLSpType.ClientID %> input:checkbox").each(function() {

                if ($(this).is(":checked")) {
                    isChecked = true;
                    return false;
                }
            });

            if (!isChecked) {
                sErrorList += "<li>Please select SP Type/Bond</li>";
            }
            sErrorList += RequiredData('<%=ddlCurrency.ClientID %>', 'DropDownList', "Currency cannot be empty!");

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
    <fieldset>
        <legend>Requisition to Register</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="80%" align="center" cellpadding="3" cellspacing="3" border="0">
                    <tr>
                        <td align="right" valign="top">
                            Status
                        </td>
                        <td valign="top">
                            <asp:RadioButtonList RepeatDirection="Vertical" runat="server" ID="rdlStatus">
                            </asp:RadioButtonList>
                        </td>
                        <td align="right" valign="top">
                            SP Type/Bond
                        </td>
                        <td rowspan="3" valign="top">
                            <div style="height: 150px; width: 312px; overflow: auto;">
                                <asp:CheckBoxList CssClass="checkboxlist_nowrap" SkinID="SBMCheckListGreen" runat="server"
                                    ID="chkLSpType" RepeatDirection="Vertical">
                                </asp:CheckBoxList>
                            </div>
                            <table cellpadding="0">
                                <tr>
                                    <td style="padding-top: 10px">
                                        <input type="button" class="ButtonAsh" id="btnSelectAll" name="Select" value="Select All"
                                            onclick="selectAllChekBox(true)" />&nbsp;&nbsp;
                                    </td>
                                    <td style="padding-top: 10px">
                                        <input type="button" class="ButtonAsh" id="btnDeselectAll" name="DeSelect" value="Deselect All"
                                            onclick="selectAllChekBox(false)" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Currency
                        </td>
                        <td valign="top" colspan="2">
                            &nbsp;<asp:DropDownList runat="server" ID="ddlCurrency" SkinID="ddlSmall" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Date
                        </td>
                        <td colspan="2">
                            &nbsp;<asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtDate"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
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
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
