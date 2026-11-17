<%@ Page Title="Especialidad | Clínica médica" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EspecialidadAlta.aspx.cs" Inherits="ClinicaTurnos.Web.EspecialidadAlta" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="Especialidades.aspx">Volver a especialidades</a>
    <h1><asp:Literal ID="litTitulo" runat="server" Text="Agregar especialidad" /></h1>
    <p>Los campos marcados con * son obligatorios.</p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="alert" />
    <asp:Panel ID="pnlFormulario" runat="server" DefaultButton="btnGuardar">
        <asp:ValidationSummary ID="resumen" runat="server" CssClass="resumen" HeaderText="Revisá los datos del formulario:" />
        <div class="campo">
            <asp:Label ID="lblNombre" runat="server" AssociatedControlID="txtNombre" Text="Nombre de la especialidad *" />
            <asp:TextBox ID="txtNombre" runat="server" TextMode="SingleLine" MaxLength="80" />
            <asp:RequiredFieldValidator ID="reqNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="Completá nombre de la especialidad." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>

        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
            <a href="Especialidades.aspx">Cancelar</a>
        </div>
    </asp:Panel>
</asp:Content>
