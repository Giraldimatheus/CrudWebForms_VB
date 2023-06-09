Imports System.Web.UI.WebControls
Public Class Home

    Inherits System.Web.UI.Page

    Dim db As New WinformDBEntities()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CarregarDados()
        End If
    End Sub

    Private Sub CarregarDados()
        Dim funcionarios = db.tblPessoa.ToList()
        GridViewFuncionarios.DataSource = funcionarios
        GridViewFuncionarios.DataBind()

        Dim empresas = db.tblEmpresa.ToList()

        Select1.DataSource = empresas
        Select1.DataTextField = "Nome"
        Select1.DataValueField = "Id"
        Select1.DataBind()
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim p As New tblPessoa()

            p.Nome = TextBox1.Text
            p.Cpf = TextBox2.Text
            p.DataNascimento = DateTime.Parse(TextBox3.Text)

            db.tblPessoa.Add(p)
            db.SaveChanges()

            Dim UltimaPessoaCadastrada = db.tblPessoa.ToList().LastOrDefault()

            Dim funcionarioEmpresa As New List(Of tblPessoaEmpresa)()

            For Each listItem As ListItem In Select1.Items
                If listItem.Selected Then
                    Dim idConvertido = Convert.ToInt32(listItem.Value)
                    Dim PE As New tblPessoaEmpresa()
                    PE.tblPessoa_id = UltimaPessoaCadastrada.Id
                    PE.tblEmpresa_id = idConvertido

                    funcionarioEmpresa.Add(PE)
                End If
            Next

            db.tblPessoaEmpresa.AddRange(funcionarioEmpresa)
            db.SaveChanges()

            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "FecharModalScript", "fecharModal();", True)
            CarregarDados()


            label4.Text = "Banco de dados atualizado!"
        Catch ex As Exception
            label4.Text = "Dados em duplicidade ou inconsistentes."
        End Try

        CarregarDados()
    End Sub

    Public Function GetEmpresasPessoa(ByVal id As Object) As String
        Dim pessoaId = Convert.ToInt32(id)

        Dim empresas As New List(Of String)()

        empresas = db.tblPessoaEmpresa.Where(Function(p) p.tblPessoa_id = pessoaId).Select(Function(p) p.tblEmpresa.Nome).ToList()

        Dim empresasConcatenadas = String.Join(", ", empresas)

        Return empresasConcatenadas
    End Function

    Protected Sub GridViewFuncionarios_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewFuncionarios.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim lblEmpresas As Label = DirectCast(e.Row.FindControl("lblEmpresas"), Label)
            Dim id As Object = DataBinder.Eval(e.Row.DataItem, "Id")
            lblEmpresas.Text = GetEmpresasPessoa(id)
        End If
    End Sub

    Protected Sub CarregarEmpresas(ByVal sender As Object, ByVal e As EventArgs)
        Select1.Items.Clear()
        Select1.Items.Add(New ListItem("", ""))

        Using db = New WinformDBEntities()
            Dim empresas = db.tblEmpresa.ToList()

            For Each empresa In empresas
                Select1.Items.Add(New ListItem(empresa.Nome, empresa.Id.ToString()))
            Next
        End Using
    End Sub

    Protected Sub GridViewFuncionarios_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim row As GridViewRow = GridViewFuncionarios.Rows(e.RowIndex)
        Dim id As String = GridViewFuncionarios.DataKeys(e.RowIndex).Value.ToString()

        GridViewFuncionarios_Update(id, row)
    End Sub

    Protected Sub GridViewFuncionarios_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        GridViewFuncionarios.EditIndex = e.NewEditIndex
        CarregarDados()
    End Sub

    Protected Sub GridViewFuncionarios_RowCanceling(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)
        GridViewFuncionarios.EditIndex = -1
        CarregarDados()
    End Sub

    Private Sub GridViewFuncionarios_Update(ByVal idFuncionario As String, ByVal row As GridViewRow)
        Dim txtNome As TextBox = DirectCast(row.FindControl("txtNome"), TextBox)
        Dim txtCpf As TextBox = DirectCast(row.FindControl("txtCpf"), TextBox)
        Dim txtDataNascimento As TextBox = DirectCast(row.FindControl("txtDataNascimento"), TextBox)

        Dim idConvertido = Convert.ToInt32(idFuncionario)
        Dim funcionario = db.tblPessoa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()
        funcionario.Nome = txtNome.Text
        funcionario.Cpf = txtCpf.Text
        funcionario.DataNascimento = DateTime.Parse(txtDataNascimento.Text)

        db.Entry(funcionario).State = System.Data.Entity.EntityState.Modified
        db.SaveChanges()
        GridViewFuncionarios.EditIndex = -1
        CarregarDados()
    End Sub

    Protected Sub GridViewFuncionarios_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim id = GridViewFuncionarios.DataKeys(e.RowIndex).Value.ToString()

        ExcluiFuncionario(id)
    End Sub

    Private Sub ExcluiFuncionario(ByVal id As String)
        Dim idConvertido = Integer.Parse(id)

        Dim funcionarioEmp = db.tblPessoaEmpresa.Where(Function(x) x.tblPessoa_id = idConvertido).FirstOrDefault()
        db.tblPessoaEmpresa.Remove(funcionarioEmp)
        db.SaveChanges()

        Dim funcionario = db.tblPessoa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()
        db.tblPessoa.Remove(funcionario)
        db.SaveChanges()

        CarregarDados()
    End Sub

    Protected Sub btnFiltrar_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not String.IsNullOrEmpty(txtFiltroId.Text) Then
            Dim idConvertido = Integer.Parse(txtFiltroId.Text)
            Dim pessoa = db.tblPessoa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()
            GridViewFuncionarios.DataSource = New List(Of tblPessoa)() From {pessoa}
        ElseIf Not String.IsNullOrEmpty(txtFiltroNome.Text) Then
            Dim nome = txtFiltroNome.Text
            Dim pessoasFiltradas = db.tblPessoa.Where(Function(p) p.Nome.Equals(nome)).ToList()
            GridViewFuncionarios.DataSource = pessoasFiltradas
        Else
            CarregarDados()
        End If

        GridViewFuncionarios.DataBind()
    End Sub

    Protected Sub ctl15_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Protected Sub Unnamed3_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", True)
        upModal.Update()
    End Sub

    Protected Sub ExibirModal()
        Dim script As String = "$(document).ready(function () {$('#myModal').modal('show');});"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "myModal", script, True)
    End Sub



End Class