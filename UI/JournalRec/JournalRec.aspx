<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="JournalRec.aspx.cs" Inherits="SBM_WebUI.mp.JournalRec" %>


<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<%@ Register Src="~/UI/UC/UCReconBalance.ascx" TagName="JBalance" TagPrefix="uc3" %>
<%@ Register Src="~/UI/UC/UCReconAuto.ascx" TagName="JAuto" TagPrefix="uc3" %>
<%@ Register Src="~/UI/UC/UCRecon.ascx" TagName="JRecon" TagPrefix="uc3" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function ClearDateFields(objId1, objId2) {
            var obj1 = document.getElementById(objId1);
            if (obj1 != null) {
                obj1.value = "";
            }
            var obj2 = document.getElementById(objId2);
            if (obj2 != null) {
                obj2.value = "";
            }
            return false;
        }

        function ResetAll() {
            ResetData('<%= ddlSpType.ClientID %>');
            ResetData('<%= ddlAccNumbe.ClientID %>');
            ResetData('<%= ddlAmountCondition.ClientID %>');
            ResetData('<%= txtAmountCondition.ClientID %>');
            ResetData('<%= ddlJournalStatus.ClientID %>');
            ResetData('<%= ddlDrCr.ClientID %>');
            ClearDateFields('ctl00_cphDet_txtPaymentFromDate', 'ctl00_cphDet_txtPaymentToDate')
            ClearDateFields('ctl00_cphDet_txtReconFromDate', 'ctl00_cphDet_txtReconToDate')

            /*Detail Section*/
            ResetData('<%= txtTotalDebitAmount.ClientID %>');
            ResetData('<%= txtTotalCreditAmount.ClientID %>');
            ResetData('<%= txtTotalNoofDebitTransaction.ClientID %>');
            ResetData('<%= txtTotalNoofCreditTransaction.ClientID %>');
            ResetData('<%= txtTotalBalance.ClientID %>');
            /*end of Detail Section*/

            $("#<%=gvAccJournals.ClientID %>").remove();
            return false;
            
        }

        function rdlInitSetUp(obj) {

            var ddlSpType = document.getElementById('<%= ddlSpType.ClientID %>');
            var ddlAccNumbe = document.getElementById('<%= ddlAccNumbe.ClientID %>');
            var lblAccName = document.getElementById('<%= lblAccName.ClientID %>');
            var ddlAmountCondition = document.getElementById('<%= ddlAmountCondition.ClientID %>');
            var txtAmountCondition = document.getElementById('<%= txtAmountCondition.ClientID %>');
            var ddlJournalStatus = document.getElementById('<%= ddlJournalStatus.ClientID %>');
            var ddlDrCr = document.getElementById('<%= ddlDrCr.ClientID %>');
           
            var btnSearch = document.getElementById('<%=btnSearch.ClientID %>');

            ResetData('<%= ddlSpType.ClientID %>');
            ResetData('<%= ddlAccNumbe.ClientID %>');
            ResetData('<%= ddlAmountCondition.ClientID %>');
            ResetData('<%= txtAmountCondition.ClientID %>');
            ResetData('<%= ddlJournalStatus.ClientID %>');
            ResetData('<%= ddlDrCr.ClientID %>');
            ClearDateFields('ctl00_cphDet_txtPaymentFromDate', 'ctl00_cphDet_txtPaymentToDate')
            ClearDateFields('ctl00_cphDet_txtReconFromDate', 'ctl00_cphDet_txtReconToDate')

            /*Detail Section*/
            ResetData('<%= txtTotalDebitAmount.ClientID %>');
            ResetData('<%= txtTotalCreditAmount.ClientID %>');
            ResetData('<%= txtTotalNoofDebitTransaction.ClientID %>');
            ResetData('<%= txtTotalNoofCreditTransaction.ClientID %>');
            ResetData('<%= txtTotalBalance.ClientID %>');
            /*end of Detail Section*/

            if (obj.value == "Download_Journal") {
                ddlSpType.disabled = true;
                ddlAccNumbe.disabled = true;
                lblAccName.disabled = true;
                ddlAmountCondition.disabled = true;
                txtAmountCondition.disabled = true;
                ddlJournalStatus.disabled = true;
                ddlDrCr.disabled = true;
                btnSearch.value = "Download";

                BtnSetStatus('<%=btnReconcile.ClientID %>',false);
                BtnSetStatus('<%=btnMoveArchive.ClientID %>',false);
                BtnSetStatus('<%=btnMoveMain.ClientID %>',false);
                BtnSetStatus('<%=btnBalance.ClientID %>',false);
            } else if (obj.value == "Current_Journal") {
                ddlSpType.disabled = false;
                ddlAccNumbe.disabled = false;
                lblAccName.disabled = false;
                ddlAmountCondition.disabled = false;
                txtAmountCondition.disabled = false;
                ddlJournalStatus.disabled = false;
                ddlDrCr.disabled = false;
                btnSearch.value = "Search";

                BtnSetStatus('<%=btnReconcile.ClientID %>', true);
                BtnSetStatus('<%=btnMoveArchive.ClientID %>', true);
                BtnSetStatus('<%=btnMoveMain.ClientID %>', false);
                BtnSetStatus('<%=btnBalance.ClientID %>', true);
            } else if (obj.value == "Archives") {
                ddlSpType.disabled = false;
                ddlAccNumbe.disabled = false;
                lblAccName.disabled = false;
                ddlAmountCondition.disabled = false;
                txtAmountCondition.disabled = false;
                ddlJournalStatus.disabled = false;
                ddlDrCr.disabled = false;
                btnSearch.value = "Search";

                BtnSetStatus('<%=btnReconcile.ClientID %>', false);
                BtnSetStatus('<%=btnMoveArchive.ClientID %>', false);
                BtnSetStatus('<%=btnMoveMain.ClientID %>', true);
                BtnSetStatus('<%=btnBalance.ClientID %>', true);
            }
        }


        function ShowReconPopup() {
            var sErrorList = "";

            
            
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
                JReconPopupReturnTrue();
                return true;
            }
            return true;
            // end of show error divErroList
        }


        function SaveValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            return OpenErrorPanel(sErrorList, '');
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
    <%-- Error --%>
    
    
    <fieldset>
        <legend>Select Subject to download</legend>
        <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td width="20%" align="right">
                    Type
                </td>
                <td>
                    <asp:RadioButtonList RepeatDirection="Horizontal" runat="server" ID="rdlType" >
                        <asp:ListItem Text="Download Journal" Value="Download_Journal" ></asp:ListItem>
                        <asp:ListItem Text="Current Journal" Value="Current_Journal" ></asp:ListItem>
                        <asp:ListItem Text="Archives" Value="Archives"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Select a subject to download</legend>
        <table width="98%" align="center" cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td width="20%" align="right">
                    SP Type
                </td>
                <td width="30%">
                    <asp:DropDownList runat="server" ID="ddlSpType" SkinID="ddlMedium">
                    </asp:DropDownList>
                </td>
                <td width="20%" align="right">
                    Date
                </td>
                <td width="30%">
                    <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Account No
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlAccNumbe" SkinID="ddlMedium">
                    </asp:DropDownList>
                </td>
                <td align="right">
                    Account Name
                </td>
                <td>
                    <asp:Label runat="server" ID="lblAccName"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Amount
                </td>
                <td colspan="3">
                    <div class="fieldLeft">
                        <asp:DropDownList runat="server" ID="ddlAmountCondition" SkinID="ddlCommon" Width="40Px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="=" Value="="></asp:ListItem>
                            <asp:ListItem Text=">" Value=">"></asp:ListItem>
                            <asp:ListItem Text="<" Value="<"></asp:ListItem>
                            <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                            <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                            <asp:ListItem Text="<>" Value="<>"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtAmountCondition" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Payment From Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="154px" ID="txtPaymentFromDate"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Payment To Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtPaymentToDate"
                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Blank" ID="btnPaymentBlank"
                            OnClientClick="return  ClearDateFields('ctl00_cphDet_txtPaymentFromDate', 'ctl00_cphDet_txtPaymentToDate')" /></div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Recon From Date
                </td>
                <td>
                    <asp:TextBox runat="server" CssClass="textInput" Width="154px" ID="txtReconFromDate"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Recon To Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtReconToDate"
                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        <asp:Button CssClass="ButtonAsh" runat="server" Text="Blank" ID="btnReconBlank" OnClientClick="return ClearDateFields('ctl00_cphDet_txtReconFromDate', 'ctl00_cphDet_txtReconToDate')" /></div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Status
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlJournalStatus" SkinID="ddlMedium">
                    </asp:DropDownList>
                </td>
                <td align="right">
                    Dr / Cr
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlDrCr" SkinID="ddlMedium">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="Dr" Value="Dr"></asp:ListItem>
                        <asp:ListItem Text="Cr" Value="Cr"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkAscending" Text="Ascending" />
                </td>
                <td>
                </td>
                <td>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" 
                        onclick="btnSearch_Click" OnClientClick="return SaveValidation()" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return ResetAll()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend>Journals</legend>
       <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
        <asp:GridView Style="width: 98%" ID="gvAccJournals" runat="server" AutoGenerateColumns="False"
            SkinID="SBMLGridGreen" ShowHeader="true" AllowPaging="True">
            <Columns>
                <asp:BoundField DataField="SPTypeID" HeaderText="Sp Type" />
                <asp:BoundField DataField="ReferenceNo" HeaderText="Ref No" />
                <asp:BoundField DataField="ValueDate" HeaderText="Trans Date" DataFormatString="{0: dd-MMM-yyyy}" />
                <asp:BoundField DataField="AccountNo" HeaderText="Account No" />
                <asp:BoundField DataField="Narration" HeaderText="Narration" />
                <asp:BoundField DataField="CurrencyID" HeaderText="Currency" />
                <asp:BoundField DataField="DrCr" HeaderText="DrCr" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" />
                <asp:BoundField DataField="MakerID" HeaderText="Maker" />
                <asp:TemplateField HeaderText="Reconciled" HeaderStyle-Width="5%">
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="chkReconciled" Enabled='<%# ((bool)Eval("IsReconciled"))==true? false : true %>'  Checked='<%# Eval("IsReconciled") %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="5%" />
                </asp:TemplateField>
                <asp:BoundField DataField="ReceiveDate" HeaderText="Recon Date" DataFormatString="{0: dd-MMM-yyyy}" />
                <asp:BoundField DataField="ReconBy" HeaderText="Recon By" />
                <asp:BoundField DataField="DownLoadBy" HeaderText="Download By" />
            </Columns>
            <EmptyDataTemplate>
                No record found
            </EmptyDataTemplate>
        </asp:GridView>
        </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Reconciliation Specification</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
        <table border="0" width="98%" align="center" cellpadding="2">
            <tr>
                <td align="right">
                    Total Debit Amount
                </td>
                <td>
                    <asp:TextBox ID="txtTotalDebitAmount" Width="100px" CssClass="textInput" runat="server"
                        Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Total Credit Amount
                </td>
                <td>
                    <asp:TextBox ID="txtTotalCreditAmount" Width="100px" CssClass="textInput" runat="server"
                        Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Total Balance
                </td>
                <td>
                    <asp:TextBox ID="txtTotalBalance" Width="100px" CssClass="textInput" runat="server"
                        Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Total No. of Debit Transaction
                </td>
                <td>
                    <asp:TextBox ID="txtTotalNoofDebitTransaction" Width="100px" CssClass="textInput"
                        runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Total No. of Credit Transaction
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txtTotalNoofCreditTransaction" Width="100px" CssClass="textInput"
                        runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
        </table>
        </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        
                <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="center">
                            <asp:Button runat="server" CssClass="ButtonAsh" Text="Reconcile" ID="btnReconcile"
                                OnClick="btnReconcile_Click" OnClientClick="return ShowReconPopup()" />
                            &nbsp;
                            <asp:Button runat="server" CssClass="ButtonAsh" Text="Move to Archive" ID="btnMoveArchive" OnClick="btnMoveArchive_Click" OnClientClick=" MsgPopupReturnTrue('Approve') "  />
                            &nbsp;
                            <asp:Button runat="server" CssClass="ButtonAsh" Text="Move to Main" ID="btnMoveMain" OnClick="btnMoveMain_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                            &nbsp;
                            <asp:Button runat="server" CssClass="ButtonAsh" Text="Balance" ID="btnBalance" OnClientClick="return JBalancePopupReturnTrue()" />
                            &nbsp;
                            <asp:Button runat="server" CssClass="ButtonAsh" Text="Auto Recon" ID="btnAutoRecon" onclick="btnAutoRecon_Click"  OnClientClick="return JAutoPopupReturnTrue()"  />
                        </td>
                    </tr>
                </table>
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReconcile" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnAutoRecon" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <uc3:JBalance ID="JBalance" runat="server" />
    <uc3:Error ID="ucMessage" runat="server" />
    <uc3:JAuto ID="JAuto" runat="server" />
    <UC3:JRecon id="J" runat="server" />
    
    <script language ="javascript" type="text/javascript" >
        var oInit = document.getElementById("ctl00_cphDet_rdlType_0");
        oInit.checked = true;
        rdlInitSetUp(oInit);
    </script>
</asp:Content>
