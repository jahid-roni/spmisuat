<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SPReceive.aspx.cs" EnableEventValidation="false" Inherits="SBM_WebUI.mp.SPReceive" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCSearchSPReceive.ascx" TagName="SPReceiveSearch" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCViewGridData.ascx" TagName="VGData" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend>Unapproved Receive List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdReceiveTransNo" />
                            <asp:HiddenField runat="server" ID="hdDenom" />
                            <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:HiddenField runat="server" ID="hdDigitsInSlNo" />
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging"
                                OnRowDataBound="gvData_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="View" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="View" runat="server" ID="btnView" Text="View"
                                                OnClientClick="CloseErrorPanel()" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnAddReceiveDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <fieldset>
                <legend>Receive Details</legend>
                <table width="100%" align="center" cellpadding="3" cellspacing="4" border="0">
                    <tr>
                        <td align="right">
                            Transaction No
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="12" ID="txtTransNo" ReadOnly="true" CssClass="textInput"
                                Width="138px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Button
                                    CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="return SearchSPReceivePopupReturnTrue()" />
                            &nbsp;
                        </td>
                        <td align="right">
                            Receive Date
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtReceiveDate" CssClass="textInput" Width="220px"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList SkinID="ddlLarge" runat="server" ID="ddlSPType" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlSPType_SelectedIndexChanged" Width="220px">
                                </asp:DropDownList>
                            </div>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Re-Order Show" ID="btnReOrder" OnClick="btnReOrder_Click"
                                OnClientClick="return ShowVGDataShow()" />
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Receive Amount
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtReceiveAmount" MaxLength="15" CssClass="textInput"
                                    Width="220px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnAddReceiveDetails" />
        </Triggers>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <legend>SP Receive Details List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" cellpadding="2" cellspacing="2" border="0">
                    <tr>
                        <td align="right">
                            Denomination
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList AutoPostBack="true" OnSelectedIndexChanged="ddlDenomination_SelectedIndexChanged"
                                    runat="server" SkinID="ddlCommon" Width="100px" ID="ddlDenomination" onChange="ResetDetailDataOnChangeDenom()">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            From
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtFrom" MaxLength="7" CssClass="textInput" Width="80px"
                                    onblur="blurActiveInputWithObj(this), SeriesCheckData('From') , SeriesQuantity(), addPadding(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            To
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtTo" MaxLength="7" CssClass="textInput" Width="80px"
                                    onblur="blurActiveInputWithObj(this), SeriesCheckData('To') , SeriesQuantity(), addPadding(this)"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Quantity
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtQuantity" Enabled="false" MaxLength="3" CssClass="textInput"
                                Width="80px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;Add&nbsp;&nbsp;" ID="btnAddReceiveDetails"
                                OnClick="btnAddReceiveDetails_Click" OnClientClick="return SaveReceiveValidation()" />
                        </td>
                    </tr>
                    <tr>
                                            <td align="right">
                            Series
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox runat="server" ID="txtSeries" Enabled="false" MaxLength="12" CssClass="textInput"
                                    Width="95px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Total Amount
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtDnmAmount" Enabled="false" MaxLength="3" CssClass="textInput"
                                Width="80px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Cabinet Number
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox runat="server" ID="txtCabinetNumber" MaxLength="2" CssClass="textInput"
                                Width="80px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                </div>
                            
                        </td>
                        <td align="right">
                            Drawer Number
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox runat="server" ID="txtDrawerNumber" MaxLength="2" CssClass="textInput"
                                Width="80px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                </div>
                            
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnResetReceiveDetails"
                                OnClick="btnResetReceiveDetails_Click" OnClientClick="return ResetDetailData()" />
                        </td>
                    </tr>
                </table>
                <div style="height: 200px; width: 100%; overflow: auto;">
                    <asp:GridView Style="width: 98%" ID="gvReceiveDetail" runat="server" AutoGenerateColumns="False"
                        SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvReceiveDetail_RowCommand"
                        OnRowDataBound="gvReceiveDetail_RowDataBound" OnRowDeleting="gvReceiveDetail_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                        Text="Select" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Delete" OnClientClick="return DeleteReceiveDetail()"
                                        runat="server" ID="btnDelete" Text="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="bfDenom" HeaderText="Denomination" />
                            <asp:BoundField DataField="bfSeries" HeaderText="Series" />
                            <asp:BoundField DataField="bfFrom" HeaderText="From" />
                            <asp:BoundField DataField="bfTo" HeaderText="To" />
                            <asp:BoundField DataField="bfQuantity" HeaderText="Quantity" />
                            <asp:BoundField DataField="bfDnmAmount" HeaderText="Total Amount" />
                            <asp:BoundField DataField="bfCabinetNumber" HeaderText="Cabinet Number" />
                            <asp:BoundField DataField="bfDrawerNumber" HeaderText="Drawer Number" />                            
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <AlternatingRowStyle CssClass="odd" />
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnAddReceiveDetails" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    
    <%--<fieldset>
        <legend>Re-Order notification</legend>
               <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <asp:GridView AutoGenerateColumns="true" Width="98%" ID="gvStock" runat="server"
                    SkinID="SBMLGridGreen">
                    <EmptyDataTemplate>
                        No record found
                    </EmptyDataTemplate>
                    <AlternatingRowStyle CssClass="odd" />
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />--%>
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="100%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="View Journals" ID="btnViewJournals"
                        OnClientClick="return ViewJournals()" onclick="btnViewJournals_Click" Visible="false"/>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="CloseErrorPanel()"
                        OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClick="btnSave_Click"
                        OnClientClick="return SaveValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:SPReceiveSearch ID="ucSearchSPReceive" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />
    <uc4:VGData ID="VGD" runat="server" />

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtReceiveAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtQuantity]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtFrom]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtTo]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtCabinetNumber]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtDrawerNumber]').keypress(function(e) { return intNumber(e); });
        }


        function ShowVGDataShow() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "You must load Denom Type first before viewing detail");

            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                CloseErrorPanel();
                VGDataShow();
                return true;
            }
            // end of show error divErroList
        }
        function ViewJournals() {
            alert("It has not compeleted..");
            return false;
        }

        function SeriesQuantity() {
            var ddlDenomination = $("#<%=ddlDenomination.ClientID %>").val();
            var txtCDSeriesFrom = $("#<%=txtFrom.ClientID %>").val();
            if (parseInt(txtCDSeriesFrom, 10) == 0) {
                $("#<%=txtFrom.ClientID %>").val('');
                txtCDSeriesFrom = "";
            }
            var txtCDSeriesTo = $("#<%=txtTo.ClientID %>").val();
            if (parseInt(txtCDSeriesTo, 10) == 0) {
                $("#<%=txtTo.ClientID %>").val('');
                txtCDSeriesTo = "";
            }
            var vQuantity = "";
            if (txtCDSeriesTo != "" && txtCDSeriesFrom != "") {
                vQuantity = parseInt(txtCDSeriesTo, 10) - parseInt(txtCDSeriesFrom, 10) + 1;
            }
            if (ddlDenomination != "" && vQuantity != "") {
                $("#<%=txtDnmAmount.ClientID %>").val(ddlDenomination * vQuantity);
            }
            $("#<%=txtQuantity.ClientID %>").val(vQuantity);
        }

        function SeriesCheckData(type) {
            var txtCDSeriesTo = $("#<%=txtTo.ClientID %>").val();
            var txtCDSeriesFrom = $("#<%=txtFrom.ClientID %>").val();
            if (txtCDSeriesTo != "" && txtCDSeriesFrom != "") {
                if (parseInt(txtCDSeriesTo, 10) < parseInt(txtCDSeriesFrom, 10)) {
                    $("#<%=txtQuantity.ClientID %>").val('');
                    if (type == "To") {
                        $("#<%=txtFrom.ClientID %>").val('');
                        return false;
                    } else {
                        $("#<%=txtTo.ClientID %>").val('');
                        return false;
                    }
                }
            }
            return true;
        }
        function addPadding(txtObj) {
            if (txtObj.value == "") {
                txtObj.value = "";
            } else {
                txtObj.value = leftPad(txtObj.value, Number($("#<%=hdDigitsInSlNo.ClientID %>").val()));
            }
            return false;
        }

        function SaveValidation() {
            window.scroll(0, 0);
            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }

            var sErrorList = "";

            sErrorList += RequiredData('<%=txtReceiveDate.ClientID %>', 'TextBox', "Receive Date cannot be empty!");
            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=txtReceiveAmount.ClientID %>', 'TextBox', "Receive Amount cannot be empty!");

            var rowsGvDenom = $("#<%=gvReceiveDetail.ClientID %> tr").length;
            if (rowsGvDenom == 1 || rowsGvDenom == 0) {
                sErrorList += "<li>Denominations List cannot be null</li>";
            }

            return OpenErrorPanel(sErrorList, 'Save');
        }


        function SaveReceiveValidation() {

            var sErrorList = "";

            sErrorList += RequiredData('<%=txtReceiveAmount.ClientID %>', 'TextBox', "Receive Amount cannot be empty!");
            sErrorList += RequiredData('<%=ddlDenomination.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtSeries.ClientID %>', 'TextBox', "Series cannot be empty!");
            sErrorList += RequiredData('<%=txtFrom.ClientID %>', 'TextBox', "From cannot be empty!");
            sErrorList += RequiredData('<%=txtTo.ClientID %>', 'TextBox', "To cannot be empty!");
           // sErrorList += RequiredData('<%=txtDrawerNumber.ClientID %>', 'TextBox', "Drawer Number cannot be empty!");
           // sErrorList += RequiredData('<%=txtCabinetNumber.ClientID %>', 'TextBox', "Cabinet Number cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function DeleteReceiveDetail() {
            if (CheckForDelete('this receive detail')) {
                ResetDetailDataOnChangeDenom();
                return true;
            } else {
                return false;
            }
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function ResetDetailDataOnChangeDenom() {
            ResetData('<%=txtSeries.ClientID %>');
            ResetData('<%=txtFrom.ClientID %>');
            ResetData('<%=txtTo.ClientID %>');
            ResetData('<%=txtQuantity.ClientID %>');
            ResetData('<%=txtDnmAmount.ClientID %>');
            return false;
        }

        function ResetDetailData() {
            ResetData('<%=hdDenom.ClientID %>');
            ResetData('<%=txtSeries.ClientID %>');
            ResetData('<%=txtFrom.ClientID %>');
            ResetData('<%=txtTo.ClientID %>');
            ResetData('<%=txtQuantity.ClientID %>');
            ResetData('<%=txtDnmAmount.ClientID %>');
            ResetData('<%=txtCabinetNumber.ClientID %>');
            ResetData('<%=txtDrawerNumber.ClientID %>');

            ResetData('<%= ddlDenomination.ClientID %>');

            return false;
        }

        function ResetTotalData() {
            ResetData('<%=hdDenom.ClientID %>');
            ResetData('<%=txtSeries.ClientID %>');
            ResetData('<%=txtFrom.ClientID %>');
            ResetData('<%=txtTo.ClientID %>');
            ResetData('<%=txtQuantity.ClientID %>');

            ResetData('<%=hdReceiveTransNo.ClientID %>');
            ResetData('<%=txtTransNo.ClientID %>');
            ResetData('<%=txtReceiveDate.ClientID %>');
            ResetData('<%=ddlSPType.ClientID %>');
            ResetData('<%=txtReceiveAmount.ClientID %>');

            $("#<%=ddlDenomination.ClientID %> option").remove();
            $("#<%=gvReceiveDetail.ClientID %>").remove();
            //$("input:radio").removeAttr("checked");

            ResetUserDetails();

            var gridTb = document.getElementById('ctl00_cphDet_gvReceiveDetail');
            if (gridTb != null) {
                gridTb.innerHTML = "";
            }
            return false;
        }

        function Delete() {

            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }


            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdReceiveTransNo = document.getElementById('<%=hdReceiveTransNo.ClientID %>');
            if (hdReceiveTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Receive Data')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Receive transaction has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function leftPad(n, len) {
            return (new Array(len - String(n).length + 1)).join("0").concat(n);
        }
    
    </script>

</asp:Content>
