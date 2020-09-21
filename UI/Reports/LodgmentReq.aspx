<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="LodgmentReq.aspx.cs" Inherits="SBM_WebUI.mp.LodgmentReq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDDQuantity]').keypress(function(e) { return intNumber(e); });
        }

        function CalculateDenomAmount(obj) {

            var amount = 0;
            var denom = $("#<%=ddlDDDenom.ClientID %> option:selected").val();

            if (denom != null) {

                if (obj.value != null) {

                    if (parseInt(obj.value) > 0) {
                        amount = parseInt(denom) * parseInt(obj.value);
                    }
                }
            }
            $("#<%=txtAmount.ClientID %>").val(amount);
        }


        function PrintValidation() {
            var sErrorList = "";

            var rowGvDenomDetail = $("#<%=gvDenomDetail.ClientID %> tr").length;
            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "Sp Type cannot be empty!");
            sErrorList += RequiredData('<%=txtPersonName.ClientID %>', 'TextBox', "Person Name cannot be empty!");
            if (rowGvDenomDetail == 1 || rowGvDenomDetail == 0) {
                sErrorList += "<li>Denomination cannot be empty!</li>";
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


        function AddValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtDDQuantity.ClientID %>', 'TextBox', "Quantity cannot be empty!");

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
        <legend>Lodgment Requisition</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0" cellspacing="3">
                    <tr align="center">
                        <td align="right" width="20%">
                            Sp Type
                        </td>
                        <td width="30%" align="left">
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" width="20%">
                            Date
                        </td>
                        <td width="30%" align="left">
                            <asp:TextBox Width="150px" ID="txtDate" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Denomiantion</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel6">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td width="50%" valign="top">
                            <div style="height: 168px; width: 98%; overflow: auto; border: solid 1px white">
                                <asp:GridView Style="width: 94%" ID="gvDenomDetail" runat="server" AutoGenerateColumns="False"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvDenomDetail_RowCommand"
                                    OnRowDeleting="gvDenomDetail_RowDeleting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemStyle Width="15px" />
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" OnClientClick="return CheckForDelete()"
                                                    ID="btnDenomRemove" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DenominationID" HeaderText="Denomination" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                                        <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Record Found.
                                    </EmptyDataTemplate>
                                    <HeaderStyle CssClass="ssHeader" />
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                            <div>
                                <div class="fieldLeft">
                                    Total Amount
                                </div>
                                <div class="errorIcon">
                                    <asp:TextBox Width="150px" ID="txtTotalAmount" CssClass="textInputDisabled" runat="server"
                                        Enabled="false" onblur="blurActiveInputWithObj(this)"></asp:TextBox>
                                </div>
                            </div>
                        </td>
                        <td valign="top" width="60%">
                            <table width="100%" align="center" class="tableBody" border="0" cellspacing="3">
                                <tr>
                                    <td align="right">
                                        Denomination
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlDDDenom" SkinID="ddlSmall" Width="120px" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Quantity
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:TextBox MaxLength="4" Width="120px" ID="txtDDQuantity" CssClass="textInput"
                                                runat="server" onblur="blurActiveInputWithObj(this)" onchange="CalculateDenomAmount(this)"
                                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="120px" ID="txtAmount" CssClass="textInputDisabled" runat="server"
                                            onblur="blurActiveInputWithObj(this)" Enabled="false" onfocus="highlightActiveInputWithObj(this)"
                                            MaxLength="9"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Button CssClass="ButtonAsh" Width="80px" runat="server" Text="Add" ID="btnAddDenomination"
                                            OnClick="btnAddDenomination_Click" OnClientClick="return AddValidation()" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <br />
    <fieldset>
        <%--<asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>--%>
        <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    Person Name
                </td>
                <td align="left">
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" ID="txtPersonName" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)" Width="250px"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right" width="50%" valign="top">
                    <%--<asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />--%>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnRe" OnClick="btnReset_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPre"
                        OnClick="btnPrintPreview_Click" OnClientClick="return PrintValidation()" />
                    &nbsp; Export Type &nbsp;
                    <asp:DropDownList ID="ddlExportType" runat="server" SkinID="ddlSmall">
                        <asp:ListItem Selected="True" Value="PDF">Portable Document</asp:ListItem>
                        <asp:ListItem Value="WRD">Word Document</asp:ListItem>
                        <asp:ListItem Value="XLS">Excel Document</asp:ListItem>
                        <asp:ListItem Value="XLR">Data Only</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <%--</ContentTemplate>
        </asp:UpdatePanel>--%>
    </fieldset>
</asp:Content>
