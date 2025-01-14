//
//  Article.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import Foundation

struct Article: Codable {
    let title: String
    let url: String
    let description: String
    let publishedAt: Date
    
    var category: Category
    enum Category: String, Codable {
        case swift
        case iOS
        case macOS
        case web
        case machineLearning
        case dataScience
        case programming
        case coding
        case codingTips
    }
    
    var isFavorite: Bool
}
