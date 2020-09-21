<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SystemConfigSetup.aspx.cs" Inherits="SystemConfigSetup" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });

        function Reset() {           
            ResetData('<%=txtCustomerDataFile.ClientID %>');
            ResetData('<%=txtAccountDataFile.ClientID %>');
            ResetData('<%=txtAllJournal.ClientID %>');
            ResetData('<%=txtCommissionClaimJournalFile.ClientID %>');
            ResetData('<%=txtCommissionReimburseJournalFile.ClientID %>');
            ResetData('<%=txtEncashmentJournal.ClientID %>');
            ResetData('<%=txtEncashmentReimburseJournal.ClientID %>');
            ResetData('<%=txtInterestPaymentJournalFile.ClientID %>');
            ResetData('<%=txtInterestReimburseJournalFile.ClientID %>');
            ResetData('<%=txtIssueJournalFile.ClientID %>');
            ResetData('<%=txtReceiveJournalFile.ClientID %>');
            ResetData('<%=txtReconDataFile.ClientID %>');
            ResetData('<%=txtSalesStatementJournalFile.ClientID %>');

            ResetData('<%=txtOrigID.ClientID %>');
            ResetData('<%=txtRowTypeHeader.ClientID %>');
            ResetData('<%=txtRowTypeFooter.ClientID %>');
            ResetData('<%=txtDrTranCode.ClientID %>');
            ResetData('<%=txtDrCode.ClientID %>');
            ResetData('<%=txtCrCode.ClientID %>');
            ResetData('<%=txtCrTran.ClientID %>');

            ResetUserDetails();
            CloseErrorPanel();
            
            return false;
        }
        
        function SaveValidation() { 
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtCustomerDataFile.ClientID %>', 'TextBox', "Customer Data File (BDDB1) cannot be empty!");
            sErrorList += RequiredData('<%=txtAccountDataFile.ClientID %>', 'TextBox', "Account Data File (BDDB2) cannot be empty!");
            sErrorList += RequiredData('<%=txtAllJournal.ClientID %>', 'TextBox', "All Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtCommissionClaimJournalFile.ClientID %>', 'TextBox', "Commission Claim Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtCommissionReimburseJournalFile.ClientID %>', 'TextBox', "Commission Reimburse Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtEncashmentJournal.ClientID %>', 'TextBox', "Encashment Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtEncashmentReimburseJournal.ClientID %>', 'TextBox', "Encashment Reimburse Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtInterestPaymentJournalFile.ClientID %>', 'TextBox', "Interest Payment Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtInterestReimburseJournalFile.ClientID %>', 'TextBox', "Interest Reimburse Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtIssueJournalFile.ClientID %>', 'TextBox', "Issue Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtReceiveJournalFile.ClientID %>', 'TextBox', "Reconciliation TXN Download File cannot be empty!");
            sErrorList += RequiredData('<%=txtReconDataFile.ClientID %>', 'TextBox', "Receive Journal File cannot be empty!");
            sErrorList += RequiredData('<%=txtSalesStatementJournalFile.ClientID %>', 'TextBox', "Sales Statement Journal File cannot be empty!");

            sErrorList += RequiredData('<%=txtOrigID.ClientID %>', 'TextBox', "Originator ID cannot be empty!");
            sErrorList += RequiredData('<%=txtRowTypeHeader.ClientID %>', 'TextBox', "Row Type Header cannot be empty!");
            sErrorList += RequiredData('<%=txtRowTypeFooter.ClientID %>', 'TextBox', "Row Type Footer cannot be empty!");
            sErrorList += RequiredData('<%=txtDrTranCode.ClientID %>', 'TextBox', "Dr Transaction Code cannot be empty!");
            sErrorList += RequiredData('<%=txtDrCode.ClientID %>', 'TextBox', "Dr Code cannot be empty!");
            sErrorList += RequiredData('<%=txtCrCode.ClientID %>', 'TextBox', "Cr Code cannot be empty!");
            sErrorList += RequiredData('<%=txtCrTran.ClientID %>', 'TextBox', "Cr Transaction Code cannot be empty!");

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
                MsgPopupReturnTrue('Save');
                return true;
            }
            // end of show error divErroList
            
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

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
                MsgPopupReturnTrue('Reject');
                return true;
            }
            // end of show error divErroList
        }
    
    
    
     function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdCustomerDataFileID = document.getElementById('<%=hdCustomerDataFileID.ClientID %>');
            if (hdCustomerDataFileID.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this System Configuration')) {
                    Reset();
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>System Configuration file has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }
        }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Hidden Field --%>
    <%-- Error --%>
    
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>

    
     <fieldset runat="server" id="fsList">
        <legend>Unapproved System Config File Data List</legend>
        <table width="100%" align="center" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                        <ContentTemplate>
                        <asp:HiddenField runat="server" ID="hdCustomerDataFileID" />
                        <asp:HiddenField runat="server" ID="hdDataType" />
                            <asp:GridView OnRowCommand="gvList_RowCommand" AutoGenerateColumns="true"
                                Width="98%" OnRowDataBound="gvList_RowDataBound" ID="gvList" runat="server" SkinID="SBMLGridGreen">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select"  OnClientClick="CloseErrorPanel()" />
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
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    
    
    <fieldset>
        <legend>System Configaration File Location</legend>
        <asp:UpdatePanel runat="server" ID="upGv">
            <ContentTemplate>
          
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td width="25%" align="right">
                            Customer Data File (BDDB1)
                        </td>
                        <td width="25%"> <div class="fieldLeft">
                            <asp:TextBox runat="server" ID="txtCustomerDataFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td width="25%" align="right">
                            Account Data File (BDDB2)
                        </td>
                        <td width="25%"> <div class="fieldLeft">
                            <asp:TextBox runat="server" ID="txtAccountDataFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Reconciliation TXN Download File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox runat="server" ID="txtReconDataFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="right">
                            Receive Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtReceiveJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Issue Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtIssueJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="right">
                            Sales Statement Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtSalesStatementJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Commission Claim Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtCommissionClaimJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="right">
                            Commission Reimburse Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtCommissionReimburseJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Interest Payment Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtInterestPaymentJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="right">
                            Interest Reimburse Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtInterestReimburseJournalFile" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Encashment Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtEncashmentJournal" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="right">
                            Encashment Reimburse Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtEncashmentReimburseJournal" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            All Journal File
                        </td>
                        <td><div class="fieldLeft">
                            <asp:TextBox  runat="server" ID="txtAllJournal" MaxLength="100" Width="160px" CssClass="textInput" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox>
                            </div><div class="errorIcon">*</div>
                        </td>
                        <td align="left">
                            <asp:Button CssClass="ButtonAsh" ID="btnLoad" runat="server" 
                                Text="Load Approved Data" onclick="btnLoad_Click" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
           </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Upload File Configuration</legend>
        <asp:UpdatePanel runat="server" ID="upData">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Originator ID
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="4" Width="120px" runat="server" CssClass="textInput" ID="txtOrigID"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Row Type Header
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="2" Width="120px" runat="server" CssClass="textInput" ID="txtRowTypeHeader"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Row Type Footer
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="2" Width="120px" runat="server" CssClass="textInput" ID="txtRowTypeFooter"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Dr Transaction Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="3" Width="120px" runat="server" CssClass="textInput" ID="txtDrTranCode"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Dr Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="1" Width="120px" runat="server" CssClass="textInput" ID="txtDrCode"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Cr Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="1" Width="120px" runat="server" CssClass="textInput" ID="txtCrCode"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Cr Transaction Code
                        </td>
                        <td colspan="5">
                            <div class="fieldLeft">
                                <asp:TextBox MaxLength="3" Width="120px" runat="server" CssClass="textInput" ID="txtCrTran"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--Start to User Comments--%>
   <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
       </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="return Reset()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()" OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>
