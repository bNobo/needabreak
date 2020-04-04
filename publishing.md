# Process to publish a new release :



- update publish version on master
- commit and push
- checkout gh-pages
- rebase gh-pages onto master (or just cherry-pick last commit if there are no other modifications)
- publish in "publish" local folder
- push binaries stored in "publish" folder => now github pages is up-to-date, you can install needabreak from https://bnobo.github.io/needabreak/NeedABreak/publish/setup.exe
- make a ZIP with all files under "publish"
- create a new release on GitHub
- upload ZIP in the new release
- publish release => now you can install needabreak from the Releases page https://github.com/bNobo/needabreak/releases

# NB :

execute this command after "git clone" to prevent CRLF alteration on commit :

git config --local core.autocrlf false

if you don't do this the manifest will be considered corrupted by ClickOnce because the hash won't match the file.
