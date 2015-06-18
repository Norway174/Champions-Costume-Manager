Public Class AutoUpdater

    Private Sub AutoUpdater_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        WebBrowser1.DocumentText = My.Resources.Auto_updater
    End Sub
End Class