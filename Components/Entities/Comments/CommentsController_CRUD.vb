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
Imports DotNetNuke.Services.Tokens

Imports DotNetNuke.Modules.Blog.Data

Namespace Entities.Comments

 Partial Public Class CommentsController

  Public Shared Function GetComment(commentID As Int32) As CommentInfo

   Return CType(CBO.FillObject(DataProvider.Instance().GetComment(commentID), GetType(CommentInfo)), CommentInfo)

  End Function

 Public Shared Function AddComment(ByRef objComment As CommentInfo, createdByUser As Integer) As Integer

  objComment.CommentID = CType(DataProvider.Instance().AddComment(objComment.Approved, objComment.Author, objComment.Comment, objComment.ContentItemId, objComment.Email, objComment.Website, createdByUser), Integer)
  Return objComment.CommentID

 End Function

 Public Shared Sub UpdateComment(objComment As CommentInfo, updatedByUser As Integer)
	
  DataProvider.Instance().UpdateComment(objComment.Approved, objComment.Author, objComment.Comment, objComment.CommentID, objComment.ContentItemId, objComment.Email, objComment.Website, updatedByUser)
	
 End Sub

 Public Shared Sub DeleteComment(commentID As Int32)
	
  DataProvider.Instance().DeleteComment(commentID)

 End Sub

 End Class
End Namespace

