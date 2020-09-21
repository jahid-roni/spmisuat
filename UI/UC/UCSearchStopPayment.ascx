<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSearchStopPayment.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCSearchStopPayment" %>
<div id="MDStopPaymentSearch" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                    </div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_StopPay_MDStopPaymentSearch');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 450px; overflow: auto;">
                    <asp:HiddenField runat="server" ID="hdPageType" />
                    <fieldset>
                        <legend>Search Criteria</legend>
                        <table width="550px" align="center" class="tableBody" border="0" cellpadding="3">
                            <tr>
                                <td align="right">
                                    Registration No
                                </td>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                                        <ContentTemplate>
                                            &nbsp;&nbsp;<asp:TextBox ID="txtRegNo" Width="140px" CssClass="textInput" runat="server"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                MaxLength="25"></asp:TextBox>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td align="right">
                                    SP Type
                                </td>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddlSPType" SkinID="ddlMedium" runat="server">
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label runat="server" ID="lblDate"></asp:Label>
                                </td>
                                <td>
                                    &nbsp;<asp:CheckBox runat="server" ID="chkDateWise" Text="Search Date Wise" onClick="CheckLastDate(this)">
                                    </asp:CheckBox>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox runat="server" ID="txtIssueDate" Width="140px" CssClass="textInput"
                                        Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Approval Status
                                </td>
                                <td colspan="3">
                                    <asp:RadioButtonList runat="server" ID="rdolApproval" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="Ready For Approve&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Approved" Value="2"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <br />
                    <fieldset>
                        <asp:UpdatePanel runat="server" ID="upSuccess">
                            <ContentTemplate>
                                <asp:HiddenField runat="server" ID="hdCheckDate" />
                                <table width="550" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="center">
                                            <asp:Label runat="server" ID="lblProgress"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnSearch" OnClientClick="ShowProgressStatus('ctl00_cphDet_StopPay_lblProgress') "
                                                OnClick="btnSearch_Click" />
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </fieldset>
                    <br />
                    <fieldset runat="server" id="fsList">
                        <legend>Search Results</legend>
                        <table width="100%" align="center" class="tableBody" border="0">
                            <tr>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="upGv">
                                        <ContentTemplate>
                                            <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                OnRowCommand="gvSearchList_RowCommand" SkinID="SBMLGridGreen" ShowHeader="true"
                                                OnRowDataBound="gvSearchList_RowDataBound" AllowPaging="true" OnPageIndexChanging="gvSearchList_PageIndexChanging">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                                OnClientClick="hideModal('ctl00_cphDet_StopPay_MDStopPaymentSearch')" Text="Select" />
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
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">        
        
    function CheckLastDate(obj) {
        var txtIssueDate = document.getElementById('<%=txtIssueDate.ClientID %>');
        var hdCheckDate = document.getElementById('<%=hdCheckDate.ClientID %>');
        hdCheckDate.value = obj.checked;
        if (obj.checked) {
            txtIssueDate.disabled = false;
        } else {
            txtIssueDate.disabled = true;
        }
    }
    

    function StopPaymentSearchPopupReturnTrue(sPageType) {
        $("#<%=gvSearchList.ClientID %>").remove();        
        $("#<%=hdPageType.ClientID %>").val(sPageType);
        
        // 1.1 StopPayment
        // 2.1 Remove: Trans Search
        // 2.2 Remove: Stop Trans Search
        // 3.1 Dup: Remove Trans Search
        // 3.2 Dup: Stop Trans Search
        // 4.2 Lien remove Trans Search
        // 5.2 Update Issue Trans Search

        showModal("ctl00_cphDet_StopPay_MDStopPaymentSearch");
        var olblDate = document.getElementById("ctl00_cphDet_StopPay_lblDate");
        var rdolApproval = document.getElementById("ctl00_cphDet_StopPay_rdolApproval_0");
        var rdolApproval_1 = document.getElementById("ctl00_cphDet_StopPay_rdolApproval_1");
        var vTitle = $("#<%=lblTitle.ClientID %>")


        if (sPageType == "1.1") {
            olblDate.innerHTML = "Date";
        } else if (sPageType == "2.1") {
            rdolApproval.disabled = false;
            rdolApproval.checked = true;
            rdolApproval_1.checked = false;
            olblDate.innerHTML = "Remove Date YN";
            if (vTitle != null) {
                vTitle.text("Stop Payment Remove Search");
            }
        } else if (sPageType == "2.2") {
            rdolApproval.disabled = true;
            rdolApproval.checked = false;
            rdolApproval_1.checked = true;
            olblDate.innerHTML = "Date";
        } else if (sPageType == "3.1") {
            rdolApproval.disabled = false;
            rdolApproval.checked = true;
            rdolApproval_1.checked = false;
            olblDate.innerHTML = "Issue Date YN";
            if (vTitle != null) {
                vTitle.text("Duplicate Issue Search");
            }
        } else if (sPageType == "3.2") {
            rdolApproval.disabled = true;
            rdolApproval.checked = false;
            rdolApproval_1.checked = true;
            olblDate.innerHTML = "Date";
            if (vTitle != null) {
                vTitle.text("Stop Payment Search");
            }
        } else if (sPageType == "4.2") {
            rdolApproval.disabled = false;
            rdolApproval.checked = true;
            rdolApproval_1.checked = false;
            olblDate.innerHTML = "Un Lien Date";
            if (vTitle != null) {
                vTitle.text("Lien Remove Search");
            }
        } else if (sPageType == "5.2") {
            rdolApproval.disabled = false;
            rdolApproval.checked = true;
            rdolApproval_1.checked = false;
            olblDate.innerHTML = "";
            if (vTitle != null) {
                vTitle.text("Issue Update Search");
            }
        }
        
        CloseErrorPanel();
        return false;
    }
</script>

