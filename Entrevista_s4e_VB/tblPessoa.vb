'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class tblPessoa
    Public Property Id As Integer
    Public Property Nome As String
    Public Property Cpf As String
    Public Property DataNascimento As Nullable(Of Date)

    Public Overridable Property tblPessoaEmpresa As ICollection(Of tblPessoaEmpresa) = New HashSet(Of tblPessoaEmpresa)

End Class
