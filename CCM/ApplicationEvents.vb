Imports System.Reflection
Namespace My

    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            EmbeddedAssembly.Load("CCM.Newtonsoft.Json.dll", "Newtonsoft.Json.dll")
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve

            'Dim form As New Form1
            'Form.ShowDialog()
        End Sub

        Private Function CurrentDomain_AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly
            Return EmbeddedAssembly.Get(args.Name)
        End Function

    End Class

End Namespace
