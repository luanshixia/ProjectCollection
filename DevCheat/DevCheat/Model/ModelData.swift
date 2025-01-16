//
//  ModelData.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import Foundation

@Observable
class ModelData {
    var articles: [Article] = [
        // generate some sample data
        .init(id: 1, title: "HTML Cheatsheet", fileName: "html", description: "HTML", publishedAt: Date(), category: .web, isFavorite: true),
        .init(id: 2, title: "CSS Cheatsheet", fileName: "css", description: "CSS", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 3, title: "ES2015+ Cheatsheet", fileName: "es6", description: "JavaScript", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 4, title: "AngularJS Cheatsheet", fileName: "angularjs", description: "AngularJS", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 5, title: "AWS CLI Cheatsheet", fileName: "awscli", description: "AWS CLI", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 6, title: "Bash Cheatsheet", fileName: "bash", description: "Bash", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 7, title: "Canvas Cheatsheet", fileName: "canvas", description: "Canvas", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 8, title: "Cron Cheatsheet", fileName: "cron", description: "Cron", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 9, title: "CSS Flexbox Cheatsheet", fileName: "css-flexbox", description: "Flexbox", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 10, title: "CSS Grid Cheatsheet", fileName: "css-grid", description: "Grid", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 11, title: "CSS Tricks Cheatsheet", fileName: "css-tricks", description: "CSS Tricks", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 12, title: "fetch() Cheatsheet", fileName: "fetch", description: "fetch()", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 13, title: "find Cheatsheet", fileName: "find", description: "find", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 14, title: "Git Branches Cheatsheet", fileName: "git-branches", description: "Git Branches", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 15, title: "Git Tricks Cheatsheet", fileName: "git-tricks", description: "Git Tricks", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 16, title: "Go Cheatsheet", fileName: "go", description: "Go", publishedAt: Date(), category: .programming, isFavorite: false),
        .init(id: 17, title: "GraphQL Cheatsheet", fileName: "graphql", description: "GraphQL", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 18, title: "grep Cheatsheet", fileName: "grep", description: "grep", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 19, title: "JS Array Cheatsheet", fileName: "js-array", description: "JS Array", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 20, title: "JS Date Cheatsheet", fileName: "js-date", description: "JS Date", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 21, title: "Linux Cheatsheet", fileName: "linux", description: "Linux", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 22, title: "MySQL Cheatsheet", fileName: "mysql", description: "MySQL", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 23, title: "Node.js Cheatsheet", fileName: "nodejs", description: "Node.js", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 24, title: "npm Cheatsheet", fileName: "npm", description: "npm", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 25, title: "Promise Cheatsheet", fileName: "promise", description: "Promise", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 26, title: "Python Cheatsheet", fileName: "python", description: "Python", publishedAt: Date(), category: .programming, isFavorite: false),
        .init(id: 27, title: "React Cheatsheet", fileName: "react", description: "React", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 28, title: "React-router Cheatsheet", fileName: "react-router", description: "React-router", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 29, title: "Redux Cheatsheet", fileName: "redux", description: "Redux", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 30, title: "Regexp Cheatsheet", fileName: "regexp", description: "Regexp", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 31, title: "REST Cheatsheet", fileName: "rest", description: "REST", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 32, title: "Sass Cheatsheet", fileName: "sass", description: "Sass", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 33, title: "sed Cheatsheet", fileName: "sed", description: "sed", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 34, title: "tar Cheatsheet", fileName: "tar", description: "tar", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 35, title: "tmux Cheatsheet", fileName: "tmux", description: "tmux", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 36, title: "TypeScript Cheatsheet", fileName: "typescript", description: "TypeScript", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 37, title: "Unicode Symbols Cheatsheet", fileName: "unicode-symbols", description: "Unicode Symbols", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 38, title: "Vim Cheatsheet", fileName: "vim", description: "Vim", publishedAt: Date(), category: .devops, isFavorite: false),
        .init(id: 39, title: "VS Code Cheatsheet", fileName: "vscode", description: "VS Code", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 40, title: "Vue Cheatsheet", fileName: "vue", description: "Vue", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 41, title: "Web Workers Cheatsheet", fileName: "web-workers", description: "Web Workers", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 42, title: "XPath Cheatsheet", fileName: "xpath", description: "XPath", publishedAt: Date(), category: .utils, isFavorite: false),
        .init(id: 43, title: "zsh Cheatsheet", fileName: "zsh", description: "zsh", publishedAt: Date(), category: .devops, isFavorite: false),
    ]
    
    var featured: [Int] = [1, 2, 3, 26]
    
    var web: [Int] = [1, 2, 3, 4, 7, 9, 10, 11, 12, 19, 20, 23, 24, 25, 27, 28, 29, 32, 36, 40, 41]
    
    var commandLine: [Int] = [6, 8, 13, 18, 21, 33, 34, 35, 38, 43]
    
    var others: [Int] = [5, 14, 15, 16, 17, 22, 26, 30, 31, 37, 39, 42]
    
    func filterArticles(by category: [Int]) -> [Article] {
        articles.filter({ category.contains($0.id) })
    }
}
