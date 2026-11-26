<%@ Page Title="Pacientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pacientes.aspx.cs" Inherits="ClinicaTurnos.Web.Pacientes" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Pacientes</h1>
    <p><a href="PacienteAlta.aspx">Agregar paciente</a></p>
    <asp:Panel ID="pnlFiltro" runat="server" DefaultButton="btnBuscar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtBuscar" Text="DNI o apellido" />
            <asp:TextBox ID="txtBuscar" runat="server" MaxLength="80" />
        </div>
        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="Filtrar_Click" />
        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="Filtrar_Click" />
    </asp:Panel>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay registros cargados." OnRowCommand="grilla_RowCommand">
            <Columns>
                <asp:BoundField DataField="Dni" HeaderText="DNI" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Email" HeaderText="Correo electrónico" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:HyperLink ID="linkHistorial" runat="server" Text="Historial" NavigateUrl='<%# "HistorialPaciente.aspx?id=" + Eval("Id") %>' />
                        <asp:HyperLink ID="linkModificar" runat="server" Text="Modificar" NavigateUrl='<%# "PacienteAlta.aspx?id=" + Eval("Id") %>' />
                        <asp:LinkButton ID="linkEliminar" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('¿Eliminar este paciente?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
