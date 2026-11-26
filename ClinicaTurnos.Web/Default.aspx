<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClinicaTurnos.Web.Inicio" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <asp:Panel ID="pnlLogin" runat="server">
        <h1>Iniciar sesión</h1>
        <div class="campo">
            <asp:Label ID="lblUsuario" runat="server" AssociatedControlID="txtUsuario">Usuario</asp:Label>
            <asp:TextBox ID="txtUsuario" runat="server" MaxLength="50" />
        </div>
        <div class="campo">
            <asp:Label ID="lblClave" runat="server" AssociatedControlID="txtClave">Contraseña</asp:Label>
            <asp:TextBox ID="txtClave" runat="server" TextMode="Password" MaxLength="100" />
        </div>
        <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
        <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" OnClick="btnIngresar_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlInicio" runat="server" Visible="false">
        <h1>Inicio</h1>
        <asp:Label ID="lblBienvenida" runat="server" />
        <p><asp:Label ID="lblDescripcionInicio" runat="server" /></p>
        <asp:Label ID="lblMensajeInicio" runat="server" CssClass="aviso" role="status" />
    </asp:Panel>
</asp:Content>
