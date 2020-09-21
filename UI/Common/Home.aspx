<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SBM_WebUI.UI.Common.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style type="text/css">

.setup_bg_0{
  background-image:url(../../Images/home_page/setup_bg1.jpg); background-position:left; background-repeat:no-repeat;
  width: 365px; height:100px; border: 1px solid #DDDADA;  
}
.setup_bg_1{
  background-image:url(../../Images/home_page/setup_bg.jpg); background-position:left; background-repeat:no-repeat;
  width: 365px; height:100px; border: 1px solid #A6A5A5;
}
.homeBullet
{
    padding-top:0px;
    padding-left:105px;
    line-height:20px;
    list-style-image: url('../../Images/arrow.gif');

}
.secTitle 
{
    padding-top:10px;
    padding-left:75px;
    line-height:20px;
    
    }
</style>

<script language="javascript" type="text/javascript">
    function ImgChange(oObj, vClass) {
        oObj.className = vClass;
    }
    //-->
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    
    
    
    <table width="95%" align="center" class="tableBody" cellpadding="5" cellspacing="5"  border="0">
        <tr>
            <td><div id="setUp" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Configration</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
            <td><div id="trans" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Transaction</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
        </tr>
        
        <tr>
            <td><div id="Div1" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Claim</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
            <td><div id="Div2" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Reconciliation</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
        </tr>
        
        <tr>
            <td><div id="Div3" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Reports</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
            <td><div id="Div4" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Security Administration</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
        </tr>
        
        <tr>
            <td><div id="Div5" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Journal Reconciliation</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
            <td><div id="Div6" onmouseover="ImgChange(this, 'setup_bg_1')" onmouseout="ImgChange(this, 'setup_bg_0')" class="setup_bg_0">
                <div class="secTitle">Utility</div>
                <ul class="homeBullet">
                    <li>Feature 1</li>
                    <li>Feature 2</li>
                </ul>
            </div></td>
        </tr>
        
    </table>

</asp:Content>
