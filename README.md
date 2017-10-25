Ham Approver
=========================

Ham Approver is a workflow item for Umbraco Forms, that helps you eliminate SPAM submissions without sacrifizing UX. It runs each submission against BlogSpam.net, to determine whether the submission is ham or spam.


## Installation

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**ZIP file**][GitHubRelease]  
Grab a ZIP file of the latest release; unzip and move the contents to the root directory of your web application.

## Features

- Tests the submission against BlogSpam.net, if the submission is OK (aka not spam), it will approve the submission.

[NuGetPackage]: https://www.nuget.org/packages/skttl.HamApprover
[GitHubRelease]: https://github.com/skybrud/skttl.HamApprover

## Configuration

Ham Approver needs some configuration to work.

### Submissions

These are settings regarding the submissions content.

**Comment fields**
This is the body text of your submission. Add the alias(es) of the field(s) to test for spam, seperated by comma. If no aliases added, the test will use a concatenation of all fields.

**Author name field**
If you have an author / sender / etc. field, you can add its alias here, and have the author name tested for spam.

**Email field**
If you have an email field, you can add its alias here, and have it tested for spam.

**Link field**
If you have an link field, you can add its alias here, and have it tested for spam.

**Subject field**
If you have a subject field, you can add its alias here, and have it tested for spam.

### Approver settings

These are settings for the approver.

**Server**
The server to test the submission against. Default is http://test.blogspam.net:9999/. You can host your own if you prefer.

**IP Blacklist**
Submissions from these IPs (seperated by comma) will always be denied.

**IP Whitelist**
Submissions from these IPs (seperated by comma) will always be approved. Note, if you're testing locally, your IP will be ::1, which is marked as spam at surbl.org. So you might want to whitelist that one.

**Max links**
The maximum number of links allowed in the submission. Default is 10

**Max length**
The maximum number of characters allowed in the submission. 0 means no limit.

**Min length**
The minimum number of characters required in the submission.

**Min words**
The minimum number of words required in the submission. Default is 4.

