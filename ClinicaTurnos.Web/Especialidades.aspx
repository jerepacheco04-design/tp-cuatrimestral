<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="ClinicaTurnos.Web.Especialidades" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Especialidades</h1>
    <p><a href="EspecialidadAlta.aspx">Agregar especialidad</a></p>
    <asp:Panel ID="pnlFiltro" runat="server" DefaultButton="btnBuscar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtBuscar" Text="Nombre de especialidad" />
            <asp:TextBox ID="txtBuscar" runat="server" MaxLength="80" />
        </div>
        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="Filtrar_Click" />
        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="Filtrar_Click" />
    </asp:Panel>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay registros cargados." OnRowCommand="grilla_RowCommand">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Especialidad" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:HyperLink ID="linkModificar" runat="server" Text="Modificar" NavigateUrl='<%# "EspecialidadAlta.aspx?id=" + Eval("Id") %>' />
                        <asp:LinkButton ID="linkEliminar" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('¿Eliminar esta especialidad?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
