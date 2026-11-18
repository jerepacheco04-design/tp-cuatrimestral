<%@ Page Title="Médico" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MedicoAlta.aspx.cs" Inherits="ClinicaTurnos.Web.MedicoAlta" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="Medicos.aspx">Volver a médicos</a>
    <h1><asp:Literal ID="litTitulo" runat="server" Text="Agregar médico" /></h1>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="alert" />
    <asp:Panel ID="pnlFormulario" runat="server" DefaultButton="btnGuardar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtMatricula" Text="Matrícula" />
            <asp:TextBox ID="txtMatricula" runat="server" MaxLength="30" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtNombre" Text="Nombre" />
            <asp:TextBox ID="txtNombre" runat="server" MaxLength="80" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtApellido" Text="Apellido" />
            <asp:TextBox ID="txtApellido" runat="server" MaxLength="80" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Correo electrónico" />
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlHorario" Text="Horario de trabajo" />
            <asp:DropDownList ID="ddlHorario" runat="server" />
        </div>
        <fieldset>
            <legend>Especialidades</legend>
            <asp:CheckBoxList ID="listaEspecialidades" runat="server" RepeatLayout="Flow" />
        </fieldset>
        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
            <a href="Medicos.aspx">Cancelar</a>
        </div>
    </asp:Panel>
</asp:Content>
