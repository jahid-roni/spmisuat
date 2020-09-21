<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRecon.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCRecon" %>
<div id="MDRecon" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Data Download Reconciliation</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_J_MDRecon');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 500px; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Search Criteria</legend>
                                <table width="650px" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="right">
                                            Search Text
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSearchText" Width="240px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" MaxLength="20"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Check" ID="btnCheck" OnClientClick="ShowProgressStatus('ctl00_cphDet_SIssue_lblProgress') "
                                                OnClick="btnCheck_Click" />
                                        </td>
                                        <td>
                                            <asp:CheckBox runat="server" ID="chkAscending" Text="Ascending" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <table width="550" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="center">
                                            <asp:Label runat="server" ID="lblProgress"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Journals</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td>
                                        <div style="height: 200px; width: 100%; overflow: auto;">
                                            <asp:GridView AutoGenerateColumns="false" Width="98%" ID="gvData" runat="server"
                                            SkinID="SBMLGridGreen">
                                            <Columns>
                                                <asp:BoundField DataField="SPTypeID" ItemStyle-Width="99px" HeaderText="SPType." />
                                                <asp:BoundField DataField="AccountNo" ItemStyle-Width="78px" HeaderText="AccountNo." />
                                                <asp:BoundField DataField="Narration" ItemStyle-Width="60px" ItemStyle-Wrap="true"
                                                    HeaderText="Narration." HtmlEncode="false" />
                                                <asp:BoundField DataField="CurrencyID" ItemStyle-Width="78px" HeaderText="Currency." />
                                                <asp:BoundField DataField="DrCr" ItemStyle-Width="78px" HeaderText="DrCr" />
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <asp:TextBox runat="server" Width="90px" Text='<%# DataBinder.Eval(Container.DataItem, "Amount").ToString()%>' ID="txtAmount"
                                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                            CssClass="textInput" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hdDrCr" Value='<%# Eval("DrCr") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ref No">
                                                    <ItemStyle HorizontalAlign="Left" Width="50px" />
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRefNo" onClick="" OnChange=""
                                                            runat="server" Width="150px" Enabled="true" Text='<%# Eval("ReferenceNo") %>'
                                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                            CssClass="textInput" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IsReconciled">
                                                    <ItemStyle HorizontalAlign="Left" Width="25px" />
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("IsReconciled") %>'
                                                            ID="chkIsReconciled" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ValueDate" ItemStyle-Width="78px" HeaderText="Trans Date."
                                                    DataFormatString="{0: dd-MMM-yyyy}" />
                                                <asp:TemplateField HeaderText="Recon Date.">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <asp:TextBox runat="server" Width="90px" Text='<%# Eval("ReceiveDate") %>' ID="txtReceiveDt"
                                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                            CssClass="textInput" Enabled="true"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ReconBy.">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <asp:TextBox runat="server" Width="90px" Text='<%# Eval("ReconBy") %>' ID="txtReconBy"
                                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                            CssClass="textInput" Enabled="true"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="DownLoadBy" ItemStyle-Width="78px" HeaderText="DownLoadBy." />
                                                <asp:BoundField DataField="MakerID" ItemStyle-Width="78px" HeaderText="Maker." />
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
                            </fieldset>
                            <br />
                            <fieldset>
                                <legend runat="server" id="Legend1">Reconciliation Specification</legend>
                                <table border="0" width="98%" align="center" cellpadding="2" border="1">
                                    <tr>
                                        <td align="right" valign="top">
                                            Total Debit Amount
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalDebitAmount" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total Credit Amount
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalCreditAmount" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total Balance
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalBalance" Width="100px" CssClass="textInput" runat="server"
                                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">
                                            Total No. of Debit Transaction
                                        </td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtTotalNoofDebitTransaction" Width="100px" CssClass="textInput"
                                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right" valign="top">
                                            Total No. of Credit Transaction
                                        </td>
                                        <td colspan="3" valign="top">
                                            <asp:TextBox ID="txtTotalNoofCreditTransaction" Width="100px" CssClass="textInput"
                                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <legend>Operation</legend>
                                <table width="98%" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="center">
                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" />
                                            &nbsp;&nbsp;<asp:Button CssClass="ButtonAsh" runat="server" Text="Close" ID="btnClose" OnClientClick="javascript:hideModal('ctl00_cphDet_J_MDRecon');" />
                                        </td>
                                    </tr>
                                 </table
                            </fieldset>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnCheck" />
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">
    function JReconPopupHide() {
        hideModal('ctl00_cphDet_J_MDRecon');
        return false;
    }
    function JReconPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_J_MDRecon');
        return true;
    }
    function JReconPopupReturnTrue() {
        $("#<%=gvData.ClientID %>").remove();
        showModal("ctl00_cphDet_J_MDRecon");
        return false;
    }
</script>

