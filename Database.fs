module NNews.Database

open System.Data.SQLite

type Post =
    { PostId: int
      Url: string
      Description: string
      BodyText: string }

let rec readPosts (reader: SQLiteDataReader): List<Post> =
    match reader.Read() with
        | true -> { PostId = reader.GetInt32(0); Url = reader.GetString(1); Description = reader.GetString(2); BodyText = reader.GetString(3) } :: (readPosts reader)
        | false -> []

let getPagePosts: List<Post> =
    use connection = new SQLiteConnection(@"Data Source=nnews.sqlite;")
    use sqlCommand = new SQLiteCommand("SELECT post_id, url, description, body_text FROM post;", connection)
    connection.Open()
    let reader = sqlCommand.ExecuteReader()
    readPosts reader

let getSinglePost (postId: int) : Post =
    use connection = new SQLiteConnection(@"Data Source=nnews.sqlite;")
    use sqlCommand = new SQLiteCommand($"SELECT post_id, url, description, body_text FROM post WHERE post_id = {postId};", connection)
    connection.Open()
    let reader = sqlCommand.ExecuteReader()
    readPosts reader |> List.head