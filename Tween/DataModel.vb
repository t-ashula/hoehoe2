Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization

Public Class DataModel

    <DataContract()> _
    Public Class urls
        <DataMember(Name:="urls")> Public urls As String
        <DataMember(Name:="indices")> Public indices(2) As Integer
    End Class

    <DataContract()> _
    Public Class hashtags
        <DataMember(Name:="indices")> Public indices(2) As Integer
        <DataMember(Name:="text")> Public text As String
    End Class

    <DataContract()> _
    Public Class user_mentions
        <DataMember(Name:="indices")> Public indices(2) As Integer
        <DataMember(Name:="screen_name")> Public screen_name As String
        <DataMember(Name:="name")> Public _name As String
        <DataMember(Name:="id")> Public id As Int64
    End Class

    <DataContract()> _
    Public Class entities
        <DataMember(Name:="urls")> Public urls() As urls
        <DataMember(Name:="hashtags")> Public hashtags() As hashtags
        <DataMember(Name:="user_mentions")> Public user_mentions() As user_mentions
    End Class

    <DataContract()> _
    Public Class user
        <DataMember(Name:="statuses_count")> Public statuses_count As Int64
        <DataMember(Name:="profile_sidebar_fill_color")> Public profile_sidebar_fill_color As String
        <DataMember(Name:="show_all_inline_media")> Public show_all_inline_media As Boolean
        <DataMember(Name:="profile_use_background_image")> Public profile_use_background_image As Boolean
        <DataMember(Name:="contributors_enabled")> Public contributors_enabled As Boolean
        <DataMember(Name:="profile_sidebar_border_color")> Public profile_sidebar_border_color As String
        <DataMember(Name:="location")> Public location As String
        <DataMember(Name:="geo_enabled")> Public geo_enabled As Boolean
        <DataMember(Name:="description")> Public description As String
        <DataMember(Name:="friends_count")> Public friends_count As Integer
        <DataMember(Name:="verified")> Public verified As Boolean
        <DataMember(Name:="favourites_count")> Public favourites_count As Integer
        <DataMember(Name:="created_at")> Public created_at As String
        <DataMember(Name:="profile_background_color")> Public profile_background_color As String
        <DataMember(Name:="follow_request_sent")> Public follow_request_sent As String
        <DataMember(Name:="time_zone")> Public time_zone As String
        <DataMember(Name:="followers_count")> Public followers_count As Integer
        <DataMember(Name:="url")> Public url As String
        <DataMember(Name:="profile_image_url")> Public profile_image_url As String
        <DataMember(Name:="notifications")> Public notifications As String
        <DataMember(Name:="profile_text_color")> Public profile_text_color As String
        <DataMember(Name:="protected")> Public [protected] As Boolean
        <DataMember(Name:="id_str")> Public id_str As String
        <DataMember(Name:="lang")> Public lang As String
        <DataMember(Name:="profile_background_image_url")> Public profile_background_image_url As String
        <DataMember(Name:="screen_name")> Public screen_name As String
        <DataMember(Name:="name")> Public _name As String
        <DataMember(Name:="following")> Public following As String
        <DataMember(Name:="profile_link_color")> Public profile_link_color As String
        <DataMember(Name:="id")> Public id As Int64
        <DataMember(Name:="listed_count")> Public listed_count As Integer
        <DataMember(Name:="profile_background_tile")> Public profile_background_tile As Boolean
        <DataMember(Name:="utc_offset")> Public utc_offset As String
        <DataMember(Name:="place")> Public place As place
    End Class

    <DataContract()> _
    Public Class coordinates
        <DataMember(Name:="type")> Public type As String
        <DataMember(Name:="coordinates")> Public _coordinates(2) As Single
    End Class

    <DataContract()> _
    Public Class geo
        <DataMember(Name:="type", IsRequired:=False)> Public type As String
        <DataMember(Name:="coordinates")> Public _coordinates(2) As Single
    End Class

    <DataContract()> _
    Public Class bounding_box
        <DataMember(Name:="type")> Public type As String
        <DataMember(Name:="coordinates")> Public _coordinates(2)() As Single
    End Class

    <DataContract()> _
    Public Class place
        <DataMember(Name:="url")> Public url As String
        <DataMember(Name:="bounding_box")> Public bounding_box As bounding_box
        <DataMember(Name:="street_address")> Public street_address As String
        <DataMember(Name:="full_name")> Public full_name As String
        <DataMember(Name:="name")> Public _name As String
        <DataMember(Name:="country_code")> Public country_code As String
        <DataMember(Name:="id")> Public id As String
        <DataMember(Name:="country")> Public country As String
        <DataMember(Name:="place_type")> Public place_type As String
    End Class

    <DataContract()> _
    Public Class retweeted_status
        <DataMember(Name:="coordinates")> Public coordinates As coordinates
        <DataMember(Name:="geo")> Public geo As geo
        <DataMember(Name:="in_reply_to_user_id")> Public in_reply_to_user_id As String
        <DataMember(Name:="source")> Public source As String
        <DataMember(Name:="user")> Public user As user
        <DataMember(Name:="in_reply_to_screen_name")> Public in_reply_to_screen_name As String
        <DataMember(Name:="created_at")> Public created_at As String
        <DataMember(Name:="contributors")> Public contributors As String
        <DataMember(Name:="favorited")> Public favorited As Boolean
        <DataMember(Name:="truncated")> Public truncated As Boolean
        <DataMember(Name:="id")> Public id As Int64
        <DataMember(Name:="annotations")> Public annotations As String
        <DataMember(Name:="place")> Public place As place
        <DataMember(Name:="in_reply_to_status_id")> Public in_reply_to_status_id As String
        <DataMember(Name:="text")> Public text As String
    End Class

    <DataContract()> _
    Public Class status
        <DataMember(Name:="in_reply_to_status_id_str")> Public in_reply_to_status_id_str As String
        <DataMember(Name:="contributors")> Public contributors As String
        <DataMember(Name:="in_reply_to_screen_name")> Public in_reply_to_screen_name As String
        <DataMember(Name:="in_reply_to_status_id")> Public in_reply_to_status_id As String
        <DataMember(Name:="in_reply_to_user_id_str")> Public in_reply_to_user_id_str As String
        <DataMember(Name:="retweet_count")> Public retweet_count As String
        <DataMember(Name:="created_at")> Public created_at As String
        <DataMember(Name:="geo")> Public geo As geo
        <DataMember(Name:="retweeted")> Public retweeted As Boolean
        <DataMember(Name:="in_reply_to_user_id")> Public in_reply_to_user_id As String
        <DataMember(Name:="source")> Public source As String
        <DataMember(Name:="id_str")> Public id_str As String
        <DataMember(Name:="coordinates")> Public coordinates As coordinates
        <DataMember(Name:="truncated")> Public truncated As Boolean
        <DataMember(Name:="place")> Public place As String
        <DataMember(Name:="user")> Public user As user
        <DataMember(Name:="retweeted_status", IsRequired:=False)> Public retweeted_status As retweeted_status
        <DataMember(Name:="id")> Public id As Int64
        <DataMember(Name:="favorited")> Public favorited As Boolean
        <DataMember(Name:="text")> Public text As String
    End Class

    <DataContract()> _
    Public Class directmessage
        <DataMember(Name:="created_at")> Public created_at As String
        <DataMember(Name:="sender_id")> Public sender_id As Int64
        <DataMember(Name:="sender_screen_name")> Public sender_screen_name As String
        <DataMember(Name:="sender")> Public sender As user
        <DataMember(Name:="id_str")> Public id_str As String
        <DataMember(Name:="recipient")> Public recipient As user
        <DataMember(Name:="recipient_screen_name")> Public recipient_screen_name As String
        <DataMember(Name:="recipient_id")> Public recipient_id As Int64
        <DataMember(Name:="id")> Public id As Int64
        <DataMember(Name:="text")> Public text As String
    End Class

    <DataContract()> _
    Public Class friendsevent
        <DataMember(Name:="friends")> Public friends As Int64()
    End Class

    <DataContract()> _
    Public Class deletedstatus
        <DataMember(Name:="id")> Public id As Int64
        <DataMember(Name:="user_id")> Public user_id As Int64
    End Class

    <DataContract()> _
    Public Class deleteevent
        <DataMember(Name:="event")> Public [event] As String
        <DataMember(Name:="status")> Public status As deletedstatus
    End Class

    <DataContract()> _
    Public Class directmessageevent
        <DataMember(Name:="direct_message")> Public direct_message As directmessage
    End Class

    <DataContract()> _
    Public Class trackcount
        <DataMember(Name:="track")> Public track As Integer
    End Class

    <DataContract()> _
    Public Class limitevent
        <DataMember(Name:="limit")> Public limit As trackcount
    End Class

    <DataContract()> _
    Public Class eventdata
        <DataMember(Name:="target")> Public target As user
        <DataMember(Name:="target_object")> Public target_object As status
        <DataMember(Name:="created_at")> Public created_at As String
        <DataMember(Name:="event")> Public [event] As String
        <DataMember(Name:="source")> Public source As user
    End Class
End Class
