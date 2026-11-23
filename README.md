# pscm-youtrack-plugin
====================

A UVCS issue-tracker extension that maps branches to YouTrack issues and allows branches to be created from tickets.

* [You Track API documentation](https://confluence.jetbrains.com/display/YTD4/YouTrack+REST+API+Reference)
* [Plastic SCM Custom Extensions documentation](https://www.plasticscm.com/documentation/extensions/plastic-scm-version-control-task-and-issue-tracking-guide.shtml#WritingPlasticSCMcustomextensions)


## Features
* Create branches from YouTrack tickets.
* Place ticket in-progress on create branch.
* Log operation comments to ticket (create branch, commits, comments w/ commits)
* Branch explorer integration shows ticket title, state, and quick 1-click access to ticket page in YouTrack.

The extension targets .NET Standard 2.0 and was validated against UVCS 11.0.16.10694 (the .NET 7 client). By default,
the build looks for `issuetrackerinterface.dll` in `C:\Program Files\PlasticSCM5\client`. Override
`UVCSClientDirectory` when the client is installed elsewhere. If the client API assembly is unavailable, the build
falls back to the reference assembly under `lib\pscm`.

## How to install

1. Build the extension in Release configuration:
   `dotnet publish src\EVS.PlasticExtensions.YouTrackPlugin\EVS.PlasticExtensions.YouTrackPlugin.csproj -c Release -o publish\youtrack`
2. Copy **all files** from `publish\youtrack` to `C:\Program Files\PlasticSCM5\client\extensions\youtrack\`.
   The loader convention is `*extension.dll`; this package uses `youtrackextension.dll`.
   The assembly's `.deps.json` and the copied runtime dependencies are required alongside the extension DLL.
   Publishing adds the installed UVCS issue-tracker API to the dependency manifest without copying that API DLL.
3. Add this line to `C:\Program Files\PlasticSCM5\client\customextensions.conf`:
   `YouTrack=extensions/youtrack/youtrackextension.dll`
   Keep any existing lines in that file. UVCS uses this registry to discover custom extensions.
4. Restart UVCS, then configure the YouTrack host and permanent token in the issue-tracker settings. Create a
   permanent token with the **YouTrack** scope (the extension does not require **YouTrack Administration**) and
   enter the full token value, including its `perm:` prefix.

The branch prefix defaults to `yt_` for compatibility, but can be cleared to use the issue ID directly as the
last branch-name segment (for example, `feature/SOR-123`). With a blank prefix, branches without valid YouTrack
issue IDs will not resolve to tasks. The Plastic WebGUI root URL is optional; when blank, check-in comments are
still posted to YouTrack but omit the changeset hyperlink. Enable or disable these comments with
**Add check-in comments to ticket(s)**.

## How to update existing installation
Use the installation steps above after building against the UVCS client version that will load the extension.