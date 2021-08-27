## .NET development kit for prismic.io

[![Build Status](https://api.travis-ci.org/prismicio/dotnet-kit.png)](https://travis-ci.org/prismicio/dotnet-kit)

#### Warning: the official kit for C# developer is now the [csharp-kit](https://github.com/prismicio/csharp-kit).
Unless you code in F#, it is strongly recommended to migrate to the new kit.

### Getting started

#### Install the kit for your project

This kit can be installed using NuGet [prismic package](https://www.nuget.org/packages/prismic/).
To install prismic from the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console), run the following command :
PM> Install-Package prismic

*(Feel free to detail the proper steps for beginners by [submitting a pull request](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits).)*

#### Get started with prismic.io

You can find out [how to get started with prismic.io](https://developers.prismic.io/documentation/UjBaQsuvzdIHvE4D/getting-started) on our [prismic.io developer's portal](https://developers.prismic.io/).

#### Get started using the kit

Also on our [prismic.io developer's portal](https://developers.prismic.io/), on top of our full documentation, you will:
 * get a thorough introduction of [how to use prismic.io kits](https://developers.prismic.io/documentation/UjBe8bGIJ3EKtgBZ/api-documentation#kits-and-helpers), including this one.
 * see [what else is available for .NET](https://developers.prismic.io/technologies/UjBiDcuvzeMJvE4u/net): starter projects, examples, ...


#### Kit's detailed documentation

You can find the documentation of the .NET kit as comments and unit tests within the kit's source code.


Thanks to .NET languages' syntax and conventions, this kit contains some mild differences and syntastic sugar over the section of our documentation that tells you [how to use prismic.io kits](https://developers.prismic.io/documentation/UjBe8bGIJ3EKtgBZ/api-documentation#kits-and-helpers) in general (which you should read first).
This kit has been written in F# and can be used from any .NET language. For your convenience though, if you are developing in C#, a set of extension methods is provided in the prismic.extensions namespace (_prismic.extensions.dll_).

The differences are listed here (F#):
 * The get method of the API
``` ocaml
     Api.get
        (Infra.NoCache() :> Infra.ICache<Api.Response>)
        (Infra.Logger.NoLogger)
        (Some("your-token"))
        "https://lesbonneschoses.prismic.io/api")
```
 * Typical querying
``` ocaml
 api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.type, "docchapter")]]""").Submit()
```
* A shortcut is provided to get a DocumentLinkResolver through the For static method:
``` ocaml
	Api.DocumentLinkResolver.For(fun l ->
                        (sprintf "http://localhost/%s/%s" l.typ l.id))
```

The differences are listed here (C#):
 * Even in C#, you will have to manipulate some FSharp types such as FSharpOption. Do not be afraid if you do not know those types, they are standard types from the Base Class Library and fully documented on msdn.
 FSharpOption for example is a kind of wrapper around a value you are supposed to retrieve. Maybe your value is here, maybe it is not, hence you have to ask the wrapper if it has the value or not.
 The prismic Development Kit for .NET (in the prismic.extension namespace) provides extension methods to ease this: just check .Exists() on the option before using .Value to get the value out of it.

* This is the typical querying chain :
``` csharp
var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.type, ""docchapter"")]]")
	.Submit();
```
Here you will get an FSharpAsync type. If you do not want to deal with it, you can still fall back to a classic Task (from System.Threading.Tasks) by calling .SubmitableAsTask():
``` csharp
var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.type, ""docchapter"")]]")
	.SubmitableAsTask()
	.Submit();
```

### Changelog

Need to see what changed, or to upgrade your kit? We keep our changelog on [this repository's "Releases" tab](https://github.com/prismicio/dotnet-kit/releases).

### Contribute to the kit

Contribution is open to all developer levels, read our "[Contribute to the official kits](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits)" documentation to learn more.

#### Install the kit locally

This kit gets installed like any .NET library.

*(Feel free to detail the proper steps for beginners by [submitting a pull request](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits).)*

#### Test

Please write tests for any bugfix or new feature.

If you find existing code that is not optimally tested and wish to make it better, we really appreciate it; but you should document it on its own branch and its own pull request.

#### Documentation

Please document any bugfix or new feature.

If you find existing code that is not optimally documented and wish to make it better, we really appreciate it; but you should document it on its own branch and its own pull request.


#### Documentation

Please document any bugfix or new feature, using the [Yard](http://yardoc.org/) syntax. Don't worry about generating the doc, we'll take care of that.

If you find existing code that is not optimally documented and wish to make it better, we really appreciate it; but you should document it on its own branch and its own pull request.


### Contributing

We hope you'll get involved! Read our [Contributors' Guide](/CONTRIBUTING.md) for details.


### Licence

This software is licensed under [Apache 2.0 license](/LICENSE.md)

Copyright 2013-2021 Prismic <contact@prismic.io> (https://prismic.io)