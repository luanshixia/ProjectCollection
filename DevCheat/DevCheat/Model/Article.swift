//
//  Article.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import Foundation

struct Article: Codable, Identifiable {
    let id: Int
    let title: String
    let fileName: String
    let description: String
    let publishedAt: Date
    
    let category: Category
    enum Category: String, Codable {
        case mobile
        case web
        case devops
        case dataScience
        case programming
        case utils
    }
    
    let isFavorite: Bool
    var viewedAt: Date?
}
