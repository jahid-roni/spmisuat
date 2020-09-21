﻿<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ReceiveReg.aspx.cs" Inherits="SBM_WebUI.mp.ReceiveReg" %>

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
                sErrorList += "<li>Please select SP Type</li>";
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
        <legend>Receive Register</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="80%" align="center" cellpadding="3" cellspacing="3" border="0">
                    <tr>
                        <td align="right" valign="top">
                            Currency
                        </td>
                        <td valign="top">
                            <asp:DropDownList runat="server" ID="ddlCurrency" SkinID="ddlSmall" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right" valign="top" rowspan="7">
                            SP Type
                        </td>
                        <td rowspan="7" valign="top">
                            <div style="height: 190px; width: 312px; overflow: auto;">
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
                        <td align="right">
                            From Date
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            To Date
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Our Reference
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtOurReference"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Our Date
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtOurDate" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            BB Reference
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtBBReference"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            BB Date
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="135px" ID="txtBBDate" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                        </td>
                        <td colspan="3">
                            <asp:CheckBox Text="For Bangladesh Bank" ID="chkFBB" runat="server" />
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
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview"
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
