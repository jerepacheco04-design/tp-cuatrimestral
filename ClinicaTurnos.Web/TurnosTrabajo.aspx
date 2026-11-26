<%@ Page Title="Horarios de trabajo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TurnosTrabajo.aspx.cs" Inherits="ClinicaTurnos.Web.TurnosTrabajo" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Horarios de trabajo</h1>
    <p><a href="TurnoTrabajoAlta.aspx">Agregar horario</a></p>
    <asp:Panel ID="pnlFiltro" runat="server" DefaultButton="btnBuscar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtBuscar" Text="Nombre del horario" />
            <asp:TextBox ID="txtBuscar" runat="server" MaxLength="80" />
        </div>
        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="Filtrar_Click" />
        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="Filtrar_Click" />
    </asp:Panel>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay horarios cargados." OnRowCommand="grilla_RowCommand">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Horario" />
                <asp:TemplateField HeaderText="Días y horas">
                    <ItemTemplate>
                        <%# ResumirFranjas(Eval("Franjas")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:HyperLink ID="linkModificar" runat="server" Text="Modificar" NavigateUrl='<%# "TurnoTrabajoAlta.aspx?id=" + Eval("Id") %>' />
                        <asp:LinkButton ID="linkEliminar" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('¿Eliminar este horario de trabajo?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
