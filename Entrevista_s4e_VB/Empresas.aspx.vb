Public Class Empresas
    Inherits System.Web.UI.Page

    Dim db As New WinformDBEntities()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CarregarDados()
        End If
    End Sub

    Private Sub CarregarDados()
        Dim empresas = db.tblEmpresa.ToList()
        GridViewEmpresas.DataSource = empresas
        GridViewEmpresas.DataBind()
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Try
            Dim emp As New tblEmpresa()
            emp.Nome = TextBox1.Text
            emp.CNPJ = TextBox2.Text

            db.tblEmpresa.Add(emp)
            db.SaveChanges()

            Label4.Text = "Banco de dados atualizado!"
        Catch ex As Exception
            Label4.Text = "Dados em duplicidade ou inconsistentes."
        End Try

        CarregarDados()
    End Sub

    Protected Sub GridViewEmpresas_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs) Handles GridViewEmpresas.RowUpdating
        Dim row As GridViewRow = GridViewEmpresas.Rows(e.RowIndex)
        Dim id As String = GridViewEmpresas.DataKeys(e.RowIndex).Value.ToString()

        GridViewEmpresas_Update(id, row)
    End Sub

    Private Sub GridViewEmpresas_Update(ByVal idEmpresa As String, ByVal row As GridViewRow)
        Dim txtNome As TextBox = DirectCast(row.FindControl("txtNome"), TextBox)
        Dim txtCnpj As TextBox = DirectCast(row.FindControl("txtCnpj"), TextBox)

        Dim idConvertido = Convert.ToInt32(idEmpresa)
        Dim empresa = db.tblEmpresa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()

        empresa.Nome = txtNome.Text
        empresa.CNPJ = txtCnpj.Text

        db.Entry(empresa).State = System.Data.Entity.EntityState.Modified
        db.SaveChanges()

        GridViewEmpresas.EditIndex = -1
        CarregarDados()
    End Sub

    Protected Sub GridViewEmpresas_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs) Handles GridViewEmpresas.RowEditing
        GridViewEmpresas.EditIndex = e.NewEditIndex
        CarregarDados()
    End Sub

    Protected Sub GridViewEmpresas_RowCanceling(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs) Handles GridViewEmpresas.RowCancelingEdit

        GridViewEmpresas.EditIndex = -1
        CarregarDados()
    End Sub

    Private Sub ExcluirEmpresa(ByVal id As String)
        Dim idConvertido = Integer.Parse(id)
        Dim empresa = db.tblEmpresa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()

        db.tblEmpresa.Remove(empresa)
        db.SaveChanges()

        CarregarDados()
    End Sub

    Protected Sub GridViewEmpresas_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles GridViewEmpresas.RowDeleting
        Dim id = GridViewEmpresas.DataKeys(e.RowIndex).Value.ToString()
        ExcluirEmpresa(id)
    End Sub

    Protected Sub btnFiltrar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click

        If Not String.IsNullOrEmpty(txtFiltroId.Text) Then
            Dim idConvertido = Integer.Parse(txtFiltroId.Text)
            Dim empresa = db.tblEmpresa.Where(Function(p) p.Id = idConvertido).FirstOrDefault()
            GridViewEmpresas.DataSource = New List(Of tblEmpresa)() From {empresa}
        ElseIf Not String.IsNullOrEmpty(txtFiltroNome.Text) Then
            Dim nome As String = txtFiltroNome.Text
            Dim empresasFiltradas = db.tblEmpresa.Where(Function(p) p.Nome.Equals(nome)).ToList()
            GridViewEmpresas.DataSource = empresasFiltradas
        Else
            CarregarDados()
        End If

        GridViewEmpresas.DataBind()
    End Sub
End Class
