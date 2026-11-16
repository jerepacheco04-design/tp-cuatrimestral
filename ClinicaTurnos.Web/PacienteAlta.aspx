<%@ Page Title="Paciente | Clínica médica" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PacienteAlta.aspx.cs" Inherits="ClinicaTurnos.Web.PacienteAlta" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="Pacientes.aspx">Volver a pacientes</a>
    <h1>Agregar paciente</h1>
    <p>Los campos marcados con * son obligatorios.</p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="alert" />
    <asp:Panel ID="pnlFormulario" runat="server" DefaultButton="btnGuardar">
        <asp:ValidationSummary ID="resumen" runat="server" CssClass="resumen" HeaderText="Revisá los datos del formulario:" />
        <div class="campo">
            <asp:Label ID="lblDni" runat="server" AssociatedControlID="txtDni" Text="DNI *" />
            <asp:TextBox ID="txtDni" runat="server" TextMode="SingleLine" MaxLength="20" />
            <asp:RequiredFieldValidator ID="reqDni" runat="server" ControlToValidate="txtDni" ErrorMessage="Completá dni." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <asp:Label ID="lblNombre" runat="server" AssociatedControlID="txtNombre" Text="Nombre *" />
            <asp:TextBox ID="txtNombre" runat="server" TextMode="SingleLine" MaxLength="80" />
            <asp:RequiredFieldValidator ID="reqNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="Completá nombre." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <asp:Label ID="lblApellido" runat="server" AssociatedControlID="txtApellido" Text="Apellido *" />
            <asp:TextBox ID="txtApellido" runat="server" TextMode="SingleLine" MaxLength="80" />
            <asp:RequiredFieldValidator ID="reqApellido" runat="server" ControlToValidate="txtApellido" ErrorMessage="Completá apellido." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <asp:Label ID="lblFechaNacimiento" runat="server" AssociatedControlID="txtFechaNacimiento" Text="Fecha de nacimiento *" />
            <asp:TextBox ID="txtFechaNacimiento" runat="server" TextMode="Date" MaxLength="10" />
            <asp:RequiredFieldValidator ID="reqFechaNacimiento" runat="server" ControlToValidate="txtFechaNacimiento" ErrorMessage="Completá fecha de nacimiento." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <asp:Label ID="lblTelefono" runat="server" AssociatedControlID="txtTelefono" Text="Teléfono" />
            <asp:TextBox ID="txtTelefono" runat="server" TextMode="SingleLine" MaxLength="30" />
        </div>
        <div class="campo">
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Correo electrónico *" />
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" />
            <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Completá correo electrónico." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <asp:Label ID="lblDireccion" runat="server" AssociatedControlID="txtDireccion" Text="Dirección" />
            <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" MaxLength="250" />
        </div>

        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
            <a href="Pacientes.aspx">Cancelar</a>
        </div>
    </asp:Panel>
</asp:Content>
