<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="IntPaymentBatch.aspx.cs" Inherits="SPMS_Web.mp.IntPaymentBatch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">

    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right" width="15%">
                    SP Type
                </td>
                <td width="29%">
                    <asp:DropDownList ID="DropDownList3" Width="200px" runat="server">
                        <asp:ListItem Text="33mmss"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="right" width="12%">
                    Year
                </td>
                <td width="12%">
                    <asp:DropDownList ID="DropDownList4" Width="150px" runat="server">
                        <asp:ListItem Text="2011"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="right" width="12%">
                    Batch Date
                </td>
                <td width="15%">
                    <asp:TextBox ID="TextBox7" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Batch No
                </td>
                <td>
                    <div class="fieldLeft"><asp:TextBox ID="TextBox8" Width="130px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </div>
                    <div class="errorTextRight"><asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="Button1" /></div>
                </td>
                <td align="right">
                    Maker
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TextBox5" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    
    
    <div style="height: 100px; width: 100%; overflow: auto;">
        <asp:GridView Style="width: 98%" ID="gv" runat="server" AutoGenerateColumns="False"
            SkinID="SCBLGridGreen" ShowHeader="true">
            <Columns>
                <asp:BoundField DataField="Val1" HeaderText="ID" />
                <asp:BoundField DataField="Val1" HeaderText="Customer Name" />
                <asp:BoundField DataField="Val1" HeaderText="Address" />
            </Columns>
        </asp:GridView>
    </div>  <br/>
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right" width="15%">
                    Total Interest Amount
                </td>
                <td width="16%">
                    <asp:TextBox ID="TextBox1" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right" width="18%">
                    Total Income Tax
                </td>
                <td width="15%">
                    <asp:TextBox ID="TextBox2" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right" width="16%">
                    Total Levi
                </td>
                <td width="15%">
                    <asp:TextBox ID="TextBox3" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Total Payment Amount
                </td>
                <td>
                    <asp:TextBox ID="TextBox4" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
                <td align="right">
                    Status
                </td>
                <td colspan="3">
                    <asp:DropDownList ID="DropDownList2" Width="150px" runat="server">
                        <asp:ListItem Text="33mmss"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Create" ID="btnCreate" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnPreview" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print" ID="btnPrint" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Close" ID="btnClose" />
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>
