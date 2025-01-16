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
        .init(id: 3, title: "ES6 Cheatsheet", fileName: "es6", description: "JavaScript", publishedAt: Date(), category: .web, isFavorite: false),
    ]
    
    var featured: [Int] = [1, 2, 3]
    
    var web: [Int] = [1, 2, 3]
    
    func filterArticles(by category: [Int]) -> [Article] {
        articles.filter({ category.contains($0.id) })
    }
}
