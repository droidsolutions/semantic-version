# Contribution Guidelines

These are guidelines to help you if you want to contribute to this project. Below are some guidelines to follow that help keep the project in the same style.

## Commit

This project uses [Commit Lint](https://commitlint.js.org/#/) to enforce a consistent commit message style. Every commit message must begin with a subject like fix, feat, chore, etc and an optional scope in brackets, followed by a colon and the commit message. This message tyle is needed for automatic release (see below).

A typical commit could look like

 ```bash
 git commit -m "feat: add fancy new feature" -m "closes #1 and introduces special feature capability"
 ```
 or 
 ```bash 
 git commit -m "ci(github): configure github actions"
 ```
 or
 ```bash 
 git commit -m "refactor(MyFeature): rework my feature" -m "BREAKING CHANGE: DoSomething() now needs a string argument to work"
 ```
 
 You can set up a Git hook that helps you enforcing a correct commit message via [Husky](https://github.com/typicode/husky) (you'll need [NodeJS](https://nodejs.org/en/) for this). Just run `npx husky install` and the commit hook will be set up for you.
 
 ## Style
 
 This project uses [Editorconfig](https://editorconfig.org/) to help maintain a consistent code style. Make sure your editor adheres the editorconfig. If you use Visual Studio it should already support editorconfig out of the box. For Visual Studio Code you can install [an extension](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig) to help you.
 
 To enforce some best practices this project uses [Roslynator](https://github.com/JosefPihrt/Roslynator) which will generate some build warnings on bad code. As another tool this repo uses [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers).
 
 The `.editorconfig` file contains rules for the project, like indentation and code style.
 
 ## Release
 
 Release of the project is automated via GitHub actions and [semantic-release](https://github.com/semantic-release/semantic-release). This tool runs in CI and automatically determines the next vewrsion based on the commit messages since the last release. It then updates the necessary files, creates a NuGet package und pushes it to [nuget.org](https://nuget.org). It will also generate a tag and comment on pull requests that are included in the release.