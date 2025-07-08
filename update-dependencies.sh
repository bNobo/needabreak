#!/bin/bash

set -e  # Stop on error

# 💡 Config personnalisable
REPO_URL="https://github.com/bNobo/needabreak.git"
BRANCH_NAME="Bump/packages-$(date +%Y%m%d)"
WORKDIR="."
BASE_BRANCH="root"

# 👉 Déplace-toi dans un dossier de travail
mkdir -p "$WORKDIR"
cd "$WORKDIR"

# 🧠 Clone ou update
if [ ! -d ".git" ]; then
  git clone "$REPO_URL" .
else
  git reset --hard
  git clean -fd
  git checkout "$BASE_BRANCH"
  git pull origin "$BASE_BRANCH"
fi

# 🔀 Crée une nouvelle branche
git checkout -b "$BRANCH_NAME"

# ✅ Exécute dotnet outdated
dotnet outdated -u --include-auto-references NeedABreak.Updater.sln

# 📦 Vérifie s’il y a eu des changements
if git diff --quiet; then
  echo "✅ Aucun paquet à mettre à jour, fin du script."
  exit 0
fi

# ✅ Commit et push
git add .
git commit -m "Bump: update NuGet packages"
git push origin "$BRANCH_NAME"

# 🔁 Crée une PR avec GitHub CLI
gh pr create \
  --title "Bump: update NuGet packages" \
  --body "Automated update of NuGet packages via dotnet-outdated." \
  --head "$BRANCH_NAME" \
  --base "$BASE_BRANCH"
