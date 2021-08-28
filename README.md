# .NET Standard development kit for prismic.io

## Build Status

**Release** 
![Build Status](https://img.shields.io/azure-devops/build/adaptive-webworks/prismic/13/master)
![Code coverage](https://img.shields.io/azure-devops/coverage/adaptive-webworks/prismic/13/master)

**Alpha** 
![Build Status](https://img.shields.io/azure-devops/build/adaptive-webworks/prismic/14/dev)
![Code coverage](https://img.shields.io/azure-devops/coverage/adaptive-webworks/prismic/14/dev)

## Getting started

### Install the kit for your project

This kit can be installed using NuGet [prismic package](https://www.nuget.org/packages/prismicio.netstandard).
For the latest stable build run the following shell command in your project folder:

```shell
dotnet package add prismicio.netstandard
```

*(Feel free to detail the proper steps for beginners by [submitting a pull request](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits).)*

### Get started with prismic.io

You can find out [how to get started with prismic.io](https://developers.prismic.io/documentation/UjBaQsuvzdIHvE4D/getting-started) on our [prismic.io developer's portal](https://developers.prismic.io/).

### Get started using the kit

Also on our [prismic.io developer's portal](https://developers.prismic.io/), on top of our full documentation, you will:
* get a thorough introduction of [how to use prismic.io kits](https://developers.prismic.io/documentation/UjBe8bGIJ3EKtgBZ/api-documentation#kits-and-helpers), including this one.

### Migration from the old prismicio kit

Until October 2019, the prismic kit was only catered to .NET Framework. The new package has some breaking changes, primarily around API instantiation. Look at the sample project to see how to get an `Api`.
Although every effort has been made to make the kit familiar new kit follows a more idiomatic C# coding style. When migrating a large code base to the new library, this gist might get your project building a little quicker.
https://gist.github.com/benembery/f83800262f9713d01324273d2a7f1a12

## Changelog
[ReleaseNodes.md](https://github.com/prismicio/csharp-kit/blob/master/ReleaseNotes.md).

## Contribute to the kit

Contribution is open to all developer levels, read our "[Contribute to the official kits](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits)" documentation to learn more.

### Install the kit locally

This kit gets installed like any .NET library.

*(Feel free to detail the proper steps for beginners by [submitting a pull request](https://developers.prismic.io/documentation/UszOeAEAANUlwFpp/contribute-to-the-official-kits).)*

### Test

Please write tests for any bugfix or new feature.

If you find existing code that is not optimally tested and wish to make it better, we really appreciate it; but you should document it on its own branch and its own pull request.

### Documentation

Please document any bugfix or new feature.

If you find existing code that is not optimally documented and wish to make it better, we really appreciate it; but you should document it on its own branch and its own pull request.

### Contributing

We hope you'll get involved! Read our [Contributors' Guide](/CONTRIBUTING.md) for details.

### Licence

This software is licensed under [Apache 2.0 license](/LICENSE.md)

Copyright 2013-2021 Prismic <contact@prismic.io> (https://prismic.io)