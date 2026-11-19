<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClinicaTurnos.Web.Inicio" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Inicio</h1>
    <p>Administración de la clínica.</p>
    <ul>
        <li><a href="Pacientes.aspx">Pacientes</a></li>
        <li><a href="Especialidades.aspx">Especialidades</a></li>
        <li><a href="Turnos.aspx">Turnos</a></li>
    </ul>
</asp:Content>
