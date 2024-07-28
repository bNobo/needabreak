# Publish to Microsoft Store

1. Create a branch `Release/<version>` to prepare the release.
1. Increase the app version in package manifest accordingly to semver.
1. Publish a version locally for validation.
1. Update the version number in csproj to match app package version.
1. Update documentation as needed.
1. Create a PR to root branch.
1. Submit the built package to https://partner.microsoft.com/