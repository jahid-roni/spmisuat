<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SPReinvestment.aspx.cs" Inherits="SBM_WebUI.mp.SPReinvestment" %>

<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCViewGridData.ascx" TagName="VGData" TagPrefix="uc4" %>
<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchReinvest.ascx" TagName="Reinvest" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="../UC/UCCustomerDetail_HSB.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

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
        <legend runat="server" id="lgText">Unapproved Reinvestment List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdReinvestRefNo" />
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <asp:HiddenField runat="server" ID="hdSupportdGndr" Value="" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                OnRowDataBound="gvData_RowDataBound" ID="gvData" runat="server" SkinID="SBMLGridGreen"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend runat="server" id="Legend1">Issue Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="1" cellspacing="1"
                    border="0">
                    <tr>
                        <td align="right">
                            Reference No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRefNo" ReadOnly="true" Width="160px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" MaxLength="20" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnRefNo" OnClientClick="return ReinvestmentPopupReturnTrue()" /></div>
                        </td>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" Enabled="false" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" Enabled="false" Width="120px" runat="server" SkinID="ddlCommon">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            New Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtNewRegNo" Width="160px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    MaxLength="25" onfocus="highlightActiveInputWithObj(this)" TabIndex="1"></asp:TextBox></div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnNewRegNo" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtNewRegNo')"
                                    OnClick="btnNewRegNo_Click" TabIndex="2" /></div>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtIssueDate" ReadOnly="true" Width="160px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Maturity Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtMaturityDate" ReadOnly="true" Width="113px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Old Reg No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtOldRegNo" Width="160px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    MaxLength="25" onfocus="highlightActiveInputWithObj(this)" AutoPostBack="True" OnTextChanged="txtOldRegNo_TextChanged"></asp:TextBox></div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearchOldRegNo"
                                    OnClick="btnSearchOldRegNo_Click" OnClientClick="return CheckOldRegNo()" /></div>
                        </td>
                        <td align="right">
                            Reinvestment Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtReinvestmentDate" Width="160px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                ReadOnly="True"></asp:TextBox>
                        </td>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCustomerType" Enabled="false" Width="120px" runat="server"
                                SkinID="ddlCommon">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" align="left">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="History Detail" ID="btnHistoryDetail"
                                OnClientClick="return ShowVGDataShow()" OnClick="btnHistoryDetail_Click" />
                            &nbsp;<asp:Button CssClass="ButtonAsh" runat="server" Text="Limit Status" ID="btnLimitStatus"
                                OnClientClick="NotDoneYet()" />
                            &nbsp;<asp:Button CssClass="ButtonAsh" runat="server" Text="Print Limit" ID="btnPrintLimit"
                                OnClientClick="NotDoneYet()" />
                            &nbsp;<asp:Button CssClass="ButtonAsh" runat="server" Text="Show Policy" ID="btnShowPolicy"
                                OnClientClick="return ShowPolicyDetail()" OnClick="btnShowPolicy_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <!-- start to 3 Tab panel  -->
    <fieldset>
        <legend>Customer(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                <ContentTemplate>
                    <div style="height: 70%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustmerDetail" runat="server"
                            AutoGenerateColumns="true" SkinID="SBMLGridGreen" OnRowCommand="gvCustomerDetail_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnCusDetailSelect"
                                            Text="Select" OnClientClick="return CustomerDetailPopupReturnTrue()" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Customer(s) Details Tab  --%>
        <%-- Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Nominee(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    
                    <div style="height: 40%; width: 100%; overflow: auto;">
                        <table width="100%" align="center" class="tableBody" border="0" cellpadding="1" cellspacing="1">
                            <tr>
                                <td align="right" valign="top">
                                    Name
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="300px" MaxLength="100" TabIndex="3"></asp:TextBox>
                                </td>
                                <td align="right" valign="top">
                                    Share
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDShare" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Share')"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px" MaxLength="5" TabIndex="6"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnNDAdd" runat="server" CssClass="ButtonAsh" Text="Add" Width="80px"
                                        OnClick="btnNDAdd_Click" OnClientClick="return SaveNomineeValidation()" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    Relation
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDRelation" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="200px" MaxLength="20" TabIndex="4"></asp:TextBox>
                                </td>
                                <td align="right" valign="top">
                                    Amount
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDAmount" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Amount')"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px" MaxLength="12" TabIndex="7"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnNDReset" runat="server" CssClass="ButtonAsh" Text="Reset" Width="80px"
                                        OnClientClick="resetNomineeDetatil()" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    Address
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDAddress" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="300px" MaxLength="150" TabIndex="5"></asp:TextBox>
                                </td>
                                <td align="right" valign="top">
                                    Data Of Birth
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNDBirthDate" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Share')"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px" MaxLength="3" TabIndex="8"></asp:TextBox>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdNomSlno" Value="" runat="server" />
                    </div>
                    <div style="height: 60%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDetail" runat="server" AutoGenerateColumns="false"
                            SkinID="SBMLGridGreen" OnRowDataBound="gvNomDetail_RowDataBound" OnRowCommand="gvNomDetail_RowCommand"
                            OnRowDeleting="gvNomDetail_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnNDSelect"
                                            Text="Select" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="5%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnNDDelete"
                                            Text="Delete" OnClientClick="return CheckForDelete()" />
                                        <asp:HiddenField ID="hdNomineeSlno" Value='<%# Eval("SlNo") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="5%" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Nominee Name" HeaderText="Nominee Name" />
                                <asp:BoundField DataField="Relation" HeaderText="Relation" />
                                <asp:BoundField DataField="Address" HeaderText="Address" />
                                <asp:BoundField DataField="Nominee Share" HeaderText="Share" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                <asp:BoundField DataField="DateOfBirth" HeaderText="Date Of Birth" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnNDAdd" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
        <%-- Nominee(s) Wise Denomination Tab  --%>
        <%--<div class="tabbertab">
            <h2>
                Nominee(s) Wise Denomination</h2>
            <div style="width: 100%; overflow: auto;">
                <table>
                    <tr>
                        <td>
                            Name
                        </td>
                        <td>
                            Denomination
                        </td>
                        <td>
                            Quantity
                        </td>
                        <td colspan="2">
                            Amount
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlNWDName" SkinID="ddlBig" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlNWDDenom" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNWDQuantity" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNWDAmount" ReadOnly="true" runat="server" CssClass="textInput"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Width="100px"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnNWDAdd" runat="server" CssClass="ButtonAsh" Text="Add" Width="80px" />
                            <asp:Button ID="btnNWDDelete" runat="server" CssClass="ButtonAsh" Text="Delete" Width="80px" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDemon" runat="server" AutoGenerateColumns="False"
                            SkinID="SBMLGridGreen">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnSelect"
                                            Text="Select" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="5%" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Val1" HeaderText="Nominee Name" />
                                <asp:BoundField DataField="Val1" HeaderText="Denomination" />
                                <asp:BoundField DataField="Val1" HeaderText="Quantity" />
                                <asp:BoundField DataField="Val1" HeaderText="Amount" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>--%>
        <%-- End of Nominee(s) Wise Denomination Tab  --%>
    <!--</div> -->
    <!-- End of 3 Tab panel  -->
    <!-- Start to Bond Holder Details panel-->
    <br />
    <fieldset>
        <legend>Bond Holder Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0" cellpadding="3">
                    <tr>
                        <td align="right">
                            Holder Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDHolderName" MaxLength="50" Width="500px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Relation
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDRelation" MaxLength="50" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Issue Name
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtIssueName" ReadOnly="true" Width="500px" CssClass="textInput"
                                runat="server" TextMode="MultiLine" Height="35px" onblur="blurActiveInputWithObj(this)"
                                MaxLength="100" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" valign="top" rowspan="2">
                            Master No
                        </td>
                        <td valign="top" rowspan="2">
                            <asp:TextBox ID="txtMasterNo" Width="150px" MaxLength="9" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" OnTextChanged="txtMasterNo_TextChanged"
                                AutoPostBack="true" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox><br />
                            <asp:Label runat="server" ID="lblMasterVarified" Text="Not Verified Yet"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            Address
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDAddress" MaxLength="255" Width="500px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                TextMode="MultiLine" Height="50px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Scrip Detail</legend>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td width="30%" valign="top">
                            Old Certificate Details<div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvCertDetail" runat="server" AutoGenerateColumns="true"
                                    SkinID="SBMLGridGreen" ShowHeader="true">
                                    <Columns>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                        <td width="40%" valign="top">
                            <table width="98%" align="center" class="tableBody" cellpadding="2" cellspacing="2"
                                border="0">
                                <tr>
                                    <td align="right">
                                        Total Amount
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTotalAmount" ReadOnly="true" Width="100px" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Selected Amount
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSelectedAmount" ReadOnly="true" Width="100px" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Denomination
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlDenomination" Width="100px" SkinID="ddlCommon" runat="server">
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
                                            <asp:TextBox ID="txtQuantity" MaxLength="4" Width="100px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button CssClass="ButtonAsh" Width="80px" runat="server" Text="Add" ID="btnDenomAdd"
                                            OnClientClick="return SaveDenomValidation()" OnClick="btnDenomAdd_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td width="30%" valign="top">
                            Reinvested Denomination Details<br />
                            <div style="height: 200px; width: 100%; overflow: auto;">
                                <asp:GridView Style="width: 98%" ID="gvDenDetail" runat="server" AutoGenerateColumns="true"
                                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvDenDetail_RowCommand"
                                    OnRowDeleting="gvDenDetail_RowDeleting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Button CssClass="ButtonAsh" CommandName="Remove" runat="server" ID="btnRemove"
                                                    OnClientClick="return CheckForDelete('this Reinvestment Denomination!')" Text="Remove" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <!-- end of Denomination(s) details   &   Payment Details -->
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
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" OnClientClick="return NotDoneYet()" runat="server"
                        Text="View Journals" ID="Button9" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click"
                        OnClientClick="return DeleteReinvestment()" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:CustomerDetails ID="CustomerDetail" runat="server" />
    <uc4:Reinvest ID="RIS" runat="server" Type="21" />
    <uc3:Error ID="ucMessage" runat="server" />
    <uc4:VGData ID="VGD" runat="server" />
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" type="12" />

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDDQuantity]').keypress(function(e) { return intNumber(e); });

            $('input[id*=txtNDShare]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtNDAmount]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtTotalAmount]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtAppliedAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtQuantity]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtSelectedAmount]').keypress(function(e) { return intNumber(e); });
        }


        function DeleteReinvestment() {

            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }

            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdRegNo = document.getElementById('<%=hdRegNo.ClientID %>');
            if (hdRegNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Reinvestment')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Reinvestment has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function resetNomineeDetatil() {
            $("#<%=txtNDName.ClientID %>").val('');
            $("#<%=txtNDRelation.ClientID %>").val('');
            $("#<%=txtNDAddress.ClientID %>").val('');
            $("#<%=txtNDShare.ClientID %>").val('');
            $("#<%=txtNDAmount.ClientID %>").val('');
            $("#<%=hdNomSlno.ClientID %>").val('');
            CloseErrorPanel();
        }

        function CheckOldRegNo() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=txtOldRegNo.ClientID %>', 'TextBox', "Old Registration No cannot be empty!");

            return OpenErrorPanel(sErrorList, '');
        }

        function SaveNomineeValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtNDName.ClientID %>', 'TextBox', "Nominee Name cannot be empty!");
            //            sErrorList += RequiredData('<%=txtNDRelation.ClientID %>', 'TextBox', "Relation cannot be empty!");
            //            sErrorList += RequiredData('<%=txtNDAddress.ClientID %>', 'TextBox', "Address cannot be empty!");
            //            sErrorList += RequiredData('<%=txtNDShare.ClientID %>', 'TextBox', "Share cannot be empty!");
            //            sErrorList += RequiredData('<%=txtNDAmount.ClientID %>', 'TextBox', "Amount cannot be empty!");

            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');
            if (oNDShare.value != "") {
                if (parseInt(oNDShare.value, 10) > 100) {
                    sErrorList += "<li>Total amount of share cannot be exceeded 100!</li>";
                    oNDAmount.value = "";
                }
            }
            return OpenErrorPanel(sErrorList, '');
        }

        function SaveDenomValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('<%=ddlDenomination.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtQuantity.ClientID %>', 'TextBox', "Quantity cannot be empty!");

            var oTotalAmount = document.getElementById('<%=txtTotalAmount.ClientID %>');
            var oSelectedAmount = document.getElementById('<%=txtSelectedAmount.ClientID %>');
            var oDenomination = document.getElementById('<%=ddlDenomination.ClientID %>');
            var oQuantity = document.getElementById('<%=txtQuantity.ClientID %>');

            if (oTotalAmount != null && oSelectedAmount != null && oDenomination != null && oQuantity != null) {
                if (oQuantity.value != "0" && oQuantity.value != "") {
                    var vTotalAmount = parseInt(oTotalAmount.value, 10);
                    var vSelectedAmount = parseInt(oSelectedAmount.value, 10);
                    var vDenomination = parseInt(oDenomination.value, 10);
                    var vQuantity = parseInt(oQuantity.value, 10);
                    var vTotalCurrent = vDenomination * vQuantity;
                    var vTotalCurrent = vTotalCurrent + vSelectedAmount;
                    if (vTotalAmount < vTotalCurrent) {
                        sErrorList += "<li>You cannot exceeded total amount </li>";
                    }
                }
            }
            return OpenErrorPanel(sErrorList, '');
        }

        function SaveValidation() {
            var checkDataType = document.getElementById('<%=hdDataType.ClientID %>');
            if (checkDataType != null) {
                if (checkDataType.value == "2") {
                    return true;
                }
            }
            var sErrorList = "";
            var rowsGvgvDenDetail = $("#<%=gvDenDetail.ClientID %> tr").length;
            if (rowsGvgvDenDetail == 1 || rowsGvgvDenDetail == 0) {
                sErrorList += "<li>Denomination Detail cannot be empty</li>";
            }
            var objHolderName = document.getElementById('<%=txtBHDHolderName.ClientID %>')
            if (objHolderName.disabled == false) {
                sErrorList += RequiredData('<%=txtBHDHolderName.ClientID %>', 'TextBox', "Holder Name cannot be empty!");
            }
            return OpenErrorPanel(sErrorList, 'Save');
        }

        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function ShowPolicyDetail() {
            var reg = document.getElementById("<%=ddlSpType.ClientID %>");
            if (reg != null) {
                if (reg.selectedIndex == 0) {
                    var divErrorPanel = document.getElementById('divErrorPanel');
                    var errorList = document.getElementById('divErrorList');
                    errorList.innerHTML = "<ul><li>You must select SP Type first before viewing policy Detail!</li></ul>";
                    divErrorPanel.style.display = "block";
                    window.scroll(0, 0);
                    return false;
                } else {
                    CloseErrorPanel();
                    PolicyDetailPopupReturnTrue();
                    return true;
                }
            } else {
                return false;
            }
        }

        function ShowVGDataShow() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=hdRegNo.ClientID %>', 'TextBox', "You must load Reinvestment first before viewing detail");

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


        function CalculateAmount(vType) {
            var oAmount = document.getElementById('<%=txtTotalAmount.ClientID %>');
            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');

            var vTotalAmount = "";
            if (oAmount.value != "") {
                vTotalAmount = oAmount.value.replace(/,/g, '');
                CloseErrorPanel();
            }
            else {
                OpenErrorPanel("<li>Total Amount cannot be empty!</li>", "");
                oNDShare.value = "";
                oNDAmount.value = "";
                return false;
            }
            vTotalAmount = parseInt(vTotalAmount, 10);


            // this is by total amount
            if (vType == "Amount") {
                if (oNDAmount.value != "") {
                    if (parseInt(oNDAmount.value, 10) > parseInt(oAmount.value, 10)) {
                        OpenErrorPanel("<li>Total Nominee amount cannot be exceeded from Total Amount</li>", "");
                        oNDShare.value = "";
                        return false;
                    }
                    var vNDAmount = parseInt(oNDAmount.value, 10);
                    var vFinalVal = (parseFloat(vNDAmount) * 100) / parseFloat(vTotalAmount);
                    vFinalVal = vFinalVal.toFixed(0);
                    oNDShare.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDShare.value = "";
                    return false;
                }
            }

            // this is by share amount
            else if (vType == "Share") {
                if (oNDShare.value != "") {
                    if (parseInt(oNDShare.value, 10) > 100) {
                        OpenErrorPanel("<li>Total amount of share cannot be exceeded 100!</li>", "");
                        oNDAmount.value = "";
                        return false;
                    }
                    var vShare = parseInt(oNDShare.value, 10);
                    var vFinalVal = (parseFloat(vShare) * parseFloat(vTotalAmount)) / 100;
                    vFinalVal = vFinalVal.toFixed(2);

                    oNDAmount.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDAmount.value = "";
                    return false;
                }
            }
        }
        
    </script>

</asp:Content>
