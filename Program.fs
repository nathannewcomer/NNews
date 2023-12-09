module NNews.Program

open Falco
open Falco.Routing
open Falco.HostBuilder
open Falco.Markup

let createLink (post : Database.Post) : XmlNode =
    match post.Url with
        | "" -> Elem.a [Attr.href $"/post/{post.PostId}"] [ Text.raw post.Description]
        | _ -> Elem.a [Attr.href post.Url] [ Text.raw post.Description]

let createPosts = 
    Database.getPagePosts
    |> List.map (fun post ->
            Elem.div [] [
                createLink post
            ])

let indexHandler : HttpHandler =
    let html =
        Elem.html [ Attr.lang "en" ] [
            Elem.head [] []
            Elem.body [] ([
                Elem.h1 [] [ Text.raw "N-News" ]
                Elem.a [Attr.href "/submit"] [ Text.raw "Submit post"]
            ] @ createPosts)
        ]

    Response.ofHtml html

let submitHandler : HttpHandler =
    let html =
        Elem.html [ Attr.lang "en" ] [
            Elem.head [] []
            Elem.body [] [
                Elem.h1 [] [Text.raw "Submit a post"]
                Elem.div [] [
                    Elem.span [] [Text.raw "URL"]
                    Elem.input [Attr.type' "text"]
                ]
                Elem.div [] [
                    Elem.span [] [Text.raw "Description"]
                    Elem.textarea [Attr.name "text"; Attr.rows "8"; Attr.cols "80"] []
                ]
            ]
        ]

    Response.ofHtml html

let postHandler : HttpHandler = fun ctx ->
    let route = Request.getRoute ctx
    let postId = route.GetInt "id"
    let post = Database.getSinglePost postId
    let html =
        Elem.html [ Attr.lang "en" ] [
            Elem.head [] []
            Elem.body [] [
                Elem.div [] [
                    Elem.a [Attr.href post.Url] [ Text.raw post.Description]
                    Elem.p [] [Text.raw post.BodyText]
                ]
            ]
        ]

    Response.ofHtml html ctx

[<EntryPoint>]
let main args =
    webHost args {
        endpoints [
            get "/" indexHandler
            get "/submit" submitHandler
            get "/post/{id:int}" postHandler
        ]
    }
    0

