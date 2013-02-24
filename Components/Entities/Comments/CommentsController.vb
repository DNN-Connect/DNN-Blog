Imports System
Imports System.Data
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Data
Imports DotNetNuke.Modules.Blog.Integration
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Entities.Entries
Imports DotNetNuke.Modules.Blog.Security.Permissions
Imports DotNetNuke.Modules.Blog.Common.Constants

Namespace Entities.Comments

 Partial Public Class CommentsController

  Public Shared Function AddComment(blog As BlogInfo, entry As EntryInfo, ByRef comment As CommentInfo) As Integer

   AddComment(comment, PortalSettings.Current.UserId)
   If comment.Approved Then
    JournalController.AddOrUpdateCommentInJournal(blog, entry, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, entry.PermaLink(PortalSettings.Current))
   Else
    Dim title As String = GetString("CommentPendingNotify", Common.Constants.SharedResourceFileName)
    Dim summary As String = "<a target='_blank' href='" + entry.PermaLink(PortalSettings.Current) + "'>" + entry.Title + "</a><br />" + comment.Comment
    NotificationController.CommentPendingApproval(comment, blog, entry, PortalSettings.Current.PortalId, summary, title)
   End If
   Return comment.CommentID

  End Function

  Public Shared Sub UpdateComment(blog As BlogInfo, entry As EntryInfo, comment As CommentInfo)

   UpdateComment(comment, PortalSettings.Current.UserId)
   NotificationController.RemoveCommentPendingNotification(blog.BlogID, comment.ContentItemId, comment.CommentID)
   If comment.Approved Then
    JournalController.AddOrUpdateCommentInJournal(blog, entry, comment, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.UserId, entry.PermaLink(PortalSettings.Current))
   Else
    Dim title As String = GetString("CommentPendingNotify", Common.Constants.SharedResourceFileName)
    Dim summary As String = "<a target='_blank' href='" + entry.PermaLink(PortalSettings.Current) + "'>" + entry.Title + "</a><br />" + comment.Comment
    NotificationController.CommentPendingApproval(comment, blog, entry, PortalSettings.Current.PortalId, summary, title)
   End If

  End Sub

  Public Shared Sub ApproveComment(blogId As Integer, comment As CommentInfo)

   Data.DataProvider.Instance.ApproveComment(comment.CommentID)
   NotificationController.RemoveCommentPendingNotification(blogId, comment.ContentItemId, comment.CommentID)

  End Sub

  Public Shared Sub DeleteComment(blogId As Integer, comment As CommentInfo)

   DataProvider.Instance().DeleteComment(comment.CommentID)
   NotificationController.RemoveCommentPendingNotification(blogId, comment.ContentItemId, comment.CommentID)

  End Sub

 End Class
End Namespace

