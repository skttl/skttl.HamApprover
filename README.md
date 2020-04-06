Ham Approver
=========================

Ham Approver is a workflow item for Umbraco Forms, that helps you eliminate SPAM submissions without sacrifizing UX. It runs each submission against [Plino](https://plino.herokuapp.com/), to determine whether the submission is ham or spam.

## Minimum requirements
Ham Approver is built against Umbraco CMS 8.4 and Umbraco Forms 8.3 and supports those versions and above. For Umbraco 7, please see version 1.0 on [NuGet](https://www.nuget.org/packages/skttl.HamApprover/1.0.0), or [Our Umbraco][OurUmbracoRelease]


## Installation

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**Umbraco Package**][OurUmbracoRelease]  
Download the package from the our.umbraco.com package repository and install using Umbraco.

## Features

- Tests the submission against [Plino](https://plino.herokuapp.com/), if the submission is OK (aka not spam), it will approve the submission.

[NuGetPackage]: https://www.nuget.org/packages/skttl.HamApprover
[OurUmbracoRelease]: https://our.umbraco.com/packages/website-utilities/ham-approver-for-umbraco-forms/

## Configuration

Ham Approver needs some configuration to work.

### Submissions

These are settings regarding the submissions content.

**Comment fields**

This is the body text of your submission. Add the alias(es) of the field(s) to test for spam, seperated by comma. If no aliases added, the test will use a concatenation of all fields.

If you have fields with sensitive information, you should probably not send them to the third party API.

