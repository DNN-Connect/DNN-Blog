'
' DNN Connect - http://dnn-connect.org
' Copyright (c) 2015
' by DNN Connect
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports DotNetNuke.Web.Client.ClientResourceManagement
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Blog.Common.Globals
Imports DotNetNuke.Modules.Blog.Entities.Terms
Imports DotNetNuke.Modules.Blog.Templating
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Web.Client
Imports DotNetNuke.Framework.JavaScriptLibraries

Namespace Common

 Public Class BlogModuleBase
  Inherits PortalModuleBase

#Region " Private Members "
#End Region

#Region " Properties "
  Private _blogContext As BlogContextInfo
  Public Property BlogContext() As BlogContextInfo
   Get
    If _blogContext Is Nothing Then
     _blogContext = BlogContextInfo.GetBlogContext(Context, Me)
    End If
    Return _blogContext
   End Get
   Set(ByVal value As BlogContextInfo)
    _blogContext = value
   End Set
  End Property

  Private _settings As ModuleSettings
  Public Shadows Property Settings() As ModuleSettings
   Get
    If _settings Is Nothing Then
     If ViewSettings.BlogModuleId = -1 Then
      _settings = ModuleSettings.GetModuleSettings(ModuleConfiguration.ModuleID)
     Else
      _settings = ModuleSettings.GetModuleSettings(ViewSettings.BlogModuleId)
     End If
    End If
    Return _settings
   End Get
   Set(ByVal value As ModuleSettings)
    _settings = value
   End Set
  End Property

  Private _categories As Dictionary(Of String, TermInfo)
  Public Property Categories() As Dictionary(Of String, TermInfo)
   Get
    If _categories Is Nothing Then
     _categories = TermsController.GetTermsByVocabulary(ModuleId, Settings.VocabularyId, BlogContext.Locale)
    End If
    Return _categories
   End Get
   Set(ByVal value As Dictionary(Of String, TermInfo))
    _categories = value
   End Set
  End Property

  Private _viewSettings As ViewSettings
  Public Property ViewSettings() As ViewSettings
   Get
    If _viewSettings Is Nothing Then _viewSettings = ViewSettings.GetViewSettings(TabModuleId)
    Return _viewSettings
   End Get
   Set(ByVal value As ViewSettings)
    _viewSettings = value
   End Set
  End Property

  Public Shadows ReadOnly Property Page As CDefault
   Get
    Return CType(MyBase.Page, CDefault)
   End Get
  End Property

  Private _BlogModuleMapPath As String = ""
  Public ReadOnly Property BlogModuleMapPath As String
   Get
    If String.IsNullOrEmpty(_BlogModuleMapPath) Then
     _BlogModuleMapPath = Server.MapPath("~/DesktopModules/Blog") & "\"
    End If
    Return _BlogModuleMapPath
   End Get
  End Property
#End Region

#Region " Event Handlers "
  Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

   If Context.Items("BlogModuleBaseInitialized") Is Nothing Then

    JavaScript.RequestRegistration(CommonJs.jQuery)
    JavaScript.RequestRegistration(CommonJs.jQueryUI)
    Dim script As New StringBuilder
    script.AppendLine("<script type=""text/javascript"">")
    script.AppendLine("//<![CDATA[")
    script.AppendLine(String.Format("var appPath='{0}'", DotNetNuke.Common.ApplicationPath))
    script.AppendLine("//]]>")
    script.AppendLine("</script>")
    ClientAPI.RegisterClientScriptBlock(Page, "blogAppPath", script.ToString)
    AddBlogService()

    Context.Items("BlogModuleBaseInitialized") = True
   End If

  End Sub
#End Region

#Region " Public Methods "
  Public Sub AddBlogService()

   If Context.Items("BlogServiceAdded") Is Nothing Then

    JavaScript.RequestRegistration(CommonJs.DnnPlugins)
    ServicesFramework.Instance.RequestAjaxScriptSupport()
    ServicesFramework.Instance.RequestAjaxAntiForgerySupport()
    AddJavascriptFile("dotnetnuke.blog.js", 70)

    ' Load initialization snippet
    Dim scriptBlock As String = ReadFile(DotNetNuke.Common.ApplicationMapPath & "\DesktopModules\Blog\js\dotnetnuke.blog.pagescript.js")
    Dim tr As New BlogTokenReplace(BlogContext.BlogModuleId)
    tr.AddResources("~/DesktopModules/Blog/App_LocalResources/SharedResources.resx")
    scriptBlock = tr.ReplaceTokens(scriptBlock)
    scriptBlock = "<script type=""text/javascript"">" & vbCrLf & "//<![CDATA[" & vbCrLf & scriptBlock & vbCrLf & "//]]>" & vbCrLf & "</script>"
    Page.ClientScript.RegisterClientScriptBlock([GetType], "BlogServiceScript", scriptBlock)

    Context.Items("BlogServiceAdded") = True
   End If

  End Sub

  Public Sub AddJavascriptFile(jsFilename As String, priority As Integer)
   Page.AddJavascriptFile(Settings.Version, jsFilename, priority)
  End Sub

  Public Sub AddJavascriptFile(jsFilename As String, name As String, version As String, priority As Integer)
   Page.AddJavascriptFile(Settings.Version, jsFilename, name, version, priority)
  End Sub

  Public Sub AddCssFile(cssFilename As String)
   Page.AddCssFile(Settings.Version, cssFilename)
  End Sub

  Public Sub AddCssFile(cssFilename As String, name As String, version As String)
   Page.AddCssFile(Settings.Version, cssFilename, name, version)
  End Sub

  Public Function LocalizeJSString(resourceKey As String) As String
   Return ClientAPI.GetSafeJSString(LocalizeString(resourceKey))
  End Function

  Public Function LocalizeJSString(resourceKey As String, resourceFile As String) As String
   Return ClientAPI.GetSafeJSString(Localization.GetString(resourceKey, resourceFile))
  End Function
#End Region

 End Class

End Namespace