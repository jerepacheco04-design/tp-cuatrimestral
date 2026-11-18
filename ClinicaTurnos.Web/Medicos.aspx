<%@ Page Title="Médicos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Medicos.aspx.cs" Inherits="ClinicaTurnos.Web.Medicos" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Médicos</h1>
    <p><a href="MedicoAlta.aspx">Agregar médico</a></p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay registros cargados." OnRowCommand="grilla_RowCommand">
            <Columns>
                <asp:BoundField DataField="Matricula" HeaderText="Matrícula" />
                <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Email" HeaderText="Correo electrónico" />
                <asp:TemplateField HeaderText="Horario de trabajo">
                    <ItemTemplate><%#: Eval("TurnoTrabajo.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Especialidades">
                    <ItemTemplate><%#: MostrarEspecialidades(Eval("Especialidades")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:HyperLink ID="linkModificar" runat="server" Text="Modificar" NavigateUrl='<%# "MedicoAlta.aspx?id=" + Eval("Id") %>' />
                        <asp:LinkButton ID="linkEliminar" runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('¿Eliminar este médico?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
