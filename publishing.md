# Process to publish a new release

- create a branch to prepare new version
- update package, assembly and file version
- update readme.md
- merge into root
- checkout gh-pages
- rebase gh-pages onto root
- choose "Release" configuration and publish in "publish" local folder
- add binaries stored in "publish" folder 
> use `git add -f` to force add new folder under "Application Files"
- push gh-pages => now github pages is up-to-date, you can install needabreak from https://bnobo.github.io/needabreak/NeedABreak/publish/setup.exe
- make a ZIP with all files under "publish"
- push root branch on remote
- create a new release on GitHub
- upload ZIP in the new release
- publish release => now you can install needabreak from the Releases page https://github.com/bNobo/needabreak/releases

# NB :

execute this command after "git clone" to prevent CRLF alteration on commit :

git config --local core.autocrlf false

if you don't do this the manifest will be considered corrupted by ClickOnce because the hash won't match the file.
